using System;
using System.Collections.Generic;
// using System.Drawing;
using System.Linq;
using System.Text;
//using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;


using Microsoft.VisualBasic.FileIO;
using FileSystem = Microsoft.VisualBasic.FileIO.FileSystem;
using Newtonsoft.Json;

using PanaGraph;
using SkiaSharp;
using System.Runtime.Intrinsics.X86;
using System.IO;

namespace OpenPanoramaLib
{
    public class PaintImage
    {
        int horizontalRange = 360; // 360 Degrees...
        //int verticalRange = 20; // 18 degrees vertical range
        //int negativeRange = 2; // -2 degrees below horizontal vertical range
        const int scalesize = 2;
        int belowHorizonRange = 4; // we have a bottom border of 2 + negativeRange degrees.
        //public int pixels = 120; // We work in 1/120 of degrees for the image.
        public int ImageRawWidth = 0;
        public int ImageRawHeight = 0;
        int zero_hght = 0;
        public int lowest_elevation_height = 0;
        int ReticleHeight = 2;
        int topReticle;
        //int maxRadius = 30000;
        const int MaxZBuf = 2000000000;
        public const double halfSecond = 1.0 / 60 / 60 / 2;


        public static Color colourSea1 = Color.Black;
        public static Color colourSea2 = Color.Black;
        public static Color colourSky = Color.DarkBlue;
        public static Color colourBase = Color.DarkGreen;
        public static Color colourTops = Color.Brown;
        public static Color colourDistant = Color.DimGray;
        public static Color colourMoon = Color.Gray;
        public static Color colourSun = Color.Yellow;
        public static Color colourSaMText = Color.Black;
        public static Color colourTitle = Color.White;
        public static Color colourCairns = Color.DarkGreen;
        public static Color colourStones = Color.DarkSlateGray;
        public static Color colourSpotHeight = Color.DarkRed;

        static Color colourReticleBackground = Color.White;
        static Color colourReticleMajor = Color.Gray;
        static Color colourReticleMinor = Color.LightGray;
        static Color colourReticleHorizontal = Color.Black;
        static Color colourReticleText = Color.Black;

        public static int colourOriginLatitude = 45;
        public static int colourSlopeAdjust = 20;
        public static int colourMaxDist = 10000;
        public static int colourGreyCount = 80;
        public static int colourGreenBrownShades = 40;
        public static int colourMaxHeight = 1000;
        public static double colourContrast = 1.3;


        static string copyrightNotice = "Copyright(c) David Hoyle 2023, https://www.standingstones.org";

        public static Color ColourSea1 { set { colourSea1 = value; } get { return colourSea1; } }
        public static Color ColourSea2 { set { colourSea2 = value; } get { return colourSea2; } }
        public static Color ColourSky { set { colourSky = value; } }
        public static Color ColourBase { set { colourBase = value; } }
        public static Color ColourTops { set { colourTops = value; } }
        public static Color ColourDistant { set { colourDistant = value; } }
        public static Color ColourMoon { set { colourMoon = value; } }
        public static Color ColourSun { set { colourSun = value; } }
        public static Color ColourTitle { set { colourTitle = value; } get { return colourTitle; } }
        public static Color ColourCairns { set { colourCairns = value; } }
        public static Color ColourStones { set { colourStones = value; } }
        public static Color ColourSaMText { set { colourSaMText = value; } }
        public static Color ColourSpotHeight { set { colourSpotHeight = value; } }

        public static Color ColourReticleBackground { set { colourReticleBackground = value; } }
        public static Color ColourReticleMajor { set { colourReticleMajor = value; } }
        public static Color ColourReticleMinor { set { colourReticleMinor = value; } }
        public static Color ColourReticleHorizontal { set { colourReticleHorizontal = value; } }
        public static Color ColourReticleText { set { colourReticleText = value; } }


        public static int ColourMaxHeight { set { colourMaxHeight = value; } }
        public static int ColourOriginLatitude { set { colourOriginLatitude = value; } }
        public static int ColourSlopeAdjust { set { colourSlopeAdjust = value; } }
        public static int ColourMaxDist { set { colourMaxDist = value; } }
        public static int ColourGreyCount { set { colourGreyCount = value; } }
        public static int ColourGreenBrownShades { set { colourGreenBrownShades = value; } }
        public static double ColourContrast { set { colourContrast = value; } }

        public static string CopyrightNotice { set { copyrightNotice = value; } get { return copyrightNotice; } }

        static Pen SeaPen = new Pen(PaintImage.ColourSea2);


        public static Color GetColor(string s)
        {
            if (s.Length != 6)
            {
                throw new Exception("Bad Colour String " + s);
            }

            int r = int.Parse(s.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            int g = int.Parse(s.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            int b = int.Parse(s.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);

            return Color.FromArgb(0xff, r, g, b);
        }

        double[] MaxElevation = null;
        public float[][] ZBuffer = null;
        public int[] labelStack = null;
        public HorizonVector[] horizonLatLon = null;
        public float[] LIDARApproxBearingHeights = null;
        const int LIDARApproxBearingHeightsSize = 360 * 4;
        public static string outfolder = "";

        public RunJobParams rjParams;
        public Image myBitmap = null;
        public PenColours pc = new PenColours();


        //public Bitmap ReadBMP()
        //{
        //    return myBitmap;
        //}

        const double earthrad = 6371000;
        const double earthcircum = earthrad * 2 * Math.PI;

        public PaintImage(RunJobParams rjPs, bool Zbuffer)
        {
            rjParams = rjPs.Clone();
            belowHorizonRange = scalesize + rjParams.negativeRange;
            ImageRawWidth = horizontalRange * rjParams.pixels;
            ImageRawHeight = (rjParams.verticalRange + belowHorizonRange) * rjParams.pixels;
            zero_hght = (rjParams.verticalRange) * rjParams.pixels;
            lowest_elevation_height = (rjParams.verticalRange + rjParams.negativeRange) * rjParams.pixels;
            topReticle = ImageRawHeight - ReticleHeight * rjParams.pixels;
            myBitmap = new Image(ImageRawWidth, ImageRawHeight);// PixelFormat.Format32bppArgb);

            // Allocate the label stack for the image
            labelStack = new int[ImageRawWidth];

            if (rjParams.TemplateFile == null && Zbuffer)
            {
                horizonLatLon = new HorizonVector[ImageRawWidth];

                // Allocate and initialise the ZBuffer.
                ZBuffer = new float[ImageRawWidth][];
                MaxElevation = new double[ImageRawWidth];

                for (int x = 0; x < ImageRawWidth; x++)
                {
                    labelStack[x] = lowest_elevation_height;

                    MaxElevation[x] = -90;
                    ZBuffer[x] = new float[lowest_elevation_height];
                    for (int y = 0; y < lowest_elevation_height; y++)
                    {
                        ZBuffer[x][y] = MaxZBuf;
                    }
                }
            }

            SeaPen = new Pen(PaintImage.ColourSea2);
        }



        /// <summary>
        /// Does the line wrap around the 0 point, due north? If so draw two lines...
        /// </summary>
        /// <param name="gradient"></param>
        /// <param name="mypn"></param>
        /// <param name="sea"></param>
        /// <param name="profGraph"></param>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="z1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="z_unused"></param>
        /// <param name="height"></param>
        /// <param name="lat1"></param>
        /// <param name="lon1"></param>
        /// <param name="lat2"></param>
        /// <param name="lon2"></param>
        public void WrappedDrawLineDouble(Image pi, double gradient, Pen mypn, bool sea, Graphics profGraph, double x1, double y1, double z1, double x2, double y2, double z2, double height, HorizonVector hv)
        {
            double diffX = Math.Abs(x1 - x2);

            // Does the line wrap around the 0 point, due north? If so draw two lines...
            if (diffX > 180)
            {
                double x1adjust1 = 0;
                double x2adjust1 = 0;
                double x1adjust2 = 0;
                double x2adjust2 = 0;

                // If x1 > x2 then adjust x1 in the first call and then x2 in the second etc...
                if (x1 > x2)
                {
                    x1adjust1 = -360;
                    x2adjust1 = 0;
                    x1adjust2 = 0;
                    x2adjust2 = 360;
                }
                else
                {
                    x1adjust1 = 0;
                    x2adjust1 = -360;
                    x1adjust2 = 360;
                    x2adjust2 = 0;
                }
                DrawLineDouble(pi,gradient, mypn, sea, profGraph, x1 + x1adjust1, y1, z1, x2 + x2adjust1, y2, z2, height, hv);
                DrawLineDouble(pi,gradient, mypn, sea, profGraph, x1 + x1adjust2, y1, z1, x2 + x2adjust2, y2, z2, height, hv);
            }
            else
            {
                DrawLineDouble(pi, gradient, mypn, sea, profGraph, x1, y1, z1, x2, y2, z2, height, hv);
            }
        }




        public void SaveHiResJSON(string filename)
        {
            Console.WriteLine("Save Hi Res JSON " + filename);
            // serialize JSON directly to a file
            using (StreamWriter file = File.CreateText(filename))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, MaxElevation);
            }
        }


