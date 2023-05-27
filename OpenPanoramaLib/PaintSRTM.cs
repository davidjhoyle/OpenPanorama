using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PanaGraph;

namespace OpenPanoramaLib
{
    internal class PaintSRTM
    {

        public Int16[][] GetGridSRTM(int latInt, int lonInt)
        {
            //ZipOSHandler zippy = new ZipOSHandler();
            return ZipMapDataHandler.GetSRTMData(latInt, lonInt);
        }


        public void PaintSRTMHeights(PaintImage pi, double lat, double lon, double hght)
        {
            if (!pi.rjParams.drwSRTM)
            {
                return;
            }


            Console.WriteLine("PaintSRTMHeights " + lat + ", " + lon);

            double angleRange = (double)pi.rjParams.maxDistance / 40000000 * 360;

            double latRange = angleRange;
            double lonRange = angleRange;

            Graphics profGraph = Graphics.FromImage(pi.myBitmap);

            Int16[][] heightData = null;
            Int16[][] oldheightData = null;


            for (int latInt = (int)Math.Floor(lat - latRange); latInt <= (int)Math.Floor(lat + latRange); latInt++)
            {
                for (int lonInt = (int)Math.Floor(lon - lonRange); lonInt <= (int)Math.Floor(lon + lonRange); lonInt++)
                {
                    heightData = GetGridSRTM(latInt, lonInt);
                    if (oldheightData != heightData)
                    {
                        // Only update every new SRTM tile...
                        pi.UpdateWorkInProgress();
                    }
                    if (heightData != null)
                    {
                        DrawSRTMData(pi, profGraph, heightData, lat, lon, hght, latInt, lonInt);
                    }
                    oldheightData = heightData;
                    heightData = null;
                }
            }
            profGraph.me.Flush();
            GeneralUtilClasses.RunGC();
        }


        static long countSRTMPts = 0;


        // Be careful adjusting this - need to make sure the result is a whole number...
        public const int gridLoop = ZipMapDataHandler.gridCount / 10; // 100;


