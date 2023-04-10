using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenPanoramaLib
{
    public class LidarTFW
    {
        // World files for raster datasets - General format of the TFW file.
        // http://webhelp.esri.com/arcgisdesktop/9.2/index.cfm?tocVisable=0&ID=2676&TopicName=World%20files%20for%20raster%20datasets&pid=2664
        public double xScale;   // a - X-Scale
        public double rot1;     // D = rotation terms
        public double rot2;     // B = rotation terms
        public double NegY;     // E = negative of y-scale; dimension of a pixel in map units in y direction
        public double xllcorner;// C -  x, y map coordinates
        public double yllcorner;// F -  x, y map coordinates

        public override string ToString()
        {
            return xllcorner + ", " + yllcorner + " XScale " + xScale + " YScale " + NegY + " Rotations " + rot1 + " " + rot2;
        }
    }
}