        public void SaveImage(string filename)
        {
            myBitmap.Save(filename);
        }


        static public void createfolders(string folder)
        {
            if (folder == null | folder.Length == 0)
            {
                return;
            }

            try
            {
                string tmp;
                for (int i = 0; i < folder.Length; i++)
                {
                    tmp = folder;
                    if (tmp[i] == '/' || tmp[i] == '\\')
                    {
                        tmp = tmp.Substring(0, i);
                        FileSystem.CreateDirectory(tmp);
                    }
                }
                FileSystem.CreateDirectory(folder);
            }
            catch (Exception ex)
            {
                Console.WriteLine("createfolders Exception caught " + folder + " " + ex.Message + " " + ex.StackTrace + " " + ex.InnerException);
            }
        }


        static public string cleanfilename(string filename)
        {
            return filename.Replace(",", "").Replace("'", "").Replace("(", " ").Replace(")", " ").Replace("?", "").Replace("  ", " ").Replace(" \\", "\\").Replace("--", "-").Replace("--", "-").Replace("--", "-");
        }



        public static string getFolderName(bool siteFolder, string county, string montype, string name, string mapref)
        {
            string foldername = "";

            if (outfolder != null && outfolder.Length > 0)
            {
                foldername += outfolder;
            }

            if (county != null && county.Length > 0)
            {
                if (foldername.Length > 0)
                {
                    foldername += "/";
                }
                foldername += county;
            }

            if (montype != null)
            {
                montype = montype.Replace("/", "-").Replace("(", "-").Replace(")", "").Replace("--", "-").Replace("--", "-").Replace("--", "-");
                if (montype.Length > 0)
                { 
                    if (foldername.Length > 0)
                    {
                        foldername += "/";
                    }

                    foldername += montype.Replace(" ", "-");
                }
            }

            foldername = cleanfilename(foldername).ToLower();
            //createfolders(foldername);

            if (siteFolder)
            {
                if (foldername.Length > 0)
                {
                    foldername += "/";
                }

                foldername += name + "_" + mapref;

                foldername = cleanfilename(foldername).ToLower();
                foldername = foldername.Replace(".", "");
                foldername = foldername.Replace(" ", "-");
                foldername = foldername.Trim();

                foldername = cleanfilename(foldername).ToLower();
                createfolders(foldername);
            }

            return foldername;
        }


        public static string getJPGName(bool siteFolder, string county, string montype, string name, string mapref)
        {
            string foldername = getFolderName(siteFolder, county, montype, name, mapref);
            string filename = "";
            if (foldername != null && foldername.Length > 0)
            {
                filename += foldername + "/";
            }
            filename += cleanfilename(name + "_" + mapref + ".jpg").ToLower();
            filename = filename.Replace(" ", "-").Replace("--", "-");

            return filename;
        }


        public static string getJPGName(bool siteFolder, SunMoonRunJob job)
        {
            return getJPGName(siteFolder, job.county, job.typestr, job.name, job.mapref);
        }


        public PaintImage RescaleToNewSize(int res)
        {
            RunJobParams rjp = rjParams.Clone();
            rjp.pixels = res;
            PaintImage newpi = new PaintImage(rjp, true);

            // Copy the exsiting image across...
            Graphics profGraph = Graphics.FromImage(newpi.myBitmap);

            profGraph.DrawImage(myBitmap, new Rectangle(0, 0, newpi.myBitmap.Width, newpi.myBitmap.Height), 0, 0, myBitmap.Width, myBitmap.Height, GraphicsUnit.Pixel);

            int scaler = rjParams.pixels / newpi.rjParams.pixels;

            // Copy and resize the Z buffer.
            for (int x = 0; x < newpi.ZBuffer.Length; x++)
            {
                for (int y = 0; y < newpi.ZBuffer[0].Length; y++)
                {
                    newpi.ZBuffer[x][y] = ZBuffer[x * scaler][y * scaler];
                }
            }
            return newpi;
        }


        public void ProcessSingleImage(List<SunMoonRunJob> rjs, double year, PaintImage mytemplate, HorizonVector[] thehorizonLatLon, float[][] theZBuffer, List<PassingPoint> passingPoints)
        {
            if (!rjParams.createTemplate)
            {
                PaintBackground(rjParams.description);
                Foresight fs;
                Foresight.HeavenlyTraces hte = Foresight.HeavenlyTraces.MoonPlusEPlusI;
                if (rjParams.crossQuarters)
                {
                    hte = Foresight.HeavenlyTraces.CrossQuarterSun2;
                }

                for (Foresight.HeavenlyTraces ht = Foresight.HeavenlyTraces.WinterSolsticeSun; ht <= hte; ht = (Foresight.HeavenlyTraces)((int)ht * 2))
                {
                    bool drawme = false;

                    switch (ht)
                    {
                        case Foresight.HeavenlyTraces.SummerSolsticeSun:
                        case Foresight.HeavenlyTraces.WinterSolsticeSun:
                            if (this.rjParams.sunSolstices)
                            {
                                drawme = true;
                            }
                            break;
                        case Foresight.HeavenlyTraces.EquinoxSun:
                            if (this.rjParams.sunEquinox)
                            {
                                drawme = true;
                            }
                            break;
                        case Foresight.HeavenlyTraces.CrossQuarterSun1:
                        case Foresight.HeavenlyTraces.CrossQuarterSun2:
                            if (this.rjParams.crossQuarters)
                            {
                                drawme = true;
                            }
                            break;
                        case Foresight.HeavenlyTraces.MoonPlusEPlusI:
                            if (this.rjParams.moonPlusEPlusI)
                            {
                                drawme = true;
                            }
                            break;
                        case Foresight.HeavenlyTraces.MoonPlusEMinusI:
                            if (this.rjParams.moonPlusEMinusI)
                            {
                                drawme = true;
                            }
                            break;
                        case Foresight.HeavenlyTraces.MoonMinusEPlusI:
                            if (this.rjParams.moonMinusEPlusI)
                            {
                                drawme = true;
                            }
                            break;
                        case Foresight.HeavenlyTraces.MoonMinusEMinusI:
                            if (this.rjParams.moonMinusEMinusI)
                            {
                                drawme = true;
                            }
                            break;

                        default:
                            break;

                    }

                    if (drawme)
                    {
                        fs = new Foresight(ht.ToString());
                        fs.CalculateDeclination(year, ht);
                        PaintSunMoonStars(ht, fs.Description, fs.declination, rjParams.lat, rjParams.lon, fs.isMoon, fs.GetDesc(ht), thehorizonLatLon, theZBuffer, passingPoints);
                    }
                }
            }

            if (rjParams.TemplateFile != null)
            {
                Image template = new Image(rjParams.TemplateFile);
                Graphics profGraph = Graphics.FromImage(myBitmap);
                profGraph.DrawImageUnscaled(template, 0, 0);
            }
            else if (mytemplate != null)
            {
                Graphics profGraph = Graphics.FromImage(myBitmap);
                profGraph.DrawImageUnscaledAndClipped(mytemplate.myBitmap, new Rectangle(0, 0, mytemplate.myBitmap.Width, mytemplate.myBitmap.Height));
            }
            else
            {
                UpdateWorkInProgress();
                PaintSea(rjParams.height, rjParams.lat, rjParams.lon);
                UpdateWorkInProgress();
                PaintAllRunJobs(rjs, rjParams.lat, rjParams.lon, rjParams.height);
                if (!rjParams.drwRJs)
                {
                    UpdateWorkInProgress();
                }
                PaintSRTM srtm = new PaintSRTM();
                PaintLidar lidar = new PaintLidar();
                PaintOS os = new PaintOS();
                lidar.NewPaintLIDAR(this,rjParams.lat, rjParams.lon, rjParams.height, rjParams.eCountry);
                LidarBlockReader.CleanLIDARCache();
                srtm.PaintSRTMHeights(this, rjParams.lat, rjParams.lon, rjParams.height);
                ZipMapDataHandler.SRTMFlushCache();
                os.PaintSpotHeightsAndContours(this, rjParams.lat, rjParams.lon, rjParams.height, rjParams.eCountry);
                ZipMapDataHandler.OSFlushCache();

                PaintForeground();
            }

            if (!rjParams.createTemplate)
            {
                PaintReticle(rjParams.lat, rjParams.lon);
            }

            UpdateWorkInProgress(true);
        }


