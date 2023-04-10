using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace OpenPanoramaLib
{
    /* LatLon_OsGridRef - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -  */
    /* Ordnance Survey Grid Reference functions                           (c) Chris Veness 2005-2019  */
    /*                                                                                   MIT Licence  */
    /* www.movable-type.co.uk/scripts/latlong-gridref.html                                            */
    /* www.movable-type.co.uk/scripts/geodesy-library.html#osgridref                                  */
    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -  */


    /**
     * Extends LatLon class with method to convert LatLon point to OS grid reference.
     * https://www.movable-type.co.uk/scripts/js/es6-geodesy/docs/latlon-ellipsoidal.js.html
     * @extends LatLon
     */
    public class LatLon_OsGridRef : LatLonEllipsoidal_Datum
    {
        public LatLon_OsGridRef(double φ, double λ, double h, datumEnum datum = datumEnum.WGS84) : base(φ, λ, h)
        {
            this.datum = datums[datum];
        }

        /**
         * Converts latitude/longitude to Ordnance Survey grid reference easting/northing coordinate.
         *
         * @returns {OsGridRef} OS Grid Reference easting/northing.
         *
         * @example
         *   const grid = new LatLon(52.65798, 1.71605).toOsGrid(); // TG 51409 13177
         *   // for conversion of (historical) OSGB36 latitude/longitude point:
         *   const grid = new LatLon(52.65798, 1.71605).toOsGrid(LatLon.datums.OSGB36);
         */
        //public OsGridRef toOsGrid()
        //{
        //    // if necessary convert to OSGB36 first
        //    var point = (this.datum == LatLonEllipsoidal_Datum.datums[datumEnum.OSGB36] ? this : this.convertDatum(datumEnum.OSGB36));

        //    double φ = Dms.toRadians(point.lat);
        //    double λ = Dms.toRadians(point.lon);

        //    double a = 6377563.396;
        //    double b = 6356256.909;              // Airy 1830 major &amp; minor semi-axes
        //    double F0 = 0.9996012717;                             // NatGrid scale factor on central meridian
        //    double φ0 = Dms.toRadians(49);
        //    double λ0 = Dms.toRadians(-2);  // NatGrid true origin is 49°N,2°W
        //    double N0 = -100000, E0 = 400000;                     // northing &amp; easting of true origin, metres
        //    double e2 = 1 - (b * b) / (a * a);                          // eccentricity squared
        //    double n = (a - b) / (a + b), n2 = n * n, n3 = n * n * n;         // n, n², n³

        //    double cosφ = Math.Cos(φ);
        //    double sinφ = Math.Sin(φ);
        //    double ν = a * F0 / Math.Sqrt(1 - e2 * sinφ * sinφ);            // nu = transverse radius of curvature
        //    double ρ = a * F0 * (1 - e2) / Math.Pow(1 - e2 * sinφ * sinφ, 1.5); // rho = meridional radius of curvature
        //    double η2 = ν / ρ - 1;                                    // eta = ?

        //    double Ma = (1 + n + (5 / 4) * n2 + (5 / 4) * n3) * (φ - φ0);
        //    double Mb = (3 * n + 3 * n * n + (21 / 8) * n3) * Math.Sin(φ - φ0) * Math.Cos(φ + φ0);
        //    double Mc = ((15 / 8) * n2 + (15 / 8) * n3) * Math.Sin(2 * (φ - φ0)) * Math.Cos(2 * (φ + φ0));
        //    double Md = (35 / 24) * n3 * Math.Sin(3 * (φ - φ0)) * Math.Cos(3 * (φ + φ0));
        //    double M = b * F0 * (Ma - Mb + Mc - Md);              // meridional arc

        //    double cos3φ = cosφ * cosφ * cosφ;
        //    double cos5φ = cos3φ * cosφ * cosφ;
        //    double tan2φ = Math.Tan(φ) * Math.Tan(φ);
        //    double tan4φ = tan2φ * tan2φ;

        //    double I = M + N0;
        //    double II = (ν / 2) * sinφ * cosφ;
        //    double III = (ν / 24) * sinφ * cos3φ * (5 - tan2φ + 9 * η2);
        //    double IIIA = (ν / 720) * sinφ * cos5φ * (61 - 58 * tan2φ + tan4φ);
        //    double IV = ν * cosφ;
        //    double V = (ν / 6) * cos3φ * (ν / ρ - tan2φ);
        //    double VI = (ν / 120) * cos5φ * (5 - 18 * tan2φ + tan4φ + 14 * η2 - 58 * tan2φ * η2);

        //    double Δλ = λ - λ0;
        //    double Δλ2 = Δλ * Δλ, Δλ3 = Δλ2 * Δλ, Δλ4 = Δλ3 * Δλ, Δλ5 = Δλ4 * Δλ, Δλ6 = Δλ5 * Δλ;

        //    double N = I + II * Δλ2 + III * Δλ4 + IIIA * Δλ6;
        //    double E = E0 + IV * Δλ + V * Δλ3 + VI * Δλ5;

        //    //N = (N.toFixed(3)); // round to mm precision
        //    //E = (E.toFixed(3));

        //    return new OsGridRef(E, N); // gets truncated to SW corner of 1m grid square
        //}




        ///**
        // * Converts latitude/longitude to Ireland grid reference easting/northing coordinate.
        // *
        // * @returns {OsGridRef} Ireland Grid Reference easting/northing.
        // *
        // * @example
        // *   const grid = new LatLon(52.65798, 1.71605).toOsGrid(); // TG 51409 13177
        // *   // for conversion of (historical) OSGB36 latitude/longitude point:
        // *   const grid = new LatLon(52.65798, 1.71605).toOsGrid(LatLon.datums.OSGB36);
        // */
        //public OsGridRef toIEGrid()
        //{
        //    // if necessary convert to OSGB36 first
        //    var point = (this.datum == LatLonEllipsoidal_Datum.datums[datumEnum.OSGB36] ? this : this.convertDatum(datumEnum.OSGB36));

        //    double φ = Dms.toRadians(point.lat);
        //    double λ = Dms.toRadians(point.lon);

        //    double a = 6377563.396;
        //    double b = 6356256.909;              // Airy 1830 major &amp; minor semi-axes
        //    double F0 = 1.000035;                             // NatGrid scale factor on central meridian
        //    double φ0 = Dms.toRadians(53.5);
        //    double λ0 = Dms.toRadians(-8);              // Ireland NatGrid true origin is 53.5°N, 8°W
        //    double N0 = 250000, E0 = 200000;                     // northing &amp; easting of true origin, metres
        //    double e2 = 1 - (b * b) / (a * a);                          // eccentricity squared
        //    double n = (a - b) / (a + b), n2 = n * n, n3 = n * n * n;         // n, n², n³

        //    double cosφ = Math.Cos(φ);
        //    double sinφ = Math.Sin(φ);
        //    double ν = a * F0 / Math.Sqrt(1 - e2 * sinφ * sinφ);            // nu = transverse radius of curvature
        //    double ρ = a * F0 * (1 - e2) / Math.Pow(1 - e2 * sinφ * sinφ, 1.5); // rho = meridional radius of curvature
        //    double η2 = ν / ρ - 1;                                    // eta = ?

        //    double Ma = (1 + n + (5 / 4) * n2 + (5 / 4) * n3) * (φ - φ0);
        //    double Mb = (3 * n + 3 * n * n + (21 / 8) * n3) * Math.Sin(φ - φ0) * Math.Cos(φ + φ0);
        //    double Mc = ((15 / 8) * n2 + (15 / 8) * n3) * Math.Sin(2 * (φ - φ0)) * Math.Cos(2 * (φ + φ0));
        //    double Md = (35 / 24) * n3 * Math.Sin(3 * (φ - φ0)) * Math.Cos(3 * (φ + φ0));
        //    double M = b * F0 * (Ma - Mb + Mc - Md);              // meridional arc

        //    double cos3φ = cosφ * cosφ * cosφ;
        //    double cos5φ = cos3φ * cosφ * cosφ;
        //    double tan2φ = Math.Tan(φ) * Math.Tan(φ);
        //    double tan4φ = tan2φ * tan2φ;

        //    double I = M + N0;
        //    double II = (ν / 2) * sinφ * cosφ;
        //    double III = (ν / 24) * sinφ * cos3φ * (5 - tan2φ + 9 * η2);
        //    double IIIA = (ν / 720) * sinφ * cos5φ * (61 - 58 * tan2φ + tan4φ);
        //    double IV = ν * cosφ;
        //    double V = (ν / 6) * cos3φ * (ν / ρ - tan2φ);
        //    double VI = (ν / 120) * cos5φ * (5 - 18 * tan2φ + tan4φ + 14 * η2 - 58 * tan2φ * η2);

        //    double Δλ = λ - λ0;
        //    double Δλ2 = Δλ * Δλ, Δλ3 = Δλ2 * Δλ, Δλ4 = Δλ3 * Δλ, Δλ5 = Δλ4 * Δλ, Δλ6 = Δλ5 * Δλ;

        //    double N = I + II * Δλ2 + III * Δλ4 + IIIA * Δλ6;
        //    double E = E0 + IV * Δλ + V * Δλ3 + VI * Δλ5;

        //    //N = (N.toFixed(3)); // round to mm precision
        //    //E = (E.toFixed(3));

        //    return new OsGridRef(E, N); // gets truncated to SW corner of 1m grid square
        //}



        ///**
        // * Converts latitude/longitude to Ireland grid reference easting/northing coordinate.
        // *
        // * @returns {OsGridRef} Ireland UTM Grid Reference easting/northing.
        // *
        // * @example
        // *   const grid = new LatLon(52.65798, 1.71605).toOsGrid(); // TG 51409 13177
        // *   // for conversion of (historical) OSGB36 latitude/longitude point:
        // *   const grid = new LatLon(52.65798, 1.71605).toOsGrid(LatLon.datums.OSGB36);
        // */
        //public OsGridRef toIEUTMGrid()
        //{
        //    // if necessary convert to OSGB36 first
        //    var point = (this.datum == LatLonEllipsoidal_Datum.datums[datumEnum.WGS84] ? this : this.convertDatum(datumEnum.WGS84));

        //    double φ = Dms.toRadians(point.lat);
        //    double λ = Dms.toRadians(point.lon);

        //    double a = 6378137.000;
        //    double b = 6356752.3141;               // Airy 1830 major &amp; minor semi-axes
        //    double F0 = 0.99982;                             // NatGrid scale factor on central meridian
        //    double φ0 = Dms.toRadians(53.5);
        //    double λ0 = Dms.toRadians(-8);              // Ireland NatGrid true origin is 53 30°N, 8°W
        //    double N0 = 750000;
        //    double E0 = 600000;                     // northing &amp; easting of true origin, metres
        //    double e2 = 1 - (b * b) / (a * a);                          // eccentricity squared
        //    double n = (a - b) / (a + b);
        //    double n2 = n * n;
        //    double n3 = n * n * n;         // n, n², n³

        //    double cosφ = Math.Cos(φ);
        //    double sinφ = Math.Sin(φ);
        //    double ν = a * F0 / Math.Sqrt(1 - e2 * sinφ * sinφ);            // nu = transverse radius of curvature
        //    double ρ = a * F0 * (1 - e2) / Math.Pow(1 - e2 * sinφ * sinφ, -1.5); // rho = meridional radius of curvature
        //    double η2 = ν / ρ - 1;                                    // eta = ?

        //    double Ma = (1 + n + (5.0 / 4) * n2 + (5.0 / 4) * n3) * (φ - φ0);
        //    double Mb = (3 * n + 3 * n2 + (21.0 / 8) * n3) * Math.Sin(φ - φ0) * Math.Cos(φ + φ0);
        //    double Mc = ((15.0 / 8) * n2 + (15.0 / 8) * n3) * Math.Sin(2 * (φ - φ0)) * Math.Cos(2 * (φ + φ0));
        //    double Md = (35.0 / 24) * n3 * Math.Sin(3.0 * (φ - φ0)) * Math.Cos(3 * (φ + φ0));
        //    double M = b * F0 * (Ma - Mb + Mc - Md);              // meridional arc

        //    double cos3φ = cosφ * cosφ * cosφ;
        //    double cos5φ = cos3φ * cosφ * cosφ;
        //    double tan2φ = Math.Tan(φ) * Math.Tan(φ);
        //    double tan4φ = tan2φ * tan2φ;

        //    double I = M + N0;
        //    double II = (ν / 2) * sinφ * cosφ;
        //    double III = (ν / 24) * sinφ * cos3φ * (5 - tan2φ + 9 * η2);
        //    double IIIA = (ν / 720) * sinφ * cos5φ * (61 - 58 * tan2φ + tan4φ);
        //    double IV = ν * cosφ;
        //    double V = (ν / 6) * cos3φ * (ν / ρ - tan2φ);
        //    double VI = (ν / 120) * cos5φ * (5 - 18 * tan2φ + tan4φ + 14 * η2 - 58 * tan2φ * η2);

        //    double Δλ = λ - λ0;
        //    double Δλ2 = Δλ * Δλ;
        //    double Δλ3 = Δλ2 * Δλ;
        //    double Δλ4 = Δλ3 * Δλ;
        //    double Δλ5 = Δλ4 * Δλ;
        //    double Δλ6 = Δλ5 * Δλ;

        //    double N = I + II * Δλ2 + III * Δλ4 + IIIA * Δλ6;
        //    double E = E0 + IV * Δλ + V * Δλ3 + VI * Δλ5;

        //    //N = (N.toFixed(3)); // round to mm precision
        //    //E = (E.toFixed(3));

        //    return new OsGridRef(E, N); // gets truncated to SW corner of 1m grid square
        //}



        public OsGridRef toGrid(string countrystr)
        {
            countryEnum country = GridRefParam.getCountry(countrystr);
            return toGrid(country);
        }

        /**
            * Converts latitude/longitude to a local grid in country grid reference easting/northing coordinate.
            *
            * @returns {OsGridRef} Grid Reference easting/northing.
            *
            * @example
            *   const grid = new LatLon(52.65798, 1.71605).toOsGrid(); // TG 51409 13177
            *   // for conversion of (historical) OSGB36 latitude/longitude point:
            *   const grid = new LatLon(52.65798, 1.71605).toOsGrid(LatLon.datums.OSGB36);
            */
        public OsGridRef toGrid(countryEnum country)
        {
            GridRefParam gridparams = GridRefParam.get(country);
            if (gridparams == null)
            {
                return null;
            }

            // if necessary convert to OSGB36 first
            var point = (this.datum == LatLonEllipsoidal_Datum.datums[gridparams.datum] ? this : this.convertDatum(gridparams.datum));

            double φ = Dms.toRadians(point.lat);
            double λ = Dms.toRadians(point.lon);

            double a = gridparams.a;
            double b = gridparams.b;
            double F0 = gridparams.F0;
            double φ0 = gridparams.φ0;
            double λ0 = gridparams.λ0;
            double N0 = gridparams.N0;
            double E0 = gridparams.E0;

            double e2 = 1 - (b * b) / (a * a);                          // eccentricity squared
            double n = (a - b) / (a + b);
            double n2 = n * n;
            double n3 = n * n * n;         // n, n², n³

            double cosφ = Math.Cos(φ);
            double sinφ = Math.Sin(φ);
            double ν = a * F0 / Math.Sqrt(1 - e2 * sinφ * sinφ);            // nu = transverse radius of curvature
            double ρ = a * F0 * (1 - e2) / Math.Pow(1 - e2 * sinφ * sinφ, 1.5); // rho = meridional radius of curvature
            double η2 = ν / ρ - 1;                                    // eta = ?

            double Ma = (1 + n + (5.0 / 4) * n2 + (5.0 / 4) * n3) * (φ - φ0);
            double Mb = (3 * n + 3 * n2 + (21.0 / 8) * n3) * Math.Sin(φ - φ0) * Math.Cos(φ + φ0);
            double Mc = ((15.0 / 8) * n2 + (15.0 / 8) * n3) * Math.Sin(2 * (φ - φ0)) * Math.Cos(2 * (φ + φ0));
            double Md = (35.0 / 24) * n3 * Math.Sin(3.0 * (φ - φ0)) * Math.Cos(3 * (φ + φ0));
            double M = b * F0 * (Ma - Mb + Mc - Md);              // meridional arc

            double cos3φ = cosφ * cosφ * cosφ;
            double cos5φ = cos3φ * cosφ * cosφ;
            double tan2φ = Math.Tan(φ) * Math.Tan(φ);
            double tan4φ = tan2φ * tan2φ;

            double I = M + N0;
            double II = (ν / 2) * sinφ * cosφ;
            double III = (ν / 24) * sinφ * cos3φ * (5 - tan2φ + 9 * η2);
            double IIIA = (ν / 720) * sinφ * cos5φ * (61 - 58 * tan2φ + tan4φ);
            double IV = ν * cosφ;
            double V = (ν / 6) * cos3φ * (ν / ρ - tan2φ);
            double VI = (ν / 120) * cos5φ * (5 - 18 * tan2φ + tan4φ + 14 * η2 - 58 * tan2φ * η2);

            double Δλ = λ - λ0;
            double Δλ2 = Δλ * Δλ;
            double Δλ3 = Δλ2 * Δλ;
            double Δλ4 = Δλ3 * Δλ;
            double Δλ5 = Δλ4 * Δλ;
            double Δλ6 = Δλ5 * Δλ;

            double N = I + II * Δλ2 + III * Δλ4 + IIIA * Δλ6;
            double E = E0 + IV * Δλ + V * Δλ3 + VI * Δλ5;

            //N = (N.toFixed(3)); // round to mm precision
            //E = (E.toFixed(3));

            return new OsGridRef(E, N); // gets truncated to SW corner of 1m grid square
        }




        /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -  */
        public static LatLon_OsGridRef GetLLFromLoc(string loc, countryEnum eCountry)
        {
            LatLon_OsGridRef ll = new LatLon_OsGridRef(0, 0, 0);

            if (loc.Contains(",") || loc[0] >= '0' && loc[0] <= '9')
            {
                loc = loc.Replace(" ", ",").Replace(",,", ",");

                char[] splitstr = { ',' };
                string[] bits = loc.Split(splitstr);
                ll.lat = Convert.ToDouble(bits[0]);
                ll.lon = Convert.ToDouble(bits[1]);
            }
            else
            {
                if (!loc.Contains(" "))
                {
                    string tmp = "" + loc[0] + loc[1] + " ";
                    if (loc.Length == 10)
                    {
                        tmp += "" + loc[2] + loc[3] + loc[4] + loc[5] + " " + loc[6] + loc[7] + loc[8] + loc[9];
                    }
                    else if (loc.Length == 8)
                    {
                        tmp += "" + loc[2] + loc[3] + loc[4] + " " + loc[5] + loc[6] + loc[7];
                    }
                    else if (loc.Length == 6)
                    {
                        tmp += "" + loc[2] + loc[3] + "0 " + loc[4] + loc[5] + "0";
                        Console.WriteLine("*** Inaccurate Gridref *** " + loc);
                    }
                    else if (loc.Length == 12)
                    {
                        tmp += "" + loc[2] + loc[3] + loc[4] + loc[5] + " " + loc[7] + loc[8] + loc[9] + loc[10];
                    }
                    else
                    {
                        tmp = "";
                    }
                    loc = tmp;
                }

                OsGridRef nee = OsGridRef.parse(loc);
                if (nee != null)
                {
                    OsGridRef osgr2 = new OsGridRef(nee.east, nee.north);
                    ll = osgr2.toLatLon(eCountry);
                }
            }
            return ll;
        }
    }
}
