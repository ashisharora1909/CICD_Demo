using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//Added references
using System.IO; //required for Directory.Exists
using DbUpClassLibrary.DataAccessComponent;

namespace DbUpClassLibrary.BusinessLogicComponent
{
    public class BLC
    {
        private DAC dac = new DAC();

        public void CreateDatabaseIfNotExists(string DatabaseName, string masterDBConnectionString, string ProjectDirectory, ref StringBuilder sbLog)
        {
            dac.CreateDatabaseIfNotExists(DatabaseName, masterDBConnectionString, ProjectDirectory, ref sbLog);
        }

        public void LogUserNametoSBLog(ref StringBuilder sbLog)
        {
            sbLog.AppendLine($"User executing the deployment: {System.Security.Principal.WindowsIdentity.GetCurrent().Name}");
        }

        public void CheckAndExecuteScripts(string DatabaseName, string targetDBConnectionString, string ProjectDirectory, string FolderName, ref StringBuilder sbLog, int DBQueryTimeOutInMinutes)
        {
            if (Directory.Exists(Path.Combine(ProjectDirectory, FolderName)))
                dac.CheckAndExecuteScripts(DatabaseName, targetDBConnectionString, ProjectDirectory, FolderName, ref sbLog, DBQueryTimeOutInMinutes);
        }

        public void CheckAndExecuteScriptsWithVariableSubstitution(string DatabaseName, string targetDBConnectionString, string ProjectDirectory, string FolderName, ref StringBuilder sbLog, int DBQueryTimeOutInMinutes, Dictionary<string, string> DictionarySQLJobParameters)
        {
            if (Directory.Exists(Path.Combine(ProjectDirectory, FolderName)))
                dac.CheckAndExecuteScriptsWithVariableSubstitution(DatabaseName, targetDBConnectionString, ProjectDirectory, FolderName, ref sbLog, DBQueryTimeOutInMinutes, DictionarySQLJobParameters);
        }
    }
}
