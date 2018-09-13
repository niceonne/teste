using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalBitcoins
{
    public class NotificationsMessages
    {
        //public string NotificationID { get; set; }

        public string Message { get; set; }

        public string contactID { get; set; }

        public DateTime created_at { get; set; }

        //public override string ToString()
        //{
        //    return "NotificationID: " + NotificationID + " Message: " + Message + " Contact ID: " + contactID;
        //}
        //public override bool Equals(object obj)
        //{
        //    if (obj == null) return false;
        //    DownloadFiles objAsPart = obj as DownloadFiles;
        //    if (objAsPart == null) return false;
        //    else return Equals(objAsPart);
        //}
        //public bool Equals(DownloadFiles other)
        //{
        //    if (other == null) return false;
        //    return (this.NotificationID.Equals(other.FileID));
        //}
        //// Should also override == and != operators.
    }
}
