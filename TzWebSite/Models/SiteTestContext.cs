using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions; 

namespace TzWebSite.Models
{
    public class SiteTestsContext:DbContext
    {
        public DbSet<SiteTest> SiteTests { get; set; }
        public DbSet<TestResult> TimeResult { get; set; }
       

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        } 
    }
}