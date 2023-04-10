using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenPanoramaLib
{
    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -    */
    /* SunMoonRunJob Class (c) David Hoyle 2020                                                                     */
    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -    */
    public class SunMoonRunJob : IComparer<SunMoonRunJob>
    {
        //"URL,C,54","NAME,C,80","TYPEID,N,9,0","TYPE,C,60","MAP_REF,C,9","LONGITUDE,N,9,6","LATITUDE,N,9,6","COUNTY,C,35","ACCURACY,N,9,0","CONDITION,N,9,0","AMBIENCE,N,9,0","ACCESS,N,9,0","IMAGE,C,9","COLOUR,C,9"
        //"26124","A55 Erratic","36","Natural Stone / Erratic / Other Natural Feature","SH375759","-4.437364","53.255155","Anglesey","0","5","4","5","56757","tr"
        //"6393","Aber Camddwr","7","Cairn","SN751870","-
        public string url;
        public string name;
        public int typeid;
        public string typestr;
        public string mapref;
        public double lat;
        public double lon;
        public string county;
        public int accuracy;
        public int condition;
        public int ambience;
        public int access;
        public string image;
        public string colour;

        public RunJobParams rjParams;

        public DateTime startTime;

        public SunMoonRunJob(string theUrl, string theName, int theTypeID, string theTypeStr, string theMapref, double theLat, double theLon, string theCounty, int theAccuracy, int theCondition, int theAmbience, int theAccess, string theImage, string theColour, RunJobParams rjPs)
        {
            url = theUrl;
            name = theName;
            typeid = theTypeID;
            typestr = theTypeStr;
            mapref = theMapref;
            lat = theLat;
            lon = theLon;
            county = theCounty;
            accuracy = theAccuracy;
            condition = theCondition;
            ambience = theAmbience;
            access = theAccess;
            image = theImage;
            colour = theColour;

            rjParams = rjPs.Clone();

            startTime = DateTime.Now;
        }


        public int Compare(SunMoonRunJob x, SunMoonRunJob y)
        {
            if (x == null || y == null)
            {
                return 0;
            }

            // CompareTo() method 
            return x.name.CompareTo(y.name);
        }
    }
}
