using System;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Documents;

namespace TesteLocalbitcoinsApp
{
    public class TableItemscs : INotifyPropertyChanged
    {
        private string _Contactnumber;
        private string _Createdat;
        private string _Nickname;
        private string _AmountBTC;
        private string _amountUSD;
        private string _paymentmethod;
        private string _status;

        public string ContactNumber
        {
            get { return _Contactnumber; }
            set
            {
                _Contactnumber = value;
                NotifyPropertyChanged("Contactnumber");
            }
        }

        public string CreatedAt
        {
            get { return _Createdat; }
            set
            {
                _Createdat = value;
                NotifyPropertyChanged("Createdat");
            }
        }

        public string Nickname
        {
            get { return _Nickname; }
            set
            {
                _Nickname = value;
                NotifyPropertyChanged("Nickname");
            }
        }

        public string AmountBtc
        {
            get { return _AmountBTC; }
            set
            {
                _AmountBTC = value;
                NotifyPropertyChanged("Amountbtc");
            }
        }

        public string Amount
        {
            get { return _amountUSD; }
            set
            {
                _amountUSD = value;
                NotifyPropertyChanged("Amount");
            }
        }

        public string PaymentMethod
        {
            get { return _paymentmethod; }
            set
            {
                _paymentmethod = value;
                NotifyPropertyChanged("Paymentmethod");
            }
        }

        public string Status
        {
            get { return _status; }
            set
            {
                _status = value;
                NotifyPropertyChanged("Status");
            }
        }

        //public Button Button
        //{
        //    get { return _Button; }
        //    set
        //    {
        //        _Button = value;
        //        NotifyPropertyChanged("Button");
        //    }
        //}

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
