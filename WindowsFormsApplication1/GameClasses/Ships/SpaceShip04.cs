using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace MarsDX
{
     public class SpaceShip04: SpaceShip
    {
         private static Vector2 faza00 = new Vector2(0.0f, 304.0f);
         private static Vector2[] faza01 = new Vector2[3] { new Vector2(167.0f, 304.0f), new Vector2(336.0f, 304.0f), new Vector2(504.0f, 304.0f) };
         private static Vector2[] faza02 = new Vector2[3] { new Vector2(672.0f, 304.0f), new Vector2(840.0f, 304.0f), new Vector2(1008.0f, 304.0f) };
         private static Vector2[] faza03 = new Vector2[3] { new Vector2(1176.0f, 304.0f), new Vector2(1344.0f, 304.0f), new Vector2(1512.0f, 304.0f) };
         private static float sprW = 156;
         private static float sprH = 146;
         private static Vector3 Scaling = new Vector3(12.822f, 12.0f, 0.0f);

         public SpaceShip04()
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
