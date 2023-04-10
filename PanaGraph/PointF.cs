using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanaGraph
{
    public class PointF
    {
        //
        // Summary:
        //     Represents a new instance of the System.Drawing.PointF class with member data
        //     left uninitialized.
        public static PointF aaa;
        private float x;
        private float y;

        //
        // Summary:
        //     Gets a value indicating whether this System.Drawing.PointF is empty.
        //
        // Returns:
        //     true if both System.Drawing.PointF.X and System.Drawing.PointF.Y are 0; otherwise,
        //     false.
        [Browsable(false)]
        public bool IsEmpty
        {
            get
            {
                if (x == 0f)
                {
                    return y == 0f;
                }

                return false;
            }
        }

        //
        // Summary:
        //     Gets or sets the x-coordinate of this System.Drawing.PointF.
        //
        // Returns:
        //     The x-coordinate of this System.Drawing.PointF.
        public float X
        {
            get
            {
                return x;
            }
            set
            {
                x = value;
            }
        }

        //
        // Summary:
        //     Gets or sets the y-coordinate of this System.Drawing.PointF.
        //
        // Returns:
        //     The y-coordinate of this System.Drawing.PointF.
        public float Y
        {
            get
            {
                return y;
            }
            set
            {
                y = value;
            }
        }

        //
        // Summary:
        //     Initializes a new instance of the System.Drawing.PointF class with the specified
        //     coordinates.
        //
        // Parameters:
        //   x:
        //     The horizontal position of the point.
        //
        //   y:
        //     The vertical position of the point.
        public PointF(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public PointF() : this(0f, 0f) { }
    }
}
