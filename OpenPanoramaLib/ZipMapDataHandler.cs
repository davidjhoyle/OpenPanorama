using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;

//using Newtonsoft.Json.Converters;
//using Newtonsoft.Json.Utilities;
using Proj4Net.Core;
using Newtonsoft.Json;


namespace OpenPanoramaLib
{
    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -    */
    /* ZIP Map Data Handling Classes (c) David Hoyle 2020 : MIT Licensed                                            */
    /*  XDocCacheLine class is to cache OS Map XML data                                                             */
    /*  TileCacheLine class is to cache SRTM tiles to speed access                                                  */
    /*  ZipMapDataHandler class is to handle OS and SRTM ZIP file data                                              */
    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -    */
    public class XDocCacheLine
    {
        public XDocument xdoc;
        public ulong lastAccess;
    }



    public class ZipMapDataHandler
    {
        static string foldername = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/OS Terrain 50/terr50_cgml_gb/data/";
        const string extension = "_OST50CONT_20180509.zip";
        static string SRTMFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/SRTM Data";

        static Dictionary<string, DateTime> failedtileCacheLines = new Dictionary<string, DateTime>();

        static Dictionary<string, XDocCacheLine> xdocCacheLines = new Dictionary<string, XDocCacheLine>();
        static Dictionary<string, SRTMTileCacheLine> tileCacheLines = new Dictionary<string, SRTMTileCacheLine>();
        static int OSDataCacheMaxLines = 500;
        static List<string> BadOSGrids = new List<string>();

        static Dictionary<string, List<SRTMDataPatch>> srtmDataPatches = new Dictionary<string, List<SRTMDataPatch>>();
        static public ulong lastAccessTicker = 1;


        public ZipMapDataHandler()
        {
        }

        public static void SelectFolder(string fn, bool os)
        {
            if (os)
            {
                foldername = fn;
            }
            else
            {
                SRTMFolder = fn;
            }
        }


        public static string GetFolder(bool os)
        {
            if (os)
            {
                return foldername;
            }
            else
            {
                return SRTMFolder;
            }
        }



        public static void SetOSMapCacheSize(int siz)
        {
            OSDataCacheMaxLines = siz;
        }

        public static string GetSRTMFilePrefixNameForLatLon(int latInt, int lonInt)
        {
            string heightFilename = "";
            // N49E000.hgt
            heightFilename = "N" + latInt + "E" + lonInt.ToString("000");
            if (lonInt < 0)
            {
                heightFilename = "N" + latInt + "W" + (-lonInt).ToString("000");
            }
            string filesearcher = heightFilename;

            return filesearcher;
        }


        public const int gridCount = 3600;
        public const int gridSize = gridCount + 1;



        public static void getSRTMDataPatches(string filename)
        {
            using (StreamReader r = new StreamReader(GetFolder(false) + "/" + filename))
            {
                string json = r.ReadToEnd();
                srtmDataPatches = JsonConvert.DeserializeObject<Dictionary<string, List<SRTMDataPatch>>>(json);
            }
        }


        public static void saveSRTMDataPatches(string filename)
        {
            srtmDataPatches = new Dictionary<string, List<SRTMDataPatch>>();

            srtmDataPatches["Tilename1"] = new List<SRTMDataPatch>();
            srtmDataPatches["Tilename1"].Add(new SRTMDataPatch(1, 2, 3));
            srtmDataPatches["Tilename1"].Add(new SRTMDataPatch(12, 22, 32));
            srtmDataPatches["Tilename1"].Add(new SRTMDataPatch(12, 22, 32));
            srtmDataPatches["Tilename2"] = new List<SRTMDataPatch>();
            srtmDataPatches["Tilename2"].Add(new SRTMDataPatch(4, 5, 6));
            srtmDataPatches["Tilename3"] = new List<SRTMDataPatch>();
            srtmDataPatches["Tilename3"].Add(new SRTMDataPatch(28388, 333, 55));
            srtmDataPatches["EmptyTilename4"] = new List<SRTMDataPatch>();

            // serialize JSON directly to a file
            using (StreamWriter file = File.CreateText(GetFolder(false) + "/" + filename))
            {
                Console.WriteLine("saveSRTMDataPatches JSON File " + filename);
                Newtonsoft.Json.JsonSerializer serializer = new Newtonsoft.Json.JsonSerializer();
                serializer.Formatting = Newtonsoft.Json.Formatting.Indented;
                serializer.Serialize(file, srtmDataPatches);
            }
        }