        public void SaveHorizonGPXCSV(bool gpx, string filename)
        {
            if (horizonLatLon != null)
            {
                Console.WriteLine("Create CSV Output File " + filename);
                System.IO.StreamWriter outfile = new System.IO.StreamWriter(filename);

                if (gpx)
                {
                    outfile.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"no\" ?>");
                    outfile.WriteLine("<gpx>");

                    outfile.WriteLine("<metadata>");
                    outfile.WriteLine("</metadata>");

                }
                else
                {
                    outfile.WriteLine("azimuth,altitude,distance,latitude,longitude,height,comment");
                }

                double azureStep = 360.0 / horizonLatLon.Length;
                int intStep = (int)(1 / azureStep);
                for (int i = 0; i < horizonLatLon.Length; i++)
                {
                    if (horizonLatLon[i] != null)
                    {
                        double deltaaz = horizonLatLon[i].azimuth2 - horizonLatLon[i].azimuth1;
                        if (deltaaz > 180) deltaaz -= 360;
                        if (deltaaz < -180) deltaaz += 360;

                        double deltaalt = horizonLatLon[i].altitude2 - horizonLatLon[i].altitude1;
                        double deltalat = horizonLatLon[i].latitude2 - horizonLatLon[i].latitude1;
                        double deltalon = horizonLatLon[i].longitude2 - horizonLatLon[i].longitude1;
                        double deltaheight = horizonLatLon[i].height2 - horizonLatLon[i].height1;
                        double deltadist = horizonLatLon[i].distance2 - horizonLatLon[i].distance1;

                        double azimuth = azureStep * i;
                        double proportion = 0;
                        if (deltaaz != 0)
                        {
                            double azStart = azimuth - horizonLatLon[i].azimuth1;
                            if (azStart > 180) azStart -= 360;
                            if (azStart < -180) azStart += 360;
                            proportion = (azStart) / deltaaz;
                        }

                        if (proportion < 0) proportion = 0;
                        if (proportion > 1) proportion = 1;

                        double altitude = horizonLatLon[i].altitude1 + deltaalt * proportion;
                        double latitude = horizonLatLon[i].latitude1 + deltalat * proportion;
                        double longitude = horizonLatLon[i].longitude1 + deltalon * proportion;
                        double distance = horizonLatLon[i].distance1 + deltadist * proportion;
                        double height = horizonLatLon[i].height1 + deltaheight * proportion;


                        string comment = horizonLatLon[i].ptsrc + ":" + horizonLatLon[i].latitude1 + ":" + horizonLatLon[i].longitude1 + ":" + horizonLatLon[i].height1 + ":" + horizonLatLon[i].latitude2 + ":" + horizonLatLon[i].longitude2 + ":" + horizonLatLon[i].height2;
                        if (gpx)
                        {
                            if (i % (intStep * 5) == 0)
                            {
                                if (i > 0)
                                {
                                    outfile.WriteLine("</trkseg>");
                                    outfile.WriteLine("</trk>");
                                }
                                outfile.WriteLine("<trk>");
                                outfile.WriteLine("<name>Bearing " + azimuth + " - " + (azimuth + 4.99) + "</name>");
                                outfile.WriteLine("<trkseg>");
                            }
                            outfile.WriteLine("<trkpt lat=\"" + latitude + "\" lon=\"" + longitude + "\" ><name>" + azimuth + "</name></trkpt>");
                        }
                        else
                        {
                            outfile.WriteLine(azimuth + "," + altitude + "," + distance + "," + latitude + "," + longitude + "," + height + "," + comment);
                        }
                    }
                }


                if (gpx)
                {
                    outfile.WriteLine("</trkseg>");
                    outfile.WriteLine("</trk>");
                    outfile.WriteLine("</gpx>");
                }

                outfile.Close();
            }
        }


