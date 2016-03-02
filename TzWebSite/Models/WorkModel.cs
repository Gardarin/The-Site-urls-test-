using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.IO;

namespace TzWebSite.Models
{
    public class WorkModel
    {
        public List<SiteTest> _tests;

        public WorkModel()
        {
            _tests = new List<SiteTest>();
        }

        public void CreateSiteTest(string name, string path)
        {
            SiteTest siteTest = new SiteTest();
            siteTest.Name = name;
            siteTest.Path = path;
            siteTest.TestResults = new List<TestResult>();

            using (var context = new SiteTestsContext())
            {
                context.SiteTests.Add(siteTest);
                context.SaveChanges();
            }
        }

        public void AddTestResult(string name, string path, List<TestResult> results)
        {
            using (var context = new SiteTestsContext())
            {
                context.SiteTests.First(s => s.Name == name && s.Path == path).TestResults = results;
                context.SaveChanges();
            }
        }

        public void DelSiteTest(string name, string path)
        {
            using (var context = new SiteTestsContext())
            {
                context.SiteTests.Remove(context.SiteTests.First(s => s.Name == name && s.Path == path));
                context.SaveChanges();
            }
        }

        public List<SiteTest> GetTests()
        {
            using(var context = new SiteTestsContext())
            {
                _tests = context.SiteTests.Include("TestResults").OrderBy(x => x.Name).ToList();
            }
            return _tests;
        }

        //creating diagram
        public byte[] GetGraphic(string name, string path)
        {
            SiteTest siteTest;
            using (var context = new SiteTestsContext())
            {
                siteTest = context.SiteTests.Include("TestResults").First(x => x.Name == name && x.Path == path);

                _tests = context.SiteTests.Include("TestResults").OrderBy(x => x.Name).ToList();

            }
            PictureBox picture=new PictureBox();
            Bitmap bitmap = new Bitmap(10*15, 200);
            picture.Image = bitmap;
            Graphics graphix = Graphics.FromImage(picture.Image);
            Pen pen = new Pen(Brushes.Red);
            pen.Width = 15;
            graphix.Clear(Color.Black);
            
            int i=7;
            int count = 0;
            foreach (var r in siteTest.TestResults.OrderByDescending(x=>Convert.ToInt32(x.Time)))
            {
                count++;
                
                int hight = Convert.ToInt32(r.Time)/5;
                graphix.DrawLine(pen, i, 200, i, 200-hight);
                graphix.DrawString(""+(count-1), new Font("Arial", 12),Brushes.Black, i-7, 180);
                i += 15;
                if (count == 10)
                {
                    break;
                }
            }

            MemoryStream memoryStream = new MemoryStream();
            picture.Image.Save(memoryStream, ImageFormat.Bmp);
            byte[] bitmapRecord = memoryStream.ToArray();

            return bitmapRecord;
        }

        
    }
}