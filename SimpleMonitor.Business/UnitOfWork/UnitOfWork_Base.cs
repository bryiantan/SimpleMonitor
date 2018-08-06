using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleMonitor.Business.UnitOfWork
{
    public class UnitOfWork_Base : IDisposable
    {
        protected SimpleMonitorContext context = new SimpleMonitorContext();

        public void Save()
        {
            try
            {
                context.SaveChanges();
            }
            catch(Exception e ){
                throw e;
            }
            
        }

        private bool disposedValue = false;

        // IDisposable
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposedValue = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


    }
}
