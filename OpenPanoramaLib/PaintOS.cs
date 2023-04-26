using System;
using System.Collections.Generic;
//using System.Drawing;
//using System.Drawing;
//using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

using PanaGraph;

namespace OpenPanoramaLib
{
    public class PaintOS
    {
        int[] CrackedHeights;
        int[][] CrackedXs;
        int[][] CrackedYs;
        int CrackedCountourLength = 0;

        public void CrackOSContours(XDocument xdoc)
        {
            XNamespace xsiNameSpace = "http://www.w3.org/2001/XMLSchema-instance";
            XNamespace osNameSpace = "http://namespaces.ordnancesurvey.co.uk/elevation/contours/v1.0";
            XNamespace gmlNameSpace = "http://www.opengis.net/gml/3.2";
            char[] splitter = { ' ' };

            var allContourLines = xdoc.Descendants(osNameSpace + "ContourLine");
            var allSeaContourLines = xdoc.Descendants(osNameSpace + "LandWaterBoundary");

            XElement[] contours = allContourLines.ToArray();
            XElement[] seacontours = allSeaContourLines.ToArray();

            CrackedCountourLength = contours.Length;

            CrackedHeights = new int[contours.Length + seacontours.Length];
            CrackedXs = new int[contours.Length + seacontours.Length][];
            CrackedYs = new int[contours.Length + seacontours.Length][];

            ContourHint chint = new ContourHint();
            chint.valid = false;

            // Crack the entire OS block in one go...
            for (int i = 0; i < contours.Length; i++)
            {
                var posList = contours[i].Element(osNameSpace + "geometry").Value.ToString();
                CrackedHeights[i] = (int)Convert.ToDouble(contours[i].Element(osNameSpace + "propertyValue").Value.ToString());
                string[] ptStrs = posList.Split(splitter);
                CrackedXs[i] = new int[ptStrs.Length / 2];
                CrackedYs[i] = new int[ptStrs.Length / 2];

                for (int j = 0; j < CrackedXs[i].Length; j++)
                {
                    CrackedXs[i][j] = (int)Convert.ToDouble(ptStrs[j * 2]);
                    CrackedYs[i][j] = (int)Convert.ToDouble(ptStrs[j * 2 + 1]);
                }
            }

            // Add in the sea contours to the heights...
            for (int i = 0; i < seacontours.Length; i++)
            {
                var posList = seacontours[i].Element(osNameSpace + "geometry").Value.ToString();
                CrackedHeights[i + contours.Length] = 0;
                string[] ptStrs = posList.Split(splitter);
                CrackedXs[i + contours.Length] = new int[ptStrs.Length / 2];
                CrackedYs[i + contours.Length] = new int[ptStrs.Length / 2];

                for (int j = 0; j < CrackedXs[i + contours.Length].Length; j++)
                {
                    CrackedXs[i + contours.Length][j] = (int)Convert.ToDouble(ptStrs[j * 2]);
                    CrackedYs[i + contours.Length][j] = (int)Convert.ToDouble(ptStrs[j * 2 + 1]);
                }
            }
        }


