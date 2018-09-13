using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace LocalBitcoins
{
    public class NotificationInfo
    {
        public string url { get; set; }
        public DateTime created_at { get; set; }
        public string contact_id { get; set; }
        public bool read { get; set; }
        public string msg { get; set; }
        public string id { get; set; }
    }

    public class RootNotification
    {
        public List<NotificationInfo> data { get; set; }
    }
}
