using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenPanoramaLib
{
    public class Foresight
    {
        [Flags]
        public enum HeavenlyTraces
        {
            WinterSolsticeSun = 1,
            SummerSolsticeSun = 2,
            EquinoxSun = 4,
            MoonMinusEMinusI = 8,
            MoonMinusEPlusI = 16,
            MoonPlusEMinusI = 32,
            MoonPlusEPlusI = 64,
            CrossQuarterSun1 = 128,
            CrossQuarterSun2 = 256,
        }


        [Flags]
        public enum TopCenBot
        {
            Top = 1,
            Centre = 2,
            Bottom = 4
        }

        public Foresight()
        {
        }

        public Foresight(string desc)
        {
            Description = desc;
        }

        public string GetDesc(HeavenlyTraces ht)
        {
            switch (ht)
            {
                case HeavenlyTraces.WinterSolsticeSun:
                    return "Solstice";

                case HeavenlyTraces.SummerSolsticeSun:
                    return "Solstice";

                case HeavenlyTraces.EquinoxSun:
                    return "Equinox";

                case HeavenlyTraces.MoonMinusEMinusI:
                    return "-e-i";

                case HeavenlyTraces.MoonMinusEPlusI:
                    return "-e+i";

                case HeavenlyTraces.MoonPlusEMinusI:
                    return "+e-i";

                case HeavenlyTraces.MoonPlusEPlusI:
                    return "+e+i";

                case HeavenlyTraces.CrossQuarterSun1:
                    return "X Q1";

                case HeavenlyTraces.CrossQuarterSun2:
                    return "X Q2";
            }
            return "Unknown " + (int)ht;
        }

        public string Description;
        public string infostr;
        //public HeavenlyTraces traces;
        public double declination = 0;
        public bool isMoon = false;

        public void CalculateDeclination(double theyear, Foresight.HeavenlyTraces t)
        {
            // Need to update to this formula - https://en.wikipedia.org/wiki/Axial_tilt
            //ε = 23° 26′ 21.448″ − 4680.93″ t − 1.55″ t2 + 1999.25″ t3 − 51.38″ t4 − 249.67″ t5 − 39.05″ t6 + 7.12″ t7 + 27.87″ t8 + 5.79″ t9 + 2.45″ t10

            //double declinationat3000BC = 24.03;
            //double declinationat2000BC = 23.9292;
            //double declinationat1900AD = 23.4523;
            //double changeperyear = (declinationat3000BC - declinationat2000BC) / 1000;
            //declination = declinationat2000BC + changeperyear * (theyear - 4000);


            //double T0 = 1;
            //double T1 = -theyear / 100;
            //double T2 = T1 * T1;
            //double T3 = T2 * T1;
            //double T4 = T3 * T1;
            //double T5 = T4 * T1;
            //double T6 = T5 * T1;
            //double T7 = T6 * T1;
            //double T8 = T7 * T1;
            //double T9 = T8 * T1;
            //double T10 = T9 * T1;


            //// Newcomb ε = 23° 27′ 08.26″ − 46.845″ T − 0.0059″ T2 + 0.00181″ T3
            //double Newcombε = 23 * 60 * 60 + 27 * 60 + 08.26 * 60 - 46.845 * T1 - 0.0059 * T2 + 0.00181 * T3;
            //double NewcombεDeg = Newcombε / 60 / 60;


            double t0 = 1;
            double t1 = -theyear / 10000;
            double t2 = t1 * t1;
            double t3 = t2 * t1;
            double t4 = t3 * t1;
            double t5 = t4 * t1;
            double t6 = t5 * t1;
            double t7 = t6 * t1;
            double t8 = t7 * t1;
            double t9 = t8 * t1;
            double t10 = t9 * t1;

            // https://en.wikipedia.org/wiki/Axial_tilt
            // ε = 23° 26′ 21.448″ − 4680.93″ t − 1.55″ t2 + 1999.25″ t3 − 51.38″ t4 − 249.67″ t5 − 39.05″ t6 + 7.12″ t7 + 27.87″ t8 + 5.79″ t9 + 2.45″ t10
            // where here t is multiples of 10,000 Julian years from J2000.0.[23]
            const double value0 = 23 * 60 * 60 + 26 * 60.0 + 21.448;
            const double value1 = -4680.93;
            const double value2 = -1.55;
            const double value3 = 1999.25;
            const double value4 = -51.38;
            const double value5 = -249.67;
            const double value6 = -39.05;
            const double value7 = 7.12;
            const double value8 = 27.87;
            const double value9 = 5.79;
            const double value10 = 2.45;


            t0 *= value0;
            t1 *= value1;
            t2 *= value2;
            t3 *= value3;
            t4 *= value4;
            t5 *= value5;
            t6 *= value6;
            t7 *= value7;
            t8 *= value8;
            t9 *= value9;
            t10 *= value10;

            double declinationε = t0 + t1 + t2 + t3 + t4 + t5 + t6 + t7 + t8 + t9 + t10;

            //double decdiff = declinationε - (declination*60*60);
            //double Newcombεdiff = Newcombε - (declination * 60 * 60);
            double declinationεDeg = declinationε / 60 / 60;
            //double adecdiffMins = decdiff / 60;
            //double adecdiffDegs = adecdiffMins / 60;
            //double halfdecdiff = decdiff / 2;

            declination = declinationεDeg;
            double moonInclination = 5.145;
            isMoon = true;

            string infostr = Description + ", " + t.ToString();

            switch (t)
            {
                case Foresight.HeavenlyTraces.WinterSolsticeSun:
                    declination = -declination;
                    isMoon = false;
                    break;

                case Foresight.HeavenlyTraces.SummerSolsticeSun:
                    isMoon = false;
                    break;

                case Foresight.HeavenlyTraces.EquinoxSun:
                    declination = 0;
                    isMoon = false;
                    break;

                case Foresight.HeavenlyTraces.MoonMinusEMinusI:
                    declination = -declination - moonInclination;
                    break;

                case Foresight.HeavenlyTraces.MoonMinusEPlusI:
                    declination = -declination + moonInclination;
                    break;

                case Foresight.HeavenlyTraces.MoonPlusEMinusI:
                    declination -= moonInclination;
                    break;

                case Foresight.HeavenlyTraces.MoonPlusEPlusI:
                    declination += moonInclination;
                    break;

                case Foresight.HeavenlyTraces.CrossQuarterSun1:
                    declination = declination / Math.Sqrt(2);
                    isMoon = false;
                    break;

                case Foresight.HeavenlyTraces.CrossQuarterSun2:
                    declination = -declination / Math.Sqrt(2);
                    isMoon = false;
                    break;

                default:
                    break;
            }
        }
    }
}