        public void ProcessAllImages(List<SunMoonRunJob> rjs, string site)
        {
            if (rjParams.ages == null || rjParams.ages.Length == 0)
            {
                return;
            }


            // Go Process the initial image... this is a template image
            ProcessSingleImage(rjs, rjParams.ages[0], null, null, null, null);

            // Save  the initial image as a PNG in case we need a template later.
            if (rjParams.createTemplate)
            {
                SaveImage(rjParams.filename.Replace(".jpg", ".png"));
            }

            if (rjParams.createHorizon)
            {
                SaveHorizonGPXCSV(true, rjParams.filename.Replace(".jpg", ".gpx"));
            }

            if (rjParams.createHorizonCSV)
            {
                SaveHorizonGPXCSV(false, rjParams.filename.Replace(".jpg", ".csv"));
            }

            // Save  the initial image as a PNG in case we need a template later.
            if (rjParams.HiResHorizon)
            {
                SaveHiResJSON(rjParams.filename.Replace(".jpg", "_hires.jsn"));
            }


            PaintImage[] PiRes = null;

            int endlabels = 2;
            if (rjParams.nodrwSpots) endlabels = 1;

            if (rjParams.createMultipleRes)
            {
                PaintImage piLowestRes = RescaleToNewSize(rjParams.pixels / 4);
                PaintImage piMidRes = RescaleToNewSize(rjParams.pixels / 2);
                PiRes = new PaintImage[3];
                PiRes[0] = this;
                PiRes[1] = piLowestRes;
                PiRes[2] = piMidRes;
            }
            else
            {
                PiRes = new PaintImage[1];
                PiRes[0] = this;
            }

            string resSuffix = "";
            string labelSuffix = "";
            string yearSuffix = "";
            string gridSuffix = "";

            if (rjParams.csvBulkFiles == null)
            {
                endlabels = 1;
            }

            for (int rs = 0; rs < PiRes.Length; rs++)
            {
                resSuffix = "";

                if (rjParams.createMultipleRes)
                {
                    resSuffix = "_" + PiRes[rs].rjParams.pixels;
                }

                for (int lab = 0; lab < endlabels; lab++)
                {
                    if (lab > 0)
                    {
                        labelSuffix = "_labels";
                        PiRes[rs].rjParams.drawLocations = true;
                        PiRes[rs].rjParams.drwSpots = true;
                        PiRes[rs].rjParams.drwContours = false;
                        PaintOS os = new PaintOS();
                        os.PaintSpotHeightsAndContours(PiRes[rs], rjParams.lat, rjParams.lon, rjParams.height, rjParams.eCountry);
                    }
                    else
                    {
                        labelSuffix = "";
                    }


                    for (int grd = 0; grd < 3; grd++)
                    {
                        if (!rjParams.allGrids)
                        {
                            bool drawme = false;

                            if (grd == 0 && rjParams.noGrid)
                            {
                                drawme = true;
                            }

                            if (grd == 2 && rjParams.fineGrid)
                            {
                                drawme = true;
                            }

                            if (grd == 1 && rjParams.grid && !rjParams.fineGrid)
                            {
                                drawme = true;
                            }

                            if (!drawme)
                            {
                                continue;
                            }
                        }


                        for (int y = 0; y < rjParams.ages.Length; y++)
                        {
                            yearSuffix = "";
                            if (rjParams.ages.Length > 1)
                            {
                                yearSuffix = "_" + ((int)rjParams.ages[y]);
                            }
                            PaintImage pi = new PaintImage(PiRes[rs].rjParams, false);
                            pi.rjParams.createTemplate = false;

                            gridSuffix = "";

                            pi.rjParams.noGrid = false;
                            pi.rjParams.grid = false;
                            pi.rjParams.fineGrid = false;

                            switch (grd)
                            {
                                case 0:
                                    pi.rjParams.noGrid = true;
                                    break;
                                case 1:
                                    pi.rjParams.grid = true;
                                    //if (rjParams.allGrids)
                                    //{
                                    gridSuffix = "_grid";
                                    //}
                                    break;
                                case 2:
                                    pi.rjParams.grid = true;
                                    pi.rjParams.fineGrid = true;
                                    //if (rjParams.allGrids)
                                    //{
                                        gridSuffix = "_fgrid";
                                    //}
                                    break;
                            }

                            string tmpfil = rjParams.filename;
                            if (rjParams.ages.Length > 1 || gridSuffix.Length > 0 || labelSuffix.Length > 0 || PiRes.Length > 1)
                            {
                                tmpfil = rjParams.filename.Replace(".jpg", yearSuffix + resSuffix + gridSuffix + labelSuffix + ".jpg");
                            }

                            List<PassingPoint> passingPoints = new List<PassingPoint>();
                            float[][] theZBuffer = null;
                            HorizonVector[] thehorizonLatLon = null;

                            if (rs == 0 && ((grd == 0 && rjParams.allGrids) || !rjParams.allGrids) && lab == 0)
                            {
                                theZBuffer = ZBuffer;
                                thehorizonLatLon = horizonLatLon;
                            }

                            pi.ProcessSingleImage(rjs, rjParams.ages[y], PiRes[rs], thehorizonLatLon, theZBuffer, passingPoints);
                            pi.SaveImage(tmpfil);
                            //pi.SaveImage(tmpfil.Replace( ".jpg", "_2.png"));
                            Console.WriteLine("Written File " + tmpfil);

                            if (passingPoints.Count > 0 && this.rjParams.createPassingPoints)
                            {
                                string pfilename = tmpfil.Replace(".jpg", "_sunmoon.gpx");
                                PassingPoint.DumpPassingPoints(true, site, rjParams.lat, rjParams.lon, pfilename, passingPoints);
                                pfilename = tmpfil.Replace(".jpg", "_sunmoon.csv");
                                PassingPoint.DumpPassingPoints(false, site, rjParams.lat, rjParams.lon, pfilename, passingPoints);
                                Console.WriteLine("Written Sun Moon Setting Points File " + pfilename);
                            }
                            pi = null;
                            GeneralUtilClasses.RunGC();
                        }
                    }
                }
            }
        }


        public static string GetHourStr(bool morn, double hour)
        {
            double tmp = Math.PI;
            if (morn)
            {
                tmp -= hour;
            }
            else
            {
                tmp += hour;
            }
            int secs = (int)(tmp / Math.PI * 60 * 60 * 12);
            int hours = (int)secs / 60 / 60;
            int mins = (int)(secs / 60) - hours * 60;
            secs = (int)secs - mins * 60 - hours * 60 * 60;

            return hours.ToString("00") + ":" + mins.ToString("00") + ":" + secs.ToString("00");
        }


        public void PaintBackground(string description)
        {
            Graphics profGraph = Graphics.FromImage(myBitmap);

            // First clear the image with the current backcolor
            profGraph.Clear(colourSky);


            string val = description;
            StringFormat sf = new StringFormat();
            sf.LineAlignment = StringAlignment.Center;
            sf.Alignment = StringAlignment.Center;

            int txtheight = 100;
            int smalltxtheight = 20;
            int width = 6000;

            SolidBrush brsh = new SolidBrush(colourTitle);

            Rectangle rct = new Rectangle(horizontalRange * rjParams.pixels / 2 - width / 2, 20, width, txtheight);
            profGraph.DrawString(val, new Font(FontFamily.GenericSansSerif, txtheight * 4 / 5, FontStyle.Regular), brsh, rct, sf);

            profGraph.DrawString(copyrightNotice, new Font(FontFamily.GenericSansSerif, smalltxtheight, FontStyle.Regular), brsh, 10.0f, 10);
        }



        public void GetDipDistanceForHeight(double height, ref double dip, ref double distance)
        {
            dip = Math.Acos(earthrad / (earthrad + height)) / (0.90 / 0.97);     // The value of .93 is a approximate made up number for K ~12 ~ 0.90/0.97
            distance = earthcircum * dip / 2 / Math.PI;
        }


        static DateTime wipTimer = DateTime.MinValue;

        public void UpdateWorkInProgress(bool final = false)
        {
            //Console.WriteLine("UpdateWorkInProgress() " + wipTimer + " updateInterval " + rjParams.updateInterval + " updateJPG " + rjParams.updateJPG );
            if ((rjParams.updateInterval > 0) && (rjParams.updateJPG != null) && (rjParams.updateJPG.Length > 0))
            {
                if ((DateTime.Now - wipTimer) > TimeSpan.FromSeconds(rjParams.updateInterval) || final)
                {
                    //Console.WriteLine("UpdateWorkInProgress() " + (DateTime.Now - wipTimer) );

                    try
                    {
                        wipTimer = DateTime.Now;
                        File.Delete(rjParams.updateJPG);
                    }
                    catch (Exception ex)
                    {
                        string exstr = ex.Message;
                        Console.WriteLine("UpdateWorkInProgress() Delete File Exception " + ex.Message + " " + ex.StackTrace);
                    }

                    try
                    {
                        //Console.WriteLine("UpdateWorkInProgress() SaveImage " + rjParams.updateJPG );
                        SaveImage(rjParams.updateJPG);
                    }
                    catch (Exception ex)
                    {
                        string exstr = ex.Message;
                        Console.WriteLine("UpdateWorkInProgress() SaveImage Exception " + ex.Message + " " + ex.StackTrace);
                    }
                }
            }
        }

        public void PaintSea(double myHeight, double mylat, double mylon)
        {
            if (myHeight < 2) myHeight = 2;

            Graphics profGraph = Graphics.FromImage(myBitmap);
            Pen seaPen = new Pen(colourSea1, 1);

            SeaHorizon sh = SeaHorizon.GetHorizonForHeight((int)myHeight);
            double angle = sh.elevation * rjParams.pixels;

            // Fill the foreground in the sea colour - sea...
            SolidBrush seaBrush = new SolidBrush(colourSea1);
            profGraph.FillRectangle(seaBrush, 0, (int)(zero_hght - angle), ImageRawWidth, lowest_elevation_height);

            //byte[] mybites = myBitmap.bm.Bytes;



            // Sort the Z Buffer
            int maxy = (int)(angle + rjParams.pixels * rjParams.negativeRange);

            double azimuthStep = 360.0 / ImageRawWidth;

            for (int y = 0; y < (int)maxy; y++)
            {
                for (int x = 0; x < ImageRawWidth; x++)
                {
                    if (y == 0)
                    {
                        // Need to create a Lat/Lon for the place on the horizon...
                        horizonLatLon[x] = new HorizonVector(azimuthStep * x);

                        double newlat = 0.0;
                        double newlon = 0.0;
                        LatLonConversions.GetLatLonFromBearingDistDeg(mylat, mylon, ((double)x) / rjParams.pixels, sh.distance, ref newlat, ref newlon);

                        horizonLatLon[x].setHVContext(HorizonVector.HorizonSource.Sea, azimuthStep * x, angle, newlat, newlon, sh.distance, 0.0, azimuthStep * x, angle, newlat, newlon, sh.distance, 0.0);
                    }

                    if (x < ZBuffer.Length && y < ZBuffer[x].Length)
                    {
                        ZBuffer[x][y] = (int)sh.distance;
                    }
                }
            }


            // Fill in the Z buffer for the sea...
            int oldy = -1;

            for (int dist = (int)sh.distance; dist >= 0; dist -= 1)
            {
                double dip = PaintImage.CalculateDip(dist);
                double Elevation = Math.Atan((-myHeight - dip) / dist) * 180 / Math.PI;

                double myangle = Elevation * rjParams.pixels + rjParams.pixels * rjParams.negativeRange;

                if (oldy != (int)myangle)
                {
                    oldy = (int)myangle;
                    if (oldy < 0) break;
                    if (oldy >= 0 && oldy < ZBuffer[0].Length)
                    {
                        for (int x = 0; x < ImageRawWidth; x++)
                        {
                            if (x < ZBuffer.Length && oldy < ZBuffer[x].Length)
                            {
                                ZBuffer[x][oldy] = dist + 5;
                            }
                        }
                    }
                }
            }
            profGraph.me.Flush();
        }


