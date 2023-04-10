using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;


using Newtonsoft.Json;


namespace OpenPanoramaLib
{
    public class RisingAndSettingPositionsAndCorrelation
    {
        XmlDocument RASPGPX = new XmlDocument();
        public string gpxfilename;
        public List<RisingAndSettingPoint> RisingAndSettingPoints = null;
        public List<SkimmingPath> SkimmingPoints = null;
        public string siteName = null;
        public string sitedDescription = null;
        public double latitude = 0;
        public double longitude = 0;

        public RisingAndSettingPositionsAndCorrelation()
        {
        }


        public void detectSkimmingPoints(double minDistance, double minAzDelta)
        {
            Dictionary<string, List<RisingAndSettingPoint>> skimminglists = new Dictionary<string, List<RisingAndSettingPoint>>();
            SkimmingPoints = new List<SkimmingPath>();

            foreach (var pt in RisingAndSettingPoints)
            {
                string posneg = pt.setting ? "Set" : "Rise";
                string name = posneg + "_" + pt.heavenlyTrace + "_" + pt.tcb;
                if (!skimminglists.ContainsKey(name))
                {
                    skimminglists.Add(name, new List<RisingAndSettingPoint>());
                }
                skimminglists[name].Add(pt);
            }

            foreach (string posskim in skimminglists.Keys)
            {
                if (skimminglists[posskim].Count > 1)
                {
                    int minAzIndex = 0;
                    int maxAzIndex = 0;
                    double minAz = skimminglists[posskim][0].bearing;
                    double maxAz = skimminglists[posskim][0].bearing;

                    string posnegOpposite = skimminglists[posskim][0].setting ? "Rise" : "Set";
                    posnegOpposite += "_" + skimminglists[posskim][0].heavenlyTrace + "_" + skimminglists[posskim][0].tcb;

                    double asymetricAzMin = 0;
                    if (!skimminglists.ContainsKey(posnegOpposite))
                    {
                        asymetricAzMin = 180;
                        for (int skimmy = 0; skimmy < skimminglists[posskim].Count; skimmy++)
                        {
                            RisingAndSettingPoint pt = skimminglists[posskim][skimmy];
                            double tmpdelta = Math.Abs(pt.bearing - 180);

                            if (tmpdelta < asymetricAzMin)
                            {
                                asymetricAzMin = tmpdelta + minAzDelta;
                            }
                        }
                    }


                    for (int skimmy = 1; skimmy < skimminglists[posskim].Count; skimmy++)
                    {
                        RisingAndSettingPoint pt = skimminglists[posskim][skimmy];
                        double dist = LatLonConversions.distHaversine(latitude, longitude, skimminglists[posskim][skimmy].latitude, skimminglists[posskim][skimmy].longitude);
                        if (dist <= minDistance)
                        {
                            continue;
                        }

                        double tmpdelta = Math.Abs(pt.bearing - 180);

                        if (pt.bearing > maxAz && tmpdelta > asymetricAzMin)
                        {
                            maxAz = pt.bearing;
                            minAzIndex = skimmy;
                        }
                        if (pt.bearing < minAz && tmpdelta > asymetricAzMin)
                        {
                            minAz = pt.bearing;
                            maxAzIndex = skimmy;
                        }
                    }


                    if (maxAz - minAz > minAzDelta)
                    {
                        double dist1 = LatLonConversions.distHaversine(latitude, longitude, skimminglists[posskim][minAzIndex].latitude, skimminglists[posskim][minAzIndex].longitude);
                        double dist2 = LatLonConversions.distHaversine(latitude, longitude, skimminglists[posskim][maxAzIndex].latitude, skimminglists[posskim][maxAzIndex].longitude);
                        if (dist1 > minDistance && dist2 > minDistance)
                        {
                            SkimmingPath sp = new SkimmingPath();
                            sp.startPath = skimminglists[posskim][minAzIndex];
                            sp.endPath = skimminglists[posskim][maxAzIndex];
                            SkimmingPoints.Add(sp);
                        }
                    }
                }
            }
        }




