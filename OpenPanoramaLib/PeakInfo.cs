using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenPanoramaLib
{
    public class PeakInfo
    {
        public HorizonPoint horizonPoint;
        public bool isPeak;
        public double slopeWeight;
        public double elevDiffWeight;
        public int indx;

        public PeakInfo(HorizonPoint ahp, bool aPeak, double weight, double aelevDiff, int xindex)
        {
            horizonPoint = ahp;
            isPeak = aPeak;
            slopeWeight = weight;
            elevDiffWeight = aelevDiff;
            indx = xindex;
        }
        public double overallWeight
        {
            get => elevDiffWeight * slopeWeight;
        }
    }
}
