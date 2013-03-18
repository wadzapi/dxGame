using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace MarsDX
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            using (Game frm = new Game())
            {
                frm.Show();
                if (!frm.InitializeGraphics())
                    MessageBox.Show("Ваша видеокарта не поддерживает shaders 1.0");
                while (frm.Created)
                {
                    frm.GameProcess();
                    frm.Render();
                    frm.Input.UpdateInput();
                    frm.GetInput();
                    Application.DoEvents();
                }
            }
        }
    }
}
