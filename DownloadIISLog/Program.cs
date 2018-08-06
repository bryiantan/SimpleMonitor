using SimpleMonitor.Business;
using SimpleMonitor.Business.Entity;
using SimpleMonitor.Business.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using WinSCP;

namespace DownloadIISLog
{
    class Program : Base
    {
        private static UnitOfWork unitOfWork = new UnitOfWork();

        static void Main(string[] args)
        {
            try
            {
                CleanUpDownloadFolder();
                WinScpGetLog();
                CallLogParser();
            }
            catch (Exception ex)
            {
                LogEvent("Error: " + ex?.Message);
            }
        }

        //using logparser to upload to database
        static void CallLogParser()
        {
            LogEvent("Start CallLogParser");

            ProcessStartInfo startInfo = new ProcessStartInfo(ApplicationSetting.LogParserPath);
            startInfo.WindowStyle = ProcessWindowStyle.Minimized;

            Process.Start(startInfo);

            startInfo.Arguments = string.Format("\"SELECT '{7}', *,1 INTO {0} FROM {1}{2}*.log\" -o:SQL -createTable:OFF -server:{3} -database:{4} -username:{5} -password:{6}",
                ApplicationSetting.LogParserSqlTable, ApplicationSetting.DownloadPath, ApplicationSetting.LogFilePrefix, ApplicationSetting.LogParserSqlserver,
                ApplicationSetting.LogParserSqlDatabase, ApplicationSetting.LogParserSqlUserName, ApplicationSetting.LogParserSqlPassword,
                ApplicationSetting.ApplicationName);

            Process.Start(startInfo);

            LogEvent("End CallLogParser");
        }

        static internal SessionOptions GetWinScpSession()
        {
            SessionOptions sessionOptions = new SessionOptions
            {
                Protocol = Protocol.Ftp,
                HostName = ApplicationSetting.FtpHost,
                UserName = ApplicationSetting.FtpUserName,
                Password = ApplicationSetting.FtpPassword//,
                //SshHostKeyFingerprint = "ssh-rsa 2048 xx:xx:xx:xx:xx:xx:xx:xx..."
            };

            return sessionOptions;
        }

        static internal void WinScpGetLog()
        {
            LogEvent("Start Download");
            using (Session session = new Session())
            {
                // Connect
                session.Open(GetWinScpSession());

                string remotePath = ApplicationSetting.FtpRemoteFolder;
                string localPath = ApplicationSetting.DownloadPath;

                // Get list of files in the directory
                RemoteDirectoryInfo directoryInfo = session.ListDirectory(remotePath);

                //latest file name in the table by application name
                var latestLogFileInDb = Path.GetFileName(unitOfWork.IISLogRepository.LatestFileName(ApplicationSetting.ApplicationName));

                //get the date of the latest file from FTP
                var logFileDate = directoryInfo.Files
                    .Where(w => w.Name.ToLower() == latestLogFileInDb?.ToLower())
                    .Select(s => s.LastWriteTime).FirstOrDefault();

                // Select the files not in database table
                IEnumerable<RemoteFileInfo> notInLogTable =
                    directoryInfo.Files
                        .Where(file => !file.IsDirectory && file.LastWriteTime > logFileDate).ToList();

                //// Download the selected file
                foreach (RemoteFileInfo fileInfo in notInLogTable)
                {
                    string localFilePath =
                        RemotePath.TranslateRemotePathToLocal(
                            fileInfo.FullName, remotePath, localPath);

                    string remoteFilePath = RemotePath.EscapeFileMask(fileInfo.FullName);

                    //download
                    TransferOperationResult transferResult =
                        session.GetFiles(remoteFilePath, localFilePath);
                }
                LogEvent("End Download");
            }
        }

        /// <summary>
        /// delete all the files in the download folder
        /// </summary>
        static void CleanUpDownloadFolder()
        {
            LogEvent("Start CleanUpDownloadFolder");

            //truncate the folder first before download
            DirectoryInfo di = new DirectoryInfo(ApplicationSetting.DownloadPath);

            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }

            LogEvent("End CleanUpDownloadFolder");
        }

        //static IList<string> GetAllFilesName()
        //{
        //    IList<string> logFilesName = new List<string>();

        //    using (FtpWebResponse response = (FtpWebResponse)FtpRequest(ApplicationSetting.WebApp1Path).GetResponse())
        //    {
        //        using (StreamReader streamReader = new StreamReader(response.GetResponseStream()))
        //        {
        //            string line = streamReader.ReadLine();
        //            while (!string.IsNullOrEmpty(line))
        //            {
        //                if (!unitOfWork.IISLogRepository.LogFileExists(line))
        //                {
        //                    logFilesName.Add(line);
        //                }
        //                line = streamReader.ReadLine();
        //            }
        //        }
        //    }
        //    return logFilesName;
        //}

        ///// <summary>
        ///// get and return all the log files name in the folder
        ///// </summary>
        ///// <returns></returns>
        //static IList<string> GetLogFileName()
        //{
        //    IList<string> logFilesName = new List<string>();

        //    using (FtpWebResponse response = (FtpWebResponse)FtpRequest(ApplicationSetting.FtpUri).GetResponse())
        //    {
        //        using (StreamReader streamReader = new StreamReader(response.GetResponseStream()))
        //        {
        //            string line = streamReader.ReadLine();
        //            while (!string.IsNullOrEmpty(line))
        //            {
        //                if (!unitOfWork.IISLogRepository.LogFileExists(line))
        //                {
        //                    logFilesName.Add(line);
        //                }
        //                line = streamReader.ReadLine();
        //            }
        //        }
        //    }
        //    return logFilesName;
        //}

        //static void DownloadLogFile(string logFileName)
        //{
        //    int bytesRead = 0;
        //    byte[] buffer = new byte[2048];

        //    try
        //    {
        //        using (Stream streamReader = FtpRequest(ApplicationSetting.FtpUri + logFileName, true).GetResponse().GetResponseStream())
        //        {
        //            using (FileStream fileStream = new FileStream(ApplicationSetting.DownloadPath + logFileName, FileMode.Create))
        //            {
        //                while (true)
        //                {
        //                    bytesRead = streamReader.Read(buffer, 0, buffer.Length);

        //                    if (bytesRead == 0)
        //                        break;

        //                    fileStream.Write(buffer, 0, bytesRead);
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        LogEvent("DownloadLogFile: " + ex.InnerException.Message);
        //    }
        //}

        //by pass the certificate dialog on local
        static bool ValidateCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            if (!string.IsNullOrEmpty(ApplicationSetting.FtpCertificateHash))
            {
                if (certificate == null)
                {
                    return false;
                }
                if (certificate.GetCertHashString() == ApplicationSetting.FtpCertificateHash)
                {
                    return true;
                }
                return false;
            }
            else
            {
                return true;
            }
        }

    }
}
