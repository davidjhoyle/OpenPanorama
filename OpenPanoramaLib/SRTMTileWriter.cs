using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace OpenPanoramaLib
{
    public class SRTMTileWriter
    {
        public int tileLat;
        public int tileLon;
        public double[][] tileDoubles;
        public int[][] tileCounts;
        public long totalpointsset = 0;

        public static Dictionary<int, SRTMTileWriter> SRTMTiles = new Dictionary<int, SRTMTileWriter>();


        public SRTMTileWriter(int LatInt, int LonInt)
        {
            tileLat = LatInt;
            tileLon = LonInt;

            tileDoubles = new double[ZipMapDataHandler.gridSize][];
            tileCounts = new int[ZipMapDataHandler.gridSize][];

            for (int i = 0; i < ZipMapDataHandler.gridSize; i++)
            {
                tileDoubles[i] = new double[ZipMapDataHandler.gridSize];
                tileCounts[i] = new int[ZipMapDataHandler.gridSize];
            }
        }


        public void SaveTile(string path)
        {
            int lat = tileLat;
            int lon = tileLon;
            if (lon > 180)
            {
                lon -= 360;
            }
            if (lat > 90)
            {
                lat -= 180;
            }

            // N27E036.hgt;
            string altlonstr = ZipMapDataHandler.GetSRTMFilePrefixNameForLatLon(lat, lon);
            string filename = path + "\\" + altlonstr + ".hgt";

            Console.WriteLine("SaveTile " + filename + " Points set " + totalpointsset + " Out of " + ZipMapDataHandler.gridSize * ZipMapDataHandler.gridSize + " = " + 100 * totalpointsset / (ZipMapDataHandler.gridSize * ZipMapDataHandler.gridSize) + "%");

            using (BinaryWriter bw = new BinaryWriter(File.Open(filename, FileMode.Create)))
            {
                byte[] point = new byte[2];
                for (lat = ZipMapDataHandler.gridSize - 1; lat >= 0; lat--)
                {
                    for (lon = 0; lon < ZipMapDataHandler.gridSize; lon++)
                    {
                        point[0] = 0;
                        point[1] = 0;
                        if (tileCounts[lat][lon] > 0)
                        {
                            Int16 value = (Int16)((tileDoubles[lat][lon] / tileCounts[lat][lon]) + 0.5);
                            point[0] = (byte)((value >> 8) & 0xff);
                            point[1] = (byte)(value & 0xff);
                        }
                        bw.Write(point);
                    }
                }
                bw.Close();
            }
        }



        public static void ProcessAndSaveTiles(string path)
        {
            foreach (SRTMTileWriter tw in SRTMTiles.Values)
            {
                tw.ProcessHeight();
                tw.SaveTile(path);
            }
        }



        public void ProcessHeight()
        {
            Console.WriteLine("SaveTile Lat " + tileLat + " Lon " + tileLon);

            for (int latint = 0; latint < ZipMapDataHandler.gridSize; latint++)
            {
                double lat = (double)tileLat + ((double)latint) / ZipMapDataHandler.gridCount;
                for (int lonint = 0; lonint < ZipMapDataHandler.gridSize; lonint++)
                {
                    if (tileCounts[latint][lonint] == 0)
                    {
                        double mindist = 0;
                        double lon = (double)tileLon + ((double)lonint) / ZipMapDataHandler.gridCount;
                        tileDoubles[latint][lonint] = ZipMapDataHandler.GetHeightFromOSVectorData(lat, lon, countryEnum.uk, true, ref mindist);
                        if (tileDoubles[latint][lonint] < 0)
                        {
                            tileDoubles[latint][lonint] = 0;
                        }
                        tileCounts[latint][lonint] = 1;
                    }
                }
                Console.Write(".");

                if ((latint % 36) == 0)
                {
                    Console.Write(" " + latint / 36 + "% ");
                }
            }
            Console.WriteLine("");
        }


        public static void AddHeight(int LatInt, int LonInt, int OffLat, int OffLon, double height)
        {
            SRTMTileWriter tw = null;

            int LatLonIndex = LatInt * 1000 + LonInt;
            if (!SRTMTiles.ContainsKey(LatLonIndex))
            {
                tw = new SRTMTileWriter(LatInt, LonInt);
                SRTMTiles.Add(LatLonIndex, tw);
            }
            else
            {
                tw = SRTMTiles[LatLonIndex];
            }

            if (OffLat < 0 || OffLat >= ZipMapDataHandler.gridSize || OffLon < 0 || OffLon >= ZipMapDataHandler.gridSize)
            {
                return;
            }

            if (tw.tileCounts[OffLat][OffLon] == 0)
            {
                tw.totalpointsset++;
            }
            tw.tileDoubles[OffLat][OffLon] += height;
            tw.tileCounts[OffLat][OffLon] += 1;
        }


        public static void AddHeight(double lat, double lon, double height)
        {
            if (lon < 0)
            {
                lon += 360;
            }
            if (lat < 0)
            {
                lat += 180;
            }

            int LatInt = (int)lat;
            int LonInt = (int)lon;

            int OffLat = (int)((lat - LatInt) * ZipMapDataHandler.gridCount);
            int OffLon = (int)((lon - LonInt) * ZipMapDataHandler.gridCount);

            AddHeight(LatInt, LonInt, OffLat, OffLon, height);

            if (OffLat == 0)
            {
                AddHeight(LatInt - 1, LonInt, ZipMapDataHandler.gridCount, OffLon, height);
            }

            if (OffLon == 0)
            {
                AddHeight(LatInt, LonInt - 1, OffLat, ZipMapDataHandler.gridCount, height);
            }

            if (OffLat == 0 && OffLon == 0)
            {
                AddHeight(LatInt - 1, LonInt - 1, ZipMapDataHandler.gridCount, OffLon, ZipMapDataHandler.gridCount);
            }
        }
    }
}
