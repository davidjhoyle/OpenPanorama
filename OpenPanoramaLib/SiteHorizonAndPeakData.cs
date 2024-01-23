using Newtonsoft.Json;
using PanaGraph;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace OpenPanoramaLib
{
    public class SiteHorizonAndPeakData
    {
        XmlDocument docGPX = null;
        public PeakHorizonVector theHoz = null;
        public double latitudeOrigin = 0;
        public double longitudeOrigin = 0;
        //public string gpxFile = "";
        //public string pngFile = "";
        public int pointsPerDegree = 120;
        public List<int> peaks = new List<int>();
        public List<int> notches = new List<int>();
        public List<PeakInfo> peakList = new List<PeakInfo>();
        public List<int> declinationpeaks = new List<int>();
        public List<int> declinationnotches = new List<int>();
        public List<PeakInfo> declinationpeakList = new List<PeakInfo>();


        public void PeakProcess(string gpxFile, string pngFile, string hiresFile, int pointsPerDegree, string peakFileName, string declpeakFileName,
            bool saveJSON, bool saveCSV, bool DeclinationPeakJSON, bool DeclinationPeakCSV, bool saveDeclinPNG,
            double minAzDistance, double minElevDistance, double minDistance, string csvPeakHeaderPrefix, string csvPeakDataPrefix, double latitude, double minSkimAzDelta)
        {
            readGPXData(gpxFile);
            readCSVHorizonData(gpxFile.Replace(".gpx", ".csv"), pointsPerDegree);
            double[] hires = readHiResData(hiresFile, pointsPerDegree);
            if (hires == null)
            {
                readPNGData(pngFile, pointsPerDegree);
            }
            TransformElevationsToDeclinations(latitude);

            if (saveDeclinPNG)
            {
                SaveTransformedDeclinationsToPNG(pngFile.ToLower().Replace(".png", "_declin.png"));
            }

            calculateDistances();
            FindPeaks(minAzDistance, minElevDistance, minDistance, false);
            EvaluatePeaks(minDistance, false);

            FindPeaks(minAzDistance, minElevDistance * 0.4, minDistance, true);
            EvaluateDeclinationPeaks(minSkimAzDelta, 20);
            //EvaluatePeaks(minDistance, true);

            if (saveJSON)
            {
                savePeaks(peakFileName + ".jsn", true, null, null, false);
            }

            if (DeclinationPeakJSON)
            {
                savePeaks(declpeakFileName + ".jsn", true, null, null, true);
            }
            if (saveCSV)
            {
                savePeaks(peakFileName + ".csv", false, csvPeakHeaderPrefix, csvPeakDataPrefix, false);
            }

            if (DeclinationPeakCSV)
            {
                savePeaks(declpeakFileName + ".csv", false, csvPeakHeaderPrefix, csvPeakDataPrefix, true);
            }
        }

        public void CreateHV(int count)
        {
            if (theHoz == null || count > theHoz.theHorizon.Length)
            {
                theHoz = new PeakHorizonVector(count);
            }

            if (count != theHoz.theHorizon.Length)
            {
                string err = "CreateHV Counts Are not the same " + count + " " + theHoz.theHorizon.Length;
                Console.WriteLine(err);
                throw new Exception(err);
            }

            for (int i = 0; i < count; i++)
            {
                if (theHoz.theHorizon[i] == null)
                {
                    theHoz.theHorizon[i] = new HorizonPoint();
                }
            }
        }


        public void readGPXData(string GpxFile)
        {
            Console.WriteLine("readGPXData " + GpxFile);

            docGPX = new XmlDocument();
            docGPX.Load(GpxFile);

            //Console.WriteLine("Read " + docGPX.InnerText );

            int HVCount = 0;

            foreach (XmlNode nodei in docGPX.ChildNodes[1].ChildNodes)
            {
                if (nodei.Name != "trk")
                {
                    continue;
                }
                foreach (XmlNode nodej in nodei)
                {
                    if (nodej.Name != "trkseg")
                    {
                        continue;
                    }
                    foreach (XmlNode nodek in nodej)
                    {
                        if (nodek.Name == "trkpt")
                        {
                            HVCount++;
                        }
                    }
                }
            }

            Console.WriteLine("CreateHV " + HVCount);
            CreateHV(HVCount);
            Console.WriteLine("CreateHV Done " + HVCount);

            int nodeindx = 0;



            foreach (XmlNode nodei in docGPX.ChildNodes[1].ChildNodes)
            {
                if (nodei.Name != "trk")
                {
                    continue;
                }
                foreach (XmlNode nodej in nodei)
                {
                    if (nodej.Name != "trkseg")
                    {
                        continue;
                    }
                    foreach (XmlNode nodek in nodej)
                    {
                        if (nodek.Name == "trkpt")
                        {
                            foreach (XmlAttribute att in nodek.Attributes)
                            {
                                if (att.Name == "lat")
                                {
                                    theHoz.theHorizon[nodeindx].latitude = Convert.ToDouble(att.Value);
                                }
                                if (att.Name == "lon")
                                {
                                    theHoz.theHorizon[nodeindx].longitude = Convert.ToDouble(att.Value);
                                }
                            }
                            nodeindx++;
                        }
                    }
                }
            }


            Console.WriteLine("readGPXData Done " + nodeindx);


            return;
        }


        public bool readCSVHorizonData(string csvFile, int ppd)
        {
            Console.WriteLine("Read CSV Horizon File " + csvFile);
            bool allOK = true;

            try
            {
                bool deleteTmp = false;

                if (csvFile.ToLower().Contains("https://") || csvFile.ToLower().Contains("http://"))
                {
                    WebClient wc = new WebClient();
                    Random rnd = new Random();
                    string tmpCSVFile = "TempJSNFile" + rnd.Next(100000, 999999) + ".csv";

                    Console.WriteLine("Temporary Downloaded JSON File " + csvFile + " Temp " + tmpCSVFile);
                    wc.DownloadFile(csvFile, tmpCSVFile);
                    csvFile = tmpCSVFile;
                    deleteTmp = true;
                }


                double[] dvals = new double[10];
                string[] columnNames = null;

                using (StreamReader r = new StreamReader(csvFile))
                {
                    string? csvLine = r.ReadLine();
                    if (csvLine != null)
                    {
                        columnNames = csvLine.Split(",");
                    }
                    else
                    {
                        allOK = false;
                    }

                    while (csvLine != null )
                    {
                        csvLine = r.ReadLine();
                        if (csvLine != null)
                        {
                            string[] tmpvals = csvLine.Split(",");

                            if (tmpvals.Length >= 6)
                            {
                                for (int c = 0; c < 6; c++)
                                {
                                    dvals[c] = Convert.ToDouble(tmpvals[c]);
                                }

                                int x = (int)(dvals[0] * ppd);

                                if ( x < 0 || x >= theHoz.theHorizon.Length)
                                {
                                    Console.WriteLine("Reading from CSV File " + csvFile + " Angle >= 360 " + dvals[0]);
                                    allOK = false;
                                    break;
                                }

                                theHoz.theHorizon[x].bearing = dvals[0];
                                theHoz.theHorizon[x].elevation = dvals[1];
                                theHoz.theHorizon[x].distance = dvals[2];
                                theHoz.theHorizon[x].latitude = dvals[3];
                                theHoz.theHorizon[x].longitude = dvals[4];
                            }
                            else
                            {
                                break;
                            }
                        }
                    } 
                }

                if (deleteTmp)
                {
                    Console.WriteLine("Delete Temporary Downloaded CSV File " + csvFile);
                    File.Delete(csvFile);
                }
            }
            catch
            {
                Console.WriteLine("Could not read CSV File " + csvFile);
                allOK = false;
            }

            return allOK;
        }



        public double[] readHiResData(string HiResFile, int ppd)
        {
            Console.WriteLine("Read HiRes Horizon File " + HiResFile);
            double[] HiResData = null;

            try
            {
                bool deleteTmp = false;


                if (HiResFile.ToLower().Contains("https://") || HiResFile.ToLower().Contains("http://"))
                {
                    WebClient wc = new WebClient();
                    Random rnd = new Random();
                    string tmpJSNFile = "TempJSNFile" + rnd.Next(100000, 999999) + ".jsn";

                    Console.WriteLine("Temporary Downloaded JSON File " + HiResFile + " Temp " + tmpJSNFile);
                    wc.DownloadFile(HiResFile, tmpJSNFile);
                    HiResFile = tmpJSNFile;
                    deleteTmp = true;
                }


                using (StreamReader r = new StreamReader(HiResFile))
                {
                    string json = r.ReadToEnd();
                    HiResData = JsonConvert.DeserializeObject<double[]>(json);


                    if (theHoz.theHorizon.Length == HiResData.Length)
                    {
                        for (int x = 0; x < theHoz.theHorizon.Length; x++)
                        {
                            theHoz.theHorizon[x].elevation = HiResData[x];
                            theHoz.theHorizon[x].bearing = ((double)x) / ppd;
                        }
                    }
                    else
                    {
                        HiResData = null;
                    }
                }

                if (deleteTmp)
                {
                    Console.WriteLine("Delete Temporary Downloaded HiRes File " + HiResFile);
                    File.Delete(HiResFile);
                }
            }
            catch
            {
                Console.WriteLine("Could not read HiRes File " + HiResFile);
            }

            return HiResData;
        }

        public void readPNGData(string PngFile, int ppd)
        {
            Console.WriteLine("Read PNG File " + PngFile);
            bool deleteTmp = false;

            if (PngFile.ToLower().Contains("https://") || PngFile.ToLower().Contains("http://"))
            {
                WebClient wc = new WebClient();
                Random rnd = new Random();
                string tmpPNGFile = "TempPNGFile" + rnd.Next(100000, 999999) + ".png";

                Console.WriteLine("Temporary Downloaded PNG File " + PngFile + " Temp " + tmpPNGFile);
                wc.DownloadFile(PngFile, tmpPNGFile);
                PngFile = tmpPNGFile;
                deleteTmp = true;
            }

            Image InputBitmap = new Image(PngFile);
            int ReticleHeight = 2;
            double positiveHeight = 20 * ppd;

            int outputHeight = InputBitmap.Height - ReticleHeight * ppd;
            CreateHV(InputBitmap.Width);

            int lasty = 0;

            for (int x = 0; x < InputBitmap.Width; x++)
            {
                Color tcol = InputBitmap.GetPixel(x, lasty);
                if (!(tcol.R == 0 && tcol.G == 0 && tcol.B == 0))
                {
                    for (int y = lasty; y >= 0; y--)
                    {
                        Color col = InputBitmap.GetPixel(x, y);
                        if (col.R == 0 && col.G == 0 && col.B == 0)
                        {
                            break;
                        }

                        lasty = y;
                    }
                }
                else
                {
                    for (int y = lasty; y < outputHeight; y++)
                    {
                        lasty = y;
                        Color col = InputBitmap.GetPixel(x, y);
                        if (!(col.R == 0 && col.G == 0 && col.B == 0))
                        {
                            break;
                        }
                    }
                }
                theHoz.theHorizon[x].elevation = (positiveHeight - lasty) / ppd;
                theHoz.theHorizon[x].bearing = ((double)x) / ppd;
            }

            if (deleteTmp)
            {
                InputBitmap.Dispose();
                Console.WriteLine("Delete Temporary Downloaded PNG File " + PngFile);
                File.Delete(PngFile);
            }

            return;
        }


        public void TransformElevationsToDeclinations(double latitude)
        {
            // Azimuth Elevation calculate Declination
            // https://astronomy.stackexchange.com/questions/34086/how-to-calculate-parallactic-angle-from-a-fixed-alt-az-position/34104#34104
            // Declination - sinδ = sinθelsinθlat + cosθelcosθlatcosθaz,
            double θlat = latitude / 180 * Math.PI;

            for (int x = 0; x < theHoz.theHorizon.Length; x++)
            {
                // Refraction = 1.02 / TAN(RADIANS((H9 - J9 / 60) + 10.3 / ((H9 - J9 / 60) + 5.11)))
                // RefractionMinutes = 1 / TAN(RADIANS(AltitudeDegs + 7.31/(AltitudeDegs+4.4)))
                Double RefractionMinutes = 1 / Math.Tan((theHoz.theHorizon[x].elevation + 7.31 / (theHoz.theHorizon[x].elevation + 4.4)) / 180 * Math.PI);
                Double RefractionRad = RefractionMinutes / 60 / 180 * Math.PI;

                double θel = (theHoz.theHorizon[x].elevation / 180 * Math.PI ) - RefractionRad;
                double θaz = theHoz.theHorizon[x].bearing / 180 * Math.PI;

                double sinDeclination = Math.Sin(θel) * Math.Sin(θlat) + Math.Cos(θel) * Math.Cos(θlat) * Math.Cos(θaz);
                theHoz.theHorizon[x].declination = Math.Asin(sinDeclination) * 180 / Math.PI;
            }
        }


        public void SaveTransformedDeclinationsToPNG(string PngFile)
        {
            int height = 100 * pointsPerDegree;
            int yOrigin = height / 2;
            Image tmpMap = new Image(theHoz.theHorizon.Length, height);
            Graphics profGraph = Graphics.FromImage(tmpMap);
            SolidBrush brsh = new SolidBrush(Color.White);
            Pen pn = new Pen(brsh);

            for (int x = 0; x < theHoz.theHorizon.Length; x++)
            {
                profGraph.DrawLine(pn, x, height, x, (int)(yOrigin - theHoz.theHorizon[x].getY(true) * pointsPerDegree));
            }

            tmpMap.Save(PngFile);
        }


        public void calculateDistances()
        {
            for (int i = 0; i < theHoz.theHorizon.Length; i++)
            {
                if (theHoz.theHorizon[i].latitude != 0 && theHoz.theHorizon[i].longitude != 0)
                {
                    theHoz.theHorizon[i].distance = LatLonConversions.distHaversine(latitudeOrigin, longitudeOrigin, theHoz.theHorizon[i].latitude, theHoz.theHorizon[i].longitude);
                }
            }
        }


        public double GetWeight(int x, bool peak, double minDistance, bool useDeclinations)
        {
            double maxslope = 0;
            const int minDist = 20;
            int maxDegs = 2 * pointsPerDegree;
            double AbsMaxSlope = 0.8;

            for (int xx = x + minDist; xx < (x + maxDegs) && xx < theHoz.theHorizon.Length; xx++)
            {
                if (theHoz.theHorizon[xx].distance < minDistance)
                {
                    continue;
                }

                double slope = (theHoz.theHorizon[x].getY(useDeclinations) - theHoz.theHorizon[xx].getY(useDeclinations)) / (xx - x);
                if (!peak) slope = -slope;
                if (slope > AbsMaxSlope)
                {
                    slope = AbsMaxSlope;
                }

                if (slope > maxslope)
                {
                    maxslope = slope;
                }
            }

            for (int xx = x - minDist; xx > (x - maxDegs) && xx >= 0; xx--)
            {
                if (theHoz.theHorizon[xx].distance < minDistance)
                {
                    continue;
                }

                double slope = (theHoz.theHorizon[x].getY(useDeclinations) - theHoz.theHorizon[xx].getY(useDeclinations)) / (x - xx);
                if (!peak) slope = -slope;
                if (slope > AbsMaxSlope)
                {
                    slope = AbsMaxSlope;
                }
                if (slope > maxslope)
                {
                    maxslope = slope;
                }
            }

            return maxslope;
        }


        public void EvaluatePeaks(double minDistance, bool useDeclinations)
        {
            List<int> tmppeaks = peaks;
            List<int> tmpnotches = notches;

            if (useDeclinations)
            {
                tmppeaks = declinationpeaks;
                tmpnotches = declinationnotches;
            }

            List<PeakInfo> tmppeakList = new List<PeakInfo>();

            double maxWeight = 0;

            for (int x = 0; x < theHoz.theHorizon.Length; x++)
            {
                bool found = false;
                bool peak = false;

                if (tmppeaks.Contains(x))
                {
                    found = true;
                    peak = true;
                }

                if (tmpnotches.Contains(x))
                {
                    found = true;
                }

                if (found)
                {
                    double weight = GetWeight(x, peak, minDistance, useDeclinations);
                    if (weight > maxWeight)
                    {
                        maxWeight = weight;
                    }
                    tmppeakList.Add(new PeakInfo(theHoz.theHorizon[x], peak, weight, 0, x));
                }
            }

            // Normalise all weights at the end.
            foreach (var p in tmppeakList)
            {
                p.slopeWeight = p.slopeWeight / maxWeight;
            }

            double maxEvelDiff = 0;

            // Normalise all weights based on relative heights.
            for (int p = 0; p < tmppeakList.Count; p++)
            {
                double diff1 = 0;
                double diff2 = 0;

                if (p - 1 >= 0)
                {
                    diff1 = tmppeakList[p].horizonPoint.getY(useDeclinations) - tmppeakList[p - 1].horizonPoint.getY(useDeclinations);
                }
                else
                {
                    diff1 = 0;
                }

                if (p + 1 < tmppeakList.Count)
                {
                    diff2 = tmppeakList[p].horizonPoint.getY(useDeclinations) - tmppeakList[p + 1].horizonPoint.getY(useDeclinations);
                }
                else
                {
                    diff2 = 0;
                }

                if (!tmppeakList[p].isPeak)
                {
                    diff1 = -diff1;
                    diff2 = -diff2;
                }

                if (diff1 > diff2)
                {
                    tmppeakList[p].elevDiffWeight = diff1;
                }
                else
                {
                    tmppeakList[p].elevDiffWeight = diff2;
                }

                if (tmppeakList[p].elevDiffWeight > maxEvelDiff)
                {
                    maxEvelDiff = tmppeakList[p].elevDiffWeight;
                }
            }

            foreach (var p in tmppeakList)
            {
                p.elevDiffWeight = p.elevDiffWeight / maxEvelDiff;
            }


            if (useDeclinations)
            {
                declinationpeakList = tmppeakList;
            }
            else
            {
                peakList = tmppeakList;
            }
        }



        public void savePeaks(string filename, bool jsn, string csvPeakHeaderPrefix, string csvPeakDataPrefix, bool useDeclinations)
        {
            List<PeakInfo> tmppeakList = peakList;
            if (useDeclinations)
            {
                tmppeakList = declinationpeakList;
            }

            // serialize JSON directly to a file
            using (StreamWriter file = File.CreateText(filename))
            {
                if (jsn)
                {
                    Console.WriteLine("Create JSON Peak File " + filename);
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Formatting = Newtonsoft.Json.Formatting.Indented;
                    serializer.Serialize(file, tmppeakList);
                }
                else
                {
                    string heightstr = "HorizonElevation";
                    string weightStr = "SlopeWeight";
                    string qualitystr = "";
                    if (useDeclinations)
                    {
                        heightstr = "Declination";
                        weightStr = "SkimWidth";
                        qualitystr = ",Quality,NotchDeclin";
                    }
                    Console.WriteLine("Create CSV Peak File " + filename);
                    string csvHeader = csvPeakHeaderPrefix +
                        ",IsPeak," + weightStr + ",ElevDiffWeight,PeakIndex" +
                        ",HorizonLatitude,HorizonLogitude," + heightstr + ",HorizonDistance,HorizonBearing,Declination" + qualitystr;
                    file.WriteLine(csvHeader);

                    foreach (var peakInfo in tmppeakList)
                    {
                        string csvData = csvPeakDataPrefix + "," +
                            peakInfo.isPeak + "," +
                            peakInfo.slopeWeight + "," +
                            peakInfo.elevDiffWeight + "," +
                            peakInfo.indx + "," +
                            peakInfo.horizonPoint.latitude + "," +
                            peakInfo.horizonPoint.longitude + "," +
                            peakInfo.horizonPoint.getY(useDeclinations) + "," +
                            peakInfo.horizonPoint.distance + "," +
                            peakInfo.horizonPoint.bearing + "," +
                            peakInfo.horizonPoint.declination;
                        if (useDeclinations)
                        {
                            csvData += "," + peakInfo.slopeWeight * peakInfo.elevDiffWeight;
                            csvData += "," + (peakInfo.horizonPoint.getY(useDeclinations) - peakInfo.elevDiffWeight);
                        }
                        file.WriteLine(csvData);
                    }
                }
            }
        }


        public void EvaluateDeclinationPeaks(double minSkimAzDelta, double maxSkimAzDelta)
        {
            Console.WriteLine("EvaluateDeclinationPeaks " + minSkimAzDelta);

            for (int pIndx = 0; pIndx < declinationpeaks.Count; pIndx++)
            {
                int p = declinationpeaks[pIndx];
                bool foundLower = false;
                bool outofRange = false;

                double tnmtmt = theHoz.theHorizon[p].bearing;

                double dec = theHoz.theHorizon[p].getY(true);
                if (dec > 40 || dec < -40)
                {
                    outofRange = true;
                }

                double deltaAz = 0;
                double deltaDeclination = 0;

                if (p >= 180 * pointsPerDegree)
                {
                    int startX = (int)(p + minSkimAzDelta * pointsPerDegree);
                    int endX = (int)(p + maxSkimAzDelta * pointsPerDegree);

                    for (int x = startX; x < theHoz.theHorizon.Length && x <= endX; x++)
                    {
                        double dDeclin = theHoz.theHorizon[p].getY(true) - theHoz.theHorizon[x].getY(true);
                        if (dDeclin > 0)
                        {
                            foundLower = true;
                            deltaAz = Math.Abs((double)x - p) / pointsPerDegree;
                            if (Math.Abs(dDeclin) > deltaDeclination)
                            {
                                deltaDeclination = Math.Abs(dDeclin);
                            }
                        }
                    }
                }
                else
                {
                    int startX = (int)(p - minSkimAzDelta * pointsPerDegree);
                    int endX = (int)(p - maxSkimAzDelta * pointsPerDegree);

                    for (int x = startX; x >= 0 && x >= endX; x--)
                    {
                        double dDeclin = theHoz.theHorizon[p].getY(true) - theHoz.theHorizon[x].getY(true);
                        if (dDeclin > 0)
                        {
                            foundLower = true;
                            deltaAz = Math.Abs((double)x - p) / pointsPerDegree;
                            if (Math.Abs(dDeclin) > deltaDeclination)
                            {
                                deltaDeclination = Math.Abs(dDeclin);
                            }
                        }
                    }
                }

                if (foundLower && !outofRange)
                {
                    declinationpeakList.Add(new PeakInfo(theHoz.theHorizon[p], true, deltaAz, deltaDeclination, p));

                    //// Remove this peak from the list.
                    //declinationpeaks.Remove(p);
                }
            }
        }


        public void FindPeaks(double minAzDistance, double minElevDistance, double minDistance, bool useDeclinations)
        {
            List<int> tmppeaks = new List<int>();
            List<int> tmpnotches = new List<int>();
            bool done = false;

            bool found_rising = false;
            int start_rising = 0;
            bool found_falling = false;
            int start_falling = 0;

            // Get the RAW peaks from the horizon data - this includes all single pixel peaks so is very noisy.
            for (int x = 1; x < theHoz.theHorizon.Length - 1; x++)
            {
                if (theHoz.theHorizon[x].getY(useDeclinations) > (theHoz.theHorizon[x - 1].getY(useDeclinations)))
                {
                    found_rising = true;
                    start_rising = x;
                    found_falling = false;
                }

                if (theHoz.theHorizon[x].getY(useDeclinations) < (theHoz.theHorizon[x - 1].getY(useDeclinations)))
                {
                    found_rising = false;
                    found_falling = true;
                    start_falling = x;
                }

                if (found_rising && theHoz.theHorizon[x].getY(useDeclinations) > (theHoz.theHorizon[x + 1].getY(useDeclinations)))
                {
                    int peakX = (x + start_rising) / 2;
                    //Console.WriteLine("Peaks at " + (double)peakX / 120 + " " + theHoz.theHorizon[x].getY(useDeclinations));
                    tmppeaks.Add(peakX);
                }

                if (found_falling && theHoz.theHorizon[x].getY(useDeclinations) < (theHoz.theHorizon[x + 1].getY(useDeclinations)))
                {
                    int notchX = (x + start_falling) / 2;
                    //Console.WriteLine("Notches at " + (double)notchX / 120 + " " + theHoz.theHorizon[x].getY(useDeclinations));
                    tmpnotches.Add(notchX);
                }
            }


            //string passInfo = "Normal";

            for (int l = 0; l < 2; l++)
            {
                if (l == 1)
                {
                    minAzDistance = 360;
                    minElevDistance = 0.02;
                    //passInfo = "Noise Remove";
                }

                done = false;
                for (int j = 0; j < 40 && !done; j++)
                {
                    done = true;
                    // Now remove peaks only just above notches.
                    for (int i = 0; i < tmppeaks.Count - 1; i++)
                    {
                        int x1 = tmppeaks[i];
                        int x2 = tmppeaks[i + 1];
                        int notchx1 = -1;
                        int notchx2 = -1;

                        int removePeakX = -1;
                        int removeNotchX = -1;


                        // if Peaks are too far apart then skip.
                        if ((x2 - x1) / pointsPerDegree > minAzDistance)
                        {
                            //Console.WriteLine("Peaks far apart - skip " + (double)x1 / 120 + " " + (double)x2 / 120 + " Difference " + (x2 - x1) / pointsPerDegree);
                            continue;
                        }


                        // Which of the two peaks do we remove? We consider removing the lowest of the two peaks
                        if (theHoz.theHorizon[x2].getY(useDeclinations) > theHoz.theHorizon[x1].getY(useDeclinations))
                        {
                            removePeakX = x1;
                        }
                        else
                        {
                            removePeakX = x2;
                        }


                        // Find the highest notch on either side of the peak to remove
                        for (int k = 0; k < tmpnotches.Count; k++)
                        {
                            if (tmpnotches[k] < removePeakX)
                            {
                                notchx1 = tmpnotches[k];
                            }
                            if (tmpnotches[k] > removePeakX)
                            {
                                notchx2 = tmpnotches[k];
                                break;
                            }
                        }

                        if (notchx2 >= 0 && notchx1 >= 0)
                        {
                            // if notches are too far apart then skip.
                            if ((notchx2 - notchx1) / pointsPerDegree > minAzDistance)
                            {
                                //Console.WriteLine("Notches far apart " + (double)notchx1 / 120 + " " + (double)notchx2 / 120 + " Difference " + (notchx2 - notchx1) / pointsPerDegree);
                                continue;
                            }
                        }

                        // Which of the two notches do we remove? We consider removing the highest of the two notches
                        if (notchx1 >= 0 && notchx2 >= 0 && theHoz.theHorizon[notchx1].getY(useDeclinations) > theHoz.theHorizon[notchx2].getY(useDeclinations))
                        {
                            removeNotchX = notchx1;
                        }
                        else
                        {
                            removeNotchX = notchx2;
                        }

                        if (removeNotchX > 0 && removePeakX > 0)
                        {
                            if (theHoz.theHorizon[removePeakX].getY(useDeclinations) - theHoz.theHorizon[removeNotchX].getY(useDeclinations) < minElevDistance)
                            {
                                //Console.WriteLine(passInfo + " Removed Peak and Notch at " + (double)removePeakX / 120 + " " + theHoz.theHorizon[removePeakX].getY(useDeclinations) + " Notch " + (double)removeNotchX / 120 + " " + theHoz.theHorizon[removeNotchX].getY(useDeclinations) + " Leaving " + (double)x2 / 120 + " Difference " + (theHoz.theHorizon[removePeakX].getY(useDeclinations) - theHoz.theHorizon[removeNotchX].getY(useDeclinations)));
                                tmppeaks.Remove(removePeakX);
                                tmpnotches.Remove(removeNotchX);
                                done = false;
                            }
                        }
                    }
                }
            }


            // Remove peaks and notches too close to the observer.
            for (int i = tmppeaks.Count - 1; i >= 0; i--)
            {
                int x = tmppeaks[i];
                if (theHoz.theHorizon[x].distance > 0 && theHoz.theHorizon[x].distance < minDistance)
                {
                    //Console.WriteLine("Removed Peak at " + (double)x / 120 + " " + theHoz.theHorizon[x].elevation + " Distance " + theHoz.theHorizon[x].distance);
                    tmppeaks.Remove(x);
                }
            }

            for (int i = tmpnotches.Count - 1; i >= 0; i--)
            {
                int x = tmpnotches[i];
                if (theHoz.theHorizon[x].distance > 0 && theHoz.theHorizon[x].distance < minDistance)
                {
                    //Console.WriteLine("Removed Notch at " + (double)x / 120 + " " + theHoz.theHorizon[x].elevation + " Distance " + theHoz.theHorizon[x].distance);
                    tmpnotches.Remove(x);
                }
            }


            //for (int i = 0; i < peaks.Count; i++)
            //{
            //    int x = peaks[i];
            //    Console.WriteLine("Peaks " + (double)x / 120 + " " + theHoz.theHorizon[x].elevation);
            //}

            //for (int i = 0; i < notches.Count; i++)
            //{
            //    int x = notches[i];
            //    Console.WriteLine("Notches " + (double)x / 120 + " " + theHoz.theHorizon[x].elevation);
            //}

            if (useDeclinations)
            {
                //        List<PeakInfo> peakList = new List<PeakInfo>();
                //public List<int> declinationpeaks = new List<int>();
                //public List<int> declinationnotches
                declinationpeaks = tmppeaks;
                //declinationnotches = tmpnotches; For declinations, we don't care about notches.
            }
            else
            {
                peaks = tmppeaks;
                notches = tmpnotches;
            }
        }
    }


}
