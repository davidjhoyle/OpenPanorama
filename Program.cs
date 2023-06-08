// See https://aka.ms/new-console-template for more information
using System;
using System.Formats.Asn1;
using System.Runtime.CompilerServices;
using Microsoft.VisualBasic.FileIO;
using System.Xml.Linq;
using OpenPanoramaLib;
using System.Security.Cryptography;

namespace OpenPanorama // Note: actual namespace depends on the project name.
{
    internal class Program
    {
        static List<SunMoonRunJob> allRunJobs = new List<SunMoonRunJob>();
        public static int maxJobsToProcess = 9999999;
        const int ERR_MORE_JOBS = 999;

        static int Main(string[] args)
        {
            Console.WriteLine("Hello OpenPanorama");

            if (args.Count() == 0)
            {
                string buildDate = "1.0.0.1";
                RunJobParams.Usage(buildDate);
                return 0;
            }

            try
            {
                RunJobParams rjParams = new RunJobParams(args);

                //ZipMapDataHandler.saveSRTMDataPatches("SRTMDataPatches.json");
                ZipMapDataHandler.getSRTMDataPatches("SRTMDataPatches.json");


                maxJobsToProcess = rjParams.maxJobs;

                if (rjParams.LASFile != null)
                {
                    BinaryReader binr = new BinaryReader(File.Open(rjParams.LASFile, FileMode.Open));
                    LASClass las = LASClass.Read(binr);
                    binr.Close();
                    las.CreateASCFile(rjParams.lat, rjParams.lon, rjParams.ASCSize, rjParams.ASCFile, LidarBlockReader.GetLidarRes(), rjParams.eCountry);
                    return 0;
                }

                if (rjParams.dumpHeights != null)
                {
                    DumpHeights(rjParams.drwLIDAR, rjParams.lat, rjParams.lon, rjParams.proximalInterpolation, rjParams.dumpHeights, rjParams.eCountry, rjParams);
                    return 0;
                }

                if (rjParams.dumpMultipleHeights != null)
                {
                    DumpMultipleHeights(rjParams.drwLIDAR, rjParams.dumpMultipleHeights, rjParams.proximalInterpolation, rjParams.eCountry, rjParams);
                    return 0;
                }

                if (rjParams.dumpLIDAR != null)
                {
                    DumpLIDAR(rjParams.drwLIDAR, rjParams.lat, rjParams.lon, rjParams.proximalInterpolation, rjParams.dumpLIDAR, rjParams.eCountry, rjParams);
                    return 0;
                }

                if (rjParams.checkLIDAR >= 0)
                {
                    CheckLIDAR(rjParams.drwLIDAR, rjParams.checkLIDAR, rjParams.lat, rjParams.lon, rjParams.drwSpots, rjParams.drwContours, rjParams.checkFile, rjParams.eCountry, rjParams);
                    return 0;
                }

                if (rjParams.CreateSRTMTiles >= 0)
                {
                    CreateSRTMTiles(rjParams);
                    return 0;
                }

                if (rjParams.CreateLIDARCache)
                {
                    LidarBlockReader.ProcessZIPsToLidarCache(rjParams.eCountry, rjParams);
                    return 0;
                }


                if (rjParams.csvBulkFiles != null)
                {
                    if (ReadProcessCSVBulkFiles(rjParams))
                    {
                        return 0;
                    }
                    else
                    {
                        return ERR_MORE_JOBS;
                    }
                }


                try
                {
                    SunMoonRunJob rj = new SunMoonRunJob("99999", rjParams.name, 0, null, "", rjParams.lat, rjParams.lon, rjParams.theCountyFile,
                        0, 0, 0, 0, "", "", rjParams);

                    ProcessJob(false, rj);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception caught " + ex.Message + " " + ex.StackTrace + " " + ex.InnerException);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception caught " + ex.Message + " " + ex.StackTrace + " " + ex.InnerException);
            }


            // Are there still jobs waiting to be done?
            if (theJobs.Count > 0)
            {
                return ERR_MORE_JOBS;
            }
            else
            {
                return 0;
            }
        }



        static void dumpJSONDetails(bool first, StreamWriter outfile, SunMoonRunJob rj)
        {
            string[] bits = new string[4];
            bits[0] = PaintImage.cleanfilename(rj.name).Replace(" ", "-");
            bits[1] = PaintImage.cleanfilename(rj.typestr).Replace(" ", "-");
            bits[2] = PaintImage.cleanfilename(rj.county).Replace(" ", "-");
            bits[3] = rj.mapref;

            for (int b = 0; b < bits.Length; b++)
            {
                bits[b] = bits[b].Trim();
            }

            var ll = new LatLon_OsGridRef(rj.lat, rj.lon, 0);

            string srcurl = "";
            string srccomment = "";

            if (Convert.ToInt32(rj.url) != 99999)
            {
                srcurl = rj.rjParams.megpURL + Convert.ToInt32(rj.url);
                srccomment = "Megalithic Portal";
            }

            dumpJSONDetails(rj.rjParams.proximalInterpolation, first, outfile, bits, ll, srcurl, srccomment, rj.rjParams.eCountry, rj.rjParams.mycountry, rj.rjParams);
        }



        static void dumpJSONDetails(bool proximalInterpolation, bool first, StreamWriter outfile, string[] bits, LatLon_OsGridRef ll, string srcurl, string srccomment, countryEnum eCountry, string cc, RunJobParams rjParams)
        {
            if (ll == null)
            {
                ll = LatLon_OsGridRef.GetLLFromLoc(bits[3], eCountry);
            }
            double lat = ll.lat;
            double lon = ll.lon;
            double height = ZipMapDataHandler.GetHeightAtLatLon(false, proximalInterpolation, lat, lon, eCountry, rjParams);

            //    "Name": "Pennal Church",
            //    "MonType": "Possible Stone Circle",
            //    "County": "Powys",
            //    "GridRef": "52.585681 -3.920656",
            //    "Filename": "Pennal Church_52.585681 -3.920656",
            //    "LatLon": "52.585681 -3.920656",
            //    "Height": 12,
            //    "Description": "Pennal Church Possible Stone Circle in Powys 52.585681 -3.920656"
            if (bits.Length == 4)
            {
                if (first)
                {
                    outfile.WriteLine("{");
                }
                else
                {
                    outfile.WriteLine(",{");
                }
                outfile.WriteLine("\"Name\": \"" + PaintImage.cleanfilename(bits[0]) + "\",");
                outfile.WriteLine("\"CC\": \"" + cc + "\",");
                outfile.WriteLine("\"MonType\": \"" + PaintImage.cleanfilename(bits[1]) + "\",");
                outfile.WriteLine("\"County\": \"" + PaintImage.cleanfilename(bits[2]) + "\",");
                outfile.WriteLine("\"GridRef\": \"" + bits[3] + "\",");
                outfile.WriteLine("\"Filename\": \"" + PaintImage.cleanfilename(bits[0] + "_" + bits[3]).ToLower() + "\",");
                outfile.WriteLine("\"LatLon\": \"" + lat + " " + lon + "\",");
                outfile.WriteLine("\"Height\": \"" + (int)height + "\",");
                outfile.WriteLine("\"Description\": \"" + PaintImage.cleanfilename(bits[0] + " " + bits[1] + " in " + bits[2]) + " " + lat + " " + lon + "\",");

                outfile.WriteLine("\"SourceURL\": \"" + srcurl + "\",");
                outfile.WriteLine("\"SourceComment\": \"" + srccomment + "\"");

                outfile.WriteLine("}");
            }
        }


        public static bool cleanupNames = true;


