using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TesteLocalbitcoinsApp
{
    public class TableAds : INotifyPropertyChanged
    {
        private string _adId;
        private string _adVisible;
        private string _adInfo;
        private string _adprice;
        private string _adEquation;
        private string _Createdat;

        public string Adid
        {
            get { return _adId; }
            set
            {
                _adId = value;
                NotifyPropertyChanged("Adid");
            }
        }

        public string AdStatus
        {
            get { return _adVisible; }
            set
            {
                _adVisible = value;
                NotifyPropertyChanged("AdStatus");
            }
        }


        public string AdInfo
        {
            get { return _adInfo; }
            set
            {
                _adInfo = value;
                NotifyPropertyChanged("AdInfo");
            }
        }

        public string AdPrice
        {
            get { return _adprice; }
            set
            {
                _adprice = value;
                NotifyPropertyChanged("AdPrice");
            }
        }

        public string AdEquation
        {
            get { return _adEquation; }
            set
            {
                _adEquation = value;
                NotifyPropertyChanged("AdEquation");
            }
        }

        public string adCreatedat
        {
            get { return _Createdat; }
            set
            {
                _Createdat = value;
                NotifyPropertyChanged("adCreatedat");
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Private Helpers

        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}
