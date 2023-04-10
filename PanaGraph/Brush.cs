using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanaGraph
{
    public class Brush
    {
        public SKPaint paint;

        public Brush(Color c)
        {
            paint = new SkiaSharp.SKPaint();
            paint.Color = new SKColor(c.R, c.G, c.B, c.A);
        }
    }
}
