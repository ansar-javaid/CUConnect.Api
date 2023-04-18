using CUConnect.Database.Helpers;
using Microsoft.EntityFrameworkCore;

namespace CUConnect.Database.Entities
{

    public partial class CUConnectDBContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.EnableSensitiveDataLogging();
                var connectionString = SettingsHelper.GetValue("ConnectionStrings:DefaultConnection");
                optionsBuilder.UseSqlServer(connectionString, builder =>
                {
                    builder.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
                });
            }
        }
    }
}
