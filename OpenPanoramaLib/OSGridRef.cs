using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OpenPanoramaLib
{
    /* OsGridRef  - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */


    /**
     * OS grid references with methods to parse and convert them to latitude/longitude points.
     *
     * @module osgridref
     */
    public class OsGridRef
    {
        public double easting;
        public double northing;

        public double east
        {
            get
            {
                return easting;
            }
            set
            {
                easting = value;
            }
        }

        public double north
        {
            get
            {
                return northing;
            }
            set
            {
                northing = value;
            }
        }

        /**
         * Creates an OsGridRef object.
         *
         * @param {number} easting - Easting in metres from OS false origin.
         * @param {number} northing - Northing in metres from OS false origin.
         *
         * @example
         *   import OsGridRef from '/js/geodesy/osgridref.js';
         *   const gridref = new OsGridRef(651409, 313177);
         */
        public OsGridRef(double e, double n)
        {
            easting = e;
            northing = n;
        }


        /**
         * Converts ‘this’ Ordnance Survey grid reference easting/northing coordinate to latitude/longitude
         * (SW corner of grid square).
         *
         * While OS grid references are based on OSGB-36, the Ordnance Survey have deprecated the use of
         * OSGB-36 for latitude/longitude coordinates (in favour of WGS-84), hence this function returns
         * WGS-84 by default, with OSGB-36 as an option. See www.ordnancesurvey.co.uk/blog/2014/12/2.
         *
         * Note formulation implemented here due to Thomas, Redfearn, etc is as published by OS, but is
         * inferior to Krüger as used by e.g. Karney 2011.
         *
         * @param   {LatLon.datum} [datum=WGS84] - Datum to convert grid reference into.
         * @returns {LatLon}       Latitude/longitude of supplied grid reference.
         *
         * @example
         *   const gridref = new OsGridRef(651409.903, 313177.270);
         *   const pWgs84 = gridref.toLatLon();                    // 52°39′28.723″N, 001°42′57.787″E
         *   // to obtain (historical) OSGB36 lat/lon point:
         *   const pOsgb = gridref.toLatLon(LatLon.datums.OSGB36); // 52°39′27.253″N, 001°43′04.518″E
         */
        //public LatLon_OsGridRef toLatLon(datumEnum datum= datumEnum.WGS84)
        //{
        //    double E = easting;
        //    double N = northing;

        //    double a = 6377563.396;
        //    double b = 6356256.909;             // Airy 1830 major &amp; minor semi-axes
        //    double F0 = 0.9996012717;                            // NatGrid scale factor on central meridian
        //    double φ0 = Dms.toRadians(49);
        //      double λ0 = Dms.toRadians(-2); // NatGrid true origin is 49°N,2°W
        //    double N0 = -100e3, E0 = 400e3;                      // northing &amp; easting of true origin, metres
        //    double e2 = 1 - (b * b) / (a * a);                         // eccentricity squared
        //    double n = (a - b) / (a + b), n2 = n * n, n3 = n * n * n;        // n, n², n³

        //    var φ = φ0;
        //    double M = 0;

        //    do
        //    {
        //        φ = (N - N0 - M) / (a * F0) + φ;

        //        double Ma = (1 + n + (5 / 4) * n2 + (5 / 4) * n3) * (φ - φ0);
        //        double Mb = (3 * n + 3 * n * n + (21 / 8) * n3) * Math.Sin(φ - φ0) * Math.Cos(φ + φ0);
        //        double Mc = ((15 / 8) * n2 + (15 / 8) * n3) * Math.Sin(2 * (φ - φ0)) * Math.Cos(2 * (φ + φ0));
        //        double Md = (35 / 24) * n3 * Math.Sin(3 * (φ - φ0)) * Math.Cos(3 * (φ + φ0));
        //        M = b * F0 * (Ma - Mb + Mc - Md);               // meridional arc

        //    } while (Math.Abs(N - N0 - M) >= 0.00001);  // ie until &lt; 0.01mm

        //    double cosφ = Math.Cos(φ), sinφ = Math.Sin(φ);
        //    double ν = a * F0 / Math.Sqrt(1 - e2 * sinφ * sinφ);             // nu = transverse radius of curvature
        //    double ρ = a * F0 * (1 - e2) / Math.Pow(1 - e2 * sinφ * sinφ, 1.5);     // rho = meridional radius of curvature
        //    double η2 = ν / ρ - 1;                                   // eta = ?

        //    double tanφ = Math.Tan(φ);
        //    double tan2φ = tanφ * tanφ, tan4φ = tan2φ * tan2φ, tan6φ = tan4φ * tan2φ;
        //    double secφ = 1 / cosφ;
        //    double ν3 = ν * ν * ν, ν5 = ν3 * ν * ν, ν7 = ν5 * ν * ν;
        //    double VII = tanφ / (2 * ρ * ν);
        //    double VIII = tanφ / (24 * ρ * ν3) * (5 + 3 * tan2φ + η2 - 9 * tan2φ * η2);
        //    double IX = tanφ / (720 * ρ * ν5) * (61 + 90 * tan2φ + 45 * tan4φ);
        //    double X = secφ / ν;
        //    double XI = secφ / (6 * ν3) * (ν / ρ + 2 * tan2φ);
        //    double XII = secφ / (120 * ν5) * (5 + 28 * tan2φ + 24 * tan4φ);
        //    double XIIA = secφ / (5040 * ν7) * (61 + 662 * tan2φ + 1320 * tan4φ + 720 * tan6φ);

        //    double dE = (E - E0), dE2 = dE * dE, dE3 = dE2 * dE, dE4 = dE2 * dE2, dE5 = dE3 * dE2, dE6 = dE4 * dE2, dE7 = dE5 * dE2;
        //    φ = φ - VII * dE2 + VIII * dE4 - IX * dE6;
        //    double λ = λ0 + X * dE - XI * dE3 + XII * dE5 - XIIA * dE7;


        //    var point = new LatLon_OsGridRef(Dms.toDegrees(φ), Dms.toDegrees(λ), 0, datumEnum.OSGB36);
        //    if (datum != datumEnum.OSGB36)
        //    {
        //        // if point is required in datum other than OSGB36, convert it
        //        LatLonEllipsoidal point2 = point.convertDatum(datum);

        //        // convertDatum() gives us a LatLon: convert to LatLon_OsGridRef which includes toOsGrid()
        //        point = new LatLon_OsGridRef(point2.lat, point2.lon, point2.height, point2.datum.datumNum);
        //    }

        //    return point;
        //}





        /**
         * Converts ‘this’ Country grid reference easting/northing coordinate to latitude/longitude
         * (SW corner of grid square).
         *
         * While OS grid references are based on OSGB-36, the Ordnance Survey have deprecated the use of
         * OSGB-36 for latitude/longitude coordinates (in favour of WGS-84), hence this function returns
         * WGS-84 by default, with OSGB-36 as an option. See www.ordnancesurvey.co.uk/blog/2014/12/2.
         *
         * Note formulation implemented here due to Thomas, Redfearn, etc is as published by OS, but is
         * inferior to Krüger as used by e.g. Karney 2011.
         *
         * @param   {LatLon.datum} [datum=WGS84] - Datum to convert grid reference into.
         * @returns {LatLon}       Latitude/longitude of supplied grid reference.
         *
         * @example
         *   const gridref = new OsGridRef(651409.903, 313177.270);
         *   const pWgs84 = gridref.toLatLon();                    // 52°39′28.723″N, 001°42′57.787″E
         *   // to obtain (historical) OSGB36 lat/lon point:
         *   const pOsgb = gridref.toLatLon(LatLon.datums.OSGB36); // 52°39′27.253″N, 001°43′04.518″E
         */
        public LatLon_OsGridRef toLatLon(countryEnum country, datumEnum datum = datumEnum.WGS84)
        {
            double E = easting;
            double N = northing;

            GridRefParam gridparams = GridRefParam.get(country);

            //double φ = Dms.toRadians(point.lat);
            //double λ = Dms.toRadians(point.lon);

            double a = gridparams.a;
            double b = gridparams.b;
            double F0 = gridparams.F0;
            double φ0 = gridparams.φ0;
            double λ0 = gridparams.λ0;
            double N0 = gridparams.N0;
            double E0 = gridparams.E0;

            //double a = 6378137.000;
            //double b = 6356752.3141;             // Airy 1830 major &amp; minor semi-axes
            //double F0 = 0.99982;                            // NatGrid scale factor on central meridian
            //double φ0 = Dms.toRadians(53.5);
            //double λ0 = Dms.toRadians(-8);      // Ireland NatGrid true origin is 53 30°N, 8°W
            //double N0 = 750000;
            //double E0 = 600000;                      // northing &amp; easting of true origin, metres

            double e2 = 1 - (b * b) / (a * a);                         // eccentricity squared
            double n = (a - b) / (a + b);
            double n2 = n * n;
            double n3 = n * n * n;        // n, n², n³

            double φ = (N - N0) / (a * F0) + φ0;
            double M = 0;
            bool loopDone = false;

            do
            {
                double Ma = (1 + n + (5.0 / 4) * n2 + (5.0 / 4) * n3) * (φ - φ0);
                double Mb = (3 * n + 3 * n * n + (21.0 / 8) * n3) * Math.Sin(φ - φ0) * Math.Cos(φ + φ0);
                double Mc = ((15.0 / 8) * n2 + (15.0 / 8) * n3) * Math.Sin(2 * (φ - φ0)) * Math.Cos(2 * (φ + φ0));
                double Md = (35.0 / 24) * n3 * Math.Sin(3 * (φ - φ0)) * Math.Cos(3 * (φ + φ0));
                M = b * F0 * (Ma - Mb + Mc - Md);               // meridional arc

                if (Math.Abs(N - N0 - M) > 0.00001)   // ie until &lt; 0.01mm
                {
                    φ = (N - N0 - M) / (a * F0) + φ;
                }
                else
                {
                    loopDone = true;
                }

            } while (!loopDone);

            double cosφ = Math.Cos(φ);
            double sinφ = Math.Sin(φ);
            double ν = a * F0 / Math.Sqrt(1 - e2 * sinφ * sinφ);             // nu = transverse radius of curvature
            double ρ = a * F0 * (1 - e2) / Math.Pow(1 - e2 * sinφ * sinφ, 1.5);     // rho = meridional radius of curvature
            double η2 = ν / ρ - 1;                                   // eta = ?

            double tanφ = Math.Tan(φ);
            double tan2φ = tanφ * tanφ, tan4φ = tan2φ * tan2φ, tan6φ = tan4φ * tan2φ;
            double secφ = 1 / cosφ;
            double ν3 = ν * ν * ν, ν5 = ν3 * ν * ν, ν7 = ν5 * ν * ν;
            double VII = tanφ / (2 * ρ * ν);
            double VIII = tanφ / (24 * ρ * ν3) * (5 + 3 * tan2φ + η2 - 9 * tan2φ * η2);
            double IX = tanφ / (720 * ρ * ν5) * (61 + 90 * tan2φ + 45 * tan4φ);
            double X = secφ / ν;
            double XI = secφ / (6 * ν3) * (ν / ρ + 2 * tan2φ);
            double XII = secφ / (120 * ν5) * (5 + 28 * tan2φ + 24 * tan4φ);
            double XIIA = secφ / (5040 * ν7) * (61 + 662 * tan2φ + 1320 * tan4φ + 720 * tan6φ);

            double dE = (E - E0), dE2 = dE * dE, dE3 = dE2 * dE, dE4 = dE2 * dE2, dE5 = dE3 * dE2, dE6 = dE4 * dE2, dE7 = dE5 * dE2;
            φ = φ - VII * dE2 + VIII * dE4 - IX * dE6;
            double λ = λ0 + X * dE - XI * dE3 + XII * dE5 - XIIA * dE7;


            // if necessary convert to correct sorce Datum first
            var point = new LatLon_OsGridRef(Dms.toDegrees(φ), Dms.toDegrees(λ), 0, gridparams.datum);
            if (datum != datumEnum.OSGB36)
            {
                // if point is required in datum other than OSGB36, convert it
                LatLonEllipsoidal point2 = point.convertDatum(datum);

                // convertDatum() gives us a LatLon: convert to LatLon_OsGridRef which includes toOsGrid()
                point = new LatLon_OsGridRef(point2.lat, point2.lon, point2.height, point2.datum.datumNum);
            }

            return point;
        }



        /**
         * Parses grid reference to OsGridRef object.
         *
         * Accepts standard grid references (eg 'SU 387 148'), with or without whitespace separators, from
         * two-digit references up to 10-digit references (1m × 1m square), or fully numeric comma-separated
         * references in metres (eg '438700,114800').
         *
         * @param   {string}    gridref - Standard format OS grid reference.
         * @returns {OsGridRef} Numeric version of grid reference in metres from false origin (SW corner of
         *   supplied grid square).
         * @throws  {Error}     Invalid grid reference.
         *
         * @example
         *   const grid = OsGridRef.parse('TG 51409 13177'); // grid: { easting: 651409, northing: 313177 }
         */
        static public OsGridRef parse(string gridref)
        {
            gridref = gridref.Trim();

            // check for fully numeric comma-separated gridref format
            Regex regie = new Regex("^([0-9]{1,6}),\\s*([0-9]{1,6})$");

            Match match = regie.Match(gridref);
            //var match = gridref.Match(/^ (\d +),\s * (\d +)$/);
            if (match.Groups.Count >= 3)
            {
                return new OsGridRef((double)Convert.ToDouble(match.Groups[1].Value), (double)Convert.ToDouble(match.Groups[2].Value));
            }

            // validate format
            regie = new Regex("^([A-Z]{2})\\s+([0-9]{1,6})\\s+([0-9]{1,6})$");
            match = regie.Match(gridref);

            //match = gridref.match(/^[A - Z]{ 2}\s *[0 - 9] +\s *[0 - 9] +$/ i);
            if (match == null || match.Groups.Count == 0)
            {
                Console.WriteLine("Invalid Grid Reference " + gridref);
                return null;
                //throw new Error(`invalid grid reference ‘${ gridref }’`);
            }

            gridref = gridref.Replace(" ", "");

            // get numeric values of letter references, mapping A->0, B->1, C->2, etc:
            char l1 = (char)(gridref.ToUpper()[0] - 'A');
            char l2 = (char)(gridref.ToUpper()[1] - 'A');

            // shuffle down letters after 'I' since 'I' is not used in grid:
            if (l1 > 7) l1--;
            if (l2 > 7) l2--;

            // sanity check
            if (l1 < 8 || l1 > 18)
            {
                Console.WriteLine("Invalid Grid Reference " + gridref);
                return null;
                // throw new Error(`invalid grid reference ‘${ gridref }’`);
            }


            // convert grid letters into 100km-square indexes from false origin (grid square SV):
            int e100km = ((l1 - 2) % 5) * 5 + (l2 % 5);
            int n100km = (int)((19 - Math.Floor((double)(l1 / 5)) * 5) - Math.Floor((double)(l2 / 5)));

            string enstr = gridref.Substring(2);
            if ((enstr.Length % 2) != 0)
            {
                // Both halves of the numeric part should be same size
                Console.WriteLine("Invalid Grid Reference " + gridref);
                return null;
            }


            string eeaststr = (enstr.Substring(0, enstr.Length / 2) + "00000").Substring(0, 5);
            string nnorthstr = (enstr.Substring(enstr.Length / 2) + "00000").Substring(0, 5);

            int eeast = Convert.ToInt32(eeaststr);
            int nnorth = Convert.ToInt32(nnorthstr);

            eeast += e100km * 100000;
            nnorth += n100km * 100000;

            //// skip grid letters to get numeric (easting/northing) part of ref
            //regie = new Regex("\\s+");
            //Match en = regie.Match(gridref);
            ////let en = gridref.slice(2).trim().split(/\s +/);
            //// if e/n not whitespace separated, split half way
            //if (en.Groups.Count == 1)
            //{
            //    //en = [en[0].slice(0, en[0].length / 2), en[0].slice(en[0].length / 2)];
            //}

            //// validation
            //if (en[0].length != en[1].length) throw new Error(`invalid grid reference ‘${ gridref }’`);

            //// standardise to 10-digit refs (metres)
            //en[0] = en[0].padEnd(5, '0');
            //en[1] = en[1].padEnd(5, '0');

            //const e = e100km + en[0];
            //const n = n100km + en[1];

            return new OsGridRef(eeast, nnorth);
        }


        /**
         * Converts ‘this’ numeric grid reference to standard OS grid reference.
         *
         * @param   {number} [digits=10] - Precision of returned grid reference (10 digits = metres);
         *   digits=0 will return grid reference in numeric format.
         * @returns {string} This grid reference in standard format.
         *
         * @example
         *   const gridref = new OsGridRef(651409, 313177).toString(8); // 'TG 5140 1317'
         *   const gridref = new OsGridRef(651409, 313177).toString(0); // '651409,313177'
         */
        public string toString(int digits = 10)
        {
            string formatter = "";

            for (int i = 0; i < digits; i += 2)
            {
                formatter += "0";
            }

            double tmpeast = Math.Round(easting);
            double tmpnorth = Math.Round(northing);

            // get the 100km-grid indices
            int e100km = (int)Math.Floor(tmpeast / 100000);
            int n100km = (int)Math.Floor(tmpnorth / 100000);

            // translate those into numeric equivalents of the grid letters
            int l1 = (19 - n100km) - (19 - n100km) % 5 + (int)Math.Floor((double)((e100km + 10) / 5));
            int l2 = (19 - n100km) * 5 % 25 + e100km % 5;

            // compensate for skipped 'I' and build grid letter-pairs
            if (l1 > 7) l1++;
            if (l2 > 7) l2++;
            string letterPair = (char)(l1 + 'A') + "" + (char)(l2 + 'A');

            // strip 100km-grid indices from easting &amp; northing, and reduce precision
            var e = Math.Floor((tmpeast % 100000) / Math.Pow(10, 5 - digits / 2));
            var n = Math.Floor((tmpnorth % 100000) / Math.Pow(10, 5 - digits / 2));

            // pad eastings &amp; northings with leading zeros
            return letterPair + " " + e.ToString(formatter) + " " + n.ToString(formatter);
        }
    }
}