        public static void DumpCounties(List<SunMoonRunJob> rjs, string myCountryLong, string CC, string theCountyFile, string theCountyHTML)
        {
            if (rjs == null || myCountryLong == null || CC == null || theCountyFile == null || theCountyHTML == null)
            {
                return;
            }

            CC = CC.ToLower();

            SortedDictionary<string, int> allCounties = new SortedDictionary<string, int>();
            foreach (SunMoonRunJob rj in rjs)
            {
                if (allCounties.ContainsKey(rj.county))
                {
                    allCounties[rj.county] += 1;
                }
                else
                {
                    allCounties[rj.county] = 1;
                }
            }

            string countiesfilename = PaintImage.outfolder + "/" + theCountyFile;
            string countiesHTMLfilename = PaintImage.outfolder + "/" + theCountyHTML;

            string gpxurl = rjs[0].rjParams.blobStoreURL + CC + "0/";

            Console.WriteLine("Create Counties File " + countiesfilename + " Count = " + allCounties.Count);
            System.IO.StreamWriter outfile = new System.IO.StreamWriter(countiesfilename);
            System.IO.StreamWriter htmlfile = new System.IO.StreamWriter(countiesHTMLfilename);

            outfile.WriteLine("[");

            htmlfile.WriteLine("<html>");
            htmlfile.WriteLine("<head>");
            htmlfile.WriteLine("<meta charset=\"utf-8\" />");
            htmlfile.WriteLine("<title> " + rjs[0].rjParams.standStonesHost + myCountryLong + " Counties/Areas Page</title >");
            htmlfile.WriteLine("</head>");
            htmlfile.WriteLine("<body>");
            htmlfile.WriteLine("<h1>");
            htmlfile.WriteLine(rjs[0].rjParams.standStonesHost + " - location of sites in " + myCountryLong);
            htmlfile.WriteLine("</h1>");
            htmlfile.WriteLine("<table>");
            htmlfile.WriteLine("<tr><th>Area or Region</th></tr>");

            bool first = true;

            foreach (string c in allCounties.Keys)
            {
                if (!first)
                    outfile.WriteLine(",");
                outfile.WriteLine("{ \"County\": \"" + c + "\" }");
                htmlfile.WriteLine("<tr><td><a href=\"" + rjs[0].rjParams.viewGPSURL + gpxurl + c + ".gpx\" target=\"_blank\">" + c + "</a></td></tr>");
                first = false;
            }
            outfile.WriteLine("]");

            htmlfile.WriteLine("</table>");
            htmlfile.WriteLine("</body>");
            htmlfile.WriteLine("</html>");
            htmlfile.Close();
            outfile.Close();
        }



        public static void DumpGeoPoints(List<SunMoonRunJob> rjs, string myCountryLong, string mycountry, RunJobParams rjParams)
        {
            if (!rjParams.dumpLocationGPXs) return;

            myCountryLong = myCountryLong.ToLower();
            mycountry = mycountry.ToLower();

            SortedDictionary<string, int> allCounties = new SortedDictionary<string, int>();

            allCounties[myCountryLong] = 1;
            string countrystr = "&amp;country=" + mycountry;


            foreach (SunMoonRunJob rj in rjs)
            {
                if (!allCounties.ContainsKey(rj.county))
                {
                    allCounties[rj.county] = 1;
                }
                else
                {
                    allCounties[rj.county] += 1;
                }
            }

            foreach (string area in allCounties.Keys)
            {
                string filename = PaintImage.outfolder + "/" + area + ".gpx";

                Console.WriteLine("Create GPX Geo Points File " + filename + " Count = " + allCounties[area]);
                System.IO.StreamWriter outfile = new System.IO.StreamWriter(filename);
                outfile.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"no\" ?>");
                outfile.WriteLine("<gpx>");

                foreach (SunMoonRunJob rj in rjs)
                {
                    if (area == myCountryLong || rj.county.ToLower() == area.ToLower()) // || (country != null && country.Contains(rj.county.ToLower())))
                    {
                        string symbol = "circle";
                        switch (rj.typestr)
                        {
                            case "Stone Circle":
                            case "Ring Cairn":
                            case "Timber Circle":
                                symbol = "circle";
                                break;
                            case "Standing Stone (Menhir)":
                            case "Standing Stones":
                            case "Stone Row / Alignment":
                                symbol = "triangle";
                                break;
                            case "Henge":
                                symbol = "diamond";
                                break;
                            case "Multiple Stone Rows / Avenue":
                                symbol = "star";
                                break;
                            case "Viewpoint":
                                symbol = "cross";
                                break;
                            default:
                                symbol = "cross";
                                break;
                        }

                        double lat = rj.lat;
                        double lon = rj.lon;

                        outfile.WriteLine("<!--Centre Waypoint " + lat + " " + lon + "-->");
                        outfile.WriteLine("<wpt lat=\"" + lat + "\" lon=\"" + lon + "\">");
                        outfile.WriteLine("<name>" + rj.name.Replace("&", " ") + " " + rj.typestr.Replace("&", " ") + " lat/lon " + lat + "," + lon + "</name>");

                        outfile.WriteLine("<url>" + rj.rjParams.viewImageURL + rj.name.Replace("&", " ") + countrystr + "</url>");
                        outfile.WriteLine("<symbol>" + symbol + "</symbol>");

                        int urlid = 99999;
                        try
                        {
                            urlid = Convert.ToInt32(rj.url);
                        }
                        catch
                        {
                        }

                        if (urlid > 0 && urlid != 99999)
                        {
                            outfile.WriteLine("<cmt>Information from the Megalithic Portal</cmt>");
                        }

                        outfile.WriteLine("</wpt>");
                    }
                }

                outfile.WriteLine("</gpx>");
                outfile.Close();
            }
        }


        static List<SunMoonRunJob> theJobs = new List<SunMoonRunJob>();
        //static bool ThreadsRunning = false;
        static bool CancelJobs = false;