        public void PaintSpotHeightsAndContours(PaintImage pi, double lat, double lon, double hght, countryEnum eCountry)
        {
            if (!pi.rjParams.drwSpots && !pi.rjParams.drwContours)
            {
                return;
            }

            Console.WriteLine("PaintSpotHeightsAndContours " + lat + ", " + lon + " drawContours " + pi.rjParams.drwContours + " drawSpots " + pi.rjParams.drwSpots);


            // First get the starting position in NE units...
            LatLon_OsGridRef ll = new LatLon_OsGridRef(lat, lon, 0);
            OsGridRef ne = ll.toGrid(pi.rjParams.eCountry);
            if (ne == null)
            {
                return;
            }

            Graphics profGraph = Graphics.FromImage(pi.myBitmap);

            int GridSiz = 10000;
            int max_rad_squares = (int)((pi.rjParams.maxDistance + GridSiz - 1) / GridSiz);

            LatLon_OsGridRef ll2 = new LatLon_OsGridRef(lat, lon, 0);
            OsGridRef ne2 = ll.toGrid(pi.rjParams.eCountry);

            for (int m = max_rad_squares; m >= 0; m--)
            {
                for (int x = -m; x <= m; x++)
                {
                    for (int y = -m; y <= m; y++)
                    {
                        if (x == -m || x == m || y == -m || y == m)
                        {
                            ne2.east = ne.east + x * GridSiz;
                            ne2.north = ne.north + y * GridSiz;

                            XDocument xdoc = GetGridXDoc(ne2);
                            if (xdoc != null)
                            {
                                if (pi.rjParams.drwContours)
                                {
                                    DrawContours(pi, profGraph, xdoc, lat, lon, hght, eCountry);
                                    pi.UpdateWorkInProgress();
                                }

                                //if (pi.rjParams.drwSpots)
                                //{
                                    DrawSpotHeights(pi, profGraph, xdoc, lat, lon, hght, eCountry, pi.rjParams.drwSpots);
                                    pi.UpdateWorkInProgress();
                                //}
                            }
                        }
                    }
                }
            }

            profGraph.me.Flush();
        }


