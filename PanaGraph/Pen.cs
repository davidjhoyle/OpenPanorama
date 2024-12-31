using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

//using SkiaSharp;


namespace PanaGraph
{
    public class Pen
    {
        public Color color;
        public float width;

        public SKPaint paint;
        
        public Pen( Color c, float w)
        {
            color = c;
            width = w;

            paint = new SKPaint();
            paint.Color = new SKColor(c.R, c.G, c.B, c.A);
            paint.IsStroke = true;
            paint.StrokeWidth = w;
            paint.Style = SKPaintStyle.Stroke;
            paint.StrokeWidth = 1;
        }

        public Pen(Brush brush)
            : this(brush, 1f)
        {
        }

        public Pen(Brush brush, float width)
        {
            this.width = width;
        }


        public Pen(Color color)
            : this(color, 1f)
        {
        }

        public Color Color
        {
            get
            {
                return color;
            }
            set
            {
                this.color = value;
            }
        }
    }
}
