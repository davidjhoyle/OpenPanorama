using PanaGraph;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using Newtonsoft.Json;


namespace OpenPanoramaLib
{
    public class StellariumHelper
    {
        const string grnd = "ground.png";

        static public StoneSite[] ReadJSONSites(string fileName)
        {
            // "\"Name\": \"" + cleanfilename(bits[0]) + "\",");
            // "\"MonType\": \"" + cleanfilename(bits[1]) + "\",");
            // "\"County\": \"" + cleanfilename(bits[2]) + "\",");
            // "\"GridRef\": \"" + bits[3] + "\",");
            // "\"Filename\": \"" + cleanfilename(bits[0] + "_" + bits[3]).ToLower() + "\",");
            // "\"LatLon\": \"" + lat + " " + lon + "\",");
            // "\"Height\": \"" + (int)height + "\",");
            // "\"Description\": \"" + cleanfilename(bits[0] + " " + bits[1] + " in " + bits[2]) + " " + lat + " " + lon + "\",");

            // "\"SourceURL\": \"" + srcurl + "\",");
            // "\"SourceComment\": \"" + srccomment + "\"");

            StoneSite[] sites = null;

            for (int i = 0; i < 10; i++)
            {
                try
                {
                    using (StreamReader fr = File.OpenText(fileName))
                    {
                        JsonSerializer serializer = new JsonSerializer();
                        sites = (StoneSite[])serializer.Deserialize(fr, typeof(StoneSite[]));
                    }
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("ReadJSONSites " + fileName + " threw Exception - retry in 1 sec " + ex.Message + " " + ex.StackTrace);
                    Thread.Sleep(1000);
                }
            }

            return sites;
        }


        static public void CreateStellariumIniFile(StoneSite site, string folder, int count, string ZipFolder)
        {
            //[landscape]
            //name = Pennal-Church
            //type = old_style
            //author = David Hoyle
            //description = pennal church stone circle SH6997300385
            //nbsidetex = 8
            //tex0 = pennal-church_sh6997300385_0.png
            //tex1 = pennal - church_sh6997300385_1.png
            //tex2 = pennal-church_sh6997300385_2.png
            //tex3 = pennal - church_sh6997300385_3.png
            //tex4 = pennal-church_sh6997300385_4.png
            //tex5 = pennal - church_sh6997300385_5.png
            //tex6 = pennal-church_sh6997300385_6.png
            //tex7 = pennal - church_sh6997300385_7.png
            //nbside = 8
            //side0 = tex0:0:0.00:1:1
            //side1 = tex1:0:0.00:1:1
            //side2 = tex2:0:0.00:1:1
            //side3 = tex3:0:0.00:1:1
            //side4 = tex4:0:0.00:1:1
            //side5 = tex5:0:0.00:1:1
            //side6 = tex6:0:0.00:1:1
            //side7 = tex7:0:0.00:1:1
            //groundtex = ground.png
            //ground = groundtex:0:0:1:1
            //nb_decor_repeat = 1
            //decor_alt_angle = 28
            //decor_angle_shift = -8
            //decor_angle_rotatez = -90
            //ground_angle_shift = -8
            //ground_angle_rotatez = 0
            //fog_alt_angle = 35
            //fog_angle_shift = -25
            //draw_ground_first = 1
            //tan_mode = true
            //[location]
            //planet = Earth
            //latitude = +48d15'36"
            //longitude = +11d40'15"
            //altitude = 443
            //timezone = Europe/Berlin

            string INIFile = ZipFolder + "\\landscape.ini";
            using (StreamWriter outfile = new System.IO.StreamWriter(INIFile))
            {
                Console.WriteLine("Create INI File " + INIFile);

                outfile.WriteLine("[landscape]");
                outfile.WriteLine("name = " + site.Name);
                outfile.WriteLine("type = old_style");
                outfile.WriteLine("author = David Hoyle");
                outfile.WriteLine("description = " + site.Description);
                outfile.WriteLine("nbsidetex = " + count);
                for (int x = 0; x < count; x++)
                {
                    outfile.WriteLine("tex" + x + " = " + GetPNGFilename(site.Filename, x, null));
                }
                outfile.WriteLine("nbside = " + count);
                for (int x = 0; x < count; x++)
                {
                    outfile.WriteLine("side" + x + " = tex" + x + ":0:0:1:1");
                }
                outfile.WriteLine("groundtex = ground.png");
                outfile.WriteLine("ground = groundtex:0:0:1:1");
                outfile.WriteLine("nb_decor_repeat = 1");
                outfile.WriteLine("decor_alt_angle = 25");
                outfile.WriteLine("decor_angle_shift = -5");
                outfile.WriteLine("decor_angle_rotatez = -90");
                outfile.WriteLine("ground_angle_shift = -5");
                outfile.WriteLine("ground_angle_rotatez = 0");
                outfile.WriteLine("fog_alt_angle = 35");
                outfile.WriteLine("fog_angle_shift = 25");
                outfile.WriteLine("draw_ground_first = 1");
                outfile.WriteLine("tan_mode = false");
                outfile.WriteLine("calibrated = true");
                outfile.WriteLine("fogtex = foggy.png");
                outfile.WriteLine("fog = fogtex:0:0:1:1");


                char[] splitter = new char[] { ' ' };
                string[] latlotstrs = site.LatLon.Split(splitter);
                outfile.WriteLine("[location]");
                outfile.WriteLine("planet = Earth");
                outfile.WriteLine("latitude = " + latlotstrs[0]);
                outfile.WriteLine("longitude = " + latlotstrs[1]);
                outfile.WriteLine("altitude = " + (int)(Convert.ToDouble(site.Height)));

                outfile.Close();

                Console.WriteLine("Completed INI File " + INIFile);
            }
        }





