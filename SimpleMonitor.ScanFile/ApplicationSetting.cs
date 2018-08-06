using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleMonitor.ScanFile
{
    public static class ApplicationSetting
    {
        public static string Get(string key)
        {
            try
            {
                return ConfigurationManager.AppSettings[key];
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        public static string WinMergePath
        {
            get
            {
                return Get("WinMergePath");
            }
        }

        public static string FileCompareOutputPath
        {
            get
            {
                return Get("FileCompareOutputPath");
            }
        }

        public static string BaselineFilesPath
        {
            get
            {
                return Get("BaselineFilesPath");
            }
        }

        public static string LatestFilesPath
        {
            get
            {
                return Get("LatestFilesPath");
            }
        }

        public static string FtpHost
        {
            get
            {
                return Get("FtpHost");
            }
        }

        public static string FtpUserName
        {
            get
            {
                return Get("FtpUserName");
            }
        }

        public static string FtpPassword
        {
            get
            {
                return Get("FtpPassword");
            }
        }

        public static bool FtpEnableSsl
        {
            get
            {
                return Get("FtpEnableSsl") == "true" ? true : false;
            }
        }

        public static string FtpCertificateHash
        {
            get
            {
                return Get("FtpCertificateHash");
            }
        }

        public static string FtpRemoteFolder
        {
            get
            {
                return Get("FtpRemoteFolder");
            }
        }

        //email
        public static string SmtpHost
        {
            get
            {
                return Get("SmtpHost");
            }
        }
        public static string SmtpTo
        {
            get
            {
                return Get("SmtpTo");
            }
        }
        public static string SmtpPort
        {
            get
            {
                return Get("SmtpPort");
            }
        }
        public static string SmtpUserName
        {
            get
            {
                return Get("SmtpUserName");
            }
        }
        public static string SmtpPassword
        {
            get
            {
                return Get("SmtpPassword");
            }
        }

        public static string SmtpSubject
        {
            get
            {
                return Get("SmtpSubject");
            }
        }

    }
}
