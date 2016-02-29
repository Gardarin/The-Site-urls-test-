using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TzWebSite.Models
{
    public class SiteTest
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public virtual ICollection<TestResult> TestResults { get; set; }
    }
}