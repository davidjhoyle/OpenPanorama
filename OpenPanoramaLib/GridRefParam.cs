using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace OpenPanoramaLib
{
    public class GridRefParam
    {
        public string country;
        public datumEnum datum;
        public double a = 6377563.396;
        public double b = 6356256.909;             // Airy 1830 major &amp; minor semi-axes
        public double F0 = 0.9996012717;                            // NatGrid scale factor on central meridian
        public double φ0 = Dms.toRadians(49);
        public double λ0 = Dms.toRadians(-2); // NatGrid true origin is 49°N,2°W
        public double N0 = -100e3;
        public double E0 = 400e3;


        public static readonly Dictionary<countryEnum, GridRefParam> _grids = new Dictionary<countryEnum, GridRefParam>()
        {
            { countryEnum.uk, new GridRefParam ( "uk,xx",  datumEnum.OSGB36, 6377563.396, 6356256.909, 0.9996012717, 49, -2, -100000, 400000 ) },
            { countryEnum.ie, new GridRefParam ( "ie,bd",  datumEnum.WGS84, 6378137.000, 6356752.3141, 0.99982, 53.5, -8, 750000, 600000 ) },
            //{ countryEnum.no, new GridRefParam ( "no",  datumEnum.WGS84, 6378137.000, 6356752.3141, 0.99982, 0, 0, 0, 500000 ) },  // EPSG:3044 - ETRS89 / UTM zone 32N (N-E)
            { countryEnum.no, new GridRefParam ( "no",  datumEnum.WGS84, 6378137.000, 6356752.3141, 0.99982, 58, 10.5, 1000000, 100000 ) },  // EPSG:3044 - ETRS89 / UTM zone 32N (N-E)


            //COMPD_CS["ETRS89 + CD Norway depth",
            //    GEOGCS["ETRS89",
            //        DATUM["European_Terrestrial_Reference_System_1989",
            //            SPHEROID["GRS 1980",6378137,298.257222101,
            //                AUTHORITY["EPSG","7019"]],
            //            AUTHORITY["EPSG","6258"]],
            //        PRIMEM["Greenwich",0,
            //            AUTHORITY["EPSG","8901"]],
            //        UNIT["degree",0.0174532925199433,
            //            AUTHORITY["EPSG","9122"]],
            //        AUTHORITY["EPSG","4258"]],
            //    VERT_CS["CD Norway depth",
            //        VERT_DATUM["Norwegian Chart Datum",2005,
            //            AUTHORITY["EPSG","1301"]],
            //        UNIT["metre",1,
            //            AUTHORITY["EPSG","9001"]],
            //        AXIS["Depth",DOWN],
            //        AUTHORITY["EPSG","9672"]],
            //    AUTHORITY["EPSG","9883"]]

            // Norway Lat Lon 58.43948N, 6.09637E goes to 6481301N, 330509E
            // easting	330472.13371732109	double
		    // north   6482727.6710656714  double
            // PROJCS["ETRS89 / UTM zone 32N (N-E)",
            // GEOGCS["ETRS89",
            // DATUM["European_Terrestrial_Reference_System_1989",
            // SPHEROID["GRS 1980",6378137,298.257222101,AUTHORITY["EPSG","7019"]],
            // TOWGS84[0,0,0,0,0,0,0],
            // AUTHORITY["EPSG","6258"]],
            // PRIMEM["Greenwich",0,AUTHORITY["EPSG","8901"]],
            // UNIT["degree",0.017453292519943278,AUTHORITY["EPSG","9102"]],
            // AUTHORITY["EPSG","4258"]],
            // PROJECTION["Transverse_Mercator",AUTHORITY["EPSG","9807"]],
            // PARAMETER["latitude_of_origin",0],
            // PARAMETER["central_meridian",9],
            // PARAMETER["scale_factor",0.9996],
            // PARAMETER["false_easting",500000],
            // PARAMETER["false_northing",0],
            // UNIT["metre",1,AUTHORITY["EPSG","9001"]],
            // AUTHORITY["EPSG","3044"]]
        };


        public GridRefParam(string _country, datumEnum _datum, double _a, double _b, double _F0, double _lat, double _lon, double _N0, double _E0)
        {
            country = _country;
            datum = _datum;
            a = _a;
            b = _b;
            F0 = _F0;
            φ0 = Dms.toRadians(_lat);
            λ0 = Dms.toRadians(_lon);
            N0 = _N0;
            E0 = _E0;
        }


        static public GridRefParam get(string country)
        {
            return get(getCountry(country));
        }


        static public GridRefParam get(countryEnum country)
        {
            if (_grids.ContainsKey(country))
            {
                return _grids[country];
            }
            return null;
        }

        static public countryEnum getCountry(string country)
        {
            foreach (var p in _grids)
            {
                if (p.Value.country.Contains(country.ToLower()))
                {
                    return p.Key;
                }
            }
            return countryEnum.unknown;
        }
    }


}
