using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanaGraph
{
    public class Color
    {
        public static readonly Color Empty = new Color((uint) 0);
        //private static short StateKnownColorValid = 1;
        //private static short StateARGBValueValid = 2;
        //private static short StateValueMask = StateARGBValueValid;
        //private static short StateNameValid = 8;
        //private static long NotDefinedValue = 0L;
        private const int ARGBAlphaShift = 24;
        private const int ARGBRedShift = 16;
        private const int ARGBGreenShift = 8;
        private const int ARGBBlueShift = 0;

        private readonly string name;
        private readonly long value;
        private readonly short knownColor;
        private readonly short state;

        //
        // Summary:
        //     Gets a system-defined color.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color Transparent => new Color((uint) 0x00000000);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFF0F8FF.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color AliceBlue => new Color(0xFFF0F8FF);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFFAEBD7.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color AntiqueWhite => new Color(0xFFFAEBD7);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FF00FFFF.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color Aqua => new Color(0xFF00FFFF);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FF7FFFD4.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color Aquamarine => new Color(0xFF7FFFD4);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFF0FFFF.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color Azure => new Color(0xFFF0FFFF);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFF5F5DC.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color Beige => new Color(0xFFF5F5DC);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFFFE4C4.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color Bisque => new Color(0xFFFFE4C4);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FF000000.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color Black => new Color(0xFF000000);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFFFEBCD.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color BlanchedAlmond => new Color(0xFFFFEBCD);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FF0000FF.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color Blue => new Color(0xFF0000FF);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FF8A2BE2.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color BlueViolet => new Color(0xFF8A2BE2);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFA52A2A.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color Brown => new Color(0xFFA52A2A);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFDEB887.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color BurlyWood => new Color(0xFFDEB887);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FF5F9EA0.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color CadetBlue => new Color(0xFF5F9EA0);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FF7FFF00.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color Chartreuse => new Color(0xFF7FFF00);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFD2691E.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color Chocolate => new Color(0xFFD2691E);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFFF7F50.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color Coral => new Color(0xFFFF7F50);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FF6495ED.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color CornflowerBlue => new Color(0xFF6495ED);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFFFF8DC.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color Cornsilk => new Color(0xFFFFF8DC);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFDC143C.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color Crimson => new Color(0xFFDC143C);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FF00FFFF.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color Cyan => new Color(0xFF00FFFF);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FF00008B.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color DarkBlue => new Color(0xFF00008B);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FF008B8B.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color DarkCyan => new Color(0xFF008B8B);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFB8860B.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color DarkGoldenrod => new Color(0xFFB8860B);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFA9A9A9.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color DarkGray => new Color(0xFFA9A9A9);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FF006400.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color DarkGreen => new Color(0xFF006400);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFBDB76B.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color DarkKhaki => new Color(0xFFBDB76B);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FF8B008B.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color DarkMagenta => new Color(0xFF8B008B);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FF556B2F.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color DarkOliveGreen => new Color(0xFF556B2F);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFFF8C00.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color DarkOrange => new Color(0xFFFF8C00);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FF9932CC.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color DarkOrchid => new Color(0xFF9932CC);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FF8B0000.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color DarkRed => new Color(0xFF8B0000);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFE9967A.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color DarkSalmon => new Color(0xFFE9967A);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FF8FBC8F.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color DarkSeaGreen => new Color(0xFF8FBC8F);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FF483D8B.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color DarkSlateBlue => new Color(0xFF483D8B);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FF2F4F4F.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color DarkSlateGray => new Color(0xFF2F4F4F);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FF00CED1.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color DarkTurquoise => new Color(0xFF00CED1);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FF9400D3.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color DarkViolet => new Color(0xFF9400D3);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFFF1493.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color DeepPink => new Color(0xFFFF1493);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FF00BFFF.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color DeepSkyBlue => new Color(0xFF00BFFF);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FF696969.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color DimGray => new Color(0xFF696969);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FF1E90FF.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color DodgerBlue => new Color(0xFF1E90FF);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFB22222.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color Firebrick => new Color(0xFFB22222);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFFFFAF0.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color FloralWhite => new Color(0xFFFFFAF0);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FF228B22.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color ForestGreen => new Color(0xFF228B22);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFFF00FF.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color Fuchsia => new Color(0xFFFF00FF);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFDCDCDC.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color Gainsboro => new Color(0xFFDCDCDC);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFF8F8FF.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color GhostWhite => new Color(0xFFF8F8FF);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFFFD700.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color Gold => new Color(0xFFFFD700);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFDAA520.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color Goldenrod => new Color(0xFFDAA520);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FF808080.
        //
        // Returns:
        //     A System.Drawing.Color strcture representing a system-defined color.
        public static Color Gray => new Color(0xFF808080);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FF008000.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color Green => new Color(0xFF008000);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFADFF2F.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color GreenYellow => new Color(0xFFADFF2F);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFF0FFF0.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color Honeydew => new Color(0xFFF0FFF0);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFFF69B4.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color HotPink => new Color(0xFFFF69B4);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFCD5C5C.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color IndianRed => new Color(0xFFCD5C5C);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FF4B0082.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color Indigo => new Color(0xFF4B0082);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFFFFFF0.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color Ivory => new Color(0xFFFFFFF0);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFF0E68C.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color Khaki => new Color(0xFFF0E68C);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFE6E6FA.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color Lavender => new Color(0xFFE6E6FA);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFFFF0F5.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color LavenderBlush => new Color(0xFFFFF0F5);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FF7CFC00.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color LawnGreen => new Color(0xFF7CFC00);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFFFFACD.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color LemonChiffon => new Color(0xFFFFFACD);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFADD8E6.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color LightBlue => new Color(0xFFADD8E6);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFF08080.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color LightCoral => new Color(0xFFF08080);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFE0FFFF.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color LightCyan => new Color(0xFFE0FFFF);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFFAFAD2.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color LightGoldenrodYellow => new Color(0xFFFAFAD2);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FF90EE90.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color LightGreen => new Color(0xFF90EE90);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFD3D3D3.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color LightGray => new Color(0xFFD3D3D3);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFFFB6C1.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color LightPink => new Color(0xFFFFB6C1);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFFFA07A.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color LightSalmon => new Color(0xFFFFA07A);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FF20B2AA.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color LightSeaGreen => new Color(0xFF20B2AA);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FF87CEFA.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color LightSkyBlue => new Color(0xFF87CEFA);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FF778899.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color LightSlateGray => new Color(0xFF778899);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFB0C4DE.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color LightSteelBlue => new Color(0xFFB0C4DE);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFFFFFE0.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color LightYellow => new Color(0xFFFFFFE0);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FF00FF00.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color Lime => new Color(0xFF00FF00);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FF32CD32.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color LimeGreen => new Color(0xFF32CD32);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFFAF0E6.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color Linen => new Color(0xFFFAF0E6);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFFF00FF.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color Magenta => new Color(0xFFFF00FF);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FF800000.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color Maroon => new Color(0xFF800000);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FF66CDAA.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color MediumAquamarine => new Color(0xFF66CDAA);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FF0000CD.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color MediumBlue => new Color(0xFF0000CD);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFBA55D3.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color MediumOrchid => new Color(0xFFBA55D3);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FF9370DB.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color MediumPurple => new Color(0xFF9370DB);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FF3CB371.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color MediumSeaGreen => new Color(0xFF3CB371);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FF7B68EE.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color MediumSlateBlue => new Color(0xFF7B68EE);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FF00FA9A.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color MediumSpringGreen => new Color(0xFF00FA9A);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FF48D1CC.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color MediumTurquoise => new Color(0xFF48D1CC);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFC71585.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color MediumVioletRed => new Color(0xFFC71585);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FF191970.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color MidnightBlue => new Color(0xFF191970);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFF5FFFA.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color MintCream => new Color(0xFFF5FFFA);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFFFE4E1.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color MistyRose => new Color(0xFFFFE4E1);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFFFE4B5.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color Moccasin => new Color(0xFFFFE4B5);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFFFDEAD.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color NavajoWhite => new Color(0xFFFFDEAD);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FF000080.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color Navy => new Color(0xFF000080);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFFDF5E6.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color OldLace => new Color(0xFFFDF5E6);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FF808000.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color Olive => new Color(0xFF808000);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FF6B8E23.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color OliveDrab => new Color(0xFF6B8E23);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFFFA500.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color Orange => new Color(0xFFFFA500);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFFF4500.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color OrangeRed => new Color(0xFFFF4500);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFDA70D6.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color Orchid => new Color(0xFFDA70D6);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFEEE8AA.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color PaleGoldenrod => new Color(0xFFEEE8AA);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FF98FB98.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color PaleGreen => new Color(0xFF98FB98);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFAFEEEE.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color PaleTurquoise => new Color(0xFFAFEEEE);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFDB7093.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color PaleVioletRed => new Color(0xFFDB7093);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFFFEFD5.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color PapayaWhip => new Color(0xFFFFEFD5);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFFFDAB9.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color PeachPuff => new Color(0xFFFFDAB9);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFCD853F.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color Peru => new Color(0xFFCD853F);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFFFC0CB.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color Pink => new Color(0xFFFFC0CB);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFDDA0DD.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color Plum => new Color(0xFFDDA0DD);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFB0E0E6.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color PowderBlue => new Color(0xFFB0E0E6);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FF800080.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color Purple => new Color(0xFF800080);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFFF0000.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color Red => new Color(0xFFFF0000);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFBC8F8F.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color RosyBrown => new Color(0xFFBC8F8F);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FF4169E1.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color RoyalBlue => new Color(0xFF4169E1);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FF8B4513.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color SaddleBrown => new Color(0xFF8B4513);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFFA8072.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color Salmon => new Color(0xFFFA8072);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFF4A460.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color SandyBrown => new Color(0xFFF4A460);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FF2E8B57.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color SeaGreen => new Color(0xFF2E8B57);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFFFF5EE.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color SeaShell => new Color(0xFFFFF5EE);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFA0522D.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color Sienna => new Color(0xFFA0522D);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFC0C0C0.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color Silver => new Color(0xFFC0C0C0);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FF87CEEB.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color SkyBlue => new Color(0xFF87CEEB);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FF6A5ACD.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color SlateBlue => new Color(0xFF6A5ACD);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FF708090.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color SlateGray => new Color(0xFF708090);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFFFFAFA.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color Snow => new Color(0xFFFFFAFA);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FF00FF7F.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color SpringGreen => new Color(0xFF00FF7F);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FF4682B4.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color SteelBlue => new Color(0xFF4682B4);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFD2B48C.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color Tan => new Color(0xFFD2B48C);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FF008080.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color Teal => new Color(0xFF008080);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFD8BFD8.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color Thistle => new Color(0xFFD8BFD8);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFFF6347.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color Tomato => new Color(0xFFFF6347);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FF40E0D0.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color Turquoise => new Color(0xFF40E0D0);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFEE82EE.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color Violet => new Color(0xFFEE82EE);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFF5DEB3.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color Wheat => new Color(0xFFF5DEB3);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFFFFFFF.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color White => new Color(0xFFFFFFFF);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFF5F5F5.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color WhiteSmoke => new Color(0xFFF5F5F5);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFFFFF00.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color Yellow => new Color(0xFFFFFF00);

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FF9ACD32.
        //
        // Returns:
        //     A System.Drawing.Color representing a system-defined color.
        public static Color YellowGreen => new Color(0xFF9ACD32);




        //
        // Summary:
        //     Gets the red component value of this System.Drawing.Color structure.
        //
        // Returns:
        //     The red component value of this System.Drawing.Color.
        public byte R => (byte)((Value >> 16) & 0xFF);

        //
        // Summary:
        //     Gets the green component value of this System.Drawing.Color structure.
        //
        // Returns:
        //     The green component value of this System.Drawing.Color.
        public byte G => (byte)((Value >> 8) & 0xFF);

        //
        // Summary:
        //     Gets the blue component value of this System.Drawing.Color structure.
        //
        // Returns:
        //     The blue component value of this System.Drawing.Color.
        public byte B => (byte)(Value & 0xFF);

        //
        // Summary:
        //     Gets the alpha component value of this System.Drawing.Color structure.
        //
        // Returns:
        //     The alpha component value of this System.Drawing.Color.
        public byte A => (byte)((Value >> 24) & 0xFF);



        private long Value
        {
            get
            {
                return value;
            }
        }


        internal Color(uint rgbk)
        {
            value = rgbk;
            //state = StateKnownColorValid;
        }



        //internal Color(KnownColor knownColor)
        //{
        //    value = 0L;
        //    //state = StateKnownColorValid;
        //    name = null;
        //    this.knownColor = (short)knownColor;
        //}

        private Color(long value, short state, string name, KnownColor knownColor)
        {
            this.value = value;
            this.state = state;
            this.name = name;
            this.knownColor = (short)knownColor;
        }
        private static void CheckByte(int value, string name)
        {
            if (value < 0 || value > 255)
            {
                throw new Exception("Bad Byte value " + value + " " + name);
            }
        }

        private static long MakeArgb(byte alpha, byte red, byte green, byte blue)
        {
            return (long)(uint)((red << 16) | (green << 8) | blue | (alpha << 24)) & 0xFFFFFFFFL;
        }

        public static Color FromArgb(int alpha, int red, int green, int blue)
        {
            CheckByte(alpha, "alpha");
            CheckByte(red, "red");
            CheckByte(green, "green");
            CheckByte(blue, "blue");
            return new Color((uint) MakeArgb((byte)alpha, (byte)red, (byte)green, (byte)blue));
            //return new Color(MakeArgb((byte)alpha, (byte)red, (byte)green, (byte)blue), StateARGBValueValid, null, (KnownColor)0);
        }

        public static Color FromArgb(int red, int green, int blue)
        {
            return FromArgb(255, red, green, blue);
        }

    }
}