        public void DrawSRTMData(PaintImage pi, Graphics profGraph, Int16[][] heightData, double lat, double lon, double hght, int latInt, int lonInt)
        {
            // Drawing Vertically First...
            SetSRTMCacheHint(false);

            double minDist = 9999999;
            HorizonVector hv = new HorizonVector();

            double dx = LatLonConversions.distHaversine(lat, lon, lat, lon + 1.0 / ZipMapDataHandler.gridCount);
            double dy = LatLonConversions.distHaversine(lat, lon, lat + 1.0 / ZipMapDataHandler.gridCount, lon);
            double diagDist = Math.Sqrt(dy * dy + dx * dx);

            double latout1 = 0;
            double lonout1 = 0;
            double latout2 = 0;
            double lonout2 = 0;


            for (int SubTileLat = 0; SubTileLat < ZipMapDataHandler.gridCount; SubTileLat += gridLoop)
            {
                for (int SubTileLon = 0; SubTileLon < ZipMapDataHandler.gridCount; SubTileLon += gridLoop)
                {
                    if (!CanSubTileBeSeen(pi, heightData, lat, lon, hght, latInt, lonInt, SubTileLat, SubTileLon))
                    {
                        continue;
                    }

                    for (int lonloop = SubTileLon; lonloop < SubTileLon + gridLoop; lonloop++)
                    {
                        for (int latloop = SubTileLat; latloop < SubTileLat + gridLoop; latloop++)
                        {
                            double z = heightData[latloop][lonloop];
                            if (z <= -100)
                            {
                                continue;
                            }

                            double distance = 0;
                            double Angle = 0;
                            double Elevation = 0;

                            GetCachedSRTMValues(pi, pi.rjParams.proximalInterpolation, lat, lon, hght, latInt, lonInt, latloop, lonloop, z, ref Angle, ref Elevation, ref distance, ref latout1, ref lonout1);
                            int ElevationInt = (int)(Elevation * pi.rjParams.pixels + pi.rjParams.pixels * pi.rjParams.negativeRange);
                            int AngleInt = (int)(Angle * pi.rjParams.pixels);

                            if (distance < minDist)
                            {
                                minDist = distance;
                            }

                            if (distance >  pi.rjParams.maxDistance)
                            {
                                continue;
                            }


                            // Don't draw closer than rjParams.minDistance (70) metres...
                            if (distance < pi.rjParams.minDistance)
                            {
                                continue;
                            }


                            // If LIDAR Enabled then don't draw within the LIDAR range.
                            if ((pi.rjParams.drwLIDAR && distance < pi.rjParams.lidarrange - 100) && !pi.rjParams.drwAllContours)
                            {
                                continue;
                            }


                            // When close to the observer, draw diagonals, otherwise just draw the other two lines...
                            int max_y_loops = 4;
                            if (distance > 5000 && !pi.rjParams.proximalInterpolation)
                            {
                                max_y_loops = 2;
                            }

                            double z2 = 0;

                            double steepest = 99999;

                            double tmpz = z;

                            // Get the max gradient for the point.
                            for (int i = 0; i < 4; i++)
                            {
                                int latIndx = latloop;
                                int lonIndx = lonloop;
                                double dist = 0;

                                switch (i)
                                {
                                    case 0:
                                        latIndx += 1;
                                        dist = dy;
                                        break;
                                    case 1:
                                        dist = dx;
                                        lonIndx += 1;
                                        break;
                                    case 2:
                                        dist = diagDist;
                                        latIndx += 1;
                                        lonIndx += 1;
                                        break;
                                    case 3:
                                        z = heightData[latIndx + 1][lonIndx];
                                        dist = diagDist;
                                        lonIndx += 1;
                                        break;
                                }


                                z2 = heightData[latIndx][lonIndx];

                                double dz = z - z2;
                                if (dz < 0) dz -= dz;
                                double grad = 0;

                                if (dz == 0)
                                {
                                    grad = 99999;
                                }
                                else
                                {
                                    grad = dist / dz;
                                }

                                if (grad < steepest)
                                {
                                    steepest = grad;
                                }
                            }
                            z = tmpz;


                            if (pi.rjParams.proximalInterpolation)
                            {
                                double prevy = latout1;
                                double prevx = lonout1;

                                double prevAngle = Angle;
                                int prevAngleInt = AngleInt;
                                double prevElevation = Elevation;
                                int prevElevationInt = ElevationInt;
                                double prevDistance = distance;

                                for (int i = 1; i <= max_y_loops; i++)
                                {
                                    int ddy = (i & 2) / 2;
                                    int ddx = (i & 1) ^ ddy;

                                    int latIndx = latloop;
                                    int lonIndx = lonloop;

                                    bool sea = z <= 0;

                                    double Angle2 = 0;
                                    double distance2 = 0;
                                    double Elevation2 = 0;

                                    GetCachedSRTMValues(pi, pi.rjParams.proximalInterpolation, lat, lon, hght, latInt, lonInt, latloop + ddy, lonloop + ddx, z, ref Angle2, ref Elevation2, ref distance2, ref latout1, ref lonout1);
                                    int Angle2Int = (int)(Angle2 * pi.rjParams.pixels);
                                    int Elevation2Int = (int)(Elevation2 * pi.rjParams.pixels + pi.rjParams.pixels * pi.rjParams.negativeRange);

                                    if ((ElevationInt > 0 || Elevation2Int > 0) && distance2 > pi.rjParams.minDistance && distance > pi.rjParams.minDistance)
                                    {
                                        hv.setHVContext(HorizonVector.HorizonSource.SRTMProximal, prevAngle, prevElevation, prevy, prevx, prevDistance, z, Angle2, Elevation2, latout1, lonout1, distance2, z);
                                        //WrappedDrawLine(steepest, null, sea, profGraph, prevAngleInt, prevElevationInt,prevDistance, Angle2Int, Elevation2Int, distance2, z, hv);
                                        pi.WrappedDrawLineDouble(pi.myBitmap, steepest, null, sea, profGraph, prevAngle, prevElevation, prevDistance, Angle2, Elevation2, distance2, z, hv);
                                    }

                                    prevy = latout1;
                                    prevx = lonout1;

                                    prevAngle = Angle2;
                                    prevAngleInt = Angle2Int;
                                    prevElevation = Elevation2;
                                    prevDistance = distance2;
                                }
                            }
                            else
                            {
                                for (int i = 0; i < max_y_loops; i++)
                                {
                                    int latIndx = latloop;
                                    int lonIndx = lonloop;

                                    switch (i)
                                    {
                                        case 0:
                                            latIndx += 1;
                                            break;
                                        case 1:
                                            lonIndx += 1;
                                            break;
                                        case 2:
                                            latIndx += 1;
                                            lonIndx += 1;
                                            break;
                                        case 3:
                                            GetCachedSRTMValues(pi, pi.rjParams.proximalInterpolation, lat, lon, hght, latInt, lonInt, latloop + 1, lonloop, z, ref Angle, ref Elevation, ref distance, ref latout1, ref lonout1);
                                            z = heightData[latloop + 1][lonloop];
                                            ElevationInt = (int)(Elevation * pi.rjParams.pixels + pi.rjParams.pixels * pi.rjParams.negativeRange);
                                            AngleInt = (int)(Angle * pi.rjParams.pixels);
                                            lonIndx += 1;
                                            break;
                                    }

                                    z2 = heightData[latIndx][lonIndx];

                                    // If height is below -100 then don't draw...
                                    if (z2 < -100)
                                    {
                                        continue;
                                    }

                                    bool sea = z <= 0 && z2 <= 0;

                                    double Angle2 = 0;
                                    double distance2 = 0;
                                    double Elevation2 = 0;

                                    GetCachedSRTMValues(pi, pi.rjParams.proximalInterpolation, lat, lon, hght, latInt, lonInt, latIndx, lonIndx, z2, ref Angle2, ref Elevation2, ref distance2, ref latout2, ref lonout2);
                                    int Angle2Int = (int)(Angle2 * pi.rjParams.pixels);
                                    int Elevation2Int = (int)(Elevation2 * pi.rjParams.pixels + pi.rjParams.pixels * pi.rjParams.negativeRange);
                                    if (ElevationInt > 0 || Elevation2Int > 0)
                                    {
                                        hv.setHVContext(HorizonVector.HorizonSource.SRTMLinear, Angle, Elevation, latout1, lonout1, distance, z, Angle2, Elevation2, latout2, lonout2, distance2, z2);
                                        pi.WrappedDrawLineDouble(pi.myBitmap, steepest, null, sea, profGraph, Angle, Elevation, distance, Angle2, Elevation2, distance2, z, hv);
                                    }
                                }
                            }

                            countSRTMPts++;
                        }
                    }
                }
            }
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


        public bool CanSubTileBeSeen(PaintImage pi, Int16[][] heightData, double lat, double lon, double hght, int latInt, int lonInt, int SubTileLat, int SubTileLon)
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

            double dip = PaintImage.CalculateDip(minDist);
            double Elevation = Math.Atan((maxZ - hght - dip) / minDist) * 180 / Math.PI;


            // Is it below the distant sea horizon - works everywhere apart from dead sea etc.
            if (minDist > sh.distance && Elevation < sh.elevation)
            {
                return false;
            }

            int IntElevation = (int)(Elevation * pi.rjParams.pixels + pi.rjParams.pixels * pi.rjParams.negativeRange);

            if (IntElevation >= 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }




        //// We cache the SRTM data to save time...
        double[][] SRTMangleMemCache = null;
        double[][] SRTMelevationMemCache = null;
        double[][] SRTMdistanceMemCache = null;
        int[] SRTMMemCacheValues = null;
        int SRTMcacheOffset = 0;
        bool SRTMcacheDirection = false;

        static double totalSRTMCalls = 0;
        static double totalSRTMHits = 0;
        static double SRTMcacheHitRatio = 0;

        public const int SRTMCacheMiss = -9999;


        public void SetSRTMCacheHint(bool horz)
        {
            SRTMcacheDirection = horz;

            // Initialise the memory SRTM cache.
            if (SRTMdistanceMemCache == null)
            {
                SRTMMemCacheValues = new int[2];
                SRTMangleMemCache = new double[2][];
                SRTMelevationMemCache = new double[2][];
                SRTMdistanceMemCache = new double[2][];

                for (int i = 0; i < SRTMMemCacheValues.Length; i++)
                {
                    SRTMcacheOffset = 0;
                    SRTMMemCacheValues[i] = SRTMCacheMiss;
                    int siz = ZipMapDataHandler.gridCount;
                    SRTMangleMemCache[i] = new double[siz];
                    SRTMelevationMemCache[i] = new double[siz];
                    SRTMdistanceMemCache[i] = new double[siz];
                }
            }

            SRTMcacheOffset = 0;

            // Reset the cache line...
            for (int oddeven = 0; oddeven < 2; oddeven++)
            {
                SRTMMemCacheValues[oddeven] = SRTMCacheMiss;
                for (int c = 0; c < SRTMangleMemCache[oddeven].Length; c++)
                {
                    SRTMangleMemCache[oddeven][c] = SRTMCacheMiss;
                }
            }
        }




        public bool GetCachedSRTMValues(PaintImage pi, bool proximalInterpolation, double lat, double lon, double hght, int latInt, int lonInt, int latloop, int lonloop, double z, ref double angle, ref double elevation, ref double distance, ref double latout, ref double lonout)
        {
            totalSRTMCalls++;
            int oddeven;
            int cacheIndex;
            int cacheLineValue;

            latout = (double)latInt + (double)(latloop) / ZipMapDataHandler.gridCount;
            lonout = (double)lonInt + (double)(lonloop) / ZipMapDataHandler.gridCount;

            if (proximalInterpolation)
            {
                latout -= PaintImage.halfSecond;
                lonout -= PaintImage.halfSecond;
            }

            if (SRTMcacheDirection)
            {
                cacheLineValue = latloop;
                oddeven = latloop % 2;
                cacheIndex = lonloop - SRTMcacheOffset;
            }
            else
            {
                cacheLineValue = lonloop;
                oddeven = lonloop % 2;
                cacheIndex = latloop - SRTMcacheOffset;
            }


            // Have we a cache hit or not?
            if (SRTMMemCacheValues[oddeven] == cacheLineValue && cacheIndex >= 0 && cacheIndex < SRTMangleMemCache[oddeven].Length)
            {
                // Do we have a cache hit or not?
                if (SRTMangleMemCache[oddeven][cacheIndex] != SRTMCacheMiss)
                {
                    angle = SRTMangleMemCache[oddeven][cacheIndex];
                    distance = SRTMdistanceMemCache[oddeven][cacheIndex];

                    if (proximalInterpolation)
                    {
                        double tmpdip = PaintImage.CalculateDip(distance);
                        elevation = Math.Atan((z - hght - tmpdip) / distance) * 180 / Math.PI;
                    }
                    else
                    {
                        elevation = SRTMelevationMemCache[oddeven][cacheIndex];
                    }

                    totalSRTMHits++;
                    SRTMcacheHitRatio = totalSRTMHits / totalSRTMCalls * 100;
                    return true;
                }
            }
            else
            {
                // Reset the cache line...
                SRTMMemCacheValues[oddeven] = cacheLineValue;
                for (int c = 0; c < SRTMangleMemCache[oddeven].Length; c++)
                {
                    SRTMangleMemCache[oddeven][c] = SRTMCacheMiss;
                }
            }


            // Get the distance and bearing
            double Ddistance = LatLonConversions.distHaversine(lat, lon, latout, lonout);
            double Dangle = LatLonConversions.bearing(lat, lon, latout, lonout) * 180 / Math.PI;

            double dip = PaintImage.CalculateDip(Ddistance);
            double Delevation = Math.Atan((z - hght - dip) / Ddistance) * 180 / Math.PI;

            angle = Dangle;
            elevation = Delevation;
            distance = Ddistance;

            // Have in the cache for later
            if (cacheIndex >= 0 && cacheIndex < SRTMangleMemCache[oddeven].Length)
            {
                SRTMangleMemCache[oddeven][cacheIndex] = angle;
                SRTMelevationMemCache[oddeven][cacheIndex] = elevation;
                SRTMdistanceMemCache[oddeven][cacheIndex] = distance;
            }

            SRTMcacheHitRatio = totalSRTMHits / totalSRTMCalls * 100;
            return true;
        }




    }
}
