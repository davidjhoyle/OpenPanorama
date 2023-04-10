using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using SkiaSharp;



namespace PanaGraph
{
    //public class Bitmap : Image
    //{
    //    public SKBitmap me = null;

    //    public Bitmap(int ImageRawWidth, int ImageRawHeight, PixelFormat fmt) : base (ImageRawWidth, ImageRawHeight )
    //    {
    //        me = new SKBitmap(ImageRawWidth, ImageRawHeight, true);
    //    }


    //    public Bitmap(string filename)
    //    {
    //        Console.WriteLine("Bitmap Read " + filename);
    //        me = SKBitmap.Decode(filename);
    //        Width = me.Width;
    //        Height = me.Height;
    //    }


    //    public Bitmap(int width, int height)
    //        : this(width, height, PixelFormat.Format32bppArgb)
    //    {
    //        me = new SKBitmap(width, height);
    //    }


    //    public void Save(string f)
    //    {
    //        string lowers = f.ToLower();
    //        SKEncodedImageFormat fmt = SKEncodedImageFormat.Jpeg;
    //        if ( lowers.Contains( "jpg"))
    //        {
    //            fmt = SKEncodedImageFormat.Jpeg;
    //        }
    //        else if (lowers.Contains("png"))
    //        {
    //            fmt = SKEncodedImageFormat.Png;
    //        }

    //        using (Stream binwrite = File.Create(f))
    //        {
    //            me.Encode(binwrite, fmt, 99);
    //        }
    //        //Console.WriteLine("Bitmap Save " + f);
    //    }

    //    public Color GetPixel(int x, int y)
    //    {
    //        int argb = 0;
    //        if (x < 0 || x >= base.Width)
    //        {
    //            throw new Exception(" ArgumentOutOfRange x " + x );
    //        }

    //        if (y < 0 || y >= base.Height)
    //        {
    //            throw new Exception(" ArgumentOutOfRange y " + y);
    //        }

    //        throw new Exception("GetPixel not implemented");

    //        return default(Color);
    //        //return Color.FromArgb(0);
    //    }

    //}
}
