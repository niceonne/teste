using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalBitcoins
{
    public class GetContactInfo
    {
        public class Advertiser
        {
            public string username { get; set; }
            public int feedback_score { get; set; }
            public string trade_count { get; set; }
            public DateTime last_online { get; set; }
            public string name { get; set; }
        }

        public class AccountDetails
        {
            public string receiver_email { get; set; }
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
            public DateTime last_online { get; set; }
            public string name { get; set; }
        }

        public class Buyer
        {
            public string username { get; set; }
            public int feedback_score { get; set; }
            public string countrycode_by_ip { get; set; }
            public string name { get; set; }
            public DateTime last_online { get; set; }
            public string countrycode_by_phone_number { get; set; }
            public string trade_count { get; set; }
            public string real_name { get; set; }
            public object company_name { get; set; }
        }

        public class Data
        {
            public DateTime exchange_rate_updated_at { get; set; }
            public Advertisement advertisement { get; set; }
            public bool is_buying { get; set; }
            public DateTime? payment_completed_at { get; set; }
            public DateTime? released_at { get; set; }
            public DateTime created_at { get; set; }
            public string reference_code { get; set; }
            public AccountDetails account_details { get; set; }
            public int contact_id { get; set; }
            public Seller seller { get; set; }
            public string currency { get; set; }
            public string amount { get; set; }
            public bool is_selling { get; set; }
            public DateTime escrowed_at { get; set; }
            public string amount_btc { get; set; }
            public DateTime? canceled_at { get; set; }
            public Buyer buyer { get; set; }
            public DateTime? closed_at { get; set; }
            public DateTime? disputed_at { get; set; }
            public DateTime funded_at { get; set; }
            public string account_info { get; set; }
            public string fee_btc { get; set; }
            //public List<Data> contact_list { get; set; }
        }

        public class Actions
        {
            public string message_post_url { get; set; }
            public string advertisement_url { get; set; }
            public string messages_url { get; set; }
            public string release_url { get; set; }
            public string advertisement_public_view { get; set; }
        }

        public class RootObject
        {
            public Data data { get; set; }
            public Actions actions { get; set; }
        }

        public class DataList
        {
            public List<RootObject> contact_list { get; set; }
            public int contact_count { get; set; }
        }

        public class GetAllContacts
        {
            public DataList data { get; set; }
        }
    }
}
