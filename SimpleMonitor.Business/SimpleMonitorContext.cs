using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleMonitor.Business
{
  
    public class SimpleMonitorContext : DbContext
    {
        public SimpleMonitorContext()
            : base("SimpleMonitorEntities")
        {

            base.Configuration.LazyLoadingEnabled = true;
        }
    }
}
