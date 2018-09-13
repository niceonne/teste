using LocalBitcoins;
using LocalBitcoins.API;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using static LocalBitcoins.GetContactInfo;

namespace TesteLocalbitcoinsApp
{

    public partial class UserControltest1 : System.Windows.Controls.UserControl
    {
        /// <summary>
        /// <para>Load with a int parameter to know the trade type</para>
        /// <para>1 - Selling bitcoins to paypal - Need verification</para>
        /// <para>2 - Selling bitcoins to a return paypal costumer</para>
        /// <para>3 - Buying bitcoins to paypal</para>
        /// <para>4 - Selling bitcoins</para>
        /// <para>5 - Buying bitcoins</para>
        /// </summary>
        public UserControltest1(string contactid, string Username, RootObject getContactApi, int TradeType)
        {
            InitializeComponent();
            ButtonUpdateFeedback1 = $"Update feedback for {Username}";
            ContactTradeNumber = contactid;
            InfoContact = getContactApi;
            UsernameTrade = Username;
            TypetradeNumber = TradeType;
            WebBrowserchatmessages.ObjectForScripting = new ScriptManager(this);
            path = MainWindow.directoryPath + contactid;
            if (Directory.Exists(path) == false)
                Directory.CreateDirectory(path);
            WindowTimer.Tick += new EventHandler(WindowTimer_tick);
            ChecktradeStatusTimer.Tick += new EventHandler(ChecktradeStatusTimer_Tick);
            ChecktradeStatusTimer.Interval = new TimeSpan(0, 0, 6);
            ChatMessagesTimer.Tick += new EventHandler(ChatMessagesTimer_Tick);
            ChatMessagesTimer.Interval = new TimeSpan(0, 0, 4);
            CloseTabTimer.Tick += new EventHandler(CloseTabTimer_Tick);
        }

        string path = "";
        int TypetradeNumber = 0;
        public int totalMinutesTrade = 0;
        decimal userTotalweekLimit = 0;
        decimal userweeklimitbeforetrade = 0;
        decimal userweeklimitaftertrade = 0;
        DateTime DayResetLimit = new DateTime();
        DateTime AccountCreatedDate = new DateTime();
        private DateTime TimeLastMessage { get; set; }
        string ContactTradeNumber = "";
        string UsernameTrade = "";
        string AttachmentFilepath = "";
        string InvoiceIdPaypal = "";
        private RootObject InfoContact { get; set; }
        private AccountInfo InfoAccount { get; set; }
        //private LocalBitcoinsDatabaseEntities _BaseDados = new LocalBitcoinsDatabaseEntities();
        DispatcherTimer WindowTimer = new DispatcherTimer();
        DispatcherTimer ChecktradeStatusTimer = new DispatcherTimer();
        DispatcherTimer ChatMessagesTimer = new DispatcherTimer();
        DispatcherTimer CloseTabTimer = new DispatcherTimer();
        bool TradeisSell = true;
        bool BoolPaymentsent = false;
        bool newMessageChat = true;
        bool TradeClosed = false;
        bool userMarkedRealName = false;
        bool userverified = false;
        bool creditCardAccepted = false;
        bool isfirstloadtimer = true;
        List<MessageListClass> ListMessages = new List<MessageListClass>();
        private Username DatabaseInfo { get; set; }
        RootObject CurrecttradeValue = null;
        string feedBackMessage = "";
        string feedbackType = "";

        public string Emailpaypal
        {
            get
            {
                return PaypalEmail;
            }
            set
            {
                if (PaypalEmail != value)
                {
                    PaypalEmail = value;
                    NotifyPropertyChanged("Emailpaypal");
                }
            }
        }
        public string PaypalEmail = "";
        public string ButtonUpdateFeedback1 { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void CloseTabTimer_Tick(object sender, EventArgs e)
        {
            MainWindow.ListremoveTab.Add($"tab{ContactTradeNumber}");
            CloseTabTimer.Stop();
        }

        private void UserInfoAccount()
        { 
            var APiKey = ConfigurationManager.AppSettings["ApiKeyNewTrades"];
            var ApiSecret = ConfigurationManager.AppSettings["ApiSecretNewTrades"];
            var client = new LocalBitcoinsAPI(APiKey, ApiSecret);
            LocalBitcoinsDatabaseEntities _BaseDados = new LocalBitcoinsDatabaseEntities();
            var InfoAccountApi = "";
            AccountInfo jsonUsername = null;
            var databaseuser = _BaseDados.Usernames.Any(x => x.Username1 == UsernameTrade);
            if (databaseuser == true)
            {
                Username UserData = _BaseDados.Usernames.FirstOrDefault(x => x.Username1 == UsernameTrade);
                if (UserData.UsernameInfo.FeedBackExists == true)
                {
                    feedBackMessage = UserData.UsernameInfo.myFeedBackMessage;
                    feedbackType = UserData.UsernameInfo.feedbacktype;
                }
                else
                {
                    InfoAccountApi = client.GetAccountInfo(UsernameTrade);
                    jsonUsername = JsonConvert.DeserializeObject<AccountInfo>(InfoAccountApi);
                    if (string.IsNullOrWhiteSpace(jsonUsername.data.my_feedback_msg))
                    {
                        feedBackMessage = "";
                        feedbackType = "neutral";
                        UserData.UsernameInfo.feedbacktype = jsonUsername.data.my_feedback;
                        UserData.UsernameInfo.myFeedBackMessage = jsonUsername.data.my_feedback_msg;
                        UserData.UsernameInfo.FeedBackExists = true;
                    }
                    else
                    {
                        feedBackMessage = jsonUsername.data.my_feedback_msg;
                        feedbackType = jsonUsername.data.my_feedback;
                        UserData.UsernameInfo.feedbacktype = jsonUsername.data.my_feedback;
                        UserData.UsernameInfo.myFeedBackMessage = jsonUsername.data.my_feedback_msg;
                        UserData.UsernameInfo.FeedBackExists = true;
                    }
                    _BaseDados.SaveChanges();
                }
            }
            else if (databaseuser == false)
            {
                if (!string.IsNullOrWhiteSpace(InfoAccountApi))
                    jsonUsername = JsonConvert.DeserializeObject<AccountInfo>(InfoAccountApi);
                else
                {
                    InfoAccountApi = client.GetAccountInfo(UsernameTrade);
                    jsonUsername = JsonConvert.DeserializeObject<AccountInfo>(InfoAccountApi);
                }
                var listusersdescending = _BaseDados.Usernames.OrderByDescending(x=> x.UsernameKey);
                var Lastuser = listusersdescending.FirstOrDefault();
                var usernameKey = Lastuser.UsernameKey + 1;

                _BaseDados.Usernames.Add(new Username()
                {
                    Username1 = UsernameTrade,
                    UsernameKey = usernameKey,
                    UsernameInfo = new UsernameInfo()
                    {
                        UsernameKey = usernameKey,
                        Verified = false,
                        Email = "",
                        AccountCreatedAt = jsonUsername.data.created_at,
                        RealName = InfoContact.data.buyer.real_name,
                        FeedBackExists = true,
                        myFeedBackMessage = jsonUsername.data.my_feedback_msg,
                        feedbacktype = jsonUsername.data.my_feedback
                    },
                });
                _BaseDados.SaveChanges();
            }
            if (TypetradeNumber == 1 || TypetradeNumber == 2 || TypetradeNumber == 4 )
            {
                Username UserData = _BaseDados.Usernames.FirstOrDefault(x => x.Username1 == UsernameTrade);
                if (UserData.UsernameInfo.AccountCreatedAt == null || UserData.UsernameInfo.RealName == null)
                {
                    if (!string.IsNullOrWhiteSpace(InfoAccountApi))
                        jsonUsername = JsonConvert.DeserializeObject<AccountInfo>(InfoAccountApi);
                    else
                    {
                        InfoAccountApi = client.GetAccountInfo(UsernameTrade);
                        jsonUsername = JsonConvert.DeserializeObject<AccountInfo>(InfoAccountApi);
                    }
                    UserData.UsernameInfo.AccountCreatedAt = jsonUsername.data.created_at;
                    UserData.UsernameInfo.RealName = InfoContact.data.buyer.real_name;
                    _BaseDados.SaveChanges();
                }
            }
            else
            {
                Username UserData = _BaseDados.Usernames.FirstOrDefault(x => x.Username1 == UsernameTrade);
                if (UserData.UsernameInfo.AccountCreatedAt == null || UserData.UsernameInfo.RealName == null)
                {
                    if (!string.IsNullOrWhiteSpace(InfoAccountApi))
                        jsonUsername = JsonConvert.DeserializeObject<AccountInfo>(InfoAccountApi);
                    else
                    {
                        InfoAccountApi = client.GetAccountInfo(UsernameTrade);
                        jsonUsername = JsonConvert.DeserializeObject<AccountInfo>(InfoAccountApi);
                    }
                    UserData.UsernameInfo.AccountCreatedAt = jsonUsername.data.created_at;
                    _BaseDados.SaveChanges();
                }
            }
            DatabaseInfo = _BaseDados.Usernames.FirstOrDefault(x => x.Username1 == UsernameTrade);
            if (TypetradeNumber == 1 || TypetradeNumber == 2 || TypetradeNumber == 4)
            {
                Emailpaypal = DatabaseInfo.UsernameInfo.Email;
            }
            else
                Emailpaypal = InfoContact.data.account_details.receiver_email;
            AccountCreatedDate = DatabaseInfo.UsernameInfo.AccountCreatedAt.Value;
        }

        bool LoadedFirstTime = true;

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (LoadedFirstTime == true)
            {
                GridTradePanel.Visibility = Visibility.Collapsed;
                GridLoading.Visibility = Visibility.Visible;
                LoadedFirstTime = false;
                try
                {
                    await Task.Run(() => UserInfoAccount());
                    DataContext = this;
                    if (TypetradeNumber == 1)// Paypal sale with verification required
                    {
                        TradeisSell = true;
                        Task WeekLimittask = new Task(WeekLimit);
                        WeekLimittask.Start();
                        await WeekLimittask;
                        Task AllInfotask = new Task(VerificationPaypalSell);
                        AllInfotask.Start();
                        Task ChatMessagestask = new Task(ContactMessages);
                        ChatMessagestask.Start();
                        await Task.WhenAll(AllInfotask, ChatMessagestask);
                    }
                    else if (TypetradeNumber == 2) // Paypal sale return costumer
                    {
                        TradeisSell = true;
                        Task WeekLimittask = new Task(WeekLimit);
                        WeekLimittask.Start();
                        await WeekLimittask;
                        Task AllInfotask = new Task(ReturnPaypalSell);
                        AllInfotask.Start();
                        Task ChatMessagestask = new Task(ContactMessages);
                        ChatMessagestask.Start();
                        await Task.WhenAll(AllInfotask, ChatMessagestask);
                    }
                    else if (TypetradeNumber == 3) // Paypal buy
                    {
                        TradeisSell = false;
                        Task AllInfotask = new Task(PaypalBuy);
                        AllInfotask.Start();
                        Task ChatMessagestask = new Task(ContactMessages);
                        ChatMessagestask.Start();
                        await Task.WhenAll(AllInfotask, ChatMessagestask);
                    }
                    else if (TypetradeNumber == 4) // Normal sell
                    {
                        TradeisSell = true;
                        Task AllInfotask = new Task(NormalSell);
                        AllInfotask.Start();
                        Task ChatMessagestask = new Task(ContactMessages);
                        ChatMessagestask.Start();
                        await Task.WhenAll(AllInfotask, ChatMessagestask);
                    }
                    else if (TypetradeNumber == 5) // Normal Buy
                    {
                        TradeisSell = false;
                        Task AllInfotask = new Task(NormalBuy);
                        AllInfotask.Start();
                        Task ChatMessagestask = new Task(ContactMessages);
                        ChatMessagestask.Start();
                        await Task.WhenAll(AllInfotask, ChatMessagestask);
                    }
                    await Task.Run(() => Feedback());
                    ChecktradeStatusTimer_Tick(null, null);
                    GridLoading.Visibility = Visibility.Collapsed;
                    GridTradePanel.Visibility = Visibility.Visible;
                }
                catch
                {
                    LoadedFirstTime = true;
                    StackPanelLoadingWaitingSpinner.Dispatcher.BeginInvoke((Action)(() =>
                    {
                        StackPanelLoadingWaitingSpinner.Visibility = Visibility.Collapsed;
                    }));
                    StackPanelLoadingWaitingWarn.Dispatcher.BeginInvoke((Action)(() =>
                    {
                        StackPanelLoadingWaitingWarn.Visibility = Visibility.Visible;
                    }));
                }
            }
        }

        private void Feedback()
        {
            PanelFeedback.Dispatcher.BeginInvoke((Action)(() =>
            {
                CheckBoxTrustworthy.IsChecked = false;
                CheckBoxPositive.IsChecked = false;
                CheckBoxNeutral.IsChecked = false;
                CheckBoxBlock.IsChecked = false;
                CheckBoxDistrust.IsChecked = false;
            }));

            if (feedbackType == "trust")
            {
                CheckBoxTrustworthy.Dispatcher.BeginInvoke((Action)(() =>
                {
                    CheckBoxTrustworthy.IsChecked = true;
                }));
            }
            else if (feedbackType == "positive")
            {
                CheckBoxPositive.Dispatcher.BeginInvoke((Action)(() =>
                {
                    CheckBoxPositive.IsChecked = true;
                }));
            }
            else if (feedbackType == "neutral")
            {
                CheckBoxNeutral.Dispatcher.BeginInvoke((Action)(() =>
                {
                    CheckBoxNeutral.IsChecked = true;
                }));
            }
            else if (feedbackType == "block")
            {
                CheckBoxBlock.Dispatcher.BeginInvoke((Action)(() =>
                {
                    CheckBoxBlock.IsChecked = true;
                }));
            }
            else if (feedbackType == "block_without_feedback")
            {
                CheckBoxDistrust.Dispatcher.BeginInvoke((Action)(() =>
                {
                    CheckBoxDistrust.IsChecked = true;
                }));
            }
            else if (string.IsNullOrEmpty(feedbackType))
            {
                CheckBoxNeutral.Dispatcher.BeginInvoke((Action)(() =>
                {
                    CheckBoxNeutral.IsChecked = true;
                }));
            }
            TextBoxFeedback.Dispatcher.BeginInvoke((Action)(() =>
            {
                TextBoxFeedback.Text = feedBackMessage;
            }));
        }

        private void VerificationPaypalSell()
        {
            PanelIdentifyingUser.Dispatcher.BeginInvoke((Action)(() =>
            {
                PanelIdentifyingUser.Visibility = Visibility.Visible;
            }));
            PanelButtonsNewBuyerPaypal.Dispatcher.BeginInvoke((Action)(() =>
            {
                PanelButtonsNewBuyerPaypal.Visibility = Visibility.Visible;
            }));

            var btcAmount = InfoContact.data.amount_btc;
            var totalAmount = InfoContact.data.amount;
            var currency = InfoContact.data.currency;
            var paymentMethod = InfoContact.data.advertisement.payment_method;
            var numberoftrades = InfoContact.data.buyer.trade_count;
            var feedbackScore = InfoContact.data.buyer.feedback_score;
            var countryIPCode = InfoContact.data.buyer.countrycode_by_ip;
            var countryPhoneCode = InfoContact.data.buyer.countrycode_by_phone_number;
            var AccountDatecreated = DateTime.Parse((AccountCreatedDate.ToShortDateString()));
            decimal.TryParse(totalAmount, out decimal amountTrade);

            TextBlockTitleLabel.Dispatcher.BeginInvoke((Action)(() =>
            {
                TextBlock TitleLabel1 = TextBlockTitleLabel;
                TitleLabel1.Inlines.Clear();
                TitleLabel1.Inlines.Add("Contact ");
                TitleLabel1.Inlines.Add(new Run("#" + ContactTradeNumber) { FontWeight = FontWeights.Bold });
                TitleLabel1.Inlines.Add(" : Selling " + btcAmount + " BTC for " + totalAmount + " " + currency);
            }));

            TextBlockTitleLabel2.Dispatcher.BeginInvoke((Action)(() =>
            {
                TextBlock TitleLabel2 = TextBlockTitleLabel2;
                TitleLabel2.Inlines.Clear();
                TitleLabel2.Inlines.Add("Selling to ");
                TitleLabel2.Inlines.Add(new Run(UsernameTrade) { FontWeight = FontWeights.Bold });
                TitleLabel2.Inlines.Add(" for " + paymentMethod);
            }));

            TextBlockLabelTitle3.Dispatcher.BeginInvoke((Action)(() =>
            {
                TextBlock Titlelabel3 = TextBlockLabelTitle3;
            Titlelabel3.Inlines.Clear();
            Titlelabel3.Inlines.Add("Trade status : ");
            Titlelabel3.Inlines.Add(new Run("Waiting for advertiser to verify identity of " + UsernameTrade) { FontWeight = FontWeights.Bold });
            }));

            TextBlockLabelTitleChat.Dispatcher.BeginInvoke((Action)(() =>
            {
                TextBlock TitleChatmessages = TextBlockLabelTitleChat;
                TitleChatmessages.Inlines.Clear();
                TitleChatmessages.Inlines.Add("Send a message to ");
                TitleChatmessages.Inlines.Add(new Run(UsernameTrade) { FontWeight = FontWeights.Bold });
            }));

            TextBlockIdentifyingUser1.Dispatcher.BeginInvoke((Action)(() =>
            {
                TextBlock Block1PanelIdentifyingUser = TextBlockIdentifyingUser1;
            Block1PanelIdentifyingUser.Inlines.Clear();
            Block1PanelIdentifyingUser.Inlines.Add("You have not verified identity of ");
            Block1PanelIdentifyingUser.Inlines.Add(new Run(UsernameTrade) { FontWeight = FontWeights.Bold });
            Block1PanelIdentifyingUser.Inlines.Add(". Please use the chat to get required info and then press the button below to mark ");
            Block1PanelIdentifyingUser.Inlines.Add(new Run(UsernameTrade) { FontWeight = FontWeights.Bold });
            Block1PanelIdentifyingUser.Inlines.Add(" as verified. ");
            }));

            TextBlockTradeInfo.Dispatcher.BeginInvoke((Action)(() =>
            {
                Hyperlink LinkuserWithFeedback = new Hyperlink();
                LinkuserWithFeedback.Inlines.Add(new Run(UsernameTrade + $" ({numberoftrades};{feedbackScore}%)") { FontWeight = FontWeights.Bold });
                TextBlock Block2TradeInfo = TextBlockTradeInfo;
                Block2TradeInfo.Inlines.Clear();
                Block2TradeInfo.Inlines.Add("Buyer : ");
                Block2TradeInfo.Inlines.Add(LinkuserWithFeedback);
                Block2TradeInfo.Inlines.Add(new LineBreak());
                Block2TradeInfo.Inlines.Add("Escrow amount : ");
                Block2TradeInfo.Inlines.Add(new Run(btcAmount) { FontWeight = FontWeights.Bold });
                Block2TradeInfo.Inlines.Add(new LineBreak());
                Block2TradeInfo.Inlines.Add("Payment amount : ");
                Block2TradeInfo.Inlines.Add(new Run(totalAmount + " " + currency) { FontWeight = FontWeights.Bold });
                Block2TradeInfo.Inlines.Add(new LineBreak());
                Block2TradeInfo.Inlines.Add("Account created at : ");
                Block2TradeInfo.Inlines.Add(new Run(AccountCreatedDate.ToString()) { FontWeight = FontWeights.Bold });
                if (AccountDatecreated.AddDays(14) > DateTime.Today)
                    Block2TradeInfo.Inlines.Add(new Run("⚠") { Foreground = new SolidColorBrush(Colors.Red), FontWeight = FontWeights.Bold });
                Block2TradeInfo.Inlines.Add(new LineBreak());
                Block2TradeInfo.Inlines.Add("Total Week Limit : ");
                Block2TradeInfo.Inlines.Add(new Run(userTotalweekLimit.ToString()) { FontWeight = FontWeights.Bold });
                if (amountTrade > userTotalweekLimit)
                    Block2TradeInfo.Inlines.Add(new Run("⚠") { Foreground = new SolidColorBrush(Colors.Red), FontWeight = FontWeights.Bold });
                Block2TradeInfo.Inlines.Add(new LineBreak());
                Block2TradeInfo.Inlines.Add("Current Week Limit : ");
                Block2TradeInfo.Inlines.Add(new Run(userweeklimitbeforetrade.ToString()) { FontWeight = FontWeights.Bold });
                if (amountTrade > userweeklimitbeforetrade)
                    Block2TradeInfo.Inlines.Add(new Run("⚠") { Foreground = new SolidColorBrush(Colors.Red), FontWeight = FontWeights.Bold });
                Block2TradeInfo.Inlines.Add(new LineBreak());
                Block2TradeInfo.Inlines.Add(new LineBreak());
                Block2TradeInfo.Inlines.Add("Real Name : ");
                Block2TradeInfo.Inlines.Add(new Run(InfoContact.data.buyer.real_name) { FontWeight = FontWeights.Bold });
                Block2TradeInfo.Inlines.Add(new LineBreak());
                Block2TradeInfo.Inlines.Add(new LineBreak());
                Block2TradeInfo.Inlines.Add("Country Ip : ");
                Block2TradeInfo.Inlines.Add(new Run(new Countries().GetCountryName(countryIPCode)) { FontWeight = FontWeights.Bold });
                if (countryIPCode != countryPhoneCode)
                    Block2TradeInfo.Inlines.Add(new Run("⚠") { Foreground = new SolidColorBrush(Colors.Red), FontWeight = FontWeights.Bold });
                Block2TradeInfo.Inlines.Add(new LineBreak());
                Block2TradeInfo.Inlines.Add("Country Phone number : ");
                Block2TradeInfo.Inlines.Add(new Run(new Countries().GetCountryName(countryPhoneCode)) { FontWeight = FontWeights.Bold });
                if (countryIPCode != countryPhoneCode)
                    Block2TradeInfo.Inlines.Add(new Run("⚠") { Foreground = new SolidColorBrush(Colors.Red), FontWeight = FontWeights.Bold });
                Block2TradeInfo.Inlines.Add(new LineBreak());
            }));
        }

