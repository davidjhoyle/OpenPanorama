using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace OpenPanoramaLib
{
    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -  */
    /* ECEF cartesian coordinates                                         (c) Chris Veness 2005-2016  */
    /*                                                                                   MIT Licence  */
    /* www.movable-type.co.uk/scripts/latlong.html                                                    */
    /* www.movable-type.co.uk/scripts/geodesy/docs/module-cartesian.html                              */
    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -  */


    //'use strict';
    //if (typeof module!='undefined' &amp;&amp; module.exports) var LatLon = require('./latlon-ellipsoidal.js'); // ≡ import LatLon from 'latlon-ellipsoidal.js'


    /**
     * A cartesian coordinate is an x/y/z value representing the position of a point relative to the
     * centre of the earth (ECEF or earth-centric earth-fixed point). It can be used as a vector with
     * units of metres.
     *
     * @module   cartesian
     * @requires latlon-ellipsoidal (tightly coupled; cyclic dependencies between toLatLon/toCartesian)
     */
    public class Cartesian
    {
        public double x;
        public double y;
        public double z;

        /**
         * Creates cartesian coordinate representing ECEF (earth-centric earth-fixed) point.
         *
         * @constructor
         * @param {number} x - x coordinate in metres.
         * @param {number} y - y coordinate in metres.
         * @param {number} z - z coordinate in metres.
         *
         * @example
         *     var p1 = new Cartesian(3980581.210, -111.159, 4966824.522);
         */
        public Cartesian(double X, double Y, double Z)
        {
            x = X;
            y = Y;
            z = Z;
        }


        /**
         * Converts ‘this’ (geocentric) cartesian (x/y/z) coordinate to (ellipsoidal geodetic)
         * latitude/longitude point on specified datum.
         *
         * Uses Bowring’s (1985) formulation for μm precision.
         *
         * @param {LatLon.datum.transform} [datum=WGS84] - Datum to use when converting point.
         */
        public LatLonEllipsoidal toLatLon(Ellipsoid datum)
        {
            double x = this.x;
            double y = this.y;
            double z = this.z;

            double a = datum.a;
            double b = datum.b;

            var e2 = (a * a - b * b) / (a * a);   // 1st eccentricity squared
            var ε2 = (a * a - b * b) / (b * b);   // 2nd eccentricity squared
            var p = Math.Sqrt(x * x + y * y); // distance from minor axis
            var R = Math.Sqrt(p * p + z * z); // polar radius

            // parametric latitude (Bowring eqn 17, replacing tanβ = z·a / p·b)
            var tanβ = (b * z) / (a * p) * (1 + ε2 * b / R);
            var sinβ = tanβ / Math.Sqrt(1 + tanβ * tanβ);
            var cosβ = sinβ / tanβ;

            // geodetic latitude (Bowring eqn 18: tanφ = z+ε²bsin³β / p−e²cos³β)
            var φ = Math.Atan2(z + ε2 * b * sinβ * sinβ * sinβ, p - e2 * a * cosβ * cosβ * cosβ);

            // longitude
            var λ = Math.Atan2(y, x);

            // height above ellipsoid (Bowring eqn 7)
            var sinφ = Math.Sin(φ);
            var cosφ = Math.Cos(φ);

            var ν = a / Math.Sqrt(1 - e2 * sinφ * sinφ); // length of the normal terminated by the minor axis
            var h = p * cosφ + z * sinφ - (a * a / ν);
            var point = new LatLonEllipsoidal(Dms.toDegrees(φ), Dms.toDegrees(λ), h);

            return point;
        }

        ///**
        // * Applies Helmert (seven-parameter) transformation to ‘this’ coordinate using transform parameters t.
        // *
        // * @param {LatLon.datum.transform} t - Transformation to apply to this coordinate.
        // */
        //Cartesian.prototype.applyTransform = function(t)
        //{
        //    var x1 = this.x, y1 = this.y, z1 = this.z;

        //    var tx = t.tx, ty = t.ty, tz = t.tz;
        //    var rx = (t.rx / 3600).toRadians(); // normalise seconds to radians
        //    var ry = (t.ry / 3600).toRadians(); // normalise seconds to radians
        //    var rz = (t.rz / 3600).toRadians(); // normalise seconds to radians
        //    var s1 = t.s / 1e6 + 1;             // normalise ppm to (s+1)

        //    // apply transform
        //    var x2 = tx + x1 * s1 - y1 * rz + z1 * ry;
        //    var y2 = ty + x1 * rz + y1 * s1 - z1 * rx;
        //    var z2 = tz - x1 * ry + y1 * rx + z1 * s1;

        //    var point = new Cartesian(x2, y2, z2);

        //    return point;
        //};


        ///**
        // * Returns a string representation of ‘this’ cartesian point.
        // *
        // * @param   {number} [dp=0|2|4] - Number of decimal places to use - default 0 for dms, 2 for dm, 4 for d.
        // * @returns {string} Comma-separated latitude/longitude.
        // */
        //Cartesian.prototype.toString = function(dp)
        //{
        //    if (dp == undefined) dp = 0; // default 0 decimals
        //    dp = Number(dp);

        //    return '[' + this.x.toFixed(dp) + ',' + this.y.toFixed(dp) + ',' + this.z.toFixed(dp) + ',' + ']';
        //};


        /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -  */
    }
    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -  */
}
