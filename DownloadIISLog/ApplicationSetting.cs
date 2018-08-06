using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DownloadIISLog
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

        public static string FtpHost
        {
            get
            {
                return Get("FtpHost");
            }
        }

        public static string FtpRemoteFolder
        {
            get
            {
                return Get("FtpRemoteFolder");
            }
        }

        public static string FtpUri
        {
            get
            {
                return Get("FtpUriPath");
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

        public static string DownloadPath
        {
            get
            {
                return Get("DownloadPath");
            }
        }

        public static string LogFilePrefix
        {
            get
            {
                return Get("FilePrefix");
            }
        }


        //public static bool FtpGetAllFiles
        //{
        //    get
        //    {
        //        return Get("FtpGetAllFiles") == "true" ? true : false;
        //    }
        //}
        public static string LogPath
        {
            get
            {
                return Get("LogPath");
            }
        }
        public static string LogParserPath
        {
            get
            {
                return Get("LogParserPath");
            }
        }

        public static string LogParserSqlTable
        {
            get
            {
                return Get("LogParserSqlTable");
            }
        }
        public static string LogParserSqlserver
        {
            get
            {
                return Get("LogParserSqlserver");
            }
        }
        public static string LogParserSqlDatabase
        {
            get
            {
                return Get("LogParserSqlDatabase");
            }
        }
        public static string LogParserSqlUserName
        {
            get
            {
                return Get("LogParserSqlUserName");
            }
        }
        public static string LogParserSqlPassword
        {
            get
            {
                return Get("LogParserSqlPassword");
            }
        }

        public static string ApplicationName
        {
            get
            {
                return Get("ApplicationName");
            }
        }

    }
}
