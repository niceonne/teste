using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalBitcoins
{
    public class GetMySelf
    {
        public class Data
        {
            public string username { get; set; }
            public int feedback_score { get; set; }
            public int feedback_count { get; set; }
            public int real_name_verifications_trusted { get; set; }
            public int trading_partners_count { get; set; }
            public string url { get; set; }
            public int real_name_verifications_untrusted { get; set; }
            public bool has_feedback { get; set; }
            public DateTime identity_verified_at { get; set; }
            public int trusted_count { get; set; }
            public int feedbacks_unconfirmed_count { get; set; }
            public int blocked_count { get; set; }
            public string trade_volume_text { get; set; }
            public bool has_common_trades { get; set; }
            public int real_name_verifications_rejected { get; set; }
            public string age_text { get; set; }
            public string confirmed_trade_count_text { get; set; }
            public DateTime created_at { get; set; }
        }

        public class RootMySelf
        {
            public Data data { get; set; }
        }
    }
}
