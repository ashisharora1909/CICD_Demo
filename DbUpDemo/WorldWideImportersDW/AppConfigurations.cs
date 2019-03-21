using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Configuration;

namespace WorldWideImportersDW
{
    class AppConfigurations
    {
        public static string TargetDBName
        {
            get
            {
                return ConfigurationManager.AppSettings["TargetDBName"];
            }
        }

        public static string TargetServerName
        {
            get
            {
                return ConfigurationManager.AppSettings["TargetServerName"];
            }
        }

        public static string LogFilePath
        {
            get
            {
                return ConfigurationManager.AppSettings["LogFilePath"];
            }
        }

        public static string FromEmailAddress
        {
            get
            {
                return ConfigurationManager.AppSettings["FromEmailAddress"];
            }
        }

        public static string ToEmailAddress
        {
            get
            {
                return ConfigurationManager.AppSettings["ToEmailAddress"];
            }
        }

        public static string SMTPHostName
        {
            get
            {
                return ConfigurationManager.AppSettings["SMTPHostName"];
            }
        }

        public static int SMTPPortNumber
        {
            get
            {
                return int.Parse(ConfigurationManager.AppSettings["SMTPPortNumber"]);
            }
        }

        public static string TargetDBConnectionString
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["TargetDBConnectionString"].ToString();
            }
        }

        public static int DBQueryTimeOutInMinutes
        {
            get
            {
                return int.Parse(ConfigurationManager.AppSettings["DBQueryTimeOutInMinutes"]);
            }
        }
    }
}
