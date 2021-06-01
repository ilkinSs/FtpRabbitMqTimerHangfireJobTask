using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DateService.Utilities
{
    public class ConfigHelper
    {
        public static string GetConfigVal(string sectionName, string property)
        {
            var builder = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json");
            var configuration = builder.Build();


            var value = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection(sectionName)[property];

            return value;
        }
    }

}