        static string GetPNGFilename(string filename, int x, string ZipFolder)
        {
            string tmp = "";
            if (ZipFolder != null)
            {
                tmp += ZipFolder + "\\";
            }

            return (tmp + filename + "_" + x + ".png").ToLower();
        }




        static public void CreateStellariumFogFile(string ZipFolder)
        {
            string PNGFile = (ZipFolder + "\\foggy.png").ToLower();
            Image foggy = new Image(100, 100);
            Graphics gr = Graphics.FromImage(foggy);

            for (int y = 0; y < foggy.Height; y++)
            {
                Color col = Color.FromArgb(foggy.Height - y, foggy.Height - y, foggy.Height - y);
                Pen pn = new Pen(col);
                gr.DrawLine(pn, 0, y, foggy.Width, y);
            }

            foggy.Save(PNGFile);
            Console.WriteLine("Created PNG File " + PNGFile);
        }


        static public void CreateStellariumPNGFiles(StoneSite site, string folder, int count, string ZipFolder, RunJobParams rjp)
        {
            string PNGFile = "";

            if (folder != null && folder.Length > 0)
            {
                PNGFile += folder + "\\";
            }

            PNGFile = (PNGFile + site.Filename + ".png").ToLower();
            PNGFile = PaintImage.cleanfilename(PNGFile);
            Console.WriteLine("Read PNG File " + PNGFile);
            Image InputBitmap = new Image(PNGFile);
            Image[] outbitmap = new Image[count];
            Graphics InputGr = Graphics.FromImage(InputBitmap);
            Rectangle srcRect;

            int ReticleHeight = 2;

            int outputHeight = InputBitmap.Height - ReticleHeight * rjp.pixels;

            Console.Write("Creating PNG Files");

            for (int x = 0; x < count; x++)
            {
                outbitmap[x] = new Image(InputBitmap.Width / count, outputHeight);
                srcRect = new Rectangle(x * InputBitmap.Width / count, 0, InputBitmap.Width / count + 5, outputHeight + 5);

                Graphics OutGr = Graphics.FromImage(outbitmap[x]);

                OutGr.DrawImage(InputBitmap, 0, 0, srcRect, GraphicsUnit.Pixel);

                // Copyright notice...
                int smalltxtheight = 20;
                SolidBrush brsh = new SolidBrush(PaintImage.ColourTitle);
                OutGr.DrawString(PaintImage.CopyrightNotice, new Font(FontFamily.GenericSansSerif, smalltxtheight, FontStyle.Regular), brsh, 10, outputHeight - 40);

                string filename = GetPNGFilename(site.Filename, x, ZipFolder);
                outbitmap[x].Save(filename);
                Console.Write(" " + x);
            }
            Console.WriteLine("");

            // Create the Ground
            Image grndBit = new Image(16, 16);
            Rectangle dstRect = new Rectangle(0, 0, 16, 16);
            srcRect = new Rectangle(200, outputHeight - 18, 16, 16);


            // InputBitmap.Height - 4 - (120 * 2), 4, 3);
            Graphics grndGr = Graphics.FromImage(grndBit);
            grndGr.DrawImage(InputBitmap, 0, 0, srcRect, GraphicsUnit.Pixel);
            grndBit.Save(ZipFolder + "\\" + grnd);

            Console.WriteLine("Created PNG File " + grnd);

            CreateStellariumFogFile(ZipFolder);
        }



