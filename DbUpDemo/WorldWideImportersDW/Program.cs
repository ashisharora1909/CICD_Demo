using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Configuration; //This is to read Configuration in appsettings
using System.IO; //This is to use Directory.GetParent function
using System.Reflection; //This is to use Assembly.GetEntryAssembly function
using DbUpClassLibrary.Common;
using DbUpClassLibrary.BusinessLogicComponent;



namespace DbUpDemo
{
    class Program
    {
        private static string logDate = DateTime.Parse(DateTime.Now.ToString()).ToString("yyyy-MM-dd");
        private static string targetDBName = AppConfigurations.TargetDBName;
        private static string targetServerName = AppConfigurations.TargetServerName;
        private static string logFilePath = AppConfigurations.LogFilePath.Replace("{TargetDBName}", targetDBName).Replace("{LogDate}", logDate);
        private static string targetDBConnectionString = AppConfigurations.TargetDBConnectionString.Replace("{TargetServerName}", targetServerName).Replace("{TargetDBName}", targetDBName);

        private static StringBuilder sbLog = new StringBuilder();
        private static bool hasDeploymentErrors = false;
        private static string projectDirectory = Directory.GetParent(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)).Parent.FullName;
        
        static void Main(string[] args)
        {
            int exitCode = 0;
            Common common = new Common(AppConfigurations.FromEmailAddress, AppConfigurations.ToEmailAddress, AppConfigurations.SMTPHostName, AppConfigurations.SMTPPortNumber);
            BLC blc = new BLC();

            try
            {
                common.PrepareLogDirectory(logFilePath, true);

                blc.LogUserNametoSBLog(ref sbLog);

                blc.CreateDatabaseIfNotExists(targetDBName, targetDBConnectionString, projectDirectory, ref sbLog);

                blc.CheckAndExecuteScripts(targetDBName, targetDBConnectionString, projectDirectory, "Migrations", ref sbLog, AppConfigurations.DBQueryTimeOutInMinutes);

                blc.CheckAndExecuteScripts(targetDBName, targetDBConnectionString, projectDirectory, "Programmability", ref sbLog, AppConfigurations.DBQueryTimeOutInMinutes);

                blc.CheckAndExecuteScripts(targetDBName, targetDBConnectionString, projectDirectory, "PostExecution", ref sbLog, AppConfigurations.DBQueryTimeOutInMinutes);


            }
            catch(Exception e)
            {
                hasDeploymentErrors = true;
                Console.WriteLine(e.ToString());
                sbLog.AppendLine(e.ToString());
                exitCode = -1;
            }

            common.WriteToLog(sbLog.ToString(), logFilePath);
            common.ProcessEmail(hasDeploymentErrors, logFilePath, targetDBName, targetServerName);

            //sleep for 10 seconds before closing the console so that you can inspect the log messages in the console
            System.Threading.Thread.Sleep(10000);

            Environment.Exit(exitCode);

        }
    }
}
