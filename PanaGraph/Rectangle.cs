using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanaGraph
{
    public struct Rectangle
    {
        //
        // Summary:
        //     Represents a System.Drawing.Rectangle structure with its properties left uninitialized.
        public static readonly Rectangle Empty;
        private int x;
        private int y;
        private int width;
        private int height;

        //
        // Summary:
        //     Gets or sets the coordinates of the upper-left corner of this System.Drawing.Rectangle
        //     structure.
        //
        // Returns:
        //     A System.Drawing.Point that represents the upper-left corner of this System.Drawing.Rectangle
        //     structure.
        [Browsable(false)]
        public Point Location
        {
            get
            {
                return new Point(X, Y);
            }
            set
            {
                X = value.X;
                Y = value.Y;
            }
        }

        //
        // Summary:
        //     Gets or sets the size of this System.Drawing.Rectangle.
        //
        // Returns:
        //     A System.Drawing.Size that represents the width and height of this System.Drawing.Rectangle
        //     structure.
        [Browsable(false)]
        public Size Size
        {
            get
            {
                return new Size(Width, Height);
            }
            set
            {
                Width = value.Width;
                Height = value.Height;
            }
        }

        //
        // Summary:
        //     Gets or sets the x-coordinate of the upper-left corner of this System.Drawing.Rectangle
        //     structure.
        //
        // Returns:
        //     The x-coordinate of the upper-left corner of this System.Drawing.Rectangle structure.
        //     The default is 0.
        public int X
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
        //     Gets or sets the y-coordinate of the upper-left corner of this System.Drawing.Rectangle
        //     structure.
        //
        // Returns:
        //     The y-coordinate of the upper-left corner of this System.Drawing.Rectangle structure.
        //     The default is 0.
        public int Y
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
        //     Gets or sets the width of this System.Drawing.Rectangle structure.
        //
        // Returns:
        //     The width of this System.Drawing.Rectangle structure. The default is 0.
        public int Width
        {
            get
            {
                return width;
            }
            set
            {
                width = value;
            }
        }

        //
        // Summary:
        //     Gets or sets the height of this System.Drawing.Rectangle structure.
        //
        // Returns:
        //     The height of this System.Drawing.Rectangle structure. The default is 0.
        public int Height
        {
            get
            {
                return height;
            }
            set
            {
                height = value;
            }
        }

        //
        // Summary:
        //     Gets the x-coordinate of the left edge of this System.Drawing.Rectangle structure.
        //
        // Returns:
        //     The x-coordinate of the left edge of this System.Drawing.Rectangle structure.
        [Browsable(false)]
        public int Left => X;

        //
        // Summary:
        //     Gets the y-coordinate of the top edge of this System.Drawing.Rectangle structure.
        //
        // Returns:
        //     The y-coordinate of the top edge of this System.Drawing.Rectangle structure.
        [Browsable(false)]
        public int Top => Y;

        //
        // Summary:
        //     Gets the x-coordinate that is the sum of System.Drawing.Rectangle.X and System.Drawing.Rectangle.Width
        //     property values of this System.Drawing.Rectangle structure.
        //
        // Returns:
        //     The x-coordinate that is the sum of System.Drawing.Rectangle.X and System.Drawing.Rectangle.Width
        //     of this System.Drawing.Rectangle.
        [Browsable(false)]
        public int Right => X + Width;

        //
        // Summary:
        //     Gets the y-coordinate that is the sum of the System.Drawing.Rectangle.Y and System.Drawing.Rectangle.Height
        //     property values of this System.Drawing.Rectangle structure.
        //
        // Returns:
        //     The y-coordinate that is the sum of System.Drawing.Rectangle.Y and System.Drawing.Rectangle.Height
        //     of this System.Drawing.Rectangle.
        [Browsable(false)]
        public int Bottom => Y + Height;

        //
        // Summary:
        //     Tests whether all numeric properties of this System.Drawing.Rectangle have values
        //     of zero.
        //
        // Returns:
        //     This property returns true if the System.Drawing.Rectangle.Width, System.Drawing.Rectangle.Height,
        //     System.Drawing.Rectangle.X, and System.Drawing.Rectangle.Y properties of this
        //     System.Drawing.Rectangle all have values of zero; otherwise, false.
        [Browsable(false)]
        public bool IsEmpty
        {
            get
            {
                if (height == 0 && width == 0 && x == 0)
                {
                    return y == 0;
                }

                return false;
            }
        }

        //
        // Summary:
        //     Initializes a new instance of the System.Drawing.Rectangle class with the specified
        //     location and size.
        //
        // Parameters:
        //   x:
        //     The x-coordinate of the upper-left corner of the rectangle.
        //
        //   y:
        //     The y-coordinate of the upper-left corner of the rectangle.
        //
        //   width:
        //     The width of the rectangle.
        //
        //   height:
        //     The height of the rectangle.
        public Rectangle(int x, int y, int width, int height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }
    }
}
