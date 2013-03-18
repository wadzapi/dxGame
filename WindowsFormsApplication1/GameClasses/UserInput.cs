using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.DirectX;
using Microsoft.DirectX.DirectInput;


namespace MarsDX
{
    public struct MouseParameters
    {
        public int dx;
        public int dy;
        public int wheel;
        public bool lButton;
        public bool rButton;
    }

    #region Классы обработки нажатия клавиш
    public class SingleKey
    {
        private Key _key;
        protected bool lastKeyState = false;
        protected bool wasUp = true;

        public Key key
        {
            get
            {
                return _key;
            }
        }

        public SingleKey(Key k)
        {
            this._key = k;
        }

        public SingleKey(){}

        public virtual bool Listen(bool nowState, double paintTime)
        {
            bool execute = false;
            if (nowState)
            {
                if (wasUp)
                {
                    wasUp = false;
                    execute = true;
                }
            }
            else
            {
                if (lastKeyState)
                {
                    wasUp = true;
                }
            }
            lastKeyState = nowState;
            return execute;
        }
    }

    public class MultipleKey: SingleKey
    {
        private double Delay;
        private double timer = 0.0d;

        public MultipleKey(Key key, double delay): base(key)
        {
            this.Delay = delay;
        }

        public MultipleKey() { }

        public override bool Listen(bool nowState, double paintTime)
        {
            timer += paintTime;
            bool execute = false;
            if (nowState)
            {
                if (wasUp)
                {
                    wasUp = false;
                    timer = 0.0d;
                    execute = true;
                }
                else
                {
                    if (timer >= Delay)
                    {
                        timer = 0.0d;
                        execute = true;
                    }
                }
            }
            else
            {
                if (lastKeyState)
                {
                    wasUp = true;
                }
            }
            lastKeyState = nowState;
            return execute;
        }
    }
    
    public class UpDownKey:SingleKey
    {
        public UpDownKey(Key key)
            : base(key)
        {
        }
        public UpDownKey() { }


        public override bool Listen(bool nowState, double paintTime)
        {
            bool execute = false;
            if (nowState)
            {
                if (wasUp)
                {
                    wasUp = false;
                    execute = true;
                }
            }
            else
            {
                if (lastKeyState)
                {
                    wasUp = true;
                    execute = true;
                }
            }
            lastKeyState = nowState;
            return execute;

        }
    }

    #endregion

    #region Классы устройств ввода
    public class MouseBuff
    {
        public MouseParameters mouseParams = new MouseParameters();

        public MouseBuff() { }

        public void Clear()
        {
            mouseParams.dy += mouseParams.dy;
            mouseParams.dx = mouseParams.dy = mouseParams.wheel = 0;
            mouseParams.lButton = mouseParams.rButton = false;
        }
    }

    public class KeyboardBuff
    {
        public KeyboardState keyParams;

        public KeyboardBuff() { }
    }

    public class UserInput
    {
        private Device mouse, keyboard;
        public MouseBuff mouseData = new MouseBuff();
        public KeyboardBuff keyboardData = new KeyboardBuff();
        private MouseState CurrMouseState;

        public UserInput(IntPtr handle)
        {
            CreateKeyboard(handle, true);
            CreateMouse(handle, true);
        }

        #region Мышь
        private void CreateMouse(IntPtr Handle, bool isExclusive)
        {
            try
            {
                mouse = new Device(SystemGuid.Mouse);
            }
            catch (InputException)
            {
                throw new InputException("Устройство ввода мышь не может быть создано");
            }
            try
            {
                if (isExclusive)
                {
                    mouse.SetCooperativeLevel(Handle, CooperativeLevelFlags.Exclusive | CooperativeLevelFlags.Foreground);
                }
                else
                {
                    mouse.SetCooperativeLevel(Handle, CooperativeLevelFlags.NonExclusive | CooperativeLevelFlags.Foreground);
                }
            }
            catch (InputException)
            {
                throw new InputException("Ошибка при выполнении SetCooperativeLevel");
            }
            try
            {
                mouse.SetDataFormat(DeviceDataFormat.Mouse);
            }
            catch (Exception)
            {
                throw new Exception("Ошибка формата устройства ввода - мыши");
            }
        }//Создание мыши
        
        private void AcquireMouse()
        {
            if (mouse != null)
            {
                try { mouse.Acquire(); }
                catch (DirectXException) { }
            }
        }//Привязка мыши
        

        private void InitMouse(IntPtr Handle, bool Exclusive)
        {
            CreateMouse(Handle, Exclusive);
            AcquireMouse();
        }//Иницализация мыши = создание + привязка
        
