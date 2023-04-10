using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenPanoramaLib
{
    public class LidarBlock
    {
        public string filename;
        public int ncols; // 1000
        public int nrows; // 1000
        public double xllcorner; //    375000
        public double yllcorner; //    305000
        public double cellsize; //     1
        public int NODATA_value; // -9999
        public float[][] values;
        public int unset_points;

        public const int NODATA_const = -9999;
        public bool changed = false;

        public LidarBlock(string fn)
        {
            filename = fn;
            ncols = 0;
            nrows = 0;
            xllcorner = 0;
            yllcorner = 0;
            cellsize = 0;
            NODATA_value = NODATA_const;
            values = null;
            unset_points = 0;
        }

        public LidarBlock(string fn, int Ncols, int Nrows)
        {
            filename = fn;
            ncols = Ncols;
            nrows = Nrows;
            unset_points = ncols * nrows;
            xllcorner = 0;
            yllcorner = 0;
            cellsize = 0;
            NODATA_value = NODATA_const;
            values = null;

            values = new float[nrows][];
            for (int y = 0; y < nrows; y++)
            {
                values[y] = new float[ncols];
                for (int x = 0; x < ncols; x++)
                {
                    values[y][x] = NODATA_value;
                }
            }
        }



        public void WriteASC(string fn, StreamWriter stream)
        {
            TextWriter tw = stream;

            Console.WriteLine("Write ASC " + fn);
            tw.WriteLine("ncols " + ncols);
            tw.WriteLine("nrows " + nrows);
            tw.WriteLine("xllcorner " + xllcorner);
            tw.WriteLine("yllcorner " + yllcorner);
            tw.WriteLine("cellsize " + cellsize);
            tw.WriteLine("NODATA_value " + NODATA_value);

            for (int y = nrows - 1; y >= 0; y--)
            {
                StringBuilder aline = new StringBuilder("");

                for (int x = 0; x < ncols; x++)
                {
                    if (x != 0)
                    {
                        aline.Append(" ");
                    }
                    aline.Append(values[y][x]);
                }
                tw.WriteLine(aline);
            }
        }
    }
}
