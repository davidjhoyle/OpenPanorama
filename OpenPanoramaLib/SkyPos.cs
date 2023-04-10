using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenPanoramaLib
{
    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -  */
    /* Sky position Moon and Sun Handling Classes (c) David Hoyle 2020 : MIT Licensed                 */
    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -  */

    public class SkyPos
    {
        public double Elevation;
        public double Azimuth;

        public SkyPos(double e, double a)
        {
            Elevation = e;
            Azimuth = a;
        }
    }
}