        private void ReturnPaypalSell()
        {
            PanelIdentifyingUser.Dispatcher.BeginInvoke((Action)(() =>
            {
                PanelIdentifyingUser.Visibility = Visibility.Visible;
            }));
            PanelButtonsReturnCostumerPaypal.Dispatcher.BeginInvoke((Action)(() =>
            {
                PanelButtonsReturnCostumerPaypal.Visibility = Visibility.Visible;
            }));
            ButtonMarkUserverified.Dispatcher.BeginInvoke((Action)(() =>
            {
                ButtonMarkUserverified.Visibility = Visibility.Collapsed;
            }));

            var btcAmount = InfoContact.data.amount_btc;
            var totalAmount = InfoContact.data.amount;
            var currency = InfoContact.data.currency;
            var paymentMethod = InfoContact.data.advertisement.payment_method;
            var numberoftrades = InfoContact.data.buyer.trade_count;
            var feedbackScore = InfoContact.data.buyer.feedback_score;
            var countryIPCode = InfoContact.data.buyer.countrycode_by_ip;
            var countryPhoneCode = InfoContact.data.buyer.countrycode_by_phone_number;
            var AccountDatecreated = DateTime.Parse((AccountCreatedDate.ToShortDateString()));
            decimal.TryParse(totalAmount, out decimal amountTrade);

            TextBlockTitleLabel.Dispatcher.BeginInvoke((Action)(() =>
            {
                TextBlock TitleLabel1 = TextBlockTitleLabel;
                TitleLabel1.Inlines.Clear();
                TitleLabel1.Inlines.Add("Contact ");
                TitleLabel1.Inlines.Add(new Run("#" + ContactTradeNumber) { FontWeight = FontWeights.Bold });
                TitleLabel1.Inlines.Add(" : Selling " + btcAmount + " BTC for " + totalAmount + " " + currency);
            }));

            TextBlockTitleLabel2.Dispatcher.BeginInvoke((Action)(() =>
            {
                TextBlock TitleLabel2 = TextBlockTitleLabel2;
                TitleLabel2.Inlines.Clear();
                TitleLabel2.Inlines.Add("Selling to ");
                TitleLabel2.Inlines.Add(new Run(UsernameTrade) { FontWeight = FontWeights.Bold });
                TitleLabel2.Inlines.Add(" for " + paymentMethod);
            }));

            TextBlockLabelTitle3.Dispatcher.BeginInvoke((Action)(() =>
            {
                TextBlock Titlelabel3 = TextBlockLabelTitle3;
                Titlelabel3.Inlines.Clear();
                Titlelabel3.Inlines.Add("Trade status : ");
                Titlelabel3.Inlines.Add(new Run($"Waiting for {UsernameTrade} to confirm the payment.") { FontWeight = FontWeights.Bold });
            }));

            TextBlockLabelTitleChat.Dispatcher.BeginInvoke((Action)(() =>
            {
                TextBlock TitleChatmessages = TextBlockLabelTitleChat;
                TitleChatmessages.Inlines.Clear();
                TitleChatmessages.Inlines.Add("Send a message to ");
                TitleChatmessages.Inlines.Add(new Run(UsernameTrade) { FontWeight = FontWeights.Bold });
            }));

            TextBlockIdentifyingUser1.Dispatcher.BeginInvoke((Action)(() =>
            {
                TextBlock Block1PanelIdentifyingUser = TextBlockIdentifyingUser1;
                Block1PanelIdentifyingUser.Inlines.Clear();
                Block1PanelIdentifyingUser.Inlines.Add("You have already verified the identity of ");
                Block1PanelIdentifyingUser.Inlines.Add(new Run(UsernameTrade + " .") { FontWeight = FontWeights.Bold });
            }));

            TextBlockTradeInfo.Dispatcher.BeginInvoke((Action)(() =>
            {
                Hyperlink LinkuserWithFeedback = new Hyperlink();
                LinkuserWithFeedback.Inlines.Add(new Run(UsernameTrade + $" ({numberoftrades};{feedbackScore}%)") { FontWeight = FontWeights.Bold });
                TextBlock Block2TradeInfo = TextBlockTradeInfo;
                Block2TradeInfo.Inlines.Clear();
                Block2TradeInfo.Inlines.Add("Buyer : ");
                Block2TradeInfo.Inlines.Add(LinkuserWithFeedback);
                Block2TradeInfo.Inlines.Add(new LineBreak());
                Block2TradeInfo.Inlines.Add("Escrow amount : ");
                Block2TradeInfo.Inlines.Add(new Run(btcAmount) { FontWeight = FontWeights.Bold });
                Block2TradeInfo.Inlines.Add(new LineBreak());
                Block2TradeInfo.Inlines.Add("Payment amount : ");
                Block2TradeInfo.Inlines.Add(new Run(totalAmount + " " + currency) { FontWeight = FontWeights.Bold });
                Block2TradeInfo.Inlines.Add(new LineBreak());
                Block2TradeInfo.Inlines.Add("Account created at : ");
                Block2TradeInfo.Inlines.Add(new Run(AccountCreatedDate.ToString()) { FontWeight = FontWeights.Bold });
                if (AccountDatecreated.AddDays(14) > DateTime.Today)
                    Block2TradeInfo.Inlines.Add(new Run("⚠") { Foreground = new SolidColorBrush(Colors.Red), FontWeight = FontWeights.Bold });
                Block2TradeInfo.Inlines.Add(new LineBreak());
                Block2TradeInfo.Inlines.Add("Total Week Limit : ");
                Block2TradeInfo.Inlines.Add(new Run(userTotalweekLimit.ToString()) { FontWeight = FontWeights.Bold });
                if (amountTrade > userTotalweekLimit)
                    Block2TradeInfo.Inlines.Add(new Run("⚠") { Foreground = new SolidColorBrush(Colors.Red), FontWeight = FontWeights.Bold });
                Block2TradeInfo.Inlines.Add(new LineBreak());
                Block2TradeInfo.Inlines.Add("Current Week Limit : ");
                Block2TradeInfo.Inlines.Add(new Run(userweeklimitbeforetrade.ToString()) { FontWeight = FontWeights.Bold });
                if (amountTrade > userweeklimitbeforetrade)
                    Block2TradeInfo.Inlines.Add(new Run("⚠") { Foreground = new SolidColorBrush(Colors.Red), FontWeight = FontWeights.Bold });
                Block2TradeInfo.Inlines.Add(new LineBreak());
                Block2TradeInfo.Inlines.Add(new LineBreak());
                Block2TradeInfo.Inlines.Add("Real Name : ");
                Block2TradeInfo.Inlines.Add(new Run(InfoContact.data.buyer.real_name) { FontWeight = FontWeights.Bold });
                Block2TradeInfo.Inlines.Add(new LineBreak());
                Block2TradeInfo.Inlines.Add(new LineBreak());
                Block2TradeInfo.Inlines.Add("Country Ip : ");
                Block2TradeInfo.Inlines.Add(new Run(new Countries().GetCountryName(countryIPCode)) { FontWeight = FontWeights.Bold });
                if (countryIPCode != countryPhoneCode)
                    Block2TradeInfo.Inlines.Add(new Run("⚠") { Foreground = new SolidColorBrush(Colors.Red), FontWeight = FontWeights.Bold });
                Block2TradeInfo.Inlines.Add(new LineBreak());
                Block2TradeInfo.Inlines.Add("Country Phone number : ");
                Block2TradeInfo.Inlines.Add(new Run(new Countries().GetCountryName(countryPhoneCode)) { FontWeight = FontWeights.Bold });
                if (countryIPCode != countryPhoneCode)
                    Block2TradeInfo.Inlines.Add(new Run("⚠") { Foreground = new SolidColorBrush(Colors.Red), FontWeight = FontWeights.Bold });
                Block2TradeInfo.Inlines.Add(new LineBreak());
            }));
        }

        private void PaypalBuy()
        {
            PanelButtonsBuyingBitcoins.Dispatcher.BeginInvoke((Action)(() =>
            {
                PanelButtonsBuyingBitcoins.Visibility = Visibility.Visible;
            }));
            PanelBuyingBitcoins.Dispatcher.BeginInvoke((Action)(() =>
            {
                PanelBuyingBitcoins.Visibility = Visibility.Visible;
            }));
            ButtonMessagesListBox.Dispatcher.BeginInvoke((Action)(() =>
            {
                ButtonMessagesListBox.Visibility = Visibility.Visible;
            }));

            var btcAmount = InfoContact.data.amount_btc;
            var btcfees = InfoContact.data.fee_btc;
            var totalAmount = InfoContact.data.amount;
            var currency = InfoContact.data.currency;
            var paymentMethod = InfoContact.data.advertisement.payment_method;
            var numberoftrades = InfoContact.data.seller.trade_count;
            var feedbackScore = InfoContact.data.seller.feedback_score;

            TextBlockTitleLabel.Dispatcher.BeginInvoke((Action)(() =>
            {
                TextBlock TitleLabel1 = TextBlockTitleLabel;
                TitleLabel1.Inlines.Clear();
                TitleLabel1.Inlines.Add("Contact ");
                TitleLabel1.Inlines.Add(new Run("#" + ContactTradeNumber) { FontWeight = FontWeights.Bold });
                TitleLabel1.Inlines.Add(" : Buying " + btcAmount + " BTC for " + totalAmount + " " + currency);
            }));

            TextBlockTitleLabel2.Dispatcher.BeginInvoke((Action)(() =>
            {
                TextBlock TitleLabel2 = TextBlockTitleLabel2;
                TitleLabel2.Inlines.Clear();
                TitleLabel2.Inlines.Add("Buying to ");
                TitleLabel2.Inlines.Add(new Run(UsernameTrade) { FontWeight = FontWeights.Bold });
                TitleLabel2.Inlines.Add(" for " + paymentMethod);
            }));

            TextBlockLabelTitle3.Dispatcher.BeginInvoke((Action)(() =>
            {
                TextBlock Titlelabel3 = TextBlockLabelTitle3;
                Titlelabel3.Inlines.Clear();
                Titlelabel3.Inlines.Add("Trade status : ");
                Titlelabel3.Inlines.Add(new Run($"Waiting for {UsernameTrade} to confirm the payment.") { FontWeight = FontWeights.Bold });
            }));

            TextBlockLabelTitleChat.Dispatcher.BeginInvoke((Action)(() =>
            {
                TextBlock TitleChatmessages = TextBlockLabelTitleChat;
                TitleChatmessages.Inlines.Clear();
                TitleChatmessages.Inlines.Add("Send a message to ");
                TitleChatmessages.Inlines.Add(new Run(UsernameTrade) { FontWeight = FontWeights.Bold });
            }));

            TextBlockSellerInfo.Dispatcher.BeginInvoke((Action)(() =>
            {
                Hyperlink LinkuserWithFeedback = new Hyperlink();
                LinkuserWithFeedback.Inlines.Add(new Run(UsernameTrade + $" ({numberoftrades};{feedbackScore}%)") { FontWeight = FontWeights.Bold });
                TextBlock Block1TradeInfo = TextBlockSellerInfo;
                Block1TradeInfo.Inlines.Clear();
                Block1TradeInfo.Inlines.Add("Seller : ");
                Block1TradeInfo.Inlines.Add(LinkuserWithFeedback);
                Block1TradeInfo.Inlines.Add(new LineBreak());
                Block1TradeInfo.Inlines.Add("Account created at : ");
                Block1TradeInfo.Inlines.Add(new Run(AccountCreatedDate.ToString()) { FontWeight = FontWeights.Bold });
                Block1TradeInfo.Inlines.Add(new LineBreak());
                Block1TradeInfo.Inlines.Add("Escrow amount : ");
                Block1TradeInfo.Inlines.Add(new Run(btcAmount) { FontWeight = FontWeights.Bold });
                Block1TradeInfo.Inlines.Add(new LineBreak());
                Block1TradeInfo.Inlines.Add("Payment amount : ");
                Block1TradeInfo.Inlines.Add(new Run(totalAmount + " " + currency) { FontWeight = FontWeights.Bold });
                Block1TradeInfo.Inlines.Add(new LineBreak());
                //Block1TradeInfo.Inlines.Add("Commom trades : ");
                //Block1TradeInfo.Inlines.Add(new Run(InfoAccount.data.has_common_trades.ToString()) { FontWeight = FontWeights.Bold });
                //Block1TradeInfo.Inlines.Add(new LineBreak());
            }));
            this.Dispatcher.BeginInvoke((Action)(() =>
            {

            }));
        }

        private void NormalSell()
        {
            columnPanelbuttons.Dispatcher.BeginInvoke((Action)(() =>
            {
                columnPanelbuttons.Width = new GridLength(0);
            }));
            PanelIdentifyingUser.Dispatcher.BeginInvoke((Action)(() =>
            {
                PanelIdentifyingUser.Visibility = Visibility.Visible;
            }));
            ButtonMarkUserverified.Dispatcher.BeginInvoke((Action)(() =>
            {
                ButtonMarkUserverified.Visibility = Visibility.Collapsed;
            }));
            ButtonMessagesListBox.Dispatcher.BeginInvoke((Action)(() =>
            {
                ButtonMessagesListBox.Visibility = Visibility.Collapsed;
            }));

            var btcAmount = InfoContact.data.amount_btc;
            var totalAmount = InfoContact.data.amount;
            var currency = InfoContact.data.currency;
            var paymentMethod = InfoContact.data.advertisement.payment_method;
            var numberoftrades = InfoContact.data.buyer.trade_count;
            var feedbackScore = InfoContact.data.buyer.feedback_score;
            var countryIPCode = InfoContact.data.buyer.countrycode_by_ip;
            var countryPhoneCode = InfoContact.data.buyer.countrycode_by_phone_number;
            var AccountDatecreated = DateTime.Parse((AccountCreatedDate.ToShortDateString()));
            decimal.TryParse(totalAmount, out decimal amountTrade);

            TextBlockTitleLabel.Dispatcher.BeginInvoke((Action)(() =>
            {
                TextBlock TitleLabel1 = TextBlockTitleLabel;
                TitleLabel1.Inlines.Clear();
                TitleLabel1.Inlines.Add("Contact ");
                TitleLabel1.Inlines.Add(new Run("#" + ContactTradeNumber) { FontWeight = FontWeights.Bold });
                TitleLabel1.Inlines.Add(" : Selling " + btcAmount + " BTC for " + totalAmount + " " + currency);
            }));

            TextBlockTitleLabel2.Dispatcher.BeginInvoke((Action)(() =>
            {
                TextBlock TitleLabel2 = TextBlockTitleLabel2;
                TitleLabel2.Inlines.Clear();
                TitleLabel2.Inlines.Add("Selling to ");
                TitleLabel2.Inlines.Add(new Run(UsernameTrade) { FontWeight = FontWeights.Bold });
                TitleLabel2.Inlines.Add(" for " + paymentMethod);
            }));

            TextBlockLabelTitle3.Dispatcher.BeginInvoke((Action)(() =>
            {
                TextBlock Titlelabel3 = TextBlockLabelTitle3;
                Titlelabel3.Inlines.Clear();
                Titlelabel3.Inlines.Add("Trade status : ");
                Titlelabel3.Inlines.Add(new Run($"Waiting for {UsernameTrade} to confirm the payment.") { FontWeight = FontWeights.Bold });
            }));

            TextBlockLabelTitleChat.Dispatcher.BeginInvoke((Action)(() =>
            {
                TextBlock TitleChatmessages = TextBlockLabelTitleChat;
                TitleChatmessages.Inlines.Clear();
                TitleChatmessages.Inlines.Add("Send a message to ");
                TitleChatmessages.Inlines.Add(new Run(UsernameTrade) { FontWeight = FontWeights.Bold });
            }));

            TextBlockIdentifyingUser1.Dispatcher.BeginInvoke((Action)(() =>
            {
                TextBlock Block1PanelIdentifyingUser = TextBlockIdentifyingUser1;
                Block1PanelIdentifyingUser.Inlines.Clear();
                Block1PanelIdentifyingUser.Inlines.Add("You have already verified the identity of ");
                Block1PanelIdentifyingUser.Inlines.Add(new Run(UsernameTrade + " .") { FontWeight = FontWeights.Bold });
            }));

            TextBlockTradeInfo.Dispatcher.BeginInvoke((Action)(() =>
            {
                Hyperlink LinkuserWithFeedback = new Hyperlink();
                LinkuserWithFeedback.Inlines.Add(new Run(UsernameTrade + $" ({numberoftrades};{feedbackScore}%)") { FontWeight = FontWeights.Bold });
                TextBlock Block2TradeInfo = TextBlockTradeInfo;
                Block2TradeInfo.Inlines.Clear();
                Block2TradeInfo.Inlines.Add("Buyer : ");
                Block2TradeInfo.Inlines.Add(LinkuserWithFeedback);
                Block2TradeInfo.Inlines.Add(new LineBreak());
                Block2TradeInfo.Inlines.Add("Escrow amount : ");
                Block2TradeInfo.Inlines.Add(new Run(btcAmount) { FontWeight = FontWeights.Bold });
                Block2TradeInfo.Inlines.Add(new LineBreak());
                Block2TradeInfo.Inlines.Add("Payment amount : ");
                Block2TradeInfo.Inlines.Add(new Run(totalAmount + " " + currency) { FontWeight = FontWeights.Bold });
                Block2TradeInfo.Inlines.Add(new LineBreak());
                Block2TradeInfo.Inlines.Add("Account created at : ");
                Block2TradeInfo.Inlines.Add(new Run(AccountCreatedDate.ToString()) { FontWeight = FontWeights.Bold });
                if (AccountDatecreated.AddDays(14) > DateTime.Today)
                    Block2TradeInfo.Inlines.Add(new Run("⚠") { Foreground = new SolidColorBrush(Colors.Red), FontWeight = FontWeights.Bold });
                Block2TradeInfo.Inlines.Add(new LineBreak());
                Block2TradeInfo.Inlines.Add("Real Name : ");
                Block2TradeInfo.Inlines.Add(new Run(InfoContact.data.buyer.real_name) { FontWeight = FontWeights.Bold });
                Block2TradeInfo.Inlines.Add(new LineBreak());
                Block2TradeInfo.Inlines.Add(new LineBreak());
                Block2TradeInfo.Inlines.Add("Country Ip : ");
                Block2TradeInfo.Inlines.Add(new Run(new Countries().GetCountryName(countryIPCode)) { FontWeight = FontWeights.Bold });
                if (countryIPCode != countryPhoneCode)
                    Block2TradeInfo.Inlines.Add(new Run("⚠") { Foreground = new SolidColorBrush(Colors.Red), FontWeight = FontWeights.Bold });
                Block2TradeInfo.Inlines.Add(new LineBreak());
                Block2TradeInfo.Inlines.Add("Country Phone number : ");
                Block2TradeInfo.Inlines.Add(new Run(new Countries().GetCountryName(countryPhoneCode)) { FontWeight = FontWeights.Bold });
                if (countryIPCode != countryPhoneCode)
                    Block2TradeInfo.Inlines.Add(new Run("⚠") { Foreground = new SolidColorBrush(Colors.Red), FontWeight = FontWeights.Bold });
                Block2TradeInfo.Inlines.Add(new LineBreak());
            }));
        }

