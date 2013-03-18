using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace MarsDX
{
    public struct LevelParams
    {
        public string backgrTexName;
        public int[] ShipProbability;
        public int maxShips;
        public double timeNewShip;

        public LevelParams(string BackGrTexName, int[] ShipProbab, double TimeNewShip, int MaxShips)
        {
            this.backgrTexName = BackGrTexName;
            this.ShipProbability = ShipProbab;
            this.timeNewShip = TimeNewShip;
            this.maxShips = MaxShips;
        }
    }

    public static class GameLevel
    {
        public static LevelParams LevParameters;
        private static int ShipCounter;

        public static void SetLevelParameters(LevelParams parameters)
        {
            LevParameters = parameters;
        }

        public static int RandomShip(int index)
        {
            int shipNumber;
            int IntervalMax = LevParameters.ShipProbability[0];
            for (shipNumber = 0; shipNumber < 5; ++shipNumber)
            {
                if (index <= IntervalMax) break;
                else IntervalMax += LevParameters.ShipProbability[shipNumber + 1];
            }
            return shipNumber;
        }

        public static void AddShipCounter()
        {
            ++ShipCounter;
        }

        public static bool checkMaxShips()
        {
            if (ShipCounter>=LevParameters.maxShips)
                return true;
            else return false;
        }
    }
}
