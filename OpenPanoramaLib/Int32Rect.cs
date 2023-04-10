using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

//namespace OpenPanoramaLib
//{
//    //
//    // Summary:
//    //     Describes the width, height, and location of an integer rectangle.
//    [Serializable]
//    //[TypeConverter(typeof(Int32RectConverter))]
//    //[ValueSerializer(typeof(Int32RectValueSerializer))]
//    public struct Int32Rect : IFormattable
//    {
//        internal int _x;

//        internal int _y;

//        internal int _width;

//        internal int _height;

//        private static readonly Int32Rect s_empty = new Int32Rect(0, 0, 0, 0);

//        //
//        // Summary:
//        //     Gets or sets the x-coordinate of the top-left corner of the rectangle.
//        //
//        // Returns:
//        //     The x-coordinate of the top-left corner of the rectangle. The default value is
//        //     0.
//        public int X
//        {
//            get
//            {
//                return _x;
//            }
//            set
//            {
//                _x = value;
//            }
//        }

//        //
//        // Summary:
//        //     Gets or sets the y-coordinate of the top-left corner of the rectangle.
//        //
//        // Returns:
//        //     The y-coordinate of the top-left corner of the rectangle. The default value is
//        //     0.
//        public int Y
//        {
//            get
//            {
//                return _y;
//            }
//            set
//            {
//                _y = value;
//            }
//        }

//        //
//        // Summary:
//        //     Gets or sets the width of the rectangle.
//        //
//        // Returns:
//        //     The width of the rectangle. The default value is 0.
//        public int Width
//        {
//            get
//            {
//                return _width;
//            }
//            set
//            {
//                _width = value;
//            }
//        }

//        //
//        // Summary:
//        //     Gets or sets the height of the rectangle.
//        //
//        // Returns:
//        //     The height of the rectangle. The default value is 0.
//        public int Height
//        {
//            get
//            {
//                return _height;
//            }
//            set
//            {
//                _height = value;
//            }
//        }

//        //
//        // Summary:
//        //     Gets the empty rectangle, a special value that represents a rectangle with no
//        //     position or area.
//        //
//        // Returns:
//        //     An empty rectangle with no position or area.
//        public static Int32Rect Empty => s_empty;

//        //
//        // Summary:
//        //     Gets a value indicating whether the rectangle is empty.
//        //
//        // Returns:
//        //     true if the rectangle is empty; otherwise, false. The default value is true.
//        public bool IsEmpty
//        {
//            get
//            {
//                if (_x == 0 && _y == 0 && _width == 0)
//                {
//                    return _height == 0;
//                }

//                return false;
//            }
//        }

//        //
//        // Summary:
//        //     Gets a value that indicates whether the System.Windows.Int32Rect.Width and System.Windows.Int32Rect.Height
//        //     properties of the System.Windows.Int32Rect are greater than 0.
//        //
//        // Returns:
//        //     true if the System.Windows.Int32Rect.Width and System.Windows.Int32Rect.Height
//        //     properties of the System.Windows.Int32Rect are greater than 0; otherwise, false.
//        public bool HasArea
//        {
//            get
//            {
//                if (_width > 0)
//                {
//                    return _height > 0;
//                }

//                return false;
//            }
//        }

//        //
//        // Summary:
//        //     Compares two rectangles for exact equality.
//        //
//        // Parameters:
//        //   int32Rect1:
//        //     The first rectangle to compare.
//        //
//        //   int32Rect2:
//        //     The second rectangle to compare.
//        //
//        // Returns:
//        //     true if int32Rect1 and int32Rect2 have the same System.Windows.Int32Rect.X, System.Windows.Int32Rect.Y,
//        //     System.Windows.Int32Rect.Width, and System.Windows.Int32Rect.Height; otherwise,
//        //     false.
//        public static bool operator ==(Int32Rect int32Rect1, Int32Rect int32Rect2)
//        {
//            if (int32Rect1.X == int32Rect2.X && int32Rect1.Y == int32Rect2.Y && int32Rect1.Width == int32Rect2.Width)
//            {
//                return int32Rect1.Height == int32Rect2.Height;
//            }

//            return false;
//        }

