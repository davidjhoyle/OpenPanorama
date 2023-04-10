using System;
using System.Collections.Generic;
//using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PanaGraph;

namespace OpenPanoramaLib
{
    public class PenColours
    {
        int maxDist = PaintImage.colourMaxDist; // Changes from 20k to 10k.
        int maxHeight = PaintImage.colourMaxHeight;
        int CountGrayShades = PaintImage.colourGreyCount;
        int CountGreenBrownShades = PaintImage.colourGreenBrownShades;
        int CountSlopes = PaintImage.colourSlopeAdjust;
        double colourContrast = 1.3;
        Color[][][] colArray = null;
        Pen[][][] penArray = null;


        public Pen GetPenForDistance(double gradient, double distance, double height, double lat)
        {
            if (colArray == null || penArray == null)
            {
                colArray = new Color[CountGrayShades][][];
                for (int gary = 0; gary < CountGrayShades; gary++)
                {
                    colArray[gary] = new Color[CountGreenBrownShades][];
                }

                penArray = new Pen[CountGrayShades][][];
                for (int gary = 0; gary < CountGrayShades; gary++)
                {
                    penArray[gary] = new Pen[CountGreenBrownShades][];
                }

                Color grn = PaintImage.colourBase;
                Color brwn = PaintImage.colourTops;
                Color gry = PaintImage.colourDistant;

                double deltaBR = (brwn.R - grn.R) / (double)CountGreenBrownShades;
                double deltaBG = (brwn.G - grn.G) / (double)CountGreenBrownShades;
                double deltaBB = (brwn.B - grn.B) / (double)CountGreenBrownShades;

                double deltaR = (grn.R - gry.R) / (double)CountGrayShades;
                double deltaG = (grn.G - gry.G) / (double)CountGrayShades;
                double deltaB = (grn.B - gry.B) / (double)CountGrayShades;

                for (int j = 0; j < CountGreenBrownShades; j++)
                {
                    double startR = grn.R + deltaBR * j;
                    double startG = grn.G + deltaBG * j;
                    double startB = grn.B + deltaBB * j;

                    if (j > 25)
                    {
                        startR = grn.R + deltaBR * 25;
                        //startG = grn.G + deltaBG * j;
                        //startB = grn.B + deltaBB * j;
                    }

                    double deltaR2 = (startR - gry.R) / (double)CountGrayShades;
                    double deltaG2 = (startG - gry.G) / (double)CountGrayShades;
                    double deltaB2 = (startB - gry.B) / (double)CountGrayShades;


                    for (int i = 0; i < CountGrayShades; i++)
                    {
                        colArray[i][j] = new Color[CountSlopes];
                        penArray[i][j] = new Pen[CountSlopes];

                        for (int k = 0; k < CountSlopes; k++)
                        {
                            double multi = ((double)(CountSlopes - k)) / CountSlopes;
                            colArray[i][j][k] = Color.FromArgb((int)((startR - deltaR2 * i) * multi), (int)((startG - deltaG2 * i) * multi), (int)((startB - deltaB2 * i) * multi));
                            penArray[i][j][k] = new Pen(colArray[i][j][k], 1);
                        }
                    }
                }
            }

            int indx = (int)(distance / maxDist * CountGrayShades);
            if (indx >= CountGrayShades)
            {
                indx = CountGrayShades - 1;
            }
            if (indx < 0)
            {
                indx = 0;
            }

            // Adjust the height for latitude... 20m height for 1 degree of latitude above/belove 45degrees.
            height += (lat - PaintImage.colourOriginLatitude) * 20;

            int jndx = (int)(height / maxHeight * CountGreenBrownShades);
            if (jndx >= CountGreenBrownShades)
            {
                jndx = CountGreenBrownShades - 1;
            }
            if (jndx < 0)
            {
                jndx = 0;
            }

            gradient = ((Math.PI / 2) - gradient) / (Math.PI / 2) * CountSlopes * colourContrast;

            if (gradient < 0)
            {
                gradient = 0;
            }

            if (gradient >= CountSlopes)
            {
                gradient = CountSlopes - 1;
            }

            return penArray[indx][jndx][(int)gradient];
        }


    }
}
