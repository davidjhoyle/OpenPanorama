using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PanaGraph;

using BitMiracle.LibTiff;
using BitMiracle.LibTiff.Classic;

namespace OpenPanoramaLib
{
    public class LidarTIF
    {
        public string filename;
        //public BitmapSource bitmap;
        //public BitMiracle.LibTiff. TiffFloat32[] pixels;
        public float[,] pixels;

        public int width;
        public int height;

        //public Stream tiffstream;
        public BitMiracle.LibTiff.Classic.TiffStream tifstream;
        public BitMiracle.LibTiff.Classic.Tiff bmtiff;
        //public BitMiracle.LibTiff.Classic. bmtiff;

        //public TiffFileReader tiff;
        //public TiffImageDecoder decoder;

        public countryEnum cc;
        public LidarTFW tfw;
    }
}
