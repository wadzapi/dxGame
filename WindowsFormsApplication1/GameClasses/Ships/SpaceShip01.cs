using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace MarsDX
{
    public class SpaceShip01: SpaceShip
    {
        private static Vector2 faza00 = new Vector2(0, 0); //Точка на тестуре, соответствующая нулевой фазе
        private static Vector2[] faza01 = new Vector2[4] { new Vector2(64.0f, 0.0f), new Vector2(132.0f, 0.0f), new Vector2(196.0f, 0.0f), new Vector2(256.0f, 0.0f) }; //Массив точек, соотв. 1ой фазе
        private static Vector2[] faza02 = new Vector2[3] { new Vector2(324.0f, 0.0f), new Vector2(388.0f, 0.0f), new Vector2(452.0f, 0.0f) }; // Массив точек, соотв. 2ой фазе
        public static float sprW = 50; // Ширина спрайта
        public static float sprH = 66; // Высота спрайта
        private static Vector3 Scaling = new Vector3(5.2f, 6.864f, 0.0f); // Вектор для Масштабирования спрайта корабля
        
        public SpaceShip01()
        {
        }

        public override UVSprWSprH getUWVH()
        {
            return new UVSprWSprH(getSprPoint(faza, fazaCount), sprW, sprH);
        }

        protected override Vector3 GetScalingVect()
        {
            return Scaling;
        }

        public static Vector2 getSprPoint(int faza, int fazaCount) //Метод для получения координат спрайта на текстуре, в зав. от текущей фазы и счетчика фазы
        {
            Vector2 sprPoint = new Vector2();
            switch (faza)
            {
                case 1:
                    {
                        if (fazaCount < faza01.Length)
                            sprPoint = faza01[fazaCount];
                        break;
                    }
                case 2:
                    {
                        if (fazaCount < faza02.Length)
                            sprPoint = faza02[fazaCount];
                        break;
                    }
            }
            if (sprPoint.X == 0 && sprPoint.Y == 0)
                sprPoint = faza00;
            return sprPoint;
        }
    }
}
