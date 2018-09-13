using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalBitcoins
{
    public class GetContactList
    {
        public class Advertiser
        {
            public string username { get; set; }
            public int feedback_score { get; set; }
            public string trade_count { get; set; }
            public DateTime? last_online { get; set; }
            public string name { get; set; }
        }

        public class Advertisement
        {
            public string payment_method { get; set; }
            public Advertiser advertiser { get; set; }
            public string trade_type { get; set; }
            public int id { get; set; }
        }

        public class Seller
        {
            public string username { get; set; }
            public int feedback_score { get; set; }
            public string trade_count { get; set; }
            public DateTime? last_online { get; set; }
            public string name { get; set; }
        }

        public class Buyer
        {
            public string username { get; set; }
            public int feedback_score { get; set; }
            public string countrycode_by_ip { get; set; }
            public string name { get; set; }
            public DateTime? last_online { get; set; }
            public string countrycode_by_phone_number { get; set; }
            public string trade_count { get; set; }
            public string real_name { get; set; }
            public object company_name { get; set; }
        }

        public class data
        {
            public DateTime? exchange_rate_updated_at { get; set; }
            public Advertisement advertisement { get; set; }
            public bool is_buying { get; set; }
            public DateTime? payment_completed_at { get; set; }
            public DateTime? released_at { get; set; }
            public DateTime created_at { get; set; }
            public string reference_code { get; set; }
            public int contact_id { get; set; }
            public Seller seller { get; set; }
            public string currency { get; set; }
            public string amount { get; set; }
            public bool is_selling { get; set; }
            public DateTime escrowed_at { get; set; }
            public string amount_btc { get; set; }
            public object canceled_at { get; set; }
            public Buyer buyer { get; set; }
            public DateTime? closed_at { get; set; }
            public object disputed_at { get; set; }
            public DateTime? funded_at { get; set; }
            public string account_info { get; set; }
            public string fee_btc { get; set; }
        }

        public class Actions
        {
            public string message_post_url { get; set; }
            public string advertisement_url { get; set; }
            public string messages_url { get; set; }
            public string release_url { get; set; }
            public string advertisement_public_view { get; set; }
        }

        public class ContactList
        {
            public data data { get; set; }
            public Actions actions { get; set; }
        }

        public class Data
        {
            public List<ContactList> contact_list { get; set; }
            public int contact_count { get; set; }
        }

        public class RootContact
        {
            public Data data { get; set; }
        }
    }
}
