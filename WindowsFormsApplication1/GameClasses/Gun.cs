using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace MarsDX
{
    public class Gun
    {
        protected UVSprWSprH[] TexPoints;
        protected int injury;
        private int fazaShoot;
        public bool isShooting;
        protected int ammo;

        public Gun()
        {
            isShooting = false;
            fazaShoot = 0;
        }

        public UVSprWSprH UVWH
        {
            get
            {
                return TexPoints[fazaShoot];
            }
        }

        public int Ammo
        {
            get
            {
                return ammo;
            }
            set
            {
                if (value >= 0) ammo = value;
                else ammo = 0;
            }
        }
    }

    public class Pistol : Gun
    {
        private static UVSprWSprH[] points = new UVSprWSprH[5]
        {
            new UVSprWSprH(0.0f, 936.0f, 97.0f, 92.0f),
            new UVSprWSprH(104.0f, 926.0f, 119.0f, 102.0f),
            new UVSprWSprH(232.0f, 918.0f, 75.0f, 110.0f),
            new UVSprWSprH(316.0f, 912.0f, 75.0f, 116.0f),
            new UVSprWSprH(399.0f, 918.0f, 94.0f, 110.0f)
        };

        public Pistol()
        {
            ammo = 10000;
            TexPoints = points;
            injury = 2;
        }
    }
}