        private void NormalBuy()
        {
            columnPanelbuttons.Dispatcher.BeginInvoke((Action)(() =>
            {
                columnPanelbuttons.Width = new GridLength(0);
            }));
            PanelBuyingBitcoins.Dispatcher.BeginInvoke((Action)(() =>
            {
                PanelBuyingBitcoins.Visibility = Visibility.Visible;
            }));
            ButtonMessagesListBox.Dispatcher.BeginInvoke((Action)(() =>
            {
                ButtonMessagesListBox.Visibility = Visibility.Collapsed;
            }));
            ButtonMessagesListBox.Dispatcher.BeginInvoke((Action)(() =>
            {
                ButtonMessagesListBox.Visibility = Visibility.Visible;
            }));

            var btcAmount = InfoContact.data.amount_btc;
            var btcfees = InfoContact.data.fee_btc;
            var totalAmount = InfoContact.data.amount;
            var currency = InfoContact.data.currency;
            var paymentMethod = InfoContact.data.advertisement.payment_method;
            var numberoftrades = InfoContact.data.seller.trade_count;
            var feedbackScore = InfoContact.data.seller.feedback_score;

            TextBlockTitleLabel.Dispatcher.BeginInvoke((Action)(() =>
            {
                TextBlock TitleLabel1 = TextBlockTitleLabel;
                TitleLabel1.Inlines.Clear();
                TitleLabel1.Inlines.Add("Contact ");
                TitleLabel1.Inlines.Add(new Run("#" + ContactTradeNumber) { FontWeight = FontWeights.Bold });
                TitleLabel1.Inlines.Add(" : Buying " + btcAmount + " BTC for " + totalAmount + " " + currency);
            }));

            TextBlockTitleLabel2.Dispatcher.BeginInvoke((Action)(() =>
            {
                TextBlock TitleLabel2 = TextBlockTitleLabel2;
                TitleLabel2.Inlines.Clear();
                TitleLabel2.Inlines.Add("Buying to ");
                TitleLabel2.Inlines.Add(new Run(UsernameTrade) { FontWeight = FontWeights.Bold });
                TitleLabel2.Inlines.Add(" for " + paymentMethod);
            }));

            TextBlockLabelTitle3.Dispatcher.BeginInvoke((Action)(() =>
            {
                TextBlock Titlelabel3 = TextBlockLabelTitle3;
                Titlelabel3.Inlines.Clear();
                Titlelabel3.Inlines.Add("Trade status : ");
                Titlelabel3.Inlines.Add(new Run($"Waiting for {UsernameTrade} to confirm the payment.") { FontWeight = FontWeights.Bold });
            }));

            TextBlockLabelTitleChat.Dispatcher.BeginInvoke((Action)(() =>
            {
                TextBlock TitleChatmessages = TextBlockLabelTitleChat;
                TitleChatmessages.Inlines.Clear();
                TitleChatmessages.Inlines.Add("Send a message to ");
                TitleChatmessages.Inlines.Add(new Run(UsernameTrade) { FontWeight = FontWeights.Bold });
            }));

            TextBlockSellerInfo.Dispatcher.BeginInvoke((Action)(() =>
            {
                Hyperlink LinkuserWithFeedback = new Hyperlink();
                LinkuserWithFeedback.Inlines.Add(new Run(UsernameTrade + $" ({numberoftrades};{feedbackScore}%)") { FontWeight = FontWeights.Bold });
                TextBlock Block1TradeInfo = TextBlockSellerInfo;
                Block1TradeInfo.Inlines.Clear();
                Block1TradeInfo.Inlines.Add("Seller : ");
                Block1TradeInfo.Inlines.Add(LinkuserWithFeedback);
                Block1TradeInfo.Inlines.Add(new LineBreak());
                Block1TradeInfo.Inlines.Add("Account created at : ");
                Block1TradeInfo.Inlines.Add(new Run(AccountCreatedDate.ToString()) { FontWeight = FontWeights.Bold });
                Block1TradeInfo.Inlines.Add(new LineBreak());
                Block1TradeInfo.Inlines.Add("Escrow amount : ");
                Block1TradeInfo.Inlines.Add(new Run(btcAmount) { FontWeight = FontWeights.Bold });
                Block1TradeInfo.Inlines.Add(new LineBreak());
                Block1TradeInfo.Inlines.Add("Payment amount : ");
                Block1TradeInfo.Inlines.Add(new Run(totalAmount + " " + currency) { FontWeight = FontWeights.Bold });
                Block1TradeInfo.Inlines.Add(new LineBreak());
                Block1TradeInfo.Inlines.Add("Commom trades : ");
                Block1TradeInfo.Inlines.Add(new Run(InfoAccount.data.has_common_trades.ToString()) { FontWeight = FontWeights.Bold });
                Block1TradeInfo.Inlines.Add(new LineBreak());
            }));
        }

        private void WeekLimit()
        {
            LocalBitcoinsDatabaseEntities _BaseDados = new LocalBitcoinsDatabaseEntities();
            var AccountDatecreated = DateTime.Parse((AccountCreatedDate.ToShortDateString()));
            var Amounttrade = InfoContact.data.amount;
            var userNumberoftrades = InfoContact.data.buyer.trade_count;
            var userFeedbackScore = InfoContact.data.buyer.feedback_score;
            int UserNumberCompletedtrades = 0;
            if (userNumberoftrades.ToString().Contains("+"))
            {
                var CompletedTrades = userNumberoftrades.Replace("+", "");
                int.TryParse(CompletedTrades, out int CompletedTradesNUmber);
                UserNumberCompletedtrades = CompletedTradesNUmber + 1;
            }
            else
            {
                int.TryParse(userNumberoftrades, out UserNumberCompletedtrades);
            }
            if (userFeedbackScore == 0 && UserNumberCompletedtrades < 10)
            {
                userFeedbackScore = 100;
            }
            decimal DifferenceFeedbackScore = 100 - userFeedbackScore;
            var FeedbackScoreDouble = decimal.ToDouble(DifferenceFeedbackScore);
            var PercentageFeedback = 1 - (FeedbackScoreDouble * 0.025);
            var FeedBackDifference = Convert.ToDecimal(PercentageFeedback);
            decimal.TryParse(Amounttrade, out decimal amountTrade);

            if (UserNumberCompletedtrades == 0 || (AccountDatecreated.AddDays(14) > DateTime.Today))
                userTotalweekLimit = 60;
            else if (UserNumberCompletedtrades > 0 && UserNumberCompletedtrades <= 10)
                userTotalweekLimit = 100;
            else if (UserNumberCompletedtrades > 10 && UserNumberCompletedtrades <= 30)
                userTotalweekLimit = 250;
            else if (UserNumberCompletedtrades > 30 && UserNumberCompletedtrades <= 70)
                userTotalweekLimit = 500;
            else if (UserNumberCompletedtrades > 70 && UserNumberCompletedtrades <= 100)
                userTotalweekLimit = 750;
            else if (UserNumberCompletedtrades > 100 && UserNumberCompletedtrades <= 500)
                userTotalweekLimit = 1500;
            else if (UserNumberCompletedtrades > 500 && UserNumberCompletedtrades <= 1000)
                userTotalweekLimit = 2500;
            else if (UserNumberCompletedtrades > 1000 && UserNumberCompletedtrades <= 10000)
                userTotalweekLimit = 5000;
            else if (UserNumberCompletedtrades > 10000)
                userTotalweekLimit = 25000;
            if (DifferenceFeedbackScore > 0)
                userTotalweekLimit = (userTotalweekLimit * FeedBackDifference);
            if (UserNumberCompletedtrades < 30)
            {
                creditCardAccepted = false;
                ButtonSendCreditcardVerification.Dispatcher.BeginInvoke((Action)(() =>
                {
                    ButtonSendCreditcardVerification.Visibility = Visibility.Collapsed;
                }));
            }
            else
            {
                creditCardAccepted = true;
                ListboxitemCreditcardnotallowed.Dispatcher.BeginInvoke((Action)(() =>
                {
                    ListboxitemCreditcardnotallowed.Visibility = Visibility.Collapsed;
                }));
            }
            var baseDadosUser = _BaseDados.Usernames.Any(x => x.Username1 == UsernameTrade);
            if (baseDadosUser == true)
            {
                var DatabaseUser = _BaseDados.Usernames.FirstOrDefault(x => x.Username1 == UsernameTrade);
                if (_BaseDados.UsernameTrades.Any(x => x.UsernameKey == DatabaseUser.UsernameKey) == true)
                {
                    var lastResetday = ResetDay().Subtract(TimeSpan.FromDays(7));
                    var totaltrades = DatabaseUser.UsernameTrades.Where(x => x.Date >= lastResetday).Where(x => x.Trade_Type.Contains("SELL"));
                    var TotalAmountLasttrades = totaltrades.Sum(x => x.Trade_amount);
                    userweeklimitbeforetrade = Math.Max(0, (userTotalweekLimit - TotalAmountLasttrades));
                    userweeklimitaftertrade = Math.Max(0, (userweeklimitbeforetrade - amountTrade));
                    DayResetLimit = ResetDay();
                }
                else
                {
                    if (ResetDay().Subtract(DateTime.Today).TotalDays <= 2 )
                    {
                        userweeklimitbeforetrade = Math.Max(0, userTotalweekLimit);
                        userweeklimitaftertrade = Math.Max(0, (userweeklimitbeforetrade - amountTrade));
                        DayResetLimit = ResetDay().AddDays(7);
                    }
                    else
                    {
                        userweeklimitbeforetrade = Math.Max(0, userTotalweekLimit);
                        userweeklimitaftertrade = Math.Max(0, (userweeklimitbeforetrade - amountTrade));
                        DayResetLimit = ResetDay();
                    }
                }
            }
            else
            {
                if (ResetDay().Subtract(DateTime.Today).TotalDays <= 2)
                {
                    userweeklimitbeforetrade = Math.Max(0, userTotalweekLimit);
                    userweeklimitaftertrade = Math.Max(0, (userweeklimitbeforetrade - amountTrade));
                    DayResetLimit = ResetDay().AddDays(7);
                }
                else
                {
                    userweeklimitbeforetrade = Math.Max(0, userTotalweekLimit);
                    userweeklimitaftertrade = Math.Max(0, (userweeklimitbeforetrade - amountTrade));
                    DayResetLimit = ResetDay();
                }
            }
        }

        DateTime ResetDay()
        {
            var date = DateTime.Now;
            var dateDay = date.DayOfWeek;
            var dateresetday = DateTime.Now;
            switch (dateDay)
            {
                case DayOfWeek.Friday:
                    dateresetday = date.AddDays(2);
                    break;
                case DayOfWeek.Sunday:
                    dateresetday = date.AddDays(7);
                    break;
                case DayOfWeek.Monday:
                    dateresetday = date.AddDays(6);
                    break;
                case DayOfWeek.Thursday:
                    dateresetday = date.AddDays(3);
                    break;
                case DayOfWeek.Wednesday:
                    dateresetday = date.AddDays(5);
                    break;
                case DayOfWeek.Tuesday:
                    dateresetday = date.AddDays(5);
                    break;
                case DayOfWeek.Saturday:
                    dateresetday = date.AddDays(1);
                    break;
            }
            return dateresetday;
        }

        async private void ChatMessagesTimer_Tick(object sender, EventArgs e)
        {
            await Task.Run(() => ContactMessages());
        }

        private void ContactMessages()
        {
            ChatMessagesTimer.Stop();
            double minutesdifference = 5;
            if (newMessageChat == true)
            {
                TimeSpan minutesspan = DateTime.Now.Subtract(InfoContact.data.created_at);
                minutesdifference = Convert.ToDouble(minutesspan.TotalMinutes);
            }
            if (minutesdifference < 4 || newMessageChat == false)
            {
                var TotalMessages = MainWindow.MessagesLCB;
                if (TotalMessages != null)
                {
                    var chatMessages = TotalMessages.data.message_list.Where(x => x.contact_id == ContactTradeNumber).Where(x => x.created_at > TimeLastMessage);
                    if (chatMessages.Count() > 0)
                    {
                        var lastMessage = chatMessages.First();
                        var lastMessageDate = lastMessage.created_at;

                        foreach (var msg in chatMessages)
                        {
                            var message = msg.msg;
                            DateTime date = msg.created_at;
                            var fileName = msg.attachment_name;
                            var contactID = msg.contact_id;
                            int fileID = 0;
                            var fileurl = msg.attachment_url;
                            var fileType = msg.attachment_type;
                            if (!string.IsNullOrWhiteSpace(fileName))
                            {
                                var idFile = fileurl.Split('/')[6];
                                int.TryParse(idFile, out fileID);
                            }
                            var listitem = new MessageListClass()
                            {
                                Message = message,
                                create_at = date,
                                Username = msg.sender.username,
                                contactID = contactID,
                                FileID = fileID,
                                FileName = fileName,
                                FileType = fileType
                            };
                            if (string.IsNullOrEmpty(message))
                            {
                                if (!ListMessages.Any(x => x.FileID == fileID))
                                    ListMessages.Add(listitem);
                            }
                            else
                            {
                                if (!ListMessages.Any(x => x.Message == message))
                                    ListMessages.Add(listitem);
                            }
                        }

                        TimeLastMessage = lastMessage.created_at;
                        StringBuilder chatString = new StringBuilder();
                        foreach (var item in ListMessages.OrderByDescending(x => x.create_at))
                        {
                            var message = item.Message;
                            var username = item.Username;
                            var filename = item.FileName;
                            var date = item.create_at;
                            var itemID = item.FileID;

                            if (!string.IsNullOrWhiteSpace(message) && string.IsNullOrWhiteSpace(filename))
                            {
                                chatString.AppendLine($"<p><small>{date.ToString()}</small>");
                                chatString.Append($"<strong>{username}</strong> : {message}</p>");
                                chatString.AppendLine("<hr size=\"0.5\">");
                            }
                            else if (string.IsNullOrWhiteSpace(message) && !string.IsNullOrWhiteSpace(filename))
                            {
                                chatString.AppendLine($"<p><small>{date.ToString()}</small>");
                                chatString.Append($"<strong>{username}</strong> :<br> <a href=\"#\" onclick=\"window.external.MethodToCallFromScript({itemID});\">{filename + "/" + itemID}</a></p>");
                                chatString.AppendLine("<hr size=\"0.5\">");
                            }
                            else if (!string.IsNullOrWhiteSpace(message) && !string.IsNullOrWhiteSpace(filename))
                            {
                                chatString.AppendLine($"<p><small>{date.ToString()}</small>");
                                chatString.AppendLine($"<strong>{username}</strong> : {message}");
                                chatString.Append($"<a href=\"#\" onclick=\"window.external.MethodToCallFromScript({itemID});\">{filename + "/" + itemID}</a></p>");
                                chatString.AppendLine("<hr size=\"0.5\">");
                            }
                        }
                        chatString.Replace("\r\n", "<br>");
                        WebBrowserchatmessages.Dispatcher.BeginInvoke((Action)(() =>
                        {
                            WebBrowserchatmessages.NavigateToString("<head><meta http-equiv='Content-Type' content='text/html;charset=UTF-8'></head><scroll=\"no\"><font size=\"2.85px\"> <body bgcolor=\"#f5f5f5\">" + chatString.ToString() + "</body>");
                        }));
                    }
                }
            }
            else
            {
                var APiKey = ConfigurationManager.AppSettings["ApiKeyChatMessages"];
                var ApiSecret = ConfigurationManager.AppSettings["ApiSecretChatMessages"];
                var client = new LocalBitcoinsAPI(APiKey, ApiSecret);
                var messagesNumber = "";
                try
                {
                    messagesNumber = client.GetContactMessages(ContactTradeNumber);
                    newMessageChat = false;
                    RootGetContactMessages countnumber = JsonConvert.DeserializeObject<RootGetContactMessages>(messagesNumber);
                    var chatMessages = countnumber.data.message_list.Where(x => !x.read).OrderByDescending(x => x.created_at);

                    if (countnumber.data.message_count == 0)
                    {
                        newMessageChat = false;
                        WebBrowserchatmessages.Dispatcher.BeginInvoke((Action)(() =>
                        {
                            WebBrowserchatmessages.NavigateToString("<head><meta http-equiv='Content-Type' content='text/html;charset=UTF-8'></head><scroll=\"no\"><font size=\"2.85px\"> <body bgcolor=\"#f5f5f5\"></body>");
                        }));
                        // do something in case there is no messages
                    }
                    else
                    {
                        var lastMessage = chatMessages.First();
                        var lastMessageDate = lastMessage.created_at;
                        foreach (var msg in chatMessages)
                        {
                            var message = msg.msg;
                            DateTime date = msg.created_at;
                            var fileName = msg.attachment_name;
                            var contactID = ContactTradeNumber;
                            int fileID = 0;
                            var fileurl = msg.attachment_url;
                            var fileType = msg.attachment_type;
                            if (!string.IsNullOrWhiteSpace(fileName))
                            {
                                var idFile = fileurl.Split('/')[6];
                                int.TryParse(idFile, out fileID);
                            }
                            var listitem = new MessageListClass()
                            {
                                Message = message,
                                create_at = date,
                                Username = msg.sender.username,
                                contactID = contactID,
                                FileID = fileID,
                                FileName = fileName,
                                FileType = fileType
                            };
                            if (string.IsNullOrEmpty(message))
                            {
                                if (ListMessages.Any(x => x.FileID == fileID) == true)
                                { }
                                else
                                    ListMessages.Add(listitem);
                            }
                            else
                            {
                                if (ListMessages.Any(x => x.Message == message) == true)
                                { }
                                else
                                    ListMessages.Add(listitem);
                            }
                        }

                        TimeLastMessage = lastMessage.created_at;
                        StringBuilder chatString = new StringBuilder();
                        foreach (var item in ListMessages.OrderByDescending(x => x.create_at))
                        {
                            var message = item.Message;
                            //var message = asd.Remove(asd.Length - 4);
                            var username = item.Username;
                            var filename = item.FileName;
                            var date = item.create_at;
                            var itemID = item.FileID;
                            if (!string.IsNullOrWhiteSpace(message) && string.IsNullOrWhiteSpace(filename))
                            {
                                chatString.AppendLine($"<p><small>{date.ToString()}</small>");
                                chatString.Append($"<strong>{username}</strong> : {message}</p>");
                                chatString.AppendLine("<hr size=\"0.5\">");
                            }
                            else if (string.IsNullOrWhiteSpace(message) && !string.IsNullOrWhiteSpace(filename))
                            {
                                chatString.AppendLine($"<p><small>{date.ToString()}</small>");
                                chatString.Append($"<strong>{username}</strong> :<br> <a href=\"#\" onclick=\"window.external.MethodToCallFromScript({itemID});\">{filename + "/" + itemID}</a></p>");
                                chatString.AppendLine("<hr size=\"0.5\">");
                            }
                            else if (!string.IsNullOrWhiteSpace(message) && !string.IsNullOrWhiteSpace(filename))
                            {
                                chatString.AppendLine($"<p><small>{date.ToString()}</small>");
                                chatString.AppendLine($"<strong>{username}</strong> : {message}");
                                chatString.Append($"<a href=\"#\" onclick=\"window.external.MethodToCallFromScript({itemID});\">{filename + "/" + itemID}</a></p>");
                                chatString.AppendLine("<hr size=\"0.5\">");
                            }
                        }
                        chatString.Replace("\r\n", "<br>");
                        WebBrowserchatmessages.Dispatcher.BeginInvoke((Action)(() =>
                        {
                            WebBrowserchatmessages.NavigateToString("<head><meta http-equiv='Content-Type' content='text/html;charset=UTF-8'></head><scroll=\"no\"><font size=\"2.85px\"> <body bgcolor=\"#f5f5f5\">" + chatString.ToString() + "</body>");
                        }));
                    }
                }
                catch (Exception ep)
                {
                    MainWindow.ErrorLogging(ep);
                }
            }
            ChatMessagesTimer.Start();
        }

