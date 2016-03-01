using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Net;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

using TzWebSite.Models;

namespace TzWebSite.Controllers
{
    public class SiteTestController : Controller
    {
        static WorkModel _workModel = new WorkModel();
        
        // GET: SiteTest
        public ActionResult Index()
        {
            return View(_workModel);
        }

        // GET: SiteTest/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: SiteTest/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            string curentUrl = collection["Path"];
            
            if (!IsValidUrl(curentUrl))
            {
                return View();
            }

            //finding links
            List<string> urls = GetUrls(collection["Path"]);

            //sites testing
            List<TestResult> testResults = CreateTestResults(urls);

            curentUrl = UrlValidation(curentUrl);
            //storage in DB
            _workModel.CreateSiteTest(collection["Name"], curentUrl);
            _workModel.AddTestResult(collection["Name"], curentUrl, testResults);

            return RedirectToAction("Index");
        }

        private List<string> GetUrls(string curentUrl)
        {
            string oldUrl = curentUrl;
            curentUrl = UrlValidation(curentUrl);
            //get html document
            HtmlWeb hw = new HtmlWeb();
            DateTime dStart = DateTime.Now;
            HtmlDocument document = hw.Load(curentUrl);
            List<string> urls = new List<string>();

            

            //finding links
            foreach (HtmlNode link in document.DocumentNode.SelectNodes("//a[@href]"))
            {
                HtmlAttribute att = link.Attributes["href"];
                string curentLink = "";

                if (IsValidUrl(att.Value))
                {
                    try
                    {
                        curentLink = CreatAbsoluteUrlFromRelative(oldUrl, att.Value);
                        urls.Add(curentLink);
                    }
                    catch (UriFormatException e)
                    {
                        continue;
                    }
                }

            }

            return urls;
        }

        //The site urls testing 
        private List<TestResult> CreateTestResults(List<string> urls)
        {
            List<TestResult> testResults = new List<TestResult>();
            TestResult tRes;
            TimeSpan timeSpan = new TimeSpan();
            DateTime dStart = DateTime.Now;
            int time = 0;
            WebRequest req;
            HtmlWeb hw = new HtmlWeb();
            
            foreach (var link in urls)
            {
                try
                {
                    req = WebRequest.CreateHttp(link);
                    req.Timeout = 12000;
                    req.Method = "get";

                    dStart = DateTime.Now;
                    req.GetResponse();

                    timeSpan = DateTime.Now - dStart;
                    time = timeSpan.Milliseconds;

                    tRes = new TestResult();
                    tRes.Url = link;
                    tRes.Time = "" + time;
                    testResults.Add(tRes);
                }
                catch (NotSupportedException)
                {
                    continue;
                }
                catch (UriFormatException e)
                {
                    continue;
                }
                catch (WebException e)
                {
                    continue;
                }
            }
            return testResults;
        }

        //Create absolute baseUrl
        private string UrlValidation(string url )
        {
            if (Uri.IsWellFormedUriString(url, UriKind.Relative))
            {
                UriBuilder myURI = new UriBuilder("http", url, 80);
                url = myURI.Uri.AbsoluteUri;
            }

            return url;
        }

        //Create absolute url
        private string CreatAbsoluteUrlFromRelative(string baseUrl,string url)
        {
            if (Uri.IsWellFormedUriString(baseUrl, UriKind.Relative))
            {
                baseUrl=UrlValidation(baseUrl);
            }

            if (Uri.IsWellFormedUriString(url, UriKind.Relative))
            {
                UriBuilder myURI = new UriBuilder(baseUrl.Remove(baseUrl.Length-1) + url);
                url = myURI.Uri.AbsoluteUri;
            }

            return url;
        }

        private bool IsValidUrl(string url)
        {
            return Uri.IsWellFormedUriString(url, UriKind.RelativeOrAbsolute);
        }
    }
}
