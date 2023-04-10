using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace OpenPanoramaLib
{
    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -  */
    /* Ordnance Survey Grid Reference functions                           (c) Chris Veness 2005-2019  */
    /*                                                                                   MIT Licence  */
    /* www.movable-type.co.uk/scripts/latlong-gridref.html                                            */
    /* www.movable-type.co.uk/scripts/geodesy-library.html#osgridref                                  */
    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -  */

    /* LatLonEllipsoidal - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -  */


    /**
     * Latitude/longitude points on an ellipsoidal model earth, with ellipsoid parameters and methods
     * for converting points to/from cartesian (ECEF) coordinates.
     *
     * This is the core class, which will usually be used via LatLonEllipsoidal_Datum or
     * LatLonEllipsoidal_ReferenceFrame.
     */
    public class LatLonEllipsoidal
    {
        double _lat;
        double _lon;
        double _height;
        EllipsoidTransform _datum;


        /*
         * Ellipsoid parameters; exposed through static getter below.
         *
         * The only ellipsoid defined is WGS84, for use in utm/mgrs, vincenty, nvector.
         */
        public static readonly Dictionary<ellipsoidEnum, Ellipsoid> _ellipsoids = new Dictionary<ellipsoidEnum, Ellipsoid>() {
                { ellipsoidEnum.WGS84, new Ellipsoid(6378137, 6356752.314245, 1 / 298.257223563) } };

        /*
         * Datums; exposed through static getter below.
         *
         * The only datum defined is WGS84, for use in utm/mgrs, vincenty, nvector.
         */
        public static readonly Dictionary<datumEnum, EllipsoidTransform> _datums = new Dictionary<datumEnum, EllipsoidTransform>()
        {
            { datumEnum.WGS84, new EllipsoidTransform ( datumEnum.WGS84, ellipsoidEnum.WGS84, new Transform( 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0) ) }
        };


        /**
         * Creates a geodetic latitude/longitude point on a (WGS84) ellipsoidal model earth.
         *
         * @param  {number} lat - Latitude (in degrees).
         * @param  {number} lon - Longitude (in degrees).
         * @param  {number} [height=0] - Height above ellipsoid in metres.
         * @throws {TypeError} Invalid lat/lon/height.
         *
         * @example
         *   import LatLon from '/js/geodesy/latlon-ellipsoidal.js';
         *   const p = new LatLon(51.47788, -0.00147, 17);
         */
        public LatLonEllipsoidal(double lat, double lon, double height = 0)
        {
            //if (IsNaN(lat)) throw new TypeError(`invalid lat ‘${ lat }’`);
            //if (isNaN(lon)) throw new TypeError(`invalid lon ‘${ lon }’`);
            //if (isNaN(height)) throw new TypeError(`invalid height ‘${ height }’`);

            this._lat = Dms.wrap90((lat));
            this._lon = Dms.wrap180((lon));
            this._height = (height);
        }


        /**
         * Latitude in degrees north from equator (including aliases lat, latitude): can be set as
         * numeric or hexagesimal (deg-min-sec); returned as numeric.
         */
        public double lat
        {
            get
            {
                return _lat;
            }
            set
            {
                _lat = Dms.wrap90(value);
            }
        }
        public double latitude
        {
            get
            {
                return _lat;
            }
            set
            {
                _lat = Dms.wrap90(value);
            }
        }

        /**
            * Longitude in degrees east from international reference meridian (including aliases lon, lng,
            * longitude): can be set as numeric or hexagesimal (deg-min-sec); returned as numeric.
            */
        public double lon
        {
            get
            {
                return _lon;
            }
            set
            {
                _lon = Dms.wrap180(value);
            }
        }

        public double longitude
        {
            get
            {
                return _lon;
            }
            set
            {
                _lon = Dms.wrap180(value);
            }
        }

        /**
        * Height in metres above ellipsoid.
        */
        public double height
        {
            get
            {
                return _height;
            }
            set
            {
                _height = value;
            }
        }

        /**
         * Datum.
         *
         * Note this is replicated within LatLonEllipsoidal in order that a LatLonEllipsoidal object can
         * be monkey-patched to look like a LatLonEllipsoidal_Datum, for Vincenty calculations on
         * different ellipsoids.
         *
         * @private
         */
        public EllipsoidTransform datum
        {
            get
            {
                return _datum;
            }
            set
            {
                _datum = value;
            }
        }

        /**
         * Ellipsoids with their parameters; this module only defines WGS84 parameters a = 6378137, b =
         * 6356752.314245, f = 1/298.257223563.
         *
         * @example
         *   const a = LatLon.ellipsoids.WGS84.a; // 6378137
         */
        public static Dictionary<ellipsoidEnum, Ellipsoid> ellipsoids
        {
            get
            {
                return _ellipsoids;
            }
        }

        /**
         * Datums; this module only defines WGS84 datum, hence no datum transformations.
         *
         * @example
         *   const a = LatLon.datums.WGS84.ellipsoid.a; // 6377563.396
         */
        public static Dictionary<datumEnum, EllipsoidTransform> datums
        {
            get
            {
                return _datums;
            }
        }


        /**
         * Parses a latitude/longitude point from a variety of formats.
         *
         * Latitude &amp; longitude (in degrees) can be supplied as two separate parameters, as a single
         * comma-separated lat/lon string, or as a single object with { lat, lon } or GeoJSON properties.
         *
         * The latitude/longitude values may be numeric or strings; they may be signed decimal or
         * deg-min-sec (hexagesimal) suffixed by compass direction (NSEW); a variety of separators are
         * accepted. Examples -3.62, '3 37 12W', '3°37′12″W'.
         *
         * Thousands/decimal separators must be comma/dot; use Dms.fromLocale to convert locale-specific
         * thousands/decimal separators.
         *
         * @param   {number|string|Object} lat|latlon - Latitude (in degrees), or comma-separated lat/lon, or lat/lon object.
         * @param   {number}               [lon]      - Longitude (in degrees).
         * @param   {number}               [height=0] - Height above ellipsoid in metres.
         * @returns {LatLon} Latitude/longitude point on WGS84 ellipsoidal model earth.
         * @throws  {TypeError} Invalid coordinate.
         *
         * @example
         *   const p1 = LatLon.parse(51.47788, -0.00147);              // numeric pair
         *   const p2 = LatLon.parse('51°28′40″N, 000°00′05″W', 17);   // dms string + height
         *   const p3 = LatLon.parse({ lat: 52.205, lon: 0.119 }, 17); // { lat, lon } object numeric + height
         */
        //        static parse(...args)
        //        {
        //            if (args.length == 0) throw new TypeError('invalid (empty) point');

        //            let lat = undefined, lon = undefined, height = undefined;

        //            // single { lat, lon } object
        //            if (typeof args[0]== 'object' & amp; &amp; (args.length == 1 || !isNaN(parseFloat(args[1])))) {
        //                const ll = args[0];
        //                if (ll.type == 'Point' & amp; &amp; Array.isArray(ll.coordinates)) { // GeoJSON
        //                [lon, lat, height] = ll.coordinates;
        //                height = height || 0;
        //            } else { // regular { lat, lon } object
        //                if (ll.latitude  != undefined) lat = ll.latitude;
        //                if (ll.lat       != undefined) lat = ll.lat;
        //                if (ll.longitude != undefined) lon = ll.longitude;
        //                if (ll.lng       != undefined) lon = ll.lng;
        //                if (ll.lon       != undefined) lon = ll.lon;
        //                if (ll.height    != undefined) height = ll.height;
        //                lat = Dms.wrap90(Dms.parse(lat));
        //                lon = Dms.wrap180(Dms.parse(lon));
        //            }
        //            if (args[1] != undefined) height = args[1];
        //            if (isNaN(lat) || isNaN(lon)) throw new TypeError(`invalid point ‘${ JSON.stringify(args[0])}’`);
        //        }

        //        // single comma-separated lat/lon
        //        if (typeof args[0] == 'string' &amp;&amp; args[0].split(',').length == 2) {
        //            [lat, lon] = args[0].split(',');
        //lat = Dms.wrap90(Dms.parse(lat));
        //            lon = Dms.wrap180(Dms.parse(lon));
        //            height = args[1] || 0;
        //            if (isNaN(lat) || isNaN(lon)) throw new TypeError(`invalid point ‘${ args[0]}’`);
        //        }

        //        // regular (lat, lon) arguments
        //        if (lat==undefined &amp;&amp; lon==undefined) {
        //            [lat, lon] = args;
        //            lat = Dms.wrap90(Dms.parse(lat));
        //            lon = Dms.wrap180(Dms.parse(lon));
        //            height = args[2] || 0;
        //            if (isNaN(lat) || isNaN(lon)) throw new TypeError(`invalid point ‘${ args.toString()}’`);
        //        }

        //        return new this(lat, lon, height); // 'new this' as may return subclassed types
        //    }


        /**
         * Converts ‘this’ point from (geodetic) latitude/longitude coordinates to (geocentric)
         * cartesian (x/y/z) coordinates.
         *
         * @returns {Cartesian} Cartesian point equivalent to lat/lon point, with x, y, z in metres from
         *   earth centre.
         */
        public Cartesian_Datum toCartesian()
        {
            // x = (ν+h)⋅cosφ⋅cosλ, y = (ν+h)⋅cosφ⋅sinλ, z = (ν⋅(1-e²)+h)⋅sinφ
            // where ν = a/√(1−e²⋅sinφ⋅sinφ), e² = (a²-b²)/a² or (better conditioned) 2⋅f-f²
            var ellipsoid = this.datum.ellipsoid;

            double φ = Dms.toRadians(this.lat);
            double λ = Dms.toRadians(this.lon);
            double h = this.height;
            double a = ellipsoid.a;
            double f = ellipsoid.f;

            double sinφ = Math.Sin(φ), cosφ = Math.Cos(φ);
            double sinλ = Math.Sin(λ), cosλ = Math.Cos(λ);

            double eSq = 2 * f - f * f;                      // 1st eccentricity squared ≡ (a²-b²)/a²
            double ν = a / Math.Sqrt(1 - eSq * sinφ * sinφ); // radius of curvature in prime vertical

            double x = (ν + h) * cosφ * cosλ;
            double y = (ν + h) * cosφ * sinλ;
            double z = (ν * (1 - eSq) + h) * sinφ;

            return new Cartesian_Datum(x, y, z);
        }


        ///**
        // * Checks if another point is equal to ‘this’ point.
        // *
        // * @param   {LatLon} point - Point to be compared against this point.
        // * @returns {bool} True if points have identical latitude, longitude, height, and datum/referenceFrame.
        // * @throws  {TypeError} Invalid point.
        // *
        // * @example
        // *   const p1 = new LatLon(52.205, 0.119);
        // *   const p2 = new LatLon(52.205, 0.119);
        // *   const equal = p1.equals(p2); // true
        // */
        //equals(point)
        //{
        //    if (!(point instanceof LatLonEllipsoidal)) throw new TypeError(`invalid point ‘${ point }’`);

        //    if (Math.abs(this.lat - point.lat) > Number.EPSILON) return false;
        //    if (Math.abs(this.lon - point.lon) > Number.EPSILON) return false;
        //    if (Math.abs(this.height - point.height) > Number.EPSILON) return false;
        //    if (this.datum != point.datum) return false;
        //    if (this.referenceFrame != point.referenceFrame) return false;
        //    if (this.epoch != point.epoch) return false;

        //    return true;
        //}


        ///**
        // * Returns a string representation of ‘this’ point, formatted as degrees, degrees+minutes, or
        // * degrees+minutes+seconds.
        // *
        // * @param   {string} [format=d] - Format point as 'd', 'dm', 'dms', or 'n' for signed numeric.
        // * @param   {number} [dp=4|2|0] - Number of decimal places to use: default 4 for d, 2 for dm, 0 for dms.
        // * @param   {number} [dpHeight=null] - Number of decimal places to use for height; default is no height display.
        // * @returns {string} Comma-separated formatted latitude/longitude.
        // * @throws  {RangeError} Invalid format.
        // *
        // * @example
        // *   const greenwich = new LatLon(51.47788, -0.00147, 46);
        // *   const d = greenwich.toString();                        // 51.4779°N, 000.0015°W
        // *   const dms = greenwich.toString('dms', 2);              // 51°28′40″N, 000°00′05″W
        // *   const [lat, lon] = greenwich.toString('n').split(','); // 51.4779, -0.0015
        // *   const dmsh = greenwich.toString('dms', 0, 0);          // 51°28′40″N, 000°00′06″W +46m
        // */
        //toString(format= 'd', dp= undefined, dpHeight= null)
        //{
        //    // note: explicitly set dp to undefined for passing through to toLat/toLon
        //    if (!['d', 'dm', 'dms', 'n'].includes(format)) throw new RangeError(`invalid format ‘${ format }’`);

        //    const height = (this.height >= 0 ? ' +' : ' ') + this.height.toFixed(dpHeight) + 'm';
        //    if (format == 'n')
        //    { // signed numeric degrees
        //        if (dp == undefined) dp = 4;
        //        const lat = this.lat.toFixed(dp);
        //        const lon = this.lon.toFixed(dp);
        //        return `${ lat}, ${ lon}${ dpHeight == null ? '' : height}`;
        //    }

        //    const lat = Dms.toLat(this.lat, format, dp);
        //    const lon = Dms.toLon(this.lon, format, dp);

        //    return `${ lat}, ${ lon}${ dpHeight == null ? '' : height}`;
        //}




        /* Cartesian  - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */


        ///**
        // * ECEF (earth-centered earth-fixed) geocentric cartesian coordinates.
        // *
        // * @extends Vector3d
        // */
        //class Cartesian extends Vector3d
        //{

        //    /**
        //     * Creates cartesian coordinate representing ECEF (earth-centric earth-fixed) point.
        //     *
        //     * @param {number} x - X coordinate in metres (=> 0°N,0°E).
        //     * @param {number} y - Y coordinate in metres (=> 0°N,90°E).
        //     * @param {number} z - Z coordinate in metres (=> 90°N).
        //     *
        //     * @example
        //     *   import { Cartesian } from '/js/geodesy/latlon-ellipsoidal.js';
        //     *   const coord = new Cartesian(3980581.210, -111.159, 4966824.522);
        //     */
        //    constructor(x, y, z) {
        //        super(x, y, z); // arguably redundant constructor, but specifies units &amp; axes
        //    }


        //    /**
        //     * Converts ‘this’ (geocentric) cartesian (x/y/z) coordinate to (geodetic) latitude/longitude
        //     * point on specified ellipsoid.
        //     *
        //     * Uses Bowring’s (1985) formulation for μm precision in concise form; ‘The accuracy of geodetic
        //     * latitude and height equations’, B R Bowring, Survey Review vol 28, 218, Oct 1985.
        //     *
        //     * @param   {LatLon.ellipsoids} [ellipsoid=WGS84] - Ellipsoid to use when converting point.
        //     * @returns {LatLon} Latitude/longitude point defined by cartesian coordinates, on given ellipsoid.
        //     * @throws  {TypeError} Invalid ellipsoid.
        //     *
        //     * @example
        //     *   const c = new Cartesian(4027893.924, 307041.993, 4919474.294);
        //     *   const p = c.toLatLon(); // 50.7978°N, 004.3592°E
        //     */
        //public LatLonEllipsoidal toLatLon(ellipsoidEnum en = ellipsoidEnum.WGS84)
        //{
        //    // note ellipsoid is available as a parameter for when toLatLon gets subclassed to
        //    // Ellipsoidal_Datum / Ellipsoidal_Referenceframe.
        //    //if (!ellipsoid || !ellipsoid.a) throw new TypeError(`invalid ellipsoid ‘${ ellipsoid }’`);
        //    Ellipsoid ellipsoid = LatLonEllipsoidal_Datum.ellipsoids[en];

        //    double x = this.;
        //    double y = this.;
        //    double z = 0;
        //    double a = ellipsoid.a;
        //    double b = ellipsoid.b;
        //    double f = ellipsoid.f;

        //    double e2 = 2 * f - f * f;           // 1st eccentricity squared ≡ (a²−b²)/a²
        //    double ε2 = e2 / (1 - e2);         // 2nd eccentricity squared ≡ (a²−b²)/b²
        //    double p = Math.Sqrt(x * x + y * y); // distance from minor axis
        //    double R = Math.Sqrt(p * p + z * z); // polar radius

        //    // parametric latitude (Bowring eqn.17, replacing tanβ = z·a / p·b)
        //    const tanβ = (b * z) / (a * p) * (1 + ε2 * b / R);
        //    const sinβ = tanβ / Math.sqrt(1 + tanβ * tanβ);
        //    const cosβ = sinβ / tanβ;

        //    // geodetic latitude (Bowring eqn.18: tanφ = z+ε²⋅b⋅sin³β / p−e²⋅cos³β)
        //    const φ = isNaN(cosβ) ? 0 : Math.atan2(z + ε2 * b * sinβ * sinβ * sinβ, p - e2 * a * cosβ * cosβ * cosβ);

        //    // longitude
        //    const λ = Math.atan2(y, x);

        //    // height above ellipsoid (Bowring eqn.7)
        //    const sinφ = Math.sin(φ), cosφ = Math.cos(φ);
        //    const ν = a / Math.sqrt(1 - e2 * sinφ * sinφ); // length of the normal terminated by the minor axis
        //    const h = p * cosφ + z * sinφ - (a * a / ν);

        //    const point = new LatLonEllipsoidal(φ.toDegrees(), λ.toDegrees(), h);

        //    return point;
        //}


        //    /**
        //     * Returns a string representation of ‘this’ cartesian point.
        //     *
        //     * @param   {number} [dp=0] - Number of decimal places to use.
        //     * @returns {string} Comma-separated latitude/longitude.
        //     */
        //    toString(dp=0) {
        //        const x = this.x.toFixed(dp), y = this.y.toFixed(dp), z = this.z.toFixed(dp);
        //        return `[${x},${y},${z}]`;
        //    }
        //}

    }
    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -  */
}
