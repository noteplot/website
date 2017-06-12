using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace NPTest1.Models
{
    // EF
    public class NPContext: DbContext
    {
        public DbSet<Test_Parameter> Test_Parameters { get; set; }
        public NPContext(DbContextOptions<NPContext> options)
            : base(options)
        {
        }

        //publicNPContext() :base("TEST")
        //{ }
    }
}
