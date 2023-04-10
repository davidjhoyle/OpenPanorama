using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenPanoramaLib
{
    /// <summary>
    /// Copyright David Hoyle 2020 - MIT Licensed.
    /// </summary>
    public class ENZ
    {
        public double e;
        public double n;
        public double z;

        public ENZ()
        {
        }
    }



    public class LatLonConversions
    {
        const double R = 6371000; // earth's mean radius in m

        public static double toRad(double deg)
        {
            return deg / 180 * Math.PI;
        }

        public static double toDeg(double rad)
        {
            return rad * 180 / Math.PI;
        }

        public static double toBrng(double rad)
        {
            // convert radians to degrees (as bearing: 0...360)
            return (toDeg(rad) + 360) % 360;
        }


        //# author of original JavaScript code: Chris Vennes
        //# (c) 2002-2009 Chris Veness
        //# http://www.movable-type.co.uk/scripts/latlong.html
        //# Licence: LGPL, without any warranty express or implied
        /*
        * Use Haversine formula to Calculate distance (in m) between two points specified by
        * latit
        * ude/longitude (in numeric degrees)
        *
        * from: Haversine formula - R. W. Sinnott, "Virtues of the Haversine",
        *       Sky and Telescope, vol 68, no 2, 1984
        *       http://www.census.gov/cgi-bin/geo/gisfaq?Q5.1
        *
        * example usage from form:
        *   result.value = LatLon.distHaversine(lat1.value.parseDeg(), long1.value.parseDeg(),
        *                                       lat2.value.parseDeg(), long2.value.parseDeg());
        * where lat1, long1, lat2, long2, and result are form fields
        */
        public static double distHaversine(double lat1, double lon1, double lat2, double lon2)
        {
            var dLat = toRad(lat2 - lat1);
            var dLon = toRad(lon2 - lon1);
            lat1 = toRad(lat1);
            lat2 = toRad(lat2);

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(lat1) * Math.Cos(lat2) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            var d = R * c;
            return d;
        }


        /*
         * Use Law of Cosines to calculate distance (in m) between two points specified by latitude/longitude
         * (in numeric degrees).
         */
        public static double distCosineLaw(double lat1, double lon1, double lat2, double lon2)
        {

            var d = Math.Acos(Math.Sin(toRad(lat1)) * Math.Sin(toRad(lat2)) +
                              Math.Cos(toRad(lat1)) * Math.Cos(toRad(lat2)) * Math.Cos(toRad(lon2 - lon1))) * R;
            return d;
        }



        /*
         * calculate (initial) bearing between two points
         *
         * from: Ed Williams' Aviation Formulary, http://williams.best.vwh.net/avform.htm#Crs
         */
        public static double bearing(double lat1, double lon1, double lat2, double lon2)
        {
            lat1 = toRad(lat1);
            lat2 = toRad(lat2);
            var dLon = toRad(lon2 - lon1);

            var y = Math.Sin(dLon) * Math.Cos(lat2);
            var x = Math.Cos(lat1) * Math.Sin(lat2) -
                    Math.Sin(lat1) * Math.Cos(lat2) * Math.Cos(dLon);

            double ang = Math.Atan2(y, x);
            if (ang < 0)
            {
                ang += 2 * Math.PI;
            }
            return ang;
        }

        public static double bearingDeg(double lat1, double lon1, double lat2, double lon2)
        {
            return toBrng(bearing(lat1, lon1, lat2, lon2));
        }


        //# author of original JavaScript code: Chris Vennes
        //# (c) 2002-2009 Chris Veness
        //# http://www.movable-type.co.uk/scripts/latlong.html
        //# Licence: LGPL, without any warranty express or implied
        public static void GetLatLonFromBearingDistDeg(double φ1, double λ1, double brng, double d, ref double newlat, ref double newlon)
        {
            GetLatLonFromBearingDistRad(toRad(φ1), toRad(λ1), toRad(brng), d, ref newlat, ref newlon);

            newlat = toDeg(newlat);
            newlon = toDeg(newlon);
        }

        //# author of original JavaScript code: Chris Vennes
        //# (c) 2002-2009 Chris Veness
        //# http://www.movable-type.co.uk/scripts/latlong.html
        //# Licence: LGPL, without any warranty express or implied
        public static void GetLatLonFromBearingDistRad(double φ1, double λ1, double brng, double d, ref double newlat, ref double newlon)
        {
            //φ is latitude, λ is longitude, θ is the bearing (clockwise from north), δ
            newlat = Math.Asin(Math.Sin(φ1) * Math.Cos(d / R) +
                        Math.Cos(φ1) * Math.Sin(d / R) * Math.Cos(brng));
            newlon = λ1 + Math.Atan2(Math.Sin(brng) * Math.Sin(d / R) * Math.Cos(φ1),
                                     Math.Cos(d / R) - Math.Sin(φ1) * Math.Sin(newlat));
        }
    }
}

