using System;
using Microsoft.Extensions.Configuration;

namespace CUConnect.Database.Helpers
{
    public class SettingsHelper
    {
        static IConfigurationRoot configuration = null;
        private static void Build()
        {
            // var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "local";
            configuration = new ConfigurationBuilder()
                //.AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.json", optional: true)
                .Build();
        }

        public static string GetValue(string key)
        {
            if (configuration == null) Build();
            return configuration[key];
        }
    }
}

