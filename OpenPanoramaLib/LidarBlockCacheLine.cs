using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenPanoramaLib
{
    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -  */
    /* LIDAR Handling Classes (c) David Hoyle 2020 : MIT Licensed                                     */
    /* LidarASCCacheLine - Caching for binary LIDAR files */
    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -  */
    public class LidarBlockCacheLine
    {
        public LidarBlock asc;
        public ulong lastAccess;
    }
}