        private void PaymentCompleted()
        {
            ButtonMarkasPaid.Dispatcher.BeginInvoke((Action)(() =>
            {
                ButtonMarkasPaid.Visibility = Visibility.Collapsed;
            }));
            TextBlockLabelTitle3.Dispatcher.BeginInvoke((Action)(() =>
            {
                TextBlock Titlelabel3 = TextBlockLabelTitle3;
                Titlelabel3.Inlines.Clear();
                Titlelabel3.Inlines.Add("Trade status : ");
                Titlelabel3.Inlines.Add(new Run("Payment Market as completed, waiting for escrow release.") { FontWeight = FontWeights.Bold });
            }));
            if (TradeisSell == false)
            {
                TextBlockPaymentWindow.Dispatcher.BeginInvoke((Action)(() =>
                {
                    TextBlock Titlelabel3 = TextBlockPaymentWindow;
                    Titlelabel3.Inlines.Clear();
                    Titlelabel3.Inlines.Add("You have marked the payment complete on ");
                    Titlelabel3.Inlines.Add(new Run(CurrecttradeValue.data.payment_completed_at.ToString()) { FontWeight = FontWeights.Bold });
                    Titlelabel3.Inlines.Add(" . The seller ");
                    Titlelabel3.Inlines.Add(new Run(CurrecttradeValue.data.seller.username.ToString()) { FontWeight = FontWeights.Bold });
                    Titlelabel3.Inlines.Add(" will release the bitcoins to you when the seller has confirmed the payment.");
                }));
            }
            else
            {
                TextBlockreleasebitcoins.Dispatcher.BeginInvoke((Action)(() =>
                {
                    TextBlock Titlelabel3 = TextBlockreleasebitcoins;
                    Titlelabel3.Inlines.Clear();
                    Titlelabel3.Inlines.Add("The buyer has marked the payment complete on ");
                    Titlelabel3.Inlines.Add(new Run(CurrecttradeValue.data.payment_completed_at.ToString()) { FontWeight = FontWeights.Bold });
                    Titlelabel3.Inlines.Add(" . The buyer ");
                    Titlelabel3.Inlines.Add(new Run(CurrecttradeValue.data.seller.username.ToString()) { FontWeight = FontWeights.Bold });
                    Titlelabel3.Inlines.Add(" will release the bitcoins to you when the buyer has confirmed the payment.");
                }));
            }
            BoolPaymentsent = true;
            WindowTimer.Stop();
        }

        private void TradeCanceled()
        {
            TextBlockLabelTitle3.Dispatcher.BeginInvoke((Action)(() =>
            {
                TextBlock Titlelabel3 = TextBlockLabelTitle3;
                Titlelabel3.Inlines.Clear();
                Titlelabel3.Inlines.Add("Trade status : ");
                Titlelabel3.Inlines.Add(new Run("Trade canceled by buyer.") { FontWeight = FontWeights.Bold });
            }));
            PanelIdentifyingUser.Dispatcher.BeginInvoke((Action)(() => {PanelIdentifyingUser.Visibility = Visibility.Collapsed;}));
            PanelFeedback.Dispatcher.BeginInvoke((Action)(() => { PanelFeedback.Visibility = Visibility.Collapsed; }));
            PanelBuyingBitcoins.Dispatcher.BeginInvoke((Action)(() => { PanelBuyingBitcoins.Visibility = Visibility.Collapsed; }));
            PanelTradeClosed.Dispatcher.BeginInvoke((Action)(() => { PanelTradeClosed.Visibility = Visibility.Visible; }));
            TextBlockTradeClosedInfo.Dispatcher.BeginInvoke((Action)(() => 
            {
                TextBlock TextBlockTradeInfo = TextBlockTradeClosedInfo;
                TextBlockTradeInfo.Inlines.Clear();
                TextBlockTradeInfo.Inlines.Add("This trade was canceled at ");
                TextBlockTradeInfo.Inlines.Add(new Run(CurrecttradeValue.data.canceled_at.ToString()) { FontWeight = FontWeights.Bold });
                TextBlockTradeInfo.Inlines.Add(" . You can still send messages to the trade user.");
            }));
            BoolPaymentsent = true;
            TradeClosed = true;
            WindowTimer.Stop();
        }

        private void TradeAutomaticallyCanceled()
        {
            TextBlockLabelTitle3.Dispatcher.BeginInvoke((Action)delegate 
            {
                TextBlockLabelTitle3.Inlines.Clear();
                TextBlockLabelTitle3.Inlines.Add("Trade status : ");
                TextBlockLabelTitle3.Inlines.Add(new Run("Payment not marked complete within time window, automatically canceled.") { FontWeight = FontWeights.Bold });
            });

            PanelIdentifyingUser.Dispatcher.BeginInvoke((Action)(() => { PanelIdentifyingUser.Visibility = Visibility.Collapsed; }));
            PanelFeedback.Dispatcher.BeginInvoke((Action)(() => { PanelFeedback.Visibility = Visibility.Collapsed; }));
            PanelBuyingBitcoins.Dispatcher.BeginInvoke((Action)(() => { PanelBuyingBitcoins.Visibility = Visibility.Collapsed; }));
            PanelTradeClosed.Dispatcher.BeginInvoke((Action)(() => { PanelTradeClosed.Visibility = Visibility.Visible; }));
            TextBlockTradeClosedInfo.Dispatcher.BeginInvoke((Action)(() => 
            {
                TextBlock TextBlockTradeInfo = TextBlockTradeClosedInfo;
                TextBlockTradeInfo.Inlines.Clear();
                TextBlockTradeInfo.Inlines.Add("This trade was closed at ");
                TextBlockTradeInfo.Inlines.Add(new Run(CurrecttradeValue.data.closed_at.ToString()) { FontWeight = FontWeights.Bold });
                TextBlockTradeInfo.Inlines.Add(" . You can still send messages to the trade user.");
                TextBlockTradeInfo.Inlines.Add(new LineBreak());
                TextBlockTradeInfo.Inlines.Add(new LineBreak());
                TextBlockTradeInfo.Inlines.Add("Noticed the payment, but buyer had forgot to mark it complete? You can reopen the escrow below.");
            }));
            ButtonReopentrade.Dispatcher.BeginInvoke((Action)(() =>
            {
                ButtonReopentrade.Visibility = Visibility.Visible;
            }));
            BoolPaymentsent = true;
            TradeClosed = true;
            WindowTimer.Stop();
        }

        private void TradeReleased()
        {
            TextBlockLabelTitle3.Dispatcher.BeginInvoke((Action)(() =>
            {
                TextBlock Titlelabel3 = TextBlockLabelTitle3;
                Titlelabel3.Inlines.Clear();
                Titlelabel3.Inlines.Add("Trade status : ");
                Titlelabel3.Inlines.Add(new Run("Bitcoins released, trade finished.") { FontWeight = FontWeights.Bold });
            }));
            PanelIdentifyingUser.Dispatcher.BeginInvoke((Action)(() => { PanelIdentifyingUser.Visibility = Visibility.Collapsed; }));
            PanelFeedback.Dispatcher.BeginInvoke((Action)(() => { PanelFeedback.Visibility = Visibility.Collapsed; }));
            PanelBuyingBitcoins.Dispatcher.BeginInvoke((Action)(() => { PanelBuyingBitcoins.Visibility = Visibility.Collapsed; }));
            PanelTradeClosed.Dispatcher.BeginInvoke((Action)(() => { PanelTradeClosed.Visibility = Visibility.Visible; }));
            TextBlockTradeClosedInfo.Dispatcher.BeginInvoke((Action)(() =>
            {
                TextBlock TextBlockTradeInfo = TextBlockTradeClosedInfo;
                TextBlockTradeInfo.Inlines.Clear();
                TextBlockTradeInfo.Inlines.Add("This trade was closed at ");
                TextBlockTradeInfo.Inlines.Add(new Run(CurrecttradeValue.data.released_at.ToString()) { FontWeight = FontWeights.Bold });
                TextBlockTradeInfo.Inlines.Add(" . You can still send messages to the trade user.");
            }));
            BoolPaymentsent = true;
            TradeClosed = true;
            WindowTimer.Stop();
        }

        async private void ChecktradeStatusTimer_Tick(object sender, EventArgs e)
        {
            ChecktradeStatusTimer.Stop();
            try
            {
                await Task.Run(() => CheckTradeStatus());
            }
            catch (Exception ep)
            {
                MainWindow.ErrorLogging(ep);
            }

            if (TradeClosed == false)
                ChecktradeStatusTimer.Start();
        }

        private void CheckTradeStatus()
        {
            var Alltrades = MainWindow.AllCurrentTrades;
            if (Alltrades != null)
            {
                var CurrenctContactTrade = Alltrades.data.contact_list.Where(x => x.data.contact_id.ToString() == ContactTradeNumber).FirstOrDefault();
                if (CurrenctContactTrade != null)
                {
                    CurrecttradeValue = CurrenctContactTrade;
                    if (CurrenctContactTrade.data.payment_completed_at != null || CurrenctContactTrade.data.canceled_at != null || CurrenctContactTrade.data.closed_at != null || CurrenctContactTrade.data.released_at != null)
                    {
                        if (CurrenctContactTrade.data.payment_completed_at != null && CurrenctContactTrade.data.canceled_at == null && CurrenctContactTrade.data.released_at == null || CurrenctContactTrade.data.closed_at == null)
                        {
                            PaymentCompleted();
                        }
                        else if (CurrenctContactTrade.data.canceled_at != null)
                        {
                            TradeCanceled();
                            MainWindow.TradesContactidsList.Remove(ContactTradeNumber);
                            CloseTabTimer.Interval = new TimeSpan(0, 10, 0);
                            CloseTabTimer.Start();
                        }
                        else if (CurrenctContactTrade.data.closed_at != null && CurrenctContactTrade.data.canceled_at == null && CurrenctContactTrade.data.released_at == null)
                        {
                            TradeAutomaticallyCanceled();
                        }
                        else if (CurrenctContactTrade.data.released_at != null)
                        {
                            TradeReleased();
                            MainWindow.TradesContactidsList.Remove(ContactTradeNumber);
                            CloseTabTimer.Interval = new TimeSpan(0, 10, 0);
                            CloseTabTimer.Start();
                        }
                    }
                }
            }
            if (isfirstloadtimer == true)
                WindowTimer_tick(null, null);
        }

        async private void WindowTimer_tick(object sender, EventArgs e)
        {
            await Task.Run(() => UpdateTextBlockPaymentTimeTask());
        }

        private void UpdateTextBlockPaymentTimeTask ()
        {
            if (isfirstloadtimer == true)
            {
                TimeSpan minutesspan = DateTime.Now.Subtract(InfoContact.data.created_at);
                int minutesdifference = Convert.ToInt32(minutesspan.TotalMinutes);
                if (TradeisSell == true)
                {
                    if (WindowTimer.IsEnabled == true)
                    {

                    }
                    else
                    {
                        WindowTimer.Interval = new TimeSpan(0, 1, 0);
                        totalMinutesTrade = 90 - minutesdifference;
                        WindowTimer.Start();
                        WindowTimer.IsEnabled = true;
                    }
                }
                else
                {
                    if (WindowTimer.IsEnabled == true)
                    {

                    }
                    else
                    {
                        WindowTimer.Interval = new TimeSpan(0, 1, 0);
                        totalMinutesTrade = 60 - minutesdifference;
                        WindowTimer.Start();
                        WindowTimer.IsEnabled = true;
                    }
                }
                isfirstloadtimer = false;
            }
            if (BoolPaymentsent == false)
            {
                if (TradeisSell == true)
                {
                    TextBlockreleasebitcoins.Dispatcher.BeginInvoke((Action)(() =>
                    {
                        TextBlock Timertextblock = TextBlockreleasebitcoins;
                        Timertextblock.Inlines.Clear();
                        Timertextblock.Inlines.Add("The buyer has not yet marked the payment complete and has ");
                        Timertextblock.Inlines.Add(new Run(totalMinutesTrade.ToString()) { FontWeight = FontWeights.Bold });
                        Timertextblock.Inlines.Add(" minutes to make the payment before the trade is automatically canceled.");
                    }));
                    totalMinutesTrade = totalMinutesTrade - 1;
                }
                else
                {
                    TextBlockPaymentWindow.Dispatcher.BeginInvoke((Action)(() =>
                    {
                         TextBlock Timertextblock = TextBlockPaymentWindow;
                         Timertextblock.Inlines.Clear();
                         Timertextblock.Inlines.Add("The bitcoins are held in escrow for ");
                         Timertextblock.Inlines.Add(new Run(totalMinutesTrade.ToString()) { FontWeight = FontWeights.Bold });
                         Timertextblock.Inlines.Add(" minutes, during which it is safe to pay. After payment, ");
                         Timertextblock.Inlines.Add(new Run("you need to mark the payment complete ") { FontWeight = FontWeights.Bold });
                         Timertextblock.Inlines.Add("by pressing the \"I have paid\" button.");
                    }));
                    totalMinutesTrade = totalMinutesTrade - 1;
                }
            }
            else
            {
                WindowTimer.Stop();
            }
        }

        StackPanel Whatpanel = null;

        private void ButtonUpdateFeedbackPanel_Click(object sender, RoutedEventArgs e)
        {
            
            if (PanelIdentifyingUser.Visibility == Visibility.Visible || PanelBuyingBitcoins.Visibility == Visibility.Visible || PanelTradeClosed.Visibility == Visibility.Visible && PanelFeedback.Visibility == Visibility.Collapsed)
            {
                if (PanelIdentifyingUser.Visibility == Visibility.Visible)
                {
                    PanelIdentifyingUser.Visibility = Visibility.Collapsed;
                    PanelFeedback.Visibility = Visibility.Visible;
                    Whatpanel = PanelIdentifyingUser;
                }
                else if (PanelBuyingBitcoins.Visibility == Visibility.Visible)
                {
                    PanelBuyingBitcoins.Visibility = Visibility.Collapsed;
                    PanelFeedback.Visibility = Visibility.Visible;
                    Whatpanel = PanelBuyingBitcoins;
                }
                else if (PanelTradeClosed.Visibility == Visibility.Visible)
                {
                    PanelFeedback.Visibility = Visibility.Visible;
                    Whatpanel = PanelTradeClosed;
                }
            }
            else
            {
                Whatpanel.Visibility = Visibility.Visible;
                PanelFeedback.Visibility = Visibility.Collapsed;
            }
        }

        public void DownloadFile(int FileId)
        {
            TextBlockSpinnerDownloadingfile.Dispatcher.BeginInvoke((Action)(() =>
            {
                TextBlockSpinnerDownloadingfile.Text = "Downloading File";
                TextBlockSpinnerDownloadingfile.Foreground = Brushes.Black;
            }));
            if (isDownloadingFile == true)
            {
                TextBlockWarnDownloadingFile.Dispatcher.BeginInvoke((Action)(() =>
                {
                    TextBlockWarnDownloadingFile.Visibility = Visibility.Visible;
                }));
            }
            else
            {
                isDownloadingFile = true;
                StackPanelDownloadingfile.Dispatcher.BeginInvoke((Action)(() =>
                {
                    StackPanelDownloadingfile.Visibility = Visibility.Visible;
                }));
                var APiKey = ConfigurationManager.AppSettings["ApiKeyNewTrades"];
                var ApiSecret = ConfigurationManager.AppSettings["ApiSecretNewTrades"];
                var client = new LocalBitcoinsAPI(APiKey, ApiSecret);
                var file = ListMessages.FirstOrDefault(x => x.FileID == FileId);
                var FileNumber = file.FileID;
                var FileName = file.FileName;
                var FilePath = ($"{MainWindow.directoryPath}{ContactTradeNumber}\\{FileNumber}_{FileName}");

                if (File.Exists(FilePath))
                {
                    Process.Start(FilePath);
                }
                else
                {
                    byte[] fileDownloaded = null;
                    try
                    {
                        fileDownloaded = client.GetContactMessageAttachment(ContactTradeNumber, FileNumber.ToString());
                        File.WriteAllBytes(FilePath, fileDownloaded);
                        Process.Start(FilePath);
                    }
                    catch
                    {
                        TextBlockSpinnerDownloadingfile.Dispatcher.BeginInvoke((Action)(() =>
                        {
                            TextBlockSpinnerDownloadingfile.Text = "File not Downloaded!";
                            TextBlockSpinnerDownloadingfile.Foreground = Brushes.Red;
                        }));
                    }
                }
                StackPanelDownloadingfile.Dispatcher.BeginInvoke((Action)(() =>
                {
                    StackPanelDownloadingfile.Visibility = Visibility.Collapsed;
                }));
                isDownloadingFile = false;
                TextBlockWarnDownloadingFile.Dispatcher.BeginInvoke((Action)(() =>
                {
                    TextBlockWarnDownloadingFile.Visibility = Visibility.Collapsed;
                }));
            }
            
        }

        bool isDownloadingFile = false;

        [ComVisible(true)]
        public class ScriptManager{
            private UserControltest1 myForm;
            public ScriptManager(UserControltest1 f)
            {
                myForm = f;
            }
            async public void MethodToCallFromScript(int FileID)
            {
                // Call a method on the form.
                try
                {
                    await Task.Run(() => myForm.DownloadFile(FileID));
                }
                catch (Exception ep)
                {
                    MainWindow.ErrorLogging(ep);
                }
            }
        }

        async private void ButtonSendMessage_Click(object sender, RoutedEventArgs e)
        {
            ButtonChooseFile.IsEnabled = false;
            SpinnerButtonSendMessage.Visibility = Visibility.Visible;
            var message = TextBoxSendMessage.Text;
            if (!string.IsNullOrWhiteSpace(message) || !string.IsNullOrWhiteSpace(AttachmentFilepath))
            {
                if (!string.IsNullOrWhiteSpace(message) && string.IsNullOrWhiteSpace(AttachmentFilepath))
                    await Task.Run(() => SendMessagetoCostumer(message, null, false));
                else if (string.IsNullOrWhiteSpace(message) && !string.IsNullOrWhiteSpace(AttachmentFilepath))
                    await Task.Run(() => SendMessagetoCostumer(null, AttachmentFilepath, true));
                else if (!string.IsNullOrWhiteSpace(message) && !string.IsNullOrWhiteSpace(AttachmentFilepath))
                    await Task.Run(() => SendMessagetoCostumer(message, AttachmentFilepath, true));
                MainWindow.newMessageUpdate = true;
            }
            else
            {
                System.Windows.MessageBox.Show("Enter a message or choose a file!");
            }
            TextBoxSendMessage.Clear();
            ButtonChooseFile.IsEnabled = true;
            SpinnerButtonSendMessage.Visibility = Visibility.Collapsed;
        }

