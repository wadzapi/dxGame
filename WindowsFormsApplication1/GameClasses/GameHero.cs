using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace MarsDX
{
    public static class GameHero
    {
        public static class Crosshair
        {
            public static Vector3 Position = new Vector3(0.0f, 0.0f, 49.0f);
            public static UVSprWSprH UVWH = new UVSprWSprH(2016.0f, 0.0f, 32.0f, 32.0f);
            public static Matrix TransformMatr
            {
                get
                {
                    return Matrix.Scaling(new Vector3(2.0f, 2.0f, 1.0f)) * Matrix.Translation(Position);
                }
            }
        }

        private static int gunIndex = 0;
        public static Vector3 Position = new Vector3(0.0f, -27.713f, 48.0f);

        public static Matrix TransformMatr
        {
            get
            {
                return Matrix.Scaling(new Vector3(10.0f, gun.UVWH.data.Z/gun.UVWH.data.W * 10.0f, 1.0f)) * Matrix.Translation(Position);
            }
        }

        private static Gun[] Guns = new Gun[1]
        {
            new Pistol()
        };

        public static Gun gun = Guns[gunIndex];

    }
}
