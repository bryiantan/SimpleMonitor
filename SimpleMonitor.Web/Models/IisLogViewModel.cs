using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimpleMonitor.Web.Models
{
    public class IisLogViewModel
    {
        public int Id { get; set; }
        public string date { get; set; }
        public string cIp { get; set; }
        public int sPort { get; set; }
        public string csMethod { get; set; }
        public string csUriStem { get; set; }
        public string csUriQuery { get; set; }
        public string scStatus { get; set; }
        public string csUserAgent { get; set; }
        public string csCookie { get; set; }
        public string csReferer { get; set; }
        public string IsBlocked { get; set; }
        public int BlockHit { get; set; }
        public string ApplicationName { get; set; }
    }

    public class HttpStatusViewModel
    {
        public string ScStatus { get; set; }
    }

    public class ApplicationNameViewModel
    {
        public string Name { get; set; }
    }

    public class IpViewModel
    {
        public string Ip { get; set; }
        public string Action { get; set; }
    }
}