        private void SendMessagetoCostumer(string message, string AttachmentFile, bool FileExists)
        {
            var APiKey = ConfigurationManager.AppSettings["ApiKeyMessages"];
            var ApiSecret = ConfigurationManager.AppSettings["ApiSecretMessages"];
            var client = new LocalBitcoinsAPI(APiKey, ApiSecret);
            string file = AttachmentFilepath;
            try
            { 
                if (FileExists == true)
                {
                    client.PostMessageToContact(ContactTradeNumber, message, file);
                    AttachmentFilepath = string.Empty;
                }
                else
                {
                    client.PostMessageToContact(ContactTradeNumber, message, null);
                }
                ButtonChooseFileIconNotification.Dispatcher.BeginInvoke((Action)(() => 
                {
                ButtonChooseFileIconNotification.Visibility = Visibility.Collapsed;
                }));
                ViewboxButtonSendMessage.Dispatcher.BeginInvoke((Action)(() =>
                {
                    ViewboxButtonSendMessage.Visibility = Visibility.Collapsed;
                }));
            }
            catch (Exception ep)
            {
                MainWindow.ErrorLogging(ep);
                ViewboxButtonSendMessage.Dispatcher.BeginInvoke((Action)(() =>
                {
                    ViewboxButtonSendMessage.Visibility = Visibility.Visible;
                }));
            }
        }

