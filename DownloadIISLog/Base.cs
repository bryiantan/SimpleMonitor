using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DownloadIISLog
{
    public class Base
    {
        public static FtpWebRequest FtpRequest(string uri, bool isDowload = false )
        {
            FtpWebRequest ftpRequest = (FtpWebRequest)WebRequest.Create(uri);

            ftpRequest.Credentials = new NetworkCredential(ApplicationSetting.FtpUserName, ApplicationSetting.FtpPassword);

            if (isDowload)
            {
                ftpRequest.Method = WebRequestMethods.Ftp.DownloadFile;
            }
            else
            {
                ftpRequest.Method = WebRequestMethods.Ftp.ListDirectory;
            }

            ftpRequest.EnableSsl = ApplicationSetting.FtpEnableSsl;

            return ftpRequest;
        }

        public static void LogEvent(string step)
        {
            // This text is added only once to the file.
            if (!File.Exists(ApplicationSetting.LogPath))
            {
                // Create a file to write to.
                using (StreamWriter sw = File.CreateText(ApplicationSetting.LogPath))
                {
                    sw.WriteLine(string.Format("{0} - {1}",DateTime.Now, step));
                }
            }

            // This text is always added, making the file longer over time
            // if it is not deleted.
            using (StreamWriter sw = File.AppendText(ApplicationSetting.LogPath))
            {
                sw.WriteLine(string.Format("{0} - {1}", DateTime.Now, step));
            }
        }

        public static string FileName
        {
           get
            {
                //format u_ex150608.log
                //download log file from yesterday. today log file is open/in use by other process
                return string.Format("{0}{1}.log", ApplicationSetting.LogFilePrefix, DateTime.Now.AddDays(-1).ToString("yyMMdd"));
            }
        }
    }
}
