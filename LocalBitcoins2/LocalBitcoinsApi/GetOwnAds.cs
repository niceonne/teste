using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalBitcoins
{
    public class GetOwnAds
    {
        public class Profile
        {
            public string username { get; set; }
            public int feedback_score { get; set; }
            public string trade_count { get; set; }
            public DateTime last_online { get; set; }
            public string name { get; set; }
        }

        public class Data2
        {
            public Profile profile { get; set; }
            public int require_feedback_score { get; set; }
            public bool hidden_by_opening_hours { get; set; }
            public string trade_type { get; set; }
            public int ad_id { get; set; }
            public string temp_price { get; set; }
            public string bank_name { get; set; }
            public int payment_window_minutes { get; set; }
            public bool trusted_required { get; set; }
            public string min_amount { get; set; }
            public string account_info { get; set; }
            public bool visible { get; set; }
            public string reference_type { get; set; }
            public bool require_trusted_by_advertiser { get; set; }
            public bool track_max_amount { get; set; }
            public string temp_price_usd { get; set; }
            public double lat { get; set; }
            public bool is_local_office { get; set; }
            public string price_equation { get; set; }
            public string first_time_limit_btc { get; set; }
            public object atm_model { get; set; }
            public string city { get; set; }
            public string location_string { get; set; }
            public string countrycode { get; set; }
            public string currency { get; set; }
            public string limit_to_fiat_amounts { get; set; }
            public DateTime created_at { get; set; }
            public string max_amount { get; set; }
            public double lon { get; set; }
            public bool display_reference { get; set; }
            public bool sms_verification_required { get; set; }
            public double require_trade_volume { get; set; }
            public string online_provider { get; set; }
            public string max_amount_available { get; set; }
            public string opening_hours { get; set; }
            public string msg { get; set; }
            public bool require_identification { get; set; }
            public string volume_coefficient_btc { get; set; }
            public bool? is_low_risk { get; set; }
        }

        public class Actions
        {
            public string html_form { get; set; }
            public string public_view { get; set; }
            public string change_form { get; set; }
        }

        public class AdList
        {
            public Data2 data { get; set; }
            public Actions actions { get; set; }
        }

        public class Data
        {
            public List<AdList> ad_list { get; set; }
            public int ad_count { get; set; }
        }

        public class AdsData
        {
            public Data data { get; set; }
        }
    }
}
