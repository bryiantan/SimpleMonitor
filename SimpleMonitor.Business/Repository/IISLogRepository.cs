using SimpleMonitor.Business.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleMonitor.Business.Repository
{
    public class IISLogRepository : GenericRepository<vwIISLog>
    {

        public IISLogRepository(SimpleMonitorContext context)
            : base(context)
        {
        }

        public IQueryable<vwIISLog> DoSomething()
        {
            return GetMany().Where(w => w.scStatus == 404);
        }

        public bool LogFileExists(string fileName)
        {
            return GetMany().Where(w => w.LogFilename.Contains(fileName)).Any();
        }

        public IQueryable<vwIISLog> GetLogFile()
        {
            return GetMany();
        }

        /// <summary>
        /// by application name
        /// </summary>
        /// <param name="applicationName"></param>
        /// <returns></returns>
        public string LatestFileName(string applicationName)
        {
            var iisLog = GetLogFile();
            var result = iisLog
                .Where(w => w.ApplicationName == applicationName)
                .OrderByDescending(p => p.Id)
                                .Select(r => r.LogFilename)
                                .FirstOrDefault();

            return result;
        }
    }

}
