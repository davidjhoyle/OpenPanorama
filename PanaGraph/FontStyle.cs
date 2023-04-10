using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanaGraph
{
    [Flags]
    public enum FontStyle
    {
        //
        // Summary:
        //     Normal text.
        Regular = 0x0,
        //
        // Summary:
        //     Bold text.
        Bold = 0x1,
        //
        // Summary:
        //     Italic text.
        Italic = 0x2,
        //
        // Summary:
        //     Underlined text.
        Underline = 0x4,
        //
        // Summary:
        //     Text with a line through the middle.
        Strikeout = 0x8
    }
}
