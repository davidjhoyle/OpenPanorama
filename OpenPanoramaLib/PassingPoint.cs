using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenPanoramaLib
{
    public class PassingPoint
    {
        public HorizonVector vec;
        public Foresight.HeavenlyTraces ht;
        public SkyPos sp;
        public Foresight.TopCenBot tcb;

        public static void DumpPassingPoints(bool gpx, string site, double lat, double lon, string filename, List<PassingPoint> passingPoints)
        {
            Console.WriteLine("Create GPX Passing Points File " + filename);
            System.IO.StreamWriter outfile = new System.IO.StreamWriter(filename);

            if (gpx)
            {
                outfile.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"no\" ?>");
                outfile.WriteLine("<gpx>");

                outfile.WriteLine("<!--Centre Waypoint " + lat + " " + lon + "-->");
                outfile.WriteLine("<wpt lat=\"" + lat + "\" lon=\"" + lon + "\">");
                outfile.WriteLine("<name>" + site + " lat/lon " + lat + "," + lon + "</name>");
                outfile.WriteLine("</wpt>");
            }
            else
            {
                outfile.WriteLine("Number,Height,TopCentreBottom,Azimuth,Elevation,Latitude,Longitude");
            }

            for (int i = 0; i < passingPoints.Count; i++)
            {
                if (passingPoints[i] == null || passingPoints[i].vec == null || passingPoints[i].sp == null)
                {
                }
                else
                {
                    double deltaaz = passingPoints[i].vec.azimuth2 - passingPoints[i].vec.azimuth1;
                    if (deltaaz > 180) deltaaz -= 360;
                    if (deltaaz < -180) deltaaz += 360;

                    double deltaalt = passingPoints[i].vec.altitude2 - passingPoints[i].vec.altitude1;
                    double deltalat = passingPoints[i].vec.latitude2 - passingPoints[i].vec.latitude1;
                    double deltalon = passingPoints[i].vec.longitude2 - passingPoints[i].vec.longitude1;
                    double deltaheight = passingPoints[i].vec.height2 - passingPoints[i].vec.height1;
                    double deltadist = passingPoints[i].vec.distance2 - passingPoints[i].vec.distance1;

                    double azimuth = passingPoints[i].sp.Azimuth;
                    double proportion = 0;
                    if (deltaaz != 0)
                    {
                        double azStart = azimuth - passingPoints[i].vec.azimuth1;
                        if (azStart > 180) azStart -= 360;
                        if (azStart < -180) azStart += 360;
                        proportion = (azStart) / deltaaz;
                    }

                    if (proportion < 0) proportion = 0;
                    if (proportion > 1) proportion = 1;

                    //double altitude = passingPoints[i].vec.altitude1 + deltaalt * proportion;
                    double latitude = passingPoints[i].vec.latitude1 + deltalat * proportion;
                    double longitude = passingPoints[i].vec.longitude1 + deltalon * proportion;
                    //double distance = passingPoints[i].vec.distance1 + deltadist * proportion;
                    //double height = passingPoints[i].vec.height1 + deltaheight * proportion;

                    if (gpx)
                    {
                        outfile.WriteLine("<!--Waypoints " + (i + 1) + "-->");
                        outfile.WriteLine("<wpt lat=\"" + latitude + "\" lon=\"" + longitude + "\">");
                        outfile.WriteLine("<name>" + passingPoints[i].ht + " " + passingPoints[i].tcb + " " + i + " Bearing " + passingPoints[i].sp.Azimuth * 180 / Math.PI + " Elevation " + passingPoints[i].sp.Elevation * 180 / Math.PI + " lat/lon " + latitude + "," + longitude + "</name>");
                        outfile.WriteLine("</wpt>");
                    }
                    else
                    {
                        outfile.WriteLine(i + "," + passingPoints[i].ht + "," + passingPoints[i].tcb + "," + passingPoints[i].sp.Azimuth * 180 / Math.PI + "," + passingPoints[i].sp.Elevation * 180 / Math.PI + "," + latitude + "," + longitude);
                    }
                }
            }
            if (gpx)
            {
                outfile.WriteLine("</gpx>");
            }
            outfile.Close();
        }
    }
}
