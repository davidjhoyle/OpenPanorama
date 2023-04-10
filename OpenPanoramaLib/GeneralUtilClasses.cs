using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenPanoramaLib
{
    public class GeneralUtilClasses
    {
        static public void RunGC()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }


        public static double CalculateDip(double distance, double adjust = 6.0 / 7)
        {
            double earthrad = 6371000;
            double earthcircum = earthrad * 2 * Math.PI;

            double distanceinradians = distance / earthcircum * 2 * Math.PI;
            double dip = earthrad * (1 - Math.Cos(distanceinradians));

            // Adjust this for "typical" refraction - not perfect...
            // dip = dip * .90 / .97;
            // https://en.wikipedia.org/wiki/International_Standard_Atmosphere
            // https://www.metabunk.org/standard-atmospheric-refraction-empirical-evidence-and-derivation.t8703/#post-205947
            dip = dip * adjust;
            return dip;
        }

    }
}
