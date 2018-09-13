using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalBitcoins
{
    public class DownloadFiles : IEquatable<DownloadFiles>
    {
      public int FileID { get; set; }

      public string FileName { get; set; }

      public string FileType { get; set; }
      
      public string contactID { get; set; }

      //public override string ToString()
      //{
      //      return "ID: " + FileID + " File Name: " + FileName + " File type: " + FileType + " Contact ID: " + contactID;
      //}

      public override bool Equals(object obj)
      {
          if (obj == null) return false;
            DownloadFiles objAsPart = obj as DownloadFiles;
          if (objAsPart == null) return false;
          else return Equals(objAsPart);
      }
      public override int GetHashCode()
      {
          return FileID;
      }
      public bool Equals(DownloadFiles other)
      {
          if (other == null) return false;
          return (this.FileID.Equals(other.FileID));
      }
        // Should also override == and != operators.
    }
}
