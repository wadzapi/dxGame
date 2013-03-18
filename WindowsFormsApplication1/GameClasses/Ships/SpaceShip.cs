using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace MarsDX
{
    public class SpaceShip
    {
        protected int health; //Для хранения жизней корабля
        protected int faza, fazaCount; //Фаза и счетчик фазы
        public Vector3 StPos, NowPos, FinPos; //Позиции корабля  - для задания траектории его движения
        double theta = 0.0d; //Угол кручения Position
        private static Random rnd = new Random();



        public Matrix getTranlsMatr()
        {
            //return Matrix.Scaling(GetScalingVect()) * Matrix.Translation(NowPos) * Matrix.Translation(new Vector3((float)(NowPos.Z / 8 *Math.Cos(theta)), (float)(NowPos.Z / 8 * Math.Sin(theta)), 1.0f));
           return getShipMatrix(GetScalingVect(), NowPos, theta);
        }

        private static Matrix getShipMatrix(Vector3 Scaling, Vector3 NowPos, double alpha)
        {
            Matrix m = Matrix.Identity;
            m.M11 = Scaling.X;
            m.M22 = Scaling.Y;
            m.M33 = Scaling.Z;
            m.M41 = (float)(NowPos.Z / 8 * Math.Cos(alpha) + NowPos.X);
            m.M42 = (float)(NowPos.Z / 8 * Math.Sin(alpha) + NowPos.Y);
            m.M43 = NowPos.Z;
            return m;
        }

        protected virtual Vector3 GetScalingVect()
        {
            return new Vector3();
        }

        public virtual UVSprWSprH getUWVH()
        {
            return new UVSprWSprH();
        }

        public SpaceShip()
        {
            //NowPos = new Vector3((float)rnd.Next(-40, 40), (float)rnd.Next(-40, 40), 50.0f);
            NowPos = StPos = new Vector3((float)rnd.Next(-510, 510),(float)rnd.Next(367, 620), 250.0f) ;
            FinPos = new Vector3((float)rnd.Next(-40, 40), rnd.Next(-20, 20), 50.0f);
            faza = 0;
            fazaCount = 0;
        }

        public void Fly(double ElapsedTime) //Метод, задающий перемещение кораблей по экрану
        {
            if (theta >= double.MaxValue)
                theta = 0;
            /*if (NowPos.Z>50)
                NowPos.Z -= NowPos.Z * ElapsedTime / 30;
            NowPos.X = (StPos.X - FinPos.X) * (NowPos.Z - FinPos.Z) / (StPos.Z - FinPos.Z) + FinPos.X;
            NowPos.Y = (StPos.Y - FinPos.Y) * (NowPos.Z - FinPos.Z) / (StPos.Z - FinPos.Z) + FinPos.Y;*/
            theta += ElapsedTime;
        }

    }
}
