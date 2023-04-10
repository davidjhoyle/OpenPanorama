using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanaGraph
{
    public class Int32Rect
    {
        internal int _x;

        internal int _y;

        internal int _width;

        internal int _height;

        private static readonly Int32Rect s_empty = new Int32Rect(0, 0, 0, 0);

        public Int32Rect(int x, int y, int width, int height )
        {
            _x = x;
            _y = y;
            _width = width;
            _height = height;
        }

        //
        // Summary:
        //     Gets or sets the x-coordinate of the top-left corner of the rectangle.
        //
        // Returns:
        //     The x-coordinate of the top-left corner of the rectangle. The default value is
        //     0.
        public int X
        {
            get
            {
                return _x;
            }
            set
            {
                _x = value;
            }
        }

        //
        // Summary:
        //     Gets or sets the y-coordinate of the top-left corner of the rectangle.
        //
        // Returns:
        //     The y-coordinate of the top-left corner of the rectangle. The default value is
        //     0.
        public int Y
        {
            get
            {
                return _y;
            }
            set
            {
                _y = value;
            }
        }

        //
        // Summary:
        //     Gets or sets the width of the rectangle.
        //
        // Returns:
        //     The width of the rectangle. The default value is 0.
        public int Width
        {
            get
            {
                return _width;
            }
            set
            {
                _width = value;
            }
        }

        //
        // Summary:
        //     Gets or sets the height of the rectangle.
        //
        // Returns:
        //     The height of the rectangle. The default value is 0.
        public int Height
        {
            get
            {
                return _height;
            }
            set
            {
                _height = value;
            }
        }

    }
}