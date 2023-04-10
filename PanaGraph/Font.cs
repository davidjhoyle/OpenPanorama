using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using SkiaSharp;

namespace PanaGraph
{
    public class Font
    {
        public SKFont me;

        public Font(FontFamily family, float emSize, FontStyle style)
        {
            SKTypefaceStyle fontstyle = SKTypefaceStyle.Normal;

            if (style.HasFlag(FontStyle.Bold))
            {
                fontstyle |= SKTypefaceStyle.Bold;
            }

            if (style.HasFlag(FontStyle.Italic))
            {
                fontstyle |= SKTypefaceStyle.Italic;
            }

            SKTypeface fonttype = SKTypeface.FromFamilyName("SansSerif", fontstyle);
            me = new SKFont(fonttype, (float)( emSize ));
        }
    }
}
