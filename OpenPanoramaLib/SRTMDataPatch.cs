using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenPanoramaLib
{
    public class SRTMDataPatch
    {
        public int latpart;
        public int lonpart;
        public int z;

        public SRTMDataPatch()
        {
        }


        public SRTMDataPatch(int llatpart, int llonpart, int zz)
        {
            latpart = llatpart;
            lonpart = llonpart;
            z = zz;
        }
    }
}
