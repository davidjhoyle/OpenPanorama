using PanaGraph;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenPanoramaLib
{
    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -    */
    /* Stellarium Landscape Generation Handling Classes (c) David Hoyle 2020 : MIT Licensed                         */
    /*  Program - main entry point and command line handling                                                        */
    /*  StoneSite - Class for the JSON data from the sites.txt file                                                 */
    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -    */

    public class StoneSite
    {
        public string Name;
        public string MonType;
        public string County;
        public string GridRef;
        public string Filename;
        public string LatLon;
        public string Height;
        public string Description;
        public string SourceURL;
        public string SourceComment;


        public StoneSite(string name, string description, string filename, double lat, double lon, double height, string county, string montype, string gridref)
        {
            Name = name;
            Description = description;
            Filename = filename;
            LatLon = lat + " " + lon;
            Height = "" + height;
            County = county;
            MonType = montype;
            GridRef = gridref;
        }
    }
}