        public void PaintForeground()
        {
            Graphics profGraph = Graphics.FromImage(myBitmap);

            // Fill the foreground in gray...
            SolidBrush brsh = new SolidBrush(colourBase);
            profGraph.FillRectangle(brsh, 0, lowest_elevation_height, ImageRawWidth, lowest_elevation_height);

            profGraph.me.Flush();
        }



        public void VerticalLine(Image pi, double slopeangle, Pen mypn, bool sea, Graphics profGraph, int x, int y, double z, double height, HorizonVector hv, double elevation = 0)
        {
            if (MaxElevation != null && x >= 0 && x < MaxElevation.Length && MaxElevation[x] < elevation)
            {
                MaxElevation[x] = elevation;
            }

            if (y < 0) y = 0;
            if (y >= lowest_elevation_height) y = lowest_elevation_height - 1;

            if (x >= 0 && x < ImageRawWidth && y >= 0 && y < lowest_elevation_height)
            {
                // If this has not been written to before then remember the lat lon for it...
                if (lowest_elevation_height - y >= 0 && (lowest_elevation_height - y) < lowest_elevation_height)
                {
                    if (ZBuffer[x][y] == MaxZBuf)
                    {
                        bool skip = false;

                        if (!skip)
                        {
                            if (horizonLatLon[x] == null)
                            {
                                double azimuthStep = 360.0 / ImageRawWidth;
                                horizonLatLon[x] = new HorizonVector(azimuthStep * x);
                            }

                            if (hv != null)
                            {
                                horizonLatLon[x].copyTo(hv);
                            }
                        }
                    }
                }


                if (ZBuffer[x][y] > z)
                {
                    int endy = y;

                    for (int yy = y; yy >= 0; yy--)
                    {
                        if (ZBuffer[x][yy] > z)
                        {
                            endy = yy;
                            ZBuffer[x][yy] = (float)z;
                        }
                        else
                        {
                            break;
                        }

                        //this.myBitmap.bm.SetPixel(x, yy, mypn.paint.Color);
                    }

                    if (pi.linePainter != null)
                    {
                        pi.linePainter.VerticalLine(x, lowest_elevation_height - y, lowest_elevation_height - endy, (uint) mypn.paint.Color);
                    }
                    else
                    {
                        profGraph.path.MoveTo(x, lowest_elevation_height - y);
                        profGraph.path.LineTo(x, lowest_elevation_height - endy);
                    }
                }
            }
        }



        public void DrawLineDouble(Image pi, double gradient, Pen mypn, bool sea, Graphics profGraph, double x1double, double y1double, double z1, double x2double, double y2double, double z2, double height, HorizonVector hv)
        {
            double slopeangle = Math.Atan(gradient);
            double myslope = 9999999999999999;

            double diffX = x1double - x2double;

            // If both xpoints are too close then assume the slope is zero. 
            if (Math.Abs(diffX) * rjParams.pixels > 0.1)
            {
                myslope = (y1double - y2double) / (diffX);
            }
            else
            {
                myslope = 0;
            }


            Pen pn = mypn;
            if (mypn == null)
            {
                if (sea)
                {
                    pn = SeaPen;
                }
                else
                {
                    double lat = 45;
                    if (hv != null)
                    {
                        lat = hv.latitude1;
                    }
                    pn = pc.GetPenForDistance(slopeangle, (z2 + z1)/2, height, lat);
                }
            }

            int x1 = (int)(x1double * rjParams.pixels);
            int y1 = (int)((y1double + rjParams.negativeRange) * rjParams.pixels);
            int x2 = (int)(x2double * rjParams.pixels);
            int y2 = (int)((y2double + rjParams.negativeRange) * rjParams.pixels);

            if (rjParams.wireframe)
            {
                profGraph.DrawLine(pn, x1, y1, x2, y2);
            }
            else
            {
                profGraph.path = new SKPath();

                bool steep = Math.Abs(y2 - y1) > Math.Abs(x2 - x1);
                if (steep)
                {
                    int t;
                    t = x1; // swap x0 and y0
                    x1 = y1;
                    y1 = t;
                    t = x2; // swap x1 and y1
                    x2 = y2;
                    y2 = t;
                }

                if (x1 > x2)
                {
                    int t;
                    t = x1; // swap x0 and x1
                    x1 = x2;
                    x2 = t;
                    t = y1; // swap y0 and y1
                    y1 = y2;
                    y2 = t;

                    double tz = z1;
                    z1 = z2;
                    z2 = tz;
                }

                int steps = x2 - x1 + 1;
                double dz = (z2 - z1) / steps;
                double myZ = z1;

                int dx = x2 - x1;
                int dy = Math.Abs(y2 - y1);
                int error = dx / 2;
                int ystep = (y1 < y2) ? 1 : -1;
                double zzstart = z1;

                int y = y1;
                int stepCount = 0;

                for (int x = x1; x <= x2; x++)
                {
                    if (steep)
                    {
                        double elevation = y1double + (((double)y) / rjParams.pixels - x1double) * myslope;
                        if (elevation > Math.Max(y1double, y2double))
                        {
                            elevation = Math.Max(y1double, y2double);
                        }
                        if (elevation < Math.Min(y1double, y2double))
                        {
                            elevation = Math.Min(y1double, y2double);
                        }

                        VerticalLine(pi,slopeangle, pn, sea, profGraph, y, x, (zzstart + stepCount * dz), height, hv, elevation);
                    }
                    else
                    {
                        double elevation = y1double + (((double)x) / rjParams.pixels - x1double) * myslope;
                        if (elevation > Math.Max(y1double, y2double))
                        {
                            elevation = Math.Max(y1double, y2double);
                        }
                        if (elevation < Math.Min(y1double, y2double))
                        {
                            elevation = Math.Min(y1double, y2double);
                        }
                        VerticalLine(pi,slopeangle, pn, sea, profGraph, x, y, (zzstart + stepCount * dz), height, hv, elevation);
                    }
                    stepCount += 1;
                    error = error - dy;
                    if (error < 0)
                    {
                        y += ystep;
                        error += dx;
                    }
                }

                //profGraph.me.DrawPath(profGraph.path, pn.paint);
            }
        }



        //static bool prev_sea = false;
        //static int prev_x1 = -1;
        //static int prev_y1 = -1;
        //static double prev_z = -1;
        //static int prev_x0 = -1;
        //static int prev_y0 = -1;


        //public void DrawLine(double gradient, Pen mypn, bool sea, Graphics profGraph, int x1, int y1, double z1, int x2, int y2, double z2, double height, HorizonVector hv)
        //{
        //    double slopeangle = Math.Atan(gradient);

