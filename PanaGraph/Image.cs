using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace PanaGraph
{
    public class Image : IDisposable
    {
        public int width = 0;
        public int height = 0;

        public SKBitmap bm;
        public PaintVerticalLineUnsafe linePainter;

        public int Width
        {
            get { return width; }
        }

        public int Height
        {
            get { return height; }
        }


        public Image()
        {
            this.width = 0;
            this.height = 0;
        }


        public Image ( int awidth, int aheight )
        {
            this.width = awidth;
            this.height = aheight;
            bm = new SKBitmap(Width, Height);
            linePainter = new PaintVerticalLineUnsafe(bm);
        }


        public Image(string filename)
        {
            bm = SKBitmap.Decode(filename);
            this.width = bm.Width;
            this.height = bm.Height;
            linePainter = new PaintVerticalLineUnsafe(bm);
        }


        //public static Image FromFile(string filename)
        //{
        //    return FromFile(filename, useEmbeddedColorManagement: false);
        //}

        //public static Image FromFile(string filename, bool useEmbeddedColorManagement)
        //{
        //    if (!File.Exists(filename))
        //    {
        //        throw new FileNotFoundException(filename);
        //    }

        //    Image image2 = new Image(filename);
        //    return image2;
        //}


        public Color GetPixel(int x, int y)
        {
            int argb = 0;
            if (x < 0 || x >= Width)
            {
                throw new Exception(" ArgumentOutOfRange x " + x);
            }

            if (y < 0 || y >= Height)
            {
                throw new Exception(" ArgumentOutOfRange y " + y);
            }

            SKColor c = bm.GetPixel(x, y);
            return Color.FromArgb(c.Red, c.Green, c.Blue ); 
        }


        public void Save(string f)
        {
            string lowers = f.ToLower();
            SKEncodedImageFormat fmt = SKEncodedImageFormat.Jpeg;
            if (lowers.Contains("jpg"))
            {
                fmt = SKEncodedImageFormat.Jpeg;

                //using (Stream binwrite = File.Create(f + ".png"))
                //{
                //    bm.Encode(binwrite, SKEncodedImageFormat.Png, 100);
                //}
            }
            else if (lowers.Contains("png"))
            {
                fmt = SKEncodedImageFormat.Png;
            }

            using (Stream binwrite = File.Create(f))
            {
                bm.Encode(binwrite, fmt, 100);
            }
            //Console.WriteLine("Bitmap Save " + f);
        }


        public void Dispose()
        {
        }

    }
}
