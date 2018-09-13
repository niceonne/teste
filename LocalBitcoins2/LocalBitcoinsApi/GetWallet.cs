using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalBitcoins
{
    public class GetWallet
    {
        public class ReceivedTransactions30d
        {
            public string amount { get; set; }
            public string description { get; set; }
            public int tx_type { get; set; }
            public DateTime created_at { get; set; }
            public object txid { get; set; }
        }

        public class OldAddressList
        {
            public string received { get; set; }
            public string address { get; set; }
        }

        public class SentTransactions30d
        {
            public string amount { get; set; }
            public string description { get; set; }
            public int tx_type { get; set; }
            public DateTime created_at { get; set; }
            public string txid { get; set; }
            public string to_address { get; set; }
        }

        public class Total
        {
            public string balance { get; set; }
            public string sendable { get; set; }
        }

        public class Data
        {
            public string receiving_address { get; set; }
            public List<ReceivedTransactions30d> received_transactions_30d { get; set; }
            public List<OldAddressList> old_address_list { get; set; }
            public List<SentTransactions30d> sent_transactions_30d { get; set; }
            public string message { get; set; }
            public Total total { get; set; }
        }

        public class walletData
        {
            public Data data { get; set; }
        }
    }
}