        private void ButtonChooseFile_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(AttachmentFilepath))
            {
                OpenFileDialog fdlg = new OpenFileDialog();
                fdlg.Title = "Localbitcoins AttachFile";
                fdlg.InitialDirectory = @"F:\Downloads";
                fdlg.Filter = "Image files (*.jpg,*.jpeg,*.jpe,*.jfif,*.png) |*.jpg;*.jpeg;*.jpe;*.jfif;*.png";
                fdlg.FilterIndex = 2;
                fdlg.RestoreDirectory = true;
                if (fdlg.ShowDialog() == DialogResult.OK)
                {
                    AttachmentFilepath = fdlg.FileName;
                    ButtonChooseFileIconNotification.Visibility = Visibility.Visible;
                }
            }
            else
            {
                AttachmentFilepath = string.Empty;
                ButtonChooseFileIconNotification.Visibility = Visibility.Collapsed;
            }
        }

        public string UserLimitExceed()
        {
            StringBuilder totalAmountString = new StringBuilder();
            if (userweeklimitbeforetrade <= 0 )
            {
                totalAmountString.AppendLine($"Hello Mr/Ms. {UsernameTrade},");
                totalAmountString.AppendLine("");
                totalAmountString.AppendLine("Thank you for choosing by services, however since you have reached your week limit this current week, ");
                totalAmountString.AppendLine("therefore you can not buy more bitcoins from me until the next reset week limit.");
                totalAmountString.AppendLine();
                totalAmountString.AppendLine($"If possible please cancel the trade. I will apreciate it.");
                totalAmountString.AppendLine();
                totalAmountString.AppendLine($"Your current limit: {userweeklimitbeforetrade.ToString()}");
                totalAmountString.AppendLine($"Your week limit will be reseted : {ResetDay()}");
            }
            else if (userweeklimitbeforetrade > 0)
            {
                totalAmountString.AppendLine($"Hello Mr/Ms. {UsernameTrade},");
                totalAmountString.AppendLine("");
                totalAmountString.AppendLine("Thank you for choosing by services, however since you have a low amount of completed trades or you already bought some bitcoins this week from me, ");
                totalAmountString.AppendLine("therefore you can not buy the full amount set in this trade.");
                totalAmountString.AppendLine();
                totalAmountString.AppendLine($"Your limit for this week is {userweeklimitbeforetrade.ToString()} {InfoContact.data.currency.ToString()}, so if possible cancel the trade and if you");
                totalAmountString.AppendLine($" interested reopen it within the limit, which is {userweeklimitbeforetrade.ToString()} {InfoContact.data.currency.ToString()}.");
                totalAmountString.AppendLine();
                totalAmountString.AppendLine($"Your week limit in total: {userTotalweekLimit.ToString()}");
                totalAmountString.AppendLine($"Your current limit: {userweeklimitbeforetrade.ToString()}");
            }
            return totalAmountString.ToString();
        }

        public string UserDifferentIp()
        {
            var countryIPCode = InfoContact.data.buyer.countrycode_by_ip;
            var countryPhoneCode = InfoContact.data.buyer.countrycode_by_phone_number;

            StringBuilder dataString = new StringBuilder();
            dataString.AppendLine("Hello, I noticed that you have a different ip adress than your phone number country, since its the first trade with me, I need to know the reason why this is happening");
            dataString.AppendLine();
            dataString.AppendLine($"Your country ip : {new Countries().GetCountryName(countryIPCode).ToString()}");
            dataString.AppendLine();
            dataString.AppendLine($"Your country Phone number : {new Countries().GetCountryName(countryPhoneCode).ToString()}");

            return dataString.ToString();
        }

        public string ScreenshotTransaction()
        {
            var RealName = InfoContact.data.buyer.real_name;
            StringBuilder screenshotString = new StringBuilder();
            screenshotString.AppendLine($"Mr/Ms. {RealName}, the last screenshot did not work for my purpose.");
            screenshotString.AppendLine();
            screenshotString.AppendLine("Please send me a screenshot of the transaction details by clicking on the transaction that is on the recent activity (Example ➥ http://prntscr.com/iqqihe) ");
            screenshotString.AppendLine("or send me the receipt you got after payment in your email to my email (Example ➥ http://prntscr.com/jezw7k)");
            screenshotString.AppendLine();
            screenshotString.AppendLine("My email is : mr.niceonne@gmail.com");

            return screenshotString.ToString();
        }

        public string PaypalTransactionsPending()
        {
            var RealName = InfoContact.data.buyer.real_name;
            DateTime paypalHours = DateTime.Now + (new TimeSpan(24, 0, 0));
            StringBuilder pendingString = new StringBuilder();
            pendingString.AppendLine($"Mr/Ms. {RealName} the payment is in hold for 24 hours due to paypal security, so it will be reviewed by PayPal.");
            pendingString.AppendLine("The Payment Review may take up to 24 hours. So if you wish I can refund the payment otherwise, I will have to wait until it gets cleared.");
            pendingString.AppendLine("");
            pendingString.AppendLine("In case you want to wait, then when PayPal have completed the review, I will send you a message with the review veredict.");
            pendingString.AppendLine();
            pendingString.AppendLine($"I will get the veredict until {paypalHours.ToString()}.");

            return pendingString.ToString();
        }

        public string CreditCardNotAllowed()
        {
            var RealName = InfoContact.data.buyer.real_name;
            StringBuilder myString = new StringBuilder();
            myString.AppendLine($"Mr/Ms.{RealName}, ");
            myString.AppendLine();
            myString.Append("I am afraid I can not accept credit/debit card or either bank from you because you do not own atleast 30 completed trades in your profile.");
            myString.AppendLine("If you cant pay using paypal balance, I will ask you to cancel the trade, I apreciate it");
            myString.AppendLine();
            myString.Append("Tip: Increase the number of trades that you have in your profile and soon you have 30 completed trades I will be able to accept credit/debit card ");
            myString.Append("through paypal from you.");

            return myString.ToString();
        }

        async private void ListBoxMessages_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            SpinnerButtonListboxMessages.Visibility = Visibility.Visible;
            ButtonMessagesListBox.IsEnabled = false;
            ButtonSendMessage.IsEnabled = false;
            ListBoxMessages.Visibility = Visibility.Collapsed;
            var indexMessages1 = ListBoxMessages.SelectedIndex;
            try
            {
                await Task.Run(() => sendOptionalMessages(indexMessages1));
                MainWindow.newMessageUpdate = true;
            }
            catch (Exception ep)
            {
                MainWindow.ErrorLogging(ep);
            }
            SpinnerButtonListboxMessages.Visibility = Visibility.Collapsed;
            ButtonMessagesListBox.IsEnabled = true;
            ButtonSendMessage.IsEnabled = true;
        }

        private void sendOptionalMessages(int MessageIndex)
        {
            var APiKey = ConfigurationManager.AppSettings["ApiKeyMessages"];
            var ApiSecret = ConfigurationManager.AppSettings["ApiSecretMessages"];
            var client = new LocalBitcoinsAPI(APiKey, ApiSecret);
            var indexMessages = MessageIndex;
            if (indexMessages == 0)
            {
                client.PostMessageToContact(ContactTradeNumber, UserLimitExceed(), null);
            }
            else if (indexMessages == 1)
            {

                client.PostMessageToContact(ContactTradeNumber, UserDifferentIp(), null);
            }
            else if (indexMessages == 2)
            {
                client.PostMessageToContact(ContactTradeNumber, ScreenshotTransaction(), null);
            }
            else if (indexMessages == 3)
            {
                client.PostMessageToContact(ContactTradeNumber, PaypalTransactionsPending(), null);
            }
            else if (indexMessages == 4)
            {
                client.PostMessageToContact(ContactTradeNumber, CreditCardNotAllowed(), null);
            }
        }

        private void ButtonMessagesListBox_Click(object sender, RoutedEventArgs e)
        {
            if (ListBoxMessages.Visibility == Visibility.Visible)
            {
                ListBoxMessages.Visibility = Visibility.Collapsed;
            }
            else if (ListBoxMessages.Visibility == Visibility.Collapsed)
            {
                if (ListBoxSettings.Visibility == Visibility.Visible)
                {
                    ListBoxSettings.Visibility = Visibility.Collapsed;
                    ListBoxMessages.Visibility = Visibility.Visible;
                }
                else
                {
                    ListBoxMessages.Visibility = Visibility.Visible;
                }
            }
        }

        private void ButtonSettingsListbox_Click(object sender, RoutedEventArgs e)
        {
            if (ListBoxSettings.Visibility == Visibility.Visible)
            {
                ListBoxSettings.Visibility = Visibility.Collapsed;
            }
            else if (ListBoxSettings.Visibility == Visibility.Collapsed)
            {
                if (ListBoxMessages.Visibility == Visibility.Visible)
                {
                    ListBoxMessages.Visibility = Visibility.Collapsed;
                    ListBoxSettings.Visibility = Visibility.Visible;
                }
                else
                {
                    ListBoxSettings.Visibility = Visibility.Visible;
                }
            }
        }

        async private void ButtonSendReturnMessage_Click(object sender, RoutedEventArgs e)
        {
            SpinnerButtonSendReturnMessage.Visibility = Visibility.Visible;
            try
            {
                await Task.Run(() => SendReturnMessage());
                MainWindow.newMessageUpdate = true;
                WarnButtonSendReturnMessage.Visibility = Visibility.Collapsed;
            }
            catch (Exception ep)
            {
                MainWindow.ErrorLogging(ep);
                WarnButtonSendReturnMessage.Visibility = Visibility.Visible;
            }
            SpinnerButtonSendReturnMessage.Visibility = Visibility.Collapsed;
        }

        private void SendReturnMessage ()
        {
            var realName = InfoContact.data.buyer.real_name;
            var APiKey = ConfigurationManager.AppSettings["ApiKeyMessages"];
            var ApiSecret = ConfigurationManager.AppSettings["ApiSecretMessages"];
            var client = new LocalBitcoinsAPI(APiKey, ApiSecret);

            StringBuilder returnString = new StringBuilder();
            returnString.Append($"Hello, {UsernameTrade},");
            returnString.AppendLine();
            returnString.AppendLine("You were already verified by my identity verification process, therefore you are able to buy without any requested photos.");
            returnString.AppendLine("Let me remind you that I can only sell you until you reach your week Limit.");
            returnString.AppendLine();
            returnString.AppendLine($"✔ Your Total week limit : {userTotalweekLimit} {InfoContact.data.currency}.");
            returnString.AppendLine($"✔ Your current week limit : {userweeklimitbeforetrade} {InfoContact.data.currency}.");
            returnString.AppendLine($"✔ Reset day of the Week Limit : {ResetDay()}");
            returnString.AppendLine();
            returnString.AppendLine($"✔ Paypal Email verified by me : {Emailpaypal}");

            client.PostMessageToContact(ContactTradeNumber, returnString.ToString(), null);
        }

        async private void ButtonSendTotal_Click(object sender, RoutedEventArgs e)
        {
            SpinnerButtonSendTotal.Visibility = Visibility.Visible;
            SpinnerButtonSendTotal2.Visibility = Visibility.Visible;
            try
            {
                await Task.Run(() => SendTotalMessage());
                MainWindow.newMessageUpdate = true;
                WarnButtonSendTotal.Visibility = Visibility.Collapsed;
                WarnButtonSendTotal2.Visibility = Visibility.Collapsed;
            }
            catch (Exception ep)
            {
                MainWindow.ErrorLogging(ep);
                WarnButtonSendTotal.Visibility = Visibility.Visible;
                WarnButtonSendTotal2.Visibility = Visibility.Visible;
        }
            SpinnerButtonSendTotal.Visibility = Visibility.Collapsed;
            SpinnerButtonSendTotal2.Visibility = Visibility.Collapsed;
        }

        private void SendTotalMessage()
        {
            var APiKey = ConfigurationManager.AppSettings["ApiKeyMessages"];
            var ApiSecret = ConfigurationManager.AppSettings["ApiSecretMessages"];
            var client = new LocalBitcoinsAPI(APiKey, ApiSecret);

            var TotalAmount = InfoContact.data.amount;
            var currency = InfoContact.data.currency;

            decimal result = 0;
            result = (Convert.ToDecimal(TotalAmount));
            {
                result += (Convert.ToDecimal(TotalAmount) * (decimal)0.0585);
            }

            StringBuilder myString = new StringBuilder();
            myString.AppendLine("I will send you a invoice to your paypal account.");
            myString.AppendLine();
            myString.AppendLine($"✔ Total Amount: {result.ToString("#.##")} {currency}.");
            myString.AppendLine("");
            myString.Append("➤ Note: The Total amount above is already set with 5.85 % of fee because, since you will be");
            myString.AppendLine(" sending the funds as services and goods I will be the one paying for the fees on paypal.");
            myString.AppendLine("The country of my paypal account is ✠ Portugal ✠.");

             client.PostMessageToContact(ContactTradeNumber, myString.ToString(), null);
        }

        async private void ButtonPaypalInvoice_Click(object sender, RoutedEventArgs e)
        {
            SpinnerButtonSendPaypalInvoice.Visibility = Visibility.Visible;
            SpinnerButtonPaypalInvoice.Visibility = Visibility.Visible;
            if (string.IsNullOrWhiteSpace(Emailpaypal))
            {
                ViewboxEmailemptyWarning.Visibility = Visibility.Visible;
            }
            else
            {
                try
                {
                    await Task.Run(() => sendInvoicePaypal());
                    MainWindow.newMessageUpdate = true;
                    ButtonCancelInvoicePaypal.Visibility = Visibility.Visible;
                    ButtonCancelInvoicePaypal1.Visibility = Visibility.Visible;
                }
                catch (Exception ep)
                {
                    MainWindow.ErrorLogging(ep);
                }
            }
            SpinnerButtonSendPaypalInvoice.Visibility = Visibility.Collapsed;
            SpinnerButtonPaypalInvoice.Visibility = Visibility.Collapsed;

        }

        bool IsInvoiceSent = false;

        private void sendInvoicePaypal()
        {
            var APiKey = ConfigurationManager.AppSettings["ApiKeyMessages"];
            var ApiSecret = ConfigurationManager.AppSettings["ApiSecretMessages"];
            var client = new LocalBitcoinsAPI(APiKey, ApiSecret);

            StringBuilder myString = new StringBuilder();
            myString.AppendLine($"Invoice sucessfully sent to : '{PaypalEmail}' , please proceed with the payment.");
            myString.AppendLine();
            myString.Append("✔ After the payment please send me a screenshot of the transaction details by clicking on the transaction that is on the recent activity(Example ➥ http://prntscr.com/iqqihe) ");
            myString.AppendLine("or send me the receipt, you got after payment to my email (Example ➥ http://prntscr.com/jezw7k) - mr.niceonne@gmail.com.");

            var TotalAmount = InfoContact.data.amount;
            var currency = InfoContact.data.currency;

            if (IsInvoiceSent == false)
            {
                try
                {
                    var result = new PayPalApi().SendInvoice(PaypalEmail, ContactTradeNumber, TotalAmount, currency);
                    IsInvoiceSent = true;
                    InvoiceIdPaypal = result.id;
                    try
                    {
                        client.PostMessageToContact(ContactTradeNumber, myString.ToString(), null);
                        WarnButtonPaypalInvoice.Dispatcher.BeginInvoke((Action)(() =>
                        {
                            WarnButtonPaypalInvoice.Visibility = Visibility.Collapsed;
                        }));
                        WarnButtonSendPaypalInvoice.Dispatcher.BeginInvoke((Action)(() =>
                        {
                            WarnButtonSendPaypalInvoice.Visibility = Visibility.Collapsed;
                        }));
                    }
                    catch (Exception ep)
                    {
                        MainWindow.ErrorLogging(ep);
                        WarnButtonPaypalInvoice.Dispatcher.BeginInvoke((Action)(() =>
                        {
                            WarnButtonPaypalInvoice.Visibility = Visibility.Visible;
                        }));
                        WarnButtonSendPaypalInvoice.Dispatcher.BeginInvoke((Action)(() =>
                        {
                            WarnButtonSendPaypalInvoice.Visibility = Visibility.Visible;
                        }));
                    }
                }
                catch (Exception ep)
                {
                    MainWindow.ErrorLogging(ep);
                    WarnButtonPaypalInvoice.Dispatcher.BeginInvoke((Action)(() =>
                    {
                        WarnButtonPaypalInvoice.Visibility = Visibility.Visible;
                    }));
                    WarnButtonSendPaypalInvoice.Dispatcher.BeginInvoke((Action)(() =>
                    {
                        WarnButtonSendPaypalInvoice.Visibility = Visibility.Visible;
                    }));
                }
            }
            else
            {
                try
                {
                    client.PostMessageToContact(ContactTradeNumber, myString.ToString(), null);
                    WarnButtonPaypalInvoice.Dispatcher.BeginInvoke((Action)(() =>
                    {
                        WarnButtonPaypalInvoice.Visibility = Visibility.Collapsed;
                    }));
                    WarnButtonSendPaypalInvoice.Dispatcher.BeginInvoke((Action)(() =>
                    {
                        WarnButtonSendPaypalInvoice.Visibility = Visibility.Collapsed;
                    }));
                }
                catch (Exception ep)
                {
                    MainWindow.ErrorLogging(ep);
                    WarnButtonPaypalInvoice.Dispatcher.BeginInvoke((Action)(() =>
                    {
                        WarnButtonPaypalInvoice.Visibility = Visibility.Visible;
                    }));
                    WarnButtonSendPaypalInvoice.Dispatcher.BeginInvoke((Action)(() =>
                    {
                        WarnButtonSendPaypalInvoice.Visibility = Visibility.Visible;
                    }));
                }
            }
    
           
        }

        async private void ButtonCancelInvoicePaypal_Click(object sender, RoutedEventArgs e)
        {
            SpinnerButtonCancelinvoice.Visibility = Visibility.Visible;
            SpinnerButtonCancelInvoice2.Visibility = Visibility.Visible;
            try
            {
                await Task.Run(() => CancelTheInvoice());
                ButtonCancelInvoicePaypal.Visibility = Visibility.Hidden;
                ButtonCancelInvoicePaypal1.Visibility = Visibility.Hidden;
                WarnButtonCancelinvoice.Visibility = Visibility.Collapsed;
                WarnButtonCancelInvoice2.Visibility = Visibility.Collapsed;
                IsInvoiceSent = false;
            }
            catch (Exception ep)
            {
                MainWindow.ErrorLogging(ep);
                WarnButtonCancelinvoice.Visibility = Visibility.Visible;
                WarnButtonCancelInvoice2.Visibility = Visibility.Visible;
                WarnButtonCancelinvoice.Visibility = Visibility.Visible;
                WarnButtonCancelInvoice2.Visibility = Visibility.Visible;
            }
            SpinnerButtonCancelinvoice.Visibility = Visibility.Collapsed;
            SpinnerButtonCancelInvoice2.Visibility = Visibility.Collapsed;
        }

        private void CancelTheInvoice()
        {
            var invoiceID = InvoiceIdPaypal;
            new PayPalApi().CancelInvoice(invoiceID);
        }

        async private void ButtonSendCreditcardVerification_Click(object sender, RoutedEventArgs e)
        {
            SpinnerButtonCreditCardVerification.Visibility = Visibility.Visible;
            SpinnerButtonCreditcardVerification2.Visibility = Visibility.Visible;
            try
            {
                await Task.Run(() => CreditCardVerification());
                MainWindow.newMessageUpdate = true;
                WarnButtonCreditCardVerification.Visibility = Visibility.Collapsed;
                WarnButtonCreditcardVerification2.Visibility = Visibility.Collapsed;
        }
            catch (Exception ep)
            {
                MainWindow.ErrorLogging(ep);
                WarnButtonCreditCardVerification.Visibility = Visibility.Visible;
                WarnButtonCreditcardVerification2.Visibility = Visibility.Visible;
        }
            SpinnerButtonCreditCardVerification.Visibility = Visibility.Collapsed;
            SpinnerButtonCreditcardVerification2.Visibility = Visibility.Collapsed;
        }

        private void CreditCardVerification()
        {
            var APiKey = ConfigurationManager.AppSettings["ApiKeyMessages"];
            var ApiSecret = ConfigurationManager.AppSettings["ApiSecretMessages"];
            var client = new LocalBitcoinsAPI(APiKey, ApiSecret);

            StringBuilder myString = new StringBuilder();
            myString.AppendLine("Credit/Debit Card verification");
            myString.AppendLine();
            myString.AppendLine("➊ Please send me a screenshot of your Paypal front page where I can see the last 4 digits of your credit or debit card.");
            myString.AppendLine("➋ After that send me a photo (Example ➥ http://prntscr.com/igjsf7) of your credit card where I can see the name and the card number (the rest can be hided).");

            client.PostMessageToContact(ContactTradeNumber, myString.ToString(), null);
        }

        async private void ButtonThankyouMessage_Click(object sender, RoutedEventArgs e)
        {
            SpinnerButtonBuyingThankmessage1.Visibility = Visibility.Visible;
            SpinnerButtonThankyouMessage.Visibility = Visibility.Visible;
            SpinnerButtonThankyouMessage2.Visibility = Visibility.Visible;
            try
            {
                await Task.Run(() => Thankyoumessage());
                WarnButtonBuyingThankmessage.Visibility = Visibility.Collapsed;
                WarnButtonThankyouMessage.Visibility = Visibility.Collapsed;
                WarnButtonSendTotal2.Visibility = Visibility.Collapsed;
                MainWindow.newMessageUpdate = true;
            }
            catch (Exception ep)
            {
                MainWindow.ErrorLogging(ep);
                WarnButtonBuyingThankmessage.Visibility = Visibility.Visible;
                WarnButtonThankyouMessage.Visibility = Visibility.Visible;
                WarnButtonSendTotal2.Visibility = Visibility.Visible;
        }
            SpinnerButtonBuyingThankmessage1.Visibility = Visibility.Collapsed;
            SpinnerButtonThankyouMessage.Visibility = Visibility.Collapsed;
            SpinnerButtonThankyouMessage2.Visibility = Visibility.Collapsed;
        }

        private void Thankyoumessage()
        {
            var APiKey = ConfigurationManager.AppSettings["ApiKeyMessages"];
            var ApiSecret = ConfigurationManager.AppSettings["ApiSecretMessages"];
            var client = new LocalBitcoinsAPI(APiKey, ApiSecret);
            if (DatabaseInfo != null)
            {
                int userTradesAny = 0;
                using (var databaseInfo = new LocalBitcoinsDatabaseEntities())
                {
                    var userkey = DatabaseInfo.UsernameKey;
                    userTradesAny = databaseInfo.UsernameTrades.Where(x => x.UsernameKey == userkey).Count();
                }
                if (TypetradeNumber == 1 || TypetradeNumber == 2)
                {
                    if (userTradesAny <= 1)
                    {
                        StringBuilder mystring = new StringBuilder();
                        mystring.AppendLine($"Mr/Ms. {UsernameTrade} thank you for choosing my services and I hope to see you again in the future.");
                        mystring.AppendLine();
                        mystring.AppendLine("Please leave feedback, I will also do it :D");
                        mystring.AppendLine();
                        mystring.AppendLine($"Your week limit is now : {userweeklimitaftertrade} ");
                        mystring.AppendLine($"Week limit reset day : {ResetDay()} ");
                        mystring.AppendLine();
                        mystring.AppendLine("Niceonne BTC");
                        client.PostMessageToContact(ContactTradeNumber, mystring.ToString(), null);
                    } 
                    else if (userTradesAny > 1)
                    {
                        StringBuilder mystring = new StringBuilder();
                        mystring.AppendLine($"Mr/Ms. {UsernameTrade} thank you for choosing my services and I hope to see you again in the future.");
                        mystring.AppendLine();
                        mystring.AppendLine($"Your week limit is now : {userweeklimitaftertrade} ");
                        mystring.AppendLine($"Week limit reset day : {ResetDay()} ");
                        mystring.AppendLine();
                        mystring.AppendLine("Niceonne BTC");
                        client.PostMessageToContact(ContactTradeNumber, mystring.ToString(), null);
                    }
                }
                else
                {
                    if (userTradesAny <= 1)
                    {
                        StringBuilder mystring = new StringBuilder();
                        mystring.AppendLine($"Mr/Ms. {UsernameTrade} thank you for choosing my services and I hope to see you again in the future.");
                        mystring.AppendLine();
                        mystring.AppendLine("Please leave feedback, I will also do it :D");
                        mystring.AppendLine();
                        mystring.AppendLine("Niceonne BTC");
                        client.PostMessageToContact(ContactTradeNumber, mystring.ToString(), null);
                    }
                    else if (userTradesAny <= 1)
                    {
                        StringBuilder mystring = new StringBuilder();
                        mystring.AppendLine($"Mr/Ms. {UsernameTrade} thank you for choosing my services and I hope to see you again in the future.");
                        mystring.AppendLine();
                        mystring.AppendLine("Niceonne BTC");
                        client.PostMessageToContact(ContactTradeNumber, mystring.ToString(), null);
                    }
                }
            }
            else
            {
                StringBuilder mystring = new StringBuilder();
                mystring.AppendLine($"Mr/Ms. {UsernameTrade} thank you for choosing my services and I hope to see you again in the future.");
                mystring.AppendLine();
                mystring.AppendLine("Please leave feedback, I will also do it :D");
                mystring.AppendLine();
                mystring.AppendLine("Niceonne BTC");
                client.PostMessageToContact(ContactTradeNumber, mystring.ToString(), null);
            }
        }

        async private void ButtonRequestEmail_Click(object sender, RoutedEventArgs e)
        {
            SpinnerButtonRequestEmail.Visibility = Visibility.Visible;
            try
            {
                await Task.Run(() => RequestEmail());
                MainWindow.newMessageUpdate = true;
                WarnButtonRequestEmail.Visibility = Visibility.Collapsed;
            }
            catch (Exception ep)
            {
                MainWindow.ErrorLogging(ep);
                WarnButtonRequestEmail.Visibility = Visibility.Visible;
            }
            SpinnerButtonRequestEmail.Visibility = Visibility.Collapsed;
        }

        private void RequestEmail()
        {
            var APiKey = ConfigurationManager.AppSettings["ApiKeyMessages"];
            var ApiSecret = ConfigurationManager.AppSettings["ApiSecretMessages"];
            var client = new LocalBitcoinsAPI(APiKey, ApiSecret);

            StringBuilder myString = new StringBuilder();
            if (creditCardAccepted == true)
            {
                myString.AppendLine($"Hello, {UsernameTrade}, ");
                myString.AppendLine("");
                myString.Append("➧You are a new costumer, therefore I will have to verify your identity, ");
                myString.AppendLine("so please send me your email adress that is associated with your paypal account.");
                myString.AppendLine();
                myString.Append("Let me remind you that you can only use 1 Paypal account with me, so I can only accept funds from the paypal that I will verify today and");
                myString.AppendLine("take in consideration that you have limits, so you can buy until a determinated limit set by me!");
                myString.AppendLine();
                myString.AppendLine("I will send you the instructions to your email that you provided to proceed with the verification process.");
                myString.AppendLine();
                myString.AppendLine("Please understand that this is for my safety and and if you agree with all we will do a great business :D");
                myString.AppendLine();
                myString.AppendLine("Things I can not accept from you : ");
                myString.AppendLine("➀ Expired documents");
                myString.AppendLine("➁ Payments by eCheck.");
            }
            else
            {
                myString.AppendLine($"Hello, {UsernameTrade}, ");
                myString.AppendLine("");
                myString.Append("➧You are a new costumer, therefore I will have to verify your identity, ");
                myString.AppendLine("so please send me your email adress that is associated with your paypal account.");
                myString.AppendLine();
                myString.Append("Let me remind you that you can only use 1 Paypal account with me, so I can only accept funds from the paypal that I will verify today and");
                myString.AppendLine("take in consideration that you have limits, so you can buy until a determinated limit set by me!");
                myString.AppendLine();
                myString.AppendLine("I will send you the instructions to your email that you provided to proceed with the verification process.");
                myString.AppendLine();
                myString.AppendLine("Please understand that this is for my safety and and if you agree with all we will do a great business :D");
                myString.AppendLine();
                myString.AppendLine("Things I can not accept from you : ");
                myString.AppendLine("➀ Payments with Credit/Debit card through paypal");
                myString.AppendLine("➁ Expired documents");
                myString.AppendLine("➂ Payments by eCheck");
            }
            client.PostMessageToContact(ContactTradeNumber, myString.ToString(), null);
        }

        async private void ButtonSendEmailMessage_Click(object sender, RoutedEventArgs e)
        {
            SpinnerButtonverificationEmail.Visibility = Visibility.Visible;
            try
            {
                await Task.Run(() => SendEmail());
                MainWindow.newMessageUpdate = true;
            }
            catch (Exception ep)
            {
                MainWindow.ErrorLogging(ep);
            }
            SpinnerButtonverificationEmail.Visibility = Visibility.Collapsed;
        }

        private void SendEmail()
        {
            var APiKey = ConfigurationManager.AppSettings["ApiKeyMessages"];
            var ApiSecret = ConfigurationManager.AppSettings["ApiSecretMessages"];
            var client = new LocalBitcoinsAPI(APiKey, ApiSecret);

            StringBuilder myString = new StringBuilder();
            myString.AppendLine($"Email with the instruction sucessfully sent to \"{PaypalEmail}\".");
            myString.AppendLine("");
            myString.AppendLine("Please answer with the requested photos by repplying the email I sent you.");
            try
            {
                EmailVerificationRequest();
                try
                {
                    client.PostMessageToContact(ContactTradeNumber, myString.ToString(), null);
                    WarnButtonverificationEmail.Dispatcher.BeginInvoke((Action)(() =>
                    {
                        WarnButtonverificationEmail.Visibility = Visibility.Collapsed;
                    }));
                }
                catch (Exception ep)
                {
                    MainWindow.ErrorLogging(ep);
                    WarnButtonverificationEmail.Dispatcher.BeginInvoke((Action)(() =>
                    {
                        WarnButtonverificationEmail.Visibility = Visibility.Visible;
                    }));
                }
            }
            catch (Exception ep)
            {
                MainWindow.ErrorLogging(ep);
                WarnButtonverificationEmail.Dispatcher.BeginInvoke((Action)(() =>
                {
                    WarnButtonverificationEmail.Visibility = Visibility.Visible;
                }));
            }
        }

        private void EmailVerificationRequest()
        {
            StringBuilder myString = new StringBuilder();
            if (creditCardAccepted == false)
            {
                myString.AppendLine($"Mr/Ms. {UsernameTrade} PLEASE READ ALL INSTRUCTION CLEARLY, OTHERWISE I CANT TRADE WITH YOU !!");
                myString.AppendLine();
                myString.AppendLine("➊ I need a screenshot of your settings page ( ➥ https://www.paypal.com/myaccount/settings/) where I can see name and adress like its on the next example (Example ➥ http://prntscr.com/iq70an).");
                myString.AppendLine("➋ I need a clear photo containing only your id (Example ➥ http://prntscr.com/ifnklz) matching the paypal name.");
                myString.Append("➌ I need a clear selfie, aswell, (Example ➥ http://prntscr.com/ifnkq6) holding the same id ");
                myString.AppendLine(string.Format("with a paper saying the date of today \"{0}/{1}/{2}\" and the contact number \"{3}\" ", DateTime.Now.Day.ToString(), DateTime.Now.Month.ToString(), DateTime.Now.Year.ToString(), ContactTradeNumber.ToString()));
                myString.AppendLine();
                myString.AppendLine("This identity verification is only made once, so I just need to verify you in the first trade.");
                myString.AppendLine("If you follow my instructions this will be done under 15 min, so please try to patient.");
                myString.AppendLine("");
                myString.AppendLine("Things I can not accept from you : ");
                myString.AppendLine("➀ Payments with Credit/Debit card through paypal");
                myString.AppendLine("➁ Expired documents");
                myString.AppendLine("➂ Payments by eCheck");
                myString.AppendLine("");
                myString.AppendLine("Thank you,");
                myString.AppendLine("niceonne BTC");
                myString.AppendLine("");
                myString.AppendLine("Niceonne LocalBitcoins - Miguel Madureira");
                myString.AppendLine("@ : mr.niceonne@gmail.com");
                myString.AppendLine("@ : https://localbitcoins.com/accounts/profile/niceonne/");
                myString.AppendLine("WhatsApp : +351913247104");
            }
            else
            {
                myString.AppendLine($"Mr/Ms. {UsernameTrade} PLEASE READ ALL INSTRUCTION CLEARLY, OTHERWISE I CANT TRADE WITH YOU !!");
                myString.AppendLine();
                myString.AppendLine("➊ If you paying with credit or debit Card, please inform me otherwise continue with the instructions below");
                myString.AppendLine("➋ I need a screenshot of your settings page ( ➥ https://www.paypal.com/myaccount/settings/) where I can see name and adress like its on the next example (Example ➥ http://prntscr.com/iq70an).");
                myString.AppendLine("➌ I need a clear photo containing only your id (Example ➥ http://prntscr.com/ifnklz) matching the paypal name.");
                myString.Append("➍ I need a clear selfie, (Example ➥ http://prntscr.com/ifnkq6) holding the same id ");
                myString.AppendLine(string.Format("with a paper saying the date of today \"{0}/{1}/{2}\" and the contact number \"{3}\" ", DateTime.Now.Day.ToString(), DateTime.Now.Month.ToString(), DateTime.Now.Year.ToString(), ContactTradeNumber.ToString()));
                myString.AppendLine("");
                myString.AppendLine("This identity verification is only made once, so I just need to verify you in the first trade.");
                myString.AppendLine("If you follow my instructions this will be done under 15 min, so please try to patient.");
                myString.AppendLine("");
                myString.AppendLine("Things I can not accept from you : ");
                myString.AppendLine("➀ Expired documents");
                myString.AppendLine("➁ Payments by eCheck");
                myString.AppendLine("");
                myString.AppendLine("Thank you,");
                myString.AppendLine("niceonne BTC");
                myString.AppendLine("");
                myString.AppendLine("Niceonne LocalBitcoins - Miguel Madureira");
                myString.AppendLine("@ : mr.niceonne@gmail.com");
                myString.AppendLine("@ : https://localbitcoins.com/accounts/profile/niceonne/");
                myString.AppendLine("WhatsApp : +351913247104");
            }

            {
                var email = PaypalEmail;
                var fromAddress = new MailAddress("mr.niceonne@gmail.com", "");
                var toAddress = new MailAddress(email, "");
                const string fromPassword = "iebafeyguarhentd";
                string subject = subject = $"Contact #{ContactTradeNumber} - {UsernameTrade}";

                string body = myString.ToString();

                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
                };
                using (var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body
                })
                    try
                    {
                        smtp.Send(message);
                    }
                    catch (Exception ep)
                    {
                        MainWindow.ErrorLogging(ep);
                    }
            }
        }

        private void ButtonCopyEmail_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Clipboard.SetText(Emailpaypal);
        }

        private void ButtonMarkUserverified_Click(object sender, RoutedEventArgs e)
        {
            GridTradePanel.Visibility = Visibility.Collapsed;
            GridMarkuserverified.Visibility = Visibility.Visible;
        }

        async private void ButtonVerifyUser_Click(object sender, RoutedEventArgs e)
        {
            ViewBoxWarningConfirmIdentificationPanel.Visibility = Visibility.Collapsed;
            LabelWarningConfirmIdentificationPanel.Visibility = Visibility.Collapsed;
            LabelWarningConfirmIdentificationPanel.Content = "";
            SpinnerButtonVerifyUser.Visibility = Visibility.Visible;
            if (string.IsNullOrWhiteSpace(Emailpaypal))
            {
                ViewboxEmailemptyWarning.Visibility = Visibility.Visible;
                ViewBoxWarningConfirmIdentificationPanel.Visibility = Visibility.Visible;
                LabelWarningConfirmIdentificationPanel.Visibility = Visibility.Visible;
                LabelWarningConfirmIdentificationPanel.Content = "Email empty!";
            }
            else
            {
                await Task.Run(() => VerifiedIDsucess());
                SpinnerButtonVerifyUser.Visibility = Visibility.Collapsed;
            }
        }

        private void VerifiedIDsucess()
        {
            var APiKey = ConfigurationManager.AppSettings["ApiKeyMarkUserVerified"];
            var ApiSecret = ConfigurationManager.AppSettings["ApiSecretMarkUserVerified"];
            LocalBitcoinsDatabaseEntities _BaseDados = new LocalBitcoinsDatabaseEntities();
            var client = new LocalBitcoinsAPI(APiKey, ApiSecret);
            bool? CheckBoxStatus = false;
            CheckBoxVerificationPanel.Dispatcher.Invoke((() =>
            {
               CheckBoxStatus = CheckBoxVerificationPanel.IsChecked;
            }));
            StringBuilder myString = new StringBuilder();
            if (CheckBoxStatus == true)
            {
                myString.AppendLine($"Mr/Ms. {UsernameTrade}, ");
                myString.AppendLine("");
                myString.AppendLine("The verification of your identity was sucessfull! Thank you.");
                myString.AppendLine($"Let me remind you that in future I can only accept funds from the Paypal email \"{Emailpaypal}\".");
                myString.AppendLine("");
                myString.AppendLine("➦ Please send me a screenshot (Example ➥ http://prntscr.com/ifnken) of your paypal balance.");
            }
            else
            {
                myString.AppendLine($"Mr/Ms. {UsernameTrade}, ");
                myString.AppendLine("");
                myString.AppendLine("The verification of your identity was sucessfull! Thank you.");
                myString.AppendLine($"Let me remind you that in future I can only accept funds from the Paypal email \"{Emailpaypal}\".");
            }
            int numberofFiles = FilesverificationList.Count();
            if (numberofFiles == 0)
            {
                ViewBoxWarningConfirmIdentificationPanel.Dispatcher.BeginInvoke((Action)(() =>
                {
                    ViewBoxWarningConfirmIdentificationPanel.Visibility = Visibility.Visible;
                }));
                LabelWarningConfirmIdentificationPanel.Dispatcher.BeginInvoke((Action)(() =>
                {
                    LabelWarningConfirmIdentificationPanel.Visibility = Visibility.Visible;
                    LabelWarningConfirmIdentificationPanel.Content = "0 Files found!";
                }));
            }
            else
            {
                try
                {
                    client.MarkContactIdentified(ContactTradeNumber);
                    userverified = true;
                    try
                    {
                        client.PostMessageToContact(ContactTradeNumber, myString.ToString(), null);
                        MainWindow.newMessageUpdate = true;
                        using (_BaseDados)
                        {
                            bool UserExists = _BaseDados.Usernames.Any(x => x.Username1 == UsernameTrade);
                            if (UserExists == true)
                            {
                                var UserData = _BaseDados.Usernames.FirstOrDefault(x => x.Username1 == UsernameTrade).UsernameInfo;
                                UserData.Email = Emailpaypal;
                                UserData.Verified = true;
                                UserData.Verification_date = DateTime.Now;
                            }
                            else if (UserExists == false)
                            {
                                int DataBaseCount = _BaseDados.Usernames.Count();
                                int usernameKey = DataBaseCount + 1;
                                var userRealName = "";
                                if (TradeisSell == true)
                                    userRealName = InfoContact.data.buyer.real_name;
                                else
                                    userRealName = null;
                                _BaseDados.Usernames.Add(new Username()
                                {
                                    Username1 = UsernameTrade,
                                    UsernameKey = usernameKey,
                                    UsernameInfo = new UsernameInfo()
                                    {
                                        UsernameKey = usernameKey,
                                        Email = Emailpaypal,
                                        Verified = true,
                                        Verification_date = DateTime.Now,
                                        AccountCreatedAt = AccountCreatedDate,
                                        RealName = userRealName
                                    }
                                });
                            }
                            _BaseDados.SaveChanges();
                            gmailVerification();
                            GridMarkuserverified.Dispatcher.BeginInvoke((Action)(() =>
                            {
                                GridMarkuserverified.Visibility = Visibility.Collapsed;
                            }));
                            GridTradePanel.Dispatcher.BeginInvoke((Action)(() =>
                            {
                                GridTradePanel.Visibility = Visibility.Visible;
                            }));
                        }
                    }
                    catch (Exception ep)
                    {
                        MainWindow.ErrorLogging(ep);
                        ViewBoxWarningConfirmIdentificationPanel.Dispatcher.BeginInvoke((Action)(() =>
                        {
                            ViewBoxWarningConfirmIdentificationPanel.Visibility = Visibility.Visible;
                        }));
                        LabelWarningConfirmIdentificationPanel.Dispatcher.BeginInvoke((Action)(() =>
                        {
                            LabelWarningConfirmIdentificationPanel.Visibility = Visibility.Visible;
                            LabelWarningConfirmIdentificationPanel.Content = "Message not sent!";
                        }));
                    }
                    ButtonMarkUserverified.Dispatcher.BeginInvoke((Action)(() =>
                    {
                        ButtonMarkUserverified.Visibility = Visibility.Collapsed;
                    }));
                    TextBlockIdentifyingUser1.Dispatcher.BeginInvoke((Action)(() =>
                    {
                        TextBlock Block1PanelIdentifyingUser = TextBlockIdentifyingUser1;
                        Block1PanelIdentifyingUser.Inlines.Clear();
                        Block1PanelIdentifyingUser.Inlines.Add("You have already verified the identity of ");
                        Block1PanelIdentifyingUser.Inlines.Add(new Run(UsernameTrade + " .") { FontWeight = FontWeights.Bold });
                    }));
                }
                catch (Exception ep)
                {
                    MainWindow.ErrorLogging(ep);
                    ViewBoxWarningConfirmIdentificationPanel.Dispatcher.BeginInvoke((Action)(() =>
                    {
                        ViewBoxWarningConfirmIdentificationPanel.Visibility = Visibility.Visible;
                    }));
                    LabelWarningConfirmIdentificationPanel.Dispatcher.BeginInvoke((Action)(() =>
                    {
                        LabelWarningConfirmIdentificationPanel.Visibility = Visibility.Visible;
                        LabelWarningConfirmIdentificationPanel.Content = "User not marked as verified!";
                    }));
                }
            }
        }

        private void ButtonReturnConfirmationpanel_Click(object sender, RoutedEventArgs e)
        {
            if (userverified == true)
            {
                ButtonMarkUserverified.Dispatcher.BeginInvoke((Action)(() =>
                {
                    ButtonMarkUserverified.Visibility = Visibility.Collapsed;
                }));
                TextBlockIdentifyingUser1.Dispatcher.BeginInvoke((Action)(() =>
                {
                    TextBlock Block1PanelIdentifyingUser = TextBlockIdentifyingUser1;
                    Block1PanelIdentifyingUser.Inlines.Clear();
                    Block1PanelIdentifyingUser.Inlines.Add("You have already verified the identity of ");
                    Block1PanelIdentifyingUser.Inlines.Add(new Run(UsernameTrade + " .") { FontWeight = FontWeights.Bold });
                }));
                GridMarkuserverified.Dispatcher.BeginInvoke((Action)(() =>
                {
                    GridMarkuserverified.Visibility = Visibility.Collapsed;
                }));
                GridTradePanel.Dispatcher.BeginInvoke((Action)(() =>
                {
                    GridTradePanel.Visibility = Visibility.Visible;
                }));
            }
            else
            {
                GridMarkuserverified.Dispatcher.BeginInvoke((Action)(() =>
                {
                    GridMarkuserverified.Visibility = Visibility.Collapsed;
                }));
                GridTradePanel.Dispatcher.BeginInvoke((Action)(() =>
                {
                    GridTradePanel.Visibility = Visibility.Visible;
                }));
            }
        }

        public string VerificationsEmailBody()
        {
            var name = InfoContact.data.buyer.real_name;

            StringBuilder myString = new StringBuilder();
            myString.AppendLine($"Localbitcoins Contact {ContactTradeNumber}");
            myString.AppendLine();
            myString.AppendLine($"https://localbitcoins.com/accounts/profile/{UsernameTrade}");
            myString.AppendLine($"https://localbitcoins.com/request/online_sell_seller/{ContactTradeNumber}");
            myString.AppendLine($"{Emailpaypal}");
            myString.AppendLine();
            myString.AppendLine($"Nome Real: {name}");

            return myString.ToString();
        }

        private void gmailVerification()
        {
            var fromAddress = new MailAddress("mr.niceonne@hotmail.com", "");
            var toAddress = new MailAddress("fialjose@gmail.com", "");
            const string fromPassword = "933897374pass";
            string subject = UsernameTrade;
            string body = VerificationsEmailBody();

            var smtp = new SmtpClient
            {
                Host = "smtp.live.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };
            var message = new MailMessage(fromAddress, toAddress);
            message.Subject = subject;
            message.Body = body;

            foreach (var anexo in FilesverificationList)
            {
                message.Attachments.Add(new Attachment(anexo));
            }

            try
            {
                smtp.Send(message);
                message.Attachments.Dispose();
            }
            catch (Exception ep)
            {
                MainWindow.ErrorLogging(ep);
                ViewBoxWarningConfirmIdentificationPanel.Dispatcher.BeginInvoke((Action)(() =>
                {
                    ViewBoxWarningConfirmIdentificationPanel.Visibility = Visibility.Visible;
                }));
                LabelWarningConfirmIdentificationPanel.Dispatcher.BeginInvoke((Action)(() =>
                {
                    LabelWarningConfirmIdentificationPanel.Visibility = Visibility.Visible;
                    LabelWarningConfirmIdentificationPanel.Content = "Photos not sent to e-mail!";
                }));
            }
        }

        private void ButtonverificationFiles_Click(object sender, RoutedEventArgs e)
        {
            AddFilesVerification();
        }

        private List<string> FilesverificationList = new List<string>();

        private void AddFilesVerification()
        {
            int numberofFiles = FilesverificationList.Count();
            if (numberofFiles == 0)
            {
                OpenFileDialog fdlg = new OpenFileDialog();
                fdlg.Title = "Localbitcoins Verification Files";
                fdlg.InitialDirectory = path;
                fdlg.Filter = "All files (*.*)|*.*";
                fdlg.FilterIndex = 2;
                fdlg.Multiselect = true;
                fdlg.RestoreDirectory = true;

                if (fdlg.ShowDialog() == DialogResult.OK)
                {
                    foreach (String file in fdlg.FileNames)
                    {
                        FilesverificationList.Add(file);
                    }
                }
                var warnVisibitly = ViewBoxWarningConfirmIdentificationPanel.Visibility;
                if (warnVisibitly == Visibility.Visible)
                {
                    ViewBoxWarningConfirmIdentificationPanel.Visibility = Visibility.Collapsed;
                    LabelWarningConfirmIdentificationPanel.Visibility = Visibility.Collapsed;
                    BorderButtonverificationFiles.Visibility = Visibility.Visible;
                    TextBlockButtonAddFilesVerification.Text = FilesverificationList.Count().ToString();
                }
                else
                {
                    BorderButtonverificationFiles.Visibility = Visibility.Visible;
                    TextBlockButtonAddFilesVerification.Text = FilesverificationList.Count().ToString();
                }
            }
            else
            {
                FilesverificationList.Clear();
                TextBlockButtonAddFilesVerification.Text = FilesverificationList.Count().ToString();
            }
        }

        private void ButtonReleaseBitcoins_Click(object sender, RoutedEventArgs e)
        {
            if (TradeisSell == true)
            {
                if (userMarkedRealName == true)
                {
                    GridTradePanel.Visibility = Visibility.Collapsed;
                    GridReleaseBitcoins.Visibility = Visibility.Visible;
                }
                else
                {
                    GridTradePanel.Visibility = Visibility.Collapsed;
                    GridMarkuserrealname.Visibility = Visibility.Visible;
                }
            }
            else
            {
                GridTradePanel.Visibility = Visibility.Collapsed;
                GridReleaseBitcoins.Visibility = Visibility.Visible;
            }
        }

        async private void ButtonPanelReleaseBitcoins_Click(object sender, RoutedEventArgs e)
        {
            SpinnerButtonPanelReleaseBitcoins.Visibility = Visibility.Visible;
            if (CheckboxReleaseBitcoins.IsChecked == true)
            {
                TextBlockWarningCheckBoxReleaseBitcoins.Visibility = Visibility.Collapsed;
                Task ReleaseBitcoinsTask = new Task(ReleaseBitcoins);
                ReleaseBitcoinsTask.Start();
                await ReleaseBitcoinsTask;
            }
            else
            {
                TextBlockWarningCheckBoxReleaseBitcoins.Visibility = Visibility.Visible;
            }
            SpinnerButtonPanelReleaseBitcoins.Visibility = Visibility.Collapsed;
        }

        private void ReleaseBitcoins()
        {
            var APiKey = ConfigurationManager.AppSettings["ApiKeyReleaseBitcoins"];
            var ApiSecret = ConfigurationManager.AppSettings["ApiSecretReleaseBitcoins"];
            var client = new LocalBitcoinsAPI(APiKey, ApiSecret);
            LocalBitcoinsDatabaseEntities _BaseDados = new LocalBitcoinsDatabaseEntities();
            var Pin = "";
            var Typetrade = "";
            if (InfoContact.data.is_buying == true)
                Typetrade = "BUY";
            else if (InfoContact.data.is_selling == true)
                Typetrade = "SELL";
            TextBoxPincodeReleaseBitcoins.Dispatcher.Invoke((Action)(() =>
            {
                Pin = TextBoxPincodeReleaseBitcoins.Text;
            }));
            try
            {
                client.ContactReleasePin(ContactTradeNumber, Pin);
                MainWindow.newContactNumberonThelist = true;
                try
                {
                    decimal.TryParse(InfoContact.data.amount, out decimal AmountTrade);
                    var DatabaseUser = _BaseDados.Usernames.FirstOrDefault(x => x.Username1 == UsernameTrade);
                    DatabaseUser.UsernameTrades.Add(new UsernameTrade()
                    {
                        Contact_number = ContactTradeNumber,
                        Email_used = Emailpaypal,
                        Date = DateTime.Now,
                        UsernameKey = DatabaseUser.UsernameKey,
                        Trade_amount = AmountTrade,
                        Trade_Type = Typetrade,
                        Currency = InfoContact.data.currency

                    });
                    _BaseDados.SaveChanges();
                    GridReleaseBitcoins.Dispatcher.BeginInvoke((Action)(() =>
                    {
                        GridReleaseBitcoins.Visibility = Visibility.Collapsed;
                    }));

                    GridTradePanel.Dispatcher.BeginInvoke((Action)(() =>
                    {
                        GridTradePanel.Visibility = Visibility.Visible;
                    }));
                }
                catch (Exception ep)
                {
                    MainWindow.ErrorLogging(ep);
                    WarnButtonPanelReleaseBitcoins.Dispatcher.BeginInvoke((Action)(() =>
                    {
                        WarnButtonPanelReleaseBitcoins.Visibility = Visibility.Visible;
                    }));
                    TextBlockWarnPanelReleaseBitcoins.Dispatcher.BeginInvoke((Action)(() =>
                    {
                        TextBlockWarnPanelReleaseBitcoins.Text = "Trade not added to Database!";
                    }));
                }

            }
            catch (Exception ep)
            {
                MainWindow.ErrorLogging(ep);
                WarnButtonPanelReleaseBitcoins.Dispatcher.BeginInvoke((Action)(() =>
                {
                    WarnButtonPanelReleaseBitcoins.Visibility = Visibility.Visible;
                }));
                TextBlockWarnPanelReleaseBitcoins.Dispatcher.BeginInvoke((Action)(() =>
                {
                    TextBlockWarnPanelReleaseBitcoins.Text = "Bitcoins not released!";
                }));
            }

        }

        async private void ButtonMarkUserrealName_Click(object sender, RoutedEventArgs e)
        {
            SpinnerButtonMarkUserrealName.Visibility = Visibility.Visible;
            try
            {
                await Task.Run(() => MarkUserRealName());
                GridMarkuserrealname.Visibility = Visibility.Collapsed;
                GridReleaseBitcoins.Visibility = Visibility.Visible;
                LabelWarnMarkuserRealName.Visibility = Visibility.Collapsed;
            }
            catch(Exception ep)
            {
                MainWindow.ErrorLogging(ep);
                ViewBoxWarnMarkuserRealName.Visibility = Visibility.Visible;
                LabelWarnMarkuserRealName.Visibility = Visibility.Visible;
                LabelWarnMarkuserRealName.Content = "Real name not confirmed!";
            }
            SpinnerButtonMarkUserrealName.Visibility = Visibility.Collapsed;
        }

        private void MarkUserRealName()
        {
            var APiKey = ConfigurationManager.AppSettings["ApiKeyMarkUserRealName"];
            var ApiSecret = ConfigurationManager.AppSettings["ApiSecretMarkUserRealName"];
            var client = new LocalBitcoinsAPI(APiKey, ApiSecret);
            int UserNameverified = 4;
            bool Verified = true;
            bool checkboxverified = false;
            bool checkBoxNameDifferentInpayment = false;
            bool checkBoxVerificationSkipped = false;
            bool checkBoxNameNotvisibleinpayment = false;
            CheckBoxVerifiedNameMatched.Dispatcher.Invoke((() =>
            {
                checkboxverified = CheckBoxVerifiedNameMatched.IsChecked.Value;
            }));
            CheckBoxNameDifferentInpayment.Dispatcher.Invoke((() =>
            {
                checkBoxNameDifferentInpayment = CheckBoxNameDifferentInpayment.IsChecked.Value;
            }));
            CheckBoxVerificationSkipped.Dispatcher.Invoke((() =>
            {
                checkBoxVerificationSkipped = CheckBoxVerificationSkipped.IsChecked.Value;
            }));
            CheckBoxNameNotvisibleinpayment.Dispatcher.Invoke((() =>
            {
                checkBoxNameNotvisibleinpayment = CheckBoxNameNotvisibleinpayment.IsChecked.Value;
            }));

            if (checkboxverified == true)
                UserNameverified = 1;
            else if (checkBoxNameDifferentInpayment == true)
                UserNameverified = 2;
            else if (checkBoxVerificationSkipped == true)
                UserNameverified = 3;
            else if (checkBoxNameNotvisibleinpayment == true)
                UserNameverified = 4;
            CheckBoxIdverifiedPanelConfirmRealName.Dispatcher.Invoke((() =>
            {
                Verified = CheckBoxIdverifiedPanelConfirmRealName.IsChecked.Value;
            }));
            client.MarkContactRealName(ContactTradeNumber, UserNameverified, Verified);
            userMarkedRealName = true;
        }

        private void ButtonCancelMarkUserrealName_Click(object sender, RoutedEventArgs e)
        {
            GridMarkuserrealname.Visibility = Visibility.Collapsed;
            GridTradePanel.Visibility = Visibility.Visible;
        }

        private void GridMarkuserrealname_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (GridMarkuserrealname.Visibility == Visibility.Visible)
            {
                LabelgridMarkuserRealName.Content = $"Confirm {UsernameTrade}'s real name";
                TextBlockMarkUserRealName.Text = $"Have you verified that the real name {InfoContact.data.buyer.real_name.ToString()} from the buyer {UsernameTrade} " +
                    $"matches one in the online payment information for bitcoin purchase #{ContactTradeNumber}?";
            }
        }

        async private void GridReleaseBitcoins_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            SpinnerGridReleaseBitcoins.Visibility = Visibility.Visible;
            if (GridReleaseBitcoins.IsVisible == true)
            {
                try
                {
                    await Task.Run(() => gridreleaseBitcoinsVisiblechangeasync());
                }
                catch (Exception ep)
                {
                    MainWindow.ErrorLogging(ep);
                }
            }
            SpinnerGridReleaseBitcoins.Visibility = Visibility.Collapsed;
        }

        private void gridreleaseBitcoinsVisiblechangeasync()
        {
            var InvoiceStatus = "";
            if (!string.IsNullOrWhiteSpace(InvoiceIdPaypal))
            {
                var PaypalPaid = new PayPalApi().GetInvoiceInfo(InvoiceIdPaypal);
                InvoiceStatus = PaypalPaid.status;
            }
            else
            {
                InvoiceStatus = "Unknonwn";
            }
            GridReleaseBitcoins.Dispatcher.BeginInvoke((Action)(() => 
            {
                LabelReleaseBitcoinsPage.Content = $"Release Bitcoins to the buyer in trade #{ContactTradeNumber}";
                TextBlockReleaseBitcoinsPage.Inlines.Clear();
                TextBlockReleaseBitcoinsPage.Inlines.Add(new Run("◉ Buyer: ") { FontWeight = FontWeights.Bold });
                TextBlockReleaseBitcoinsPage.Inlines.Add(UsernameTrade);
                TextBlockReleaseBitcoinsPage.Inlines.Add(new LineBreak());
                TextBlockReleaseBitcoinsPage.Inlines.Add(new Run("◉ Real name: ") { FontWeight = FontWeights.Bold });
                TextBlockReleaseBitcoinsPage.Inlines.Add(InfoContact.data.buyer.real_name.ToString());
                TextBlockReleaseBitcoinsPage.Inlines.Add(new LineBreak());
                TextBlockReleaseBitcoinsPage.Inlines.Add(new Run("◉ Amount BTC: ") { FontWeight = FontWeights.Bold });
                TextBlockReleaseBitcoinsPage.Inlines.Add(InfoContact.data.amount_btc);
                TextBlockReleaseBitcoinsPage.Inlines.Add(new LineBreak());
                TextBlockReleaseBitcoinsPage.Inlines.Add(new Run($"◉ Amount {InfoContact.data.currency}: ") { FontWeight = FontWeights.Bold });
                TextBlockReleaseBitcoinsPage.Inlines.Add(InfoContact.data.amount);
                TextBlockReleaseBitcoinsPage.Inlines.Add(new LineBreak());
                TextBlockReleaseBitcoinsPage.Inlines.Add(new Run($"◉ Payment method: ") { FontWeight = FontWeights.Bold });
                TextBlockReleaseBitcoinsPage.Inlines.Add(InfoContact.data.advertisement.payment_method.ToString());
                TextBlockReleaseBitcoinsPage.Inlines.Add(new LineBreak());
                if (TypetradeNumber == 1 || TypetradeNumber == 2)
                {
                    if (InvoiceStatus == "PAID")
                    {
                        TextBlockReleaseBitcoinsPage.Inlines.Add(new Run($"◉ Paypal Invoice status: ") { FontWeight = FontWeights.Bold });
                        TextBlockReleaseBitcoinsPage.Inlines.Add(new Run(InvoiceStatus + " ") { Foreground = new SolidColorBrush(Colors.Green), FontWeight = FontWeights.Bold });
                        TextBlockReleaseBitcoinsPage.Inlines.Add(new Run("✔") { Foreground = new SolidColorBrush(Colors.Green), FontWeight = FontWeights.Bold });
                        TextBlockReleaseBitcoinsPage.Inlines.Add(new LineBreak());
                    }
                    else if (InvoiceStatus == "Unknonwn")
                    {
                        TextBlockReleaseBitcoinsPage.Inlines.Add(new Run($"◉ Paypal Invoice status: ") { FontWeight = FontWeights.Bold });
                        TextBlockReleaseBitcoinsPage.Inlines.Add("Unknonwn ");
                        TextBlockReleaseBitcoinsPage.Inlines.Add(new Run("✘") { Foreground = new SolidColorBrush(Colors.Red), FontWeight = FontWeights.Bold });
                        TextBlockReleaseBitcoinsPage.Inlines.Add(new LineBreak());
                    }
                    else
                    {
                        TextBlockReleaseBitcoinsPage.Inlines.Add(new Run($"◉ Paypal Invoice status: ") { FontWeight = FontWeights.Bold });
                        TextBlockReleaseBitcoinsPage.Inlines.Add(new Run(InvoiceStatus + " ") { Foreground = new SolidColorBrush(Colors.Red), FontWeight = FontWeights.Bold });
                        TextBlockReleaseBitcoinsPage.Inlines.Add(new Run("✘") { Foreground = new SolidColorBrush(Colors.Red), FontWeight = FontWeights.Bold });
                        TextBlockReleaseBitcoinsPage.Inlines.Add(new LineBreak());
                    }
                    TextBlockReleaseBitcoinsPage.Inlines.Add(new Run($"◉ Paypal email: ") { FontWeight = FontWeights.Bold });
                    TextBlockReleaseBitcoinsPage.Inlines.Add(PaypalEmail);
                    TextBlockReleaseBitcoinsPage.Inlines.Add(new LineBreak());
                }
            }));
        }

        async private void ButtonGridReleaseBitcoinsReload_Click(object sender, RoutedEventArgs e)
        {
            SpinnerGridReleaseBitcoins.Visibility = Visibility.Visible;
            if (GridReleaseBitcoins.IsVisible == true)
            {
                try
                {
                    await Task.Run(() => gridreleaseBitcoinsVisiblechangeasync());
                }
                catch (Exception ep)
                {
                    MainWindow.ErrorLogging(ep);
                }
            }
            SpinnerGridReleaseBitcoins.Visibility = Visibility.Collapsed;
        }

        async private void ButtonBuyingEmailVerification_Click(object sender, RoutedEventArgs e)
        {
            SpinnerButtonBuyingEmailverification.Visibility = Visibility.Visible;
            try
            {
                await Task.Run(() => EmailConfirmationMessage());
                MainWindow.newMessageUpdate = true;
            }
            catch (Exception ep)
            {
                MainWindow.ErrorLogging(ep);
            }
            SpinnerButtonBuyingEmailverification.Visibility = Visibility.Collapsed;
        }

        private void EmailConfirmationMessage()
        {
            var APiKey = ConfigurationManager.AppSettings["ApiKeyMessages"];
            var ApiSecret = ConfigurationManager.AppSettings["ApiSecretMessages"];
            var client = new LocalBitcoinsAPI(APiKey, ApiSecret);

            StringBuilder myString = new StringBuilder();
            myString.AppendLine($"Hello, {UsernameTrade}");
            myString.AppendLine();
            myString.AppendLine("Thank you for choosing my services again. If possible just tell me if the below email is correct.");
            myString.AppendLine($"Email: {Emailpaypal}");
            myString.AppendLine();
            myString.Append("⌬ By default all the payments are made by 'Services and goods', but if necessary or if you want I can send as 'Friend and family', ");
            myString.AppendLine("however if you choose 'Family and friends' we will split the fee which, usually is 1%.");
            try
            {
                client.PostMessageToContact(ContactTradeNumber, myString.ToString(), null);
                WarnButtonBuyingEmailverification.Dispatcher.BeginInvoke((Action)(() =>
                {
                    WarnButtonBuyingEmailverification.Visibility = Visibility.Collapsed;
                }));
            }
            catch (Exception ep)
            {
                MainWindow.ErrorLogging(ep);
                WarnButtonBuyingEmailverification.Dispatcher.BeginInvoke((Action)(() =>
                {
                    WarnButtonBuyingEmailverification.Visibility = Visibility.Visible;
                }));
            }

        }

        private void ButtonCancelPanelReleaseBitcoins_Click(object sender, RoutedEventArgs e)
        {
            GridTradePanel.Visibility = Visibility.Visible;
            GridReleaseBitcoins.Visibility = Visibility.Collapsed;
        }

        private void CheckBoxVerifiedNameMatched_Checked(object sender, RoutedEventArgs e)
        {
            if (CheckBoxNameDifferentInpayment.IsChecked == true || CheckBoxVerificationSkipped.IsChecked == true || CheckBoxNameNotvisibleinpayment.IsChecked == true)
            {
                CheckBoxNameDifferentInpayment.IsChecked = false;
                CheckBoxVerificationSkipped.IsChecked = false;
                CheckBoxNameNotvisibleinpayment.IsChecked = false;
                CheckBoxVerifiedNameMatched.IsChecked = true;
            }
            else
                CheckBoxVerifiedNameMatched.IsChecked = true;
        }

        private void CheckBoxNameDifferentInpayment_Checked(object sender, RoutedEventArgs e)
        {
            if (CheckBoxVerifiedNameMatched.IsChecked == true || CheckBoxVerificationSkipped.IsChecked == true || CheckBoxNameNotvisibleinpayment.IsChecked == true)
            {
                CheckBoxVerifiedNameMatched.IsChecked = false;
                CheckBoxVerificationSkipped.IsChecked = false;
                CheckBoxNameNotvisibleinpayment.IsChecked = false;
                CheckBoxNameDifferentInpayment.IsChecked = true;
            }
            else
                CheckBoxNameDifferentInpayment.IsChecked = true;
        }

        private void CheckBoxVerificationSkipped_Checked(object sender, RoutedEventArgs e)
        {
            if (CheckBoxVerifiedNameMatched.IsChecked == true || CheckBoxNameDifferentInpayment.IsChecked == true || CheckBoxNameNotvisibleinpayment.IsChecked == true)
            {
                CheckBoxVerifiedNameMatched.IsChecked = false;
                CheckBoxNameDifferentInpayment.IsChecked = false;
                CheckBoxNameNotvisibleinpayment.IsChecked = false;
                CheckBoxVerificationSkipped.IsChecked = true;
            }
            else
                CheckBoxVerificationSkipped.IsChecked = true;
        }

        private void CheckBoxNameNotvisibleinpayment_Checked(object sender, RoutedEventArgs e)
        {
            if (CheckBoxVerifiedNameMatched.IsChecked == true || CheckBoxNameDifferentInpayment.IsChecked == true || CheckBoxVerificationSkipped.IsChecked == true)
            {
                CheckBoxVerifiedNameMatched.IsChecked = false;
                CheckBoxNameDifferentInpayment.IsChecked = false;
                CheckBoxVerificationSkipped.IsChecked = false;
                CheckBoxNameNotvisibleinpayment.IsChecked = true;
            }
            else
                CheckBoxNameNotvisibleinpayment.IsChecked = true;
        }

        private void CheckBoxTrustworthy_Checked(object sender, RoutedEventArgs e)
        {
            CheckBoxTrustworthy.IsChecked = true;
            CheckBoxPositive.IsChecked = false;
            CheckBoxNeutral.IsChecked = false;
            CheckBoxBlock.IsChecked = false;
            CheckBoxDistrust.IsChecked = false;
        }

        private void CheckBoxPositive_Checked(object sender, RoutedEventArgs e)
        {
            CheckBoxTrustworthy.IsChecked = false;
            CheckBoxPositive.IsChecked = true;
            CheckBoxNeutral.IsChecked = false;
            CheckBoxBlock.IsChecked = false;
            CheckBoxDistrust.IsChecked = false;
        }

        private void CheckBoxNeutral_Checked(object sender, RoutedEventArgs e)
        {
            CheckBoxTrustworthy.IsChecked = false;
            CheckBoxPositive.IsChecked = false;
            CheckBoxNeutral.IsChecked = true;
            CheckBoxBlock.IsChecked = false;
            CheckBoxDistrust.IsChecked = false;
        }

        private void CheckBoxDistrust_Checked(object sender, RoutedEventArgs e)
        {
            CheckBoxTrustworthy.IsChecked = false;
            CheckBoxPositive.IsChecked = false;
            CheckBoxNeutral.IsChecked = false;
            CheckBoxBlock.IsChecked = false;
            CheckBoxDistrust.IsChecked = true;
        }

        private void CheckBoxBlock_Checked(object sender, RoutedEventArgs e)
        {
            CheckBoxTrustworthy.IsChecked = false;
            CheckBoxPositive.IsChecked = false;
            CheckBoxNeutral.IsChecked = false;
            CheckBoxBlock.IsChecked = true;
            CheckBoxDistrust.IsChecked = false;
        }

        public String randomFeedback()
        {
            List<string> feedback = new List<string>();
            feedback.Add("Smooth and fast Trade :D");
            feedback.Add("Great Trader and Honest person :D");
            feedback.Add("Good Instructions Follower, Hope to do business with you again :D");
            feedback.Add("Smooth trade :D");
            feedback.Add("Good trader");
            feedback.Add("Nice trader");
            feedback.Add("Hope to do business with you again");
            feedback.Add("Trade went smooth without any problems");
            feedback.Add("Honest person and smooth trade");
            feedback.Add("Excelent trade, A++. Recommended ");

            Random rnd = new Random();

            string resultado = feedback[rnd.Next(feedback.Count)];

            return resultado;
        }

        async private void ButtonUpdateFeedback_Click(object sender, RoutedEventArgs e)
        {
            SpinnerButtonfeedback.Visibility = Visibility.Visible;
            StringBuilder feedbackString = new StringBuilder();
            if (CheckBoxTrustworthy.IsChecked == true)
            {
                feedbackString.Clear();
                feedbackString.Append($"+feedback ☛ {randomFeedback()} ");
                feedbackString.AppendLine("https://localbitcoins.com/accounts/profile/niceonne/");
                feedbackString.AppendLine("");
                feedbackString.AppendLine("☑️ Niceonne BTC Pro Trader ⭐ ");
                var FeedBackType = "trust";
                await Task.Run(() => UserFeedBack(feedbackString.ToString(), FeedBackType));
            }
            else if (CheckBoxPositive.IsChecked == true)
            {
                feedbackString.Clear();
                feedbackString.Append($"+feedback ☛ {randomFeedback()} ");
                feedbackString.AppendLine("https://localbitcoins.com/accounts/profile/niceonne/");
                feedbackString.AppendLine("");
                feedbackString.AppendLine("☑️ Niceonne BTC Pro Trader ⭐ ");

                var FeedBackType = "positive";
                await Task.Run(() => UserFeedBack(feedbackString.ToString(), FeedBackType));
            }
            else if (CheckBoxNeutral.IsChecked == true)
            {
                feedbackString.Clear();
                feedbackString.Append($"User didnt complete the trade with me!");
                feedbackString.AppendLine("https://localbitcoins.com/accounts/profile/niceonne/");
                feedbackString.AppendLine("");
                feedbackString.AppendLine("☑️ Niceonne BTC Pro Trader ⭐ ");
                var FeedBackType = "neutral";
                await Task.Run(() => UserFeedBack(feedbackString.ToString(), FeedBackType));
            }
            else if (CheckBoxDistrust.IsChecked == true)
            {
                feedbackString.Clear();
                feedbackString.Append($"Do not trade with this trades, scammer! Be carefull");
                feedbackString.AppendLine("https://localbitcoins.com/accounts/profile/niceonne/");
                feedbackString.AppendLine("");
                feedbackString.AppendLine("☑️ Niceonne BTC Pro Trader ⭐ ");
                var FeedBackType = "block";
                await Task.Run(() => UserFeedBack(feedbackString.ToString(), FeedBackType));
            }
            else if (CheckBoxBlock.IsChecked == true)
            {
                feedbackString.Clear();
                feedbackString = null;
                var FeedBackType = "block_without_feedback";
                await Task.Run(() => UserFeedBack(feedbackString.ToString(), FeedBackType));
            }
            SpinnerButtonfeedback.Visibility = Visibility.Collapsed;
        }

        private void UserFeedBack(string feedbackMessage, string feedbackType)
        {
            var APiKey = ConfigurationManager.AppSettings["ApiKeyReleaseBitcoins"];
            var ApiSecret = ConfigurationManager.AppSettings["ApiSecretReleaseBitcoins"];
            var client = new LocalBitcoinsAPI(APiKey, ApiSecret);
            try
            {
                client.PostFeedbackToUser(UsernameTrade, feedbackType, feedbackMessage.ToString());
                try
                {
                    using (var basedadosInfo = new LocalBitcoinsDatabaseEntities())
                    {
                        var userkey = DatabaseInfo.UsernameKey;
                        var user = basedadosInfo.UsernameInfoes.FirstOrDefault(x => x.UsernameKey == userkey);
                        user.feedbacktype = feedbackType;
                        user.myFeedBackMessage = feedbackMessage;
                        user.FeedBackExists = true;
                        basedadosInfo.SaveChanges();
                    };
                    TextBoxFeedback.Dispatcher.BeginInvoke((Action)(() =>
                    {
                        TextBoxFeedback.Text = feedbackMessage;
                    }));
                    ViewBoxWarnButtonFeedback.Dispatcher.BeginInvoke((Action)(() =>
                    {
                        ViewBoxWarnButtonFeedback.Visibility = Visibility.Collapsed;
                    }));
                }
                catch (Exception ep)
                {
                    MainWindow.ErrorLogging(ep);
                    ViewBoxWarnButtonFeedback.Dispatcher.BeginInvoke((Action)(() => 
                    {
                        ViewBoxWarnButtonFeedback.Visibility = Visibility.Visible;
                    }));
                    TextBlockWarnButtonFeedback.Dispatcher.BeginInvoke((Action)(() =>
                    {
                        TextBlockWarnButtonFeedback.Text = "Database not updated with feedback! Check log!";
                    }));
                }
            }
            catch (Exception ep)
            {
                MainWindow.ErrorLogging(ep);
                ViewBoxWarnButtonFeedback.Dispatcher.BeginInvoke((Action)(() =>
                {
                    ViewBoxWarnButtonFeedback.Visibility = Visibility.Visible;
                }));
                TextBlockWarnButtonFeedback.Dispatcher.BeginInvoke((Action)(() =>
                {
                    TextBlockWarnButtonFeedback.Text = "Feedback not updated! Check log!";
                }));
            }

        }

        async private void ButtonMarkasPaid_Click(object sender, RoutedEventArgs e)
        {
            SpinnerButtonMarkasPaid.Visibility = Visibility.Visible;
            try
            {
                await Task.Run(() => PaymentMessage());
                MainWindow.newMessageUpdate = true;
            }
            catch (Exception ep)
            {
                MainWindow.ErrorLogging(ep);
            }
            SpinnerButtonMarkasPaid.Visibility = Visibility.Collapsed;
        }

        private void PaymentMessage()
        {
            var APiKey = ConfigurationManager.AppSettings["ApiKeyMarkUserVerified"];
            var ApiSecret = ConfigurationManager.AppSettings["ApiSecretMarkUserVerified"];
            var client = new LocalBitcoinsAPI(APiKey, ApiSecret);

            StringBuilder myString = new StringBuilder();
            myString.AppendLine($"Payment successfully sent to : '{Emailpaypal}'");
            myString.AppendLine("If you have any questions feel free to ask me!");
            myString.AppendLine();
            myString.AppendLine("If you have no further questions and you confirmed the payment, do not forget to release the bitcoins!!");
            myString.AppendLine("Thank you, Niceonne");

            try
            {
                client.MarkContactAsPaid(ContactTradeNumber);
                MainWindow.newContactNumberonThelist = true;
                try
                {
                    client.PostMessageToContact(ContactTradeNumber, myString.ToString(), null);
                    ViewboxWarnButtonMarkaspaid.Dispatcher.BeginInvoke((Action)(() =>
                    {
                        ViewboxWarnButtonMarkaspaid.Visibility = Visibility.Collapsed;
                    }));
                }
                catch (Exception ep)
                {
                    MainWindow.ErrorLogging(ep);
                    ViewboxWarnButtonMarkaspaid.Dispatcher.BeginInvoke((Action)(() => 
                    {
                        ViewboxWarnButtonMarkaspaid.Visibility = Visibility.Visible;
                    }));
                    TextBlockWarnButtonMarkasPaid.Dispatcher.BeginInvoke((Action)(() =>
                    {
                        TextBlockWarnButtonMarkasPaid.Text = "Message not sent! Check logs!";
                    }));
                }
            }
            catch (Exception ep)
            {
                MainWindow.ErrorLogging(ep);
                ViewboxWarnButtonMarkaspaid.Dispatcher.BeginInvoke((Action)(() =>
                {
                    ViewboxWarnButtonMarkaspaid.Visibility = Visibility.Visible;
                }));
                TextBlockWarnButtonMarkasPaid.Dispatcher.BeginInvoke((Action)(() =>
                {
                    TextBlockWarnButtonMarkasPaid.Text = "Contact not marked as paid! Check logs!";
                }));
            }
        }

        async private void ListBoxSettings_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            int indexofseleted = ListBoxSettings.SelectedIndex;
            if (indexofseleted == 0)
            {
                LocalBitcoinsDatabaseEntities _BaseDados = new LocalBitcoinsDatabaseEntities();
                Username UserData = _BaseDados.Usernames.FirstOrDefault(x => x.Username1 == UsernameTrade);
                if (UserData.UsernameInfo.Verified == true) // Sell Return costumer
                {
                    GridTradePanel.Visibility = Visibility.Collapsed;
                    GridTradePanelLoading.Visibility = Visibility.Visible;
                    PanelButtonsNewBuyerPaypal.Visibility = Visibility.Collapsed;
                    Task AllInfotask = new Task(ReturnPaypalSell);
                    AllInfotask.Start();
                    await AllInfotask;
                    GridTradePanelLoading.Visibility = Visibility.Collapsed;
                    GridTradePanel.Visibility = Visibility.Visible;
                    Emailpaypal = UserData.UsernameInfo.Email;
                    NotifyPropertyChanged("Emailpaypal");
                }
            }
            ListBoxSettings.Visibility = Visibility.Collapsed;
        }

        private void ButtonRefreshPanelTrades_Click(object sender, RoutedEventArgs e)
        {
            StackPanelLoadingWaitingWarn.Visibility = Visibility.Collapsed;
            StackPanelLoadingWaitingSpinner.Visibility = Visibility.Visible;
            UserControl_Loaded(sender, e);
        }

        private void ButtonFolderTrade_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(path);
        }
    }
}
