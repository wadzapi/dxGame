using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace MarsDX
{
    public class SpaceShip02: SpaceShip
    {
        private static Vector2 faza00 = new Vector2(0.0f, 80.0f);
        private static Vector2[] faza01 = new Vector2[3] { new Vector2(120.0f, 80.0f), new Vector2(240.0f, 80.0f), new Vector2(360.0f, 80.0f) };
        private static Vector2[] faza02 = new Vector2[3] { new Vector2(480.0f, 80.0f), new Vector2(600.0f, 80.0f), new Vector2(712.0f, 80.0f) };
        private static float sprW = 100;
        private static float sprH = 76;
        private static Vector3 Scaling = new Vector3(7.88f, 6.0f, 0.0f);

        public SpaceShip02()
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

        private static Vector2 getSprPoint(int faza, int fazaCount)
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
