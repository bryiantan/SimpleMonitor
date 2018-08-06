using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SimpleMonitor.Business.Entity
{
    
    public class BaseEntity
    {
      

        public BaseEntity()
        {
        }

        public void InsertAuditValues()
        {
            UpdateAuditValues();
        }

        public void UpdateAuditValues()
        {
        }
    }
}
