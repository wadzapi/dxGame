using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace MarsDX
{
    public class SpaceShip05: SpaceShip
    {
        private static Vector2 faza00 = new Vector2(0.0f, 464.0f);
        private static Vector2[] faza01 = new Vector2[3] { new Vector2(400.0f, 464.0f), new Vector2(800.0f, 464.0f), new Vector2(1200.0f, 464.0f) };
        private static Vector2[] faza02 = new Vector2[3] { new Vector2(1600.0f, 464.0f), new Vector2(0.0f, 616.0f), new Vector2(400.0f, 616.0f) };
        private static Vector2[] faza03 = new Vector2[3] { new Vector2(800.0f, 616.0f), new Vector2(1200.0f, 616.0f), new Vector2(1600.0f, 616.0f) };
        private static Vector2[] faza04 = new Vector2[3] { new Vector2(0.0f, 768.0f), new Vector2(400.0f, 768.0f), new Vector2(800.0f, 768.0f) };
        private static float sprW = 385;
        private static float sprH = 139;
        private static Vector3 Scaling = new Vector3(33.2972f, 12.0f, 1.0f);

        public SpaceShip05()
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
                case 4:
                    {
                        if (fazaCount < faza04.Length)
                            sprPoint = faza04[fazaCount];
                        break;
                    }
            }
            if (sprPoint.X == 0 && sprPoint.Y == 0)
                sprPoint = faza00;
            return sprPoint;
        }

    }
}
