using Proj4Net.Core;
using System;
using System.Collections.Generic;
//using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using PanaGraph;


namespace OpenPanoramaLib
{
    internal class PaintLidar
    {
        //// We cache the LIDAR data to save time...
        double[][] LIDARangleMemCache = null;
        double[][] LIDARelevationMemCache = null;
        double[][] LIDARdistanceMemCache = null;
        LatLon_OsGridRef[][] LIDARllCache = null;
        ProjCoordinate[][] NewLIDARllCache = null;
        int[] LIDARMemCacheValues = null;
        int LIDARcacheOffset = 0;
        bool LIDARcacheDirection = false;

        static double totalLIDARCalls = 0;
        static double totalLIDARHits = 0;
        static double LIDARcacheHitRatio = 0;



        public void NewPaintLIDAR(PaintImage pi, double lat, double lon, double hght, countryEnum eCountry)
        {
            if (!pi.rjParams.drwLIDAR)
            {
                return;
            }

            int LIDAR_CellSize = 1;
            if (eCountry == countryEnum.no)
            {
                LIDAR_CellSize = 10;
            }

            Console.WriteLine("PaintLIDAR " + lat + ", " + lon);

            ProjCoordinate ll = new ProjCoordinate(lon, lat);
            ProjCoordinate ne = new ProjCoordinate();
            pi.rjParams.FromLLtrans.Transform(ll, ne);

            Graphics profGraph = Graphics.FromImage(pi.myBitmap);

            // Drawing Vertically First...
            SetCacheHint(pi, lat, lon, false, eCountry);

            int MinDist = 50;


            for (int quad = 0; quad < 4; quad++)
            {
                int dx = 1;
                int dy = 1;
                if ((quad & 1) != 0)
                {
                    dx = -1;
                }
                if (quad >= 2)
                {
                    dy = -1;
                }
                // Draw from 50m outwards...
                for (int x = 0; x < pi.rjParams.lidarrange; x += LIDAR_CellSize)
                {
                    if (x % 1000 == 0)
                    {
                        Console.WriteLine("LIDAR Quadrant " + (quad + 1) + " Distance " + x / 1000 + "km");
                        pi.UpdateWorkInProgress();
                    }

                    for (int y = 0; y < pi.rjParams.lidarrange; y += LIDAR_CellSize)
                    {
                        if (x > MinDist || y > MinDist)
                        {
                            if (!NewDrawLIDAR(pi, profGraph, ne.X, ne.Y, lat, lon, hght, (int)(ne.X + x * dx), (int)(ne.Y + y * dy), eCountry, pi.rjParams))
                            {
                                pi.rjParams.lidarrange = Math.Min(pi.rjParams.lidarrange, (int)Math.Sqrt(x * x + y * y) - 1);
                                Console.WriteLine("Dropped LIDAR Range to " + pi.rjParams.lidarrange);
                            };
                        }
                    }
                }
            }

            profGraph.me.Flush();
        }


