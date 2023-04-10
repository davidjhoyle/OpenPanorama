using System;
using System.Collections.Generic;
//using System.Drawing;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;


using SkiaSharp;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography;

namespace PanaGraph
{
    public class Graphics
    {
        public SKCanvas me = null;


        public delegate bool DrawImageAbort(IntPtr callbackdata);

        public static Graphics FromImage(Image im)
        {
            Graphics g = new Graphics();
            g.me = new SKCanvas(im.bm);
            return g;
        }

        //public static Graphics FromImage(Image im)
        //{
        //    Graphics g = new Graphics();
        //    g.me = new SKCanvas(im.me);
        //    return g;
        //}


        public void Clear(Color c)
        {
            SKColor col = new SKColor( c.R, c.G, c.B, c.A);
            this.me.Clear(col);
        }

        public void DrawString(string s, Font f, SolidBrush b, Rectangle rct, StringFormat sf)
        {
            //Console.WriteLine("DrawString Native Calls Here " + s);
            if (sf != null)
            {
                if (sf.Alignment == StringAlignment.Center)
                {
                    b.paint.TextAlign = SKTextAlign.Center;
                }
                else if (sf.Alignment == StringAlignment.Far)
                {
                    b.paint.TextAlign = SKTextAlign.Right;
                }
                else
                {
                    b.paint.TextAlign = SKTextAlign.Left;
                }
            }
            else
            {
                b.paint.TextAlign = SKTextAlign.Left;
            }

            int x = rct.Left;
            if (sf != null && sf.Alignment == StringAlignment.Center )
            {
                x += rct.Width / 2;
            }
            int y = rct.Bottom;
            if (sf != null && sf.LineAlignment == StringAlignment.Center )
            {
                y -= (int) ((rct.Height - f.me.Size) / 2);
            }

            me.DrawText(s, x, y, f.me, b.paint);
        }

        public void DrawString(string s, Font font, Brush brush, float x, float y)
        {
            DrawString(s, font, brush, new RectangleF(x, y, 0f, 0f), null);
        }

        public void DrawString(string s, Font f, Brush b, RectangleF layoutRectangle, StringFormat sf)
        {
            if (b == null)
            {
                throw new ArgumentNullException("brush");
            }

            if (sf != null)
            {
                if (sf.Alignment == StringAlignment.Center)
                {
                    b.paint.TextAlign = SKTextAlign.Center;
                }
                else if (sf.Alignment == StringAlignment.Far)
                {
                    b.paint.TextAlign = SKTextAlign.Right;
                }
                else
                {
                    b.paint.TextAlign = SKTextAlign.Left;
                }
            }
            else
            {
                b.paint.TextAlign = SKTextAlign.Left;
            }

            float x = layoutRectangle.X;
            if (sf != null && sf.Alignment == StringAlignment.Center)
            {
                x += layoutRectangle.Width / 2;
            }

            float y = layoutRectangle.Y + f.me.Size;
            if (sf != null && sf.LineAlignment == StringAlignment.Center)
            {
                y -= (int)((layoutRectangle.Height - f.me.Size) / 2);
            }

            me.DrawText(s, x, y, f.me, b.paint);
            //Console.WriteLine("DrawString Native Calls Here " + s);
        }


        public void DrawLine(Pen pn, float x1, float y1, float x2, float y2)
        {
            //Console.WriteLine("DrawLine Native Call Here " + x1 + ", " + y1 + ", " + x2 + ", " + y2);
            me.DrawLine(x1, y1, x2, y2, pn.paint);
        }

        public void DrawImage(Image image, int x, int y, Rectangle srcRect, GraphicsUnit srcUnit)
        {
            //Console.WriteLine("DrawImage Native Call Here " + x + ", " + y);

            SKRect dst= new SKRect( x, y, srcRect.Width, srcRect.Height);
            SKRect src = new SKRect(srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height);

            me.DrawBitmap( image.bm, src, dst);
        }
 

        public void DrawImage(Image image, Rectangle destRect, int srcX, int srcY, int srcWidth, int srcHeight, GraphicsUnit srcUnit)
        {
            DrawImage(image, destRect, srcX, srcY, srcWidth, srcHeight, srcUnit, null);
        }

        public void DrawImage(Image image, Rectangle destRect, int srcX, int srcY, int srcWidth, int srcHeight, GraphicsUnit srcUnit, ImageAttributes imageAttr)
        {
            DrawImage(image, destRect, srcX, srcY, srcWidth, srcHeight, srcUnit, imageAttr, null);
        }

        public void DrawImage(Image image, Rectangle destRect, int srcX, int srcY, int srcWidth, int srcHeight, GraphicsUnit srcUnit, ImageAttributes imageAttr, DrawImageAbort callback)
        {
            DrawImage(image, destRect, srcX, srcY, srcWidth, srcHeight, srcUnit, imageAttr, callback, IntPtr.Zero);
        }


        public void DrawImage(Image image, Rectangle destRect, int srcX, int srcY, int srcWidth, int srcHeight, GraphicsUnit srcUnit, ImageAttributes imageAttrs, DrawImageAbort callback, IntPtr callbackData)
        {
            if (image == null)
            {
                throw new ArgumentNullException("Bitmap");
            }

            //Console.WriteLine("DrawImage Native Call Here " + srcX + ", " + srcY + ", " + srcWidth + ", " + srcHeight);

            SKRect dst = new SKRect(destRect.X, destRect.Y, destRect.Width, destRect.Height);
            SKRect src = new SKRect(srcX, srcY, srcWidth, srcHeight);

            if (image.bm == null)
            {
                Console.WriteLine("DrawImage null image");
            }
            else
            {
                //SKPaint paint = new SKPaint();
                //paint.MaskFilter = new SKMaskFilter()
                //image.im.ToTextureImage()

                me.DrawBitmap(image.bm, src, dst );
            }
        }


        public void DrawImage(Image image, int x, int y)
        {
            if (image == null)
            {
                throw new ArgumentNullException("image");
            }

            //Console.WriteLine("DrawImage Native Call Here " + x + ", " + y );
            this.me.DrawBitmap(image.bm, x, y);
        }

        public void DrawImageUnscaledAndClipped(Image image, Rectangle rect)
        {
            if (image == null)
            {
                throw new ArgumentNullException("image");
            }

            int srcWidth = Math.Min(rect.Width, image.Width);
            int srcHeight = Math.Min(rect.Height, image.Height);

            SKPaint paint = new SKPaint();
            // high quality with antialiasing
            paint.IsAntialias = true;
            paint.FilterQuality = SKFilterQuality.High;

            SKRect skrect = new SKRect(rect.X, rect.Y, rect.Width, rect.Height);

            me.DrawBitmap(image.bm, skrect, skrect, paint);
            //DrawImage(image, rect, 0, 0, srcWidth, srcHeight, GraphicsUnit.Pixel);
        }

        public void DrawImageUnscaled(Image image, int x, int y)
        {
            DrawImage(image, x, y);
        }


        public void FillEllipse(Brush brush, float x, float y, float width, float height)
        {
            if (brush == null)
            {
                throw new ArgumentNullException("brush");
            }

            this.me.DrawOval(x + width / 2, y + height / 2, width/2, height/2, brush.paint);
            //Console.WriteLine("FillEllipse Native Call Here " + x + ", " + y);
        }


        public void FillRectangle(Brush brush, int x, int y, int width, int height)
        {
            if (brush == null)
            {
                throw new ArgumentNullException("brush");
            }

            this.me.DrawRect(x, y, width, height, brush.paint);
            //Console.WriteLine("FillRectangle Native Call Here " + x + ", " + y);
        }
    }
}
