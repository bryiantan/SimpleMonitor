using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using WinSCP;

namespace SimpleMonitor.ScanFile
{
    class Program
    {
        static void Main(string[] args)
        {
            DownloadFiles();
            CompareFiles();
        }

        static void CompareFiles()
        {
            var tempFileName = $"{Guid.NewGuid()}.html";

            ProcessStartInfo startInfo = new ProcessStartInfo(ApplicationSetting.WinMergePath);
            startInfo.WindowStyle = ProcessWindowStyle.Minimized;

            startInfo.Arguments = $" {ApplicationSetting.BaselineFilesPath} {ApplicationSetting.LatestFilesPath} -minimize " +
                "-noninteractive -noprefs -cfg Settings/DirViewExpandSubdirs=1 -cfg Settings/DiffContextV2=1 " +
                "-cfg ReportFiles/ReportType=2 -cfg ReportFiles/IncludeFileCmpReport=1 -cfg Settings/ShowIdentical=0 " +
                $" -r -u -or {ApplicationSetting.FileCompareOutputPath}{tempFileName}";

            var process = Process.Start(startInfo);
            process.WaitForExit();

            Email.SendEmail($"{ ApplicationSetting.FileCompareOutputPath}{ tempFileName}");
        }

        static void DownloadFiles()
        {
            SessionOptions sessionOptions = new SessionOptions
            {
                Protocol = Protocol.Ftp,
                HostName = ApplicationSetting.FtpHost,
                UserName = ApplicationSetting.FtpUserName,
                Password = ApplicationSetting.FtpPassword//,
                //SshHostKeyFingerprint = "ssh-rsa 2048 xx:xx:xx:xx:xx:xx:xx:xx..."
            };

            string remotePath = ApplicationSetting.FtpRemoteFolder;
            string localPath = ApplicationSetting.LatestFilesPath;

            using (Session session = new Session())
            {
                // Connect
                session.Open(sessionOptions);

                // Enumerate files and directories to download
                IEnumerable<RemoteFileInfo> fileInfos =
                    session.EnumerateRemoteFiles(
                        remotePath, null,
                        EnumerationOptions.EnumerateDirectories |
                            EnumerationOptions.AllDirectories);

                foreach (RemoteFileInfo fileInfo in fileInfos)
                {
                    string localFilePath =
                        RemotePath.TranslateRemotePathToLocal(
                            fileInfo.FullName, remotePath, localPath);

                    if (fileInfo.IsDirectory)
                    {
                        // Create local subdirectory, if it does not exist yet
                        if (!Directory.Exists(localFilePath))
                        {
                            Directory.CreateDirectory(localFilePath);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Downloading file {0}...", fileInfo.FullName);
                        // Download file
                        string remoteFilePath = RemotePath.EscapeFileMask(fileInfo.FullName);
                        TransferOperationResult transferResult =
                            session.GetFiles(remoteFilePath, localFilePath);

                        // Did the download succeeded?
                        if (!transferResult.IsSuccess)
                        {
                            // Print error (but continue with other files)
                            Console.WriteLine(
                                "Error downloading file {0}: {1}",
                                fileInfo.FullName, transferResult.Failures[0].Message);
                        }
                    }
                }
            }

        }

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