        private void GetMouseState()
        {
            if (mouse == null)
                throw new InvalidOperationException("Устройство мышь не существует");
            try
            {
                mouse.Poll();
            }
            catch (DirectXException ex)
            {
                if ((ex is NotAcquiredException) || (ex is InputLostException))
                {
                    bool loop = false;
                    do
                    {
                        try
                        {
                            mouse.Acquire();
                            loop = false;
                        }
                        catch (InputLostException)
                        {
                            loop = true;
                        }
                        catch (InputException)
                        {
                            loop = false;
                        }
                    } while (loop);
                }
            }
            try
            {
                CurrMouseState = mouse.CurrentMouseState;
            }
            catch (DirectXException) { }
        }//Метод получение текущего состояния мыши
        
        private void Mouse2Input()
        {
            mouseData.Clear();
            mouseData.mouseParams.dx = CurrMouseState.X;
            mouseData.mouseParams.dy = CurrMouseState.Y;
            mouseData.mouseParams.wheel = CurrMouseState.Z;
            byte[] buttons = CurrMouseState.GetMouseButtons();
            if (buttons != null)
            {
                if (buttons[0] != 0)
                    mouseData.mouseParams.lButton = true;
                if (buttons[1] != 0)
                    mouseData.mouseParams.rButton = true;
            }
        }//Метод сохранения текущих параметров мыши внутри класса MouseData
        
        private void UpdateMouse()
        {
            if (mouse != null)
            {
                GetMouseState();
                Mouse2Input();
            }
        }//Обновление состояния клавиатуры

        private void DisposeMouse()
        {
            if (mouse != null)
            {
                mouse.Unacquire();
                mouse.Dispose();
            }
        }//Ликвидация мыши

        public void ChangeMouse(IntPtr Handle, bool isExclusive)
        {
            DisposeMouse();
            InitMouse(Handle, isExclusive);
        }//Пересоздание мыши с новыми параметрами эксклюзивности

        #endregion

        #region Клавиатура
        private void CreateKeyboard(IntPtr Handle, bool isExclusive)
        {
            try
            {
                keyboard = new Device(SystemGuid.Keyboard);
            }
            catch (InputException)
            {
                throw new InputException("Устройство ввода клавиатура не может быть создано");
            }
            try
            {
                if (isExclusive)
                {
                    keyboard.SetCooperativeLevel(Handle, CooperativeLevelFlags.Exclusive | CooperativeLevelFlags.Foreground);
                }
                else
                {
                    keyboard.SetCooperativeLevel(Handle, CooperativeLevelFlags.NonExclusive | CooperativeLevelFlags.Foreground);
                }
            }
            catch (InputException)
            {
                throw new InputException("Ошибка при выполнении SetCooperativeLevel");
            }
            try
            {
                keyboard.SetDataFormat(DeviceDataFormat.Keyboard);
            }
            catch(Exception)
            {
                throw new Exception("Ошибка формата устройства ввода - клавиатуры");
            }
        }//Создание клавиатуры
        
        private void AcquireKeyboard()
        {
            if (keyboard != null)
            {
                try { keyboard.Acquire(); }
                catch (DirectXException){}
            }
        }//Привязка клавиатуры

        private void InitKeyboard(IntPtr Handle, bool Exclusive)
        {
            CreateKeyboard(Handle, Exclusive);
            AcquireKeyboard();
        }//Инициализация клавиатуры = создание + привязка

        private void GetKeyboardState()
        {
            if (keyboard == null)
                throw new InvalidOperationException("Устройство ввода клавиатура не существует");
            try
            {
                keyboardData.keyParams = keyboard.GetCurrentKeyboardState();
            }
            catch (DirectXException ex)
            {
                if ((ex is NotAcquiredException) || (ex is InputLostException))
                {
                    bool loop = false;
                    do
                    {
                        try
                        {
                            keyboard.Acquire();
                            loop = false;
                        }
                        catch (InputLostException)
                        {
                            loop = true;
                        }
                        catch (InputException)
                        {
                            loop = false;
                        }
                    } while (loop);
                }
            }
            try
            {
                keyboardData.keyParams = keyboard.GetCurrentKeyboardState();
            }
            catch (DirectXException) { }
        }//Метод получения текущего состояния клавиатуры

        private void DisposeKeyboard()
        {
            if (keyboard != null)
            {
                keyboard.Unacquire();
                keyboard.Dispose();
            }
        }//Ликвидация клавиатуры

        public void ChangeKeyboard(IntPtr Handle, bool isExclusive)//Пересоздание клавиатуры с новыми параметрами эксклюзивности
        {
            DisposeKeyboard();
            InitKeyboard(Handle, isExclusive);
        }
        #endregion

        #region Устройства ввода = Мышь + клавиатура
        
        public void UpdateInput()
        {
            UpdateMouse();
            GetKeyboardState();
        }//Обновление состояний устройств ввода
        
        public void Dispose()
        {
            DisposeMouse();
            DisposeKeyboard();
        } //Метод удаления устройств ввода
        
        #endregion
    }
    
    #endregion
}
