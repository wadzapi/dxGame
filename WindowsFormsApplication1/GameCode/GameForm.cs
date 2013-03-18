using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace MarsDX
{

    public struct WindowParams //Хранит параметры окна, для из переключения полноэкранного в оконный
    {
        public System.Drawing.Size WindowSize; //Внутренний размер окна в пикселях
        public System.Drawing.Point Location;//Координаты расположения окна
        public int WindowWidh;
        public int WindowHeight;
    }

    public partial class Game : Form
    {
        //Параметры прошлого окна - для перехода из полноэкранного режима, начальной иницизации
        private static WindowParams oldWindow = new WindowParams(){ Location = new System.Drawing.Point(115,115), WindowSize = new System.Drawing.Size(1024, 768), WindowWidh = 1032, WindowHeight= 802 };
        private static bool isMenu = false;

        public Game()
        {
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.Opaque, true);
            InitializeComponent();
            this.ClientSize = oldWindow.WindowSize;
            this.Width = oldWindow.WindowWidh;
            this.Height = oldWindow.WindowHeight;
            this.Location = oldWindow.Location;
            for (int i = 0; i < 1000; ++i)
            {
                mySpaceShips.Add(new SpaceShip01());
            }
            Input = new UserInput(this.Handle);
            GameLevel.SetLevelParameters(Levels[0]);
        }

        private WindowParams getWindowParams(System.Drawing.Point location, System.Drawing.Size windSize, int windW, int windH)
        {
            WindowParams wp = new WindowParams();
            wp.Location = location;
            wp.WindowSize = windSize;
            wp.WindowWidh = windW;
            wp.WindowHeight = windH;
            return wp;
        }


        //Обработчики событий формы
        protected override void OnClosed(EventArgs e)
        {
            CleanUp3D();
            Input.Dispose();
        }
        protected override void OnActivated(EventArgs e)
        {
            state.isActive = true;
            GameStart();
        }
        protected override void OnDeactivate(EventArgs e)
        {
            state.isActive = false;
            GamePause();
        }

        protected override void OnResize(EventArgs e)
        {
            if (this != null && device3D != null)
            {
                if (this.WindowState != FormWindowState.Minimized)
                {
                    GamePause();
                    this.Width = (int)(4.0f / 3 * this.Height);
                    GameStart();
                }
            }
        }

    }
}