        static void deletefolder(string ZipFolder)
        {
            try
            {
                if (Directory.Exists(ZipFolder))
                {
                    Console.WriteLine("Delete Temporary Folder and Files " + ZipFolder);
                    foreach (var file in Directory.EnumerateFiles(ZipFolder))
                    {
                        //Console.WriteLine("Delete Files " + file);
                        File.Delete(file);
                    }
                    Console.WriteLine("Delete folder " + ZipFolder);
                    Directory.Delete(ZipFolder);
                }
            }
            catch (Exception ex)
            {
                Console.Write("Delet Folder Exception " + ZipFolder + " Exception " + ex.Message + " " + ex.StackTrace);
            }
        }



        static public bool CheckStellariumFile(bool siteFolder, StoneSite site, string basefolder)
        {
            string SiteFolder = PaintImage.getFolderName(siteFolder, site.County, site.MonType, site.Name, site.GridRef);
            string zipFile = SiteFolder + "\\" + site.Filename + ".zip";
            zipFile = zipFile.Replace("--", "-").Replace("--", "-");

            if (File.Exists(zipFile))
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        static public bool DeleteStellariumFile(bool siteFolder, StoneSite site, string basefolder)
        {
            string SiteFolder = PaintImage.getFolderName(siteFolder, site.County, site.MonType, site.Name, site.GridRef);
            string zipFile = SiteFolder + "\\" + site.Filename + ".zip";
            zipFile = zipFile.Replace("--", "-").Replace("--", "-");

            if (File.Exists(zipFile))
            {
                File.Delete(zipFile);
                return true;
            }
            else
            {
                return false;
            }
        }



        static public void CreateStellariumZipFile(bool siteFolder, StoneSite site, string basefolder, RunJobParams rjp)
        {
            int NumOutputImages = 8;

            string SiteFolder = PaintImage.getFolderName(siteFolder, site.County, site.MonType, site.Name, site.GridRef);

            string zipFile = "";

            if (SiteFolder.Length > 0)
            {
                zipFile = SiteFolder + "\\";
            }
            zipFile += site.Filename + ".zip";
            zipFile = zipFile.Replace("--", "-").Replace("--", "-");

            string tmpZip = site.Filename + ".zip";

            string tmpZipFolder = "tmpppything_" + Process.GetCurrentProcess().Id + "_" + Thread.CurrentThread.ManagedThreadId;

            deletefolder(tmpZipFolder);
            try
            {
                Directory.CreateDirectory(tmpZipFolder);

                CreateStellariumPNGFiles(site, SiteFolder, NumOutputImages, tmpZipFolder, rjp);
                CreateStellariumIniFile(site, SiteFolder, NumOutputImages, tmpZipFolder);

                Console.WriteLine("Create ZIP File " + zipFile);
                ZipFile.CreateFromDirectory(tmpZipFolder, zipFile);
            }
            finally
            {
                deletefolder(tmpZipFolder);
            }
        }
    }
}
