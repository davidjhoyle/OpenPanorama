using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenPanoramaLib
{
    public class MovingBody
    {
        //// https://en.wikipedia.org/wiki/Solar_zenith_angle
        //// https://en.wikipedia.org/wiki/Solar_azimuth_angle
        //public static SkyPos CalculateSolarElevationAzimuth( double latitude, double declination, double hour )
        //{
        //    double tmp = Math.Sin(latitude) * Math.Sin(declination) + Math.Cos(latitude) * Math.Cos(declination) * Math.Cos(hour);

        //    double elevation = Math.Asin(tmp);
        //    double zenith = Math.Acos(tmp);

        //    double tmp2 = ( Math.Sin(declination) - Math.Cos(zenith) * Math.Sin(latitude)) / Math.Sin(zenith) / Math.Cos( latitude );
        //    double azimuth = Math.Acos(tmp2);

        //    elevation = AdjustElevationForDefraction(elevation);

        //    return new SkyPos(elevation, azimuth);
        //}

        public static double AdjustElevationForParallelax(double elevation, bool moon)
        {
            double moondist = 384399000;
            double sundist = 92955902000;
            double earthradius = 6371000;
            double dist = sundist;

            if (moon)
            {
                dist = moondist;
            }

            double elevation_diff = Math.Atan(earthradius / dist) * Math.Cos(elevation);
            return elevation_diff;
        }

        //static double[] oldRefractionIntervals = new double[]
        //    { -1.0/3, 0, 1.0/3, 2.0/3, 1, 4.0/3, 5.0/6, 2, 2.5, 3, 4, 5, 10 };

        //static double[] RefractionIntervals = new double[]
        //    { -1.0/3, 0, 1.0/3, 2.0/3, 1, 4.0/3, 5.0/6, 2, 2.5, 3, 4, 5, 10 };

        //static double[] RefractionValues = new double[]
        //    { 40, 34.8, 30.6, 27.2, 24.5, 22.1, 20.1, 18.3, 16.2, 14.4, 11.8, 9.9, 5 };

        public static double AdjustElevationForRefraction(double elevation)
        {
            //bool newFormula = true;
            //double refraction = 0;
            //double pressure = 1000;
            double degs = elevation * 180 / Math.PI;
            double temp = 10.0;

            // The TAN FN Blows up around -4.65 - this will prevent the odd strange dot appearing occasionally.
            if (degs < -4)
            {
                degs = -4;
            }

            //// Barbados and Edmonton Refraction Formula - http://wise-obs.tau.ac.il/~eran/Wise/Util/Refraction.html
            //double refraction2 = pressure * (0.1594 + 0.0196 * degs + 0.00002 * degs * degs) / ((273 + temp) * (1 + 0.505 * degs + 0.0845 * degs * degs));
            //refraction2 = refraction2 / 180.0 * Math.PI;
            //double ref2Mins = refraction2 * 180 / Math.PI * 60;

            //return refraction2;

            // https://en.wikipedia.org/wiki/Atmospheric_refraction
            // 
            double tmp = degs + 10.3 / (degs + 5.11);
            tmp = tmp / 180 * Math.PI;
            double pressurekPa = 101.0;
            double refraction = 1.02 / Math.Tan(tmp);
            double adjust = pressurekPa / 101.0 * 283.0 / (273.0 + temp);
            refraction = refraction * adjust;
            refraction = refraction / 60 / 180 * Math.PI;

            return refraction;
        }


        public static double AdjustElevationForTopMidBottom(bool moon, Foresight.TopCenBot tcb)
        {
            if (((tcb & Foresight.TopCenBot.Centre) != 0) || tcb == 0)
            {
                return 0;
            }

            double moondist = 384399000;
            double sundist = 149600000000;
            double moonradius = 1737100;
            double sunradius = 696342000;
            double dist = sundist;
            double radius = sunradius;

            if (moon)
            {
                dist = moondist;
                radius = moonradius;
            }

            double elevation_diff = Math.Atan(radius / dist);

            if ((tcb & Foresight.TopCenBot.Bottom) != 0)
            {
                return -elevation_diff;
            }
            else
            {
                return +elevation_diff;
            }
        }


        public static double ToDegrees(double rad)
        {
            return rad * 180 / Math.PI;
        }


        public static SkyPos CalculateBodyElevationAzimuth(double latitude, double longitude, double declination, double hour, bool moon, Foresight.TopCenBot tcb, ref string infostr, bool wobble)
        {
            //            infostr += ", Lat/Long " + ToDegrees(latitude) + ", " + ToDegrees( longitude) + ", " + tcb.ToString();
            bool new_formula = false;

            // https://astronomy.stackexchange.com/questions/1855/how-to-get-the-longitude-latitude-from-solar-zenith-azimuth


            double MidTopBottom = AdjustElevationForTopMidBottom(moon, tcb);
            declination += MidTopBottom;
            //infostr += ", MTB " + ToDegrees(MidTopBottom);

            if (wobble && moon)
            {
                const double wobbleRad = (9.0 / 60) / 180 * Math.PI;
                if (tcb == Foresight.TopCenBot.Top)
                {
                    declination += wobbleRad;
                }
                if (tcb == Foresight.TopCenBot.Bottom)
                {
                    declination -= wobbleRad;
                }
            }

            double elevation = 0;
            double azimuth = 0;

            if (new_formula)
            {
                double tmpaz = (Math.Sin(latitude) / Math.Tan(hour)) - (Math.Cos(latitude) * Math.Tan(declination) / Math.Sin(hour));
                if (tmpaz > 1)
                {
                    tmpaz = 1;
                }
                if (tmpaz < -1)
                {
                    tmpaz = -1;
                }
                azimuth = Math.Atan(1 / tmpaz);

                double tmpel = (Math.Sin(declination) * Math.Sin(latitude)) - (Math.Cos(declination) * Math.Cos(latitude) * Math.Cos(hour));
                if (tmpel > 1)
                {
                    tmpel = 1;
                }
                if (tmpel < -1)
                {
                    tmpel = -1;
                }
                elevation = Math.Asin(tmpel);
            }
            else
            {
                // Get the Elevation Angle first
                double tmp = Math.Sin(declination) * Math.Sin(latitude) + Math.Cos(declination) * Math.Cos(latitude) * Math.Cos(hour);
                if (tmp > 1)
                {
                    tmp = 1;
                }
                if (tmp < -1)
                {
                    tmp = -1;
                }
                elevation = Math.Asin(tmp);

                // Now the azimuth
                double tmp2 = (Math.Sin(declination) * Math.Cos(latitude) - Math.Cos(declination) * Math.Sin(latitude) * Math.Cos(hour)) / Math.Cos(elevation);
                if (tmp2 > 1)
                {
                    tmp2 = 1;
                }
                if (tmp2 < -1)
                {
                    tmp2 = -1;
                }
                azimuth = Math.Acos(tmp2);
            }

            double parallax = AdjustElevationForParallelax(elevation, moon);
            infostr += ", Parallax " + ToDegrees(parallax);
            elevation -= parallax;

            double refraction = AdjustElevationForRefraction(elevation);

            elevation += refraction;
            //infostr += ", El " + ToDegrees(elevation);

            return new SkyPos(elevation, azimuth);
        }
    }
}
