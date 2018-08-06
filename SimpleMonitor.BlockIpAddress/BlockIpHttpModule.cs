using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;

namespace SimpleMonitor.BlockIpAddress
{
    public class BlockIpHttpModule : IHttpModule
    {
        private static string FILE_PATH = System.Configuration.ConfigurationManager.AppSettings["BlockedIpAddressTxtPath"];
        private static string CACHE_EXPIRATION = System.Configuration.ConfigurationManager.AppSettings["BlockedIpAddressCacheDuration"]; // Seconds
        private const string BLOCKEDIPSKEY = "BLOCKEDIPSKEY";

        // Define member variables
        private FileContents _fileContents;

        //public BlockIpHttpModule()
        //{
        //    _fileContents = ReadBannedIpListFile();
        //}

        public bool IsIpBlocked(string ip, HttpContext context)
        {
            IPAddress ipAddress;

            _fileContents = ReadBannedIpListFile(context);

            if (IPAddress.TryParse(ip, out ipAddress))
            {
                if (ipAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    // IPv4 address
                    string[] ipParts = ip.Split('.');

                    foreach (string banned in _fileContents.Ipv4Masks)
                    {
                        string[] blockedParts = banned.Split('.');
                        if (blockedParts.Length > 4) continue; // Not valid IP mask.

                        if (IsIpBlocked(ipParts, blockedParts))
                        {
                            return true;
                        }
                    }
                }
                else if (ipAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                {
                    // IPv6 address
                    string[] ipParts = ExpandIpv6Address(ipAddress).Split(':');

                    foreach (string banned in _fileContents.Ipv6Masks)
                    {
                        string bannedIP = banned.Split('/')[0]; // Take IP address part.
                        string[] blockedParts = bannedIP.Split(':');
                        if (blockedParts.Length > 8) continue; // Not valid IP mask.

                        if (IsIpBlocked(ipParts, blockedParts))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private bool IsIpBlocked(string[] ipParts, string[] blockedIpParts)
        {
            for (int i = 0; i < blockedIpParts.Length; i++)
            {
                // Compare if not wildcard
                if (blockedIpParts[i] != "*")
                {
                    // Compare IP address part
                    if (ipParts[i] != blockedIpParts[i].ToLower())
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private string ExpandIpv6Address(IPAddress ipAddress)
        {
            string expanded = "", separator = "";
            byte[] bytes = ipAddress.GetAddressBytes();

            for (int i = 0; i < bytes.Length; i += 2)
            {
                expanded += separator + bytes[i].ToString("x2");
                expanded += bytes[i + 1].ToString("x2");
                separator = ":";
            }

            return expanded;
        }

        private FileContents ReadBannedIpListFile(HttpContext context)
        {

            ObjectCache cache = MemoryCache.Default;
            FileContents fileContents = cache[BLOCKEDIPSKEY] as FileContents;

            if (fileContents == null)
            {
                FileContents tempFileContents = new FileContents();

                string cachedFilePath = context.Server.MapPath(FILE_PATH);
                if (File.Exists(cachedFilePath))
                {
                    List<string> filePaths = new List<string>();
                    filePaths.Add(cachedFilePath);

                   // context.Cache.Insert(BLOCKEDIPSKEY, tempFileContents, new CacheDependency(GetBlockedIPsFilePathFromCurrentContext(context)));

                    CacheItemPolicy policy = new CacheItemPolicy();
                    policy.AbsoluteExpiration = DateTimeOffset.Now.AddSeconds(Convert.ToDouble(CACHE_EXPIRATION));
                    policy.ChangeMonitors.Add(new HostFileChangeMonitor(filePaths));

                    List<string> tempIpv4List = new List<string>();
                    List<string> tempIpv6List = new List<string>();

                    // Read the file line by line.
                    using (StreamReader file = new StreamReader(cachedFilePath))
                    {
                        string line;
                        while ((line = file.ReadLine()) != null)
                        {
                            if (line.Contains("."))
                            {
                                tempIpv4List.Add(line);
                            }
                            else if (line.Contains(":"))
                            {
                                tempIpv6List.Add(line);
                            }
                        }
                    }

                    tempFileContents.Ipv4Masks = tempIpv4List.ToArray();
                    tempFileContents.Ipv6Masks = tempIpv6List.ToArray();

                    cache.Set(BLOCKEDIPSKEY, tempFileContents, policy);
                }

                fileContents = tempFileContents;
            }

            return fileContents;
        }

        public void Init(HttpApplication context)
        {
            context.BeginRequest += new EventHandler(Application_BeginRequest);
        }

        private void Application_BeginRequest(object source, EventArgs e)
        {
            HttpApplication app = source as HttpApplication;
            HttpContext context = ((HttpApplication)source).Context;
            //string ipAddress = context.Request.UserHostAddress;

            if (app != null)
            {
                string ipAddress = app.Context.Request.ServerVariables["REMOTE_ADDR"];
                if (ipAddress == null || ipAddress.Length == 0)
                {
                    return;
                }

                if (!context.Request.FilePath.Contains("error.html"))
                {
                    
                    if (DAL.IsIpBlocked(ipAddress))
                    {
                        DAL.UpdateBlockedHit(ipAddress, "h");
                        context.Response.Redirect("~/error.html");
                    }
                }

            }
        }

        public void Dispose() { /* clean up */ }
    }

    public class FileContents
    {
        public string[] Ipv4Masks = new string[0];
        public string[] Ipv6Masks = new string[0];
    }

}
