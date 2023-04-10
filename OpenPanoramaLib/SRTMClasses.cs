using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenPanoramaLib
{
    public class SRTMTileCacheLine
    {
        public Int16[][] tile;
        public ulong lastAccess;
    }


    class SRTMClass
    {
        public static int SRTMDataCacheMaxLines = 12;

        public static void SetSRTMCacheSize(int siz)
        {
            SRTMDataCacheMaxLines = siz;
        }


        public Int16[][] GetGridSRTM(int latInt, int lonInt)
        {
            //ZipOSHandler zippy = new ZipOSHandler();
            return ZipMapDataHandler.GetSRTMData(latInt, lonInt);
        }



        static Dictionary<string, double> AllMaxHeights = new Dictionary<string, double>();

        public double GetMaxHeight(int latInt, int lonInt, Int16[][] heightData, int SubTileLat, int SubTileLon)
        {
            string Indx = latInt + " " + lonInt + " " + SubTileLat + " " + SubTileLon;
            lock (AllMaxHeights)
            {
                if (AllMaxHeights.ContainsKey(Indx))
                {
                    return AllMaxHeights[Indx];
                }
                else
                {
                    short maxZ = -1;
                    for (int latloop = SubTileLat; latloop < SubTileLat + gridLoop; latloop++)
                    {
                        for (int lonloop = SubTileLon; lonloop < SubTileLon + gridLoop; lonloop++)
                        {
                            short z = heightData[latloop][lonloop];
                            if (z > maxZ)
                            {
                                maxZ = z;
                            }
                        }
                    }
                    AllMaxHeights[Indx] = maxZ;
                    return AllMaxHeights[Indx];
                }
            }
        }

        // Be careful adjusting this - need to make sure the result is a whole number...
        public const int gridLoop = ZipMapDataHandler.gridCount / 10; // 100;


        public bool CanSubTileBeSeen(Int16[][] heightData, double lat, double lon, double hght, int latInt, int lonInt, int SubTileLat, int SubTileLon )
        {
            double maxZ = GetMaxHeight(latInt, lonInt, heightData, SubTileLat, SubTileLon);
            double minDist = 24000000;

            for (int subla = SubTileLat; subla <= SubTileLat + gridLoop; subla += gridLoop)
            {
                for (int sublo = SubTileLon; sublo <= SubTileLon + gridLoop; sublo += gridLoop)
                {
                    double distance = LatLonConversions.distHaversine(lat, lon,
                            (double)latInt + (double)subla / (double)ZipMapDataHandler.gridCount,
                            (double)lonInt + (double)sublo / (double)ZipMapDataHandler.gridCount);
                    if (distance < minDist)
                    {
                        minDist = distance;
                    }
                }
            }

            SeaHorizon sh = SeaHorizon.GetHorizonForHeight((int)hght);
            if (sh == null)
            {
                return true;
            }

            double dip = GeneralUtilClasses.CalculateDip(minDist);
            double Elevation = Math.Atan((maxZ - hght - dip) / minDist) * 180 / Math.PI;


            // Is it below the distant sea horizon - works everywhere apart from dead sea etc.
            if (minDist > sh.distance && Elevation < sh.elevation)
            {
                return false;
            }

            return true;

            //int IntElevation = (int)(Elevation * rjParams.pixels + rjParams.pixels * rjParams.negativeRange);

            //if (IntElevation >= 0)
            //{
            //    return true;
            //}
            //else
            //{
            //    return false;
            //}
        }
    }
}
