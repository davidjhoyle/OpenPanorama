using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace OpenPanoramaLib
{
    /* Cartesian  - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */
    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -  */
    /* Geodesy tools for an ellipsoidal earth model                       (c) Chris Veness 2005-2016  */
    /*                                                                                   MIT Licence  */
    /* www.movable-type.co.uk/scripts/geodesy/docs/module-latlon-ellipsoidal-datum-Cartesian_Datum.html                     */
    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -  */

    /**
     * Augments Cartesian with datum the cooordinate is based on, and methods to convert between datums
     * (using Helmert 7-parameter transforms) and to convert cartesian to geodetic latitude/longitude
     * point.
     *
     * @extends Cartesian
     */
    public class Cartesian_Datum : Cartesian
    {
        EllipsoidTransform _datum;
        datumEnum _datumEnum;

        /**
         * Creates cartesian coordinate representing ECEF (earth-centric earth-fixed) point, on a given
         * datum. The datum will identify the primary meridian (for the x-coordinate), and is also
         * useful in transforming to/from geodetic (lat/lon) coordinates.
         *
         * @param  {number} x - X coordinate in metres (=> 0°N,0°E).
         * @param  {number} y - Y coordinate in metres (=> 0°N,90°E).
         * @param  {number} z - Z coordinate in metres (=> 90°N).
         * @param  {LatLon.datums} [datum] - Datum this coordinate is defined within.
         * @throws {TypeError} Unrecognised datum.
         *
         * @example
         *   import { Cartesian } from '/js/geodesy/latlon-ellipsoidal-datum.js';
         *   const coord = new Cartesian(3980581.210, -111.159, 4966824.522);
         */
        public Cartesian_Datum(double x, double y, double z, datumEnum datum = datumEnum.WGS84) : base(x, y, z)
        {
            _datumEnum = datum;
            _datum = LatLonEllipsoidal_Datum.datums[datum];
        }


        /**
         * Datum this point is defined within.
         */
        public EllipsoidTransform datum
        {
            get
            {
                return _datum;
            }
            set
            {
                for (int de = 0; de < LatLonEllipsoidal_Datum.datums.Count; de++)
                {
                    if (LatLonEllipsoidal_Datum.datums[(datumEnum)de].datumNum == (datumEnum)de)
                    {
                        _datumEnum = (datumEnum)de;
                    }
                }
                _datum = value;
            }
        }


        /**
         * Converts ‘this’ (geocentric) cartesian (x/y/z) coordinate to (geodetic) latitude/longitude
         * point (based on the same datum, or WGS84 if unset).
         *
         * Shadow of Cartesian.toLatLon(), returning LatLon augmented with LatLonEllipsoidal_Datum
         * methods convertDatum, toCartesian, etc.
         *
         * @returns {LatLon} Latitude/longitude point defined by cartesian coordinates.
         * @throws  {TypeError} Unrecognised datum
         *
         * @example
         *   const c = new Cartesian(4027893.924, 307041.993, 4919474.294);
         *   const p = c.toLatLon(); // 50.7978°N, 004.3592°E
         */
        public LatLonEllipsoidal toLatLon()
        {
            var datum = this.datum;

            var latLon = base.toLatLon(datum.ellipsoid); // TODO: what if datum is not geocentric?
            var point = new LatLonEllipsoidal_Datum(latLon.lat, latLon.lon, latLon.height, _datumEnum);
            return point;
        }


        /**
         * Converts ‘this’ cartesian coordinate to new datum using Helmert 7-parameter transformation.
         *
         * @param   {LatLon.datums} toDatum - Datum this coordinate is to be converted to.
         * @returns {Cartesian} This point converted to new datum.
         * @throws  {Error} Undefined datum.
         *
         * @example
         *   const c = new Cartesian(3980574.247, -102.127, 4966830.065, LatLon.datums.OSGB36);
         *   c.convertDatum(LatLon.datums.Irl1975); // [??,??,??]
         */
        public Cartesian_Datum convertDatum(datumEnum toDatum)
        {
            Cartesian_Datum oldCartesian = this;
            var transform = LatLonEllipsoidal_Datum.datums[toDatum].transform;

            // Don't do anything if to and from are the same.
            if (_datumEnum == toDatum)
            {
                return oldCartesian;
            }

            // Convert this to WGS84 first
            if (_datumEnum != datumEnum.WGS84)
            {
                var invtransform = LatLonEllipsoidal_Datum.datums[this.datum.datumNum].invTransform;
                oldCartesian = oldCartesian.applyTransform(invtransform);
            }

            if (oldCartesian.datum.datumNum != toDatum)
            {
                var newCartesian = oldCartesian.applyTransform(transform);
                newCartesian.datum = LatLonEllipsoidal_Datum.datums[toDatum];
                return newCartesian;
            }
            else
            {
                return oldCartesian;
            }
        }


        /**
         * Applies Helmert 7-parameter transformation to ‘this’ coordinate using transform parameters t.
         *
         * This is used in converting datums (geodetic->cartesian, apply transform, cartesian->geodetic).
         *
         * @private
         * @param   {number[]} t - Transformation to apply to this coordinate.
         * @returns {Cartesian} Transformed point.
         */
        public Cartesian_Datum applyTransform(Transform t)
        {
            // this point
            double x1 = x;
            double y1 = y;
            double z1 = z;

            // transform parameters
            double tx = t.tx;                    // x-shift in metres
            double ty = t.ty;                    // y-shift in metres
            double tz = t.tz;                    // z-shift in metres
            double s = t.s / 1e6 + 1;            // scale: normalise parts-per-million to (s+1)
            double rx = Dms.toRadians(t.rx / 3600); // x-rotation: normalise arcseconds to radians
            double ry = Dms.toRadians(t.ry / 3600); // y-rotation: normalise arcseconds to radians
            double rz = Dms.toRadians(t.rz / 3600); // z-rotation: normalise arcseconds to radians

            // apply transform
            double x2 = tx + x1 * s - y1 * rz + z1 * ry;
            double y2 = ty + x1 * rz + y1 * s - z1 * rx;
            double z2 = tz - x1 * ry + y1 * rx + z1 * s;

            return new Cartesian_Datum(x2, y2, z2);
        }
    }


    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -  */

    //export { LatLonEllipsoidal_Datum as default, Cartesian_Datum as Cartesian, datums, Dms };
}
