using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace BMS.Entities
{
    public static  class ConfigReader
    {
        public static string GetConnectionString() => ConfigurationManager.ConnectionStrings["BMSConnectionString"].ConnectionString;

        public static string GetSettings(string Key) => ConfigurationManager.AppSettings[Key];

    }
}