        public void saveSkimmingPoints(string filename, bool csv, bool json, string csvheaderprefix, string csvprefix)
        {
            if (filename != null && filename.Length > 0)
            {
                if (json)
                {
                    // serialize JSON directly to a file
                    using (StreamWriter file = File.CreateText(filename + ".jsn"))
                    {
                        Console.WriteLine("Create JSON Skim File " + filename + ".jsn");

                        JsonSerializer serializer = new JsonSerializer();
                        serializer.Formatting = Newtonsoft.Json.Formatting.Indented;
                        serializer.Serialize(file, SkimmingPoints);
                    }
                }

                if (csv)
                {
                    // serialize CSV directly to a file
                    using (StreamWriter file = File.CreateText(filename + ".csv"))
                    {
                        Console.WriteLine("Create CSV Skim File " + filename + ".csv");

                        string csvHeader = csvheaderprefix + ",Body,TCB,Setting,Bearing1,Elevation1,Latitude1,Longitude1,Bearing2,Elevation2,Latitude2,Longitude2";
                        file.WriteLine(csvHeader);

                        foreach (var rasp in SkimmingPoints)
                        {
                            string csvData = csvprefix + "," +
                                rasp.startPath.heavenlyTrace + "," +
                                rasp.startPath.tcb + "," +
                                rasp.startPath.setting + "," +
                                rasp.startPath.bearing + "," +
                                rasp.startPath.elevation + "," +
                                rasp.startPath.latitude + "," +
                                rasp.startPath.longitude + "," +
                                rasp.endPath.bearing + "," +
                                rasp.endPath.elevation + "," +
                                rasp.endPath.latitude + "," +
                                rasp.endPath.longitude;
                            file.WriteLine(csvData);
                        }
                    }
                }
            }
        }



        public void readRisingAndSettingsGPXData(string GpxFile)
        {
            Console.WriteLine("readRisingAndSettingsGPXData " + GpxFile);
            char[] splitter = { ' ' };

            RASPGPX = new XmlDocument();
            RASPGPX.Load(GpxFile);

            RisingAndSettingPoints = new List<RisingAndSettingPoint>();

            var RAS_pts = RASPGPX.ChildNodes[1];
            double tmplatitude = 0;
            double tmplongitude = 0;

            int i = 0;
            foreach (XmlNode node in RAS_pts.ChildNodes)
            {
                if (node.NodeType == XmlNodeType.Element)
                {
                    string raspname = node.Value;
                    if (raspname == null)
                    {
                        raspname = node.InnerText;
                    }

                    raspname = raspname.Trim();

                    if (node.Attributes != null)
                    {
                        foreach (XmlAttribute att in node.Attributes)
                        {
                            if (att.Name == "lat")
                            {
                                tmplatitude = Convert.ToDouble(att.Value);
                            }
                            if (att.Name == "lon")
                            {
                                tmplongitude = Convert.ToDouble(att.Value);
                            }
                        }
                    }

                    string[] bits = raspname.Split(splitter);
                    if (i == 0)
                    {
                        latitude = tmplatitude;
                        longitude = tmplongitude;
                        siteName = bits[0];
                        sitedDescription = raspname;
                    }
                    else
                    {
                        RisingAndSettingPoint rasp = new RisingAndSettingPoint();

                        // WinterSolsticeSun Top 3 Bearing 222.384415683264 Elevation 0.467287458227328 lat/lon 57.0301649117403,-6.30843845378634
                        rasp.name = raspname;
                        rasp.heavenlyTrace = bits[0];
                        rasp.tcb = bits[1];
                        rasp.index = Convert.ToInt32(bits[2]);
                        rasp.bearing = Convert.ToDouble(bits[4]);
                        rasp.elevation = Convert.ToDouble(bits[6]);
                        rasp.setting = (rasp.bearing >= 180);
                        rasp.longitude = tmplongitude;
                        rasp.latitude = tmplatitude;

                        RisingAndSettingPoints.Add(rasp);
                    }

                    i++;
                }
            }

            return;
        }
    }

}
