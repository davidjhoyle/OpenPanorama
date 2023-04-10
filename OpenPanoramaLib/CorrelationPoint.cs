using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenPanoramaLib
{
    public class CorrelationPoint
    {
        public PeakInfo peakInfo;
        public RisingAndSettingPoint rasp;
        public double deltaBearing;
        public double correlationValue;
        public double correlationValueSlopeWeighted;
        public double correlationValueElevDiffWeighted;
        public double correlationValueWeighted;
    }
}
