using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace MarsDX
{
    partial class Game
    {
        public struct State
        {
            public bool isDeviceLost; //потери устройства
            public bool isActive; //фокуса
            public bool isRenderPause; //паузы рендеринга
            public bool isWindowed; //оконный/полноэкранный режим
            public bool isFromFullScr; //из полноэкранного режима
        }//Индикаторы состояний 3D устройства

        #region Константы

        private const int ScreenWidth = 1024; //Параметры экрана
        private const int ScreenHeight = 768; //
        private const float TexSize = 2048.0f; //Размер стороны основной текстуры
        private const string resPath = "MarsDX.Res.";
        #endregion

        #region Масссивы вершин и индексов кораблей
        
        private static CustomVertex.PositionOnly[] origPos = // Набор вершин корабля в координатах модели
        {
            new CustomVertex.PositionOnly(0.0f, 1.0f, 0.0f),
            new CustomVertex.PositionOnly(1.0f, 1.0f, 0.0f),
            new CustomVertex.PositionOnly(1.0f, 0.0f, 0.0f),
            new CustomVertex.PositionOnly(0.0f, 0.0f, 0.0f)
        };

        private static GeometryData[] geomPacket; //набор вершин корабля (геометрия)

        private static readonly short[] origIndices = //Индексы корабля
        {
            0,1,3, //Верхний левый
            1,2,3 //Нижний правый треугольник
        };

        #endregion

        #region Буферы

        private VertexBuffer GeometryVertexBuff; //VB, хранящий геометрию
        private VertexBuffer InstancingVertexBuff;//VB, содержащий параметры "клонов"
        private IndexBuffer GeometryIndexBuff; //Индексный буфер
        private VertexDeclaration HWInstancingDecl, DBInstancingDecl; //VertexDeclaration для HW и DB Instancing

        #endregion

        #region Device 3d и его параметры

        private Device device3D;

        private static bool SupportVS3; //Индикатор поддержки vertex shader 3.0

        private PresentParameters presParams;

        //Экземпляр индикаторов сотояния
        public State state = new State() { isActive = true, isRenderPause = false, isDeviceLost = false, isWindowed = true, isFromFullScr = false };
        
        #endregion

        #region Текструры, поверхности, эффекты

        Texture tex = null; //Основня текстура
        Texture background = null;
        private Effect effect;
        
        #endregion

        #region Матрицы вида, проекции, их методы
        private static Vector3 eyePos = new Vector3();
        private static Matrix viewMatr; //Матрицы вида

        private static Matrix projMatrix = Matrix.PerspectiveFovLH((float)Math.PI/3.0f, 4.0f/3, 47.0f, 250.0f);//Матрица проекции
        private static Matrix ViewProjMatr;//Перемноженная Вида на Проекции
        
        private void SetViewProjMatrix()
        {
            viewMatr = Matrix.LookAtLH(eyePos, new Vector3(eyePos.X, eyePos.Y, 1.0f), new Vector3(0.0f, 1.0f, 0.0f));
            ViewProjMatr = viewMatr * projMatrix;
        }

        #endregion

        #region Методы инициализации устройства

        public bool InitializeGraphics() //Инициализация 3D устройства
        {
            int adapterOrder = Manager.Adapters.Default.Adapter;
            CreatePresentParams(adapterOrder);
            bool CanDoShader = CreateDevice3D(adapterOrder);
            GameStart(); //Процесса игры
            return CanDoShader;
        }

        private void CreatePresentParams(int defaultAdapter) //Создание PresentParameters
        {
            presParams = new PresentParameters();
            presParams.PresentationInterval = PresentInterval.Immediate;
            presParams.EnableAutoDepthStencil = true;            //Создание z буффера
            presParams.AutoDepthStencilFormat = DepthFormat.D16;// формат z буффера
            presParams.SwapEffect = SwapEffect.Discard; //Настройка своппинга

            Format currFormat = Manager.Adapters[defaultAdapter].CurrentDisplayMode.Format;
            presParams.BackBufferFormat = currFormat;
            presParams.BackBufferCount = 1;
            presParams.MultiSample = MultiSampleType.None;

            if (!state.isWindowed && Manager.CheckDeviceType(defaultAdapter, DeviceType.Hardware, currFormat, currFormat, false)) //Настройки полоноэкранного режима
            {
                presParams.Windowed = false;
                presParams.BackBufferWidth = ScreenWidth;
                presParams.BackBufferHeight = ScreenHeight;
            }
            else
            {
                presParams.Windowed = true;
                presParams.BackBufferWidth = this.Width;
                presParams.BackBufferHeight = this.Height;
            }
        }
        
        private bool CreateDevice3D(int defaultAdapter) //Создание Device3D
        {
            CreateFlags flags = CreateFlags.SoftwareVertexProcessing;             //Параметры обработки вершин
            Caps caps = Manager.GetDeviceCaps(defaultAdapter, DeviceType.Hardware);
            bool CanDoShader = true;
            if (caps.VertexShaderVersion >= new Version(1, 1) && caps.PixelShaderVersion >= new Version(2, 0))
            {
                if (caps.VertexShaderVersion >= new Version(3, 0))
                    SupportVS3 = true;
                if (caps.DeviceCaps.SupportsHardwareTransformAndLight)
                    flags = CreateFlags.HardwareVertexProcessing;
                if (caps.DeviceCaps.SupportsPureDevice)
                    flags |= CreateFlags.PureDevice;
                device3D = new Device(defaultAdapter, DeviceType.Hardware, this.Handle, flags, presParams); //Содание устройства
            }
            else
            {
                CanDoShader = false;
                device3D = new Device(defaultAdapter, DeviceType.Reference, this.Handle, flags, presParams);
            }
            device3D.DeviceReset += new EventHandler(device3D_DeviceReset);
            device3D_DeviceReset(device3D, null);
            device3D.DeviceLost += new EventHandler(device3D_DeviceLost);
            device3D.Disposing += new EventHandler(device3D_Disposing);
            return CanDoShader;
        }
        
        #endregion

        #region Загрузка ресурсов

        private void LoadTextureFromStream(out Texture tex, string TexName) //Метод для загрузки текстуры
        {
            try
            {
                tex = TextureLoader.FromStream(device3D, this.GetType().Assembly.GetManifestResourceStream(resPath + TexName));
            }
            catch (InvalidCallException)
            {
                throw new InvalidCallException(string.Format("Ошибка при загрузке текстуры {0}", TexName));
            }
            catch (OutOfMemoryException)
            {
                throw new OutOfMemoryException(string.Format("Недостаточно памяти для загрузки текстуры {0}", TexName));
            }
        }
        #endregion

        #region Методы загрузки эффекта, задания его параметров

        private void LoadEffectFromStream(string EffectName)
        {
            try
            {
                effect = Effect.FromStream(device3D, this.GetType().Assembly.GetManifestResourceStream(resPath + EffectName), null, null, ShaderFlags.NotCloneable, null);
            }
            catch (InvalidDataException)
            {
                throw new InvalidDataException(string.Format("Ресурс, содержащий {0}, не найден или поврежден", EffectName));
            }
            catch (DirectXException)
            {
                throw new DirectXException(string.Format("Ошибка при компиляции эффекта {0}", EffectName));
            }
        }
        private void LoadEffectValue(string TexName, Texture t)
        {
            try
            {
                effect.SetValue(TexName, t);
            }
            catch (InvalidCallException)
            {
                throw new InvalidCallException(string.Format("Ошибка при передаче в эффект текстуры {0}", TexName));
            }
        }
        private void LoadEffectValue(string MatrixName, Matrix m)
        {
            try
            {
                effect.SetValue(MatrixName, m);
            }
            catch (InvalidCallException)
            {
                throw new InvalidCallException(string.Format("Ошибка при передаче в эффект матрицы {0}", MatrixName));
            }
        }
        private void ValidateEffectTechnique(string TecniqueName)
        {
            try
            {
                effect.ValidateTechnique(TecniqueName);
                effect.Technique = TecniqueName;
            }
            catch (InvalidCallException)
            {
                throw new InvalidCallException(string.Format("Техника с именем {0} не найдена", TecniqueName));
            }
            catch (DirectXException)
            {
                throw new DirectXException(string.Format("Техника {0} превышает возможности GPU", TecniqueName));
            }
        }
        
        #endregion

        #region Создание, заполнение буферов

        private void CreateBuff() //Метод для создания буферов вершин и индексов
        {
            if (SupportVS3)
            {
                GeometryIndexBuff = new IndexBuffer(typeof(short), origIndices.Length, device3D, Usage.WriteOnly, Pool.Default);
                GeometryIndexBuff.SetData(origIndices, 0, LockFlags.None);

                GeometryVertexBuff = new VertexBuffer(typeof(GeometryData), origPos.Length, device3D, Usage.WriteOnly, GeometryData.Format, Pool.Default);
                geomPacket = new GeometryData[4];
                for (int i = 0; i < origPos.Length; ++i)
                {
                    geomPacket[i] = new GeometryData(origPos[i], i);
                }
                GeometryVertexBuff.SetData(geomPacket, 0, LockFlags.None);

                VertexElement[] InstElement = InstanceData.Declaration;
                HWInstancingDecl = new VertexDeclaration(device3D, InstElement);
                InstancingVertexBuff = new VertexBuffer(typeof(InstanceData), maxShips, device3D, Usage.WriteOnly | Usage.Dynamic, InstanceData.Format, Pool.Default);
            }
            else
            {
                VertexElement[] elem = new VertexElement[]
                {
                    new VertexElement(0, 0, DeclarationType.Float3, DeclarationMethod.Default, DeclarationUsage.Position, 0),
                    new VertexElement(0, 12, DeclarationType.Float3, DeclarationMethod.Default, DeclarationUsage.TextureCoordinate, 0),
                    VertexElement.VertexDeclarationEnd
                };
                DBInstancingDecl = new VertexDeclaration(device3D, elem);
                GeometryIndexBuff = new IndexBuffer(typeof(short), origIndices.Length * maxShips, device3D, Usage.WriteOnly | Usage.Dynamic, Pool.Default);
                InstancingVertexBuff = new VertexBuffer(typeof(CustomVertex.PositionTextured), origPos.Length * maxShips, device3D, Usage.WriteOnly | Usage.Dynamic, CustomVertex.PositionTextured.Format, Pool.Default);
            }
        }

        private void CreateHWInstanceData() //Метод для заполнения значениями параметров "клонов" техники HWInstancing и установка буфера вершин для них
        {
            InstanceData[] instData = new InstanceData[mySpaceShips.Count + 2];
            if (mySpaceShips.Count != 0)
            {
                for (int shipIndex = 0; shipIndex < mySpaceShips.Count; ++shipIndex)
                {
                    instData[shipIndex] = new InstanceData(mySpaceShips[shipIndex].getTranlsMatr(), mySpaceShips[shipIndex].getUWVH());
                }
            }
            instData[mySpaceShips.Count] = new InstanceData(GameHero.Crosshair.TransformMatr, GameHero.Crosshair.UVWH);
            instData[mySpaceShips.Count + 1] = new InstanceData(GameHero.TransformMatr, GameHero.gun.UVWH);
            InstancingVertexBuff.SetData(instData, 0, LockFlags.Discard);

        }

        private void CreateDBInstanceData() //Метод для заполнения значениями параметров техники DBInstancing, установка буферов
        {
            CustomVertex.PositionTextured[] instData = new CustomVertex.PositionTextured[mySpaceShips.Count * 4];
            short[] instIndeces = new short[mySpaceShips.Count * 6];
            for (int spaceship = 0; spaceship < mySpaceShips.Count; ++spaceship)
            {
                Matrix translMatr = mySpaceShips[spaceship].getTranlsMatr();
                UVSprWSprH UVWH = mySpaceShips[spaceship].getUWVH();
                for (int vert = 0; vert < 4; ++vert)
                {
                    int offset = spaceship * 4 + vert;
                    instData[offset] = new CustomVertex.PositionTextured();
                    Vector3 instPos = origPos[vert].Position;
                    instPos.TransformCoordinate(translMatr);
                    instData[offset].Position = instPos;
                    switch (vert)
                    {
                        case 0:
                            {
                                instData[offset].Tu = UVWH.data.X / TexSize;
                                instData[offset].Tv = UVWH.data.Y / TexSize;
                                break;
                            }
                        case 1:
                            {
                                instData[offset].Tu = (UVWH.data.X + UVWH.data.Z) / TexSize;
                                instData[offset].Tv = UVWH.data.Y / TexSize;
                                break;
                            }
                        case 2:
                            {
                                instData[offset].Tu = (UVWH.data.X + UVWH.data.Z) / TexSize;
                                instData[offset].Tv = (UVWH.data.Y + UVWH.data.W) / TexSize;
                                break;
                            }
                        case 3:
                            {
                                instData[offset].Tu = UVWH.data.X / TexSize;
                                instData[offset].Tv = (UVWH.data.Y + UVWH.data.W) / TexSize;
                                break;
                            }
                    }
                }
                for (int ind = 0; ind < 6; ++ind)
                {
                    int offset = spaceship * 6 + ind;
                    instIndeces[offset] = (short)(origIndices[ind] + spaceship * 4);
                }
            }
            GeometryIndexBuff.SetData(instIndeces, 0, LockFlags.Discard);
            InstancingVertexBuff.SetData(instData, 0, LockFlags.Discard);
        }
        
        #endregion

        #region Rendering

        private void CheckState()
        {
            if (device3D == null)
                return;
            if (state.isDeviceLost || state.isRenderPause) //Приложение свернуто, или рендеринг приостановлен
            {
                System.Threading.Thread.Sleep(100);
            }
            if (!state.isActive) //Окно неактивно
            {
                System.Threading.Thread.Sleep(20);
            }
            if (state.isDeviceLost && !state.isRenderPause) //Устройство потеряно, но рендеринг не был остановлен
            {
                int result;
                if (!device3D.CheckCooperativeLevel(out result))
                {
                    if (result == (int)ResultCode.DeviceLost)
                    {
                        //Устройство потеряно, но не может быть сейчас восстановлено.
                        System.Threading.Thread.Sleep(50);//Ожидание возможности сброса
                        return;
                    }
                    //Попытка восстановить устройство
                    try
                    {
                        ResetDevice();
                    }
                    catch (DeviceLostException)
                    {
                        //Устройство снова потеряно
                        System.Threading.Thread.Sleep(50); //Ожидание возможности сброса устройства
                        return;
                    }
                    catch
                    {
                        //Ошибка сброса устройство, без DeviceLost. Что-то серьезное.
                        //Попытаемся пересоздать устройство
                        try
                        {
                            ChangeDevice();
                        }
                        catch
                        {
                            CleanUp3D();
                            this.Dispose();
                            throw;
                        }
                    }
                }
            }
            state.isDeviceLost = false;
        }//Метод проверки состояния устройства
        
        public void Render()
        {
            CheckState();
            if (!state.isRenderPause)
            {
                device3D.Clear(ClearFlags.Target | ClearFlags.ZBuffer, System.Drawing.Color.CornflowerBlue, 1.0f, 0);
                bool BeginSceneCalled = false;
                try
                {
                    device3D.BeginScene();
                    BeginSceneCalled = true;
                    RenderBackGround();
                    MainRender();
                }
                finally
                {
                    if (BeginSceneCalled)
                        device3D.EndScene();
                }
                try
                {
                    device3D.Present();
                }
                catch (DeviceLostException)
                {
                    state.isDeviceLost = true;
                }
                catch (DriverInternalErrorException)
                {
                    state.isDeviceLost = true;
                }
                PaintTime = DXTimer.Timer(DirectXTimer.GetElapsedTime);
            }
        }//Отрисовка всех элементов

        private void RenderBackGround()
        {
            ValidateEffectTechnique("Background");
            effect.SetValue("tex0", background);
            device3D.SetStreamSource(0, GeometryVertexBuff, 0);
            int numpass = effect.Begin(FX.None);
            for (int i = 0; i < numpass; ++i)
            {
                effect.BeginPass(i);
                device3D.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, origPos.Length, 0, 2);
                effect.EndPass();
            }
            effect.End();

        }

        private void MainRender()
        {
            LoadEffectValue("tex0", tex); //Передача текстуры в шейдер
            if (SupportVS3)
            {
                ValidateEffectTechnique("HWInstancing");
                RenderHWInstancing();
            }
            else
            {
                ValidateEffectTechnique("DBInstancing");
                RenderDBInstancing();
            }
        }//Отрисовка кораблей, персонажа и прицела

        private void RenderDBInstancing() //Отрисовка кораблей при DBInstancing
        {
            CreateDBInstanceData();
            device3D.VertexDeclaration = DBInstancingDecl;
            device3D.SetStreamSource(0, InstancingVertexBuff, 0);
            device3D.Indices = GeometryIndexBuff;

            int numpass = effect.Begin(FX.None);
            for (int i = 0; i < numpass; ++i)
            {
                effect.BeginPass(i);
                device3D.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, mySpaceShips.Count * 4, 0, mySpaceShips.Count * 2);
                effect.EndPass();
            }
            effect.End();
        }

        private void RenderHWInstancing() //Отрисовка кораблей при HWInstancing
        {
            CreateHWInstanceData();

            device3D.VertexDeclaration = HWInstancingDecl;
            device3D.SetStreamSource(0, GeometryVertexBuff, 0);
            device3D.SetStreamSourceFrequency(0, (int)StreamSourceFrequency.IndexedObjectData | mySpaceShips.Count + 2);

            device3D.SetStreamSource(1, InstancingVertexBuff, 0);
            device3D.SetStreamSourceFrequency(1, (int)StreamSourceFrequency.InstanceData | 1);

            device3D.Indices = GeometryIndexBuff;

            int numpass = effect.Begin(FX.None);
            for (int i = 0; i < numpass; ++i)
            {
                effect.BeginPass(i);
                device3D.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, origPos.Length, 0, 2);
                effect.EndPass();
            }
            effect.End();
            device3D.SetStreamSourceFrequency(0, 1);
            device3D.SetStreamSourceFrequency(1, 1);
        }
        
        #endregion

        #region Обработчики событий Device3D

        private void device3D_DeviceReset(object sender, EventArgs e) //Отвечает на событие DeviceReset
        {
            CreateBuff(); //Восстановление буферов после сброса
            SetViewProjMatrix(); //Вычисление значения матрицы View*Projection
            LoadTextureFromStream(out tex,"SPRITES.dds");//Загрузка основной текстуры
            LoadTextureFromStream(out background, GameLevel.LevParameters.backgrTexName);//Загрузка текстуры фона
            LoadEffectFromStream("MY.fx"); //Загрузка эффекта
            LoadEffectValue("ViewProj", ViewProjMatr); //Передача матрицы ViewProj в шейдер
        }
        
        private void device3D_DeviceLost(object sender, EventArgs e)
        {
            if (tex != null)
            {
                tex.Dispose();
                tex = null;
            }
            if (effect != null)
            {
                effect.Dispose();
                effect = null;
            }
            if (GeometryVertexBuff != null)
            {
                GeometryVertexBuff.Dispose();
                GeometryVertexBuff = null;
            }
            if (GeometryIndexBuff != null)
            {
                GeometryIndexBuff.Dispose();
                GeometryIndexBuff = null;
            }
            if (InstancingVertexBuff != null)
            {
                InstancingVertexBuff.Dispose();
                InstancingVertexBuff = null;
            }
        }
        
        private void device3D_Disposing(object sender, EventArgs e)
        {
            if (presParams != null)
            {
                presParams = null;
            }
        }

        private void ResetDevice()
        {
            if (!state.isRenderPause)
                GamePause();
            //System.Diagnostics.Debug.Assert(device3D != null, "Устройство Direct3D не должно быть null");
            if (device3D == null)
                throw new InvalidOperationException("Устройство Device3d не установлено (= null)");
            //Зажигаем событие deviceLost
            device3D_DeviceLost(this, EventArgs.Empty);
            try
            {
                device3D.Reset((PresentParameters)presParams);
            }
            catch (Exception e)
            {
                //device.Reset провален
                device3D_DeviceLost(this, EventArgs.Empty);

                if (e is InvalidCallException)
                    throw new InvalidCallException();
                if (e is DriverInternalErrorException)
                    throw new DriverInternalErrorException();
                if (e is OutOfMemoryException)
                    throw new OutOfMemoryException();
                if (e is OutOfVideoMemoryException)
                    throw new OutOfVideoMemoryException();
            }
            finally
            {
                GameStart();
            }
        } //Метод для сброса устройства

        private void CleanUp3D()
        {
            device3D_DeviceLost(this, EventArgs.Empty);
            device3D.Dispose();
        } //Освобождение всех 3D ресурсов 
        
        #endregion

        #region Переинициализация, переключение полноэкранный-оконный

        private void ToggleFullScreen()
        {
            GamePause();
            state.isWindowed = !presParams.Windowed;
            try
            {
                if ((!presParams.Windowed))
                {
                    if (this != null)
                    {
                        state.isFromFullScr = true;
                        this.WindowState = System.Windows.Forms.FormWindowState.Normal;
                    }
                }
                else if (presParams.Windowed)
                {
                    if (this != null)
                    {
                        oldWindow = getWindowParams(this.Location, this.Size, this.Width, this.Height);
                        this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
                    }
                }
                ChangeDevice();
                this.ClientSize = oldWindow.WindowSize;
                this.Width = oldWindow.WindowWidh;
                this.Height = oldWindow.WindowHeight;
                this.Location = oldWindow.Location;
            }
            finally
            {
                if (state.isRenderPause)
                    GameStart();
            }
        } //Метод переключения между полноэкранным/оконным режимами.

        private void ChangeDevice()//Для смены устройства с новыми presentParameters(для смены полноэкранного-оконного режимов)
        {
            if (device3D.Disposed)
                return;
            CleanUp3D();
            if (state.isRenderPause == false)
                GamePause();
            InitializeGraphics();
        }
        
        #endregion
    }
}
