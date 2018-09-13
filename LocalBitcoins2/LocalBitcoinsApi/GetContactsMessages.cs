using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalBitcoins
{
    public class Sender
    {
        public string username { get; set; }
        public int feedback_score { get; set; }
        public string trade_count { get; set; }
        public bool userread { get; set; }
        public DateTime? last_online { get; set; }
        public string name { get; set; }
    }

    public class MessageList
    {
        public string msg { get; set; }
        public DateTime created_at { get; set; }
        public bool read { get; set; }
        public bool is_admin { get; set; }
        public Sender sender { get; set; }
        public string attachment_url { get; set; }
        public string attachment_name { get; set; }
        public string attachment_type { get; set; }
    }

    public class DataContactMessage
    {
        public int message_count { get; set; }
        public List<MessageList> message_list { get; set; }
    }

    public class RootGetContactMessages
    {
        public DataContactMessage data { get; set; }
    }
}
