using System;
using System.Collections.Generic;
//using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Proj4Net.Core;
using PanaGraph;
using SkiaSharp;

namespace OpenPanoramaLib
{
    public enum countryEnum { unknown, uk, ie, no };

    public class RunJobParams
    {
        public double lat = -9999; // 54.602849;
        public double lon = -9999; // -3.098584;
        public double height = -999;
        public double[] ages = new double[] { 3750 };
        public string name = null;
        public string filename = null;
        public int pixels = 120;
        public bool contours = false;
        public bool drwSRTM = false;
        public bool drwLIDAR = false;
        public bool drwContours = false;
        public bool drwAllContours = false;
        public bool drawLocations = false;
        public bool drwSpots = false;
        public bool nodrwSpots = false;
        public bool noGrid = false;
        public bool grid = false;
        public bool fineGrid = false;
        public bool allGrids = false;
        public double lidarrange = 10000;
        public bool drwRJs = false;
        public bool sunSolstices = false;
        public bool sunEquinox = false;
        public bool crossQuarters = false;
        public bool moonPlusEPlusI = false;
        public bool moonPlusEMinusI = false;
        public bool moonMinusEPlusI = false;
        public bool moonMinusEMinusI = false;
        public bool stellarium = false;
        public int verticalRange = 20;
        public int negativeRange = 5;
        public bool replaceFiles = false;
        public string mycountry;
        public countryEnum eCountry = countryEnum.uk;   // Default country for OS Grid Refs - UK.
        public string myCountryLong;
        public string match;
        public string region;
        public double heightfilter = 0;
        public string theCountyFile;
        public string theCountyHTML;
        public bool dumpLocationGPXs;
        public bool createTemplate;
        public bool createHorizon;
        public bool recreateHorizon;
        public bool createHorizonCSV;
        public bool createPassingPoints;
        public bool createMultipleRes;
        public string csvinfile;
        public string jsonfile;
        public string sidfiles;
        public int maxDistance = 10000;
        public int minDistance = 50;
        public string TemplateFile;
        public string description = null;
        public string[] csvBulkFiles = null;
        public int numThreads = 1;
        public string dumpHeights = null;
        public string dumpMultipleHeights = null;
        public string dumpLIDAR = null;
        public bool LIDAROptim = false;
        public int maxJobs = 9999999;
        public bool proximalInterpolation = false;
        public double observerHeight = 1.5;
        public string[] siteTypes = new string[] { "Stone Circle", "Ring Cairn", "Standing Stone (Menhir)", "Standing Stones", "Stone Row / Alignment", "Henge", "Multiple Stone Rows / Avenue", "Timber Circle", "Viewpoint" };
        public string blobStoreURL = "https://standingstonesorg.blob.core.windows.net/";
        public string viewGPSURL = "https://gpsvisualizer.com/atlas/map?url=";
        public string viewImageURL = "https://www.standingstones.org/viewer.html?name=";
        public string megpURL = "https://www.megalithic.co.uk/article.php?sid=";
        public string standStonesHost = "www.standingstones.org";
        public string updateJPG = "wip.jpg";
        public int updateInterval = 0;
        public bool wireframe = false;
        public bool allWireframes = false;
        public double checkLIDAR = -99999;
        public string checkFile = null;
        public string LASFile = null;
        public double LIDARRes = 1.0;
        public string ASCFile = null;
        public double ASCSize = 0; // 0KM square by default - default to whatever the cloud size is.
        public bool CreateLIDARCache = false;
        public double CreateSRTMTiles = -1;
        public bool LockFiles = true;
        public bool LidarDebug = false;
        public bool HiResHorizon = false;
        public bool PeakProcess = false;
        public bool PeakCSV = false;
        public bool PeakJSON = false;
        public bool DeclinationPeakJSON = false;
        public bool DeclinationPeakCSV = false;
        public bool SaveDeclinPNG = false;
        public double minPPAzDistance = 20;
        public double minPPElevDistance = 0.066;
        public double minPPDistance = 1000;
        public bool Correlate = false;
        public bool CorrelateCSV = false;
        public bool CorrelateJSON = false;
        public bool Skim = false;
        public bool SkimCSV = false;
        public bool SkimJSON = false;
        public double minSkimDistance = 700;
        public double minSkimAzDelta = 0.15;

        public double minCorrelationValue = 0.5;
        public double minCorrelationValueSlopeWeighted = 0;
        public double minCorrelationValueElevDiffWeighted = 0;
        public double minCorrelationValueWeighted = 0.2;

        public ICoordinateTransform FromLLtrans = null;
        public ICoordinateTransform ToLLTrans = null;


        //public RunJobParams()
        //{
        //}