        public bool NewDrawLIDAR(PaintImage pi, Graphics profGraph, double orgxll, double orgyll, double lat, double lon, double hght, int xll, int yll, countryEnum eCountry, RunJobParams rjParams)
        {
            HorizonVector hv = new HorizonVector();

            // We do some approximations to avoid a full expensive Lat Lon calculation later...
            double approxDist = Math.Sqrt((orgxll - xll) * (orgxll - xll) + (orgyll - yll) * (orgyll - yll));
            if (approxDist >= rjParams.lidarrange || approxDist < rjParams.minDistance)
            {
                return true;
            }

            double z = LidarBlockReader.GetHeight(rjParams.proximalInterpolation, xll, yll, true, eCountry, rjParams);
            int LIDAR_CellSize = (int)LidarBlockReader.GetLidarRes();
            double deltadist = LIDAR_CellSize;
            double deltadistdiag = Math.Sqrt(LIDAR_CellSize * LIDAR_CellSize * 2);
            double steepest = 99999;

            if (z == LidarBlock.NODATA_const)
            {
                return false;
            }


            // Otherwise just draw the other two lines...
            int max_y_loops = 4;

            double[] zs = new double[max_y_loops];

            // Get the max gradient for the point.
            for (int i = 0; i < max_y_loops; i++)
            {
                int yIndx = yll;
                int xIndx = xll;

                double dist = 0;

                switch (i)
                {
                    case 0:
                        yIndx += LIDAR_CellSize;
                        dist = deltadist;
                        break;
                    case 1:
                        dist = deltadist;
                        xIndx += LIDAR_CellSize;
                        break;
                    case 2:
                        dist = deltadistdiag;
                        yIndx += LIDAR_CellSize;
                        xIndx += LIDAR_CellSize;
                        break;
                    case 3:
                        dist = deltadistdiag;
                        yIndx -= LIDAR_CellSize;
                        xIndx += LIDAR_CellSize;
                        break;
                }

                zs[i] = LidarBlockReader.GetHeight(rjParams.proximalInterpolation, xIndx, yIndx, true, eCountry, rjParams);
                if (zs[i] == LidarBlock.NODATA_const)
                {
                    return false;
                }

                double steepness = 99999;
                double dz = z - zs[i];
                if (dz < 0) dz = -dz;

                if (dz > 0)
                {
                    steepness = dist / dz;
                }
                if (steepness < steepest)
                {
                    steepest = steepness;
                }
            }


            double angle = -1;
            double elevation = -1;
            double distance = -1;
            ProjCoordinate ll = null;

            NewGetCachedLIDARValues(pi, lat, lon, hght, xll, yll, z, ref angle, ref elevation, ref distance, ref ll, LIDAR_CellSize);

            bool sea = z <= 0;


            // Get the max gradient for the point.
            for (int i = 0; i < max_y_loops; i++)
            {
                int yIndx = yll;
                int xIndx = xll;

                double angle2 = -1;
                double elevation2 = -1;
                double distance2 = -1;

                switch (i)
                {
                    case 0:
                        yIndx += LIDAR_CellSize;
                        break;
                    case 1:
                        xIndx += LIDAR_CellSize;
                        break;
                    case 2:
                        yIndx += LIDAR_CellSize;
                        xIndx += LIDAR_CellSize;
                        break;
                    case 3:
                        yIndx -= LIDAR_CellSize;
                        xIndx += LIDAR_CellSize;
                        break;
                }

                ProjCoordinate ll2 = null;
                NewGetCachedLIDARValues(pi, lat, lon, hght, xIndx, yIndx, zs[i], ref angle2, ref elevation2, ref distance2, ref ll2, LIDAR_CellSize);

                hv.setHVContext(HorizonVector.HorizonSource.LIDAR, angle, elevation, ll.Y, ll.X, distance, z, angle2, elevation2, ll2.Y, ll2.X, distance2, zs[i]);
                int angleInt = (int)(angle * rjParams.pixels);
                int elevationInt = (int)(elevation * rjParams.pixels + rjParams.pixels * rjParams.negativeRange);
                int angle2Int = (int)(angle2 * rjParams.pixels);
                int elevation2Int = (int)(elevation2 * rjParams.pixels + rjParams.pixels * rjParams.negativeRange);

                //WrappedDrawLine(steepest, null, sea, profGraph, angleInt, elevationInt, distance, angle2Int, elevation2Int, distance2, z, hv);
                pi.WrappedDrawLineDouble(steepest, null, sea, profGraph, angle, elevation, distance, angle2, elevation2, distance2, z, hv);
            }

            return true;
        }



        public void SetCacheHint(PaintImage pi, double lat, double lon, bool horz, countryEnum eCountry)
        {
            LIDARcacheDirection = horz;

            // Initialise the memory LIDAR cache.
            if (LIDARdistanceMemCache == null)
            {
                LIDARMemCacheValues = new int[2];
                LIDARangleMemCache = new double[2][];
                LIDARelevationMemCache = new double[2][];
                LIDARdistanceMemCache = new double[2][];
                LIDARllCache = new LatLon_OsGridRef[2][];
                NewLIDARllCache = new ProjCoordinate[2][];

                for (int i = 0; i < LIDARMemCacheValues.Length; i++)
                {
                    LIDARcacheOffset = (int)(pi.rjParams.lidarrange + 2);
                    LIDARMemCacheValues[i] = -9999;
                    int siz = LIDARcacheOffset * 2;
                    LIDARangleMemCache[i] = new double[siz];
                    LIDARelevationMemCache[i] = new double[siz];
                    LIDARdistanceMemCache[i] = new double[siz];
                    LIDARllCache[i] = new LatLon_OsGridRef[siz];
                    NewLIDARllCache[i] = new ProjCoordinate[siz];
                }
            }


            Coordinate centrell = new Coordinate(lon, lat);
            ProjCoordinate ne = new ProjCoordinate();
            pi.rjParams.FromLLtrans.Transform(centrell, ne);

            if (LIDARcacheDirection)
            {
                LIDARcacheOffset = (int)ne.Y - LIDARcacheOffset;
            }
            else
            {
                LIDARcacheOffset = (int)ne.Y - LIDARcacheOffset;
            }

            // Reset the cache line...
            for (int oddeven = 0; oddeven < 2; oddeven++)
            {
                LIDARMemCacheValues[oddeven] = -9999;
                for (int c = 0; c < LIDARangleMemCache[oddeven].Length; c++)
                {
                    LIDARllCache[oddeven][c] = null;
                    NewLIDARllCache[oddeven][c] = null;
                }
            }
        }



