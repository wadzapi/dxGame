using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace MarsDX
{
    public class SpaceShip03: SpaceShip
    {
        private static Vector2 faza00 = new Vector2(0.0f, 168.0f);
        private static Vector2[] faza01 = new Vector2[3] { new Vector2(120.0f, 168.0f), new Vector2(240.0f, 168.0f), new Vector2(360.0f, 168.0f) };
        private static Vector2[] faza02 = new Vector2[3] { new Vector2(480.0f, 168.0f), new Vector2(600.0f, 168.0f), new Vector2(720.0f, 168.0f) };
        private static Vector2[] faza03 = new Vector2[3] { new Vector2(840.0f, 168.0f), new Vector2(960.0f, 168.0f), new Vector2(1080.0f, 168.0f) };
        private static float sprW = 102;
        private static float sprH = 120;
        private static Vector3 Scaling = new Vector3(10.0f, 11.764f, 0.0f);

        public SpaceShip03()
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
                case 3:
                    {
                        if (fazaCount < faza03.Length)
                            sprPoint = faza03[fazaCount];
                        break;
                    }
            }
            if (sprPoint.X == 0 && sprPoint.Y == 0)
                sprPoint = faza00;
            return sprPoint;
        }

    }
}
