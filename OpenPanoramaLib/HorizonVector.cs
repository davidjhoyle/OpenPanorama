using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenPanoramaLib
{
    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -    */
    /* Main Drawing code Classes (c) David Hoyle 2020 : MIT Licensed                                                */
    /*  HorizonVector class represents points on the visible horizon                                                */
    /*  PassingPoint class is used to record a rising or setting position and then dump into a GPX file             */
    /*  SeaHorizon class is a simple utility class for building the horizon distances for different observe heights */
    /*  PaintImage is the main drawing class of the entire program - ProcessSingleImage draws an image              */
    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -    */
    public class HorizonVector
    {
        private double az;
        public enum HorizonSource { Unknown = 0, Sea = 1, SRTMLinear = 2, SRTMProximal = 3, OSLandContour = 4, OSContourDiagonal = 5, OSSeaContour = 6, OSTrig = 7, LIDAR = 8 }
        public HorizonSource ptsrc;

        public double azimuth1;
        public double altitude1;
        public double latitude1;
        public double longitude1;
        public double distance1;
        public double height1;

        public double azimuth2;
        public double altitude2;
        public double latitude2;
        public double longitude2;
        public double distance2;
        public double height2;


        public HorizonVector()
        {
            az = -9999;
        }

        public HorizonVector(double myaz)
        {
            az = myaz;
        }

        public override string ToString()
        {
            return ptsrc + "," + latitude1 + "," + longitude1 + "," + azimuth1 + "," + distance1 + "," + height1 + "," + latitude2 + "," + longitude2 + "," + azimuth2 + "," + distance2 + "," + height2;
        }

        public void setHVContext(HorizonSource mysrc, double az1, double al1, double lat1, double lon1, double dist1, double hght1, double az2, double al2, double lat2, double lon2, double dist2, double hght2)
        {
            ptsrc = mysrc;

            azimuth1 = az1;
            altitude1 = al1;
            latitude1 = lat1;
            longitude1 = lon1;
            distance1 = dist1;
            height1 = hght1;

            azimuth2 = az2;
            altitude2 = al2;
            latitude2 = lat2;
            longitude2 = lon2;
            distance2 = dist2;
            height2 = hght2;
        }

        public void copyTo(HorizonVector src)
        {
            // The following is just some sanity check code - it does not do anything and used to debug
            if (az != -9999)
            {
                double deltaAZ = src.azimuth1 - src.azimuth2;
                if (deltaAZ > 180) deltaAZ -= 360;
                if (deltaAZ < -180) deltaAZ += 360;

                double maxAz = src.azimuth1;
                double minAz = src.azimuth2;
                if (deltaAZ < 0)
                {
                    maxAz = src.azimuth2;
                    minAz = src.azimuth1;
                }

                double dmaxaz = (az - maxAz);
                if (dmaxaz > 180)
                    dmaxaz -= 360;
                if (dmaxaz < -180)
                    dmaxaz += 360;
                double dminaz = (minAz - az);
                if (dminaz > 180)
                    dminaz -= 360;
                if (dminaz < -180)
                    dminaz += 360;
                //double daz_limit = 1;
                dmaxaz *= 60;
                dminaz *= 60;
                deltaAZ *= 60;
            }

            ptsrc = src.ptsrc;

            azimuth1 = src.azimuth1;
            altitude1 = src.altitude1;
            latitude1 = src.latitude1;
            longitude1 = src.longitude1;
            distance1 = src.distance1;
            height1 = src.height1;

            azimuth2 = src.azimuth2;
            altitude2 = src.altitude2;
            latitude2 = src.latitude2;
            longitude2 = src.longitude2;
            distance2 = src.distance2;
            height2 = src.height2;
        }
    }
}
