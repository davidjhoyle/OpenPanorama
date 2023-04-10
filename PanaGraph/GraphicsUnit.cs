using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanaGraph
{
    public enum GraphicsUnit
    {
        //
        // Summary:
        //     Specifies the world coordinate system unit as the unit of measure.
        World,
        //
        // Summary:
        //     Specifies the unit of measure of the display device. Typically pixels for video
        //     displays, and 1/100 inch for printers.
        Display,
        //
        // Summary:
        //     Specifies a device pixel as the unit of measure.
        Pixel,
        //
        // Summary:
        //     Specifies a printer's point (1/72 inch) as the unit of measure.
        Point,
        //
        // Summary:
        //     Specifies the inch as the unit of measure.
        Inch,
        //
        // Summary:
        //     Specifies the document unit (1/300 inch) as the unit of measure.
        Document,
        //
        // Summary:
        //     Specifies the millimeter as the unit of measure.
        Millimeter
    }
}