        public static Int16[][] GetSRTMData(int latInt, int lonInt, int laty = -1, int longy = -1)
        {
            string fileprefix = GetSRTMFilePrefixNameForLatLon(latInt, lonInt);
            string filesearcher = fileprefix + "*.zip";
            Int16[][] data = null;
            string gridfolder = SRTMFolder;
            int byteOffset = -1;
            byte[] tmpdata = null;


            if (failedtileCacheLines.ContainsKey(filesearcher))
            {
                // If the file has not been found in the last 10 mins, return fail...
                if (failedtileCacheLines[filesearcher] > DateTime.Now - TimeSpan.FromMinutes(10))
                {
                    return null;
                }
                else
                {
                    // If the failure is old, evict the cache line and continue...
                    failedtileCacheLines.Remove(filesearcher);
                }

                return null;
            }

            if (tileCacheLines.ContainsKey(fileprefix) && laty < 0 && longy < 0)
            {
                tileCacheLines[fileprefix].lastAccess = lastAccessTicker++;
                return tileCacheLines[fileprefix].tile;
            }
            else
            {
                Console.WriteLine("Cache Miss File " + fileprefix + " Cache size " + tileCacheLines.Count);
            }


            if (laty >= 0 && longy >= 0)
            {
                byteOffset = ((gridSize - laty) * gridSize + longy) * 2;
            }

            try
            {
                var filenames = Directory.EnumerateFiles(gridfolder, filesearcher);

                foreach (string filename in filenames)
                {
                    var archive = ZipFile.OpenRead(filename);
                    {
                        foreach (var entry in archive.Entries)
                        {
                            if (entry.Name.EndsWith(".hgt"))
                            {
                                if (byteOffset >= 0)
                                {
                                    // Now check the patches the loaded tile.
                                    if (srtmDataPatches.ContainsKey(fileprefix))
                                    {
                                        foreach (var patch in srtmDataPatches[fileprefix])
                                        {
                                            if (patch.latpart == laty && patch.lonpart == longy )
                                            {
                                                Console.WriteLine("Patched Data " + fileprefix + " Lat " + laty + " Long " + longy + " Height " + patch.z);

                                                data = new Int16[1][];
                                                data[0] = new Int16[1];
                                                data[0][0] = (short)patch.z;
                                                return data;
                                            }
                                        }
                                    }
                                }

                                Console.WriteLine("Read Height File " + entry.Name);
                                using (Stream stream = entry.Open())
                                {
                                    if (byteOffset >= 0)
                                    {
                                        tmpdata = new byte[byteOffset];
                                        // Grab some memory for the data.
                                        data = new Int16[1][];
                                        data[0] = new Int16[1];
                                        data[0][0] = 0;
                                        stream.Read(tmpdata, 0, byteOffset);
                                        //stream.Seek(byteOffset, SeekOrigin.Begin);
                                        int bite1 = stream.ReadByte();
                                        int bite2 = stream.ReadByte();
                                        if (bite1 >= 0 && bite2 >= 0)
                                        {
                                            data[0][0] = (Int16)(bite1 * 256 + bite2);
                                            if (data[0][0] < -1000)
                                            {
                                                data[0][0] = 0;
                                            }
                                        }
                                        return data;
                                    }
                                    else
                                    {
                                        // Grab some memory for the data.
                                        data = new Int16[gridSize][];
                                        for (int i = 0; i < gridSize; i++)
                                        {
                                            data[i] = new Int16[gridSize];
                                        }
                                    }

                                    tmpdata = new byte[gridSize * gridSize * 2];

                                    int totbytes = tmpdata.Length;
                                    int myoffset = 0;
                                    while (totbytes > 0)
                                    {
                                        int bitesred = stream.Read(tmpdata, myoffset, totbytes);
                                        totbytes -= bitesred;
                                        myoffset += bitesred;
                                    }
                                    for (int lat = 0; lat < gridSize; lat++)
                                    {
                                        for (int lon = 0; lon < gridSize; lon++)
                                        {
                                            int bite1 = tmpdata[(lat * gridSize + lon) * 2];
                                            int bite2 = tmpdata[(lat * gridSize + lon) * 2 + 1];
                                            if (bite1 >= 0 && bite2 >= 0)
                                            {
                                                int tmpLat = gridSize - lat - 1;
                                                data[tmpLat][lon] = (Int16)(bite1 * 256 + bite2);

                                                if (data[tmpLat][lon] < -1000)
                                                {
                                                    data[tmpLat][lon] = 0;
                                                }
                                            }
                                        }
                                    }


                                    // Now patch the loaded tile.
                                    if (srtmDataPatches.ContainsKey(fileprefix))
                                    {
                                        foreach (var patch in srtmDataPatches[fileprefix])
                                        {
                                            if (patch.latpart < gridSize && patch.lonpart < gridSize && patch.latpart >= 0 && patch.lonpart >= 0)
                                            {
                                                data[patch.latpart][patch.lonpart] = (short)patch.z;
                                            }
                                        }
                                    }

                                    lock (tileCacheLines)
                                    {
                                        // Sometimes due to a race condition, this happens.
                                        if (!tileCacheLines.ContainsKey(fileprefix))
                                        {
                                            if (tileCacheLines.Count >= SRTMClass.SRTMDataCacheMaxLines)
                                            {
                                                ulong oldestTime = 0xffffffff;
                                                string oldest = null;
                                                foreach (var clin in tileCacheLines)
                                                {
                                                    if (clin.Value.lastAccess < oldestTime)
                                                    {
                                                        oldest = clin.Key;
                                                        oldestTime = clin.Value.lastAccess;
                                                    }
                                                }
                                                if (oldest != null)
                                                {
                                                    tileCacheLines.Remove(oldest);
                                                }
                                            }

                                            SRTMTileCacheLine nwlin = new SRTMTileCacheLine();
                                            nwlin.tile = data;
                                            nwlin.lastAccess = lastAccessTicker++;
                                            tileCacheLines.Add(fileprefix, nwlin);
                                            Console.WriteLine("Cache Filled File " + fileprefix + " Size " + tileCacheLines.Count);
                                        }
                                        else
                                        {
                                            Console.WriteLine("Cache Filled Race Condition - ALL OK " + fileprefix + " Size " + tileCacheLines.Count);
                                        }
                                    }

                                    return data;
                                }
                            }
                            else
                            {
                                Console.WriteLine("File Entry for " + filename + " contains non height file " + entry.Name);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string sghsgsg = ex.Message;
                Console.WriteLine("Exception on Height ZIP File Found - ignoring " + ex.Message + " Trace " + ex.StackTrace);
            }

            // If we get here, there is no NGT data.
            failedtileCacheLines[filesearcher] = DateTime.Now;
            Console.WriteLine("Failed File Access Cache Entry Added " + filesearcher);

            return null;
        }



        public static void SRTMFlushCache()
        {
            tileCacheLines.Clear();
            GeneralUtilClasses.RunGC();
        }

        public static void OSFlushCache()
        {
            xdocCacheLines.Clear();
            GeneralUtilClasses.RunGC();
        }

        //using (FileStream zipToOpen = new FileStream(@"c:\users\exampleuser\release.zip", FileMode.Open))

        public XDocument NewGetOSVectorData(string GridRef)
        {
            char[] splitter = { ' ' };
            // "SH 69880 00431"
            string[] gridStrs = GridRef.Split(splitter);

            string gridfolder = foldername + "/" + gridStrs[0];
            string filesearcher = gridStrs[0] + gridStrs[1][0] + gridStrs[2][0] + "_OST50CONT_*.zip";
            // C:\Users\David\Desktop\OS Terrain 50\terr50_cgml_gb\data\sn\sn34_OST50CONT_*.zip
            XDocument doc = null;


            if (xdocCacheLines.ContainsKey(filesearcher))
            {
                // Cache hit - good
                Console.Write(".");
                xdocCacheLines[filesearcher].lastAccess = lastAccessTicker++;
                return xdocCacheLines[filesearcher].xdoc;
            }

            if (BadOSGrids.Contains(filesearcher))
            {
                Console.WriteLine("NewGetOSVectorData Bad GridRef " + filesearcher);
                return null;
            }

            Console.Write("NewGetOSVectorData " + filesearcher);

            try
            {
                var filenames = Directory.EnumerateFiles(gridfolder, filesearcher);

                foreach (string filename in filenames)
                {
                    using (FileStream zipToOpen = new FileStream(filename, FileMode.Open))
                    {
                        using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Read))
                        //var archive = ZipFile.OpenRead(filename);
                        //using (ZipArchive archive = ZipFile.OpenRead(filename))
                        {
                            try
                            {
                                foreach (var entry in archive.Entries)
                                {
                                    if (entry.Name.EndsWith(".gml"))
                                    {
                                        using (Stream stream = entry.Open())
                                        {
                                            doc = XDocument.Load(stream);

                                            lock (xdocCacheLines)
                                            {
                                                if (xdocCacheLines.Count >= OSDataCacheMaxLines)
                                                {
                                                    ulong oldestTime = 0xffffffff;
                                                    string oldest = null;
                                                    foreach (var lin in xdocCacheLines)
                                                    {
                                                        if (lin.Value.lastAccess < oldestTime)
                                                        {
                                                            oldest = lin.Key;
                                                            oldestTime = lin.Value.lastAccess;
                                                        }
                                                    }
                                                    if (oldest != null)
                                                    {
                                                        xdocCacheLines.Remove(oldest);
                                                        //Console.WriteLine("Evicted Cache Item " + oldest + " Time " + oldestTime);
                                                        Console.Write(" -");
                                                    }
                                                }

                                                XDocCacheLine alin = new XDocCacheLine();
                                                alin.lastAccess = lastAccessTicker++;
                                                alin.xdoc = doc;
                                                xdocCacheLines.Add(filesearcher, alin);
                                                //Console.WriteLine("Added Cache Item " + filesearcher + " Time " + alin.lastAccess);
                                                Console.Write(" +");

                                            }
                                            Console.WriteLine(".");

                                            return doc;
                                        }
                                    }
                                }
                            }
                            finally
                            {
                                archive.Dispose();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(" NewGetOSVectorData Exception caught " + GridRef + " " + ex.Message + " Folder " + gridfolder + " Search " + filesearcher);
            }

            Console.WriteLine(" NewGetOSVectorData return null " + GridRef);
            BadOSGrids.Add(filesearcher);
            return null;
        }


        static int charsoutput = 0;

        public static void OutputStr(string str)
        {
            charsoutput += str.Length;
            if (charsoutput > 250)
            {
                Console.WriteLine(str);
                charsoutput = 0;
            }
            else
            {
                Console.Write(str);
            }
        }

        public XDocument GetOSVectorData(string GridRef, bool quiet)
        {
            char[] splitter = { ' ' };
            // "SH 69880 00431"
            string[] gridStrs = GridRef.ToLower().Split(splitter);

            string gridfolder = foldername + "/" + gridStrs[0];
            string gridy = gridStrs[0] + gridStrs[1][0] + gridStrs[2][0];
            string filesearcher = gridy + "_OST50CONT_*.zip";
            // C:\Users\David\Desktop\OS Terrain 50\terr50_cgml_gb\data\sn\sn34_OST50CONT_*.zip
            XDocument doc = null;


            if (xdocCacheLines.ContainsKey(filesearcher))
            {
                // Cache hit - good
                if (!quiet) OutputStr(".");
                xdocCacheLines[filesearcher].lastAccess = lastAccessTicker++;
                return xdocCacheLines[filesearcher].xdoc;
            }

            if (BadOSGrids.Contains(filesearcher))
            {
                if (!quiet) OutputStr("#");
                //Console.WriteLine("GetOSVectorData Bad GridRef " + filesearcher);
                return null;
            }

            if (!quiet) OutputStr(" @ " + gridy);

            try
            {
                IEnumerable<string> filenames = null;
                try
                {
                    filenames = Directory.EnumerateFiles(gridfolder, filesearcher);
                }
                catch
                {
                    if (!quiet) OutputStr("*");
                    BadOSGrids.Add(filesearcher);
                    return null;
                }


                foreach (string filename in filenames)
                {
                    var archive = ZipFile.OpenRead(filename);
                    try
                    {
                        foreach (var entry in archive.Entries)
                        {
                            if (entry.Name.EndsWith(".gml"))
                            {
                                using (Stream stream = entry.Open())
                                {
                                    doc = XDocument.Load(stream);

                                    lock (xdocCacheLines)
                                    {
                                        if (xdocCacheLines.Count >= OSDataCacheMaxLines)
                                        {
                                            ulong oldestTime = 0xffffffff;
                                            string oldest = null;
                                            foreach (var lin in xdocCacheLines)
                                            {
                                                if (lin.Value.lastAccess < oldestTime)
                                                {
                                                    oldest = lin.Key;
                                                    oldestTime = lin.Value.lastAccess;
                                                }
                                            }
                                            if (oldest != null)
                                            {
                                                xdocCacheLines.Remove(oldest);
                                                //Console.WriteLine("Evicted Cache Item " + oldest + " Time " + oldestTime);
                                                Console.Write(" -");
                                                GeneralUtilClasses.RunGC();
                                            }
                                        }

                                        XDocCacheLine alin = new XDocCacheLine();
                                        alin.lastAccess = lastAccessTicker++;
                                        alin.xdoc = doc;
                                        xdocCacheLines.Add(filesearcher, alin);
                                        //Console.WriteLine("Added Cache Item " + filesearcher + " Time " + alin.lastAccess);
                                        if (!quiet) OutputStr(" +");

                                    }
                                    //OutputStr(".");

                                    return doc;
                                }
                            }
                        }
                    }
                    finally
                    {
                        archive.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetOSVectorData Exception caught " + GridRef + " " + ex.Message + " Folder " + gridfolder + " Search " + filesearcher);
            }

            //Console.WriteLine("GetOSVectorData return null " + GridRef);
            BadOSGrids.Add(filesearcher);
            return null;
        }



        public class MyNEZ
        {
            public double e;
            public double n;
            public double height;

            public MyNEZ(double thee, double then, double aheight)
            {
                e = thee;
                n = then;

                height = aheight;
            }
        }


        //public class MyxyzCacheLine
        //{
        //    const int CacheLineWidthHeight = 100;
        //    const int maxcachelines = 100 * 100 * 10;
        //    static Dictionary<string, MyxyzCacheLine> AllxyzCacheLines = new Dictionary<string, MyxyzCacheLine>();
        //    static long Allxyzcachehit = 0;
        //    static long Allxyzcachetotal = 0;
        //    static double AllCacheHitRatio = 0;

        //    DateTime myage = DateTime.UtcNow;
        //    List<MyNEZ> nez = new List<MyNEZ>();
        //    string docname = null;
        //    XDocument xdoc = null;


        //    MyxyzCacheLine(string mydocname, XDocument myxdoc)
        //    {
        //        docname = mydocname;
        //        xdoc = myxdoc;
        //    }


        //    void touch()
        //    {
        //        myage = DateTime.UtcNow;
        //    }


        //    public static XDocument GetXDoc(string mydocname )
        //    {
        //        foreach (string ky in AllxyzCacheLines.Keys)
        //        {
        //            if ( ky.StartsWith( mydocname))
        //            {
        //                return AllxyzCacheLines[ky].xdoc;
        //            }
        //        }
        //        return null;
        //    }


        //    //public static void AddToCache(string mydocname, XDocument myxdoc, double easting, double northing, double height)
        //    //{
        //    //    string indx = mydocname + ((int)(easting / CacheLineWidthHeight)) + "_" + ((int)(northing / CacheLineWidthHeight));

        //    //    Allxyzcachetotal++;

        //    //    if (!AllxyzCacheLines.ContainsKey(indx))
        //    //    {
        //    //        EvictLine();
        //    //        AllxyzCacheLines[indx] = new MyxyzCacheLine(mydocname, myxdoc);
        //    //    }
        //    //    else
        //    //    {
        //    //        Allxyzcachehit++;
        //    //    }
        //    //    AllxyzCacheLines[indx].nez.Add(new MyNEZ( easting, northing, height ));
        //    //    AllxyzCacheLines[indx].touch();

        //    //    if (Allxyzcachetotal > 0)
        //    //    {
        //    //        AllCacheHitRatio = (double)Allxyzcachehit / Allxyzcachetotal * 100;
        //    //    }
        //    //}



        //    //static void EvictLine()
        //    //{
        //    //    if (AllxyzCacheLines.Count > maxcachelines)
        //    //    {
        //    //        DateTime oldest = DateTime.MinValue;
        //    //        string oldestindx = "";

        //    //        foreach (string akey in AllxyzCacheLines.Keys)
        //    //        {
        //    //            if (AllxyzCacheLines[akey].myage < oldest )
        //    //            {
        //    //                oldest = AllxyzCacheLines[akey].myage;
        //    //                oldestindx = akey;
        //    //            }
        //    //        }

        //    //        AllxyzCacheLines.Remove(oldestindx);
        //    //    }
        //    //}
        //}





        //public static double CacheHeightFromOSVectorData(OsGridRef ne, countryEnum eCountry, bool quiet, ref double mindist)
        //{
        //    OsGridRef gr = new OsGridRef(ne.east, ne.north);
        //    string GridRef = gr.toString();

        //    XDocument xdoc = GetXDoc(GridRef);
        //    if (xdoc == null)
        //    {
        //        ZipMapDataHandler zippy = new ZipMapDataHandler();
        //        xdoc = zippy.GetOSVectorData(GridRef, quiet);
        //        if (xdoc == null)
        //        {
        //            return 0;
        //        }
        //    }

        //    if (xdoc != LastXdoc)
        //    {
        //        myxyzs = new List<MyNEZ>();
        //        lastxyz = myxyzs;
        //        LastXdoc = xdoc;

        //        //Process Contours looking for heights...
        //        XNamespace xsiNameSpace = "http://www.w3.org/2001/XMLSchema-instance";
        //        XNamespace osNameSpace = "http://namespaces.ordnancesurvey.co.uk/elevation/contours/v1.0";
        //        XNamespace gmlNameSpace = "http://www.opengis.net/gml/3.2";
        //        char[] splitter = { ' ' };

        //        var allContourLines = xdoc.Descendants(osNameSpace + "ContourLine");

        //        for (int j = 0; j < 2; j++)
        //        {
        //            foreach (XElement contour in allContourLines)
        //            {
        //                var posList = contour.Element(osNameSpace + "geometry").Value.ToString();
        //                var h = contour.Element(osNameSpace + "propertyValue").Value.ToString();
        //                double z = (int)Convert.ToDouble(h);

        //                string[] ptStrs = posList.Split(splitter);

        //                for (int i = 0; i < ptStrs.Length; i += 2)
        //                {
        //                    double x = Convert.ToDouble(ptStrs[i]);
        //                    double y = Convert.ToDouble(ptStrs[i + 1]);
        //                    MyxyzCacheLine.AddToCache(GridRef, xdoc, x, y, z);
        //                }
        //            }
        //            allContourLines = xdoc.Descendants(osNameSpace + "LandWaterBoundary");
        //        }

        //        var allSpotHeights = xdoc.Descendants(osNameSpace + "SpotHeight");
        //        foreach (XElement spot in allSpotHeights)
        //        {
        //            var loc = spot.Element(osNameSpace + "geometry").Value.ToString();
        //            string h = spot.Element(osNameSpace + "propertyValue").Value.ToString();

        //            string[] ptStrs = loc.Split(splitter);
        //            double x = Convert.ToDouble(ptStrs[0]);
        //            double y = Convert.ToDouble(ptStrs[1]);
        //            double z = Convert.ToDouble(h);
        //            MyxyzCacheLine.AddToCache(GridRef, xdoc, x, y, z);
        //        }
        //    }

        //    return 0;
        //}


        // The following will be removed eventually.
        static XDocument LastXdoc = new XDocument();
        static List<MyNEZ> lastxyz = new List<MyNEZ>();
        static long xyzcachehit = 0;
        static long xyzcachetotal = 0;



        public static double GetHeightFromOSVectorData(double lat, double lon, countryEnum eCountry, bool quiet, ref double mindist)
        {
            LatLon_OsGridRef originll = new LatLon_OsGridRef(lat, lon, 0);
            OsGridRef ne = originll.toGrid(eCountry);

            OsGridRef gr = new OsGridRef(ne.east, ne.north);
            string GridRef = gr.toString();

            ZipMapDataHandler zippy = new ZipMapDataHandler();
            XDocument xdoc = zippy.GetOSVectorData(GridRef, quiet);
            if (xdoc == null)
            {
                return 0;
            }

            xyzcachetotal += 1;

            List<MyNEZ> myxyzs = lastxyz;
            if (xdoc != LastXdoc)
            {
                myxyzs = new List<MyNEZ>();
                lastxyz = myxyzs;
                LastXdoc = xdoc;

                //Process Contours looking for heights...
                XNamespace xsiNameSpace = "http://www.w3.org/2001/XMLSchema-instance";
                XNamespace osNameSpace = "http://namespaces.ordnancesurvey.co.uk/elevation/contours/v1.0";
                XNamespace gmlNameSpace = "http://www.opengis.net/gml/3.2";
                char[] splitter = { ' ' };

                var allContourLines = xdoc.Descendants(osNameSpace + "ContourLine");

                for (int j = 0; j < 2; j++)
                {
                    foreach (XElement contour in allContourLines)
                    {
                        var posList = contour.Element(osNameSpace + "geometry").Value.ToString();
                        var h = contour.Element(osNameSpace + "propertyValue").Value.ToString();
                        double z = (int)Convert.ToDouble(h);

                        string[] ptStrs = posList.Split(splitter);

                        for (int i = 0; i < ptStrs.Length; i += 2)
                        {
                            double x = Convert.ToDouble(ptStrs[i]);
                            double y = Convert.ToDouble(ptStrs[i + 1]);
                            MyNEZ xyz = new MyNEZ(x, y, z);
                            myxyzs.Add(xyz);
                        }
                    }
                    allContourLines = xdoc.Descendants(osNameSpace + "LandWaterBoundary");
                }

                var allSpotHeights = xdoc.Descendants(osNameSpace + "SpotHeight");
                foreach (XElement spot in allSpotHeights)
                {
                    var loc = spot.Element(osNameSpace + "geometry").Value.ToString();
                    string h = spot.Element(osNameSpace + "propertyValue").Value.ToString();

                    string[] ptStrs = loc.Split(splitter);
                    double x = Convert.ToDouble(ptStrs[0]);
                    double y = Convert.ToDouble(ptStrs[1]);
                    double z = Convert.ToDouble(h);

                    MyNEZ xyz = new MyNEZ(x, y, z);
                    myxyzs.Add(xyz);
                }
            }
            else
            {
                xyzcachehit += 1;
            }

            Double ratio = (double)xyzcachehit / xyzcachetotal * 100;

            double height = -32000;
            double distancesq = 9999999999e50;
            mindist = distancesq;
            double dx;
            double dy;

            double[] heights = new double[4];
            double[] distances = new double[4];
            for (int i = 0; i < distances.Length; i++)
            {
                heights[i] = height;
                distances[i] = distancesq;
            }

            for (int i = 0; i < lastxyz.Count; i++)
            {
                int indx = 0;
                dx = lastxyz[i].e - ne.east;
                dy = lastxyz[i].n - ne.north;

                if (dx < 0)
                {
                    indx += 1;
                }
                if (dy < 0)
                {
                    indx += 2;
                }

                double newdistancesq = dx * dx + dy * dy;
                if (newdistancesq < distances[indx])
                {
                    distances[indx] = newdistancesq;
                    heights[indx] = lastxyz[i].height;
                }
            }

            double totalweight = 0;
            double heightotal = 0;
            for (int i = 0; i < distances.Length; i++)
            {
                if (heights[i] >= 0)
                {
                    double distance = Math.Sqrt(distances[i]);
                    if (distance <= 0)
                    {
                        distance = 0.001;
                    }

                    if (i == 0)
                    {
                        mindist = distance;
                    }

                    double weight = 1 / distance;
                    totalweight += weight;
                    heightotal += heights[i] * weight;
                }
            }

            if (totalweight > 0)
            {
                height = heightotal / totalweight;
                return height;
            }
            else
            {
                return 0;
            }
        }



        public static double GetHeightAtLatLon(bool LIDAR, bool minecraft, double lat, double lon, countryEnum eCountry, RunJobParams rjParams, bool online = true)
        {
            double height = -9999;


            // LIDAR case
            if (LIDAR)
            {
               
                ProjCoordinate ll = new ProjCoordinate(lon, lat);
                ProjCoordinate ne = new ProjCoordinate();

                rjParams.FromLLtrans.Transform(ll, ne);
                if (ne != null)
                {
                    double z = LidarBlockReader.GetHeight(minecraft, ne.X, ne.Y, true, eCountry, rjParams);
                    if (z != LidarBlock.NODATA_const)
                    {
                        return z;
                    }
                }
            }


            height = GetSRTMHeightAtLL(minecraft, lat, lon);

            if (height < -100 && online)
            {
                // Convert to metres whatever they are...
                LatLon_OsGridRef ll = new LatLon_OsGridRef(lat, lon, 0);
                OsGridRef ne = ll.toGrid(eCountry);
                if (ne != null)
                {
                    return GetHeightAtNE(ne.east, ne.north);
                }
            }

            return height;
        }


        public static double GetSRTMHeightAtLL(bool minecraft, double lat, double lon)
        {
            //const double halfSecond = 1.0 / 60 / 60 / 2;
            if (minecraft)
            {
                // Round up and then convert to int
                double tmplat = Math.Floor(lat * 60 * 60 + 0.5);
                double tmplon = Math.Floor(lon * 60 * 60 + 0.5);

                lat = tmplat / 60 / 60;
                lon = tmplon / 60 / 60;
            }

            // This gets the tile lat, lon.
            int latInt = (int)Math.Floor(lat);
            int lonInt = (int)Math.Floor(lon);

            Int16[][] tile = ZipMapDataHandler.GetSRTMData(latInt, lonInt);
            if (tile != null)
            {

                // This then gets the square in the tile.
                int latPart = (int)((lat - latInt) * ZipMapDataHandler.gridCount);
                int lonPart = (int)((lon - lonInt) * ZipMapDataHandler.gridCount);

                if (latPart < 0 || latPart >= ZipMapDataHandler.gridCount || lonPart < 0 || lonPart >= ZipMapDataHandler.gridCount)
                {
                    return -9999;
                }

                // Get the two heights based on lat first.
                double latweight = ((lat - latInt - ((double)latPart / ZipMapDataHandler.gridCount)) * ZipMapDataHandler.gridCount);
                double lonweight = ((lon - lonInt - ((double)lonPart / ZipMapDataHandler.gridCount)) * ZipMapDataHandler.gridCount);

                double h1 = tile[latPart][lonPart];
                double h2 = tile[latPart][lonPart + 1];
                double h3 = tile[latPart + 1][lonPart];
                double h4 = tile[latPart + 1][lonPart + 1];

                double lonh1 = h1 + (h2 - h1) * lonweight;
                double lonh2 = h3 + (h4 - h3) * lonweight;

                double latlonh1 = lonh1 + (lonh2 - lonh1) * latweight;

                return latlonh1;
            }
            return -9999;
        }


        public static Double GetHeightAtNE(double east, double north)
        {
            try
            {
                //  "http://ukterrain.appspot.com/?e=275000&n=084000&m=1"
                string uri = "http://ukterrain.appspot.com/?e=" + (int)east + "&n=" + (int)north + "&m=1";

                WebRequest req = WebRequest.Create(uri);
                WebResponse res = req.GetResponse();

                Stream dataStream = res.GetResponseStream();

                StreamReader reader = new StreamReader(dataStream);

                string rt = reader.ReadToEnd();

                //AltitudeLabel.Text = rt;

                // { "e":275000,"n":84000,"z":221}
                ENZ enz = JsonConvert.DeserializeObject<ENZ>(rt);

                return enz.z;
            }
            catch
            {
                return 0;
            }
        }
    }
}