//        //
//        // Summary:
//        //     Compares two rectangles for inequality.
//        //
//        // Parameters:
//        //   int32Rect1:
//        //     The first rectangle to compare.
//        //
//        //   int32Rect2:
//        //     The second rectangle to compare.
//        //
//        // Returns:
//        //     false if int32Rect1 and int32Rect2 have the same System.Windows.Int32Rect.X,
//        //     System.Windows.Int32Rect.Y, System.Windows.Int32Rect.Width, and System.Windows.Int32Rect.Height;
//        //     otherwise, if all of these values are the same, then true.
//        public static bool operator !=(Int32Rect int32Rect1, Int32Rect int32Rect2)
//        {
//            return !(int32Rect1 == int32Rect2);
//        }

//        //
//        // Summary:
//        //     Determines whether the specified rectangles are equal.
//        //
//        // Parameters:
//        //   int32Rect1:
//        //     The first rectangle to compare.
//        //
//        //   int32Rect2:
//        //     The second rectangle to compare.
//        //
//        // Returns:
//        //     true if int32Rect1 and int32Rect2 have the same System.Windows.Int32Rect.X, System.Windows.Int32Rect.Y,
//        //     System.Windows.Int32Rect.Width, and System.Windows.Int32Rect.Height; otherwise,
//        //     false.
//        public static bool Equals(Int32Rect int32Rect1, Int32Rect int32Rect2)
//        {
//            if (int32Rect1.IsEmpty)
//            {
//                return int32Rect2.IsEmpty;
//            }

//            if (int32Rect1.X.Equals(int32Rect2.X) && int32Rect1.Y.Equals(int32Rect2.Y) && int32Rect1.Width.Equals(int32Rect2.Width))
//            {
//                return int32Rect1.Height.Equals(int32Rect2.Height);
//            }

//            return false;
//        }

//        //
//        // Summary:
//        //     Determines whether the specified rectangle is equal to this rectangle.
//        //
//        // Parameters:
//        //   o:
//        //     The object to compare to the current rectangle.
//        //
//        // Returns:
//        //     true if o is an System.Windows.Int32Rect and the same System.Windows.Int32Rect.X,
//        //     System.Windows.Int32Rect.Y, System.Windows.Int32Rect.Width, and System.Windows.Int32Rect.Height
//        //     as this rectangle; otherwise, false.
//        public override bool Equals(object o)
//        {
//            if (o == null || !(o is Int32Rect))
//            {
//                return false;
//            }

//            Int32Rect int32Rect = (Int32Rect)o;
//            return Equals(this, int32Rect);
//        }

//        //
//        // Summary:
//        //     Determines whether the specified rectangle is equal to this rectangle.
//        //
//        // Parameters:
//        //   value:
//        //     The rectangle to compare to the current rectangle.
//        //
//        // Returns:
//        //     true if both rectangles have the same System.Windows.Int32Rect.X, System.Windows.Int32Rect.Y,
//        //     System.Windows.Int32Rect.Width, and System.Windows.Int32Rect.Height as this rectangle;
//        //     otherwise, false.
//        public bool Equals(Int32Rect value)
//        {
//            return Equals(this, value);
//        }

//        //
//        // Summary:
//        //     Creates a hash code from this rectangle's System.Windows.Int32Rect.X, System.Windows.Int32Rect.Y,
//        //     System.Windows.Int32Rect.Width, and System.Windows.Int32Rect.Height values.
//        //
//        // Returns:
//        //     This rectangle's hash code.
//        public override int GetHashCode()
//        {
//            if (IsEmpty)
//            {
//                return 0;
//            }

//            return X.GetHashCode() ^ Y.GetHashCode() ^ Width.GetHashCode() ^ Height.GetHashCode();
//        }

