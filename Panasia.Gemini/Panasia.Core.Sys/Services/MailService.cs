using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Panasia.Core.Sys
{
    public class MailService
    {
        public static MailServer GetItem(string idOrName)
        {
            using (var db = SysContext.GetCurrent())
            {
                return db.MailServers.FirstOrDefault(m => m.ServerID == idOrName || m.ServerName == idOrName);
            }
        }

        public static IEnumerable<MailServer> GetIndex()
        {
            using (var db = SysContext.GetCurrent())
            {
                return db.MailServers.ToList();
            }
        }
    }
}