        public bool NewGetCachedLIDARValues(PaintImage pi, double lat, double lon, double hght, int xll, int yll, double z, ref double angle, ref double elevation, ref double distance, ref ProjCoordinate ll, int LIDAR_CellSize)
        {
            totalLIDARCalls++;
            int oddeven;
            int cacheIndex;
            int cacheLineValue;

            if (LIDARcacheDirection)
            {
                cacheLineValue = yll;
                oddeven = (yll / LIDAR_CellSize) % 2;
                cacheIndex = (xll - LIDARcacheOffset) / LIDAR_CellSize;
            }
            else
            {
                cacheLineValue = xll;
                oddeven = (xll / LIDAR_CellSize) % 2;
                cacheIndex = (yll - LIDARcacheOffset) / LIDAR_CellSize;
            }

            if (oddeven < 0)
            {
                oddeven = -oddeven;
            }

            try
            {
                // Have we a cache hit or not?
                if (LIDARMemCacheValues[oddeven] == cacheLineValue && cacheIndex >= 0 && cacheIndex < NewLIDARllCache[oddeven].Length)
                {
                    // Do we have a cache hit or not?
                    if (NewLIDARllCache[oddeven][cacheIndex] != null)
                    {
                        angle = LIDARangleMemCache[oddeven][cacheIndex];
                        elevation = LIDARelevationMemCache[oddeven][cacheIndex];
                        distance = LIDARdistanceMemCache[oddeven][cacheIndex];
                        ll = NewLIDARllCache[oddeven][cacheIndex];
                        totalLIDARHits++;
                        LIDARcacheHitRatio = totalLIDARHits / totalLIDARCalls * 100;
                        return true;
                    }
                }
                else
                {
                    // Reset the cache line...
                    LIDARMemCacheValues[oddeven] = cacheLineValue;
                    for (int c = 0; c < LIDARangleMemCache[oddeven].Length; c++)
                    {
                        NewLIDARllCache[oddeven][c] = null;
                    }
                }
            }
            catch (Exception ex)
            {
                string sgsgsg = "";
            }

            Coordinate src = new Coordinate(xll + 0.5, yll + 0.5);
            ll = new ProjCoordinate();
            pi.rjParams.ToLLTrans.Transform(src, ll);

            // Get the distance and bearing
            double Ddistance = LatLonConversions.distHaversine(lat, lon, ll.Y, ll.X);
            double Dangle = LatLonConversions.bearing(lat, lon, ll.Y, ll.X) * 180 / Math.PI; // * rjParams.pixels;

            double dip = PaintImage.CalculateDip(Ddistance);
            double Delevation = Math.Atan((z - hght - dip) / Ddistance) * 180 / Math.PI; // * rjParams.pixels + rjParams.pixels * rjParams.negativeRange;

            angle = Dangle;
            elevation = Delevation;
            distance = Ddistance;

            // Have in the cache for later
            if (cacheIndex >= 0 && cacheIndex < LIDARllCache[oddeven].Length)
            {
                LIDARangleMemCache[oddeven][cacheIndex] = angle;
                LIDARelevationMemCache[oddeven][cacheIndex] = elevation;
                LIDARdistanceMemCache[oddeven][cacheIndex] = distance;
                NewLIDARllCache[oddeven][cacheIndex] = ll;
            }

            LIDARcacheHitRatio = totalLIDARHits / totalLIDARCalls * 100;
            return true;
        }
    }
}
