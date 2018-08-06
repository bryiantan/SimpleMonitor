using SimpleMonitor.Business.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleMonitor.Business.Repository
{
    public class BlockedIpRepository : GenericRepository<BlockedIp>
    {
        public BlockedIpRepository(SimpleMonitorContext context)
            : base(context)
        {
        }

        /// <summary>
        /// check if ip already in the table
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public bool IsIpAddressExists(string ip)
        {
            return GetMany().Where(w => w.IpAddress == ip).Any();
        }

        public void BlockIpAddress(string ip)
        {
            //insert
            if (!IsIpAddressExists(ip))
            {
                BlockedIp model = new BlockedIp();
                model.IpAddress = ip;
                model.LastUpdatedDate = DateTime.Now;
                model.IsBlocked = true;

                Insert(model);
            }
            else
            {
                //update
                BlockedIp model = GetMany().Where(w => w.IpAddress == ip).FirstOrDefault();
                model.IsBlocked = true;
                model.LastUpdatedDate = DateTime.Now;
                Update(model);
            }

            context.SaveChanges();
        }

        public void UnBlockIpAddress(string ip)
        {
            //unblock
            if (IsIpAddressExists(ip))
            {
                BlockedIp model = GetMany().Where(w => w.IpAddress == ip).FirstOrDefault();
                model.IsBlocked = false;
                model.LastUpdatedDate = DateTime.Now;
                Update(model);

                context.SaveChanges();
            }
        }
    }
}
