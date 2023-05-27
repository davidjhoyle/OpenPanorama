using Proj4Net.Core;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//using ImageProcessor;
//using ImageProcessor.Core;
//using ImageProcessor.Core.Common;
//using ImageProcessor.Core.Imaging;
//using ImageProcessor.Core.Processors;

//using CoreCompat.System.Drawing;

using PanaGraph;

using BitMiracle;
using BitMiracle.LibTiff;
using BitMiracle.LibTiff.Classic;
using System.Reflection.Metadata;
using static System.Net.Mime.MediaTypeNames;
using System.Collections;
using System.Drawing;


namespace OpenPanoramaLib
{
    public class LidarBlockReader
    {
        static string LidarFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\LIDAR\\";
        static string LIDARCacheFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\LIDARCACHE\\";

        static int LidarSiz = 10000;
        static double LidarRes = 1;
        const string extension = "LIDAR-DTM-1m-2019-SK26nw.zip";
        const string ENprefix = "england\\"; // LIDAR-DTM-1m-2019-SK26nw.zip
        const string CYprefix = "wales\\"; // 2m_res_SR_dtm.zip
        const string SCprefix = "scotland\\";
        const string IEprefix = "ie\\"; // 2m_res_SR_dtm.zip
        const string NOprefix = "norway\\"; // Basisdata_6601-1_Celle_25833_DTM10-2021UTM33_TIFF.zip
        const string NOpatchesprefix = "norwaypatches\\"; // Basisdata_6601-1_Celle_25833_DTM10-2021UTM33_TIFF.zip

        static readonly string[] WalesSquares = new string[] { "sh", "sj", "sm", "sn", "so", "sr", "ss", "st" };


        static Dictionary<int, LidarBlockCacheLine> cachelines = new Dictionary<int, LidarBlockCacheLine>();
        static List<string> allLIDARCacheFiles = new List<string>();
        static int maxlines = 4;
        static public ulong lastAccessTicker = 1;


        public LidarBlockReader()
        {
        }

        public static void SetLIDARCacheSize(int siz)
        {
            maxlines = siz;
        }

        public static int GetLidarSiz()
        {
            return LidarSiz;
        }

        public static void SetLidarSiz(int siz)
        {
            LidarSiz = siz;
        }

        public static double GetLidarRes()
        {
            return LidarRes;
        }

        public static void SetLidarRes(double res)
        {
            LidarRes = res;
        }

        public static void SelectFolder(string fn)
        {
            LidarFolder = fn;
            if (LidarFolder.Length > 0 && LidarFolder[LidarFolder.Length - 1] != '\\')
            {
                LidarFolder += "\\";
            }
        }

        public static void SelectCacheFolder(string fn)
        {
            LIDARCacheFolder = fn;
            if (LIDARCacheFolder.Length > 0 && LIDARCacheFolder[LIDARCacheFolder.Length - 1] != '\\')
            {
                LIDARCacheFolder += "\\";
            }
        }


        public static string GetLidarCacheFilename(string gridname, string country)
        {
            // Is there a cached copy of the data we can use?
            return LIDARCacheFolder + "BinCache_" + country + gridname + ".lid";
        }


        public static LidarBlock CacheLidarFileAdd(LidarBlock cacheblock)
        {
            lock (cachelines)
            {
                int lookup = GetLookup((int)cacheblock.xllcorner, (int)cacheblock.yllcorner, (int)(cacheblock.cellsize * cacheblock.nrows));


                // Is item already in the cache or not?
                if (cachelines.ContainsKey(lookup))
                {
                    return cachelines[lookup].asc;
                }

                cachelines.Add(lookup, new LidarBlockCacheLine());
                cachelines[lookup].asc = cacheblock;
                cachelines[lookup].lastAccess = lastAccessTicker++;
                cachelines[lookup].asc.changed = false;

                if (!allLIDARCacheFiles.Contains(cacheblock.filename))
                {
                    allLIDARCacheFiles.Add(cacheblock.filename);
                }

                CacheLidarFileEvictOldest();

                return cacheblock;
            }
        }



        public static LidarBlock CreateAndAdd(string filename, double xll, double yll, int siz, double res)
        {
            double orgxll = xll;
            double orgyll = yll;

            // Round to the start of the ASC grid.
            double axll2 = (xll / siz) * siz;
            double ayll2 = (yll / siz) * siz;

            int ixll = (int)(Math.Floor((double)xll / siz) * siz);
            int iyll = (int)(Math.Floor((double)yll / siz) * siz);

            // Now create the final composite LIDAR Tile...
            LidarBlock theLidarBlock = new LidarBlock(filename, (int)(siz / res), (int)(siz / res));
            theLidarBlock.xllcorner = (double)ixll + res / 2;
            theLidarBlock.yllcorner = (double)iyll + res / 2;
            theLidarBlock.cellsize = (float)res;

            CacheLidarFileAdd(theLidarBlock);

            return theLidarBlock;
        }


        public static LidarBlock ReadOrAdd(string filname, double xll, double yll, int siz, double res)
        {
            LidarBlock asc = GetCachedASC((int)xll, (int)yll, siz);
            if (asc != null)
            {
                return asc;
            }

            asc = LidarBlockReader.ReadFileCache(filname);
            if (asc == null)
            {
                asc = CreateAndAdd(filname, xll, yll, siz, res);
            }

            return asc;
        }


