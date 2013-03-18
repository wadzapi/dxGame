using System;
using System.Collections.Generic;
using System.Text;

namespace MarsDX
{
    partial class Game
    {
        private const int maxShips = 3000;//Максимальное число кораблей
        private static double PaintTime = 0.0d; //время, затраченное на отрисовку
        private static double shipTime = 0.0d; //время появления нового корабля
        
        private List<SpaceShip> mySpaceShips = new List<SpaceShip>(); //Список кораблей
        
        private static LevelParams[] Levels = new LevelParams[4]
        {
            new LevelParams("BackGrounds.RedSquare.dds",new int[6]{50, 30, 20, 0, 0, 0}, 4.0d, 40),
            new LevelParams("BackGrounds.EiffelTower.dds", new int[6]{10, 40, 30, 20, 0, 0}, 4.0d, 40),
            new LevelParams("BackGrounds.Whitehouse.dds", new int[6]{0, 10, 40, 30, 20, 0}, 4.0d, 40),
            new LevelParams("BackGrounds.AlienCity.dds", new int[6]{0, 0, 0, 30, 50, 20}, 4.0d, 40) 
        };

        private void GamePause()
        {
            DXTimer.Timer(DirectXTimer.Stop);
            state.isRenderPause = true;
        }//Остановка игры
        private void GameStart()
        {
            DXTimer.Timer(DirectXTimer.Start);
            state.isRenderPause = false;
        }//Продолжение игры

        public void GameProcess() //Задание параметров, отслеживание событий игры
        {
            if (state.isRenderPause)
                return;
            /*shipTime += ElpTime;
            if (shipTime > 3)
            {
                mySpaceShips.Add(new SpaceShip02());
                shipTime = 0.0f;
            }*/
            foreach (SpaceShip spsh in mySpaceShips)
            {
                spsh.Fly(PaintTime);
            }

        }


    }
}
