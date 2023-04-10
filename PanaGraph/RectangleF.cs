using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanaGraph
{
    [Serializable]
    public struct RectangleF
    {
        //
        // Summary:
        //     Represents an instance of the System.Drawing.RectangleF class with its members
        //     uninitialized.
        public static readonly RectangleF Empty;

        private float x;

        private float y;

        private float width;

        private float height;

        //
        // Summary:
        //     Gets or sets the coordinates of the upper-left corner of this System.Drawing.RectangleF
        //     structure.
        //
        // Returns:
        //     A System.Drawing.PointF that represents the upper-left corner of this System.Drawing.RectangleF
        //     structure.
        [Browsable(false)]
        public PointF Location
        {
            get
            {
                return new PointF(X, Y);
            }
            set
            {
                X = value.X;
                Y = value.Y;
            }
        }

        //
        // Summary:
        //     Gets or sets the size of this System.Drawing.RectangleF.
        //
        // Returns:
        //     A System.Drawing.SizeF that represents the width and height of this System.Drawing.RectangleF
        //     structure.
        [Browsable(false)]
        public SizeF Size
        {
            get
            {
                return new SizeF(Width, Height);
            }
            set
            {
                Width = value.Width;
                Height = value.Height;
            }
        }

        //
        // Summary:
        //     Gets or sets the x-coordinate of the upper-left corner of this System.Drawing.RectangleF
        //     structure.
        //
        // Returns:
        //     The x-coordinate of the upper-left corner of this System.Drawing.RectangleF structure.
        //     The default is 0.
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
        //     Gets or sets the y-coordinate of the upper-left corner of this System.Drawing.RectangleF
        //     structure.
        //
        // Returns:
        //     The y-coordinate of the upper-left corner of this System.Drawing.RectangleF structure.
        //     The default is 0.
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
        //     Gets or sets the width of this System.Drawing.RectangleF structure.
        //
        // Returns:
        //     The width of this System.Drawing.RectangleF structure. The default is 0.
        public float Width
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
        //     Gets or sets the height of this System.Drawing.RectangleF structure.
        //
        // Returns:
        //     The height of this System.Drawing.RectangleF structure. The default is 0.
        public float Height
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
        //     Gets the x-coordinate of the left edge of this System.Drawing.RectangleF structure.
        //
        // Returns:
        //     The x-coordinate of the left edge of this System.Drawing.RectangleF structure.
        [Browsable(false)]
        public float Left => X;

        //
        // Summary:
        //     Gets the y-coordinate of the top edge of this System.Drawing.RectangleF structure.
        //
        // Returns:
        //     The y-coordinate of the top edge of this System.Drawing.RectangleF structure.
        [Browsable(false)]
        public float Top => Y;

        //
        // Summary:
        //     Gets the x-coordinate that is the sum of System.Drawing.RectangleF.X and System.Drawing.RectangleF.Width
        //     of this System.Drawing.RectangleF structure.
        //
        // Returns:
        //     The x-coordinate that is the sum of System.Drawing.RectangleF.X and System.Drawing.RectangleF.Width
        //     of this System.Drawing.RectangleF structure.
        [Browsable(false)]
        public float Right => X + Width;

        //
        // Summary:
        //     Gets the y-coordinate that is the sum of System.Drawing.RectangleF.Y and System.Drawing.RectangleF.Height
        //     of this System.Drawing.RectangleF structure.
        //
        // Returns:
        //     The y-coordinate that is the sum of System.Drawing.RectangleF.Y and System.Drawing.RectangleF.Height
        //     of this System.Drawing.RectangleF structure.
        [Browsable(false)]
        public float Bottom => Y + Height;

        //
        // Summary:
        //     Gets a value that indicates whether the System.Drawing.RectangleF.Width or System.Drawing.RectangleF.Height
        //     property of this System.Drawing.RectangleF has a value of zero.
        //
        // Returns:
        //     true if the System.Drawing.RectangleF.Width or System.Drawing.RectangleF.Height
        //     property of this System.Drawing.RectangleF has a value of zero; otherwise, false.
        [Browsable(false)]
        public bool IsEmpty
        {
            get
            {
                if (!(Width <= 0f))
                {
                    return Height <= 0f;
                }

                return true;
            }
        }

        //
        // Summary:
        //     Initializes a new instance of the System.Drawing.RectangleF class with the specified
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
        public RectangleF(float x, float y, float width, float height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }

        //
        // Summary:
        //     Initializes a new instance of the System.Drawing.RectangleF class with the specified
        //     location and size.
        //
        // Parameters:
        //   location:
        //     A System.Drawing.PointF that represents the upper-left corner of the rectangular
        //     region.
        //
        //   size:
        //     A System.Drawing.SizeF that represents the width and height of the rectangular
        //     region.
        public RectangleF(PointF location, SizeF size)
        {
            x = location.X;
            y = location.Y;
            width = size.Width;
            height = size.Height;
        }

        //
        // Summary:
        //     Creates a System.Drawing.RectangleF structure with upper-left corner and lower-right
        //     corner at the specified locations.
        //
        // Parameters:
        //   left:
        //     The x-coordinate of the upper-left corner of the rectangular region.
        //
        //   top:
        //     The y-coordinate of the upper-left corner of the rectangular region.
        //
        //   right:
        //     The x-coordinate of the lower-right corner of the rectangular region.
        //
        //   bottom:
        //     The y-coordinate of the lower-right corner of the rectangular region.
        //
        // Returns:
        //     The new System.Drawing.RectangleF that this method creates.
        public static RectangleF FromLTRB(float left, float top, float right, float bottom)
        {
            return new RectangleF(left, top, right - left, bottom - top);
        }

        //
        // Summary:
        //     Tests whether obj is a System.Drawing.RectangleF with the same location and size
        //     of this System.Drawing.RectangleF.
        //
        // Parameters:
        //   obj:
        //     The System.Object to test.
        //
        // Returns:
        //     true if obj is a System.Drawing.RectangleF and its X, Y, Width, and Height properties
        //     are equal to the corresponding properties of this System.Drawing.RectangleF;
        //     otherwise, false.
        public override bool Equals(object obj)
        {
            if (!(obj is RectangleF))
            {
                return false;
            }

            RectangleF rectangleF = (RectangleF)obj;
            if (rectangleF.X == X && rectangleF.Y == Y && rectangleF.Width == Width)
            {
                return rectangleF.Height == Height;
            }

            return false;
        }

        //
        // Summary:
        //     Tests whether two System.Drawing.RectangleF structures have equal location and
        //     size.
        //
        // Parameters:
        //   left:
        //     The System.Drawing.RectangleF structure that is to the left of the equality operator.
        //
        //   right:
        //     The System.Drawing.RectangleF structure that is to the right of the equality
        //     operator.
        //
        // Returns:
        //     true if the two specified System.Drawing.RectangleF structures have equal System.Drawing.RectangleF.X,
        //     System.Drawing.RectangleF.Y, System.Drawing.RectangleF.Width, and System.Drawing.RectangleF.Height
        //     properties; otherwise, false.
        public static bool operator ==(RectangleF left, RectangleF right)
        {
            if (left.X == right.X && left.Y == right.Y && left.Width == right.Width)
            {
                return left.Height == right.Height;
            }

            return false;
        }

        //
        // Summary:
        //     Tests whether two System.Drawing.RectangleF structures differ in location or
        //     size.
        //
        // Parameters:
        //   left:
        //     The System.Drawing.RectangleF structure that is to the left of the inequality
        //     operator.
        //
        //   right:
        //     The System.Drawing.RectangleF structure that is to the right of the inequality
        //     operator.
        //
        // Returns:
        //     true if any of the System.Drawing.RectangleF.X , System.Drawing.RectangleF.Y,
        //     System.Drawing.RectangleF.Width, or System.Drawing.RectangleF.Height properties
        //     of the two System.Drawing.Rectangle structures are unequal; otherwise, false.
        public static bool operator !=(RectangleF left, RectangleF right)
        {
            return !(left == right);
        }

        //
        // Summary:
        //     Determines if the specified point is contained within this System.Drawing.RectangleF
        //     structure.
        //
        // Parameters:
        //   x:
        //     The x-coordinate of the point to test.
        //
        //   y:
        //     The y-coordinate of the point to test.
        //
        // Returns:
        //     true if the point defined by x and y is contained within this System.Drawing.RectangleF
        //     structure; otherwise, false.
        public bool Contains(float x, float y)
        {
            if (X <= x && x < X + Width && Y <= y)
            {
                return y < Y + Height;
            }

            return false;
        }

        //
        // Summary:
        //     Determines if the specified point is contained within this System.Drawing.RectangleF
        //     structure.
        //
        // Parameters:
        //   pt:
        //     The System.Drawing.PointF to test.
        //
        // Returns:
        //     true if the point represented by the pt parameter is contained within this System.Drawing.RectangleF
        //     structure; otherwise, false.
        public bool Contains(PointF pt)
        {
            return Contains(pt.X, pt.Y);
        }

        //
        // Summary:
        //     Determines if the rectangular region represented by rect is entirely contained
        //     within this System.Drawing.RectangleF structure.
        //
        // Parameters:
        //   rect:
        //     The System.Drawing.RectangleF to test.
        //
        // Returns:
        //     true if the rectangular region represented by rect is entirely contained within
        //     the rectangular region represented by this System.Drawing.RectangleF; otherwise,
        //     false.
        public bool Contains(RectangleF rect)
        {
            if (X <= rect.X && rect.X + rect.Width <= X + Width && Y <= rect.Y)
            {
                return rect.Y + rect.Height <= Y + Height;
            }

            return false;
        }

        //
        // Summary:
        //     Gets the hash code for this System.Drawing.RectangleF structure. For information
        //     about the use of hash codes, see Object.GetHashCode.
        //
        // Returns:
        //     The hash code for this System.Drawing.RectangleF.
        public override int GetHashCode()
        {
            return (int)((uint)X ^ (((uint)Y << 13) | ((uint)Y >> 19)) ^ (((uint)Width << 26) | ((uint)Width >> 6)) ^ (((uint)Height << 7) | ((uint)Height >> 25)));
        }

        //
        // Summary:
        //     Enlarges this System.Drawing.RectangleF structure by the specified amount.
        //
        // Parameters:
        //   x:
        //     The amount to inflate this System.Drawing.RectangleF structure horizontally.
        //
        //   y:
        //     The amount to inflate this System.Drawing.RectangleF structure vertically.
        public void Inflate(float x, float y)
        {
            X -= x;
            Y -= y;
            Width += 2f * x;
            Height += 2f * y;
        }

        //
        // Summary:
        //     Enlarges this System.Drawing.RectangleF by the specified amount.
        //
        // Parameters:
        //   size:
        //     The amount to inflate this rectangle.
        public void Inflate(SizeF size)
        {
            Inflate(size.Width, size.Height);
        }

        //
        // Summary:
        //     Creates and returns an enlarged copy of the specified System.Drawing.RectangleF
        //     structure. The copy is enlarged by the specified amount and the original rectangle
        //     remains unmodified.
        //
        // Parameters:
        //   rect:
        //     The System.Drawing.RectangleF to be copied. This rectangle is not modified.
        //
        //   x:
        //     The amount to enlarge the copy of the rectangle horizontally.
        //
        //   y:
        //     The amount to enlarge the copy of the rectangle vertically.
        //
        // Returns:
        //     The enlarged System.Drawing.RectangleF.
        public static RectangleF Inflate(RectangleF rect, float x, float y)
        {
            RectangleF result = rect;
            result.Inflate(x, y);
            return result;
        }

        //
        // Summary:
        //     Replaces this System.Drawing.RectangleF structure with the intersection of itself
        //     and the specified System.Drawing.RectangleF structure.
        //
        // Parameters:
        //   rect:
        //     The rectangle to intersect.
        public void Intersect(RectangleF rect)
        {
            RectangleF rectangleF = Intersect(rect, this);
            X = rectangleF.X;
            Y = rectangleF.Y;
            Width = rectangleF.Width;
            Height = rectangleF.Height;
        }

        //
        // Summary:
        //     Returns a System.Drawing.RectangleF structure that represents the intersection
        //     of two rectangles. If there is no intersection, and empty System.Drawing.RectangleF
        //     is returned.
        //
        // Parameters:
        //   a:
        //     A rectangle to intersect.
        //
        //   b:
        //     A rectangle to intersect.
        //
        // Returns:
        //     A third System.Drawing.RectangleF structure the size of which represents the
        //     overlapped area of the two specified rectangles.
        public static RectangleF Intersect(RectangleF a, RectangleF b)
        {
            float num = Math.Max(a.X, b.X);
            float num2 = Math.Min(a.X + a.Width, b.X + b.Width);
            float num3 = Math.Max(a.Y, b.Y);
            float num4 = Math.Min(a.Y + a.Height, b.Y + b.Height);
            if (num2 >= num && num4 >= num3)
            {
                return new RectangleF(num, num3, num2 - num, num4 - num3);
            }

            return Empty;
        }

        //
        // Summary:
        //     Determines if this rectangle intersects with rect.
        //
        // Parameters:
        //   rect:
        //     The rectangle to test.
        //
        // Returns:
        //     true if there is any intersection; otherwise, false.
        public bool IntersectsWith(RectangleF rect)
        {
            if (rect.X < X + Width && X < rect.X + rect.Width && rect.Y < Y + Height)
            {
                return Y < rect.Y + rect.Height;
            }

            return false;
        }

        //
        // Summary:
        //     Creates the smallest possible third rectangle that can contain both of two rectangles
        //     that form a union.
        //
        // Parameters:
        //   a:
        //     A rectangle to union.
        //
        //   b:
        //     A rectangle to union.
        //
        // Returns:
        //     A third System.Drawing.RectangleF structure that contains both of the two rectangles
        //     that form the union.
        public static RectangleF Union(RectangleF a, RectangleF b)
        {
            float num = Math.Min(a.X, b.X);
            float num2 = Math.Max(a.X + a.Width, b.X + b.Width);
            float num3 = Math.Min(a.Y, b.Y);
            float num4 = Math.Max(a.Y + a.Height, b.Y + b.Height);
            return new RectangleF(num, num3, num2 - num, num4 - num3);
        }

        //
        // Summary:
        //     Adjusts the location of this rectangle by the specified amount.
        //
        // Parameters:
        //   pos:
        //     The amount to offset the location.
        public void Offset(PointF pos)
        {
            Offset(pos.X, pos.Y);
        }

        //
        // Summary:
        //     Adjusts the location of this rectangle by the specified amount.
        //
        // Parameters:
        //   x:
        //     The amount to offset the location horizontally.
        //
        //   y:
        //     The amount to offset the location vertically.
        public void Offset(float x, float y)
        {
            X += x;
            Y += y;
        }

        //
        // Summary:
        //     Converts the specified System.Drawing.Rectangle structure to a System.Drawing.RectangleF
        //     structure.
        //
        // Parameters:
        //   r:
        //     The System.Drawing.Rectangle structure to convert.
        //
        // Returns:
        //     The System.Drawing.RectangleF structure that is converted from the specified
        //     System.Drawing.Rectangle structure.
        public static implicit operator RectangleF(Rectangle r)
        {
            return new RectangleF(r.X, r.Y, r.Width, r.Height);
        }

        //
        // Summary:
        //     Converts the Location and System.Drawing.Size of this System.Drawing.RectangleF
        //     to a human-readable string.
        //
        // Returns:
        //     A string that contains the position, width, and height of this System.Drawing.RectangleF
        //     structure. For example, "{X=20, Y=20, Width=100, Height=50}".
        public override string ToString()
        {
            return "{X=" + X.ToString(CultureInfo.CurrentCulture) + ",Y=" + Y.ToString(CultureInfo.CurrentCulture) + ",Width=" + Width.ToString(CultureInfo.CurrentCulture) + ",Height=" + Height.ToString(CultureInfo.CurrentCulture) + "}";
        }
    }
}
