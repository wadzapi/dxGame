using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.DirectX;
using Microsoft.DirectX.DirectInput;

namespace MarsDX
{
    partial class Game
    {
        public UserInput Input; //экземпляр класса, отвечающего за устройства ввода (DirectInput)
        private static bool isMouseExclusive = true;
        private static SingleKey[] keyboardKeys = new SingleKey[] { new SingleKey(Key.Escape), new SingleKey(Key.Return), new UpDownKey(Key.LeftControl), new MultipleKey(Key.A, 0.2d), new MultipleKey(Key.D, 0.2d), new MultipleKey(Key.UpArrow, 0.2d), new MultipleKey(Key.LeftArrow, 0.2d), new MultipleKey(Key.RightArrow, 0.2d), new MultipleKey(Key.DownArrow, 0.2d) };

        public void GetInput()//Обработка данных устройств ввода
        {
            getMouseInput();
            if(Input.keyboardData.keyParams!=null)
                getKeyboardInput();
        }

        #region Обработка событий мыши

        private void getMouseInput()//Обработка данных мыши
        {
            if (Input.mouseData.mouseParams.dx != 0 || Input.mouseData.mouseParams.dy != 0)
            {
                MoveCrossHair();
                moveEye();
            }
            //this.Text = eyePos.Y.ToString() + "dx" + Input.mouseData.mouseParams.dx.ToString() + " ; dy" + Input.mouseData.mouseParams.dy.ToString();
        }

        private void MoveCrossHair()
        {
            GameHero.Crosshair.Position.X += Input.mouseData.mouseParams.dx/4.0f;
            GameHero.Crosshair.Position.Y -= Input.mouseData.mouseParams.dy/4.0f;
        }


        private void moveEye()
        {
            bool changed = false;
            if (GameHero.Crosshair.Position.X <= eyePos.X - 38.7f || GameHero.Crosshair.Position.X >= eyePos.X + 36.7f)
            {
                eyePos.X += Input.mouseData.mouseParams.dx;
                if (eyePos.X < -319.55f)
                    eyePos.X = -319.55f;
                else if (eyePos.X > 319.55f)
                    eyePos.X = 319.55f;
                changed = true;
                if (GameHero.Crosshair.Position.X < eyePos.X - 38.7f)
                    GameHero.Crosshair.Position.X = eyePos.X - 38.7f;
                else if (GameHero.Crosshair.Position.X > eyePos.X + 36.7)
                    GameHero.Crosshair.Position.X = eyePos.X + 36.7f;
            }
            if (GameHero.Crosshair.Position.Y <= eyePos.Y - 29.3 || GameHero.Crosshair.Position.Y >= eyePos.Y + 27.3f)
            {
                eyePos.Y -= Input.mouseData.mouseParams.dy;
                if (eyePos.Y < 0)
                    eyePos.Y = 0;
                else if (eyePos.Y > 479.32f)
                    eyePos.Y = 479.32f;
                changed = true;
                if (GameHero.Crosshair.Position.Y < eyePos.Y - 29.3f)
                    GameHero.Crosshair.Position.Y = eyePos.Y - 29.3f;
                else if (GameHero.Crosshair.Position.Y > eyePos.Y + 27.3f)
                    GameHero.Crosshair.Position.Y = eyePos.Y + 27.3f;
            }
            if (changed)
            {
                if (Input.mouseData.mouseParams.dx!=0)
                    GameHero.Position.X = eyePos.X;
                SetViewProjMatrix();
                LoadEffectValue("ViewProj", ViewProjMatr);
            }
        }
        #endregion

        #region Обработка событий клавиатуры

        private void getKeyboardInput()
        {
            if (keyboardKeys[0].Listen(Input.keyboardData.keyParams[Key.Escape], PaintTime))
            {
                goMenu();
            }
            else
            {
                if (isMenu)
                {
                    if (keyboardKeys[1].Listen(Input.keyboardData.keyParams[Key.Return], PaintTime))
                        SelectItem();
                    else if (keyboardKeys[5].Listen(Input.keyboardData.keyParams[Key.UpArrow], PaintTime))
                        PrevItem();
                    else if (keyboardKeys[8].Listen(Input.keyboardData.keyParams[Key.DownArrow], PaintTime))
                        NextItem();
                }
                else
                {
                    if (keyboardKeys[2].Listen(Input.keyboardData.keyParams[Key.LeftControl], PaintTime))
                        Mouse2Windows();
                    else
                    {
                        if (keyboardKeys[3].Listen(Input.keyboardData.keyParams[Key.A], PaintTime) || keyboardKeys[6].Listen(Input.keyboardData.keyParams[Key.LeftArrow], PaintTime))
                            MoveLeft();
                        else if (keyboardKeys[4].Listen(Input.keyboardData.keyParams[Key.D], PaintTime) || keyboardKeys[7].Listen(Input.keyboardData.keyParams[Key.RightArrow], PaintTime))
                            MoveRight();
                    }
                }
            }
        }

        private void goMenu()
        {
            CloseApp();
        }

        private void NextItem()
        {
        }

        private void PrevItem()
        {
        }

        private void SelectItem()
        {
        }

        private void Mouse2Windows()
        {
            /*
            GamePause();
            isMouseExclusive = !isMouseExclusive;
            Input.ChangeMouse(this.Handle, isMouseExclusive);
            GameStart();*/
        }

        private void MoveLeft()
        {
            ToggleFullScreen();
        }

        private void MoveRight()
        {
        }

        private void CloseApp()
        {
            this.Close();
            System.Windows.Forms.Application.Exit();
        }
        #endregion

    }
}
