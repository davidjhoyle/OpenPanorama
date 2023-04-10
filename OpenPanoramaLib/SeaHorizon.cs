using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenPanoramaLib
{
    public class SeaHorizon
    {
        public double distance;
        public double elevation;

        static Dictionary<int, SeaHorizon> heightHorizons = new Dictionary<int, SeaHorizon>();

        static public SeaHorizon GetHorizonForHeight(int height)
        {
            if (heightHorizons.ContainsKey(height))
            {
                return heightHorizons[height];
            }

            double maxElevation = -9999;

            for (int dist = 1; dist < 10000000; dist += 1)
            {
                double dip = PaintImage.CalculateDip(dist);
                double Elevation = Math.Atan((-height - dip) / dist) * 180 / Math.PI;

                if (Elevation > maxElevation)
                {
                    maxElevation = Elevation;
                }
                else
                {
                    SeaHorizon sh = new SeaHorizon();
                    sh.elevation = Elevation;
                    sh.distance = dist;
                    lock (heightHorizons)
                    {
                        heightHorizons[height] = sh;
                        return sh;
                    }
                }
            }
            return null;
        }
    }
}
