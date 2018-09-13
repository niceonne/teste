using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TesteLocalbitcoinsApp
{
    /// <summary>
    /// Interaction logic for HomePageusercontrol.xaml
    /// </summary>
    public partial class HomePageusercontrol : UserControl
    {
        public HomePageusercontrol(MainWindow parentMain)
        {
            InitializeComponent();
            DataContext = this;
            this.parentMinwindow = parentMain;
        }

        private MainWindow parentMinwindow;
        public ICollectionView Trades { get; private set; }

        private void Fillingdatagrid()
        {
            //DataTable dt = new DataTable();
            //DataColumn contactid = new DataColumn();
            //DataColumn tradecreatedat = new DataColumn();
            //DataColumn username = new DataColumn();
            //DataColumn amountBTC = new DataColumn();
            //DataColumn amount = new DataColumn();
            //DataColumn paymentmethod = new DataColumn();
            //DataColumn tradestatus = new DataColumn();

            //dt.Columns.Add(contactid);
            //dt.Columns.Add(tradecreatedat);
            //dt.Columns.Add(username);
            //dt.Columns.Add(amountBTC);
            //dt.Columns.Add(amount);
            //dt.Columns.Add(paymentmethod);
            //dt.Columns.Add(tradestatus);

            //DataRow firstrow = dt.NewRow();
            //firstrow[0] = "#123123131";
            //firstrow[1] = "20/10/20";
            //firstrow[2] = "niceonne";
            //firstrow[3] = "0.004";
            //firstrow[4] = "20 USD";
            //firstrow[5] = "Paypal";
            //firstrow[6] = " Waiting for payment";

            //dt.Rows.Add(firstrow);
            //myDataGrid.Dispatcher.BeginInvoke((Action)(() =>
            //{
            //    myDataGrid.ItemsSource = dt.DefaultView;
            //}));
            //var costumer = new TableItemscs
            //{
            //    Nickname = "niceonne",
            //    Amount = "20",
            //    AmountBtc = "0.01",
            //    CreatedAt = DateTime.Now,
            //    ContactNumber = "192813",
            //    PaymentMethod = "paypal",
            //    Status = "Verioyiong",
            //};
            //myDataGrid.Items.Add(costumer);
        }

        private void StackPanel_Loaded(object sender, RoutedEventArgs e)
        {
            Fillingdatagrid();
        }

        private void ButtonHomeViewTrade_Click(object sender, RoutedEventArgs e)
        {
            var contactid = TextBoxHomeNewTrade.Text.Replace(" ", "");
            parentMinwindow.NewTrade(contactid);
        }
    }
}