        //    if ((sea == prev_sea) && (x1 == prev_x0) && (y1 == prev_y0) && (z1 == prev_z) && (x2 == prev_x1) && (y2 == prev_y1))
        //    {
        //        return;
        //    }
        //    else
        //    {
        //        prev_sea = sea;
        //        prev_x0 = x1;
        //        prev_y0 = y1;
        //        prev_z = z1;
        //        prev_x1 = x2;
        //        prev_y1 = y2;
        //    }

        //    bool steep = Math.Abs(y2 - y1) > Math.Abs(x2 - x1);
        //    if (steep)
        //    {
        //        int t;
        //        t = x1; // swap x0 and y0
        //        x1 = y1;
        //        y1 = t;
        //        t = x2; // swap x1 and y1
        //        x2 = y2;
        //        y2 = t;
        //    }

        //    if (x1 > x2)
        //    {
        //        int t;
        //        t = x1; // swap x0 and x1
        //        x1 = x2;
        //        x2 = t;
        //        t = y1; // swap y0 and y1
        //        y1 = y2;
        //        y2 = t;

        //        double tz = z1;
        //        z1 = z2;
        //        z2 = tz;
        //    }

        //    int steps = x2 - x1 + 1;
        //    double dz = (z2 - z1) / steps;
        //    double myZ = z1;

        //    int dx = x2 - x1;
        //    int dy = Math.Abs(y2 - y1);
        //    int error = dx / 2;
        //    int ystep = (y1 < y2) ? 1 : -1;
        //    double zzstart = z1;
        //    zzstart = z1;

        //    int y = y1;
        //    int stepCount = 0;

        //    for (int x = x1; x <= x2; x++)
        //    {
        //        if (steep)
        //        {
        //            VerticalLine(slopeangle, mypn, sea, profGraph, y, x, (zzstart + stepCount * dz), height, hv);
        //        }
        //        else
        //        {
        //            VerticalLine(slopeangle, mypn, sea, profGraph, x, y, (zzstart + stepCount * dz), height, hv);
        //        }
        //        stepCount += 1;
        //        error = error - dy;
        //        if (error < 0)
        //        {
        //            y += ystep;
        //            error += dx;
        //        }
        //    }
        //}



        public void PaintAllRunJobs(List<SunMoonRunJob> rjs, double lat, double lon, double hght)
        {
            if (rjs == null)
            {
                return;
            }

            if (!rjParams.drwRJs)
                return;

            bool extraDebug = false;

            Graphics profGraph = Graphics.FromImage(myBitmap);

            Pen stones_pn = new Pen(colourStones, 1);
            Pen cairns_pn = new Pen(colourCairns, 1);

            foreach (SunMoonRunJob rj in rjs)
            {
                Pen pn = null;

                double monRad = 0;
                double monHeight = 0;

                //Console.WriteLine("Monument " + rj.name + " Type " + rj.typestr);

                switch (rj.typestr)
                {
                    case "Round Cairn":
                        monRad = 3;
                        monHeight = 2;
                        pn = cairns_pn;
                        if (rj.name.ToLower().Contains("minning"))
                        {
                            // Minning Low is 34m by 44m or a radius of about 20M
                            monRad = 20;
                            monHeight = 2;
                        }
                        break;

                    case "Cairn":
                    case "Barrow Cemetery":
                    case "Ring Cairn":
                    case "Burial Chamber(Dolmen)":
                    case "Cist":
                    case "Long Barrow":
                        monRad = 3;
                        monHeight = 2;
                        pn = cairns_pn;
                        break;

                    case "Stone Row / Alignment":
                    case "Stone Circle":
                    case "Timber Circle":
                    case "Henge":
                        monRad = 4;
                        monHeight = 1;
                        pn = stones_pn;
                        break;

                    case "Multiple Stone Rows / Avenue":
                    case "Standing Stones":
                    case "Standing Stone (Menhir)":
                    case "Holed Stone":
                        monRad = 0.5;
                        monHeight = 2.5;
                        pn = stones_pn;

                        if (rj.name.ToLower().Contains("bodewryd"))
                        {
                            //extraDebug = true;
                            //Console.WriteLine("ExtraDebug");
                            // Bodewryd is 4m high...
                            monHeight = 4;
                        }

                        break;

                    default:
                        monRad = 0.5;
                        monHeight = 1;
                        pn = cairns_pn;
                        break;
                }


                double distance = LatLonConversions.distHaversine(lat, lon, rj.lat, rj.lon);
                if (extraDebug)
                {
                    Console.WriteLine("Monument " + rj.name + " Distance " + distance);
                }

                if (distance > 10000 || distance < 50)
                {
                    // If more than 10km or less than 50 skip drawing at all - too far to really see and saves time.
                    if (extraDebug)
                    {
                        Console.WriteLine("Skipping");
                    }
                    continue;
                }

                // Get the bearing and height of the item
                double Angle = LatLonConversions.bearing(lat, lon, rj.lat, rj.lon);
                LatLon_OsGridRef ll = new LatLon_OsGridRef(rj.lat, rj.lon, 0);
                double z = ZipMapDataHandler.GetHeightAtLatLon(false, rjParams.proximalInterpolation, rj.lat, rj.lon, rjParams.eCountry, rjParams, false);

                double dip = CalculateDip(distance);

                // Get the top of the monument to see if it can be seen or not...
                Double Elevation = Math.Atan((z - hght - dip + monHeight) / distance);

                Angle = Angle * 180 / Math.PI * rjParams.pixels;
                Elevation = Elevation * 180 / Math.PI * rjParams.pixels + rjParams.pixels * rjParams.negativeRange;

                if (extraDebug)
                {
                    Console.WriteLine("Monument " + rj.name + " Elevation " + Elevation + " Angle " + Angle);
                }

                // Can we see the item or not?
                if (Elevation > 0 &&
                    Angle > 0 && Angle < ZBuffer.Count() &&
                    Elevation < ZBuffer[0].Count() &&
                    ZBuffer[(int)Angle][(int)Elevation] > distance &&
                    pn != null)
                {
                    // Get the angle in radians of the tile from the tile centre... assumes the tile is 2m across and split into two 1M halves.
                    double WidthAngleDelta = Math.Asin(monRad / distance) / Math.PI * 180 * rjParams.pixels;
                    double HeightAngleDelta = Math.Asin(monHeight / distance) / Math.PI * 180 * rjParams.pixels;

                    if (extraDebug)
                    {
                        Console.WriteLine("DrawLine " + (int)(Angle + WidthAngleDelta) + ", " + (int)(Elevation - HeightAngleDelta) + ", " + (int)distance + ", " + (int)(Angle) + ", " + (int)(Elevation));
                        Console.WriteLine("DrawLine " + (int)(Angle) + ", " + (int)(Elevation) + ", " + (int)distance + ", " + (int)(Angle - WidthAngleDelta) + ", " + (int)(Elevation - HeightAngleDelta));
                    }

                    //WrappedDrawLine( 99999, pn, false, profGraph, (int)(Angle + WidthAngleDelta), (int)(Elevation - HeightAngleDelta), distance, (int)(Angle ), (int)(Elevation), distance, z, null);
                    //WrappedDrawLine( 99999, pn, false, profGraph, (int)(Angle), (int)(Elevation), distance, (int)(Angle - WidthAngleDelta), (int)(Elevation - HeightAngleDelta), distance, z, null);
                    WrappedDrawLineDouble(myBitmap, 99999, pn, false, profGraph, Angle + WidthAngleDelta, Elevation - HeightAngleDelta, distance, Angle, Elevation, distance, z, null);
                    WrappedDrawLineDouble(myBitmap, 99999, pn, false, profGraph, Angle, Elevation, distance, Angle - WidthAngleDelta, Elevation - HeightAngleDelta, distance, z, null);
                }
            }

            profGraph.me.Flush();
        }


