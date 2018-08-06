using SimpleMonitor.Business.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleMonitor.Business.UnitOfWork
{
    public class UnitOfWork : UnitOfWork_Base
    {
        private IISLogRepository _iisLogRepository;
        private BlockedIpRepository __blockedIpRepository;

        public IISLogRepository IISLogRepository
        {
            get
            {
                if (this._iisLogRepository == null)
                {
                    this._iisLogRepository = new IISLogRepository(context);
                }
                return _iisLogRepository;
            }
        }

        public BlockedIpRepository BlockedIpRepository
        {
            get
            {
                if (this.__blockedIpRepository == null)
                {
                    this.__blockedIpRepository = new BlockedIpRepository(context);
                }
                return __blockedIpRepository;
            }
        }


    }
}
