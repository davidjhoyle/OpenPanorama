using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanaGraph
{
    public enum PixelFormatEnum
    {
        Default = 0,
        Extended = 0,
        Indexed1 = 1,
        Indexed2 = 2,
        Indexed4 = 3,
        Indexed8 = 4,
        BlackWhite = 5,
        Gray2 = 6,
        Gray4 = 7,
        Gray8 = 8,
        Bgr555 = 9,
        Bgr565 = 10,
        Gray16 = 11,
        Bgr24 = 12,
        Rgb24 = 13,
        Bgr32 = 14,
        Bgra32 = 0xF,
        Pbgra32 = 0x10,
        Gray32Float = 17,
        Bgr101010 = 20,
        Rgb48 = 21,
        Rgba64 = 22,
        Prgba64 = 23,
        Rgba128Float = 25,
        Prgba128Float = 26,
        Rgb128Float = 27,
        Cmyk32 = 28
    }


    public enum PixelFormat
    {
        //
        // Summary:
        //     The pixel data contains color-indexed values, which means the values are an index
        //     to colors in the system color table, as opposed to individual color values.
        Indexed = 0x10000,
        //
        // Summary:
        //     The pixel data contains GDI colors.
        Gdi = 0x20000,
        //
        // Summary:
        //     The pixel data contains alpha values that are not premultiplied.
        Alpha = 0x40000,
        //
        // Summary:
        //     The pixel format contains premultiplied alpha values.
        PAlpha = 0x80000,
        //
        // Summary:
        //     Reserved.
        Extended = 0x100000,
        //
        // Summary:
        //     The default pixel format of 32 bits per pixel. The format specifies 24-bit color
        //     depth and an 8-bit alpha channel.
        Canonical = 0x200000,
        //
        // Summary:
        //     The pixel format is undefined.
        Undefined = 0,
        //
        // Summary:
        //     No pixel format is specified.
        DontCare = 0,
        //
        // Summary:
        //     Specifies that the pixel format is 1 bit per pixel and that it uses indexed color.
        //     The color table therefore has two colors in it.
        Format1bppIndexed = 196865,
        //
        // Summary:
        //     Specifies that the format is 4 bits per pixel, indexed.
        Format4bppIndexed = 197634,
        //
        // Summary:
        //     Specifies that the format is 8 bits per pixel, indexed. The color table therefore
        //     has 256 colors in it.
        Format8bppIndexed = 198659,
        //
        // Summary:
        //     The pixel format is 16 bits per pixel. The color information specifies 65536
        //     shades of gray.
        Format16bppGrayScale = 1052676,
        //
        // Summary:
        //     Specifies that the format is 16 bits per pixel; 5 bits each are used for the
        //     red, green, and blue components. The remaining bit is not used.
        Format16bppRgb555 = 135173,
        //
        // Summary:
        //     Specifies that the format is 16 bits per pixel; 5 bits are used for the red component,
        //     6 bits are used for the green component, and 5 bits are used for the blue component.
        Format16bppRgb565 = 135174,
        //
        // Summary:
        //     The pixel format is 16 bits per pixel. The color information specifies 32,768
        //     shades of color, of which 5 bits are red, 5 bits are green, 5 bits are blue,
        //     and 1 bit is alpha.
        Format16bppArgb1555 = 397319,
        //
        // Summary:
        //     Specifies that the format is 24 bits per pixel; 8 bits each are used for the
        //     red, green, and blue components.
        Format24bppRgb = 137224,
        //
        // Summary:
        //     Specifies that the format is 32 bits per pixel; 8 bits each are used for the
        //     red, green, and blue components. The remaining 8 bits are not used.
        Format32bppRgb = 139273,
        //
        // Summary:
        //     Specifies that the format is 32 bits per pixel; 8 bits each are used for the
        //     alpha, red, green, and blue components.
        Format32bppArgb = 2498570,
        //
        // Summary:
        //     Specifies that the format is 32 bits per pixel; 8 bits each are used for the
        //     alpha, red, green, and blue components. The red, green, and blue components are
        //     premultiplied, according to the alpha component.
        Format32bppPArgb = 925707,
        //
        // Summary:
        //     Specifies that the format is 48 bits per pixel; 16 bits each are used for the
        //     red, green, and blue components.
        Format48bppRgb = 1060876,
        //
        // Summary:
        //     Specifies that the format is 64 bits per pixel; 16 bits each are used for the
        //     alpha, red, green, and blue components.
        Format64bppArgb = 3424269,
        //
        // Summary:
        //     Specifies that the format is 64 bits per pixel; 16 bits each are used for the
        //     alpha, red, green, and blue components. The red, green, and blue components are
        //     premultiplied according to the alpha component.
        Format64bppPArgb = 1851406,
        //
        // Summary:
        //     The maximum value for this enumeration.
        Max = 0xF
    }

}

namespace PanaGraph.Media
{

    public class PixelFormat
    {
        private uint _bitsPerPixel;

        //[NonSerialized]
        //private static readonly Guid WICPixelFormatPhotonFirst = new Guid(1876804388, 19971, 19454, 177, 133, 61, 119, 118, 141, 201, 29);

        //[NonSerialized]
        //private static readonly Guid WICPixelFormatPhotonLast = new Guid(1876804388, 19971, 19454, 177, 133, 61, 119, 118, 141, 201, 66);

        //private PixelFormatFlags FormatFlags => _flags;
        public int BitsPerPixel
        {
            get { return (int)_bitsPerPixel; }
        }

        //Format32bppArgb

        public PixelFormat(PixelFormatEnum pfe)
        {
        }
    }

    [Flags]
    internal enum PixelFormatFlags
    {
        BitsPerPixelMask = 0xFF,
        BitsPerPixelUndefined = 0x0,
        BitsPerPixel1 = 0x1,
        BitsPerPixel2 = 0x2,
        BitsPerPixel4 = 0x4,
        BitsPerPixel8 = 0x8,
        BitsPerPixel16 = 0x10,
        BitsPerPixel24 = 0x18,
        BitsPerPixel32 = 0x20,
        BitsPerPixel48 = 0x30,
        BitsPerPixel64 = 0x40,
        BitsPerPixel96 = 0x60,
        BitsPerPixel128 = 0x80,
        IsGray = 0x100,
        IsCMYK = 0x200,
        IsSRGB = 0x400,
        IsScRGB = 0x800,
        Premultiplied = 0x1000,
        ChannelOrderMask = 0x1E000,
        ChannelOrderRGB = 0x2000,
        ChannelOrderBGR = 0x4000,
        ChannelOrderARGB = 0x8000,
        ChannelOrderABGR = 0x10000,
        Palettized = 0x20000,
        NChannelAlpha = 0x40000,
        IsNChannel = 0x80000
    }

}
