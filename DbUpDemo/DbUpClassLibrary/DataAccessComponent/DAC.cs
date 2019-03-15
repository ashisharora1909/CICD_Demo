using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//added references
using System.IO;
using System.Data.SqlClient;

using DbUp;
using DbUp.ScriptProviders; //Required for FileSystemScriptOptions
using DbUp.Engine; //Required for ScriptOptions

namespace DbUpClassLibrary.DataAccessComponent
{
    class DAC
    {
        public void CreateDatabaseIfNotExists(string DatabaseName, string masterDBConnectionString, string ProjectDirectory, ref StringBuilder sbLog)
        {
               try
                {
                    using (var con = new SqlConnection(masterDBConnectionString))
                    {
                        using (var cmd = new SqlCommand())
                        {
                            cmd.Connection = con;
                            con.Open();

                            cmd.CommandText = $"select count(name) from sys.databases where name ='{DatabaseName}'";

                            var databaseExists = Convert.ToBoolean(cmd.ExecuteScalar());

                            if (!databaseExists)
                                CreateDatabaseFromBaseline(masterDBConnectionString, ProjectDirectory, DatabaseName, ref sbLog);

                        }
                    }
                }
                catch (Exception e)
                {
                    throw new Exception($"Exception in CreateDatabaseIfNotExists(), details:{e.ToString()}");
                }
            
        }

        public void CreateDatabaseFromBaseline(string masterDBConnectionString, string ProjectDirectory, string DatabaseName, ref StringBuilder sbLog)
        {
            var upgrader = DeployChanges.To
                           .SqlDatabase(masterDBConnectionString)
                           .WithScriptsFromFileSystem
                           (
                                Path.Combine(ProjectDirectory, "Baseline")
                           )
                           .Build();

            var result = upgrader.PerformUpgrade();

            if (result.Scripts.Count() == 0)
            {
                throw new Exception($"Error creating database {DatabaseName} from Baseline. Ensure to clear the entry in master.dbo.SchemaVersions table");
            }

            if (result.Scripts.Count() > 0)
            {
                sbLog.AppendLine($"Database {DatabaseName} created from Baseline");
                sbLog.AppendLine($"Success - Script execution count from the folder Baseline: {result.Scripts.Count()}");
            }

            if (!result.Successful)
                throw new Exception($"Exception in CreateDatabaseFromBaseline(), details:{result.Error}");
        }

        public void CheckAndExecuteScripts(string DatabaseName, string targetDBConnectionString, string ProjectDirectory, string FolderName, ref StringBuilder sbLog, int DBQueryTimeOutInMinutes)
        {
            try
            {

                var upgrader = DeployChanges.To
                              .SqlDatabase(targetDBConnectionString)
                              .LogToConsole()
                              .WithScriptsFromFileSystem
                              (
                                   Path.Combine(ProjectDirectory, FolderName),
                                   new FileSystemScriptOptions() { IncludeSubDirectories = true},
                                   new DbUp.Engine.ScriptOptions()
                                   {
                                       RedeployOnChange = true,
                                       FirstDeploymentAsStartingPoint = false,
                                       IncludeSubDirectoryInName = true
                                   }
                              )
                              .WithTransaction()
                              .WithExecutionTimeout(TimeSpan.FromMinutes(DBQueryTimeOutInMinutes))
                              .Build();

                var result = upgrader.PerformUpgrade();

                sbLog.AppendLine($"Success - Scription Exectuion Count from Folder {FolderName}:{result.Scripts.Count()}");

                if (result.Scripts.Count() > 0)
                {
                    sbLog.AppendLine($"Following Scripts have been executed from the folder: {FolderName}");
                    sbLog.AppendLine("-----------------------------------------------------------------------");

                    foreach(var sqlScript in result.Scripts)
                    {
                        sbLog.AppendLine(sqlScript.Name);
                    }
                }

                if (!result.Successful)
                    throw new Exception($"Exception in CheckAndExecuteScripts(), details:{result.Error}");
            }
            catch (Exception e)
            {
                throw new Exception($"Exception in CheckAndExecuteScripts(), details:{e.ToString()}");
            }
        }

        public void CheckAndExecuteScriptsWithVariableSubstitution(string DatabaseName, string targetDBConnectionString, string ProjectDirectory, string FolderName, ref StringBuilder sbLog, int DBQueryTimeOutInMinutes, Dictionary<string, string> DictionarySQLJobParameters)
        {
            try
            {

                var upgrader = DeployChanges.To
                              .SqlDatabase(targetDBConnectionString)
                              .LogToConsole()
                              .WithScriptsFromFileSystem
                              (
                                   Path.Combine(ProjectDirectory, FolderName),
                                   new FileSystemScriptOptions() { IncludeSubDirectories = true },
                                   new DbUp.Engine.ScriptOptions()
                                   {
                                       RedeployOnChange = true,
                                       FirstDeploymentAsStartingPoint = false,
                                       IncludeSubDirectoryInName = true
                                   }
                              )
                              .WithVariables(DictionarySQLJobParameters)
                              .WithTransaction()
                              .WithExecutionTimeout(TimeSpan.FromMinutes(DBQueryTimeOutInMinutes))
                              .Build();

                var result = upgrader.PerformUpgrade();

                sbLog.AppendLine($"Success - Scription Exectuion Count from Folder {FolderName}:{result.Scripts.Count()}");

                if (result.Scripts.Count() > 0)
                {
                    sbLog.AppendLine($"Following Scripts have been executed from the folder: {FolderName}");
                    sbLog.AppendLine("-----------------------------------------------------------------------");

                    foreach (var sqlScript in result.Scripts)
                    {
                        sbLog.AppendLine(sqlScript.Name);
                    }
                }

                if (!result.Successful)
                    throw new Exception($"Exception in CheckAndExecuteScripts(), details:{result.Error}");
            }
            catch (Exception e)
            {
                throw new Exception($"Exception in CheckAndExecuteScripts(), details:{e.ToString()}");
            }
        }
    }
}
