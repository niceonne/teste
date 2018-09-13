using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalBitcoins
{
    public class GetWalletBalance
    {
        public class ReceivingAddressList
        {
            public string received { get; set; }
            public string address { get; set; }
        }

        public class Total
        {
            public string balance { get; set; }
            public string sendable { get; set; }
        }

        public class Data
        {
            public List<ReceivingAddressList> receiving_address_list { get; set; }
            public string message { get; set; }
            public Total total { get; set; }
            public int receiving_address_count { get; set; }
            public string receiving_address { get; set; }
        }

        public class RootWalletBalance
        {
            public Data data { get; set; }
        }
    }
}