//        //
//        // Summary:
//        //     Creates an System.Windows.Int32Rect structure from the specified System.String
//        //     representation.
//        //
//        // Parameters:
//        //   source:
//        //     A string representation of an System.Windows.Int32Rect.
//        //
//        // Returns:
//        //     The equivalent System.Windows.Int32Rect structure.
//        //public static Int32Rect Parse(string source)
//        //{
//        //    //IFormatProvider invariantEnglishUS = TypeConverterHelper.InvariantEnglishUS;
//        //    //TokenizerHelper tokenizerHelper = new TokenizerHelper(source, invariantEnglishUS);
//        //    //string text = tokenizerHelper.NextTokenRequired();
//        //    //Int32Rect result = ((!(text == "Empty")) ? new Int32Rect(Convert.ToInt32(text, invariantEnglishUS), Convert.ToInt32(tokenizerHelper.NextTokenRequired(), invariantEnglishUS), Convert.ToInt32(tokenizerHelper.NextTokenRequired(), invariantEnglishUS), Convert.ToInt32(tokenizerHelper.NextTokenRequired(), invariantEnglishUS)) : Empty);
//        //    //tokenizerHelper.LastTokenRequired();
//        //    return (Int32Rect) null;
//        //}

//        //
//        // Summary:
//        //     Creates a string representation of this System.Windows.Int32Rect.
//        //
//        // Returns:
//        //     A string containing the same System.Windows.Int32Rect.X, System.Windows.Int32Rect.Y,
//        //     System.Windows.Int32Rect.Width, and System.Windows.Int32Rect.Height values of
//        //     this System.Windows.Int32Rect structure.
//        public override string ToString()
//        {
//            return ConvertToString(null, null);
//        }

//        //
//        // Summary:
//        //     Creates a string representation of this System.Windows.Int32Rect based on the
//        //     supplied System.IFormatProvider.
//        //
//        // Parameters:
//        //   provider:
//        //     The format provider to use. If provider is null, the current culture is used.
//        //
//        // Returns:
//        //     A string representation of this instance of System.Windows.Int32Rect.
//        public string ToString(IFormatProvider provider)
//        {
//            return ConvertToString(null, provider);
//        }

//        //
//        // Summary:
//        //     Formats the value of the current instance using the specified format.
//        //
//        // Parameters:
//        //   format:
//        //     The format to use.
//        //
//        //   provider:
//        //     The provider to use to format the value
//        //
//        // Returns:
//        //     The value of the current instance in the specified format.
//        string IFormattable.ToString(string format, IFormatProvider provider)
//        {
//            return ConvertToString(format, provider);
//        }

//        internal string ConvertToString(string format, IFormatProvider provider)
//        {
//            if (IsEmpty)
//            {
//                return "Empty";
//            }

//            return string.Format(provider, "{1:" + format + "}{0}{2:" + format + "}{0}{3:" + format + "}{0}{4:" + format + "}", _x, _y, _width, _height);
//        }

//        //
//        // Summary:
//        //     Initializes a new instance of an System.Windows.Int32Rect with the specified
//        //     System.Windows.Int32Rect.X and System.Windows.Int32Rect.Y coordinates and the
//        //     specified System.Windows.Int32Rect.Width and System.Windows.Int32Rect.Height.
//        //
//        // Parameters:
//        //   x:
//        //     The System.Windows.Int32Rect.X of the new System.Windows.Int32Rect instance which
//        //     specifies the x-coordinate of the top-left corner of the rectangle.
//        //
//        //   y:
//        //     The System.Windows.Int32Rect.Y of the new System.Windows.Int32Rect instance which
//        //     specifies the y-coordinate of the top-left corner of the rectangle.
//        //
//        //   width:
//        //     The System.Windows.Int32Rect.Width of the new System.Windows.Int32Rect instance
//        //     which specifies the width of the rectangle.
//        //
//        //   height:
//        //     The System.Windows.Int32Rect.Height of the new System.Windows.Int32Rect instance
//        //     which specifies the height of the rectangle.
//        public Int32Rect(int x, int y, int width, int height)
//        {
//            _x = x;
//            _y = y;
//            _width = width;
//            _height = height;
//        }

//        internal void ValidateForDirtyRect(string paramName, int width, int height)
//        {
//            if (_x < 0)
//            {
//                throw new Exception("ParameterCannotBeNegative " + paramName);
//            }

//            if (_y < 0)
//            {
//                throw new Exception("ParameterCannotBeNegative " + paramName);
//            }

//            if (_width < 0 || _width > width)
//            {
//                throw new Exception("ParameterMustBeBetween " + paramName);
//            }

//            if (_height < 0 || _height > height)
//            {
//                throw new Exception("ParameterMustBeBetween " + paramName);
//            }
//        }
//    }
//}
