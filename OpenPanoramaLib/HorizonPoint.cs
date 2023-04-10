using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenPanoramaLib
{
    public class HorizonPoint
    {
        public double latitude;
        public double longitude;
        public double elevation;
        public double distance;
        public double bearing;
        public double declination;

        public double getY(bool useDeclinations)
        {
            if (useDeclinations)
            {
                return declination;
            }
            else
            {
                return elevation;
            }
        }
    }
}