        public void PaintReticle(double lat, double lon)
        {
            Graphics profGraph = Graphics.FromImage(myBitmap);

            // Fill the foreground in gray - sea...
            SolidBrush brsh = new SolidBrush(colourReticleBackground);
            profGraph.FillRectangle(brsh, 0, topReticle, ImageRawWidth, ReticleHeight * rjParams.pixels);

            brsh = new SolidBrush(colourReticleText);
            Pen BlackPn = new Pen(colourReticleHorizontal, 1);
            Pen gridpn = new Pen(colourReticleMajor, 1);
            Pen pn = BlackPn;
            Pen pn2 = new Pen(colourReticleHorizontal, 5);
            int txtheight = 50;


            if (rjParams.fineGrid)
            {
                Pen finegridpn = new Pen(colourReticleMinor, 1);

                for (int DegHour = 0; DegHour < horizontalRange * rjParams.pixels; DegHour += rjParams.pixels / 6)
                {
                    profGraph.DrawLine(finegridpn, DegHour, 0, DegHour, lowest_elevation_height);
                }
                for (int y = -rjParams.pixels * rjParams.negativeRange; y < rjParams.verticalRange * rjParams.pixels; y += rjParams.pixels / 6)
                {
                    profGraph.DrawLine(finegridpn, 0, zero_hght - y, horizontalRange * rjParams.pixels, zero_hght - y);
                }
            }

            for (int DegHour = 0; DegHour < horizontalRange * rjParams.pixels; DegHour += rjParams.pixels)
            {
                Pen tmppn = pn;
                int len = ReticleHeight / 2 * rjParams.pixels;
                if (DegHour % (5 * rjParams.pixels) == 0)
                {
                    len = ReticleHeight * rjParams.pixels * 3 / 4;
                }
                if (DegHour % (10 * rjParams.pixels) == 0)
                {
                    len = ReticleHeight * rjParams.pixels - txtheight;
                    tmppn = pn2;
                }
                profGraph.DrawLine(tmppn, DegHour, topReticle, DegHour, topReticle + len);

                if (DegHour % (10 * rjParams.pixels) == 0)
                {
                    string val = ((int)(DegHour / rjParams.pixels)).ToString();
                    StringFormat sf = new StringFormat();
                    sf.LineAlignment = StringAlignment.Center;
                    sf.Alignment = StringAlignment.Center;

                    int width = 300;
                    int y = topReticle + len;

                    Rectangle rct = new Rectangle(DegHour - width / 2, y, width, txtheight);
                    profGraph.DrawString(val, new Font(FontFamily.GenericSansSerif, txtheight * 4 / 5, FontStyle.Regular), brsh, rct, sf);
                }

                if (rjParams.grid)
                {
                    profGraph.DrawLine(gridpn, DegHour, 0, DegHour, lowest_elevation_height);
                }
            }

            if (rjParams.grid)
            {
                for (int y = -rjParams.negativeRange; y < rjParams.verticalRange; y++)
                {
                    if (y != 0)
                    {
                        pn = gridpn;
                    }
                    else
                    {
                        pn = BlackPn;
                    }
                    profGraph.DrawLine(pn, 0, zero_hght - y * rjParams.pixels, horizontalRange * rjParams.pixels, zero_hght - y * rjParams.pixels);
                }
            }

            profGraph.me.Flush();
        }


        public int getYFromAngle(double ang)
        {
            return (int)(ang * 180 / Math.PI * rjParams.pixels + rjParams.pixels * rjParams.negativeRange);
        }


        enum HiddenStates { NotHidden, Hidden, OffScreen };

