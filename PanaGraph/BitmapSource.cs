using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanaGraph
{
    public class BitmapSource
    {
        internal Media.PixelFormat _format;
        internal int _pixelWidth;
        internal int _pixelHeight;

        public void CopyPixels(Int32Rect rct, byte[] bytes, int bytesPerPixel, int spare)
        {
        }

        public Media.PixelFormat Format
        {
            get
            {
                return _format;
            }
        }

        int _width = 0;
        int _height = 0;
            
        public int Width
        {
            get { return _width; }
        }

        public int Height
        {
            get { return _height; }
        }
    }
}