        static string CleanupSiteName(string name)
        {
            name = name.Replace("Š", "S");
            name = name.Replace("Œ", "OE");
            name = name.Replace("Ž", "Z");
            name = name.Replace("™", "tm");
            name = name.Replace("š", "s");
            name = name.Replace("œ", "oe");
            name = name.Replace("ž", "z");
            name = name.Replace("Ÿ", "Y");
            name = name.Replace("¡", "i");
            name = name.Replace("¢", "c");
            name = name.Replace("À", "A");
            name = name.Replace("Á", "A");
            name = name.Replace("Â", "A");
            name = name.Replace("Ã", "A");
            name = name.Replace("Ä", "A");
            name = name.Replace("Å", "A");
            name = name.Replace("Æ", "AE");
            name = name.Replace("Ç", "C");
            name = name.Replace("È", "E");
            name = name.Replace("É", "E");
            name = name.Replace("Ê", "E");
            name = name.Replace("Ë", "E");
            name = name.Replace("Ì", "I");
            name = name.Replace("Í", "I");
            name = name.Replace("Î", "I");
            name = name.Replace("Ï", "I");
            name = name.Replace("Ð", "ETH");
            name = name.Replace("Ñ", "N");
            name = name.Replace("Ò", "O");
            name = name.Replace("Ó", "O");
            name = name.Replace("Ô", "O");
            name = name.Replace("Õ", "O");
            name = name.Replace("Ö", "O");
            name = name.Replace("Ø", "O");
            name = name.Replace("Ù", "U");
            name = name.Replace("Ú", "U");
            name = name.Replace("Û", "U");
            name = name.Replace("Ü", "U");
            name = name.Replace("Ý", "Y");
            name = name.Replace("Þ", "THORN");
            name = name.Replace("ß", "s");
            name = name.Replace("à", "a");
            name = name.Replace("á", "a");
            name = name.Replace("â", "a");
            name = name.Replace("ã", "a");
            name = name.Replace("ä", "a");
            name = name.Replace("å", "a");
            name = name.Replace("æ", "ae");
            name = name.Replace("ç", "c");
            name = name.Replace("è", "e");
            name = name.Replace("é", "e");
            name = name.Replace("ê", "e");
            name = name.Replace("ë", "e");
            name = name.Replace("ì", "i");
            name = name.Replace("í", "i");
            name = name.Replace("î", "i");
            name = name.Replace("ï", "i");
            name = name.Replace("ð", "eth");
            name = name.Replace("ñ", "n");
            name = name.Replace("ò", "o");
            name = name.Replace("ó", "o");
            name = name.Replace("ô", "o");
            name = name.Replace("õ", "o");
            name = name.Replace("ö", "o");
            name = name.Replace("ø", "o");
            name = name.Replace("ù", "u");
            name = name.Replace("ú", "u");
            name = name.Replace("û", "u");
            name = name.Replace("ü", "u");
            name = name.Replace("ý", "y");
            name = name.Replace("þ", "thorn");
            name = name.Replace("ÿ", "y");

            name = name.Replace("'", "");
            name = name.Replace(";", " ");
            name = name.Replace("/", "");
            name = name.Replace("?", "");
            name = name.Replace(".", " ");
            name = name.Replace(",", " ");
            name = name.Replace("&", " ");
            name = name.Replace("'", "");
            name = name.Replace("(", " ");
            name = name.Replace(")", " ");
            name = name.Replace("  ", " ");
            name = name.Replace("  ", " ");
            name = name.Replace("  ", " ");
            name = name.Replace("  ", " ");
            name = name.Trim();
            name = name.Replace(" ", "-");
            name = name.ToLower();

            string newname = "";

            for (int i = 0; i < name.Length; i++)
            {
                if ((name[i] >= '0' && name[i] <= '9') || (name[i] >= 'a' && name[i] <= 'z') || name[i] == '-')
                {
                    newname += name[i];
                }
                else
                {
                    Console.WriteLine("Unsupported Character *** " + ((ushort)name[i]) + " *** in " + name);
                }
            }

            return newname;
        }