        public void DrawContours(PaintImage pi, Graphics profGraph, XDocument xdoc, double lat, double lon, double hght, countryEnum eCountry)
        {
            CrackOSContours(xdoc);

            //XNamespace xsiNameSpace = "http://www.w3.org/2001/XMLSchema-instance";
            XNamespace osNameSpace = "http://namespaces.ordnancesurvey.co.uk/elevation/contours/v1.0";
            //XNamespace gmlNameSpace = "http://www.opengis.net/gml/3.2";
            char[] splitter = { ' ' };

            //var allContourLines = xdoc.Descendants(osNameSpace + "ContourLine");
            var allSeaContourLines = xdoc.Descendants(osNameSpace + "LandWaterBoundary");

            //SeaHorizon sh = SeaHorizon.GetHorizonForHeight((int)hght);
            HorizonVector hv = new HorizonVector();

            double oldAzimuth = 0;
            int oldAzimuthInt = 0;
            double oldElevation = 0;
            int oldElevationInt = 0;
            double oldDistance = 0;
            double oldGradient = 0;
            double oldHeight = 0;

            LatLon_OsGridRef oldll = null;

            foreach (XElement contour in allSeaContourLines)
            {
                var posList = contour.Element(osNameSpace + "geometry").Value.ToString();
                var h = contour.Element(osNameSpace + "propertyValue").Value.ToString();
                double z = (int)Convert.ToDouble(h);

                string[] ptStrs = posList.Split(splitter);

                for (int i = 0; i < ptStrs.Length; i += 2)
                {
                    double x = Convert.ToDouble(ptStrs[i]);
                    double y = Convert.ToDouble(ptStrs[i + 1]);

                    OsGridRef osgr = new OsGridRef(x, y);
                    var ll = osgr.toLatLon(eCountry);

                    double Azimuth = LatLonConversions.bearing(lat, lon, ll.lat, ll.lon) * 180 / Math.PI;
                    double distance = LatLonConversions.distHaversine(lat, lon, ll.lat, ll.lon);

                    double dip = PaintImage.CalculateDip(distance);
                    double Elevation = Math.Atan((z - hght - dip) / distance) * 180 / Math.PI;

                    int AngleInt = (int)(Azimuth * pi.rjParams.pixels);
                    int ElevationInt = (int)(Elevation * pi.rjParams.pixels + pi.rjParams.pixels * pi.rjParams.negativeRange);

                    if (distance >= pi.rjParams.minDistance && oldDistance >= pi.rjParams.minDistance && i > 0)
                    {
                        hv.setHVContext(HorizonVector.HorizonSource.OSSeaContour, Azimuth, Elevation, ll.lat, ll.lon, distance, z, oldAzimuth, oldElevation, oldll.lat, oldll.lon, oldDistance, oldHeight);
                        //WrappedDrawLine(99999, null, true, profGraph, AngleInt, ElevationInt, distance, oldAzimuthInt, oldElevationInt, oldDistance, z, hv );
                        pi.WrappedDrawLineDouble(99999, null, true, profGraph, Azimuth, Elevation, distance, oldAzimuth, oldElevation, oldDistance, z, hv);
                    }

                    oldAzimuth = Azimuth;
                    oldAzimuthInt = AngleInt;
                    oldElevation = Elevation;
                    oldElevationInt = ElevationInt;
                    oldDistance = distance;
                    oldll = ll;
                    oldHeight = z;
                }
            }


            double Gradient = 0;


            //XElement[] contours = null;

            //contours = allContourLines.ToArray();
            //XElement[] seacontours = allSeaContourLines.ToArray();

            //int[] heights = new int[contours.Length + seacontours.Length];
            //int[][] xs = new int[contours.Length + seacontours.Length][];
            //int[][] ys = new int[contours.Length + seacontours.Length][];

            ContourHint chint = new ContourHint();
            chint.valid = false;

            //// Crack the entire OS block in one go...
            //for (int i = 0; i < contours.Length; i++)
            //{
            //    var posList = contours[i].Element(osNameSpace + "geometry").Value.ToString();
            //    heights[i] = (int)Convert.ToDouble(contours[i].Element(osNameSpace + "propertyValue").Value.ToString());
            //    string[] ptStrs = posList.Split(splitter);
            //    xs[i] = new int[ptStrs.Length / 2];
            //    ys[i] = new int[ptStrs.Length / 2];

            //    for (int j = 0; j < xs[i].Length; j++)
            //    {
            //        xs[i][j] = (int)Convert.ToDouble(ptStrs[j * 2]);
            //        ys[i][j] = (int)Convert.ToDouble(ptStrs[j * 2 + 1]);
            //    }
            //}

            //// Add in the sea contours to the heights...
            //for (int i = 0; i < seacontours.Length; i++)
            //{
            //    var posList = seacontours[i].Element(osNameSpace + "geometry").Value.ToString();
            //    heights[i + contours.Length] = 0;
            //    string[] ptStrs = posList.Split(splitter);
            //    xs[i + contours.Length] = new int[ptStrs.Length / 2];
            //    ys[i + contours.Length] = new int[ptStrs.Length / 2];

            //    for (int j = 0; j < xs[i + contours.Length].Length; j++)
            //    {
            //        xs[i + contours.Length][j] = (int)Convert.ToDouble(ptStrs[j * 2]);
            //        ys[i + contours.Length][j] = (int)Convert.ToDouble(ptStrs[j * 2 + 1]);
            //    }
            //}


            for (int hi = 0; hi < CrackedHeights.Length; hi++)
            {
                //string infostuff = "";

                double z = CrackedHeights[hi];

                // If this is sea then we have already drawn it so skip - don't draw it again...
                if (hi >= CrackedCountourLength)
                {
                    continue;
                }

                chint.valid = false;

                for (int i = 0; i < CrackedXs[hi].Length; i += 1)
                {
                    int nearest_X = 0;
                    int nearest_y = 0;
                    int nearest_z = 0;


                    double x = CrackedXs[hi][i];
                    double y = CrackedYs[hi][i];

                    OsGridRef osgr = new OsGridRef(x, y);
                    var ll = osgr.toLatLon(eCountry);

                    double Azimuth = LatLonConversions.bearing(lat, lon, ll.lat, ll.lon) * 180 / Math.PI;
                    double distance = LatLonConversions.distHaversine(lat, lon, ll.lat, ll.lon);

                    double dip = PaintImage.CalculateDip(distance);
                    double Elevation = Math.Atan((z - hght - dip) / distance) * 180 / Math.PI;

                    bool drawMe = true;

                    int AzimuthInt = (int)(Azimuth * pi.rjParams.pixels);
                    int ElevationInt = (int)(Elevation * pi.rjParams.pixels + pi.rjParams.pixels * pi.rjParams.negativeRange);


                    // If LIDAR Enabled then don't draw within the LIDAR range.
                    if ((pi.rjParams.drwLIDAR && distance < pi.rjParams.lidarrange - 100) && !pi.rjParams.drwAllContours)
                    {
                        drawMe = false;
                    }

                    // Minor optimisation - don't bother with contours below the horizon... and don't draw closer than rjParams.minDistance (70) metres...
                    if (ElevationInt > 0 && distance >= pi.rjParams.minDistance && drawMe)
                    {
                        double min_dist = 0;
                        bool nearest = GetNearestLowerPoint(CrackedHeights, CrackedXs, CrackedYs, CrackedHeights[hi], CrackedXs[hi][i], CrackedYs[hi][i], ref nearest_X, ref nearest_y, ref nearest_z, ref min_dist, chint);
                        if (nearest)
                        {
                            OsGridRef osgr2 = new OsGridRef(nearest_X, nearest_y);
                            var ll2 = osgr2.toLatLon(eCountry);

                            double Angle2 = LatLonConversions.bearing(lat, lon, ll2.lat, ll2.lon) * 180 / Math.PI;
                            double distance2 = LatLonConversions.distHaversine(lat, lon, ll2.lat, ll2.lon);

                            double dip2 = PaintImage.CalculateDip(distance2);
                            double Elevation2 = Math.Atan((nearest_z - hght - dip2) / distance2) * 180 / Math.PI;

                            int Angle2Int = (int)(Angle2 * pi.rjParams.pixels);
                            int Elevation2Int = (int)(Elevation2 * pi.rjParams.pixels + pi.rjParams.pixels * pi.rjParams.negativeRange);

                            Gradient = min_dist / (CrackedHeights[hi] - nearest_z);
                            if (distance >= pi.rjParams.minDistance && distance2 >= pi.rjParams.minDistance && !pi.rjParams.proximalInterpolation)
                            {
                                hv.setHVContext(HorizonVector.HorizonSource.OSContourDiagonal, Azimuth, Elevation, ll.lat, ll.lon, distance, z, Angle2, Elevation2, ll2.lat, ll2.lon, distance2, nearest_z);
                                //WrappedDrawLine(Gradient, null, false, profGraph, AzimuthInt, ElevationInt, distance, Angle2Int, Elevation2Int, distance2, z, hv);
                                pi.WrappedDrawLineDouble(Gradient, null, false, profGraph, Azimuth, Elevation, distance, Angle2, Elevation2, distance2, z, hv);
                            }
                        }
                        else
                        {
                            //infostuff += " GetNearestLowerPoint false " + heights[hi] + " x " + xs[hi][i] +  " y " + ys[hi][i];
                        }
                    }


                    // Don't draw closer than rjParams.minDistance (70) metres...
                    if (distance >= pi.rjParams.minDistance && drawMe && i > 0)
                    {
                        if (i > 0 && (ElevationInt > 0 || oldElevationInt > 0))
                        {
                            double avGrad = 0;
                            bool ngrdSet = false;
                            if (Gradient > 0)
                            {
                                avGrad = Gradient;
                                ngrdSet = true;
                            }
                            if (oldGradient > 0)
                            {
                                avGrad += oldGradient;
                                if (ngrdSet)
                                {
                                    avGrad = avGrad / 2;
                                }
                            }

                            hv.setHVContext(HorizonVector.HorizonSource.OSLandContour, Azimuth, Elevation, ll.lat, ll.lon, distance, z, oldAzimuth, oldElevation, oldll.lat, oldll.lon, oldDistance, oldHeight);
                            //WrappedDrawLine(avGrad, null, false, profGraph, AzimuthInt, ElevationInt, distance, oldAzimuthInt, oldElevationInt, oldDistance, z, hv);
                            pi.WrappedDrawLineDouble(avGrad, null, false, profGraph, Azimuth, Elevation, distance, oldAzimuth, oldElevation, oldDistance, z, hv);
                        }
                    }

                    oldAzimuth = Azimuth;
                    oldAzimuthInt = AzimuthInt;
                    oldElevation = Elevation;
                    oldElevationInt = ElevationInt;
                    oldDistance = distance;
                    oldGradient = Gradient;
                    oldHeight = z;
                    oldll = ll;
                }
            }
        }


