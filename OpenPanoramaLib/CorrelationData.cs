using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace OpenPanoramaLib
{
    public class CorrelationData
    {
        List<CorrelationPoint> correlations = new List<CorrelationPoint>();

        public void CorrelatePeaksAndRASP(List<PeakInfo> peakList, List<RisingAndSettingPoint> rasps,
            double minCorrelationValue, double minCorrelationValueSlopeWeighted, double minCorrelationValueElevDiffWeighted, double minCorrelationValueWeighted)
        {
            for (int r = 0; r < rasps.Count; r++)
            {
                for (int p = 0; p < peakList.Count; p++)
                {
                    double deltaBearing = Math.Abs(peakList[p].horizonPoint.bearing - rasps[r].bearing);
                    // in case to protect from div zero and also to protect again cases where the delta is almost zero and it gives too much weight.
                    // We set the minimum delta to 3 minutes of Arc - nothing is considered more accurate than this.
                    double MinDeltaDegrees = 3.0 / 60;
                    if (deltaBearing < MinDeltaDegrees)
                    {
                        deltaBearing = MinDeltaDegrees;
                    }

                    double weightAdjust = 1;

                    // We concentrate on peaks more than nothces 
                    if (!peakList[p].isPeak)
                    {
                        weightAdjust = weightAdjust / 8;
                    }

                    // We care about the top of the sun more than middle or bottom.
                    if (rasps[r].heavenlyTrace.ToLower().Contains("sun") && rasps[r].tcb.ToLower() != "top")
                    {
                        weightAdjust = weightAdjust / 4;
                    }

                    double correlationValue = weightAdjust / (deltaBearing * deltaBearing);
                    double correlationValueSlopeWeighted = peakList[p].slopeWeight * peakList[p].slopeWeight * correlationValue;
                    double correlationValueElevDiffWeighted = peakList[p].elevDiffWeight * correlationValue;
                    double correlationValueWeighted = peakList[p].overallWeight * correlationValue;

                    if (correlationValue > minCorrelationValue &&
                        correlationValueSlopeWeighted > minCorrelationValueSlopeWeighted &&
                        correlationValueElevDiffWeighted > minCorrelationValueElevDiffWeighted &&
                        correlationValueWeighted > minCorrelationValueWeighted)
                    {
                        int bearing1 = (int)peakList[p].horizonPoint.bearing;
                        int bearing2 = (int)rasps[r].bearing;


                        CorrelationPoint point = new CorrelationPoint();
                        point.deltaBearing = deltaBearing;
                        point.correlationValue = correlationValue;
                        point.correlationValueSlopeWeighted = correlationValueSlopeWeighted;
                        point.correlationValueElevDiffWeighted = correlationValueElevDiffWeighted;
                        point.correlationValueWeighted = correlationValueWeighted;
                        point.peakInfo = peakList[p];
                        point.rasp = rasps[r];

                        int insertionPoint = 0;
                        for (int c = 0; c < correlations.Count; c++)
                        {
                            if (correlations[c].correlationValueWeighted > point.correlationValueWeighted)
                            {
                                insertionPoint = c + 1;
                            }
                        }

                        correlations.Insert(insertionPoint, point);
                    }
                }
            }
        }



        public void saveCorrelations(string filename, string summaryfilename, bool csv, bool json, string csvheaderprefix, string csvprefix)
        {
            if (summaryfilename != null && summaryfilename.Length > 0)
            {
                CorrelationSummary cs = new CorrelationSummary();

                foreach (var c in correlations)
                {
                    cs.totalCorrelationValue += c.correlationValue;
                    cs.totalCorrelationValueSlopeWeighted += c.correlationValueSlopeWeighted;
                    cs.totalCorrelationValueElevDiffWeighted += c.correlationValueElevDiffWeighted;
                    cs.totalCorrelationValueWeighted += c.correlationValueWeighted;
                }

                if (json)
                {
                    // serialize JSON directly to a file
                    using (StreamWriter file = File.CreateText(summaryfilename + ".jsn"))
                    {
                        Console.WriteLine("Create JSON Summary Correlations File " + summaryfilename + ".jsn");

                        JsonSerializer serializer = new JsonSerializer();
                        serializer.Formatting = Newtonsoft.Json.Formatting.Indented;
                        serializer.Serialize(file, cs);
                    }
                }

                if (csv)
                {
                    // serialize CSV directly to a file
                    using (StreamWriter file = File.CreateText(summaryfilename + ".csv"))
                    {
                        Console.WriteLine("Create CSV Summary Correlations File " + summaryfilename + ".csv");

                        string csvHeader = csvheaderprefix +
                                ",CorrelationValueWeighted,CorrelationValue,SlopeWeighted,ElevDiffWeighted";
                        file.WriteLine(csvHeader);

                        string csvData = csvprefix + "," +
                            cs.totalCorrelationValueWeighted + "," +
                            cs.totalCorrelationValue + "," +
                            cs.totalCorrelationValueSlopeWeighted + "," +
                            cs.totalCorrelationValueElevDiffWeighted;
                        file.WriteLine(csvData);
                    }
                }
            }

            if (filename != null && filename.Length > 0)
            {
                if (json)
                {
                    // serialize JSON directly to a file
                    using (StreamWriter file = File.CreateText(filename + ".jsn"))
                    {
                        Console.WriteLine("Create JSON Correlations File " + filename + ".jsn");

                        JsonSerializer serializer = new JsonSerializer();
                        serializer.Formatting = Newtonsoft.Json.Formatting.Indented;
                        serializer.Serialize(file, correlations);
                    }
                }

                if (csv)
                {
                    // serialize CSV directly to a file
                    using (StreamWriter file = File.CreateText(filename + ".csv"))
                    {
                        Console.WriteLine("Create CSV Correlations File " + filename + ".csv");

                        string csvHeader = csvheaderprefix +
                                ",CorrelationValueWeighted,CorrelationValue,SlopeWeighted,ElevDiffWeighted,DeltaBearing" +
                                ",RASPLatitude,RASPLongitude,Body,TCB,RASPBearing,RASPElevation,RASPSetting" +
                                ",IsPeak,SlopeWeight,ElevDiffWeaight,PeakIndex" +
                                ",HorizonLatitude,HorizonLongitude,HorizonElevation,HorizonDistance,HorizonBearing";
                        file.WriteLine(csvHeader);

                        foreach (var c in correlations)
                        {
                            string csvData = csvprefix + "," +
                                c.correlationValueWeighted + "," +
                                c.correlationValue + "," +
                                c.correlationValueSlopeWeighted + "," +
                                c.correlationValueElevDiffWeighted + "," +
                                c.deltaBearing + "," +
                                c.rasp.latitude + "," +
                                c.rasp.longitude + "," +
                                c.rasp.heavenlyTrace + "," +
                                c.rasp.tcb + "," +
                                c.rasp.bearing + "," +
                                c.rasp.elevation + "," +
                                c.rasp.setting + "," +
                                c.peakInfo.isPeak + "," +
                                c.peakInfo.slopeWeight + "," +
                                c.peakInfo.elevDiffWeight + "," +
                                c.peakInfo.indx + "," +
                                c.peakInfo.horizonPoint.latitude + "," +
                                c.peakInfo.horizonPoint.longitude + "," +
                                c.peakInfo.horizonPoint.elevation + "," +
                                c.peakInfo.horizonPoint.distance + "," +
                                c.peakInfo.horizonPoint.bearing;
                            file.WriteLine(csvData);
                        }
                    }
                }
            }
        }
    }
}
