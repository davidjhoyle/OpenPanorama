using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenPanoramaLib
{
    public class PeakHorizonVector
    {
        public HorizonPoint[] theHorizon = null;

        public PeakHorizonVector(int len)
        {
            theHorizon = new HorizonPoint[len];
        }
    }
}