        public XDocument GetGridXDoc(OsGridRef ne)
        {
            //string GridRef = OGBRect.NE2NGR(ne.east, ne.north);
            OsGridRef gr = new OsGridRef(ne.east, ne.north);
            string GridRef = gr.toString();

            ZipMapDataHandler zippy = new ZipMapDataHandler();

            return zippy.GetOSVectorData(GridRef, false);
        }




        static long totalCalls = 0;
        static long hintHandledCalls = 0;
        static double hintRatio = 0;


        bool GetNearestLowerPoint(int[] heights, int[][] xs, int[][] ys, int h, int search_x, int search_y, ref int nearest_X, ref int nearest_y, ref int nearest_z, ref double dist, ContourHint hint)
        {
            nearest_z = h - 10; // 10M contours - look for the nearest point 10m below the current point...

            totalCalls++;

            // Max distance we draw to a contour is 100m - for hints.
            int mindist = 100 * 100;
            bool foundMin = false;

            if (hint != null && hint.valid)
            {
                int searchRange = 30;

                if (hint.hi >= 0 && hint.hi < heights.Length)
                {
                    int tmpdist = (int)(hint.distance * hint.distance * 1.3);
                    if (tmpdist < mindist)
                    {
                        mindist = tmpdist;
                    }
                    hint.indx -= searchRange;
                    if (hint.indx < 0) hint.indx = 0;

                    for (int p = hint.indx; p < hint.indx + 2 * searchRange && p < xs[hint.hi].Length; p++)
                    {
                        int dx = xs[hint.hi][p] - search_x;
                        int dy = ys[hint.hi][p] - search_y;

                        int distsq = dx * dx + dy * dy;
                        if (distsq < mindist)
                        {
                            mindist = distsq;
                            foundMin = true;
                            nearest_X = xs[hint.hi][p];
                            nearest_y = ys[hint.hi][p];
                            hint.indx = p;
                            hint.distance = Math.Sqrt(mindist);
                        }
                    }
                }
            }

            if (!foundMin)
            {
                // Max distance we draw to a contour is 200m
                mindist = 200 * 200;
                // Go looking for the nearest contour point on the next lower contour...
                for (int i = 0; i < heights.Length; i++)
                {
                    // Found some contours at the correct height - linear search for nearest...
                    if (nearest_z == heights[i])
                    {
                        for (int p = 0; p < xs[i].Length; p++)
                        {
                            int dx = xs[i][p] - search_x;
                            int dy = ys[i][p] - search_y;

                            int distsq = dx * dx + dy * dy;
                            if (distsq < mindist)
                            {
                                mindist = distsq;
                                foundMin = true;
                                nearest_X = xs[i][p];
                                nearest_y = ys[i][p];

                                if (hint != null)
                                {
                                    hint.valid = true;
                                    hint.hi = i;
                                    hint.indx = p;
                                    hint.distance = Math.Sqrt(mindist);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                hintHandledCalls++;
            }

            hintRatio = (double)hintHandledCalls / totalCalls;

            dist = Math.Sqrt(mindist);
            return foundMin;
        }




        public void DrawSpotHeights(PaintImage pi, Graphics profGraph, XDocument xdoc, double lat, double lon, double hght, countryEnum eCountry, bool AddLabels)
        {
            XNamespace xsiNameSpace = "http://www.w3.org/2001/XMLSchema-instance";
            XNamespace osNameSpace = "http://namespaces.ordnancesurvey.co.uk/elevation/contours/v1.0";
            XNamespace gmlNameSpace = "http://www.opengis.net/gml/3.2";
            char[] splitter = { ' ' };

            var allSpotHeights = xdoc.Descendants(osNameSpace + "SpotHeight");

            // OGBLatLng ll;
            LatLon_OsGridRef ll;

            Pen pn = new Pen(PaintImage.colourSpotHeight, 1);
            Pen markerpn = new Pen(PaintImage.colourSpotHeight, 1);
            SolidBrush txtbrsh = new SolidBrush(PaintImage.colourSpotHeight);


            PointF[] poly = new PointF[3];
            for (int i = 0; i < poly.Length; i++)
            {
                poly[i] = new PointF();
            }

            ContourHint chint = new ContourHint();
            chint.valid = false;

            HorizonVector hv = new HorizonVector();

            for (int hi = 0; hi < CrackedHeights.Length; hi++)
            {
                //var loc = spot.Element(osNameSpace + "geometry").Value.ToString();
                //string h = spot.Element(osNameSpace + "propertyValue").Value.ToString();

                double z = CrackedHeights[hi];

                for (int i = 0; i < CrackedXs[hi].Length; i += 1)
                {
                    int nearest_X = 0;
                    int nearest_y = 0;
                    int nearest_z = 0;

                    double x = CrackedXs[hi][i];
                    double y = CrackedYs[hi][i];

                    OsGridRef osgr = new OsGridRef(x, y);
                    ll = osgr.toLatLon(eCountry);

                    double Azimuth = LatLonConversions.bearing(lat, lon, ll.lat, ll.lon) * 180 / Math.PI;
                    double distance = LatLonConversions.distHaversine(lat, lon, ll.lat, ll.lon);

                    double dip = PaintImage.CalculateDip(distance);
                    double Elevation = Math.Atan((z - hght - dip) / distance) * 180 / Math.PI;

                    chint.valid = false;


                    //string[] ptStrs = loc.Split(splitter);
                    //double x = Convert.ToDouble(ptStrs[0]);
                    //double y = Convert.ToDouble(ptStrs[1]);
                    //double z = Convert.ToDouble(h);

                    ////ll = OGBLatLng.NEtoLL(x, y);
                    //OsGridRef osgr = new OsGridRef(x, y);
                    //ll = osgr.toLatLon(eCountry);

                    //double Angle = LatLonConversions.bearing(lat, lon, ll.lat, ll.lon);
                    //double distance = LatLonConversions.distHaversine(lat, lon, ll.lat, ll.lon);

                    //double dip = PaintImage.CalculateDip(distance);
                    //Double Elevation = Math.Atan((z - hght - dip) / distance);

                    double Angle = Azimuth * pi.rjParams.pixels;
                    Elevation = Elevation * 180 / Math.PI * pi.rjParams.pixels + pi.rjParams.pixels * pi.rjParams.negativeRange;
                    int SpotSiz = 5;

                    // Can we see the spot or not?
                    if (Elevation > 0 &&
                        Angle > 0 && Angle < pi.ZBuffer.Count() &&
                        Elevation < pi.ZBuffer[0].Count() &&
                        pi.ZBuffer[(int)Angle][(int)Elevation] > distance)
                    {
                        // If LIDAR Enabled then don't draw within the LIDAR range.
                        if ((pi.rjParams.drwLIDAR && distance < pi.rjParams.lidarrange - 100) && !pi.rjParams.drwAllContours)
                        {
                        }
                        else
                        {
                            int ElevationInt = (int)(Elevation * pi.rjParams.pixels + pi.rjParams.pixels * pi.rjParams.negativeRange);

                            // Minor optimisation - don't bother with contours below the horizon... and don't draw closer than rjParams.minDistance (70) metres...
                            if (ElevationInt > 0 && distance >= pi.rjParams.minDistance)
                            {
                                double min_dist = 0;
                                bool nearest = GetNearestLowerPoint(CrackedHeights, CrackedXs, CrackedYs, CrackedHeights[hi], CrackedXs[hi][i], CrackedYs[hi][i], ref nearest_X, ref nearest_y, ref nearest_z, ref min_dist, chint);
                                if (nearest)
                                {
                                    OsGridRef osgr2 = new OsGridRef(nearest_X, nearest_y);
                                    var ll2 = osgr2.toLatLon(eCountry);

                                    double Angle2 = LatLonConversions.bearing(lat, lon, ll2.lat, ll2.lon) * 180 / Math.PI;
                                    double distance2 = LatLonConversions.distHaversine(lat, lon, ll2.lat, ll2.lon);

                                    double dip2 = PaintImage.CalculateDip(distance2);
                                    double Elevation2 = Math.Atan((nearest_z - hght - dip2) / distance2) * 180 / Math.PI;

                                    int Angle2Int = (int)(Angle2 * pi.rjParams.pixels);
                                    int Elevation2Int = (int)(Elevation2 * pi.rjParams.pixels + pi.rjParams.pixels * pi.rjParams.negativeRange);

                                    double Gradient = min_dist / (CrackedHeights[hi] - nearest_z);
                                    if (distance >= pi.rjParams.minDistance && distance2 >= pi.rjParams.minDistance && !pi.rjParams.proximalInterpolation)
                                    {
                                        hv.setHVContext(HorizonVector.HorizonSource.OSTrig, Azimuth, Elevation, ll.lat, ll.lon, distance, z, Angle2, Elevation2, ll2.lat, ll2.lon, distance2, nearest_z);
                                        //WrappedDrawLine(Gradient, null, false, profGraph, AzimuthInt, ElevationInt, distance, Angle2Int, Elevation2Int, distance2, z, hv);
                                        pi.WrappedDrawLineDouble(Gradient, null, false, profGraph, Azimuth, Elevation, distance, Angle2, Elevation2, distance2, z, hv);
                                    }
                                }
                                else
                                {
                                    //infostuff += " GetNearestLowerPoint false " + heights[hi] + " x " + xs[hi][i] +  " y " + ys[hi][i];
                                }
                            }
                        }

                        if (AddLabels)
                        {
                            profGraph.DrawLine(pn, (float)(Angle - SpotSiz), (float)(pi.lowest_elevation_height - Elevation), (float)(Angle + SpotSiz), (float)(pi.lowest_elevation_height - Elevation));
                            profGraph.DrawLine(pn, (float)Angle, (float)((pi.lowest_elevation_height - Elevation) - SpotSiz), (float)Angle, (float)((pi.lowest_elevation_height - Elevation) + SpotSiz));

                            // Add txt for spot heights above 1 degrees.
                            if (Elevation > 0 * pi.rjParams.pixels && pi.rjParams.drawLocations)
                            {
                                string val = (int) z + " " + ll.lat.ToString("0.00000") + ", " + ll.lon.ToString("0.00000");

                                StringFormat sf = new StringFormat();
                                sf.LineAlignment = StringAlignment.Near;
                                sf.Alignment = StringAlignment.Center;

                                int txtwidth = (int)pi.rjParams.pixels; // This should fit in about 1 degree of width...


                                int txtheight = 12;
                                int yy = (int)(pi.lowest_elevation_height - Elevation - txtheight + SpotSiz);
                                int xx = (int)(Angle - txtwidth / 2);
                                int start_yy = yy;

                                // Make sure the label is visible.
                                if (xx < 0) xx = 0;
                                if (xx + txtwidth >= pi.ImageRawWidth) xx = pi.ImageRawWidth - txtwidth;

                                for (int lx = 0; lx < txtwidth; lx++)
                                {
                                    if (pi.labelStack[xx + lx] - txtheight <= yy)
                                    {
                                        yy = pi.labelStack[xx + lx] - txtheight - 1;
                                    }
                                }

                                if (yy > start_yy - pi.rjParams.pixels)
                                {
                                    for (int lx = 0; lx < txtwidth; lx++)
                                    {
                                        pi.labelStack[xx + lx] = yy;
                                    }

                                    Rectangle rct = new Rectangle(xx, yy, txtwidth, txtheight + 1);
                                    profGraph.DrawString(val, new Font(FontFamily.GenericSansSerif, txtheight, FontStyle.Regular), txtbrsh, rct, sf);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
