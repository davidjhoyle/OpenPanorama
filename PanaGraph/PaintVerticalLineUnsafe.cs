using PanaGraph;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanaGraph
{
    unsafe public class PaintVerticalLineUnsafe
    {
        int bmWidth = 0;
        int bmHeight = 0;
        IntPtr pixelsAddr;
        uint* pixelsptr;

        public PaintVerticalLineUnsafe(SKBitmap bm)
        {
            bmWidth = bm.Width;
            bmHeight = bm.Height;
            pixelsAddr = bm.GetPixels();
            pixelsptr = (uint*)pixelsAddr.ToPointer();
        }

        ~PaintVerticalLineUnsafe() // finalizer
        {

        }

        public void VerticalLine(int x, int y1, int y2, uint col)
        {
            if (y2 < y1)
            {
                int tmp = y1;
                y1 = y2;
                y2 = tmp;
            }

            if (y1 < 0)
            {
                y1 = 0;
            }

            if (y2 >= bmHeight)
            {
                y2 = bmHeight - 1;
            }

            if (x < 0 || x >= bmWidth)
            {
                return;
            }

            if (pixelsAddr == (IntPtr) 0)
            {
                return;
            }

            for (int y = y1; y <= y2; y++)
            {
                // Calculate the address and set the value
                uint* ptr = pixelsptr + bmWidth * y + x;
                *ptr = (uint)col;
            }
        }
    }
}