        public static void CacheLidarFileEvictOldest()
        {
            if (cachelines.Count > maxlines)
            {
                ulong oldesttick = 0xffffffff;
                int oldest = -1;
                bool found = false;
                try
                {
                    foreach (int key in cachelines.Keys)
                    {
                        if (cachelines[key].lastAccess < oldesttick)
                        {
                            oldesttick = cachelines[key].lastAccess;
                            oldest = key;
                            found = true;
                        }
                    }
                    if (found)
                    {
                        CacheLidarFileEvict(oldest);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("CacheLidarFileEvictOldest Failed " + oldest + " Exception " + ex.Message + " " + ex.StackTrace);
                }
            }
        }



        public static void CacheLidarFileEvictAll()
        {
            List<int> allkeys = new List<int>();
            foreach (int key in cachelines.Keys)
            {
                allkeys.Add(key);
            }
            foreach (int key in allkeys)
            {
                CacheLidarFileEvict(key);
            }
        }



        public static void CacheLidarFileEvict(int evictLine)
        {
            if (cachelines.ContainsKey(evictLine))
            {
                // Do we need to flush this LIDAR Cache line to disk?
                if (cachelines[evictLine].asc.changed)
                {
                    SaveFileCache(cachelines[evictLine].asc.filename, cachelines[evictLine].asc, cachelines[evictLine].asc.unset_points);
                }
                cachelines.Remove(evictLine);
            }
            else
            {
                Console.WriteLine("CacheLidarFileEvict( " + evictLine + " Failed as key not in cache");
            }
        }


        public static LidarBlock GetCachedASC(int lookup)
        {
            lock (cachelines)
            {
                if (cachelines.ContainsKey(lookup))
                {
                    cachelines[lookup].lastAccess = lastAccessTicker++;
                    return cachelines[lookup].asc;
                }
            }
            return null;
        }

        public static LidarBlock GetCachedASC(int x, int y, int siz)
        {
            return GetCachedASC(GetLookup(x, y, siz));
        }


        public static int GetLookup(int x, int y, int siz = 10000)
        {
            int lookup = (x / siz) + (y / siz) * siz;
            return lookup;
        }


        public static void SetCachedValuePointHeight(double x, double y, float height, countryEnum eCountry, int siz)
        {
            string gridname = GetGridName((int)x, (int)y, GetLidarSiz(), eCountry);
            string fn = GetLidarCacheFilename(gridname, eCountry.ToString());

            LidarBlock asc = GetCachedASC((int)x, (int)y, siz);
            if (asc == null)
            {
                asc = ReadOrAdd(fn, x, y, GetLidarSiz(), GetLidarRes());
            }

            if (asc != null)
            {
                int indxX = (int)((x - asc.xllcorner) / asc.cellsize + asc.cellsize * 0.5);
                int indxY = (int)((y - asc.yllcorner) / asc.cellsize + asc.cellsize * 0.5);

                if (indxX >= 0 && indxX < asc.ncols && indxY >= 0 && indxY < asc.nrows)
                {
                    asc.values[indxY][indxX] = height;
                    asc.changed = true;

                    // Is is already set or not?
                    if (asc.values[indxY][indxX] < -10)
                    {
                        asc.unset_points -= 1;
                    }
                }
            }
        }


        public static string GetFolder(string country)
        {
            switch (country)
            {
                case "england":
                    return LidarFolder + ENprefix;
                case "wales":
                    return LidarFolder + CYprefix;
                case "scotland":
                    return LidarFolder + SCprefix;
                case "ie":
                    return LidarFolder + IEprefix;
                case "bd":
                    return LidarFolder + IEprefix;
                case "norway":
                    return LidarFolder + NOprefix;
                case "nopatches":
                    return LidarFolder + NOpatchesprefix;
                case "patches":
                    return LidarFolder + "patches";
                case "iepatches":
                    return LidarFolder + "iepatches";
                default:
                    return null;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="xll"></param>
        /// <param name="yll"></param>
        /// <param name="siz">The size determines the number of digits to return. For a size of 1000m, the grid should be to the nearest km so needs 4 digits.</param>
        /// <returns></returns>
        public static string GetGridName(int xll, int yll, int siz, countryEnum eCountry)
        {
            string gridname = "";
            if (eCountry == countryEnum.no)
            {
                if (xll < 0)
                {
                    gridname = "" + ((int)yll / 100000).ToString("00") + "m" + ((int)(-xll / 100000 + 1)).ToString("0");
                }
                else
                {
                    gridname = "" + ((int)yll / 100000).ToString("00") + "" + ((int)(xll / 100000)).ToString("00");
                }
            }
            else if (eCountry == countryEnum.ie)
            {
                gridname = "" + (int)(xll / 10000) + "" + (int)yll / 10000;
            }
            else
            {
                OsGridRef osref = new OsGridRef(xll, yll);
                int digits = 10; // (1m)
                while (siz > 1 && digits > 0)
                {
                    siz = siz / 10;
                    digits -= 2;
                }

                gridname = osref.toString(digits).Replace(" ", "");
            }
            return gridname;
        }


        public static LidarTFW ReadTFW(string fn, Stream stream)
        {
            LidarTFW tfw = new LidarTFW();
            string aline;

            using (TextReader tr = new StreamReader(stream))
            {
                //Console.WriteLine("Read TFW ");
                aline = tr.ReadLine();
                if (aline != null)
                {
                    tfw.xScale = Convert.ToDouble(aline);
                }
                aline = tr.ReadLine();
                if (aline != null)
                {
                    tfw.rot1 = Convert.ToDouble(aline);
                }
                aline = tr.ReadLine();
                if (aline != null)
                {
                    tfw.rot2 = Convert.ToDouble(aline);
                }
                aline = tr.ReadLine();
                if (aline != null)
                {
                    tfw.NegY = Convert.ToDouble(aline);
                }
                aline = tr.ReadLine();
                if (aline != null)
                {
                    tfw.xllcorner = Convert.ToDouble(aline);
                }
                aline = tr.ReadLine();
                if (aline != null)
                {
                    tfw.yllcorner = Convert.ToDouble(aline);
                }
                //Console.WriteLine("TFW " + tfw );
            }
            return tfw;
        }







        public static LidarTIF ReadTIFBounded(string fn, Stream stream, bool seekable, int xllcorner, int yllcorner, int siz)
        {
            //Console.WriteLine("Read TIF " + fn );

            // Round to the start of the ASC grid.
            //double axll2 = (xllcorner / siz) * siz;
            //double ayll2 = (yllcorner / siz) * siz;

            xllcorner = (xllcorner / siz) * siz;
            yllcorner = (yllcorner / siz) * siz;

            //int ixll = (int)(Math.Floor((double)xllcorner / siz) * siz);
            //int iyll = (int)(Math.Floor((double)yllcorner / siz) * siz);


            if (fn != null && stream == null)
            {
                stream = new FileStream(fn, FileMode.Open, FileAccess.Read);
                seekable = true;
            }

            LidarTIF tif = new LidarTIF();
            tif.filename = fn;
            tif.tifstream = new TiffStream();
            MemoryStream ms = null;
            if (!seekable)
            {
                ms = new MemoryStream();
                stream.CopyTo(ms);
                ms.Position = 0;
                stream = ms;
            }

            tif.bmtiff = Tiff.ClientOpen(fn, "r", stream, tif.tifstream);
            tif.cc = countryEnum.unknown;

            //Image size
            int nTIFWidth = tif.bmtiff.GetField(TiffTag.IMAGEWIDTH)[0].ToInt();
            int nTIFHeight = tif.bmtiff.GetField(TiffTag.IMAGELENGTH)[0].ToInt();

            bool largeTIF = nTIFWidth > siz || nTIFHeight > siz;

            if (largeTIF)
            {
                //nTIFWidth = siz;
                //nTIFHeight = siz;
                tif.width = siz;
                tif.height = siz;
            }
            else
            {
                tif.width = nTIFWidth;
                tif.height = nTIFHeight;
            }


            // Allocate and initialise the data
            tif.pixels = new float[tif.height, tif.width];
            for (int y = 0; y < tif.height; y++)
            {
                for (int x = 0; x < tif.width; x++)
                {
                    tif.pixels[y, x] = LidarBlock.NODATA_const;
                }
            }

            if (nTIFHeight != nTIFWidth)
            {
                Console.WriteLine("TIF " + fn + " Width " + nTIFWidth + " Height " + nTIFHeight);
            }

            //33550(0x830e) - GEOTIFF_MODELPIXELSCALETAG
            //33922(0x8482) - GEOTIFF_MODELTIEPOINTTAG
            //34735(0x87af) - GEOTIFF_GEOKEYDIRECTORYTAG
            //34736(0x87b0) - GEOTIFF_GEODOUBLEPARAMSTAG
            //34737(0x87b1) - GEOTIFF_GEOASCIIPARAMSTAG
            //42112(0xa480) - No Idea
            //42113(0xa481) - No Idea

            /*
                * Another Tag 1
            +		Value	{byte[136]}	object {byte[]}

            Another Tag 2
            +		[1]	{:R¢Fß?bXÙTXA¹3µr@	BitMiracle.LibTiff.Classic.FieldValue

            Another Tag 3
            +		[1]	{PCS Name = British_National_Grid|GCS Name = GCS_OSGB_1936|Datum = D_OSGB_1936|Ellipsoid = Airy_1830|Primem = Greenwich||ESRI PE String = PROJCS["British_National_Grid",GEOGCS["GCS_OSGB_1936",DATUM["D_OSGB_1936",SPHEROID["Airy_1830",6377563.396,299.3249646]],PRIMEM["Greenwich",0.0],UNIT["Degree",0.0174532925199433]],PROJECTION["Transverse_Mercator"],PARAMETER["False_Easting",400000.0],PARAMETER["False_Northing",-100000.0],PARAMETER["Central_Meridian",-2.0],PARAMETER["Scale_Factor",0.9996012717],PARAMETER["Latitude_Of_Origin",49.0],UNIT["Meter",1.0]]|	BitMiracle.LibTiff.Classic.FieldValue

            Another Tag 4
            -		[1]	{<GDALMetadata>
                <Item name="DataType">Generic</Item>
                <Item name="PyramidResamplingType" domain="Esri">AVERAGE</Item>
                <Item name="BandName" sample="0">Band_1</Item>
                <Item name="RepresentationType" sample="0">ATHEMATIC</Item>
                <Item name="STATISTICS_COVARIANCES" sample="0">41146.60973943795</Item>
                <Item name="STATISTICS_MAXIMUM" sample="0">930.39001464844</Item>
                <Item name="STATISTICS_MEAN" sample="0">456.75267080621</Item>
                <Item name="STATISTICS_MINIMUM" sample="0">71.282005310059</Item>
                <Item name="STATISTICS_SKIPFACTORX" sample="0">1</Item>
                <Item name="STATISTICS_SKIPFACTORY" sample="0">1</Item>
                <Item name="STATISTICS_STDDEV" sample="0">202.84627119925</Item>
            </GDALMetadata>
                BitMiracle.LibTiff.Classic.FieldValue

            Another Tag 5
            +		[1]	{-3.4028234663852886e+38	BitMiracle.LibTiff.Classic.FieldValue

                */

            FieldValue[] modelPixelScaleTag = tif.bmtiff.GetField(TiffTag.GEOTIFF_MODELPIXELSCALETAG);
            FieldValue[] modelTiePointTag = tif.bmtiff.GetField(TiffTag.GEOTIFF_MODELTIEPOINTTAG);
            FieldValue[] anotherTag1 = tif.bmtiff.GetField(TiffTag.GEOTIFF_GEOKEYDIRECTORYTAG);
            FieldValue[] anotherTag2 = tif.bmtiff.GetField(TiffTag.GEOTIFF_GEODOUBLEPARAMSTAG);
            FieldValue[] anotherTag3 = tif.bmtiff.GetField(TiffTag.GEOTIFF_GEOASCIIPARAMSTAG);
            FieldValue[] anotherTag4 = tif.bmtiff.GetField((TiffTag)42112);
            FieldValue[] anotherTag5 = tif.bmtiff.GetField((TiffTag)42113);

            byte[] modelPixelScale = modelPixelScaleTag[1].GetBytes();
            double dW = BitConverter.ToDouble(modelPixelScale, 0);
            double dH = BitConverter.ToDouble(modelPixelScale, 8) * -1;


            byte[] modelTransformation = modelTiePointTag[1].GetBytes();
            double originLon = BitConverter.ToDouble(modelTransformation, 24);
            double originLat = BitConverter.ToDouble(modelTransformation, 32);

            double startW = originLon + dW / 2.0;
            double startH = originLat + dH / 2.0;

            if (largeTIF)
            {
                startW = xllcorner + dW / 2.0;
                startH = yllcorner + dH / 2.0;

                if (dH < 0)
                {
                    dH = -dH;
                    //startH -= dH * siz;
                    originLat -= dH * nTIFHeight;
                }
            }
            else
            {
                if (dH < 0)
                {
                    dH = -dH;
                    startH -= dH * nTIFHeight;
                    originLat -= dH * nTIFHeight;
                }
            }

            FieldValue[] tileByteCountsTag = tif.bmtiff.GetField(TiffTag.TILEBYTECOUNTS);
            long[] tileByteCounts = tileByteCountsTag[0].TolongArray();

            FieldValue[] bitsPerSampleTag = tif.bmtiff.GetField(TiffTag.BITSPERSAMPLE);
            int bytesPerSample = bitsPerSampleTag[0].ToInt() / 8;

            FieldValue[] tilewtag = tif.bmtiff.GetField(TiffTag.TILEWIDTH);
            FieldValue[] tilehtag = tif.bmtiff.GetField(TiffTag.TILELENGTH);
            int tilew = tilewtag[0].ToInt();
            int tileh = tilehtag[0].ToInt();


            tif.tfw = new LidarTFW();
            tif.tfw.xScale = dW;
            tif.tfw.rot1 = 0;
            tif.tfw.rot2 = 0;
            tif.tfw.NegY = dH;     // E = negative of y-scale; dimension of a pixel in map units in y direction
            tif.tfw.xllcorner = startW; // C -  x, y map coordinates
            tif.tfw.yllcorner = startH; // - tif.decoder.Height;// F -  x, y map coordinates



            int tileWidthCount = nTIFWidth / tilew;
            int remainingWidth = nTIFWidth - tileWidthCount * tilew;
            if (remainingWidth > 0)
            {
                tileWidthCount++;
            }

            int tileHeightCount = nTIFHeight / tileh;
            int remainingHeight = nTIFHeight - tileHeightCount * tileh;
            if (remainingHeight > 0)
            {
                tileHeightCount++;
            }

            int offsetX = 0;
            int offsetY = 0;
            int endX = nTIFWidth;
            int endY = nTIFHeight;

            if ( largeTIF)
            {
                offsetX = (int) (xllcorner - originLon);
                offsetY = (int) (yllcorner - originLat);
                endX = offsetX + siz;
                endY = offsetY + siz;
            }

            int tileSize = tif.bmtiff.TileSize();
            for (int iw = 0; iw < nTIFWidth; iw += tilew)
            {
                if (iw + tilew < offsetX || iw > endX)
                {
                    continue;
                }
                for (int ih = 0; ih < nTIFHeight; ih += tileh)
                {
                    int tmpih = nTIFHeight - ih - 1;
                    if (tmpih + tileh < offsetY || tmpih + tileh > endY)
                    {
                        continue;
                    }

                    byte[] buffer = new byte[tileSize];
                    tif.bmtiff.ReadTile(buffer, 0, iw, ih, 0, 0);
                    for (int itw = 0; itw < tilew; itw++)
                    {
                        int iwhm = iw + itw - offsetX;
                        if (iwhm >= tif.width)
                        {
                            break;
                        }
                        if ( iwhm < 0)
                        {
                            continue;
                        }
                        for (int ith = 0; ith < tileh; ith++)
                        {
                            //int iyhm = ih + ith;

                            int iyhm = (nTIFHeight - (ih + ith + 1)) - offsetY;
                            //int iwhm = (nTIFHeight - (ih + ith + 1)) - offsetX;

                            //int iyhm = ih + ith - offsetY;

                            if (iyhm >= tif.height)
                            {
                                break;
                            }
                            if ( iyhm < 0)
                            {
                                continue;
                            }
                            try
                            {
                                tif.pixels[iyhm, iwhm] = BitConverter.ToSingle(buffer, (itw + tileh * ith ) * 4);
                            }
                            catch (Exception e)
                            {
                                throw;
                            }
                        }
                    }
                }
            }
            return tif;
        }



        public static LidarTIF ReadTIF(string fn, Stream stream, bool seekable)
        {
            //Console.WriteLine("Read TIF " + fn );

            if (fn != null && stream == null)
            {
                stream = new FileStream(fn, FileMode.Open, FileAccess.Read);
                seekable = true;
            }

            LidarTIF tif = new LidarTIF();
            tif.filename = fn;
            tif.tifstream = new TiffStream();
            MemoryStream ms = null;
            if (!seekable)
            {
                ms = new MemoryStream();
                stream.CopyTo(ms);
                ms.Position = 0;
                stream = ms;
            }

            tif.bmtiff = Tiff.ClientOpen(fn, "r", stream, tif.tifstream);

            tif.cc = countryEnum.unknown;
                
            //Image size
            int nWidth = tif.bmtiff.GetField(TiffTag.IMAGEWIDTH)[0].ToInt();
            int nHeight = tif.bmtiff.GetField(TiffTag.IMAGELENGTH)[0].ToInt();

            tif.width = nWidth;
            tif.height = nHeight;

            tif.pixels = new float[nHeight, nWidth ];

            if (nHeight != nWidth)
            {
                Console.WriteLine("TIF " + fn + " Width " + nWidth + " Height " + nHeight);
            }

            //33550(0x830e) - GEOTIFF_MODELPIXELSCALETAG
            //33922(0x8482) - GEOTIFF_MODELTIEPOINTTAG
            //34735(0x87af) - GEOTIFF_GEOKEYDIRECTORYTAG
            //34736(0x87b0) - GEOTIFF_GEODOUBLEPARAMSTAG
            //34737(0x87b1) - GEOTIFF_GEOASCIIPARAMSTAG
            //42112(0xa480) - No Idea
            //42113(0xa481) - No Idea

            /*
                * Another Tag 1
            +		Value	{byte[136]}	object {byte[]}

            Another Tag 2
            +		[1]	{:R¢Fß?bXÙTXA¹3µr@	BitMiracle.LibTiff.Classic.FieldValue

            Another Tag 3
            +		[1]	{PCS Name = British_National_Grid|GCS Name = GCS_OSGB_1936|Datum = D_OSGB_1936|Ellipsoid = Airy_1830|Primem = Greenwich||ESRI PE String = PROJCS["British_National_Grid",GEOGCS["GCS_OSGB_1936",DATUM["D_OSGB_1936",SPHEROID["Airy_1830",6377563.396,299.3249646]],PRIMEM["Greenwich",0.0],UNIT["Degree",0.0174532925199433]],PROJECTION["Transverse_Mercator"],PARAMETER["False_Easting",400000.0],PARAMETER["False_Northing",-100000.0],PARAMETER["Central_Meridian",-2.0],PARAMETER["Scale_Factor",0.9996012717],PARAMETER["Latitude_Of_Origin",49.0],UNIT["Meter",1.0]]|	BitMiracle.LibTiff.Classic.FieldValue

            Another Tag 4
            -		[1]	{<GDALMetadata>
                <Item name="DataType">Generic</Item>
                <Item name="PyramidResamplingType" domain="Esri">AVERAGE</Item>
                <Item name="BandName" sample="0">Band_1</Item>
                <Item name="RepresentationType" sample="0">ATHEMATIC</Item>
                <Item name="STATISTICS_COVARIANCES" sample="0">41146.60973943795</Item>
                <Item name="STATISTICS_MAXIMUM" sample="0">930.39001464844</Item>
                <Item name="STATISTICS_MEAN" sample="0">456.75267080621</Item>
                <Item name="STATISTICS_MINIMUM" sample="0">71.282005310059</Item>
                <Item name="STATISTICS_SKIPFACTORX" sample="0">1</Item>
                <Item name="STATISTICS_SKIPFACTORY" sample="0">1</Item>
                <Item name="STATISTICS_STDDEV" sample="0">202.84627119925</Item>
            </GDALMetadata>
                BitMiracle.LibTiff.Classic.FieldValue

            Another Tag 5
            +		[1]	{-3.4028234663852886e+38	BitMiracle.LibTiff.Classic.FieldValue

                */

            FieldValue[] modelPixelScaleTag = tif.bmtiff.GetField(TiffTag.GEOTIFF_MODELPIXELSCALETAG);
            FieldValue[] modelTiePointTag = tif.bmtiff.GetField(TiffTag.GEOTIFF_MODELTIEPOINTTAG);
            FieldValue[] anotherTag1 = tif.bmtiff.GetField(TiffTag.GEOTIFF_GEOKEYDIRECTORYTAG);
            FieldValue[] anotherTag2 = tif.bmtiff.GetField(TiffTag.GEOTIFF_GEODOUBLEPARAMSTAG);
            FieldValue[] anotherTag3 = tif.bmtiff.GetField(TiffTag.GEOTIFF_GEOASCIIPARAMSTAG);
            FieldValue[] anotherTag4 = tif.bmtiff.GetField((TiffTag) 42112);
            FieldValue[] anotherTag5 = tif.bmtiff.GetField((TiffTag) 42113);

            byte[] modelPixelScale = modelPixelScaleTag[1].GetBytes();
            double dW = BitConverter.ToDouble(modelPixelScale, 0);
            double dH = BitConverter.ToDouble(modelPixelScale, 8) * -1;


            byte[] modelTransformation = modelTiePointTag[1].GetBytes();
            double originLon = BitConverter.ToDouble(modelTransformation, 24);
            double originLat = BitConverter.ToDouble(modelTransformation, 32);

            double startW = originLon + dW / 2.0;
            double startH = originLat + dH / 2.0;
            if (dH < 0)
            {
                dH = -dH;
                startH -= dH * nHeight;
                originLat -= dH * nHeight;
            }


            FieldValue[] tileByteCountsTag = tif.bmtiff.GetField(TiffTag.TILEBYTECOUNTS);
            long[] tileByteCounts = tileByteCountsTag[0].TolongArray();

            FieldValue[] bitsPerSampleTag = tif.bmtiff.GetField(TiffTag.BITSPERSAMPLE);
            int bytesPerSample = bitsPerSampleTag[0].ToInt() / 8;

            FieldValue[] tilewtag = tif.bmtiff.GetField(TiffTag.TILEWIDTH);
            FieldValue[] tilehtag = tif.bmtiff.GetField(TiffTag.TILELENGTH);
            int tilew = tilewtag[0].ToInt();
            int tileh = tilehtag[0].ToInt();


            tif.tfw = new LidarTFW();
            tif.tfw.xScale = dW;
            tif.tfw.rot1 = 0;
            tif.tfw.rot2 = 0;
            tif.tfw.NegY = dH;     // E = negative of y-scale; dimension of a pixel in map units in y direction
            tif.tfw.xllcorner = startW; // C -  x, y map coordinates
            tif.tfw.yllcorner = startH; // - tif.decoder.Height;// F -  x, y map coordinates



            int tileWidthCount = nWidth / tilew;
            int remainingWidth = nWidth - tileWidthCount * tilew;
            if (remainingWidth > 0)
            {
                tileWidthCount++;
            }

            int tileHeightCount = nHeight / tileh;
            int remainingHeight = nHeight - tileHeightCount * tileh;
            if (remainingHeight > 0)
            {
                tileHeightCount++;
            }


            int tileSize = tif.bmtiff.TileSize();
            for (int iw = 0; iw < nWidth; iw += tilew)
            {
                for (int ih = 0; ih < nHeight; ih += tileh)
                {
                    byte[] buffer = new byte[tileSize];
                    tif.bmtiff.ReadTile(buffer, 0, iw, ih, 0, 0);
                    for (int itw = 0; itw < tilew; itw++)
                    {
                        int iwhm = iw + itw;
                        if (iwhm > nWidth - 1)
                        {
                            break;
                        }
                        for (int ith = 0; ith < tileh; ith++)
                        {
                            //int iyhm = ih + ith;
                            int iyhm = nHeight - ih - ith - 1;

                            if (iyhm > nHeight - 1 || iyhm < 0)
                            {
                                break;
                            }
                            try
                            {
                                tif.pixels[iyhm, iwhm] = BitConverter.ToSingle(buffer, (itw + tileh * ith) * 4);
                            }
                            catch (Exception e)
                            {
                                throw;
                            }
                        }
                    }
                }
            }
            return tif;
        }




        public static void ReadAndCacheLidarBlock(LidarBlock blk, countryEnum eCountry)
        {
            if (blk == null)
            {
                return;
            }

            int ncols = blk.ncols;
            int nrows = blk.nrows;

            double yllcorner = blk.xllcorner;
            double xllcorner = blk.xllcorner;

            for (int y = 0; y < nrows; y++)
            {
                double dstY = yllcorner + (double)y * blk.cellsize;

                for (int x = 0; x < ncols; x++)
                {
                    double dstX = xllcorner + x * blk.cellsize;

                    if (blk.values[y][x] > -10)
                    {
                        SetCachedValuePointHeight(dstX, dstY, blk.values[y][x], eCountry, (int)(blk.nrows * blk.cellsize));
                    }
                }
            }
        }


        public static void ReadAndCacheTIFBlock(LidarTFW tfw, LidarTIF tif, countryEnum eCountry)
        {
            int ncols = (int)tif.width;
            int nrows = (int)tif.height;

            double yllcorner = tfw.yllcorner;
            double xllcorner = tfw.xllcorner;

            for (int y = 0; y < nrows; y++)
            {
                double dstY = yllcorner + (double)y * tfw.NegY;

                for (int x = 0; x < ncols; x++)
                {
                    double dstX = xllcorner + x * tfw.xScale;

                    if (tif.pixels[y,x] > -10)
                    {
                        SetCachedValuePointHeight(dstX, dstY, tif.pixels[y,x], eCountry, (int)(nrows * tfw.xScale));
                    }
                }
            }
        }



        public static void ReadProcessTIFBlock(LidarTFW tfw, LidarTIF tif, LidarBlock theLidarBlock)
        {
            int ncols = tif.width;
            int nrows = tif.height;

            double yllcorner = tfw.yllcorner;
            double xllcorner = tfw.xllcorner;

            // Work out how far the two areas overlap.
            double dstOffsetX = xllcorner - theLidarBlock.xllcorner;
            double dstOffsetY = yllcorner - theLidarBlock.yllcorner;


            double startX = xllcorner;
            if (startX < theLidarBlock.xllcorner)
            {
                startX = theLidarBlock.xllcorner;
            }

            double endX = xllcorner + ncols * tfw.xScale;
            if (endX > (theLidarBlock.xllcorner + theLidarBlock.ncols * theLidarBlock.cellsize))
            {
                endX = theLidarBlock.xllcorner + theLidarBlock.ncols * theLidarBlock.cellsize;
            }

            int srxIndxStartX = (int)((startX - xllcorner) / tfw.xScale);
            int srxIndxEndX = (int)((endX - xllcorner) / tfw.xScale);

            int srcLenX = srxIndxEndX - srxIndxStartX;
            if (srcLenX < 0)
            {
                Console.WriteLine("ReadProcessTIFBlock srcLenX Less than 0");
                srcLenX = 0;
            }

            for (int y = 0; y < nrows; y++)
            {
                double dstY = yllcorner + (double)y * tfw.NegY;
                int dstIndexY = (int)((dstY - theLidarBlock.yllcorner) / theLidarBlock.cellsize);

                if (dstIndexY >= 0 && dstIndexY < theLidarBlock.nrows)
                {
                    for (int x = 0; x < srcLenX; x++)
                    {
                        double dstX = xllcorner + (double)x * tfw.xScale;

                        if (tif.pixels[y,x] > -10 && tif.pixels[y,x] < 10000) // On the earth all Lidar data above 10000m is wrong and should be ignored...
                        {
                            int dstIndexX = (int)((dstX - theLidarBlock.xllcorner) / theLidarBlock.cellsize);

                            if (dstIndexX >= 0 && dstIndexX < theLidarBlock.values[dstIndexY].Length)
                            {
                                if (theLidarBlock.values[dstIndexY][dstIndexX] < -10)
                                {
                                    theLidarBlock.values[dstIndexY][dstIndexX] = tif.pixels[y, x];
                                    theLidarBlock.unset_points -= 1;
                                }
                            }
                        }
                    }
                }
            }
        }



        public static LidarBlock ReadASC(string fn, Stream stream)
        {
            string aline = "";

            try
            {
                using (TextReader tr = new StreamReader(stream))
                {
                    //Console.WriteLine("Read ASC");
                    bool centreX = false;
                    bool centreY = false;

                    int ncols = 0;
                    aline = tr.ReadLine();
                    if (aline.Contains("ncols"))
                    {
                        ncols = Convert.ToInt32(aline.Replace("ncols", ""));
                    }
                    //Console.WriteLine("Read ASC ncols " + ncols + " : " + aline);

                    int nrows = 0;
                    aline = tr.ReadLine();
                    if (aline.Contains("nrows"))
                    {
                        nrows = Convert.ToInt32(aline.Replace("nrows", ""));
                    }
                    //Console.WriteLine("Read ASC nrows " + nrows + " : " + aline);

                    LidarBlock asc = new LidarBlock(fn, ncols, nrows);

                    aline = tr.ReadLine();
                    if (aline.Contains("xllcorner") || aline.Contains("xllcenter"))
                    {
                        if (aline.Contains("xllcenter"))
                        {
                            centreX = true;
                        }
                        asc.xllcorner = Convert.ToDouble(aline.Replace("xllcorner", "").Replace("xllcenter", ""));
                    }
                    //Console.WriteLine("Read ASC xllcorner " + asc.xllcorner + " : " + aline);


                    aline = tr.ReadLine();
                    if (aline.Contains("yllcorner") || aline.Contains("yllcenter"))
                    {
                        if (aline.Contains("yllcenter"))
                        {
                            centreY = true;
                        }
                        asc.yllcorner = Convert.ToDouble(aline.Replace("yllcorner", "").Replace("yllcenter", ""));
                    }
                    //Console.WriteLine("Read ASC yllcorner " + asc.yllcorner + " : " + aline);

                    aline = tr.ReadLine();
                    if (aline.Contains("cellsize"))
                    {
                        asc.cellsize = Convert.ToSingle(aline.Replace("cellsize", ""));
                        if (!centreX)
                        {
                            asc.xllcorner += asc.cellsize / 2;
                        }
                        if (!centreY)
                        {
                            asc.yllcorner += asc.cellsize / 2;
                        }
                    }
                    //Console.WriteLine("Read ASC cellsize " + asc.cellsize + " : " + aline);

                    aline = tr.ReadLine().ToLower();
                    if (aline.Contains("nodata_value"))
                    {
                        asc.NODATA_value = (int)Convert.ToDouble(aline.Replace("nodata_value", ""));
                        if (asc.NODATA_value != -9999)
                        {
                            Console.WriteLine("ProcessZIPs asc.NODATA_value " + asc.NODATA_value);
                        }
                    }
                    //Console.WriteLine("Read ASC NODATA_value " + asc.NODATA_value + " : " + aline);

                    string[] dataitems = null;
                    int itemIndx = 0;
                    char[] splitstr = { ' ' };

                    // The rows are in reverse order in the ASC file - YUK! Read and correct the order...
                    asc.values = new float[asc.nrows][];
                    for (int y = asc.nrows - 1; y >= 0; y--)
                    {
                        asc.values[y] = new float[asc.ncols];
                        for (int x = 0; x < asc.ncols; x++)
                        {
                            asc.values[y][x] = asc.NODATA_value;
                        }
                    }

                    for (int y = asc.nrows - 1; y >= 0; y--)
                    {
                        aline = tr.ReadLine();
                        if (aline == null)
                        {
                            Console.WriteLine("ReadASC Read to EOF - null returned " + fn + " " + y);
                            break;
                        }
                        aline = aline.Trim();
                        if (aline.Length == 0)
                        {
                            Console.WriteLine("ReadASC Read to EOF - Blank Line " + fn + " " + y);
                            break;
                        }
                        dataitems = aline.Split(splitstr);
                        itemIndx = 0;

                        for (int x = 0; x < asc.ncols && x < dataitems.Length; x++)
                        {
                            try
                            {
                                float val = Convert.ToSingle(dataitems[itemIndx]);
                                if (val > -100 && val < 10000) // On the earth all Lidar data above 10000m is wrong and should be ignored...
                                {
                                    asc.values[y][x] = val;
                                }
                                else
                                {
                                    // Console.WriteLine("ReadASC Bad Data Point " + fn + " " + dataitems[itemIndx] + " " + x + " " + y);
                                }
                                itemIndx++;
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("ReadASC Exception " + fn + " " + ex.Message + " : " + ex.StackTrace);
                                Console.WriteLine("ReadASC Exception : y = " + y + " row = " + (asc.nrows - y - 1) + " x = " + x + " length " + dataitems.Length + " : " + dataitems[itemIndx] + " : " + itemIndx);
                                return null;
                            }
                        }
                        //Console.WriteLine("Read ASC values " + asc.values.Length + " : " + aline);
                    }
                    return asc;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ReadASC Exception " + fn + " " + aline + " " + ex.Message + " : " + ex.StackTrace);
                throw;
            }
        }




        public static List<LidarBlock> ReadASCs(int xllcorner, int yllcorner)
        {
            return null;
        }


        static Dictionary<string, List<string>> myTileFilenames = null;



        public static void IndexSingleLidarFile(countryEnum eCountry, string container_or_tif_filename, ref int ll, RunJobParams rjParams)
        {
            string lowerfilename = container_or_tif_filename.ToLower();

            if (container_or_tif_filename.Contains(".zip"))
            {
                if (lowerfilename.Contains("england") || lowerfilename.Contains("patches"))
                {
                    int zipindx = lowerfilename.IndexOf(".zip");
                    if (lowerfilename.Contains("patches"))
                    {
                        Console.WriteLine("Adding ASC file " + lowerfilename + " zipindx " + zipindx);
                    }

                    if (zipindx > 6)
                    {
                        string tilename = lowerfilename.Substring(zipindx - 6, 4);
                        if (lowerfilename.Contains("patches") || rjParams.LidarDebug)
                        {
                            Console.WriteLine("Adding ASC file " + lowerfilename + " tilename " + tilename);
                        }

                        if (!myTileFilenames.ContainsKey(tilename))
                        {
                            myTileFilenames.Add(tilename, new List<string>());
                        }

                        List<string> tilefiles = myTileFilenames[tilename];
                        if (lowerfilename.ToLower().Contains("national-lidar-programme") || lowerfilename.Contains("patches"))
                        {
                            if (lowerfilename.Contains("patches") || rjParams.LidarDebug)
                            {
                                Console.WriteLine("Insert ASC file " + lowerfilename);
                            }
                            tilefiles.Insert(0, lowerfilename);
                        }
                        else
                        {
                            if (lowerfilename.Contains("patches") || rjParams.LidarDebug)
                            {
                                Console.WriteLine("Appending ASC file " + lowerfilename);
                            }
                            tilefiles.Add(lowerfilename);
                        }

                        if ((ll++) % 20 == 0)
                        {
                            Console.Write(",");
                        }
                        //Console.WriteLine("Cache ZIP Directory Entry " + filename);
                    }
                }

                if (eCountry == countryEnum.ie)
                {
                    // DTM_695_775.zip - tile name is 6977 (hopefully)
                    // P_568818.zip - tile name is 5681 (again hopefully)

                    string tilename = null;

                    int dtmindx = lowerfilename.IndexOf("dtm_");
                    if (dtmindx >= 0)
                    {
                        tilename = lowerfilename.Substring(dtmindx + 4, 2) + lowerfilename.Substring(dtmindx + 8, 2);
                    }
                    if (dtmindx < 0)
                    {
                        dtmindx = lowerfilename.IndexOf("p_");
                        if (dtmindx >= 0)
                        {
                            tilename = lowerfilename.Substring(dtmindx + 2, 2) + lowerfilename.Substring(dtmindx + 5, 2);
                        }
                    }
                    if (dtmindx < 0)
                    {
                        dtmindx = lowerfilename.IndexOf("tii_");
                        if (dtmindx >= 0)
                        {
                            tilename = lowerfilename.Substring(dtmindx + 4, 2) + lowerfilename.Substring(dtmindx + 6, 2);
                        }
                    }
                    if (dtmindx >= 0)
                    {
                        if (!myTileFilenames.ContainsKey(tilename))
                        {
                            myTileFilenames.Add(tilename, new List<string>());
                        }
                        List<string> tilefiles = myTileFilenames[tilename];

                        if (rjParams.LidarDebug)
                        {
                            Console.WriteLine("Add Lidar file " + lowerfilename);
                        }
                        tilefiles.Add(lowerfilename);
                    }
                }


                if (lowerfilename.Contains("wales"))
                {
                    int zipindx = lowerfilename.IndexOf("_dtm.zip");
                    if (zipindx > 4 && (lowerfilename.IndexOf("1m") >= 0 || lowerfilename.IndexOf("2m") >= 0))
                    {
                        if (lowerfilename.Substring(zipindx - 5, 1) == "_")
                        {
                            string tilename = lowerfilename.Substring(zipindx - 4, 4);
                            if (!myTileFilenames.ContainsKey(tilename))
                            {
                                myTileFilenames.Add(tilename, new List<string>());
                            }

                            List<string> tilefiles = myTileFilenames[tilename];
                            if (lowerfilename.IndexOf("1m") > 0)
                            {
                                if (rjParams.LidarDebug)
                                {
                                    Console.WriteLine("Insert Lidar file " + lowerfilename);
                                }
                                // Insert the 1m tiles at the start - these take priority...
                                tilefiles.Insert(0, lowerfilename);
                            }

                            if (lowerfilename.IndexOf("2m") > 0)
                            {
                                if (rjParams.LidarDebug)
                                {
                                    Console.WriteLine("Add Lidar file " + lowerfilename);
                                }
                                // Add the 2m tiles at the end
                                tilefiles.Add(lowerfilename);
                            }
                            //Console.WriteLine("Cache ZIP Directory Entry " + filename);
                            if ((ll++) % 20 == 0)
                            {
                                Console.Write("#");
                            }
                        }
                        else
                        {
                            string bigTilename = lowerfilename.Substring(zipindx - 2, 2);
                            var archive = ZipFile.OpenRead(lowerfilename);
                            {
                                foreach (var entry in archive.Entries)
                                {
                                    string fn = entry.Name.ToLower();

                                    if (fn.EndsWith("dtm_1m.asc"))
                                    {
                                        for (int n = 0; n < 10; n++)
                                        {
                                            for (int e = 0; e < 10; e++)
                                            {
                                                string tilename = bigTilename + e + n;

                                                string tmptile = fn.Substring(0, 3) + fn.Substring(4, 1);
                                                if (tmptile.ToLower() == tilename)
                                                {
                                                    List<string> tilefiles;
                                                    if (!myTileFilenames.ContainsKey(tilename))
                                                    {
                                                        myTileFilenames.Add(tilename, new List<string>());
                                                    }
                                                    tilefiles = myTileFilenames[tilename];
                                                    tilefiles.Add(lowerfilename);

                                                    if ((ll++) % 20 == 0)
                                                    {
                                                        Console.Write("@");
                                                    }

                                                    //Console.WriteLine("Cache ZIP Directory Entry " + lowerfilename);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                // Index Norway TIF DTM files...
                if (eCountry == countryEnum.no && lowerfilename.Contains("norway") && lowerfilename.Contains("_tiff") && lowerfilename.Contains("dtm") && !lowerfilename.Contains("nhs"))
                {
                    string tilename = lowerfilename.Substring(lowerfilename.LastIndexOf("\\") + 11, 4);
                    if (!myTileFilenames.ContainsKey(tilename))
                    {
                        myTileFilenames.Add(tilename, new List<string>());
                    }
                    if (rjParams.LidarDebug)
                    {
                        Console.WriteLine("Add Lidar file " + lowerfilename);
                    }
                    myTileFilenames[tilename].Add(lowerfilename);
                }
            }

            // The large Wales TIF file goes in ahead of anything else...
            if (lowerfilename.Contains("wales_lidar_dtm_1m_32bit_cog.tif"))
            {
                foreach (string mySquare in WalesSquares)
                {
                    for (int x = 0; x < 10; x++)
                    {
                        for (int y = 0; y < 10; y++)
                        {
                            string tilename = mySquare + x.ToString("D1") + y.ToString("D1");
                            if (!myTileFilenames.ContainsKey(tilename))
                            {
                                myTileFilenames.Add(tilename, new List<string>());
                            }
                            List<string> tilefiles = myTileFilenames[tilename];
                            tilefiles.Insert(0, lowerfilename);
                        }
                    }
                }
            }

            // Index Scottish TIF DTM files...
            if (lowerfilename.Contains("scotland") && lowerfilename.Contains(".tif") && lowerfilename.Contains("dtm"))
            {
                if (lowerfilename.Contains("_50cm_") || lowerfilename.Contains("_1m_") || lowerfilename.Contains("_25cm_"))
                {
                    // Tiles can have long or short names - handle this here.
                    string longtilename = lowerfilename.Substring(lowerfilename.LastIndexOf("\\") + 1, 6);
                    string tilename = lowerfilename.Substring(lowerfilename.LastIndexOf("\\") + 1, 4);
                    if (longtilename.IndexOf('_') <= 0 && longtilename.IndexOf('n') <= 0 && longtilename.IndexOf('e') <= 0 && longtilename.IndexOf('s') <= 0 && longtilename.IndexOf('w') <= 0)
                    {
                        tilename = longtilename.Substring(0, 3) + longtilename.Substring(4, 1);
                    }
                    if (!myTileFilenames.ContainsKey(tilename))
                    {
                        myTileFilenames.Add(tilename, new List<string>());
                    }
                    if (rjParams.LidarDebug)
                    {
                        Console.WriteLine("Add Lidar file " + lowerfilename);
                    }
                    myTileFilenames[tilename].Add(lowerfilename);
                }
            }


        }

        public static void IndexLidarFiles(countryEnum eCountry, RunJobParams rjParams)
        {
            if (myTileFilenames != null) return;

            Console.WriteLine("IndexLidarFiles Index LIDAR Files " + eCountry);
            int ll = 0;

            myTileFilenames = new Dictionary<string, List<string>>();

            try
            {
                string[] folders = new string[4];
                if (eCountry == countryEnum.uk)
                {
                    folders[0] = GetFolder("england");
                    folders[1] = GetFolder("wales");
                    folders[2] = GetFolder("scotland");
                    folders[3] = GetFolder("patches");
                }
                else if (eCountry == countryEnum.ie)
                {
                    folders[0] = GetFolder("ie");
                    folders[1] = GetFolder("iepatches");
                    folders[2] = null;
                    folders[3] = null;
                }
                else if (eCountry == countryEnum.no)
                {
                    folders[0] = GetFolder("norway");
                    folders[1] = GetFolder("nopatches");
                    folders[2] = null;
                    folders[3] = null;
                }
                else
                {
                    return;
                }
                string filesearcher = "*.*";

                foreach (var gridfolder in folders)
                {
                    if (gridfolder == null) continue;

                    Console.WriteLine("Index ZIP Folders " + gridfolder);

                    var filenames = Directory.EnumerateFiles(gridfolder, filesearcher);
                    foreach (string filename in filenames)
                    {
                        IndexSingleLidarFile(eCountry, filename, ref ll, rjParams);
                    }
                }
            }
            catch (Exception ex)
            {
                string sghsgsg = ex.Message;
                Console.WriteLine("IndexLidarFiles Exception " + ex.Message + " : " + ex.StackTrace);
            }

            Console.WriteLine("IndexLidarFiles Finished " + myTileFilenames.Values.Count + " Entries");
        }


        public static void Pad2mData(LidarBlock dstBlock)
        {
            if (dstBlock == null) return;

            Console.WriteLine("Pad2mData Called " + dstBlock.filename);
            int totalPadded = 0;

            for (int y = 0; y < dstBlock.nrows - 1; y += 2)
            {
                for (int x = 0; x < dstBlock.ncols - 1; x += 2)
                {
                    // Is this value set or not?..
                    if (dstBlock.values[y][x] != dstBlock.NODATA_value)
                    {
                        for (int i = 1; i < 4; i++)
                        {
                            int ix = i & 1;
                            int iy = (i & 2) / 2;

                            double total = dstBlock.values[y][x];
                            int count = 1;
                            bool setvalue = false;

                            // Horizontal, Vertical and then Diagonal
                            if (dstBlock.values[y + iy][x + ix] == dstBlock.NODATA_value)
                            {
                                if ((y + 2 * iy) < dstBlock.nrows && (x + 2 * ix) < dstBlock.ncols)
                                {
                                    if (dstBlock.values[y + 2 * iy][x + 2 * ix] != dstBlock.NODATA_value)
                                    {
                                        total += dstBlock.values[y + 2 * iy][x + 2 * ix];
                                        count += 1;
                                        setvalue = true;
                                    }
                                }
                                else
                                {
                                    setvalue = true;
                                }

                                if (setvalue)
                                {
                                    dstBlock.values[y + iy][x + ix] = (float)(total / count);
                                    totalPadded++;
                                    dstBlock.unset_points -= 1;
                                    dstBlock.changed = true;
                                }
                            }
                        }
                    }
                }
            }
            Console.WriteLine("Pad2mData Finished - Padded " + totalPadded + " Points, Unset Remaining " + dstBlock.unset_points);
        }



        public static int ProcessLidarBlock(LidarBlock dstBlock, LidarBlock srcBlock)
        {
            if (srcBlock == null)
            {
                return dstBlock.unset_points;
            }

            for (int y = 0; y < srcBlock.nrows; y++)
            {
                for (int x = 0; x < srcBlock.ncols; x++)
                {
                    if (srcBlock.values[y][x] != srcBlock.NODATA_value)
                    {
                        double absx = (srcBlock.xllcorner + srcBlock.cellsize * x);
                        double absy = (srcBlock.yllcorner + srcBlock.cellsize * y);

                        int compx = (int)((absx - dstBlock.xllcorner) / dstBlock.cellsize);
                        int compy = (int)((absy - dstBlock.yllcorner) / dstBlock.cellsize);

                        if (compx >= 0 && compx < dstBlock.ncols && compy >= 0 && compy < dstBlock.nrows)
                        {
                            if (dstBlock.values[compy][compx] == srcBlock.NODATA_value)
                            {
                                if (srcBlock.values[y][x] >= -3 && srcBlock.values[y][x] < 10000)
                                {
                                    dstBlock.values[compy][compx] = srcBlock.values[y][x];
                                    dstBlock.unset_points -= 1;
                                }
                            }
                        }
                    }
                }
            }

            return dstBlock.unset_points;
        }



        public static LidarBlock NewReadZIPs(string binASC, int xllcorner, int yllcorner, countryEnum eCountry, bool CreateAllLIDARCaches, RunJobParams rjParams)
        {
            int siz = GetLidarSiz();
            double res = GetLidarRes();
            //bool twoMDataUsed = true;

            LidarBlock theLidarBlock = null;

            if (!CreateAllLIDARCaches)
            {
                theLidarBlock = CreateAndAdd(binASC, xllcorner, yllcorner, siz, res);
            }
            else
            {
                theLidarBlock = new LidarBlock(binASC);
                theLidarBlock.unset_points = 0x7fffffff;
            }

            bool ok = false;

            int prevunsetpts = theLidarBlock.unset_points;

            string gridName = GetGridName(xllcorner, yllcorner, siz, eCountry).ToLower();

            if (myTileFilenames == null)
            {
                IndexLidarFiles(eCountry, rjParams);
            }

            foreach (string tilname in myTileFilenames.Keys)
            {
                if (gridName == tilname || CreateAllLIDARCaches)
                {
                    List<string> tilefiles = myTileFilenames[tilname];

                    for (int metres = 1; metres <= 2; metres++)
                    {
                        foreach (string filename in tilefiles)
                        {
                            // Process the 1m data first and then the 2m...
                            if (filename.ToLower().Contains("-2m") && metres != 2)
                            {
                                continue;
                            }
                            if (!filename.ToLower().Contains("-2m") && metres != 1)
                            {
                                continue;
                            }

                            Console.WriteLine("Open ZIP File " + filename);
                            LidarTFW tfw = null;
                            LidarTIF tif = null;

                            try
                            {
                                if (filename.ToLower().EndsWith(".zip"))
                                {
                                    FileStream zipToOpen = new FileStream(filename, FileMode.Open, FileAccess.Read);
                                    var archive = new ZipArchive(zipToOpen, ZipArchiveMode.Read);
                                    {
                                        foreach (var entry in archive.Entries)
                                        {
                                            string fn = entry.Name.ToLower();

                                            if (filename.ToLower().Contains("patches"))
                                            {
                                                Console.WriteLine("DTM/DSM File ZIP " + entry.Name + " in ZIP " + filename + " fn " + fn);
                                            }

                                            // England format file names - tfw, tif and asc files...
                                            bool match = true;

                                            if (filename.ToLower().Contains("wales"))
                                            {
                                                // st0067_dtm_1m.asc - Wales format filenames - not all files match
                                                string tmptile = fn.Substring(0, 3) + fn.Substring(4, 1);
                                                if (tmptile.ToLower() != gridName.ToLower())
                                                {
                                                    match = false;
                                                }
                                            }

                                            if (match)
                                            {
                                                if (fn.EndsWith(".asc"))
                                                {
                                                    if (filename.ToLower().Contains("patches"))
                                                    {
                                                        Console.WriteLine("DTM/DSM File ZIP " + entry.Name + " in ZIP " + filename);
                                                    }
                                                    //Console.WriteLine("DTM/DSM File ZIP " + entry.Name + " in ZIP " + filename);
                                                    Console.Write(".");

                                                    using (Stream stream = entry.Open())
                                                    {
                                                        prevunsetpts = theLidarBlock.unset_points;
                                                        LidarBlock blk = ReadASC(fn, stream);
                                                        if (blk.cellsize > 1)
                                                        {
                                                            //twoMDataUsed = true;
                                                        }


                                                        if (!CreateAllLIDARCaches)
                                                        {
                                                            var unsettpts = theLidarBlock.unset_points;
                                                            ProcessLidarBlock(theLidarBlock, blk);
                                                            if (filename.ToLower().Contains("patches"))
                                                            {
                                                                Console.WriteLine("Patch ProcessLidarBlock Added " + (unsettpts - theLidarBlock.unset_points));
                                                            }
                                                        }
                                                        else
                                                        {
                                                            ReadAndCacheLidarBlock(blk, eCountry);
                                                        }


                                                        if (prevunsetpts != theLidarBlock.unset_points)
                                                        {
                                                            ok = true;
                                                        }

                                                        if (theLidarBlock.unset_points == 0 && !CreateAllLIDARCaches)
                                                        {
                                                            archive.Dispose();

                                                            // Nothing left to do as all points are set - return.
                                                            return theLidarBlock;
                                                        }
                                                    }
                                                }
                                            }

                                            if (fn.EndsWith(".tfw"))
                                            {
                                                Console.WriteLine("TFW File " + entry.Name + " in ZIP " + filename);

                                                using (Stream stream = entry.Open())
                                                {
                                                    tfw = ReadTFW(fn, stream);
                                                }
                                            }


                                            if (fn.EndsWith(".tif"))
                                            {
                                                if (!entry.FullName.ToLower().Contains("dsm"))
                                                {
                                                    Console.WriteLine("TIF File " + entry.FullName + " in ZIP " + filename);

                                                    Stream stream = entry.Open();
                                                    //tif = ReadTIF(fn, stream, false);
                                                    tif = ReadTIFBounded(fn, stream, false, xllcorner,yllcorner,siz);
                                                    if (tif.tfw != null)
                                                    {
                                                        tfw = tif.tfw;
                                                    }
                                                }
                                            }

                                            if (tfw != null && tif != null)
                                            {
                                                prevunsetpts = theLidarBlock.unset_points;
                                                if (tfw.xScale > 1.1)
                                                {
                                                    //twoMDataUsed = true;
                                                }

                                                if (!CreateAllLIDARCaches)
                                                {
                                                    ReadProcessTIFBlock(tfw, tif, theLidarBlock);
                                                }
                                                else
                                                {
                                                    ReadAndCacheTIFBlock(tfw, tif, eCountry);
                                                }

                                                tfw = null;
                                                tif = null;

                                                if (prevunsetpts != theLidarBlock.unset_points)
                                                {
                                                    ok = true;
                                                }
                                                // Force a GC run now... not ideal - For some reason we get out of memory errors...
                                                GeneralUtilClasses.RunGC();
                                                if (theLidarBlock.unset_points == 0 && !CreateAllLIDARCaches)
                                                {
                                                    archive.Dispose();

                                                    // Nothing left to do as all points are set - return.
                                                    return theLidarBlock;
                                                }
                                            }
                                        }

                                        archive.Dispose();
                                    }
                                }
                                else if (filename.ToLower().EndsWith(".tif"))
                                {
                                    Console.WriteLine("TIF File " + filename + " NO ZIP");

                                    FileStream stream = new FileStream(filename, FileMode.Open, FileAccess.Read);
                                    //tif = ReadTIF(filename, (Stream)stream, true);
                                    tif = ReadTIFBounded(filename, (Stream) stream, true, xllcorner, yllcorner, siz);
                                    if (tif != null && tif.tfw != null)
                                    {
                                        tfw = tif.tfw;

                                        prevunsetpts = theLidarBlock.unset_points;
                                        if (tfw.xScale > 1.1)
                                        {
                                            //twoMDataUsed = true;
                                        }

                                        if (!CreateAllLIDARCaches)
                                        {
                                            ReadProcessTIFBlock(tfw, tif, theLidarBlock);
                                        }
                                        else
                                        {
                                            ReadAndCacheTIFBlock(tfw, tif, eCountry);
                                        }

                                        tfw = null;
                                        tif = null;

                                        if (prevunsetpts != theLidarBlock.unset_points)
                                        {
                                            ok = true;
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                string sghsgsg = ex.Message;
                                Console.WriteLine("ProcessZIPs Exception " + filename + " " + ex.Message + " : " + ex.StackTrace);
                            }
                        }
                    }
                }
            }

            // We have not processed anything...
            if (!ok)
            {
                Console.WriteLine("NewReadZIPs Failed to do anything " + binASC + ", " + xllcorner + ", " + yllcorner + ", " + gridName);
                if (myTileFilenames.ContainsKey(gridName))
                {
                    List<string> tilefiles = myTileFilenames[gridName];

                    foreach (string filename in tilefiles)
                    {
                        Console.WriteLine("ZIP File " + filename);
                    }
                }
                return null;
            }


            //if (twoMDataUsed)
            //{
            Pad2mData(theLidarBlock);
            //}

            return theLidarBlock;
        }



        public static bool Check(bool stepped, double xll, double yll, bool infillandcache, countryEnum eCountry, RunJobParams rjParams)
        {
            // Get the LIDAR for the location...
            //LidarBlock asc = GetASC(stepped, xll, yll, infillandcache, eCountry);
            LidarBlock asc = ProcessZIPs(stepped, (int)xll, (int)yll, infillandcache, eCountry, rjParams);
            return (asc != null);
        }


        public static double GetHeight(bool stepped, double xll, double yll, bool infillandcache, countryEnum eCountry, RunJobParams rjParams)
        {
            double xll2 = xll;
            double yll2 = yll;

            // Get the LIDAR for the location...
            LidarBlock asc = ProcessZIPs(stepped, (int)xll, (int)yll, infillandcache, eCountry, rjParams);
            if (asc == null)
            {
                return LidarBlock.NODATA_const;
            }

            int ixll = (int)((xll - asc.xllcorner) / asc.cellsize);
            int iyll = (int)((yll - asc.yllcorner) / asc.cellsize);

            if (ixll >= 0 && ixll < asc.ncols && iyll >= 0 && iyll < asc.nrows)
            {
                // Get the height for the lat lon... 
                return asc.values[iyll][ixll];
            }

            return LidarBlock.NODATA_const;
        }


        const int CacheMagic = 0x5643faea;

        public static bool SaveFileCache(string filename, LidarBlock asc, int total_calculated)
        {
            try
            {
                using (BinaryWriter writer = new BinaryWriter(File.Open(filename, FileMode.Create)))
                {
                    {
                        writer.Write(CacheMagic);
                        writer.Write(total_calculated);
                        writer.Write(asc.ncols);
                        writer.Write(asc.nrows);
                        writer.Write(asc.xllcorner);
                        writer.Write(asc.yllcorner);
                        writer.Write(asc.cellsize);
                        writer.Write(asc.NODATA_value);

                        for (int y = 0; y < asc.values.Length; y++)
                        {
                            for (int x = 0; x < asc.values[y].Length; x++)
                            {
                                writer.Write(asc.values[y][x]);
                            }
                        }
                    }
                    writer.Close();
                    Console.WriteLine("SaveFileCache Filename " + filename);
                    asc.changed = false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("SaveFileCache Exception caught " + filename + " : " + ex.Message + " " + ex.StackTrace + " " + ex.InnerException);
                return false;
            }
            return true;
        }



        public static LidarBlock ReadFileCache(string filname)
        {
            //public int ncols; // 1000
            //public int nrows; // 1000
            //public int xllcorner; //    375000
            //public int yllcorner; //    305000
            //public float cellsize; //     1
            //public int NODATA_value; // -9999
            //public float[][] values;

            try
            {
                if (File.Exists(filname))
                {
                    using (BinaryReader reader = new BinaryReader(File.Open(filname, FileMode.Open, FileAccess.Read, FileShare.Read)))
                    {
                        int magic;
                        LidarBlock asc = new LidarBlock(filname);
                        magic = reader.ReadInt32();
                        if (magic != CacheMagic)
                        {
                            return null;
                        }
                        int total_calculated = reader.ReadInt32();
                        asc.ncols = reader.ReadInt32();
                        asc.nrows = reader.ReadInt32();
                        asc.xllcorner = reader.ReadDouble();
                        asc.yllcorner = reader.ReadDouble();
                        asc.cellsize = reader.ReadDouble();
                        asc.NODATA_value = reader.ReadInt32();

                        asc.values = new float[asc.nrows][];
                        for (int y = 0; y < asc.values.Length; y++)
                        {
                            asc.values[y] = new float[asc.ncols];
                            for (int x = 0; x < asc.values[y].Length; x++)
                            {
                                asc.values[y][x] = reader.ReadSingle();
                            }
                        }

                        reader.Close();

                        CacheLidarFileAdd(asc);

                        Console.WriteLine("ReadFileCache Filename " + filname + " : Quality " + (double)100.0 * (1.0 - ((double)total_calculated / asc.nrows / asc.ncols)) + "%");

                        return asc;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ReadFileCache Exception caught " + filname + " : " + ex.Message + " " + ex.StackTrace + " " + ex.InnerException);
            }

            return null;
        }




        //public static void FillInLidarWithOSData(bool stepped, LidarBlock ascComposite, countryEnum eCountry, RunJobParams rjParams)
        //{
        //    if (ascComposite == null)
        //    {
        //        return;
        //    }

        //    if (eCountry != countryEnum.uk)
        //    {
        //        return;
        //    }

        //    // Now fill it in the compostite tile from the SRTM Data
        //    for (int compy = 0; compy < ascComposite.nrows; compy++)
        //    {
        //        for (int compx = 0; compx < ascComposite.ncols; compx++)
        //        {
        //            if (ascComposite.values[compy][compx] == ascComposite.NODATA_value)
        //            {
        //                double n = ascComposite.yllcorner + ascComposite.cellsize * compy;
        //                double e = ascComposite.xllcorner + ascComposite.cellsize * compx;

        //                double tmpmindist = 99999999;
        //                double height = ZipMapDataHandler.GetHeightFromOSVectorData(ll.lat, ll.lon, eCountry, true, ref tmpmindist);

        //                // If the OS data nearest point is greater than 30m away then use SRTM data.
        //                if (tmpmindist < 30)
        //                {
        //                    ascComposite.values[compy][compx] = (float)height;
        //                    ascComposite.changed = true;
        //                }
        //            }
        //        }
        //    }
        //}



        public static void FillInLidarWithSRTM(bool stepped, LidarBlock ascComposite, countryEnum eCountry, RunJobParams rjParams)
        {
            if (ascComposite == null) return;

            // Now fill it in the compostite tile from the SRTM Data
            for (int compy = 0; compy < ascComposite.nrows; compy++)
            {
                for (int compx = 0; compx < ascComposite.ncols; compx++)
                {
                    if (ascComposite.values[compy][compx] == ascComposite.NODATA_value)
                    {
                        double n = ascComposite.yllcorner + ascComposite.cellsize * compy;
                        double e = ascComposite.xllcorner + ascComposite.cellsize * compx;

                        OsGridRef osgr = new OsGridRef(e, n);
                        var ll = osgr.toLatLon(eCountry);

                        double height = 0;
                        double tmpmindist = 99999999;
                        //if (eCountry == countryEnum.uk)
                        //{
                        //    height = ZipMapDataHandler.GetHeightFromOSVectorData(ll.lat, ll.lon, eCountry, true, ref tmpmindist);
                        //}

                        // If the OS data nearest point is greater than 30m away then use SRTM data.
                        if (tmpmindist > 30)
                        {
                            // Go get the height from somewhere for this location... 
                            height = ZipMapDataHandler.GetHeightAtLatLon(false, stepped, ll.lat, ll.lon, eCountry, rjParams, false);
                        }

                        ascComposite.values[compy][compx] = (float)height;
                        ascComposite.changed = true;
                    }
                }
            }
        }




        public static void NewFillInLidarWithSRTM(bool stepped, LidarBlock ascComposite, countryEnum eCountry, RunJobParams rjParams)
        {
            if (ascComposite == null) return;

            // Now fill it in the compostite tile from the SRTM Data
            for (int compy = 0; compy < ascComposite.nrows; compy++)
            {
                for (int compx = 0; compx < ascComposite.ncols; compx++)
                {
                    if (ascComposite.values[compy][compx] == ascComposite.NODATA_value)
                    {
                        double n = ascComposite.yllcorner + ascComposite.cellsize * compy;
                        double e = ascComposite.xllcorner + ascComposite.cellsize * compx;

                        ProjCoordinate ne = new ProjCoordinate(e, n);
                        ProjCoordinate ll = new ProjCoordinate();
                        rjParams.ToLLTrans.Transform(ne, ll);


                        //OsGridRef osgr = new OsGridRef(e, n);
                        //var ll = osgr.toLatLon(eCountry);

                        double height = 0;
                        double tmpmindist = 99999999;
                        //if (eCountry == countryEnum.uk)
                        //{
                        //    height = ZipMapDataHandler.GetHeightFromOSVectorData(ll.lat, ll.lon, eCountry, true, ref tmpmindist);
                        //}

                        // If the OS data nearest point is greater than 30m away then use SRTM data.
                        if (tmpmindist > 30)
                        {
                            // Go get the height from somewhere for this location... 
                            height = ZipMapDataHandler.GetHeightAtLatLon(false, stepped, ll.Y, ll.X, eCountry, rjParams, false);
                        }

                        ascComposite.values[compy][compx] = (float)height;
                        ascComposite.changed = true;
                    }
                }
            }
        }


        public static void ProcessZIPsToLidarCache(countryEnum eCountry, RunJobParams rjParams)
        {
            NewReadZIPs("Undefined ProcessZIPsToLidarCache", 0, 0, eCountry, true, rjParams);

            CacheLidarFileEvictAll();

            foreach (string fn in allLIDARCacheFiles)
            {
                LidarBlock asc = ReadFileCache(fn);

                Pad2mData(asc);
                NewFillInLidarWithSRTM(false, asc, eCountry, rjParams);
            }
            CacheLidarFileEvictAll();
        }



        public static LidarBlock ProcessZIPs(bool stepped, int xllcorner, int yllcorner, bool infillandcache, countryEnum eCountry, RunJobParams rjParams)
        {
            LidarBlock ascComposite = GetCachedASC(xllcorner, yllcorner, (int)LidarBlockReader.GetLidarSiz());
            if (ascComposite != null)
            {
                return ascComposite;
            };

            DateTime startLIDAR = DateTime.Now;

            string gridname = GetGridName(xllcorner, yllcorner, GetLidarSiz(), eCountry);

            // Is there a cached copy of the data we can use?
            string binASC = GetLidarCacheFilename(gridname, eCountry.ToString());

            if (infillandcache)
            {
                ascComposite = ReadFileCache(binASC);
                //ascComposite = ReadOrAdd(binASC, xllcorner, yllcorner, GetLidarSiz(), GetLidarRes());
                //ascComposite = ReadFileCache(binASC);
                if (ascComposite != null)
                {
                    return ascComposite;
                }
            }

            Console.WriteLine("ProcessZIPs Cache Filename " + binASC);

            ascComposite = NewReadZIPs(binASC, xllcorner, yllcorner, eCountry, false, rjParams);
            if (ascComposite == null)
            {
                return null;
            }

            double quality = 100 * (1 - (double)ascComposite.unset_points / ascComposite.nrows / ascComposite.ncols);

            if (infillandcache)
            {
                NewFillInLidarWithSRTM(stepped, ascComposite, eCountry, rjParams);
            }
            DateTime endLIDAR = DateTime.Now;

            Console.WriteLine("ProcessZIPs Quality " + quality + "%, Processing Time " + (endLIDAR - startLIDAR));

            //If the total number of missing points is 10% or more or it took > 2min to process, save the ASC block for later use - the cache file size is huge so best not to keep if possible.
            if (quality < 90 || (endLIDAR - startLIDAR) > TimeSpan.FromMinutes(2))
            {
                if (infillandcache)
                {
                    // Cache the data for later reuse... this should speed up things somewhat.
                    Console.WriteLine("ProcessZIPs Save Cache Entry Filename " + binASC);
                    SaveFileCache(binASC, ascComposite, ascComposite.unset_points);
                }
            }

            return ascComposite;
        }
    }
}