        public RunJobParams(string[] args)
        {
            char[] splitterCSV = new char[] { ',' };
            string param;

            GetLocationTransforms();

            for (int i = 0; i < args.Count(); i++)
            {
                int cur_param = i;

                try
                {
                    bool found = true;
                    param = args[i];

                    switch (args[i])
                    {
                        case "-drawJobs":
                            drwRJs = true;
                            break;

                        case "-lidar":
                            drwLIDAR = true;
                            break;

                        case "-r":
                            replaceFiles = true;
                            break;

                        case "-locs":
                            dumpLocationGPXs = true;
                            break;

                        case "-t":
                            createTemplate = true;
                            break;

                        case "-mr":
                            createMultipleRes = true;
                            break;

                        case "-ch":
                            createHorizon = true;
                            break;

                        case "-rch":
                            recreateHorizon = true;
                            break;

                        case "-hzCSV":
                            createHorizonCSV = true;
                            break;

                        case "-pp":
                            createPassingPoints = true;
                            break;

                        case "-labels":
                            drawLocations = true;
                            break;

                        case "-spots":
                            drwSpots = true;
                            break;

                        case "-nospots":
                            nodrwSpots = true;
                            break;

                        case "-contours":
                            drwContours = true;
                            break;

                        case "-allContours":
                            drwAllContours = true;
                            break;

                        case "-srtm":
                            drwSRTM = true;
                            break;

                        case "-grid":
                            grid = true;
                            break;

                        case "-fgrid":
                            fineGrid = true;
                            break;

                        case "-nogrid":
                            noGrid = true;
                            break;

                        case "-allgrids":
                            allGrids = true;
                            noGrid = true;
                            fineGrid = true;
                            grid = true;
                            break;

                        case "-stellarium":
                            stellarium = true;
                            createTemplate = true;
                            break;

                        case "-Solstice":
                            sunSolstices = true;
                            break;

                        case "-Equinox":
                            sunEquinox = true;
                            break;

                        case "-xq":
                            crossQuarters = true;
                            break;

                        case "-AllSun":
                            sunSolstices = true;
                            sunEquinox = true;
                            crossQuarters = true;
                            break;

                        case "-Moon+e+i":
                            moonPlusEPlusI = true;
                            break;

                        case "-Moon+e-i":
                            moonPlusEMinusI = true;
                            break;

                        case "-Moon-e+i":
                            moonMinusEPlusI = true;
                            break;

                        case "-Moon-e-i":
                            moonMinusEMinusI = true;
                            break;

                        case "-AllMoon":
                            moonPlusEPlusI = true;
                            moonPlusEMinusI = true;
                            moonMinusEPlusI = true;
                            moonMinusEMinusI = true;
                            break;

                        case "-LIDAROptim":
                            LIDAROptim = true;
                            break;

                        case "-ProximalInterpolation":
                            proximalInterpolation = true;
                            break;

                        case "-Wireframe":
                            wireframe = true;
                            break;

                        case "-AllWireframes":
                            allWireframes = true;
                            break;

                        case "-CreateLIDARCache":
                            CreateLIDARCache = true;
                            break;

                        case "-LidarDebug":
                            LidarDebug = true;
                            break;

                        case "-NoLock":
                            LockFiles = false;
                            break;

                        case "-HiResHorizon":
                            HiResHorizon = true;
                            break;

                        case "-PP":
                            PeakProcess = true;
                            break;

                        case "-PPCSV":
                            PeakCSV = true;
                            break;

                        case "-PPJSON":
                            PeakJSON = true;
                            break;

                        case "-DeclinationPeakJSON":
                            DeclinationPeakJSON = true;
                            break;

                        case "-DeclinationPeakCSV":
                            DeclinationPeakCSV = true;
                            break;

                        case "-SaveDeclinPNG":
                            SaveDeclinPNG = true;
                            break;

                        case "-Correlate":
                            Correlate = true;
                            break;

                        case "-CorrelateCSV":
                            CorrelateCSV = true;
                            break;

                        case "-CorrelateJSON":
                            CorrelateJSON = true;
                            break;

                        case "-Skim":
                            Skim = true;
                            break;
                        case "-SkimCSV":
                            SkimCSV = true;
                            break;
                        case "-SkimJSON":
                            SkimJSON = true;
                            break;

                        case "-NoJPGPNG":
                            PanaGraph.Image.jPGSavePNG = false;
                            break;

                        case "-CacheClean":
                            ZipMapDataHandler.SetAggressiveCacheClean(true);
                            break;

                        default:
                            found = false;
                            break;
                    }


                    if (!found && i < (args.Count() - 1))
                    {
                        found = true;

                        switch (args[i])
                        {
                            case "-PosRange":
                                verticalRange = Convert.ToInt32(args[++i]);
                                break;

                            case "-NegRange":
                                negativeRange = Convert.ToInt32(args[++i]);
                                break;

                            case "-lidarfolder":
                                LidarBlockReader.SelectFolder(args[++i]);
                                break;

                            case "-lidarcache":
                                LidarBlockReader.SelectCacheFolder(args[++i]);
                                break;

                            case "-lidarrange":
                                lidarrange = Convert.ToDouble(args[++i]);
                                break;

                            case "-f":
                                filename = args[++i].Replace(",", "");
                                break;

                            case "-Q":
                                PanaGraph.Image.jPGQuality = Convert.ToInt32(args[++i]);
                                break;

                            case "-name":
                                name = args[++i].Replace(",", "");
                                break;

                            case "-country":
                                mycountry = args[++i].ToLower();
                                eCountry = GridRefParam.getCountry(mycountry);
                                GetLocationTransforms();
                                break;

                            case "-CountryLong":
                                myCountryLong = args[++i].ToLower();
                                break;

                            case "-match":
                                match = args[++i].ToLower().Replace("-", "").Replace(" ", "");
                                break;

                            case "-region":
                                region = args[++i].ToLower().Replace("-", "").Replace(" ", "");
                                break;

                            case "-hf":
                                heightfilter = Convert.ToDouble(args[++i]);
                                break;

                            case "-county":
                                theCountyFile = args[++i].ToLower();
                                break;

                            case "-countyHTML":
                                theCountyHTML = args[++i].ToLower();
                                break;

                            case "-out":
                                PaintImage.outfolder = args[++i].ToLower();
                                break;

                            case "-srtmFolder":
                                ZipMapDataHandler.SelectFolder(args[++i], false);
                                break;

                            case "-osFolder":
                                ZipMapDataHandler.SelectFolder(args[++i], true);
                                break;

                            case "-p":
                                pixels = Convert.ToInt32(args[++i]);
                                break;

                            case "-CacheLIDAR":
                                LidarBlockReader.SetLIDARCacheSize(Convert.ToInt32(args[++i]));
                                break;

                            case "-CacheSRTM":
                                SRTMClass.SetSRTMCacheSize(Convert.ToInt32(args[++i]));
                                break;

                            case "-CacheOSMap":
                                ZipMapDataHandler.SetOSMapCacheSize(Convert.ToInt32(args[++i]));
                                break;

                            case "-oldcsv":
                                csvinfile = args[++i];
                                break;

                            case "-json":
                                jsonfile = args[++i].ToLower();
                                break;

                            case "-SidFiles":
                                sidfiles = args[++i].ToLower();
                                break;

                            case "-max":
                                maxDistance = Convert.ToInt32(args[++i]);
                                break;

                            case "-min":
                                minDistance = Convert.ToInt32(args[++i]);
                                break;

                            case "-l":
                                var ll = LatLon_OsGridRef.GetLLFromLoc(args[++i], eCountry);
                                lat = ll.lat;
                                lon = ll.lon;
                                if (height < 0)
                                {
                                    height = ZipMapDataHandler.GetHeightAtLatLon(drwLIDAR, proximalInterpolation, lat, lon, eCountry, this) + observerHeight;
                                    if (height < 2)
                                    {
                                        height = observerHeight;
                                    }
                                }
                                break;

                            case "-ObserverHeight":
                                observerHeight = Convert.ToDouble(args[++i]);
                                break;

                            case "-a":
                                string[] agesstr = args[++i].Split(splitterCSV);
                                ages = new double[agesstr.Length];
                                for (int j = 0; j < agesstr.Length; j++)
                                {
                                    ages[j] = Convert.ToDouble(agesstr[j]);
                                }
                                break;

                            case "-tf":
                                TemplateFile = args[++i];
                                break;

                            case "-d":
                                description = args[++i];
                                break;

                            case "-h":
                                height = Convert.ToDouble(args[++i]);
                                break;

                            case "-csv":
                                csvBulkFiles = args[++i].Split(splitterCSV);
                                break;

                            case "-threads":
                                numThreads = Convert.ToInt32(args[++i]);
                                break;

                            case "-ColourSea":
                                Color col = PaintImage.GetColor(args[++i]);
                                PaintImage.ColourSea1 = col;
                                PaintImage.ColourSea2 = col;
                                break;

                            case "-ColourSea1":
                                PaintImage.ColourSea1 = PaintImage.GetColor(args[++i]);
                                break;

                            case "-ColourSea2":
                                PaintImage.ColourSea2 = PaintImage.GetColor(args[++i]);
                                break;

                            case "-ColourSky":
                                PaintImage.ColourSky = PaintImage.GetColor(args[++i]);
                                break;

                            case "-ColourBase":
                                PaintImage.ColourBase = PaintImage.GetColor(args[++i]);
                                break;

                            case "-ColourTops":
                                PaintImage.ColourTops = PaintImage.GetColor(args[++i]);
                                break;

                            case "-ColourDistant":
                                PaintImage.ColourDistant = PaintImage.GetColor(args[++i]);
                                break;

                            case "-ColourOriginLatitude":
                                PaintImage.ColourOriginLatitude = Convert.ToInt32(args[++i]);
                                break;

                            case "-ColourSlopeAdjust":
                                PaintImage.ColourSlopeAdjust = Convert.ToInt32(args[++i]);
                                break;

                            case "-ColourMaxDist":
                                PaintImage.ColourMaxDist = Convert.ToInt32(args[++i]);
                                break;

                            case "-ColourGreyCount":
                                PaintImage.ColourGreyCount = Convert.ToInt32(args[++i]);
                                break;

                            case "-ColourGreenBrownShades":
                                PaintImage.ColourGreenBrownShades = Convert.ToInt32(args[++i]);
                                break;

                            case "-ColourMaxHeight":
                                PaintImage.ColourMaxHeight = Convert.ToInt32(args[++i]);
                                break;

                            case "-ColourMoon":
                                PaintImage.ColourMoon = PaintImage.GetColor(args[++i]);
                                break;

                            case "-ColourSun":
                                PaintImage.ColourSun = PaintImage.GetColor(args[++i]);
                                break;

                            case "-ColourSaMText":
                                PaintImage.ColourSaMText = PaintImage.GetColor(args[++i]);
                                break;

                            case "-ColourTitle":
                                PaintImage.ColourTitle = PaintImage.GetColor(args[++i]);
                                break;

                            case "-ColourCairns":
                                PaintImage.ColourCairns = PaintImage.GetColor(args[++i]);
                                break;

                            case "-ColourStones":
                                PaintImage.ColourStones = PaintImage.GetColor(args[++i]);
                                break;

                            case "-ColourSpotHeight":
                                PaintImage.ColourSpotHeight = PaintImage.GetColor(args[++i]);
                                break;

                            case "-ColourReticleBackground":
                                PaintImage.ColourReticleBackground = PaintImage.GetColor(args[++i]);
                                break;

                            case "-ColourReticleMajor":
                                PaintImage.ColourReticleMajor = PaintImage.GetColor(args[++i]);
                                break;

                            case "-ColourReticleMinor":
                                PaintImage.ColourReticleMinor = PaintImage.GetColor(args[++i]);
                                break;

                            case "-ColourReticleHorizontal":
                                PaintImage.ColourReticleHorizontal = PaintImage.GetColor(args[++i]);
                                break;

                            case "-ColourReticleText":
                                PaintImage.ColourReticleText = PaintImage.GetColor(args[++i]);
                                break;

                            case "-ColourContrast":
                                PaintImage.ColourContrast = Convert.ToDouble(args[++i]);
                                break;

                            case "-CopyrightNotice":
                                PaintImage.CopyrightNotice = args[++i];
                                break;

                            case "-DumpHeights":
                                dumpHeights = args[++i];
                                break;

                            case "-DumpMultipleHeights":
                                dumpMultipleHeights = args[++i];
                                break;

                            case "-DumpLIDAR":
                                dumpLIDAR = args[++i];
                                break;

                            case "-CheckLIDAR":
                                checkLIDAR = Convert.ToDouble(args[++i]);
                                break;

                            case "-CheckFile":
                                checkFile = args[++i];
                                break;

                            case "-MaxJobs":
                                maxJobs = Convert.ToInt32(args[++i]);
                                break;

                            case "-SiteTypes":
                                siteTypes = args[++i].Split(splitterCSV);
                                break;

                            case "-BlobStoreURL":
                                blobStoreURL = args[++i];
                                break;

                            case "-ViewGPSURL":
                                viewGPSURL = args[++i];
                                break;

                            case "-ViewImageURL":
                                viewImageURL = args[++i];
                                break;

                            case "-MegpURL":
                                megpURL = args[++i];
                                break;

                            case "-StandStonesHost":
                                standStonesHost = args[++i];
                                break;

                            case "-UpdateJPG":
                                updateJPG = args[++i];
                                break;

                            case "-UpdateInterval":
                                updateInterval = Convert.ToInt32(args[++i]);
                                break;

                            case "-LIDARSize":
                                LidarBlockReader.SetLidarSiz(Convert.ToInt32(args[++i]));
                                break;

                            case "-LIDARRes":
                                LIDARRes = Convert.ToDouble(args[++i]);
                                LidarBlockReader.SetLidarRes(LIDARRes);
                                break;

                            case "-LAS":
                                LASFile = args[++i];
                                break;

                            case "-ASCFile":
                                ASCFile = args[++i];
                                break;

                            case "-ASCSize":
                                ASCSize = Convert.ToDouble(args[++i]);
                                break;

                            case "-CreateSRTMTiles":
                                CreateSRTMTiles = Convert.ToDouble(args[++i]);
                                break;

                            case "-minPPAzDistance":
                                minPPAzDistance = Convert.ToDouble(args[++i]);
                                break;
                            case "-minPPElevDistance":
                                minPPElevDistance = Convert.ToDouble(args[++i]);
                                break;
                            case "-minPPDistance":
                                minPPDistance = Convert.ToDouble(args[++i]);
                                break;

                            case "-minCorrelationValue":
                                minCorrelationValue = Convert.ToDouble(args[++i]);
                                break;
                            case "-minCorrelationValueSlopeWeighted":
                                minCorrelationValueSlopeWeighted = Convert.ToDouble(args[++i]);
                                break;
                            case "-minCorrelationValueElevDiffWeighted":
                                minCorrelationValueElevDiffWeighted = Convert.ToDouble(args[++i]);
                                break;
                            case "-minCorrelationValueWeighted":
                                minCorrelationValueWeighted = Convert.ToDouble(args[++i]);
                                break;

                            case "-minSkimDistance":
                                minSkimDistance = Convert.ToDouble(args[++i]);
                                break;
                            case "-minSkimAzDelta":
                                minSkimAzDelta = Convert.ToDouble(args[++i]);
                                break;

                            default:
                                found = false;
                                string sggsgs = args[i];
                                break;
                        }
                    }

                    if (!allGrids && !noGrid && !fineGrid && !grid)
                    {
                        noGrid = true;
                    }

                    if (!found)
                    {
                        throw (new Exception("Invalid Parameter Command Line Parameter " + i + " " + " " + args[i]));
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception - Bad Parameter " + args[cur_param] + " caught " + ex.Message + " " + ex.StackTrace + " " + ex.InnerException);
                    throw;
                }
            }

            GetLocationTransforms();
        }

        protected RunJobParams(RunJobParams other)
        {
            // Cloning code goes here...
            lat = other.lat;
            lon = other.lon;
            height = other.height;
            ages = other.ages;
            filename = other.filename;
            name = other.name;
            pixels = other.pixels;
            contours = other.contours;
            drwSRTM = other.drwSRTM;
            drwLIDAR = other.drwLIDAR;
            drwContours = other.drwContours;
            drwAllContours = other.drwAllContours;
            drawLocations = other.drawLocations;
            drwSpots = other.drwSpots;
            nodrwSpots = other.nodrwSpots;
            noGrid = other.noGrid;
            grid = other.grid;
            fineGrid = other.fineGrid;
            allGrids = other.allGrids;
            lidarrange = other.lidarrange;
            drwRJs = other.drwRJs;
            sunSolstices = other.sunSolstices;
            sunEquinox = other.sunEquinox;
            crossQuarters = other.crossQuarters;
            moonPlusEPlusI = other.moonPlusEPlusI;
            moonPlusEMinusI = other.moonPlusEMinusI;
            moonMinusEPlusI = other.moonMinusEPlusI;
            moonMinusEMinusI = other.moonMinusEMinusI;
            stellarium = other.stellarium;
            verticalRange = other.verticalRange;
            negativeRange = other.negativeRange;
            replaceFiles = other.replaceFiles;
            mycountry = other.mycountry;
            eCountry = other.eCountry;
            myCountryLong = other.myCountryLong;
            match = other.match;
            region = other.region;
            heightfilter = other.heightfilter;
            theCountyFile = other.theCountyFile;
            theCountyHTML = other.theCountyHTML;
            dumpLocationGPXs = other.dumpLocationGPXs;
            createTemplate = other.createTemplate;
            createMultipleRes = other.createMultipleRes;
            csvinfile = other.csvinfile;
            jsonfile = other.jsonfile;
            sidfiles = other.sidfiles;
            maxDistance = other.maxDistance;
            minDistance = other.minDistance;
            TemplateFile = other.TemplateFile;
            description = other.description;
            csvBulkFiles = other.csvBulkFiles;
            numThreads = other.numThreads;
            createHorizon = other.createHorizon;
            recreateHorizon = other.recreateHorizon;
            createHorizonCSV = other.createHorizonCSV;
            createPassingPoints = other.createPassingPoints;
            LIDAROptim = other.LIDAROptim;
            maxJobs = other.maxJobs;
            proximalInterpolation = other.proximalInterpolation;
            observerHeight = other.observerHeight;
            siteTypes = other.siteTypes;
            blobStoreURL = other.blobStoreURL;
            viewGPSURL = other.viewGPSURL;
            viewImageURL = other.viewImageURL;
            megpURL = other.megpURL;
            standStonesHost = other.standStonesHost;
            updateJPG = other.updateJPG;
            updateInterval = other.updateInterval;
            wireframe = other.wireframe;
            allWireframes = other.allWireframes;
            dumpHeights = other.dumpHeights;
            dumpMultipleHeights = other.dumpMultipleHeights;
            dumpLIDAR = other.dumpLIDAR;
            checkLIDAR = other.checkLIDAR;
            checkFile = other.checkFile;
            CreateLIDARCache = other.CreateLIDARCache;
            CreateSRTMTiles = other.CreateSRTMTiles;
            LockFiles = other.LockFiles;
            LidarDebug = other.LidarDebug;

            HiResHorizon = other.HiResHorizon;
            PeakProcess = other.PeakProcess;
            PeakCSV = other.PeakCSV;
            PeakJSON = other.PeakJSON;
            DeclinationPeakJSON = other.DeclinationPeakJSON;
            DeclinationPeakCSV = other.DeclinationPeakCSV;
            SaveDeclinPNG = other.SaveDeclinPNG;
            minPPAzDistance = other.minPPAzDistance;
            minPPElevDistance = other.minPPElevDistance;
            minPPDistance = other.minPPDistance;

            LASFile = other.LASFile;
            ASCFile = other.ASCFile;
            LIDARRes = other.LIDARRes;

            Correlate = other.Correlate;
            CorrelateCSV = other.CorrelateCSV;
            CorrelateJSON = other.CorrelateJSON;

            Skim = other.Skim;
            SkimCSV = other.SkimCSV;
            SkimJSON = other.SkimJSON;

            minSkimDistance = other.minSkimDistance;
            minSkimAzDelta = other.minSkimAzDelta;

            minCorrelationValue = other.minCorrelationValue;
            minCorrelationValueSlopeWeighted = other.minCorrelationValueSlopeWeighted;
            minCorrelationValueElevDiffWeighted = other.minCorrelationValueElevDiffWeighted;
            minCorrelationValueWeighted = other.minCorrelationValueWeighted;


            ToLLTrans = other.ToLLTrans;
            FromLLtrans = other.FromLLtrans;
        }


        public void GetLocationTransforms()
        {
            const string llName = "EPSG:4326"; // WGS 84 - Latitude Longitude - GPS System.
            const string UKLidarName = "EPSG:27700"; // UK Grid
            const string NorwayLidarName = "EPSG:25833"; // EPSG:25833 - ETRS89 / UTM zone 33N;
            const string IELidarName = "EPSG:2157"; // "IRENET95"; // Ireland - no idea if this is correct or not.

            string LidarName = UKLidarName;

            switch (eCountry)
            {
                case countryEnum.uk:
                    LidarName = UKLidarName;
                    break;
                case countryEnum.ie:
                    LidarName = IELidarName;
                    break;
                case countryEnum.no:
                    LidarName = NorwayLidarName;
                    break;
            }

            CoordinateTransformFactory ctFactory = new CoordinateTransformFactory();
            CoordinateReferenceSystemFactory csFactory = new CoordinateReferenceSystemFactory();

            CoordinateReferenceSystem Lidarcrs = csFactory.CreateFromName(LidarName);
            CoordinateReferenceSystem llcrs = csFactory.CreateFromName(llName);
            FromLLtrans = ctFactory.CreateTransform(llcrs, Lidarcrs);
            ToLLTrans = ctFactory.CreateTransform(Lidarcrs, llcrs);
        }


        public RunJobParams Clone()
        {
            return new RunJobParams(this);
        }

        public static void Usage(string versionstr)
        {
            Console.WriteLine("Panorama version " + versionstr);
            Console.WriteLine("\t-drawJobs (draw cairns, stones etc)");
            Console.WriteLine("\t-PosRange <angle> - Draw degrees above horizon");
            Console.WriteLine("\t-NegRange <angle> - Draw degrees below horizon");
            Console.WriteLine("\t-lidar (Process LIDAR data for locations");
            Console.WriteLine("\t-lidarfolder (Location where the LIDAR source data is stored (ZIPs and ASC files)");
            Console.WriteLine("\t-lidarcache (Location to save processed binary LIDAR cache data for later use");
            Console.WriteLine("\t-lidarrange MaxDistance (Maximum distance (m) from observer to use LIDAR data (default 10000)");
            Console.WriteLine("\t-f OutputFilename");
            Console.WriteLine("\t-name <string> - sitename");
            Console.WriteLine("\t-r (replace file)");
            Console.WriteLine("\t-country <xx> - short country code, e.g. FR for France");
            Console.WriteLine("\t-CountryLong <country>  - long country name, e.g. France");
            Console.WriteLine("\t-match <sitename>");
            Console.WriteLine("\t-region <region>");
            Console.WriteLine("\t-hf <height> - Height Filter");
            Console.WriteLine("\t-county <CountyFile>");
            Console.WriteLine("\t-countyHTML <CountyFileHTML>");
            Console.WriteLine("\t-locs (dump location GPX files)");
            Console.WriteLine("\t-mr - Create multiple resolutions of images (30, 60 pixels per degree)");
            Console.WriteLine("\t-ch - Save Horizon GPX File");
            Console.WriteLine("\t-hzCSV - Save Horizon CSV File");
            Console.WriteLine("\t-pp - Create Passing Points, where the sun/moon cross the horizon");
            Console.WriteLine("\t-labels (spot height labels)");
            Console.WriteLine("\t-spots (Add OS spot markers)");
            Console.WriteLine("\t-nospots (Don't add OS spot markers)");
            Console.WriteLine("\t-contours (Add OS Contours)");
            Console.WriteLine("\t-allContours (Draw All OS Contours overlapping LIDAR)");
            Console.WriteLine("\t-srtm (Use SRTM Data)");
            Console.WriteLine("\t-grid (add a grid)");
            Console.WriteLine("\t-fgrid (fine grid)");
            Console.WriteLine("\t-allgrids (Create files with no grid, grid and fine grid in one go)");
            Console.WriteLine("\t-stellarium (Generate Stellarium ZIP file)");
            Console.WriteLine("\t-out OutputFolder");
            Console.WriteLine("\t-srtmFolder <folder> - SRTM Source folder name, e.g. %USERPROFILE%/Appdata/roaming/SRTM Data");
            Console.WriteLine("\t-osFolder <folder> - OS Maps 50M data FOlder e.g. %USERPROFILE%/Appdata/roaming/OS Terrain 50");
            Console.WriteLine("\t-p Pixels (pixels per degree - default 120)");
            Console.WriteLine("\t-oldcsv <oldcsvfile> - Not really used any more");
            Console.WriteLine("\t-json JSONOutFile (Create JSON File)");
            Console.WriteLine("\t-SidFiles SidFilePath - Create Sid Files in folder");
            Console.WriteLine("\t-max MaxDistance in M");
            Console.WriteLine("\t-min MinDistance in M (70)");
            Console.WriteLine("\t-l Location");
            Console.WriteLine("\t-a age");
            Console.WriteLine("\t-t (Create Template file)");
            Console.WriteLine("\t-tf InputTemplateFilename");
            Console.WriteLine("\t-d Description");
            Console.WriteLine("\t-h Height");
            Console.WriteLine("\t-csv CSVFiles (Bulk process from a list of CSV files)");
            Console.WriteLine("\t-threads N (Number of threads to run - don't use as it slows down drawing)");

            Console.WriteLine("\t-Solstice - Draw Sun Solstices");
            Console.WriteLine("\t-Equinox - Draw Sun Equinox");
            Console.WriteLine("\t-xq - Draw Sun Cross Quarters");
            Console.WriteLine("\t-Moon+e+i - Draw Moon +e+i");
            Console.WriteLine("\t-Moon+e-i - Draw Moon +e-i");
            Console.WriteLine("\t-Moon-e+i - Draw Moon -e+i");
            Console.WriteLine("\t-Moon-e-i - Draw Moon -e-i");
            Console.WriteLine("\t-AllSun - Draw all Sun positions");
            Console.WriteLine("\t-AllMoon - Draw all Moon positions");

            Console.WriteLine("\t-CacheLIDAR N (Number of LIDAR tiles to cache (400MB per tile, default 4)");
            Console.WriteLine("\t-CacheSRTM N (Number of SRTM tiles to cache (26MB per tile, default 12)");
            Console.WriteLine("\t-CacheOSMap N (Number of OS Map tiles to cache (few MB each, default 500)");
            Console.WriteLine("\t-CacheClean - Clean caches frequently and also run GC to reduce memory usage");
            Console.WriteLine("\t-LIDARSize N (Size of LIDAR blocks in M (default 10000)");
            Console.WriteLine("\t-LIDAROptim Attempt to optimise some LIDAR drawing (don't use)");
            Console.WriteLine("\t-CreateLIDARCache");
            Console.WriteLine("\t-Wireframe - Switch to wireframe drawing mode");
            Console.WriteLine("\t-AllWireframes - Draw all wireframes and ignore hidden surface removal");

            

            Console.WriteLine("\t-ColourSea <RGB> - Default is Hex RGB (e.g. 000000) for Black");
            Console.WriteLine("\t-ColourSea1 <RGB> - Default is Hex RGB (e.g. 000000) for Black");
            Console.WriteLine("\t-ColourSea2 <RGB> - Default is Hex RGB (e.g. 000000) for Black");
            Console.WriteLine("\t-ColourSky <RGB> - Default is Hex RGB for Blue");
            Console.WriteLine("\t-ColourBase <RGB> - Default is Hex RGB for DarkGreen");
            Console.WriteLine("\t-ColourTops <RGB> - Default is Hex RGB for Brown");
            Console.WriteLine("\t-ColourDistant <RGB> - Default is Hex RGB for DimGray");
            Console.WriteLine("\t-ColourOriginLatitude <latitude> - default 45 ");
            Console.WriteLine("\t-ColourSlopeAdjust <int> - default 20");
            Console.WriteLine("\t-ColourMaxDist <metres> - default 10k");
            Console.WriteLine("\t-ColourGreenBrownShades <count) - default 40");
            Console.WriteLine("\t-ColourGreyCount <count) - default 80");
            Console.WriteLine("\t-ColourMaxHeight <height> - max height colour in metres");
            Console.WriteLine("\t-ColourMoon <RGB> - Default is Hex RGB for Grey");
            Console.WriteLine("\t-ColourSun <RGB> - Default is Hex RGB for Yellow");
            Console.WriteLine("\t-ColourSaMText <RGB> - Sun and Moon Text - Default is Hex RGB for Black");
            Console.WriteLine("\t-ColourTitle <RGB> - Default is Hex RGB for White");
            Console.WriteLine("\t-ColourCairns <RGB> - Default is Hex RGB for Red");
            Console.WriteLine("\t-ColourStones <RGB> - Default is Hex RGB for DarkGrey");
            Console.WriteLine("\t-ColourSpotHeight <RGB> - Default is Hex RGB for Dark Red");
            Console.WriteLine("\t-ColourReticleBackground <RGB> - Default is Hex RGB for White");
            Console.WriteLine("\t-ColourReticleMajor <RGB> - Default is Hex RGB for Dark Grey");
            Console.WriteLine("\t-ColourReticleMinor <RGB> - Default is Hex RGB for Grey");
            Console.WriteLine("\t-ColourReticleHorizontal <RGB> - Default is Hex RGB for Black");
            Console.WriteLine("\t-ColourReticleText <RGB> - Default is Hex RGB for Black");
            Console.WriteLine("\t-ColourContrast 1.3 - Default is 1.0 - above 1.0 to increase contrast for slopes");
            Console.WriteLine("\t-CopyrightNotice <Text> - Change Copyright Notice Text");
            Console.WriteLine("\t-DumpHeights <string> - Where string is count,increment[,longitudecount,longitudeincrement]");
            Console.WriteLine("\t-DumpMultipleHeights <latlon.csv> - Where string is a file of latitude and longitudes");
            Console.WriteLine("\t-DumpLIDAR <string> - Where string is count,increment[,eastcount,eastincrement]");
            Console.WriteLine("\t-CheckLIDAR - Read a LIDAR Block and OS Map Data block and check all spot heights against the LIDAR height");
            Console.WriteLine("\t-LidarDebug - Enable debug mode of LIDAR data");
            Console.WriteLine("\t-MaxJobs <number> - Stop processesing after at most N jobs");
            Console.WriteLine("\t-ProximalInterpolation - Perform Proximal Interpolation rather than Linear (Minecraft mode)");
            Console.WriteLine("\t-ObserverHeight <height> - Observer eye height (1.5m)");
            Console.WriteLine("\t-SiteTypes = <types> - where default types are \"Stone Circle,Ring Cairn,Standing Stone(Menhir),Standing Stones,Stone Row / Alignment,Henge,Multiple Stone Rows / Avenue,Timber Circle,Viewpoint\"");

            Console.WriteLine("\t-UpdateJPG <string> - Work in progress image (wip.jpg)");
            Console.WriteLine("\t-UpdateInterval <number> - Work in progress interval secs (default never)");
            Console.WriteLine("\t-NoLock - Do not create lock files");

            Console.WriteLine("\t-BlobStoreURL <URL> - where URL default is https://standingstonesorg.blob.core.windows.net/");
            Console.WriteLine("\t-ViewGPSURL <URL> - where URL default is https://gpsvisualizer.com/atlas/map?url=");
            Console.WriteLine("\t-ViewImageURL <URL> - where URL default is https://www.standingstones.org/viewer.html?name=");
            Console.WriteLine("\t-MegpURL <URL> - where URL default is https://www.megalithic.co.uk/article.php?sid=");
            Console.WriteLine("\t-StandStonesHost <Host> - where host default is www.standingstones.org");
            Console.WriteLine("\t-LAS File - Process the LAS File");
            Console.WriteLine("\t-ASCFile file - Output patch ASC file from LAS");
            Console.WriteLine("\t-ASCSize distance - Size of patch file to create in M from center - tile size is 2*distance");
            Console.WriteLine("\t-CreateSRTMTiles <distance> - Create SRTM Tiles for location");

            Console.WriteLine("\t-HiResHorizon - Save High Resolution Horizon");

            Console.WriteLine("\t-PP - Peak Process the output");
            Console.WriteLine("\t-PPCSV - Save Peak Process as CSV");
            Console.WriteLine("\t-PPJSON - Save Peak Process as JSON");

            Console.WriteLine("\t-DeclinationPeakJSON - Save Declination Peak Process as CSV");
            Console.WriteLine("\t-DeclinationPeakCSV - Save Declination Peak Process as JSON");
            Console.WriteLine("\t-SaveDeclinPNG - Save Declination HiRes Data PNG");

            Console.WriteLine("\t-minPPAzDistance degrees - Remove similar peaks within this horizonal range (default 20)");
            Console.WriteLine("\t-minPPElevDistancedegrees - Remove similar peaks within this vertical range (default 0.066)");
            Console.WriteLine("\t-minPPDistance metres - Remove peaks closer to this (default 1000)");
            Console.WriteLine("\t-Correlate - Correlate Peak and Rising and Setting positions");
            Console.WriteLine("\t-CorrelateCSV - Save Correlation as CSV");
            Console.WriteLine("\t-CorrelateJSON - Save Correlation as JSON");
            Console.WriteLine("\t-Skim - Detect Skimming Rising and Setting positions");
            Console.WriteLine("\t-SkimCSV - Save Skimming as CSV");
            Console.WriteLine("\t-SkimJSON - Save Skimming as JSON");
            Console.WriteLine("\t-minSkimDistance - metres - Ignore skims below this value (default 700)");
            Console.WriteLine("\t-minSkimAzDelta - degrees - Ignore skims below this value (default 0.25)");

            Console.WriteLine("\t-minCorrelationValue metres - Remove correlated items below this value - 1/square of Aziumth difference (0.5)");
            Console.WriteLine("\t-minCorrelationValueSlopeWeighted metres - Remove correlated items with normalised slope below this value (default 0)");
            Console.WriteLine("\t-minCorrelationValueElevDiffWeighted metres - Remove correlated items with height to neighbours below this (default 0)");
            Console.WriteLine("\t-minCorrelationValueWeighted metres - Remove correlated items with value * slope below this (default 0.2))");

            return;
        }
    }
}