        public void PaintSunMoonStars(Foresight.HeavenlyTraces ht, string infostrPrefix, double declination, double lat, double lon, bool moon, string desc, HorizonVector[] thehorizonLatLon, float[][] theZBuffer, List<PassingPoint> passingPoints)
        {
            // Hour Angle(HRA)
            //The Hour Angle converts the local solar time(LST) into the number of degrees which the sun moves across the sky.
            // By definition, the Hour Angle is 0° at solar noon.Since the Earth rotates 15° per hour, each hour away from
            // solar noon corresponds to an angular motion of the sun in the sky of 15°. In the morning the hour angle is negative,sav
            // in the afternoon the hour angle is positive.
            SkyPos[] sps = new SkyPos[3];

            Pen pn;
            SolidBrush brsh;
            string MoonOrSun = "Sun";
            SolidBrush txtbrsh = new SolidBrush(colourSaMText);
            StringFormat sf = new StringFormat();
            sf.LineAlignment = StringAlignment.Center;
            sf.Alignment = StringAlignment.Center;

            StringFormat sfdesc = new StringFormat();
            sfdesc.LineAlignment = StringAlignment.Center;
            sfdesc.Alignment = StringAlignment.Center;

            SkyPos[] prevsps = new SkyPos[3];

            bool[] firstpt = new bool[3];
            firstpt[0] = firstpt[1] = firstpt[2] = true;

            if (moon)
            {
                pn = new Pen(colourMoon, 1);
                brsh = new SolidBrush(colourMoon);
                MoonOrSun = "Moon";
            }
            else
            {
                pn = new Pen(colourSun, 1);
                brsh = new SolidBrush(colourSun);
            }

            Graphics profGraph = Graphics.FromImage(myBitmap);
            HiddenStates[] prevHidden = new HiddenStates[3];
            int[] prevangle = new int[] { -1, -1, -1 };
            Foresight.TopCenBot[] prevtcb = new Foresight.TopCenBot[3];

            for (int DegHour = 0; DegHour < horizontalRange * rjParams.pixels; DegHour += 1)
            {
                bool morning = true;
                double hour = Math.PI * DegHour / rjParams.pixels / 180;

                int tcbindx = 0;
                for (Foresight.TopCenBot tcb = Foresight.TopCenBot.Top; tcb <= Foresight.TopCenBot.Bottom; tcb = (Foresight.TopCenBot)((int)tcb * 2))
                {
                    if (hour > Math.PI)
                    {
                        morning = false;
                    }

                    //string infostr = infostrPrefix + ", Declination " + declination + ", hour " + hour + ", time " + GetHourStr(morning, hour);
                    string tmpnotused = "";

                    SkyPos sp = MovingBody.CalculateBodyElevationAzimuth(
                        lat / 180 * Math.PI,
                        lon / 180 * Math.PI,
                        declination / 180 * Math.PI,
                        hour,
                        moon,
                        tcb,
                        ref tmpnotused,
                        false);
                    if (!morning)
                    {
                        sp.Azimuth = 2 * Math.PI - sp.Azimuth; // 2 * Math.PI - azimuth...
                                                               //infostr += ", PM";
                    }
                    else
                    {
                        //infostr += ", AM";
                    }


                    if (sp.Azimuth > 0)
                    {
                        int angle = (int)(sp.Azimuth * 180 * rjParams.pixels / Math.PI);
                        int elev = (int)(sp.Elevation * 180 * rjParams.pixels / Math.PI);
                        int ZBufElev = elev + rjParams.pixels * rjParams.negativeRange;


                        Pen drawpn = pn;
                        Brush drawbrsh = brsh;
                        profGraph.FillRectangle(drawbrsh, angle, zero_hght - (elev), 1, 1);

                        if (theZBuffer != null && thehorizonLatLon != null)
                        {
                            if (angle >= 0 && angle < theZBuffer.Length && ZBufElev >= 0 && ZBufElev < theZBuffer[0].Length)
                            {
                                if (theZBuffer[angle][ZBufElev] == MaxZBuf)
                                {
                                    if (prevHidden[(int)tcbindx] == HiddenStates.Hidden && !firstpt[(int)tcbindx])
                                    {
                                        PassingPoint pp = new PassingPoint();
                                        pp.vec = thehorizonLatLon[prevangle[tcbindx]];
                                        pp.sp = sp;
                                        pp.ht = ht;
                                        pp.tcb = tcb;

                                        passingPoints.Add(pp);
                                    }
                                    prevHidden[(int)tcbindx] = HiddenStates.NotHidden;
                                }
                                else
                                {
                                    if (prevHidden[(int)tcbindx] == HiddenStates.NotHidden && prevsps[(int)tcbindx] != null && !firstpt[(int)tcbindx])
                                    {
                                        PassingPoint pp = new PassingPoint();
                                        pp.vec = thehorizonLatLon[angle];
                                        pp.sp = prevsps[(int)tcbindx];
                                        pp.ht = ht;
                                        pp.tcb = prevtcb[(int)tcbindx];

                                        passingPoints.Add(pp);
                                    }
                                    prevHidden[(int)tcbindx] = HiddenStates.Hidden;
                                }
                            }
                            else
                            {
                                prevHidden[(int)tcbindx] = HiddenStates.OffScreen;
                            }
                        }

                        prevangle[tcbindx] = angle;
                        firstpt[(int)tcbindx] = false;
                    }

                    prevtcb[(int)tcbindx] = tcb;
                    prevsps[tcbindx] = sp;
                    sps[tcbindx++] = sp;
                }

                // Every degree draw the sun or moon...
                if (DegHour % rjParams.pixels == 0)
                {
                    // Get the Radius of the sun/moon.
                    double radDeg = 0;
                    double ElDelta = (sps[0].Elevation - sps[2].Elevation) * 180 / Math.PI;

                    double deltaElev = sps[0].Elevation - sps[2].Elevation;
                    double deltaAzim = sps[0].Azimuth - sps[2].Azimuth;

                    Double rad2Deg = Math.Sqrt(deltaElev * deltaElev + deltaAzim * deltaAzim) / 2;
                    radDeg = rad2Deg * 180 / Math.PI;

                    if (ElDelta > 0 && ElDelta < 1 && sps[0].Azimuth >= 0 && sps[1].Azimuth > 0 && sps[2].Azimuth > 0)
                    {
                        double centrex = sps[1].Azimuth * 180 / Math.PI * rjParams.pixels;
                        double centrey = zero_hght - sps[1].Elevation * 180 * rjParams.pixels / Math.PI;
                        double width = radDeg * 2 * rjParams.pixels;
                        double deltax = sps[0].Azimuth - sps[2].Azimuth;
                        double deltay = sps[0].Elevation - sps[2].Elevation;
                        centrey = zero_hght - (sps[0].Elevation + sps[2].Elevation) / 2 * 180 * rjParams.pixels / Math.PI;
                        double height = Math.Sqrt((deltax * deltax) + (deltay * deltay)) * rjParams.pixels * 180 / Math.PI;

                        float x1 = (float)(centrex - width / 2);
                        float y1 = (float)(centrey - height / 2);
                        //float x2 = (float)(width - 1);
                        //float y2 = (float)(height - 1);
                        float x2 = (float)(width);
                        float y2 = (float)(height);

                        // profGraph.DrawRectangle(pn, x1, y1, x2, y2);
                        profGraph.FillEllipse(brsh, x1, y1, x2, y2);


                        // Enter the name into it if high enough resolution.
                        if (rjParams.pixels >= 60 && x2 > 0 && y2 > 0)
                        {
                            Rectangle txtrct = new Rectangle((int)x1, (int)y1, (int)x2, (int)y2 / 2);
                            //profGraph.DrawString(MoonOrSun, new Font(FontFamily.GenericSansSerif, y2 / 6, FontStyle.Regular), txtbrsh, txtrct, sf);
                            profGraph.DrawString(MoonOrSun, new Font(FontFamily.GenericSansSerif, y2 / 4, FontStyle.Regular), txtbrsh, txtrct, sf);
                            txtrct = new Rectangle((int)x1, (int)(y1 + y2 / 2), (int)x2, (int)y2 / 2);
                            //profGraph.DrawString(desc, new Font(FontFamily.GenericSansSerif, y2 / 6, FontStyle.Regular), txtbrsh, txtrct, sf);
                            profGraph.DrawString(desc, new Font(FontFamily.GenericSansSerif, y2 / 4, FontStyle.Regular), txtbrsh, txtrct, sf);
                        }
                    }
                }
            }

            if (moon)
            {
                for (int DegHour = 0; DegHour < horizontalRange * rjParams.pixels; DegHour += 10)
                {
                    bool morning = true;
                    double hour = Math.PI * DegHour / rjParams.pixels / 180;

                    for (Foresight.TopCenBot tcb = Foresight.TopCenBot.Top; tcb <= Foresight.TopCenBot.Bottom; tcb = (Foresight.TopCenBot)((int)tcb * 2))
                    {
                        if (tcb == Foresight.TopCenBot.Centre)
                        {
                            continue;
                        }

                        if (hour > Math.PI)
                        {
                            morning = false;
                        }

                        string tmpnotused = "";

                        SkyPos sp = MovingBody.CalculateBodyElevationAzimuth(
                            lat / 180 * Math.PI,
                            lon / 180 * Math.PI,
                            declination / 180 * Math.PI,
                            hour,
                            moon,
                            tcb,
                            ref tmpnotused,
                            true);
                        if (!morning)
                        {
                            sp.Azimuth = 2 * Math.PI - sp.Azimuth;
                            //infostr += ", PM";
                        }
                        else
                        {
                            //infostr += ", AM";
                        }

                        if (sp.Azimuth > 0)
                        {
                            int angle = (int)(sp.Azimuth * 180 * rjParams.pixels / Math.PI);
                            int elev = (int)(sp.Elevation * 180 * rjParams.pixels / Math.PI);
                            int ZBufElev = elev + rjParams.pixels * rjParams.negativeRange;

                            Pen drawpn = pn;
                            Brush drawbrsh = brsh;

                            profGraph.FillRectangle(drawbrsh, angle, zero_hght - (elev), 1, 1);
                        }
                    }
                }
            }

            profGraph.me.Flush();
        }

        public XDocument GetGridXDoc(OsGridRef ne)
        {
            //string GridRef = OGBRect.NE2NGR(ne.east, ne.north);
            OsGridRef gr = new OsGridRef(ne.east, ne.north);
            string GridRef = gr.toString();

            ZipMapDataHandler zippy = new ZipMapDataHandler();

            return zippy.GetOSVectorData(GridRef, false);
        }



        public double GetAngle(double deltaX, double deltaY)
        {
            double Angle = 0;
            try
            {
                Angle = Math.Atan(deltaX / deltaY);
            }
            catch (Exception ex)
            {
                string sjsj = "Possible divide by zero here ";
                sjsj += ex.Message + " " + ex.StackTrace;
                Angle = Math.PI / 2;
            }

            if (deltaY < 0)
            {
                Angle += Math.PI;
            }

            if (Angle < 0)
            {
                Angle += 2 * Math.PI;
            }

            return Angle;
        }


        public static double CalculateDip(double distance, double adjust = 6.0 / 7)
        {
            double earthrad = 6371000;
            double earthcircum = earthrad * 2 * Math.PI;

            double distanceinradians = distance / earthcircum * 2 * Math.PI;
            double dip = earthrad * (1 - Math.Cos(distanceinradians));

            // Adjust this for "typical" refraction - not perfect...
            // dip = dip * .90 / .97;
            // https://en.wikipedia.org/wiki/International_Standard_Atmosphere
            // https://www.metabunk.org/standard-atmospheric-refraction-empirical-evidence-and-derivation.t8703/#post-205947
            dip = dip * adjust;
            return dip;
        }


        // http://www.archaeocosmology.org/eng/refract.htm
        public static double CalculateDipThom(double distance) // , double nearHeight, double distantHeight)
        {
            double P = 1013.25; // air pressure at Near Height[mbar]
            double T = 15.0; // Temperature...
            //double Elev = distantHeight - nearHeight;
            double Lray = distance / 1000; // Distance converted to km...
            double K = 10.64;

            double earthrad = 6378137;
            double earthcircum = earthrad * 2 * Math.PI;

            double distanceinradians = distance / earthcircum * 2 * Math.PI;

            double TerrestRefract = 0.00830 * K * Lray * P / ((273.15 + T) * (273.15 + T));

            distanceinradians = distanceinradians - TerrestRefract / 180 * Math.PI;
            double dip = earthrad * (1 - Math.Cos(distanceinradians));

            return dip;
        }
    }
}