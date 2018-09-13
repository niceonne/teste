using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalBitcoins
{
    public class MessageListClass
    {
        public string Message { get; set; }

        public string Username { get; set; }

        public DateTime create_at { get; set; }

        public int FileID { get; set; }

        public string FileName { get; set; }

        public string FileType { get; set; }

        public string contactID { get; set; }

    }
}
