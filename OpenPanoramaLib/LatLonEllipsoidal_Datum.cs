using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenPanoramaLib
{
    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -  */
    /* Various Classes definitions (not needed in javascript) (c) David Hoyle 2020 : MIT Licensed     */
    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -  */

    public class Ellipsoid
    {
        public double a;
        public double b;
        public double f;

        public Ellipsoid(double A, double B, double F)
        {
            a = A;
            b = B;
            f = F;
        }
    }


    public class Transform
    {
        public double tx;
        public double ty;
        public double tz;
        public double s;
        public double rx;
        public double ry;
        public double rz;

        public Transform(double TX, double TY, double TZ, double S, double RX, double RY, double RZ)
        {
            tx = TX;
            ty = TY;
            tz = TZ;
            s = S;
            rx = RX;
            ry = RY;
            rz = RZ;
        }
    }



    public class EllipsoidTransform
    {
        public datumEnum datumNum;
        public ellipsoidEnum ellipsoidVal;
        public Ellipsoid ellipsoid;
        public Transform transform;
        public Transform invTransform;

        public EllipsoidTransform(datumEnum de, ellipsoidEnum elli, Transform t)
        {
            datumNum = de;
            ellipsoidVal = elli;
            ellipsoid = LatLonEllipsoidal_Datum.ellipsoids[elli];
            transform = t;
            invTransform = new Transform(-transform.tx, -transform.ty, -transform.tz, -transform.s, -transform.rx, -transform.ry, -transform.rz);
        }
    }

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -  */
    /* Geodesy tools for conversions between (historical) datums          (c) Chris Veness 2005-2019  */
    /*                                                                                   MIT Licence  */
    /* www.movable-type.co.uk/scripts/latlong-convert-coords.html                                     */
    /* www.movable-type.co.uk/scripts/geodesy-library.html#latlon-ellipsoidal-datum                  */
    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -  */


    //    import LatLonEllipsoidal, { Cartesian, Dms }
    //      from './latlon-ellipsoidal.js';


    /**
     * Historical geodetic datums: a latitude/longitude point defines a geographic location on or
     * above/below the  earth’s surface, measured in degrees from the equator & the International
     * Reference Meridian and metres above the ellipsoid, and based on a given datum. The datum is
     * based on a reference ellipsoid and tied to geodetic survey reference points.
     *
     * Modern geodesy is generally based on the WGS84 datum (as used for instance by GPS systems), but
     * previously various reference ellipsoids and datum references were used.
     *
     * This module extends the core latlon-ellipsoidal module to include ellipsoid parameters and datum
     * transformation parameters, and methods for converting between different (generally historical)
     * datums.
     *
     * It can be used for UK Ordnance Survey mapping (OS National Grid References are still based on the
     * otherwise historical OSGB36 datum), as well as for historical purposes.
     *
     * q.v. Ordnance Survey ‘A guide to coordinate systems in Great Britain’ Section 6,
     * www.ordnancesurvey.co.uk/docs/support/guide-coordinate-systems-great-britain.pdf, and also
     * www.ordnancesurvey.co.uk/blog/2014/12/2.
     *
     * @module latlon-ellipsoidal-datum
     */

    /* sources:
     * - ED50:       www.gov.uk/guidance/oil-and-gas-petroleum-operations-notices#pon-4
     * - Irl1975:    www.osi.ie/wp-content/uploads/2015/05/transformations_booklet.pdf
     * - NAD27:      en.wikipedia.org/wiki/Helmert_transformation
     * - NAD83:      www.uvm.edu/giv/resources/WGS84_NAD83.pdf [strictly, WGS84(G1150) -> NAD83(CORS96) @ epoch 1997.0]
     *               (note NAD83(1986) ≡ WGS84(Original); confluence.qps.nl/pages/viewpage.action?pageId=29855173)
     * - NTF:        Nouvelle Triangulation Francaise geodesie.ign.fr/contenu/fichiers/Changement_systeme_geodesique.pdf
     * - OSGB36:     www.ordnancesurvey.co.uk/docs/support/guide-coordinate-systems-great-britain.pdf
     * - Potsdam:    kartoweb.itc.nl/geometrics/Coordinate%20transformations/coordtrans.html
     * - TokyoJapan: www.geocachingtoolbox.com?page=datumEllipsoidDetails
     * - WGS72:      www.icao.int/safety/pbn/documentation/eurocontrol/eurocontrol wgs 84 implementation manual.pdf
     *
     * more transform parameters are available from earth-info.nga.mil/GandG/coordsys/datums/NATO_DT.pdf,
     * www.fieldenmaps.info/cconv/web/cconv_params.js
     */
    /* note:
     * - ETRS89 reference frames are coincident with WGS-84 at epoch 1989.0 (ie null transform) at the one metre level.
     */


    //// freeze static properties
    //Object.keys(ellipsoids).forEach(e => Object.freeze(ellipsoids[e]));
    //Object.keys(datums).forEach(d => { Object.freeze(datums[d]); Object.freeze(datums[d].transform); });


    /* LatLon - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */


    public enum ellipsoidEnum { WGS84, Airy1830, AiryModified, Bessel1841, Clarke1866, Clarke1880IGN, GRS80, Intl1924, WGS72 };
    public enum datumEnum { ED50, Irl1975, NAD27, NAD83, NTF, OSGB36, Potsdam, TokyoJapan, WGS72, WGS84 };


    /**
     * Latitude/longitude points on an ellipsoidal model earth, with ellipsoid parameters and methods
     * for converting between datums and to geocentric (ECEF) cartesian coordinates.
     *
     * @extends LatLonEllipsoidal
     */
    public class LatLonEllipsoidal_Datum : LatLonEllipsoidal
    {
        //newEllipsoidTransform _datum;

        /*
         * Ellipsoid parameters; exposed through static getter below.
         */
        public new static readonly Dictionary<ellipsoidEnum, Ellipsoid> _ellipsoids = new Dictionary<ellipsoidEnum, Ellipsoid>() {
                { ellipsoidEnum.WGS84, new Ellipsoid(6378137, 6356752.314245, 1 / 298.257223563) },
                { ellipsoidEnum.Airy1830, new Ellipsoid( 6377563.396, 6356256.909,   1 / 299.3249646   ) },
                { ellipsoidEnum.AiryModified, new Ellipsoid( 6377340.189, 6356034.448,   1 / 299.3249646   ) },
                { ellipsoidEnum.Bessel1841, new Ellipsoid( 6377397.155, 6356078.962818, 1 / 299.1528128   ) },
                { ellipsoidEnum.Clarke1866, new Ellipsoid( 6378206.4,  6356583.8,     1 / 294.978698214 ) },
                { ellipsoidEnum.Clarke1880IGN, new Ellipsoid( 6378249.2,  6356515.0,     1 / 293.466021294 ) },
                { ellipsoidEnum.GRS80, new Ellipsoid( 6378137,    6356752.314140, 1 / 298.257222101 ) },
                { ellipsoidEnum.Intl1924, new Ellipsoid( 6378388,    6356911.946,   1 / 297           ) }, // aka Hayford
                { ellipsoidEnum.WGS72, new Ellipsoid( 6378135,    6356750.5,     1 / 298.26        ) },
        };

        /*
         * Datums; exposed through static getter below.
         */
        public static new readonly Dictionary<datumEnum, EllipsoidTransform> _datums = new Dictionary<datumEnum, EllipsoidTransform>()
        {
            // transforms: t in metres, s in ppm, r in arcseconds              tx       ty        tz       s        rx        ry        rz
            // en.wikipedia.org/wiki/European_Terrestrial_Reference_System_1989
            { datumEnum.ED50, new EllipsoidTransform ( datumEnum.ED50,  ellipsoidEnum.Intl1924,      new Transform( 89.5, 93.8, 123.1, -1.2, 0.0, 0.0, 0.156) ) }, // epsg.io/1311
            { datumEnum.Irl1975, new EllipsoidTransform ( datumEnum.Irl1975,  ellipsoidEnum.AiryModified,  new Transform( -482.530, 130.596, -564.557, -8.150, 1.042, 0.214, 0.631) ) }, // epsg.io/1954
            { datumEnum.NAD27, new EllipsoidTransform ( datumEnum.NAD27, ellipsoidEnum.Clarke1866,    new Transform( 8, -160, -176, 0, 0, 0, 0) ) },
            { datumEnum.NAD83, new EllipsoidTransform ( datumEnum.NAD83,  ellipsoidEnum.GRS80,         new Transform( 0.9956, -1.9103, -0.5215, -0.00062, 0.025915, 0.009426, 0.011599) ) },
            { datumEnum.NTF, new EllipsoidTransform (  datumEnum.NTF, ellipsoidEnum.Clarke1880IGN, new Transform( 168, 60, -320, 0, 0, 0, 0) ) },
            { datumEnum.OSGB36, new EllipsoidTransform ( datumEnum.OSGB36, ellipsoidEnum.Airy1830,      new Transform( -446.448, 125.157, -542.060, 20.4894, -0.1502, -0.2470, -0.8421) ) }, // epsg.io/1314
            { datumEnum.Potsdam, new EllipsoidTransform (datumEnum.Potsdam,  ellipsoidEnum.Bessel1841,    new Transform( -582, -105, -414, -8.3, 1.04, 0.35, -3.08) ) },
            { datumEnum.TokyoJapan, new EllipsoidTransform ( datumEnum.TokyoJapan,  ellipsoidEnum.Bessel1841,    new Transform( 148, -507, -685, 0, 0, 0, 0) ) },
            { datumEnum.WGS72, new EllipsoidTransform ( datumEnum.WGS72, ellipsoidEnum.WGS72,         new Transform( 0, 0, -4.5, -0.22, 0, 0, 0.554) ) },
            { datumEnum.WGS84, new EllipsoidTransform ( datumEnum.WGS84,  ellipsoidEnum.WGS84,         new Transform( 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0) ) },
        };


        /**
         * Creates a geodetic latitude/longitude point on an ellipsoidal model earth using given datum.
         *
         * @param {number} lat - Latitude (in degrees).
         * @param {number} lon - Longitude (in degrees).
         * @param {number} [height=0] - Height above ellipsoid in metres.
         * @param {LatLon.datums} datum - Datum this point is defined within.
         *
         * @example
         *   import LatLon from '/js/geodesy/latlon-ellipsoidal-datum.js';
         *   const p = new LatLon(53.3444, -6.2577, 17, LatLon.datums.Irl1975);
         */
        public LatLonEllipsoidal_Datum(double lat, double lon, double height = 0, datumEnum datum = datumEnum.WGS84) : base(lat, lon, height)
        {
            //if (!datum || datum.ellipsoid == undefined) throw new TypeError(`unrecognised datum ‘${ datum }’`);

            this.datum = LatLonEllipsoidal_Datum.datums[datum];
        }


        /**
         * Datum this point is defined within.
         */
        //get datum() {
        //    return this._datum;
        //}


        /**
         * Ellipsoids with their parameters; semi-major axis (a), semi-minor axis (b), and flattening (f).
         *
         * Flattening f = (a−b)/a; at least one of these parameters is derived from defining constants.
         *
         * @example
         *   const a = LatLon.ellipsoids.Airy1830.a; // 6377563.396
         */
        public new static Dictionary<ellipsoidEnum, Ellipsoid> ellipsoids
        {
            get
            {
                return _ellipsoids;
            }
        }


        /**
         * Datums; with associated ellipsoid, and Helmert transform parameters to convert from WGS-84
         * into given datum.
         *
         * Note that precision of various datums will vary, and WGS-84 (original) is not defined to be
         * accurate to better than ±1 metre. No transformation should be assumed to be accurate to
         * better than a metre, for many datums somewhat less.
         *
         * This is a small sample of commoner datums from a large set of historical datums. I will add
         * new datums on request.
         *
         * @example
         *   const a = LatLon.datums.OSGB36.ellipsoid.a;                    // 6377563.396
         *   const tx = LatLon.datums.OSGB36.transform;                     // [ tx, ty, tz, s, rx, ry, rz ]
         *   const availableDatums = Object.keys(LatLon.datums).join(', '); // ED50, Irl1975, NAD27, ...
         */
        public new static Dictionary<datumEnum, EllipsoidTransform> datums
        {
            get
            {
                return _datums;
            }
        }


        // note instance datum getter/setters are in LatLonEllipsoidal


        /**
         * Parses a latitude/longitude point from a variety of formats.
         *
         * Latitude & longitude (in degrees) can be supplied as two separate parameters, as a single
         * comma-separated lat/lon string, or as a single object with { lat, lon } or GeoJSON properties.
         *
         * The latitude/longitude values may be numeric or strings; they may be signed decimal or
         * deg-min-sec (hexagesimal) suffixed by compass direction (NSEW); a variety of separators are
         * accepted. Examples -3.62, '3 37 12W', '3°37′12″W'.
         *
         * Thousands/decimal separators must be comma/dot; use Dms.fromLocale to convert locale-specific
         * thousands/decimal separators.
         *
         * @param   {number|string|Object} lat|latlon - Geodetic Latitude (in degrees) or comma-separated lat/lon or lat/lon object.
         * @param   {number}               [lon] - Longitude in degrees.
         * @param   {number}               [height=0] - Height above ellipsoid in metres.
         * @param   {LatLon.datums}        [datum=WGS84] - Datum this point is defined within.
         * @returns {LatLon} Latitude/longitude point on ellipsoidal model earth using given datum.
         * @throws  {TypeError} Unrecognised datum.
         *
         * @example
         *   const p = LatLon.parse('51.47736, 0.0000', 0, LatLon.datums.OSGB36);
         */
        //static parse(...args)
        //{
        //    let datum = datums.WGS84;

        //    // if the last argument is a datum, use that, otherwise use default WGS-84
        //    if (args.length == 4 || (args.length == 3 && typeof args[2] == 'object')) datum = args.pop();

        //    if (!datum || datum.ellipsoid == undefined) throw new TypeError(`unrecognised datum ‘${ datum }’`);

        //const point = super.parse(...args);

        //point._datum = datum;

        //return point;
        //}


        /**
         * Converts ‘this’ lat/lon coordinate to new coordinate system.
         *
         * @param   {LatLon.datums} toDatum - Datum this coordinate is to be converted to.
         * @returns {LatLon} This point converted to new datum.
         * @throws  {TypeError} Unrecognised datum.
         *
         * @example
         *   const pWGS84 = new LatLon(51.47788, -0.00147, 0, LatLon.datums.WGS84);
         *   const pOSGB = pWGS84.convertDatum(LatLon.datums.OSGB36); // 51.4773°N, 000.0001°E
         */
        public LatLonEllipsoidal convertDatum(datumEnum toDatum)
        {
            //if (!toDatum || toDatum.ellipsoid == undefined) throw new TypeError(`unrecognised datum ‘${ toDatum }’`);

            Cartesian_Datum oldCartesian = this.toCartesian();                 // convert geodetic to cartesian
            Cartesian_Datum newCartesian = oldCartesian.convertDatum(toDatum); // convert datum
            var newLatLon = newCartesian.toLatLon();               // convert cartesian back to geodetic

            return newLatLon;
        }


        /**
         * Converts ‘this’ point from (geodetic) latitude/longitude coordinates to (geocentric) cartesian
         * (x/y/z) coordinates.
         *
         * @returns {Cartesian} Cartesian point equivalent to lat/lon point, with x, y, z in metres from
         *   earth centre.
         */
        public new Cartesian_Datum toCartesian()
        {
            var cartesian = base.toCartesian();
            var cartesianDatum = new Cartesian_Datum(cartesian.x, cartesian.y, cartesian.z, this.datum.datumNum);
            return cartesianDatum;
        }

    }

}
