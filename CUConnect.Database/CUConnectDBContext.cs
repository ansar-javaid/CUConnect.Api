using CUConnect.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CUConnect.Database
{
    public class CUConnectDBContext : IdentityDbContext<AppUser> 
    {
        public CUConnectDBContext(DbContextOptions<CUConnectDBContext> options) : base(options)
        {

        }

        public CUConnectDBContext()
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }

        public DbSet<AppUser> AppUser { get; set; }
    }
}
