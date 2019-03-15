using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO; //required for FileStream class
using System.Net.Mail; //required for MailMessage class

namespace DbUpClassLibrary.Common
{
    public class Common
    {
        private static readonly object _lock = new Object();

        private string FromEmailAddress;
        private string ToEmailAddress;
        private string SMTPHostName;
        private int SMTPPortNumber;

        public Common(string FromEmail, string ToEmail, string SMTPHost, int SMTPPort)
        {
            FromEmailAddress = FromEmail;
            ToEmailAddress = ToEmail;
            SMTPHostName = SMTPHost;
            SMTPPortNumber = SMTPPort;
        }

        public void WriteToLog(string Message, string LogFilePath)
        {
            lock (_lock)
            {
                try
                {
                    using (FileStream fs = new FileStream(LogFilePath, FileMode.Append, FileAccess.Write))
                    using (StreamWriter sw = new StreamWriter(fs))
                    {
                        sw.WriteLine(Message);
                    }
                }
                catch (Exception e)
                {
                    SendEmail(string.Format("Error writing to the log file: {0}, details {1} ", LogFilePath, e.ToString()));
                }
            }
        }

        public void PrepareLogDirectory(string Path, bool IsFilePath)
        {
            string destinationDirectory = string.Empty;
            if (IsFilePath)
                destinationDirectory = GetDirectoryNameFromFilePath(Path);
            else
                destinationDirectory = Path;

            if (!Directory.Exists(destinationDirectory))
                Directory.CreateDirectory(destinationDirectory);

            if (File.Exists(Path))
                File.Delete(Path);
           
        }

        public string GetDirectoryNameFromFilePath(string FilePath)
        {
            return Path.GetDirectoryName(FilePath);
        }

        public void ProcessEmail(bool HasDeploymentErrors, string LogFilePath, string DatabaseName, string TargetDBServer)
        {
            string subject = string.Empty;
            string body = string.Empty;
            string recipients = ToEmailAddress;

            if (HasDeploymentErrors)
            {
                subject = $"FAILED: Deployment for Database : {DatabaseName} on Server: {TargetDBServer}";
                body = $"Error deploying scripts to the database: {DatabaseName}" + Environment.NewLine;
                body += "Please review the attached log file";
            }
            else
            {
                subject = $"SUCCESS: Deployment for Database : {DatabaseName} on Server: {TargetDBServer}";
                body = $"Scripts deployed successfully to the database: {DatabaseName}" + Environment.NewLine;
                body += "Attached the script file showing the scripts deployed";
            }

            List<string> fileAttachments = new List<string>();
            fileAttachments.Add(LogFilePath);
            SendEmail(subject, body, recipients, fileAttachments);
        }

        public void SendEmail(string Subject, string Body, string Recipients, List<string> FileAttachmentPaths)
        {
                MailMessage mailMessage = new MailMessage(FromEmailAddress, Recipients, Subject, Body);

                if(FileAttachmentPaths != null)
                {
                    foreach(var filePath in FileAttachmentPaths)
                    {
                        if (File.Exists(filePath))
                            mailMessage.Attachments.Add(new Attachment(filePath));
                    }
                }

            if (Subject.ToUpper().Contains("ACTION REQUIRED"))
                mailMessage.Priority = MailPriority.High;

            SmtpClient emailClient = new SmtpClient(SMTPHostName, SMTPPortNumber);

            emailClient.Send(mailMessage);

            //always dispose the mailmessage object otherwise when invoking this method thorugh asp.net website from IIS/Parallel framework will lock the file
            mailMessage.Dispose();
            emailClient.Dispose();
            
        }

        public void SendEmail(string message)
        {
            string mailSubject = string.Empty;
            string mailBody = string.Empty;
            mailSubject = "**Errors** occurred during deployment automation";
            mailBody = message;
            string emailRecipients = ToEmailAddress;
            MailMessage mailMessage = new MailMessage(FromEmailAddress, emailRecipients, mailSubject, mailBody);
            SmtpClient emailClient = new SmtpClient(SMTPHostName, SMTPPortNumber);
            emailClient.Send(mailMessage);

            mailMessage.Dispose();
            emailClient.Dispose();
        }
    }
}