        static public bool ReadProcessCSVBulkFiles(RunJobParams rjParms)
        {
            System.IO.StreamWriter outfile = null;


            if (rjParms.jsonfile != null)
            {
                Console.WriteLine("Open Files, Out = " + rjParms.jsonfile);
                outfile = new System.IO.StreamWriter(rjParms.jsonfile);
                outfile.WriteLine("[");
            }

            bool first = true;

            List<string> notAddedTypes = new List<string>();

            foreach (string csv in rjParms.csvBulkFiles)
            {
                int count = 0;

                Console.WriteLine("Open Files, in = " + csv);

                // Read from a file
                TextFieldParser parser = new TextFieldParser(csv);

                parser.HasFieldsEnclosedInQuotes = true;
                parser.SetDelimiters(",");

                string[] fields;
                //bool firstLine = true;

                while (!parser.EndOfData)
                {
                    fields = parser.ReadFields();
                    if (fields.Length < 12 || fields[0].Contains("URL"))
                    {
                        continue;
                    }

                    for (int f = 8; f <= 11; f++)
                    {
                        if (fields[f].Length == 0)
                        {
                            fields[f] = "0";
                        }
                    }

                    fields[1] = CleanupSiteName(fields[1]);
                    fields[7] = CleanupSiteName(fields[7]);

                    if (fields[7].Length == 0)
                    {
                        fields[7] = "unknown";
                    }

                    for (int f = 0; f < fields.Length; f++)
                    {
                        fields[f] = fields[f].Trim();
                    }

                    //fields[3] = fields[3].Replace("/", "-").Replace("(", "-").Replace(")", "-").Replace("--", "-").Replace("--", "-").Replace("--", "-");
                    //while (fields[3].EndsWith("-"))
                    //{
                    //    fields[3] = fields[3].Substring(0, fields[3].Length - 1);
                    //}

                    try
                    {
                        SunMoonRunJob rj = new SunMoonRunJob(fields[0], fields[1], Convert.ToInt32(fields[2]), fields[3], fields[4],
                            Convert.ToDouble(fields[6]), Convert.ToDouble(fields[5]), fields[7],
                            Convert.ToInt32(fields[8]), Convert.ToInt32(fields[9]),
                            Convert.ToInt32(fields[10]), Convert.ToInt32(fields[11]),
                            fields[12], fields[13], rjParms);

                        // Keep a list of all run jobs for later...
                        allRunJobs.Add(rj);

                        bool added = false;

                        if (rjParms.siteTypes.Contains(rj.typestr))
                        {
                            if (rjParms.match == null || rj.name.ToLower().Replace("-", "").Replace(" ", "").IndexOf(rjParms.match) >= 0)
                            {
                                if (rjParms.region == null || rj.county.ToLower().Replace("-", "").Replace(" ", "").IndexOf(rjParms.region) >= 0)
                                {
                                    if (rjParms.heightfilter >= 0)
                                    {

                                    }
                                    theJobs.Add(rj);
                                    count++;
                                    added = true;
                                    if (outfile != null)
                                    {
                                        dumpJSONDetails(first, outfile, rj);
                                        first = false;
                                    }

                                    if (rjParms.sidfiles != null && rjParms.sidfiles.Length > 0)
                                    {
                                        string sidsfilename = rjParms.sidfiles + "/" + rj.url + ".json";
                                        Console.WriteLine("Open SID File, Out = " + sidsfilename + " " + rj.name + " " + rj.county + " " + rjParms.myCountryLong);
                                        System.IO.StreamWriter sidfile = new System.IO.StreamWriter(sidsfilename);
                                        dumpJSONDetails(true, sidfile, rj);
                                        sidfile.Close();
                                    }
                                }
                            }
                        }
                        if (!added)
                        {
                            //Console.WriteLine("Job not added " + rjParms.siteTypes.Contains(rj.typestr) + " " + rj.name + " : " + rj.typestr );
                            if (!notAddedTypes.Contains(rj.typestr))
                            {
                                notAddedTypes.Add(rj.typestr);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Exception caught " + ex.Message + " " + ex.StackTrace + " " + ex.InnerException);
                    }
                }
                parser.Close();
                Console.WriteLine("Number of Jobs from " + csv + " = " + count);
            }

            if (outfile != null)
            {
                outfile.WriteLine("]");
                outfile.Close();
            }

            theJobs.Sort(delegate (SunMoonRunJob x, SunMoonRunJob y)
            {
                return x.name.CompareTo(y.name);
            });

            DumpCounties(theJobs, rjParms.myCountryLong, rjParms.mycountry, rjParms.theCountyFile, rjParms.theCountyHTML);
            DumpGeoPoints(theJobs, rjParms.myCountryLong, rjParms.mycountry, rjParms);
            CancelJobs = false;

            if (rjParms.numThreads <= 1)
            {
                PollJobs();
            }
            else
            {
                // Kick off N worker threads...
                for (int t = 0; t < rjParms.numThreads; t++)
                {
                    // Or lambda expressions if you are using C# 3.0
                    Thread t3 = new Thread(() => PollJobs());
                    t3.Start();
                    //ThreadsRunning = true;
                }
            }

            Console.Write("Not Added Types Found ");
            foreach (string s in notAddedTypes)
            {
                Console.Write("," + s);
            }
            Console.WriteLine("");

            // Are there still jobs? Return false;
            if (theJobs.Count > 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }



        static public double GetMaxDistanceForHeight(double maxDistance, double height, double maxHeight)
        {
            // Get the total dip.
            double totalDip = maxHeight + height;

            for (double dist = maxDistance; dist > 1000; dist -= 100)
            {
                double myDip = PaintImage.CalculateDip(dist);
                if (myDip < totalDip)
                {
                    break;
                }
                maxDistance = dist;
            }

            return maxDistance;
        }



        public static double GetSiteHeight(SunMoonRunJob job)
        {
            //double height = ZipOSHandler.GetHeightAtLatLonFromSRTMData(job.lat, job.lon);
            double height = ZipMapDataHandler.GetHeightAtLatLon(job.rjParams.drwLIDAR, job.rjParams.proximalInterpolation, job.lat, job.lon, job.rjParams.eCountry, job.rjParams);
            if (height < job.rjParams.heightfilter)
            {
                Console.WriteLine(job.name + " Height Filtered " + height);
                return -9999;
            }

            if (height < 2)
            {
                height = 0; // job.rjParams.observerHeight;
            }

            // Special case - the modern town has obscured the view from the links... raise the height as a work around.
            if (job.name.ToLower().Replace(" ", "-").Contains("lundin-links"))
            {
                Console.WriteLine("Found lundin-links - Raise Height");
                height += 2;
            }

            if (job.name.ToLower().Replace(" ", "-").Contains("esguan"))
            {
                Console.WriteLine("Found esguan - Raise Height");
                height += 3;
            }

            // Add observerHeight (default 1.5 metres) to the height - apprx height of a human.
            height += job.rjParams.observerHeight;

            return height;
        }



        static public void ProcessStellariumSite(bool overwrite, bool siteFolder, StoneSite stonestite, RunJobParams rjp)
        {
            if (overwrite)
            {
                // Need to delete the file if set to overwrite...
                StellariumHelper.DeleteStellariumFile(siteFolder, stonestite, PaintImage.outfolder);
            }

            if (!StellariumHelper.CheckStellariumFile(siteFolder, stonestite, PaintImage.outfolder))
            {
                try
                {
                    StellariumHelper.CreateStellariumZipFile(siteFolder, stonestite, PaintImage.outfolder, rjp);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("CreateStellariumZipFile Exception " + stonestite.Filename + " " + ex.Message + " " + ex.StackTrace);
                }
            }
        }



        static public bool GetLock(string filename)
        {
            try
            {
                bool createLockFile = false;

                filename = filename.Replace(".jpg", ".lck");
                FileInfo fi = new FileInfo(filename);

                if (fi.Exists)
                {
                    DateTime lastmodified = fi.LastWriteTime;

                    if (DateTime.Now - lastmodified > TimeSpan.FromHours(3))
                    {
                        fi.Delete();
                        fi.Refresh();
                        createLockFile = true;
                    }
                }
                else
                {
                    createLockFile = true;
                }

                if (createLockFile)
                {
                    var stream = fi.Create();
                    stream.Close();
                    Console.WriteLine("GetLock " + filename + " Success");
                    return true;
                }
                else
                {
                    Console.WriteLine("GetLock " + filename + " Already Locked - Skipping");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetLock Exception " + filename + " " + ex.Message + " " + ex.StackTrace);
            }
            return false;
        }


        static public void RemoveLock(string filename)
        {
            filename = filename.Replace(".jpg", ".lck");
            FileInfo fi = new FileInfo(filename);
            fi.Delete();
        }



        static public bool CheckOutputFilesExists(string baseFile, StoneSite stonestite, SunMoonRunJob job)
        {
            string peaksFile = baseFile + "_peaks";
            if (job.rjParams.PeakProcess && job.rjParams.PeakCSV)
            {
                if (!File.Exists(peaksFile + ".csv"))
                {
                    return false;
                }
            }

            string declpeakFile = baseFile + "_declpeaks";
            if (job.rjParams.PeakProcess && job.rjParams.DeclinationPeakCSV)
            {
                if (!File.Exists(declpeakFile + ".csv"))
                {
                    return false;
                }
            }

            if (job.rjParams.PeakProcess && job.rjParams.PeakJSON)
            {
                if (!File.Exists(peaksFile + ".jsn"))
                {
                    return false;
                }
            }

            if (job.rjParams.PeakProcess && job.rjParams.DeclinationPeakJSON)
            {
                if (!File.Exists(declpeakFile + ".jsn"))
                {
                    return false;
                }
            }

            string correlateFile = baseFile + "_" + (int)job.rjParams.ages[job.rjParams.ages.Length - 1] + "_correlation";
            if (job.rjParams.Correlate && job.rjParams.CorrelateCSV)
            {
                if (!File.Exists(correlateFile + ".csv"))
                {
                    return false;
                }
            }

            if (job.rjParams.Correlate && job.rjParams.CorrelateJSON)
            {
                if (!File.Exists(correlateFile + ".jsn"))
                {
                    return false;
                }
            }

            string skimFile = baseFile + "_" + (int)job.rjParams.ages[job.rjParams.ages.Length - 1] + "_skim";
            if (job.rjParams.Correlate && job.rjParams.SkimCSV)
            {
                if (!File.Exists(skimFile + ".csv"))
                {
                    return false;
                }
            }

            if (job.rjParams.Correlate && job.rjParams.SkimJSON)
            {
                if (!File.Exists(skimFile + ".jsn"))
                {
                    return false;
                }
            }

            return true;
        }


        static public void PeakProcessAndCorrelate(bool siteFolder, StoneSite stonestite, SunMoonRunJob job)
        {
            string SiteFolder = PaintImage.getFolderName(siteFolder, stonestite.County, stonestite.MonType, stonestite.Name, stonestite.GridRef);
            string baseFile = SiteFolder + "/" + stonestite.Filename;
            baseFile = baseFile.Replace("--", "-").Replace("--", "-");

            string gpxFile = baseFile + ".gpx";
            string pngFile = baseFile + ".png";
            string hiResFile = baseFile + "_hires.jsn";
            string peakFile = baseFile + "_peaks";
            string declpeakFile = baseFile + "_declpeaks";

            if (CheckOutputFilesExists(baseFile, stonestite, job))
            {
                return;
            }

            SiteHorizonAndPeakData shapd = new SiteHorizonAndPeakData();


            shapd.latitudeOrigin = job.lat;
            shapd.longitudeOrigin = job.lon;

            string csvPeakHeaderPrefix = "Name,GridRef,MonType,Region,Country";
            string csvPeakDataPrefix = stonestite.Name + "," + stonestite.GridRef + "," + stonestite.MonType + "," + stonestite.County + "," + job.rjParams.mycountry;

            shapd.PeakProcess(gpxFile, pngFile, hiResFile, job.rjParams.pixels, peakFile, declpeakFile,
                job.rjParams.PeakJSON, job.rjParams.PeakCSV, job.rjParams.DeclinationPeakJSON, job.rjParams.DeclinationPeakCSV, job.rjParams.SaveDeclinPNG,
                job.rjParams.minPPAzDistance, job.rjParams.minPPElevDistance, job.rjParams.minPPDistance, csvPeakHeaderPrefix, csvPeakDataPrefix,
                job.lat, job.rjParams.minSkimAzDelta);


            foreach (var age in job.rjParams.ages)
            {
                if (job.rjParams.PeakProcess || job.rjParams.Correlate)
                {
                    RisingAndSettingPositionsAndCorrelation rasp_data = new RisingAndSettingPositionsAndCorrelation();
                    string raspFilename = baseFile + "_" + (int)age + "_120_sunmoon.gpx";
                    rasp_data.readRisingAndSettingsGPXData(raspFilename);

                    if (job.rjParams.Skim)
                    {
                        rasp_data.detectSkimmingPoints(job.rjParams.minSkimDistance, job.rjParams.minSkimAzDelta);

                        string skimFilename = baseFile + "_" + (int)age + "_skim";
                        string csvSkimHeaderPrefix = "Name,GridRef,MonType,Age,Region,Country";
                        string csvSkimDataPrefix = stonestite.Name + "," + stonestite.GridRef + "," + stonestite.MonType + "," + (int)age + "," + stonestite.County + "," + job.rjParams.mycountry;

                        rasp_data.saveSkimmingPoints(skimFilename, job.rjParams.SkimCSV, job.rjParams.SkimJSON, csvSkimHeaderPrefix, csvSkimDataPrefix);
                    }

                    CorrelationData cd = new CorrelationData();
                    cd.CorrelatePeaksAndRASP(shapd.peakList, rasp_data.RisingAndSettingPoints,
                                job.rjParams.minCorrelationValue, job.rjParams.minCorrelationValueSlopeWeighted,
                                job.rjParams.minCorrelationValueElevDiffWeighted, job.rjParams.minCorrelationValueWeighted);
                    string correlationFilename = baseFile + "_" + (int)age + "_correlation";
                    string correlationSummaryFilename = baseFile + "_" + (int)age + "_correlation_summary";
                    string csvHeaderPrefix = "Name,GridRef,MonType,Age,Region,Country";
                    string csvDataPrefix = stonestite.Name + "," + stonestite.GridRef + "," + stonestite.MonType + "," + (int)age + "," + stonestite.County + "," + job.rjParams.mycountry;

                    cd.saveCorrelations(correlationFilename, correlationSummaryFilename, job.rjParams.CorrelateCSV, job.rjParams.CorrelateJSON, csvHeaderPrefix, csvDataPrefix);
                }
            }
        }



        static public void ProcessJob(bool siteFolder, SunMoonRunJob job)
        {
            job.rjParams.filename = PaintImage.getJPGName(siteFolder, job).Replace("--", "-").Replace("--", "-");
            job.rjParams.lat = job.lat;
            job.rjParams.lon = job.lon;
            double height = -9999;
            string stellarfile = (job.name + "_" + job.mapref).ToLower();
            string desc = job.name + " " + job.typestr + " in " + job.county + " " + job.lat + " " + job.lon;

            StoneSite stonestite = new StoneSite(job.name, desc, stellarfile, job.lat, job.lon, height, job.county, job.typestr, job.mapref);

            try
            {
                string lastFilename = job.rjParams.filename.Replace(".jpg", "_" + (int)job.rjParams.ages[job.rjParams.ages.Length - 1] + "_60_fgrid.jpg");
                if (!job.rjParams.replaceFiles && File.Exists(lastFilename))
                {
                    Console.WriteLine(lastFilename + " Already exists - Skipping - Remaining " + theJobs.Count);

                    if (job.rjParams.stellarium)
                    {
                        if (!StellariumHelper.CheckStellariumFile(siteFolder, stonestite, PaintImage.outfolder))
                        {
                            height = GetSiteHeight(job);
                            stonestite.Height = "" + height;

                            ProcessStellariumSite(job.rjParams.replaceFiles, siteFolder, stonestite, job.rjParams);
                        }
                    }

                    if (job.rjParams.PeakProcess || job.rjParams.Correlate || job.rjParams.Skim)
                    {
                        try
                        {
                            PeakProcessAndCorrelate(siteFolder, stonestite, job);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Exception caught " + ex.Message + " " + ex.StackTrace + " " + ex.InnerException);
                        }

                    }


                    if (job.rjParams.recreateHorizon)
                    {
                        PaintImage pi = new PaintImage(job.rjParams, true);
                        SiteHorizonAndPeakData shapd = new SiteHorizonAndPeakData();

                        string gpxfilename = job.rjParams.filename.Replace(".jpg", ".gpx");

                        Console.WriteLine("recreateHorizon readGPXData " + gpxfilename);

                        shapd.readGPXData(gpxfilename);

                        Console.WriteLine("recreateHorizon build new HV " + shapd.theHoz.theHorizon.Length);

                        pi.horizonLatLon = new HorizonVector[shapd.theHoz.theHorizon.Length];
                        for (int i = 0; i < shapd.theHoz.theHorizon.Length; i++)
                        {
                            pi.horizonLatLon[i] = new HorizonVector();
                            pi.horizonLatLon[i].azimuth1 = pi.horizonLatLon[i].azimuth2 = shapd.theHoz.theHorizon[i].bearing;
                            pi.horizonLatLon[i].latitude1 = pi.horizonLatLon[i].latitude2 = shapd.theHoz.theHorizon[i].latitude;
                            pi.horizonLatLon[i].longitude1 = pi.horizonLatLon[i].longitude2 = shapd.theHoz.theHorizon[i].longitude;
                            pi.horizonLatLon[i].altitude1 = pi.horizonLatLon[i].altitude2 = shapd.theHoz.theHorizon[i].elevation;
                            pi.horizonLatLon[i].distance1 = pi.horizonLatLon[i].distance2 = shapd.theHoz.theHorizon[i].distance;
                        }

                        //gpxfilename += ".gpxtmp";

                        Console.WriteLine("recreateHorizon SaveHorizonGPXCSV " + gpxfilename);
                        pi.SaveHorizonGPXCSV(true, gpxfilename);
                    }

                    return;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception caught " + ex.Message + " " + ex.StackTrace + " " + ex.InnerException);
            }


            if (job.rjParams.LockFiles && !GetLock(job.rjParams.filename))
            {
                return;
            }

            maxJobsToProcess--;

            try
            {
                if (job.rjParams.height > 0 && job.rjParams.height != job.rjParams.observerHeight)
                {
                    height = job.rjParams.height;
                }
                else
                {
                    height = GetSiteHeight(job);
                    if (height < 0)
                    {
                        height = job.rjParams.observerHeight;
                    }
                    job.rjParams.height = height;
                }
                stonestite.Height = "" + height;

                // Max dist based on the highest hill in Europe (Mount Blanc)...
                job.rjParams.maxDistance = (int)GetMaxDistanceForHeight(job.rjParams.maxDistance, height, 4696);


                PaintImage pi = new PaintImage(job.rjParams, true);
                Console.WriteLine("Processing for " + job.rjParams.filename + " on thread " + Thread.CurrentThread.ManagedThreadId + " Jobs Remaining = " + theJobs.Count + " Wait Time " + (DateTime.Now - job.startTime).TotalSeconds + " Height " + height);

                job.startTime = DateTime.Now;
                if (pi.rjParams.description == null)
                {
                    pi.rjParams.description = job.name + ", " + job.typestr + ", " + job.county + ", " + job.lat + " " + job.lon;
                }
                pi.ProcessAllImages(allRunJobs, job.name);
                Console.WriteLine("Completed Job " + job.name + " - " + job.county + " - " + job.typestr + " Time " + (DateTime.Now - job.startTime).TotalSeconds);

                if (job.rjParams.stellarium)
                {
                    ProcessStellariumSite(job.rjParams.replaceFiles, siteFolder, stonestite, job.rjParams);
                }

                if (job.rjParams.PeakProcess || job.rjParams.Correlate || job.rjParams.Skim)
                {
                    PeakProcessAndCorrelate(siteFolder, stonestite, job);
                }
            }
            finally
            {
                if (job.rjParams.LockFiles)
                {
                    RemoveLock(job.rjParams.filename);
                }
            }
        }



        static private void PollJobs()
        {
            SunMoonRunJob job = null;

            byte[] rndbytes = new byte[3];
            System.Security.Cryptography.RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            
            do
            {
                job = null;
                lock (theJobs)
                {
                    if (theJobs.Count > 0)
                    {
                        // Process in a random order - allows multiple instances to run at once without clashing too much.
                        rng.GetBytes(rndbytes);
                        int rInt = rndbytes[0] + rndbytes[1] * 256 + rndbytes[2] * 256 * 256;
                        rInt = rInt % theJobs.Count;

                        job = theJobs[rInt];
                        theJobs.Remove(job);
                    }
                }
                if (job != null)
                {
                    try
                    {
                        ProcessJob(true, job);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Exception caught " + ex.Message + " " + ex.StackTrace + " " + ex.InnerException);
                    }
                }
                else
                {
                    Thread.Sleep(100);
                }
            } while (!CancelJobs && job != null && maxJobsToProcess > 0);

            //ThreadsRunning = false;
        }



        static string CheckSingleHeight(bool xmlout, HorizonVector.HorizonSource src, int count, string shortGridStr, double deltaH, double x, double y, double heightOS, countryEnum eCountry, RunJobParams rjParams)
        {
            OsGridRef osgr = new OsGridRef(x, y);
            string shortGridStr2 = osgr.toString(2);
            if (shortGridStr != shortGridStr2)
            {
                return null;
            }

            LatLon_OsGridRef tmpll = osgr.toLatLon(eCountry);
            double heightLIDAR = LidarBlockReader.GetHeight(false, x, y, false, eCountry, rjParams);
            double delta = (heightOS - heightLIDAR);
            double delta2 = delta;
            if (delta2 < 0) delta2 = -delta2;

            if (heightLIDAR > -9990 && delta2 >= deltaH)
            {
                if (xmlout)
                {
                    string xmlline = "<wpt lat=\"" + tmpll.lat + "\" lon=\"" + tmpll.lon + "\">";
                    xmlline += "<name>" + src + " " + tmpll.lat + "," + tmpll.lon + " Grid Ref " + osgr.toString() + " HeightOS " + heightOS + " HeightLIDAR " + heightLIDAR + " Delta " + delta + "</name>";
                    xmlline += "</wpt>";
                    return xmlline;
                }
                else
                {
                    return "@@@@," + src + "," + count + "," + tmpll.lat + "," + tmpll.lon + "," + y + "," + x + "," + osgr.toString() + "," + heightOS + "," + heightLIDAR + "," + delta + "," + delta2;
                }
            }
            return null;
        }




        static void CreateSRTMTiles(RunJobParams rjParams)
        {
            List<string> allGrids = new List<string>();

            double lat = rjParams.lat;
            double lon = rjParams.lon;

            LatLon_OsGridRef ll = new LatLon_OsGridRef(lat, lon, 0);
            OsGridRef ne = ll.toGrid(rjParams.eCountry);
            OsGridRef ne2 = new OsGridRef(ne.east, ne.north);

            for (double x = -rjParams.CreateSRTMTiles; x < rjParams.CreateSRTMTiles; x += 5000)
            {
                for (double y = -rjParams.CreateSRTMTiles; y < rjParams.CreateSRTMTiles; y += 5000)
                {
                    ne2.east = ne.east + x;
                    ne2.north = ne.north + y;

                    OsGridRef gr = new OsGridRef(ne2.east, ne2.north);
                    string shortGridStr = gr.toString(2);

                    if (!allGrids.Contains(shortGridStr))
                    {
                        allGrids.Add(shortGridStr);
                    }
                }
            }

            ZipMapDataHandler zippy = new ZipMapDataHandler();
            XNamespace xsiNameSpace = "http://www.w3.org/2001/XMLSchema-instance";
            XNamespace osNameSpace = "http://namespaces.ordnancesurvey.co.uk/elevation/contours/v1.0";
            XNamespace gmlNameSpace = "http://www.opengis.net/gml/3.2";
            char[] splitter = { ' ' };

            foreach (string gr in allGrids)
            {
                Console.WriteLine("CreateSRTMTiles Process " + gr);

                XDocument xdoc = zippy.GetOSVectorData(gr, false);
                if (xdoc != null)
                {
                    var allSpotHeights = xdoc.Descendants(osNameSpace + "SpotHeight");
                    if (allSpotHeights == null)
                    {
                        Console.WriteLine("No Stop Heights in " + gr);
                    }

                    foreach (XElement spot in allSpotHeights)
                    {
                        var val = spot.Element(osNameSpace + "geometry");
                        if (val == null) throw new Exception("Null " + osNameSpace + " geometry Value");
                        var loc = val.Value.ToString();

                        val = spot.Element(osNameSpace + "propertyValue");
                        if (val == null) throw new Exception("Null " + osNameSpace + " propertyValue Value");
                        string h = val.Value.ToString();

                        string[] ptStrs = loc.Split(splitter);
                        double x = Convert.ToDouble(ptStrs[0]);
                        double y = Convert.ToDouble(ptStrs[1]);
                        double heightOS = Convert.ToDouble(h);

                        OsGridRef osgr = new OsGridRef(x, y);
                        LatLon_OsGridRef tmpll = osgr.toLatLon(rjParams.eCountry);
                        SRTMTileWriter.AddHeight(tmpll.lat, tmpll.lon, heightOS);
                    }


                    var allContourLines = xdoc.Descendants(osNameSpace + "ContourLine");
                    var allSeaContourLines = xdoc.Descendants(osNameSpace + "LandWaterBoundary");

                    foreach (XElement contour in allSeaContourLines)
                    {
                        var val = contour.Element(osNameSpace + "geometry");
                        if (val == null) throw new Exception("Null " + osNameSpace + " geometry Value");
                        var posList = val.Value.ToString();

                        val = contour.Element(osNameSpace + "propertyValue");
                        if (val == null) throw new Exception("Null " + osNameSpace + " propertyValue Value");
                        var h = val.Value.ToString();
                        double heightOS = (int)Convert.ToDouble(h);

                        string[] ptStrs = posList.Split(splitter);

                        for (int i = 0; i < ptStrs.Length; i += 2)
                        {
                            double x = Convert.ToDouble(ptStrs[i]);
                            double y = Convert.ToDouble(ptStrs[i + 1]);

                            OsGridRef osgr = new OsGridRef(x, y);
                            LatLon_OsGridRef tmpll = osgr.toLatLon(rjParams.eCountry);
                            SRTMTileWriter.AddHeight(tmpll.lat, tmpll.lon, heightOS);
                        }
                    }

                    foreach (XElement contour in allContourLines)
                    {
                        var val = contour.Element(osNameSpace + "geometry");
                        if (val == null) throw new Exception("Null " + osNameSpace + " geometry Value");
                        var posList = val.Value.ToString();

                        val = contour.Element(osNameSpace + "propertyValue");
                        if (val == null) throw new Exception("Null " + osNameSpace + " propertyValue Value");
                        var h = val.Value.ToString();
                        double heightOS = (int)Convert.ToDouble(h);

                        string[] ptStrs = posList.Split(splitter);

                        for (int i = 0; i < ptStrs.Length; i += 2)
                        {
                            double x = Convert.ToDouble(ptStrs[i]);
                            double y = Convert.ToDouble(ptStrs[i + 1]);

                            OsGridRef osgr = new OsGridRef(x, y);
                            LatLon_OsGridRef tmpll = osgr.toLatLon(rjParams.eCountry);
                            SRTMTileWriter.AddHeight(tmpll.lat, tmpll.lon, heightOS);
                        }
                    }
                }
            }

            SRTMTileWriter.ProcessAndSaveTiles(ZipMapDataHandler.GetFolder(false));
        }



        static void CheckLIDAR(bool LIDAR, double deltaH, double lat, double lon, bool spots, bool contours, string filename, countryEnum eCountry, RunJobParams rjParams)
        {
            bool xmlout = false;
            StreamWriter outfile = StreamWriter.Null;
            List<string> lines = new List<string>();

            if (filename != null)
            {
                if (filename.ToLower().EndsWith(".gpx") || filename.ToLower().EndsWith(".xml"))
                {
                    xmlout = true;
                }
                outfile = new StreamWriter(filename);
            }

            if (xmlout)
            {
                lines.Add("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"no\" ?>");
                lines.Add("<gpx>");
            }
            else
            {
                lines.Add("CSVMarker,Source,Count,Lat,Lon,North,East,GridRef,HeightOS,HeightLIDAR,HeightDelta,HeightDelta2");
            }

            LatLon_OsGridRef ll = new LatLon_OsGridRef(lat, lon, 0);
            OsGridRef ne = ll.toGrid(eCountry);
            ne.north = (int)(ne.north + 0.5);
            ne.east = (int)(ne.east + 0.5);


            OsGridRef gr = new OsGridRef(ne.east, ne.north);
            string GridRef = gr.toString();
            string shortGridStr = gr.toString(2);
            if (!LidarBlockReader.Check(false, ne.east, ne.north, false, eCountry, rjParams))
            {
                Console.WriteLine("No LIDAR Data for " + GridRef);
                return;
            }


            ZipMapDataHandler zippy = new ZipMapDataHandler();
            XDocument xdoc = zippy.GetOSVectorData(GridRef, false);
            if (xdoc == null)
            {
                Console.WriteLine("Failed to open OS Map " + GridRef);
                return;
            }

            Console.WriteLine("CheckLIDAR " + lat + "," + lon + " Grid Ref " + GridRef);


            XNamespace xsiNameSpace = "http://www.w3.org/2001/XMLSchema-instance";
            XNamespace osNameSpace = "http://namespaces.ordnancesurvey.co.uk/elevation/contours/v1.0";
            XNamespace gmlNameSpace = "http://www.opengis.net/gml/3.2";
            char[] splitter = { ' ' };

            var allSpotHeights = xdoc.Descendants(osNameSpace + "SpotHeight");
            if (allSpotHeights == null)
            {
                Console.WriteLine("No Stop Heights in " + GridRef);
                return;
            }

            int count = 1;

            if (spots)
            {
                foreach (XElement spot in allSpotHeights)
                {
                    var val = spot.Element(osNameSpace + "geometry");
                    if (val == null) throw new Exception("Null " + osNameSpace + " geometry Value");
                    var loc = val.Value.ToString();

                    val = spot.Element(osNameSpace + "propertyValue");
                    if (val == null) throw new Exception("Null " + osNameSpace + " propertyValue Value");
                    string h = val.Value.ToString();

                    string[] ptStrs = loc.Split(splitter);
                    double x = Convert.ToDouble(ptStrs[0]);
                    double y = Convert.ToDouble(ptStrs[1]);
                    double heightOS = Convert.ToDouble(h);

                    string tmpout = CheckSingleHeight(xmlout, HorizonVector.HorizonSource.OSTrig, count, shortGridStr, deltaH, x, y, heightOS, eCountry, rjParams);
                    if (tmpout != null)
                    {
                        count++;
                        lines.Add(tmpout);
                    }
                }
            }

            if (contours)
            {
                var allContourLines = xdoc.Descendants(osNameSpace + "ContourLine");
                var allSeaContourLines = xdoc.Descendants(osNameSpace + "LandWaterBoundary");

                foreach (XElement contour in allSeaContourLines)
                {
                    var val = contour.Element(osNameSpace + "geometry");
                    if (val == null) throw new Exception("Null " + osNameSpace + " geometry Value");
                    var posList = val.Value.ToString();
                    val = contour.Element(osNameSpace + "propertyValue");
                    if (val == null) throw new Exception("Null " + osNameSpace + " propertyValue Value");
                    var h = val.Value.ToString();
                    double heightOS = (int)Convert.ToDouble(h);

                    string[] ptStrs = posList.Split(splitter);

                    for (int i = 0; i < ptStrs.Length; i += 2)
                    {
                        double x = Convert.ToDouble(ptStrs[i]);
                        double y = Convert.ToDouble(ptStrs[i + 1]);

                        string tmpout = CheckSingleHeight(xmlout, HorizonVector.HorizonSource.OSSeaContour, count, shortGridStr, deltaH, x, y, heightOS, eCountry, rjParams);
                        if (tmpout != null)
                        {
                            count++;
                            lines.Add(tmpout);
                        }

                    }
                }


                foreach (XElement contour in allContourLines)
                {
                    var val = contour.Element(osNameSpace + "geometry");
                    if (val == null) throw new Exception("Null " + osNameSpace + " geometry Value");
                    var posList = val.Value.ToString();

                    val = contour.Element(osNameSpace + "propertyValue");
                    if (val == null) throw new Exception("Null " + osNameSpace + " propertyValue Value");
                    var h = val.Value.ToString();
                    double heightOS = (int)Convert.ToDouble(h);

                    string[] ptStrs = posList.Split(splitter);

                    for (int i = 0; i < ptStrs.Length; i += 2)
                    {
                        double x = Convert.ToDouble(ptStrs[i]);
                        double y = Convert.ToDouble(ptStrs[i + 1]);

                        string tmpout = CheckSingleHeight(xmlout, HorizonVector.HorizonSource.OSLandContour, count, shortGridStr, deltaH, x, y, heightOS, eCountry, rjParams);
                        if (tmpout != null)
                        {
                            count++;
                            lines.Add(tmpout);
                        }
                    }
                }
            }


            if (xmlout)
            {
                lines.Add("</gpx>");
            }

            for (int i = 0; i < lines.Count; i++)
            {
                if (filename != null)
                {
                    outfile.WriteLine(lines[i]);
                }
                else
                {
                    Console.WriteLine(lines[i]);
                }
            }


            if (outfile != null)
            {
                outfile.Close();
            }

            Console.WriteLine("CheckLIDAR Complete");
        }



        static void DumpLIDAR(bool LIDAR, double lat, double lon, bool minecraft, string param, countryEnum eCountry, RunJobParams rjParams)
        {
            Console.WriteLine("DumpLIDAR " + param);

            char[] splitter = { ',' };
            string[] bits = param.Replace(" ", "").Split(splitter);

            LatLon_OsGridRef ll = new LatLon_OsGridRef(lat, lon, 0);
            OsGridRef ne = ll.toGrid(eCountry);
            ne.north = (int)(ne.north + 0.5);
            ne.east = (int)(ne.east + 0.5);

            double incNorth = 1.0;
            double incEast = 1.0;

            int countNorth = 100;
            int countEast = 100;

            if (bits.Length >= 1 && bits[0].Length > 0)
            {
                countNorth = Convert.ToInt32(bits[0]);
                countEast = Convert.ToInt32(bits[0]);
            }

            if (bits.Length >= 2)
            {
                incNorth = Convert.ToDouble(bits[1]);
                incEast = Convert.ToDouble(bits[1]);
            }

            if (bits.Length >= 3)
            {
                countEast = Convert.ToInt32(bits[2]);
            }

            if (bits.Length >= 4)
            {
                incEast = Convert.ToDouble(bits[3]);
            }

            double tmpNorth = ne.north;
            double tmpEast = ne.east;
            double height = 0;

            List<string> lines = new List<string>();
            string tmpline = "";

            for (int y = -countNorth; y <= countNorth; y++)
            {
                tmpNorth = ne.north + y * incNorth;
                tmpline = "" + tmpNorth;

                for (int x = -countEast; x <= countEast; x++)
                {
                    tmpEast = ne.east + x * incEast;

                    height = LidarBlockReader.GetHeight(minecraft, tmpEast, tmpNorth, false, eCountry, rjParams);
                    tmpline += "," + height;
                }
                lines.Insert(0, tmpline);
            }

            Console.WriteLine("Heights at Lat/Lon," + lat + "," + lon + ",North/East," + ne.north + "," + ne.east + ",Increment," + incNorth + "," + incEast);
            Console.Write("North/East");
            for (int x = -countEast; x <= countEast; x++)
            {
                tmpEast = ne.east + x * incEast;
                Console.Write("," + tmpEast);
            }
            Console.WriteLine("");

            for (int i = 0; i < lines.Count; i++)
            {
                Console.WriteLine(lines[i]);
            }

            Console.WriteLine("DumpLIDAR Complete");
        }


        static void DumpMultipleHeights(bool LIDAR, string csvfile, bool proximalInterpolation, countryEnum eCountry, RunJobParams rjParams)
        {
            Console.WriteLine("DumpMultipleHeights " + csvfile);
            char[] splitter = { ',' };

            List<LatLon_OsGridRef> lls = new List<LatLon_OsGridRef>();
            string aline = "";
            int lineno = 1;

            try
            {
                using (StreamReader fr = File.OpenText(csvfile))
                {
                    while (!fr.EndOfStream)
                    {
                        aline = fr.ReadLine();
                        if (aline == null) throw new Exception("Read from " + csvfile + " Filed");
                        string[] parts = aline.Split(splitter);
                        if (parts.Length == 2)
                        {
                            LatLon_OsGridRef alatlon = new LatLon_OsGridRef(0, 0, 0);
                            alatlon.lat = Convert.ToDouble(parts[0]);
                            alatlon.lon = Convert.ToDouble(parts[1]);
                            alatlon.height = ZipMapDataHandler.GetHeightAtLatLon(LIDAR, proximalInterpolation, alatlon.lat, alatlon.lon, eCountry, rjParams);
                            lls.Add(alatlon);
                        }
                        else
                        {
                            Console.WriteLine("Bad CSV line " + aline);
                        }
                        lineno++;
                    }
                    fr.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("DumpMultipleHeights Read " + csvfile + " Line " + aline + " Number " + lineno + " threw Exception " + ex.Message + " " + ex.StackTrace);
            }


            if (rjParams.filename == null)
            {
                foreach (var llh in lls)
                {
                    Console.WriteLine(llh.lat + "," + llh.lon + "," + llh.height);
                }
            }
            else
            {
                Console.WriteLine("DumpMultipleHeights filename = " + rjParams.filename);
                StreamWriter tw = File.CreateText(rjParams.filename);
                if (rjParams.filename.ToLower().Contains(".gpx"))
                {
                    tw.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
                    //<gpx xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns="http://www.topografix.com/GPX/1/1" xmlns:gpxdata="http://www.cluetrust.com/XML/GPXDATA/1/0" xsi:schemaLocation="http://www.topografix.com/GPX/1/1 http://www.topografix.com/GPX/1/1/gpx.xsd http://www.cluetrust.com/XML/GPXDATA/1/0 http://www.cluetrust.com/Schemas/gpxdata10.xsd" version="1.1" creator="http://ridewithgps.com/">
                    //  <metadata>
                    //    <name>coldemadeleine</name>
                    //    <link href="https://ridewithgps.com/routes/30566921">
                    //      <text>coldemadeleine</text>
                    //    </link>
                    //    <time>2019-07-15T21:58:47Z</time>
                    //  </metadata>
                    //  <trk>
                    //    <name>coldemadeleine</name>
                    //    <trkseg>

                    tw.WriteLine("<gpx xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"http://www.topografix.com/GPX/1/1\" xmlns:gpxdata=\"http://www.cluetrust.com/XML/GPXDATA/1/0\" xsi:schemaLocation=\"http://www.topografix.com/GPX/1/1 http://www.topografix.com/GPX/1/1/gpx.xsd http://www.cluetrust.com/XML/GPXDATA/1/0 http://www.cluetrust.com/Schemas/gpxdata10.xsd\" version=\"1.1\" >");
                    tw.WriteLine("  <metadata>");
                    tw.WriteLine("    <name>Panorama Generated GPX</name>");
                    tw.WriteLine("  </metadata>");
                    tw.WriteLine("  <trk>");
                    tw.WriteLine("    <name>Panorama Generated GPX</name>");
                    tw.WriteLine("    <trkseg>");
                    foreach (var llh in lls)
                    {
                        tw.WriteLine("      <trkpt lat=\"" + llh.lat + "\" lon=\"" + llh.lon + "\" >");
                        tw.WriteLine("        <ele>" + llh.height + "</ele>");
                        tw.WriteLine("      </trkpt>");
                    }
                    tw.WriteLine("    </trkseg>");
                    tw.WriteLine("  </trk>");
                    tw.WriteLine("</gpx>");
                }
                else
                {
                    foreach (var llh in lls)
                    {
                        tw.WriteLine(llh.lat + "," + llh.lon + "," + llh.height);
                    }
                }
                tw.Close();
            }
        }


        static void DumpHeights(bool LIDAR, double lat, double lon, bool proximalInterpolation, string param, countryEnum eCountry, RunJobParams rjParams)
        {
            Console.WriteLine("DumpHeights " + param);

            char[] splitter = { ',' };
            string[] bits = param.Replace(" ", "").Split(splitter);

            double inclat = 1.0 / 60 / 60;
            double inclon = 1.0 / 60 / 60;

            int countlat = 10;
            int countlon = 10;

            if (bits.Length >= 1 && bits[0].Length > 0)
            {
                countlat = Convert.ToInt32(bits[0]);
                countlon = Convert.ToInt32(bits[0]);
            }

            if (bits.Length >= 2)
            {
                inclat = Convert.ToDouble(bits[1]);
                inclon = Convert.ToDouble(bits[1]);
            }

            if (bits.Length >= 3)
            {
                countlon = Convert.ToInt32(bits[2]);
            }

            if (bits.Length >= 4)
            {
                inclon = Convert.ToDouble(bits[3]);
            }

            double tmplat = lat;
            double tmplon = lon;
            double height = 0;

            List<string> lines = new List<string>();
            string tmpline = "";

            for (int y = -countlat; y <= countlat; y++)
            {
                tmplat = lat + y * inclat;
                tmpline = "" + tmplat;

                for (int x = -countlon; x <= countlon; x++)
                {
                    tmplon = lon + x * inclon;

                    height = ZipMapDataHandler.GetHeightAtLatLon(LIDAR, proximalInterpolation, tmplat, tmplon, eCountry, rjParams);
                    tmpline += "," + height;
                }
                lines.Insert(0, tmpline);
            }

            Console.WriteLine("Heights at Lat/Lon " + lat + "," + lon + " and Increment " + inclat + "," + inclon);
            Console.Write("Lat/Lon");
            for (int x = -countlon; x <= countlon; x++)
            {
                tmplon = lon + x * inclon;
                Console.Write("," + tmplon);
            }
            Console.WriteLine("");

            for (int i = 0; i < lines.Count; i++)
            {
                Console.WriteLine(lines[i]);
            }

            Console.WriteLine("DumpHeights Complete");
        }

    }
}

