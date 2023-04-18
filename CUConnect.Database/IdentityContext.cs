using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CUConnect.Database
{
    public class IdentityContext : IdentityDbContext<AppUser>
    {
        public IdentityContext(DbContextOptions<IdentityContext> options) : base(options)
        {

        }

        public IdentityContext()
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }

        public DbSet<AppUser> AppUser { get; set; }
    }
}
