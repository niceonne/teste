using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Timers;
using System.Threading;
using System.Configuration;
using LocalBitcoins;
using Newtonsoft.Json;
using static LocalBitcoins.GetContactInfo;
using System.Runtime.InteropServices;
using System.IO;
using static LocalBitcoins.GetRecentMessages;
using static LocalBitcoins.GetOwnAds;
using Microsoft.Win32;
using System.Net.Mail;
using System.Net;
using System.Windows.Media.Animation;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.WindowsAPICodePack.Taskbar;
using System.Windows.Shell;

namespace TesteLocalbitcoinsApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            NotificationTimer.Tick += new EventHandler(NotificationTimer_Tick);
            NotificationTimer.Interval = new TimeSpan(0, 0, 4);
            NotificationTimer.Start();
            CloseTabsTimer.Tick += new EventHandler(CloseTabsTimer_Tick);
            CloseTabsTimer.Interval = new TimeSpan(0, 0, 60);
            CloseTabsTimer.Start();
            UpdateConfigFile();
        }

        public static string directoryPath = "";
        public static GetRecentMessages.RootRecentMessages MessagesLCB { set; get; }
        public static int OpenTotalTrades = 0;
        public static bool newMessageUpdate { get; set; }
        public static List<string> TradesContactidsList = new List<string>();
        public static GetAllContacts AllCurrentTrades { get; set; }
        public static bool newContactNumberonThelist = false;
        bool AdsUpdated = false;
        string lastidNotification { get; set; }
        bool newMessagebool = false;
        private DateTime lastNotificationDate { get; set; }
        Dictionary<string, NotificationsMessages> dictionary = new Dictionary<string, NotificationsMessages>();
        public static List<string> ListremoveTab = new List<string>();
        DispatcherTimer NotificationTimer = new DispatcherTimer();
        DispatcherTimer CloseTabsTimer = new DispatcherTimer();

        async private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            tabControlBTC.SelectedItem = HomeTab;
            bool MissingApis = ApiKeysCheckMissing();
            if (MissingApis == true)
                WarnButtonSettingsPanel.Visibility = Visibility.Visible;

            await Task.Run(() => Getmysefl());
            await Task.Run(() => GetBitcoinsWalletInfo());

        }

        private void SetTaskBarOverlay(int numberofnotification)
        {
            if (numberofnotification == 0)
            {
                Application.Current.Dispatcher.BeginInvoke((Action)(() =>
                {
                    var tbi = new TaskbarItemInfo();
                    tbi.Overlay = null;
                    TaskbarItemInfo = tbi;
                }));
            }
            else if( numberofnotification > 0 && numberofnotification < 10)
            {
                string notificationnumber = numberofnotification.ToString();
                Application.Current.Dispatcher.BeginInvoke((Action)delegate()
                {
                    var dg = new DrawingGroup();
                    var dc = dg.Open();
                    dc.DrawEllipse(((SolidColorBrush)new BrushConverter().ConvertFromString("#F23C34")), new Pen(((SolidColorBrush)new BrushConverter().ConvertFromString("#F23C34")), 1), new Point(7.5, 7), 8, 8);
                    dc.DrawText(new FormattedText(notificationnumber, System.Threading.Thread.CurrentThread.CurrentUICulture, System.Windows.FlowDirection.LeftToRight,
                        new Typeface("Arial"), 12, Brushes.White), new Point(4, 0));
                    dc.Close();
                    var geometryImage = new DrawingImage(dg);
                    geometryImage.Freeze();

                    // set on this window
                    var tbi = new TaskbarItemInfo();
                    tbi.Overlay = geometryImage;

                    this.TaskbarItemInfo = tbi;
                });
            }
            else if (numberofnotification > 10 && numberofnotification < 100)
            {
                string notificationnumber = numberofnotification.ToString();
                Application.Current.Dispatcher.BeginInvoke((Action)delegate ()
                {
                    var dg = new DrawingGroup();
                    var dc = dg.Open();
                    dc.DrawEllipse(((SolidColorBrush)new BrushConverter().ConvertFromString("#F23C34")), new Pen(((SolidColorBrush)new BrushConverter().ConvertFromString("#F23C34")), 1), new Point(10.5, 7), 10, 10);
                    dc.DrawText(new FormattedText(notificationnumber, System.Threading.Thread.CurrentThread.CurrentUICulture, System.Windows.FlowDirection.LeftToRight,
                        new Typeface("Arial"), 12, Brushes.White), new Point(4, 0));
                    dc.Close();
                    var geometryImage = new DrawingImage(dg);
                    geometryImage.Freeze();

                    // set on this window
                    var tbi = new TaskbarItemInfo();
                    tbi.Overlay = geometryImage;

                    this.TaskbarItemInfo = tbi;
                });
            }
        }

        private void Getmysefl()
        {
            var APiKey = ConfigurationManager.AppSettings["ApiKeyReleaseBitcoins"];
            var ApiSecret = ConfigurationManager.AppSettings["ApiSecretReleaseBitcoins"];
            var client = new LocalBitcoinsAPI(APiKey, ApiSecret);
            try
            {
                var InfoMyself = client.GetMyself();
                GetMySelf.RootMySelf infoJsonMyself = JsonConvert.DeserializeObject<GetMySelf.RootMySelf>(InfoMyself);
                var username = infoJsonMyself.data.username;
                TextBlockUsernameOwner.Dispatcher.BeginInvoke((Action)(() => 
                {
                    TextBlockUsernameOwner.Text = username;
                    TextBlockUsernameOwner.Foreground = new SolidColorBrush(Color.FromRgb(119, 119, 119));
                }));
                TextbloxkusernameOwnerWarn.Dispatcher.BeginInvoke((Action)(() =>
                {
                    TextbloxkusernameOwnerWarn.Visibility = Visibility.Collapsed;
                }));
                ButtonUsernameOwner.Dispatcher.BeginInvoke((Action)(() =>
                {
                    ButtonUsernameOwner.Visibility = Visibility.Collapsed;
                }));
            }
            catch (Exception ep)
            {
                ErrorLogging(ep);
                TextBlockUsernameOwner.Dispatcher.BeginInvoke((Action)(() =>
                {
                    TextBlockUsernameOwner.Text = "Error!";
                    TextBlockUsernameOwner.Foreground = Brushes.Red;
                }));
                TextbloxkusernameOwnerWarn.Dispatcher.BeginInvoke((Action)(() =>
                {
                    TextbloxkusernameOwnerWarn.Visibility = Visibility.Visible;
                }));
                ButtonUsernameOwner.Dispatcher.BeginInvoke((Action)(() =>
                {
                    ButtonUsernameOwner.Visibility = Visibility.Visible;
                }));
            }

        }

        private void GetBitcoinsWalletInfo()
        {
            var APiKey = ConfigurationManager.AppSettings["ApiKeyReleaseBitcoins"];
            var ApiSecret = ConfigurationManager.AppSettings["ApiSecretReleaseBitcoins"];
            var client = new LocalBitcoinsAPI(APiKey, ApiSecret);
            try
            {
                var getWallet = client.GetWallet();
                GetWallet.walletData infoWallet = JsonConvert.DeserializeObject<GetWallet.walletData>(getWallet);
                TextBlockBitcoinsBalance.Dispatcher.BeginInvoke((Action)(() => 
                {
                    TextBlockBitcoinsBalance.Text = infoWallet.data.total.balance.ToString();
                }));
                ViewboxWarnBitcoinsBalance.Dispatcher.BeginInvoke((Action)(() =>
                {
                    ViewboxWarnBitcoinsBalance.Visibility = Visibility.Collapsed;
                }));
                ButtonBitcoinsBalance.Dispatcher.BeginInvoke((Action)(() =>
                {
                    ButtonBitcoinsBalance.Visibility = Visibility.Collapsed;
                }));
            }
            catch (Exception ep)
            {
                ErrorLogging(ep);
                ViewboxWarnBitcoinsBalance.Dispatcher.BeginInvoke((Action)(() =>
                {
                    ViewboxWarnBitcoinsBalance.Visibility = Visibility.Visible;
                }));
                ButtonBitcoinsBalance.Dispatcher.BeginInvoke((Action)(() =>
                {
                    ButtonBitcoinsBalance.Visibility = Visibility.Visible;
                }));
            }
        }

        async private void ButtonUsernameOwner_Click(object sender, RoutedEventArgs e)
        {
            await Task.Run(() => Getmysefl());
        }

        async private void ButtonBitcoinsBalance_Click(object sender, RoutedEventArgs e)
        {
            await Task.Run(() => GetBitcoinsWalletInfo());
        }

        private void UpdateConfigFile()
        {
            var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var settings = configFile.AppSettings.Settings;
            if (settings["DirectoryPath"].Value == null)
            {
                MessageBox.Show("Directory not found!");
            }
            else
            {
                directoryPath = settings["DirectoryPath"].Value;
            }
            configFile.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
        }

        async private void CloseTabsTimer_Tick(object sender, EventArgs e)
        {
            CloseTabsTimer.Stop();
            await Task.Run(() => CloseTabtask());
            CloseTabsTimer.Start();
        }

        private void CloseTabtask()
        {
            var itemtodelete = new List<string>();
            if (ListremoveTab.Count() > 0)
            {
                foreach(var listitem in ListremoveTab)
                {
                    try
                    {
                        CloseTabvoid(null, listitem);
                        itemtodelete.Add(listitem);
                    }
                    catch (Exception ep)
                    {
                        ErrorLogging(ep);
                    }

                }
            }
            itemtodelete.ForEach(x => ListremoveTab.Remove(x));
        }

        async private void NotificationTimer_Tick(object sender, EventArgs e)
        {
            NotificationTimer.Stop();
            try
            {
                await Task.Run(() => GetNotifications());
            }
            catch (Exception ep)
            {
                MainWindow.ErrorLogging(ep);
            }
            NotificationTimer.Start();
        }

        public void GetNotifications()
        {
            var APiKey = ConfigurationManager.AppSettings["ApikeyNotifications"];
            var ApiSecret = ConfigurationManager.AppSettings["ApiSecretNotifications"];
            var client = new LocalBitcoinsAPI(APiKey, ApiSecret);
            RootNotification messages = null;
            var getNotifications = client.GetNotifications();
            if (!string.IsNullOrWhiteSpace(getNotifications))
            {
                messages = JsonConvert.DeserializeObject<RootNotification>(getNotifications);
                var notifications = messages.data.Where(x => !x.read);
                if (notifications.Count() > 0 )
                {
                    var countNotifications = notifications.Count();
                    var firstMessageNotification = notifications.FirstOrDefault(x => x.msg.Contains("new message from"));
                    DateTime firstMessageDatetime = lastNotificationDate;
                    if (firstMessageNotification == null)
                        firstMessageDatetime = lastNotificationDate;
                    else
                        firstMessageDatetime = firstMessageNotification.created_at;
                    if (firstMessageDatetime > lastNotificationDate)
                        newMessagebool = true;
                    else
                        newMessagebool = false;

                    lastNotificationDate = firstMessageDatetime;

                    foreach (var item in notifications)
                    {
                        var idContact = item.contact_id;
                        var message = item.msg;
                        var notificationId = item.id;
                        var datenotification = item.created_at;

                        var itemList = new NotificationsMessages() { Message = message, contactID = idContact, created_at = datenotification };
                        if (TradesContactidsList.Contains(idContact) == false && idContact != null)
                        {
                            TradesContactidsList.Add(idContact);
                            newContactNumberonThelist = true;
                        }
                        if (dictionary.Any(x => x.Value.Message == message))
                        {

                        }
                        else
                        {
                            if (message.Contains("payment marked complete"))
                                newContactNumberonThelist = true;
                            dictionary.Add(notificationId, itemList);
                        }
                    }
                }
            }
            if (newMessagebool == true || newMessageUpdate == true)
                GetChatMessagesGeral();
            if (newContactNumberonThelist == true)
                GetContactTrades();
            if (messages != null)
            {
                var notifications = messages.data.Where(x => !x.read);
                if (notifications.Count() > 0)
                {
                    string firstNotificaionid = notifications.First().id;
                    if (lastidNotification != firstNotificaionid)
                    {
                        ListBoxNotifications.Dispatcher.BeginInvoke((Action)(() =>
                        {
                            ListBoxNotifications.Items.Clear();
                        }));
                        foreach (var item in dictionary.Values)
                        {
                            ListBoxNotifications.Dispatcher.BeginInvoke((Action)(() =>
                            {
                                ListBoxNotifications.Items.Insert(0, item.Message);
                            }));
                        }
                    }
                    lastidNotification = notifications.FirstOrDefault().id;
                }
            }
            if (dictionary.Count > 0)
            {
                BorderNotifications.Dispatcher.BeginInvoke((Action)(() =>
                {
                    BorderNotifications.Background = Brushes.Red;
                }));
                ListBoxNotifications.Dispatcher.BeginInvoke((Action)(() =>
                {
                    SetTaskBarOverlay(ListBoxNotifications.Items.Count);
                }));
            }
            else if (dictionary.Count == 0)
            {
                BorderNotifications.Dispatcher.BeginInvoke((Action)(() =>
                {
                    BorderNotifications.Background = new SolidColorBrush(Color.FromRgb(119, 119, 119));
                }));
                ListBoxNotifications.Dispatcher.BeginInvoke((Action)(() =>
                {
                    SetTaskBarOverlay(ListBoxNotifications.Items.Count);
                }));
            }
        }

        private void GetChatMessagesGeral()
        {
            if (newMessagebool == true || newMessageUpdate == true)
            {
                var APiKey = ConfigurationManager.AppSettings["ApikeyGobalMessages"];
                var ApiSecret = ConfigurationManager.AppSettings["ApiSecretGobalMessages"];
                var client = new LocalBitcoinsAPI(APiKey, ApiSecret);
                try
                {
                    var recentMessages = client.GetRecentMessages();
                    RootRecentMessages RootMessages = JsonConvert.DeserializeObject<RootRecentMessages>(recentMessages);
                    MessagesLCB = RootMessages;
                    var lastmessage = RootMessages.data.message_list.FirstOrDefault();
                    newMessagebool = false;
                    newMessageUpdate = false;
                }
                catch (Exception ep)
                {
                    ErrorLogging(ep);
                }
            }
        }

        private void GetContactTrades()
        {
            var APiKey = ConfigurationManager.AppSettings["ApiKeyNotifications"];
            var ApiSecret = ConfigurationManager.AppSettings["ApiSecretNotifications"];
            var client = new LocalBitcoinsAPI(APiKey, ApiSecret);
            if (newContactNumberonThelist == true)
            {
                StringBuilder ContactidString = new StringBuilder();
                var countListContactTrades = TradesContactidsList.Count;
                if (countListContactTrades == 0)
                {

                }
                else if (countListContactTrades == 1)
                {
                    ContactidString.Append($"{TradesContactidsList.FirstOrDefault()}");
                    try
                    {
                        var getContacts = client.GetContactsInfo(ContactidString.ToString());
                        GetAllContacts contactInfosJson = JsonConvert.DeserializeObject<GetAllContacts>(getContacts);
                        AllCurrentTrades = contactInfosJson;
                        newContactNumberonThelist = false;
                    }
                    catch (Exception ep)
                    {
                        ErrorLogging(ep);
                    }
                }
                else if (countListContactTrades > 1)
                {
                    foreach (var item in TradesContactidsList)
                    {
                        if (item == TradesContactidsList.Last())
                        {
                            ContactidString.Append($"{item}");
                        }
                        else
                        {
                            ContactidString.Append($"{item},");
                        }
                    }
                    try
                    {
                        var getContacts = client.GetContactsInfo(ContactidString.ToString());
                        GetAllContacts contactInfosJson = JsonConvert.DeserializeObject<GetAllContacts>(getContacts);
                        AllCurrentTrades = contactInfosJson;
                        newContactNumberonThelist = false;
                    }
                    catch (Exception ep)
                    {
                        ErrorLogging(ep);
                    }
                }
                ContactidString.Clear();
            }
        }

        private void ButtonNotifications_Click(object sender, RoutedEventArgs e)
        {
            if (ListBoxNotifications.Visibility == Visibility.Collapsed)
            {
                ListBoxNotifications.Visibility = Visibility.Visible;
            }
            else
            {
                ListBoxNotifications.Visibility = Visibility.Collapsed;
            }
        }

        async private void ListBoxNotifications_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ListBoxNotifications.Visibility = Visibility.Collapsed;
            try
            {
                if (ListBoxNotifications.Items.Count != 0)
                {
                    await Task.Run(() => MarkNotificationsAsread());
                }
            }
            catch (Exception ep)
            {
                ErrorLogging(ep);
            }
        }

        private void MarkNotificationsAsread()
        {
            var APiKey = ConfigurationManager.AppSettings["ApiKeyMarkNotifications"];
            var ApiSecret = ConfigurationManager.AppSettings["ApiSecretMarkNotifications"];
            var client = new LocalBitcoinsAPI(APiKey, ApiSecret);
            object message = null;
            ListBoxNotifications.Dispatcher.Invoke((Action)(() =>
            {
                message = ListBoxNotifications.SelectedValue;
            }));
            if (message != null)
            {
                var notification = dictionary.FirstOrDefault(x => x.Value.Message == message.ToString());
                var notificationId = notification.Key;
                var contactId = notification.Value.contactID;
                if (!string.IsNullOrWhiteSpace(contactId))
                    NewTrade(contactId);
                client.NotificationMarkAsRead(notificationId);
                ListBoxNotifications.Dispatcher.BeginInvoke((Action)(() =>
                {
                    ListBoxNotifications.Items.Remove(message);
                }));
                dictionary.Remove(notification.Key);
                ListBoxNotifications.Dispatcher.BeginInvoke((Action)(() =>
                {
                    SetTaskBarOverlay(ListBoxNotifications.Items.Count);
                }));
            }
        }

        private void ButtonHome_Click(object sender, RoutedEventArgs e)
        {
            tabControlBTC.SelectedItem = HomeTab;
        }

        private void ButtonSettings_Click(object sender, RoutedEventArgs e)
        {
            tabControlBTC.SelectedItem = Settingstab;
        }

        private void CloseTabvoid(Control sender, string tabname)
        {
            //var parent = sender.Parent;
            var nametab = "";
            if (sender == null)
                nametab = tabname;
            else
                nametab = sender.Name;
            TabItem TabExisted = null;
            tabControlBTC.Dispatcher.BeginInvoke((Action)(() =>
            {
                foreach (TabItem item in tabControlBTC.Items)
                {
                    if (item.Name == nametab)
                    {
                        TabExisted = item;
                        break;
                    }
                }
                if (TabExisted != null)
                {
                    tabControlBTC.Items.Remove(TabExisted);
                }
            }));
        }

        /// <summary>
        /// New trade method, insert a Contact number as a string.
        /// </summary>
        /// <param name="ContactID"></param>
        public void NewTrade(string ContactID)
        {
            LocalBitcoinsDatabaseEntities _BaseDados = new LocalBitcoinsDatabaseEntities();
            var contactNumber = ContactID.Replace(" ", "");
            bool tabExists = false;
            TabItem TabExisted = null;
            tabControlBTC.Dispatcher.Invoke((Action)(() =>
            {
                foreach (TabItem item in tabControlBTC.Items)
                {
                    if (item.Name == $"tab{contactNumber}")
                    {
                        tabExists = true;
                        TabExisted = item;
                        break;

                    }
                    else
                        tabExists = false;
                }
            }));
            if (tabExists == true)
            {
                tabControlBTC.Dispatcher.Invoke((() =>
                {
                    tabControlBTC.SelectedItem = TabExisted;
                }));
            }
            else
            {
                if (string.IsNullOrEmpty(contactNumber))
                {
                    MessageBox.Show("Please write a valid contact number");
                }
                else
                {
                    var APiKey = ConfigurationManager.AppSettings["ApiKeyNewTrades"];
                    var ApiSecret = ConfigurationManager.AppSettings["ApiSecretNewTrades"];
                    var client = new LocalBitcoinsAPI(APiKey, ApiSecret);
                    TabItem newTab = null;
                    RootObject jsonSellorBuy = null;
                    int.TryParse(contactNumber, out int contactIDInt);
                    if (AllCurrentTrades != null)
                    {
                        if (AllCurrentTrades.data.contact_list.Any(x => x.data.contact_id == contactIDInt) == true)
                        {
                            jsonSellorBuy = AllCurrentTrades.data.contact_list.FirstOrDefault(x => x.data.contact_id == contactIDInt);
                        }
                        else
                        {
                            if (!TradesContactidsList.Contains(contactNumber))
                            {
                                TradesContactidsList.Add(contactNumber);
                            }
                            newContactNumberonThelist = true;
                            GetContactTrades();
                            jsonSellorBuy = AllCurrentTrades.data.contact_list.FirstOrDefault(x => x.data.contact_id == contactIDInt);
                        }
                    }
                    else
                    {
                        if (!TradesContactidsList.Contains(contactNumber))
                        {
                            TradesContactidsList.Add(contactNumber);
                        }
                        newContactNumberonThelist = true;
                        GetContactTrades();
                        jsonSellorBuy = AllCurrentTrades.data.contact_list.FirstOrDefault(x => x.data.contact_id == contactIDInt);
                    }
                    if (jsonSellorBuy == null)
                        throw new System.ArgumentException("Contact info cant be null!");
                    var advertiser = jsonSellorBuy.data.advertisement.advertiser;
                    var SellorBuy = jsonSellorBuy.data.advertisement.trade_type;
                    var currency = jsonSellorBuy.data.currency;
                    var feebtcAmount = jsonSellorBuy.data.fee_btc;
                    var btcTradeAmount = "";
                    if (feebtcAmount == null)
                    {
                        // do something
                    }
                    else
                    {
                        decimal.TryParse(jsonSellorBuy.data.amount_btc, out var btcAmount);
                        decimal.TryParse(feebtcAmount, out var FeesBTC);
                        var btcTotalAmount = btcAmount - FeesBTC;
                        btcTradeAmount = btcTotalAmount.ToString();
                    }
                    if (advertiser.username == "niceonne")
                    {
                        if (SellorBuy == "ONLINE_SELL")
                        {
                            var username = jsonSellorBuy.data.buyer.username;
                            var totalAmount = jsonSellorBuy.data.amount;
                            var isPaypal = jsonSellorBuy.data.advertisement.payment_method;
                            Grid grid = null;
                            this.Dispatcher.BeginInvoke((Action)(() => 
                            {
                                newTab = new TabItem();
                                newTab.Name = "tab" + contactNumber;

                                PathGeometry CircleGeometry = new PathGeometry();
                                CircleGeometry.FillRule = FillRule.Nonzero;
                                PathFigureCollectionConverter pfcc = new PathFigureCollectionConverter();
                                CircleGeometry.Figures = (PathFigureCollection)pfcc.ConvertFrom("M12,2A10,10 0 0,0 2,12A10,10 0 0,0 12,22A10,10 0 0,0 22,12A10,10 0 0,0 12,2Z");
                                System.Windows.Shapes.Path PathCirle = new System.Windows.Shapes.Path();
                                PathCirle.Fill = randomColor2();
                                PathCirle.Data = CircleGeometry;

                                var CanvasCirle = new Canvas() { Width = 24, Height = 24 };

                                TextBlock textBlockCircle = new TextBlock();
                                textBlockCircle.Text = username[0].ToString().ToUpper();
                                textBlockCircle.Foreground = new SolidColorBrush(Colors.Black);
                                textBlockCircle.Margin = new Thickness(0, 3, 0, 3);
                                textBlockCircle.FontWeight = FontWeights.DemiBold;
                                textBlockCircle.TextAlignment = TextAlignment.Center;
                                textBlockCircle.Width = CanvasCirle.Width;
                                textBlockCircle.HorizontalAlignment = HorizontalAlignment.Center;

                                CanvasCirle.Children.Add(PathCirle);
                                CanvasCirle.Children.Add(textBlockCircle);

                                var viewBoxCircle = new Viewbox() { Width = 32, Height = 32, Margin = new Thickness(0, 0, 5, 0) };
                                viewBoxCircle.Child = CanvasCirle;

                                var stackPanel = new StackPanel() { Orientation = Orientation.Horizontal, Margin = new Thickness(-5, 0, 0, 0) };

                                var Buttoncircle = new Button() { Background = Brushes.Transparent, HorizontalAlignment = HorizontalAlignment.Right, BorderThickness = new Thickness(0) };
                                PathGeometry ButtonGeometry = new PathGeometry();
                                ButtonGeometry.FillRule = FillRule.Nonzero;
                                PathFigureCollectionConverter pfccc = new PathFigureCollectionConverter();
                                ButtonGeometry.Figures = (PathFigureCollection)pfccc.ConvertFrom("M13.46,12L19,17.54V19H17.54L12,13.46L6.46,19H5V17.54L10.54,12L5,6.46V5H6.46L12,10.54L17.54,5H19V6.46L13.46,12Z");
                                System.Windows.Shapes.Path PathCirleButton = new System.Windows.Shapes.Path();
                                PathCirleButton.Fill = Brushes.Red;
                                PathCirleButton.Data = ButtonGeometry;
                                Buttoncircle.Name = $"tab{contactNumber}";

                                var CanvasCirleButton = new Canvas() { Width = 24, Height = 24 };
                                CanvasCirleButton.Children.Add(PathCirleButton);

                                var viewBoxCircleButton = new Viewbox() { Width = 15 };
                                viewBoxCircleButton.Child = CanvasCirleButton;
                                Buttoncircle.Content = viewBoxCircleButton;
                                Buttoncircle.Click += (s, e) => { CloseTabvoid(Buttoncircle, null); };

                                var stack = new StackPanel() { VerticalAlignment = VerticalAlignment.Top, Margin = new Thickness(0, -1, 0, 0)};
                                stack.Children.Add(new TextBlock() { Text = username, FontSize = 15 });
                                stack.Children.Add(new TextBlock() { Text = $"Trade #{contactNumber}", FontSize = 11 });
                                stackPanel.Children.Add(viewBoxCircle);
                                stackPanel.Children.Add(stack);

                                grid = new Grid();
                                grid.Children.Add(stackPanel);
                                grid.Children.Add(Buttoncircle);
                            }));
                            if (isPaypal == "PAYPAL")
                            {
                                bool UserExists = _BaseDados.Usernames.Any(x => x.Username1 == username);
                                if (UserExists == true)
                                {
                                    Username UserData = _BaseDados.Usernames.FirstOrDefault(x => x.Username1 == username);
                                    if (UserData.UsernameInfo.Verified == true) // Sell Return costumer
                                    {
                                        tabControlBTC.Dispatcher.BeginInvoke((Action)(() =>
                                        {
                                            UserControltest1 NewUserControlPage = new UserControltest1(contactNumber, username, jsonSellorBuy, 2);
                                            newTab.Content = NewUserControlPage;
                                            newTab.Header = grid;
                                            tabControlBTC.Items.Insert(1, newTab);
                                        }));
                                    }
                                    else if (UserData.UsernameInfo.Verified == false) // Sell New user - Verificação talvez seja necessaria
                                    {
                                        tabControlBTC.Dispatcher.BeginInvoke((Action)(() =>
                                        {
                                            UserControltest1 NewUserControlPage = new UserControltest1(contactNumber, username, jsonSellorBuy, 1);
                                            newTab.Content = NewUserControlPage;
                                            newTab.Header = grid;
                                            tabControlBTC.Items.Insert(1, newTab);
                                        }));
                                    }
                                }
                                else // Sell New user - Verificação necessária
                                {
                                    tabControlBTC.Dispatcher.BeginInvoke((Action)(() =>
                                    {
                                        UserControltest1 NewUserControlPage = new UserControltest1(contactNumber, username, jsonSellorBuy, 1);
                                        newTab.Content = NewUserControlPage;
                                        newTab.Header = grid;
                                        tabControlBTC.Items.Insert(1, newTab);
                                    }));
                                }
                            }
                            else if (isPaypal != "PAYPAL")
                            {
                                tabControlBTC.Dispatcher.BeginInvoke((Action)(() =>
                                {
                                    UserControltest1 NewUserControlPage = new UserControltest1(contactNumber, username, jsonSellorBuy, 4);
                                    newTab.Content = NewUserControlPage;
                                    newTab.Header = grid;
                                    tabControlBTC.Items.Insert(1, newTab);
                                }));
                            }
                        }
                        else if (SellorBuy == "ONLINE_BUY")
                        {
                            var isPaypal = jsonSellorBuy.data.advertisement.payment_method;
                            if (isPaypal == "PAYPAL")
                            {
                                var username = jsonSellorBuy.data.seller.username;
                                tabControlBTC.Dispatcher.BeginInvoke((Action)(() =>
                                {
                                    UserControltest1 NewUserControlPage = new UserControltest1(contactNumber, username, jsonSellorBuy, 3);
                                    newTab = new TabItem();
                                    newTab.Name = "tab" + contactNumber;

                                    PathGeometry CircleGeometry = new PathGeometry();
                                    CircleGeometry.FillRule = FillRule.Nonzero;
                                    PathFigureCollectionConverter pfcc = new PathFigureCollectionConverter();
                                    CircleGeometry.Figures = (PathFigureCollection)pfcc.ConvertFrom("M12,2A10,10 0 0,0 2,12A10,10 0 0,0 12,22A10,10 0 0,0 22,12A10,10 0 0,0 12,2Z");
                                    System.Windows.Shapes.Path PathCirle = new System.Windows.Shapes.Path();
                                    PathCirle.Fill = randomColor2();
                                    PathCirle.Data = CircleGeometry;

                                    var CanvasCirle = new Canvas() { Width = 24, Height = 24 };

                                    TextBlock textBlockCircle = new TextBlock();
                                    textBlockCircle.Text = username[0].ToString().ToUpper();
                                    textBlockCircle.Foreground = new SolidColorBrush(Colors.Black);
                                    textBlockCircle.Margin = new Thickness(0, 3, 0, 3);
                                    textBlockCircle.FontWeight = FontWeights.DemiBold;
                                    textBlockCircle.TextAlignment = TextAlignment.Center;
                                    textBlockCircle.Width = CanvasCirle.Width;
                                    textBlockCircle.HorizontalAlignment = HorizontalAlignment.Center;

                                    CanvasCirle.Children.Add(PathCirle);
                                    CanvasCirle.Children.Add(textBlockCircle);

                                    var viewBoxCircle = new Viewbox() { Width = 32, Height = 32, Margin = new Thickness(0, 0, 5, 0) };
                                    viewBoxCircle.Child = CanvasCirle;

                                    var stackPanel = new StackPanel() { Orientation = Orientation.Horizontal, Margin = new Thickness(-5, 0, 0, 0) };

                                    var Buttoncircle = new Button() { Background = Brushes.Transparent, HorizontalAlignment = HorizontalAlignment.Right, BorderThickness = new Thickness(0) };
                                    PathGeometry ButtonGeometry = new PathGeometry();
                                    ButtonGeometry.FillRule = FillRule.Nonzero;
                                    PathFigureCollectionConverter pfccc = new PathFigureCollectionConverter();
                                    ButtonGeometry.Figures = (PathFigureCollection)pfccc.ConvertFrom("M13.46,12L19,17.54V19H17.54L12,13.46L6.46,19H5V17.54L10.54,12L5,6.46V5H6.46L12,10.54L17.54,5H19V6.46L13.46,12Z");
                                    System.Windows.Shapes.Path PathCirleButton = new System.Windows.Shapes.Path();
                                    PathCirleButton.Fill = Brushes.Red;
                                    PathCirleButton.Data = ButtonGeometry;
                                    Buttoncircle.Name = $"tab{contactNumber}";

                                    var CanvasCirleButton = new Canvas() { Width = 24, Height = 24 };
                                    CanvasCirleButton.Children.Add(PathCirleButton);

                                    var viewBoxCircleButton = new Viewbox() { Width = 15 };
                                    viewBoxCircleButton.Child = CanvasCirleButton;
                                    Buttoncircle.Content = viewBoxCircleButton;
                                    Buttoncircle.Click += (s, e) => { CloseTabvoid(Buttoncircle, null); };

                                    var stack = new StackPanel() { VerticalAlignment = VerticalAlignment.Top, Margin = new Thickness(0, -1, 0, 0)};
                                    stack.Children.Add(new TextBlock() { Text = username, FontSize = 15 });
                                    stack.Children.Add(new TextBlock() { Text = $"Trade #{contactNumber}", FontSize = 11 });
                                    stackPanel.Children.Add(viewBoxCircle);
                                    stackPanel.Children.Add(stack);

                                    var grid = new Grid();
                                    grid.Children.Add(stackPanel);
                                    grid.Children.Add(Buttoncircle);

                                    newTab.Content = NewUserControlPage;
                                    newTab.Header = grid;
                                    tabControlBTC.Items.Insert(1, newTab);
                                    tabControlBTC.SelectedItem = NewUserControlPage;
                                }));
                            }
                            else
                            {
                                var username = jsonSellorBuy.data.seller.username;
                                tabControlBTC.Dispatcher.BeginInvoke((Action)(() =>
                                {
                                    UserControltest1 NewUserControlPage = new UserControltest1(contactNumber, username, jsonSellorBuy, 5);
                                    newTab = new TabItem();
                                    newTab.Name = "tab" + contactNumber;

                                    PathGeometry CircleGeometry = new PathGeometry();
                                    CircleGeometry.FillRule = FillRule.Nonzero;
                                    PathFigureCollectionConverter pfcc = new PathFigureCollectionConverter();
                                    CircleGeometry.Figures = (PathFigureCollection)pfcc.ConvertFrom("M12,2A10,10 0 0,0 2,12A10,10 0 0,0 12,22A10,10 0 0,0 22,12A10,10 0 0,0 12,2Z");
                                    System.Windows.Shapes.Path PathCirle = new System.Windows.Shapes.Path();
                                    PathCirle.Fill = randomColor2();
                                    PathCirle.Data = CircleGeometry;

                                    var CanvasCirle = new Canvas() { Width = 24, Height = 24 };

                                    TextBlock textBlockCircle = new TextBlock();
                                    textBlockCircle.Text = username[0].ToString().ToUpper();
                                    textBlockCircle.Foreground = new SolidColorBrush(Colors.Black);
                                    textBlockCircle.Margin = new Thickness(0, 3, 0, 3);
                                    textBlockCircle.FontWeight = FontWeights.DemiBold;
                                    textBlockCircle.TextAlignment = TextAlignment.Center;
                                    textBlockCircle.Width = CanvasCirle.Width;
                                    textBlockCircle.HorizontalAlignment = HorizontalAlignment.Center;

                                    CanvasCirle.Children.Add(PathCirle);
                                    CanvasCirle.Children.Add(textBlockCircle);

                                    var viewBoxCircle = new Viewbox() { Width = 32, Height = 32, Margin = new Thickness(0, 0, 5, 0) };
                                    viewBoxCircle.Child = CanvasCirle;

                                    var stackPanel = new StackPanel() { Orientation = Orientation.Horizontal, Margin = new Thickness(-5, 0, 0, 0) };

                                    var Buttoncircle = new Button() { Background = Brushes.Transparent, HorizontalAlignment = HorizontalAlignment.Right, BorderThickness = new Thickness(0) };
                                    PathGeometry ButtonGeometry = new PathGeometry();
                                    ButtonGeometry.FillRule = FillRule.Nonzero;
                                    PathFigureCollectionConverter pfccc = new PathFigureCollectionConverter();
                                    ButtonGeometry.Figures = (PathFigureCollection)pfccc.ConvertFrom("M13.46,12L19,17.54V19H17.54L12,13.46L6.46,19H5V17.54L10.54,12L5,6.46V5H6.46L12,10.54L17.54,5H19V6.46L13.46,12Z");
                                    System.Windows.Shapes.Path PathCirleButton = new System.Windows.Shapes.Path();
                                    PathCirleButton.Fill = Brushes.Red;
                                    PathCirleButton.Data = ButtonGeometry;
                                    Buttoncircle.Name = $"tab{contactNumber}";

                                    var CanvasCirleButton = new Canvas() { Width = 24, Height = 24 };
                                    CanvasCirleButton.Children.Add(PathCirleButton);

                                    var viewBoxCircleButton = new Viewbox() { Width = 15 };
                                    viewBoxCircleButton.Child = CanvasCirleButton;
                                    Buttoncircle.Content = viewBoxCircleButton;
                                    Buttoncircle.Click += (s, e) => { CloseTabvoid(Buttoncircle, null); };

                                    var stack = new StackPanel() { VerticalAlignment = VerticalAlignment.Top, Margin = new Thickness(0, -1, 0, 0)};
                                    stack.Children.Add(new TextBlock() { Text = username, FontSize = 15 });
                                    stack.Children.Add(new TextBlock() { Text = $"Trade #{contactNumber}", FontSize = 11 });
                                    stackPanel.Children.Add(viewBoxCircle);
                                    stackPanel.Children.Add(stack);

                                    var grid = new Grid();
                                    grid.Children.Add(stackPanel);
                                    grid.Children.Add(Buttoncircle);

                                    newTab.Content = NewUserControlPage;
                                    newTab.Header = grid;
                                    tabControlBTC.Items.Insert(1, newTab);
                                    tabControlBTC.SelectedItem = NewUserControlPage;
                                }));
                            }
                        }
                    }
                    else
                    {
                        if (SellorBuy == "ONLINE_SELL")
                        {
                            var username = jsonSellorBuy.data.seller.username;
                            tabControlBTC.Dispatcher.BeginInvoke((Action)(() =>
                            {
                                UserControltest1 NewUserControlPage = new UserControltest1(contactNumber, username, jsonSellorBuy, 5);
                                newTab = new TabItem();
                                newTab.Name = "tab" + contactNumber;

                                PathGeometry CircleGeometry = new PathGeometry();
                                CircleGeometry.FillRule = FillRule.Nonzero;
                                PathFigureCollectionConverter pfcc = new PathFigureCollectionConverter();
                                CircleGeometry.Figures = (PathFigureCollection)pfcc.ConvertFrom("M12,2A10,10 0 0,0 2,12A10,10 0 0,0 12,22A10,10 0 0,0 22,12A10,10 0 0,0 12,2Z");
                                System.Windows.Shapes.Path PathCirle = new System.Windows.Shapes.Path();
                                PathCirle.Fill = randomColor2();
                                PathCirle.Data = CircleGeometry;

                                var CanvasCirle = new Canvas() { Width = 24, Height = 24 };

                                TextBlock textBlockCircle = new TextBlock();
                                textBlockCircle.Text = username[0].ToString().ToUpper();
                                textBlockCircle.Foreground = new SolidColorBrush(Colors.Black);
                                textBlockCircle.Margin = new Thickness(0, 3, 0, 3);
                                textBlockCircle.FontWeight = FontWeights.DemiBold;
                                textBlockCircle.TextAlignment = TextAlignment.Center;
                                textBlockCircle.Width = CanvasCirle.Width;
                                textBlockCircle.HorizontalAlignment = HorizontalAlignment.Center;

                                CanvasCirle.Children.Add(PathCirle);
                                CanvasCirle.Children.Add(textBlockCircle);

                                var viewBoxCircle = new Viewbox() { Width = 32, Height = 32, Margin = new Thickness(0, 0, 5, 0) };
                                viewBoxCircle.Child = CanvasCirle;

                                var stackPanel = new StackPanel() { Orientation = Orientation.Horizontal, Margin = new Thickness(-5, 0, 0, 0) };

                                var Buttoncircle = new Button() { Background = Brushes.Transparent, HorizontalAlignment = HorizontalAlignment.Right, BorderThickness = new Thickness(0) };
                                PathGeometry ButtonGeometry = new PathGeometry();
                                ButtonGeometry.FillRule = FillRule.Nonzero;
                                PathFigureCollectionConverter pfccc = new PathFigureCollectionConverter();
                                ButtonGeometry.Figures = (PathFigureCollection)pfccc.ConvertFrom("M13.46,12L19,17.54V19H17.54L12,13.46L6.46,19H5V17.54L10.54,12L5,6.46V5H6.46L12,10.54L17.54,5H19V6.46L13.46,12Z");
                                System.Windows.Shapes.Path PathCirleButton = new System.Windows.Shapes.Path();
                                PathCirleButton.Fill = Brushes.Red;
                                PathCirleButton.Data = ButtonGeometry;
                                Buttoncircle.Name = $"tab{contactNumber}";

                                var CanvasCirleButton = new Canvas() { Width = 24, Height = 24 };
                                CanvasCirleButton.Children.Add(PathCirleButton);

                                var viewBoxCircleButton = new Viewbox() { Width = 15 };
                                viewBoxCircleButton.Child = CanvasCirleButton;
                                Buttoncircle.Content = viewBoxCircleButton;
                                Buttoncircle.Click += (s, e) => { CloseTabvoid(Buttoncircle, null); };

                                var stack = new StackPanel() { VerticalAlignment = VerticalAlignment.Top, Margin = new Thickness(0, -1, 0, 0)};
                                stack.Children.Add(new TextBlock() { Text = username, FontSize = 15 });
                                stack.Children.Add(new TextBlock() { Text = $"Trade #{contactNumber}", FontSize = 11 });
                                stackPanel.Children.Add(viewBoxCircle);
                                stackPanel.Children.Add(stack);

                                var grid = new Grid();
                                grid.Children.Add(stackPanel);
                                grid.Children.Add(Buttoncircle);

                                newTab.Content = NewUserControlPage;
                                newTab.Header = grid;
                                tabControlBTC.Items.Insert(1, newTab);
                                tabControlBTC.SelectedItem = NewUserControlPage;
                            }));
                        }
                        else if (SellorBuy == "ONLINE_BUY")
                        {
                            var username = jsonSellorBuy.data.buyer.username;
                            tabControlBTC.Dispatcher.BeginInvoke((Action)(() =>
                            {
                                UserControltest1 NewUserControlPage = new UserControltest1(contactNumber, username, jsonSellorBuy, 4);
                                newTab = new TabItem();
                                newTab.Name = "tab" + contactNumber;

                                PathGeometry CircleGeometry = new PathGeometry();
                                CircleGeometry.FillRule = FillRule.Nonzero;
                                PathFigureCollectionConverter pfcc = new PathFigureCollectionConverter();
                                CircleGeometry.Figures = (PathFigureCollection)pfcc.ConvertFrom("M12,2A10,10 0 0,0 2,12A10,10 0 0,0 12,22A10,10 0 0,0 22,12A10,10 0 0,0 12,2Z");
                                System.Windows.Shapes.Path PathCirle = new System.Windows.Shapes.Path();
                                PathCirle.Fill = randomColor2();
                                PathCirle.Data = CircleGeometry;

                                var CanvasCirle = new Canvas() { Width = 24, Height = 24 };

                                TextBlock textBlockCircle = new TextBlock();
                                textBlockCircle.Text = username[0].ToString().ToUpper();
                                textBlockCircle.Foreground = new SolidColorBrush(Colors.Black);
                                textBlockCircle.Margin = new Thickness(0, 3, 0, 3);
                                textBlockCircle.FontWeight = FontWeights.DemiBold;
                                textBlockCircle.TextAlignment = TextAlignment.Center;
                                textBlockCircle.Width = CanvasCirle.Width;
                                textBlockCircle.HorizontalAlignment = HorizontalAlignment.Center;

                                CanvasCirle.Children.Add(PathCirle);
                                CanvasCirle.Children.Add(textBlockCircle);

                                var viewBoxCircle = new Viewbox() { Width = 32, Height = 32, Margin = new Thickness(0, 0, 5, 0) };
                                viewBoxCircle.Child = CanvasCirle;

                                var stackPanel = new StackPanel() { Orientation = Orientation.Horizontal, Margin = new Thickness(-5, 0, 0, 0) };

                                var Buttoncircle = new Button() { Background = Brushes.Transparent, HorizontalAlignment = HorizontalAlignment.Right, BorderThickness = new Thickness(0) };
                                PathGeometry ButtonGeometry = new PathGeometry();
                                ButtonGeometry.FillRule = FillRule.Nonzero;
                                PathFigureCollectionConverter pfccc = new PathFigureCollectionConverter();
                                ButtonGeometry.Figures = (PathFigureCollection)pfccc.ConvertFrom("M13.46,12L19,17.54V19H17.54L12,13.46L6.46,19H5V17.54L10.54,12L5,6.46V5H6.46L12,10.54L17.54,5H19V6.46L13.46,12Z");
                                System.Windows.Shapes.Path PathCirleButton = new System.Windows.Shapes.Path();
                                PathCirleButton.Fill = Brushes.Red;
                                PathCirleButton.Data = ButtonGeometry;
                                Buttoncircle.Name = $"tab{contactNumber}";

                                var CanvasCirleButton = new Canvas() { Width = 24, Height = 24 };
                                CanvasCirleButton.Children.Add(PathCirleButton);

                                var viewBoxCircleButton = new Viewbox() { Width = 15 };
                                viewBoxCircleButton.Child = CanvasCirleButton;
                                Buttoncircle.Content = viewBoxCircleButton;
                                Buttoncircle.Click += (s, e) => { CloseTabvoid(Buttoncircle, null); };

                                var stack = new StackPanel() { VerticalAlignment = VerticalAlignment.Top, Margin = new Thickness(0, -1, 0, 0)};
                                stack.Children.Add(new TextBlock() { Text = username, FontSize = 15 });
                                stack.Children.Add(new TextBlock() { Text = $"Trade #{contactNumber}", FontSize = 11 });
                                stackPanel.Children.Add(viewBoxCircle);
                                stackPanel.Children.Add(stack);

                                var grid = new Grid();
                                grid.Children.Add(stackPanel);
                                grid.Children.Add(Buttoncircle);

                                newTab.Content = NewUserControlPage;
                                newTab.Header = grid;
                                tabControlBTC.Items.Insert(1, newTab);
                                tabControlBTC.SelectedItem = NewUserControlPage;
                            }));
                        }
                    }
                    var tabname = $"tab{contactNumber}";
                }
            }
        }

        private Random r = new Random();

        private System.Windows.Media.Brush randomColor2()
        {
            System.Windows.Media.Brush brush = new SolidColorBrush(System.Windows.Media.Color.FromRgb((byte)r.Next(1, 255), (byte)r.Next(1, 255), (byte)r.Next(1, 233)));
            return brush;
        }

        public static void ErrorLogging(Exception ExceptionError)
        {
            //await Task.Run(() => SaveError(ExceptionError));
        }

        private static void SaveError(Exception Exception)
        {
            string strPath = directoryPath + "Log.txt";
            if (!File.Exists(strPath))
            {
                File.Create(strPath).Dispose();
            }
            using (StreamWriter sw = File.AppendText(strPath))
            {
                sw.WriteLine("=============Error Logging ===========");
                sw.WriteLine("===========Start============= " + DateTime.Now);
                sw.WriteLine("Error Message: " + Exception.Message);
                sw.WriteLine("Stack Trace: " + Exception.StackTrace);
                sw.WriteLine("===========End============= " + DateTime.Now);
            }
        }

        async private void ButtonHomeViewTrade_Click(object sender, RoutedEventArgs e)
        {
            SpinnerButtonViewTrade.Visibility = Visibility.Visible;
            WarnButtonViewTrade.Visibility = Visibility.Collapsed;
            var idtrade = TextBoxHomeNewTrade.Text;
            var contactid = idtrade.Replace(" ", "");
            bool IsDigitsOnly = true;
            foreach (char c in contactid)
            {
                if (!char.IsDigit(c))
                    IsDigitsOnly = false;
                else
                    IsDigitsOnly = true;
            }
            if (IsDigitsOnly == true)
            {
                try
                {
                    await Task.Run(() => NewTrade(contactid));
                    TextBoxHomeNewTrade.Text = "";
                }
                catch (Exception ep)
                {
                    ErrorLogging(ep);
                    WarnButtonViewTrade.Visibility = Visibility.Visible;
                    TextBlockWarningViewTrade.Text = "Trade not found or error! Check log!";
                }
            }
            else
            {
                WarnButtonViewTrade.Visibility = Visibility.Visible;
                TextBlockWarningViewTrade.Text = "Enter a valid contact number!";
            }
            SpinnerButtonViewTrade.Visibility = Visibility.Collapsed;
        }

        async private void PanelHome_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (PanelHome.IsVisible == true)
            {
                try
                {
                    await Task.Run(() => Fillingdatagrid());
                }            
                catch (Exception ep)
                {
                    ErrorLogging(ep);
                }
            }
        }

        private void Fillingdatagrid()
        {
            var APiKey = ConfigurationManager.AppSettings["ApiKeyNewTrades"];
            var ApiSecret = ConfigurationManager.AppSettings["ApiSecretNewTrades"];
            var client = new LocalBitcoinsAPI(APiKey, ApiSecret);
            //var getallDashboard = client.GetDashboard();
            //GetDashboard.RootDashboard alltradesDashboard = JsonConvert.DeserializeObject<GetDashboard.RootDashboard>(getallDashboard);
            this.Dispatcher.BeginInvoke((Action)(() =>
            {
                 myDataGridSelling.Dispatcher.BeginInvoke((Action)(() =>
                 {
                     myDataGridSelling.Items.Clear();
                 }));
                 myDataGridBuying.Dispatcher.BeginInvoke((Action)(() =>
                 {
                     myDataGridBuying.Items.Clear();
                 }));
            }));
            if (AllCurrentTrades != null)
            {
                List<RootObject> AllTrades = null;
                this.Dispatcher.Invoke(() =>
                {
                    AllTrades = AllCurrentTrades.data.contact_list;
                });
                decimal totalBtc = 0;
                foreach (var item in AllTrades)
                {
                    if (item.data.is_buying == true)
                    {
                        var Statustrade = "";
                        if (item.data.payment_completed_at != null && item.data.canceled_at == null && item.data.released_at == null)
                            Statustrade = "Payment completed";
                        else if (item.data.canceled_at != null)
                            Statustrade = "Canceled!";
                        else if (item.data.closed_at != null && item.data.canceled_at == null && item.data.released_at == null)
                            Statustrade = "Canceled!";
                        else if (item.data.released_at != null)
                            Statustrade = "Released!";
                        else
                            Statustrade = "Waiting Payment";
                        var newCostumer = new TableItemscs
                        {
                            Nickname = item.data.seller.username,
                            Amount = item.data.amount,
                            AmountBtc = item.data.amount_btc,
                            CreatedAt = item.data.created_at.ToString(),
                            ContactNumber = item.data.contact_id.ToString(),
                            PaymentMethod = item.data.advertisement.payment_method,
                            Status = Statustrade
                        };
                        this.Dispatcher.BeginInvoke((Action)(() =>
                        {
                            myDataGridBuying.Dispatcher.BeginInvoke((Action)(() =>
                            {
                                myDataGridBuying.Items.Add(newCostumer);
                            }));
                        }));
                    }
                    else
                    {
                        var Statustrade = "";
                        if (item.data.payment_completed_at != null && item.data.canceled_at == null && item.data.released_at == null)
                            Statustrade = "Payment completed";
                        else if (item.data.canceled_at != null)
                            Statustrade = "Canceled!";
                        else if (item.data.closed_at != null && item.data.canceled_at == null && item.data.released_at == null)
                            Statustrade = "Canceled!";
                        else if (item.data.released_at != null)
                            Statustrade = "Released!";
                        else
                            Statustrade = "Waiting Payment";
                        var newCostumer = new TableItemscs
                        {
                            Nickname = item.data.buyer.username.ToString(),
                            Amount = item.data.amount.ToString(),
                            AmountBtc = item.data.amount_btc.ToString(),
                            CreatedAt = item.data.created_at.ToString(),
                            ContactNumber = item.data.contact_id.ToString(),
                            PaymentMethod = item.data.advertisement.payment_method.ToString(),
                            Status = Statustrade
                        };
                        this.Dispatcher.BeginInvoke((Action)(() =>
                        {
                            myDataGridSelling.Dispatcher.BeginInvoke((Action)(() =>
                            {
                                myDataGridSelling.Items.Add(newCostumer);
                            }));
                        }));
                    }
                    decimal.TryParse(item.data.amount_btc, out decimal itemAmountBtc);
                    decimal.TryParse(item.data.fee_btc, out decimal itemAmountfeeBTC);
                    totalBtc = totalBtc + itemAmountBtc + itemAmountfeeBTC;
                }
                this.Dispatcher.BeginInvoke((Action)(() =>
                {
                    TextBlocktotalbitcoinsunderescrow.Dispatcher.BeginInvoke((Action)(() =>
                    {
                        TextBlocktotalbitcoinsunderescrow.Text = $"Your total amount of bitcoins under escrow is {totalBtc} BTC";
                    }));
                }));
            }
            if (AdsUpdated == false)
            {
                myDataGridAds.Dispatcher.BeginInvoke((Action)(() =>
                {
                    myDataGridAds.Items.Clear();
                }));
                var ads = client.GetOwnAds();
                AdsData myAds = JsonConvert.DeserializeObject<AdsData>(ads);
                if (myAds != null)
                {
                    foreach (var item in myAds.data.ad_list)
                    {
                        string Advisibilty = "Disabled";
                        if (item.data.visible == true)
                            Advisibilty = "Active";
                        var newRow = new TableAds
                        {
                            Adid = item.data.ad_id.ToString(),
                            AdStatus = Advisibilty,
                            AdEquation = item.data.price_equation,
                            AdInfo = item.data.trade_type.ToString() + " " + item.data.online_provider.ToString(),
                            AdPrice = item.data.temp_price.ToString() + item.data.currency.ToString(),
                            adCreatedat = item.data.created_at.ToString()
                        };
                        this.Dispatcher.BeginInvoke((Action)(() =>
                        {
                                myDataGridAds.Dispatcher.BeginInvoke((Action)(() =>
                            {
                                myDataGridAds.Items.Add(newRow);
                            }));
                            AdsUpdated = true;
                        }));
                    }
                }
            }
        }

        async private void DG_Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            Hyperlink link = (Hyperlink)e.OriginalSource;
            string contactid = link.NavigateUri.ToString();
            await Task.Run(() => NewTrade(contactid));
        }

        public void ConfigSettings(string key, string value)
        {
            var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var settings = configFile.AppSettings.Settings;
            if (settings[key] == null)
            {
                settings.Add(key, value);
            }
            else
            {
                settings[key].Value = value;
            }
            configFile.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
        }

        private void PanelSettingsPage_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (PanelSettingsPage.Visibility == Visibility.Visible)
                UpdatingAPikeys();
            bool MissingApis = ApiKeysCheckMissing();
            if (MissingApis == true)
                WarnButtonSettingsPanel.Visibility = Visibility.Visible;
        }

        private void UpdatingAPikeys()
        {
            var ApiNotificationskey = ConfigurationManager.AppSettings["ApikeyNotifications"];
            var ApiNotificationsSecret = ConfigurationManager.AppSettings["ApiSecretNotifications"];
            var ApikeyGobalMessages = ConfigurationManager.AppSettings["ApikeyGobalMessages"];
            var ApiSecretGobalMessages = ConfigurationManager.AppSettings["ApiSecretGobalMessages"];
            var ApiMarkNotificationsKey = ConfigurationManager.AppSettings["ApiKeyMarkNotifications"];
            var ApiMarkNotificationsSecret = ConfigurationManager.AppSettings["ApiSecretMarkNotifications"];
            var ApiKeyNewTrades = ConfigurationManager.AppSettings["ApiKeyNewTrades"];
            var ApiSecretNewTrades = ConfigurationManager.AppSettings["ApiSecretNewTrades"];
            var ApiKeyMessages = ConfigurationManager.AppSettings["ApiKeyMessages"];
            var ApiSecretMessages = ConfigurationManager.AppSettings["ApiSecretMessages"];
            var ApiKeyChatMessages = ConfigurationManager.AppSettings["ApiKeyChatMessages"];
            var ApiSecretChatMessages = ConfigurationManager.AppSettings["ApiSecretChatMessages"];
            var ApiKeyMarkUserRealName = ConfigurationManager.AppSettings["ApiKeyMarkUserRealName"];
            var ApiSecretMarkUserRealName = ConfigurationManager.AppSettings["ApiSecretMarkUserRealName"];
            var ApiKeyMarkUserVerified = ConfigurationManager.AppSettings["ApiKeyMarkUserVerified"];
            var ApiSecretMarkUserVerified = ConfigurationManager.AppSettings["ApiSecretMarkUserVerified"];
            var ApiKeyReleaseBitcoins = ConfigurationManager.AppSettings["ApiKeyReleaseBitcoins"];
            var ApiSecretReleaseBitcoins = ConfigurationManager.AppSettings["ApiSecretReleaseBitcoins"];
            var DiretoryPath = ConfigurationManager.AppSettings["DirectoryPath"];

            if (string.IsNullOrWhiteSpace(ApiNotificationskey) || string.IsNullOrWhiteSpace(ApiNotificationsSecret))
            {
                if (string.IsNullOrWhiteSpace(ApiNotificationskey) && string.IsNullOrWhiteSpace(ApiNotificationsSecret))
                {
                    WarnNotificationsSecret.Visibility = Visibility.Visible;
                    WarnNotificationsKey.Visibility = Visibility.Visible;
                }

                if (string.IsNullOrWhiteSpace(ApiNotificationskey))
                    WarnNotificationsKey.Visibility = Visibility.Visible;
                else if (string.IsNullOrWhiteSpace(ApiNotificationsSecret))
                    WarnNotificationsSecret.Visibility = Visibility.Visible;
            }
            else
            {
                TextBoxGlobalMessagesKey.Text = ApikeyGobalMessages;
                TextBoxGlobalMessagesSecret.Text = ApiSecretGobalMessages;
            }
            if (string.IsNullOrWhiteSpace(ApikeyGobalMessages) || string.IsNullOrWhiteSpace(ApiSecretGobalMessages))
            {
                if (string.IsNullOrWhiteSpace(ApikeyGobalMessages) && string.IsNullOrWhiteSpace(ApiSecretGobalMessages))
                {
                    WarnGlobalMessagesKey.Visibility = Visibility.Visible;
                    WarnGlobalMessagesSecret.Visibility = Visibility.Visible;
                }
                if (string.IsNullOrWhiteSpace(ApikeyGobalMessages))
                    WarnGlobalMessagesKey.Visibility = Visibility.Visible;
                else if (string.IsNullOrWhiteSpace(ApiSecretGobalMessages))
                    WarnGlobalMessagesSecret.Visibility = Visibility.Visible;
            }
            else
            {
                TextBoxNotificationsKey.Text = ApiNotificationskey;
                TextBoxNotificationsSecret.Text = ApiNotificationsSecret;
            }
            if (string.IsNullOrWhiteSpace(ApiMarkNotificationsKey) || string.IsNullOrWhiteSpace(ApiMarkNotificationsSecret))
            {
                if (string.IsNullOrWhiteSpace(ApiMarkNotificationsKey) && string.IsNullOrWhiteSpace(ApiMarkNotificationsSecret))
                {
                    WarnNotificationsReadingKey.Visibility = Visibility.Visible;
                    WarnNotificationsReadingSecret.Visibility = Visibility.Visible;
                }
                if (string.IsNullOrWhiteSpace(ApiMarkNotificationsKey))
                    WarnNotificationsReadingKey.Visibility = Visibility.Visible;
                else if (string.IsNullOrWhiteSpace(ApiMarkNotificationsSecret))
                    WarnNotificationsReadingSecret.Visibility = Visibility.Visible;
            }
            else
            {
                TextBoxNotificationsReadingKey.Text = ApiMarkNotificationsKey;
                TextBoxNotificationsReadingSecret.Text = ApiMarkNotificationsSecret;
            }
            if (string.IsNullOrWhiteSpace(ApiKeyNewTrades) || string.IsNullOrWhiteSpace(ApiSecretNewTrades))
            {
                if (string.IsNullOrWhiteSpace(ApiKeyNewTrades) && string.IsNullOrWhiteSpace(ApiSecretNewTrades))
                {
                    WarnAccountINfokey.Visibility = Visibility.Visible;
                    WarnAccountINfoSecret.Visibility = Visibility.Visible;
                }
                if (string.IsNullOrWhiteSpace(ApiKeyNewTrades))
                    WarnAccountINfokey.Visibility = Visibility.Visible;
                else if (string.IsNullOrWhiteSpace(ApiSecretNewTrades))
                    WarnAccountINfoSecret.Visibility = Visibility.Visible;
            }
            else
            {
                TextBoxAccountINfoKey.Text = ApiKeyNewTrades;
                TextBoxAccountINfoSecret.Text = ApiSecretNewTrades;
            }
            if (string.IsNullOrWhiteSpace(ApiKeyMessages) || string.IsNullOrWhiteSpace(ApiSecretMessages))
            {
                if (string.IsNullOrWhiteSpace(ApiKeyMessages) && string.IsNullOrWhiteSpace(ApiSecretMessages))
                {
                    WarnMessageskey.Visibility = Visibility.Visible;
                    WarnMessagessecret.Visibility = Visibility.Visible;
                }
                else if (string.IsNullOrWhiteSpace(ApiKeyMessages))
                    WarnMessageskey.Visibility = Visibility.Visible;
                else if (string.IsNullOrWhiteSpace(ApiSecretMessages))
                    WarnMessagessecret.Visibility = Visibility.Visible;
            }
            else
            {
                TextBoxMessagesKeys.Text = ApiKeyMessages;
                TextBoxMessagesSecret.Text = ApiSecretMessages;
            }
            if (string.IsNullOrWhiteSpace(ApiKeyChatMessages) || string.IsNullOrWhiteSpace(ApiSecretChatMessages))
            {
                if (string.IsNullOrWhiteSpace(ApiKeyChatMessages) && string.IsNullOrWhiteSpace(ApiSecretChatMessages))
                {
                    WarnChatMessagesKey.Visibility = Visibility.Visible;
                    WarnChatMessagesSecret.Visibility = Visibility.Visible;
                }
                else if (string.IsNullOrWhiteSpace(ApiKeyChatMessages))
                    WarnChatMessagesKey.Visibility = Visibility.Visible;
                else if (string.IsNullOrWhiteSpace(ApiSecretChatMessages))
                    WarnChatMessagesSecret.Visibility = Visibility.Visible;
            }
            else
            {
                TextBoxChatMessagesKey.Text = ApiKeyChatMessages;
                TextboxChatMessagesSecret.Text = ApiSecretChatMessages;
            }
            if (string.IsNullOrWhiteSpace(ApiKeyMarkUserRealName) || string.IsNullOrWhiteSpace(ApiSecretMarkUserRealName))
            {
                if (string.IsNullOrWhiteSpace(ApiKeyMarkUserRealName) && string.IsNullOrWhiteSpace(ApiSecretMarkUserRealName))
                {
                    WarnRealNameapiKey.Visibility = Visibility.Visible;
                    WarnRealNameapiSecret.Visibility = Visibility.Visible;
                }
                else if (string.IsNullOrWhiteSpace(ApiKeyMarkUserRealName))
                    WarnRealNameapiKey.Visibility = Visibility.Visible;
                else if (string.IsNullOrWhiteSpace(ApiSecretMarkUserRealName))
                    WarnRealNameapiSecret.Visibility = Visibility.Visible;
            }
            else
            {
                TextBoxRealNameapiKeys.Text = ApiKeyMarkUserRealName;
                TextBoxRealNameapiSecret.Text = ApiSecretMarkUserRealName;
            }
            if (string.IsNullOrWhiteSpace(ApiKeyMarkUserVerified) || string.IsNullOrWhiteSpace(ApiSecretMarkUserVerified))
            {
                if (string.IsNullOrWhiteSpace(ApiKeyMarkUserVerified) && string.IsNullOrWhiteSpace(ApiSecretMarkUserVerified))
                {
                    WarnVerifyingUserApiKey.Visibility = Visibility.Visible;
                    WarnVerifyingUserApiSecret.Visibility = Visibility.Visible;
                }
                else if (string.IsNullOrWhiteSpace(ApiKeyMarkUserVerified))
                    WarnVerifyingUserApiKey.Visibility = Visibility.Visible;
                else if (string.IsNullOrWhiteSpace(ApiSecretMarkUserVerified))
                    WarnVerifyingUserApiSecret.Visibility = Visibility.Visible;
            }
            else
            {
                TextBoxVerifyingUserApiKeys.Text = ApiKeyMarkUserVerified;
                TextBoxVerifyingUserApiSecret.Text = ApiSecretMarkUserVerified;
            }
            if (string.IsNullOrWhiteSpace(ApiKeyReleaseBitcoins) || string.IsNullOrWhiteSpace(ApiSecretReleaseBitcoins))
            {
                if (string.IsNullOrWhiteSpace(ApiKeyReleaseBitcoins) && string.IsNullOrWhiteSpace(ApiSecretReleaseBitcoins))
                {
                    WarnReleaseBitcoinskey.Visibility = Visibility.Visible;
                    WarnReleaseBitcoinsSecret.Visibility = Visibility.Visible;
                }
                else if (string.IsNullOrWhiteSpace(ApiKeyReleaseBitcoins))
                    WarnReleaseBitcoinskey.Visibility = Visibility.Visible;
                else if (string.IsNullOrWhiteSpace(ApiSecretReleaseBitcoins))
                    WarnReleaseBitcoinsSecret.Visibility = Visibility.Visible;
            }
            else
            {
                TextBoxReleaseBitcoinsKeys.Text = ApiKeyReleaseBitcoins;
                TextBoxReleaseBitcoinsSecret.Text = ApiSecretReleaseBitcoins;
            }
            if (string.IsNullOrWhiteSpace(DiretoryPath))
                WarnDirectoryFiles.Visibility = Visibility.Visible;
            else
                TextBoxDirectoryFiles.Text = DiretoryPath;
        }

        private bool ApiKeysCheckMissing()
        {
            var ApiNotificationskey = ConfigurationManager.AppSettings["ApikeyNotifications"];
            var ApiNotificationsSecret = ConfigurationManager.AppSettings["ApiSecretNotifications"];
            var ApikeyGobalMessages = ConfigurationManager.AppSettings["ApikeyGobalMessages"];
            var ApiSecretGobalMessages = ConfigurationManager.AppSettings["ApiSecretGobalMessages"];
            var ApiMarkNotificationsKey = ConfigurationManager.AppSettings["ApiKeyMarkNotifications"];
            var ApiMarkNotificationsSecret = ConfigurationManager.AppSettings["ApiSecretMarkNotifications"];
            var ApiKeyNewTrades = ConfigurationManager.AppSettings["ApiKeyNewTrades"];
            var ApiSecretNewTrades = ConfigurationManager.AppSettings["ApiSecretNewTrades"];
            var ApiKeyMessages = ConfigurationManager.AppSettings["ApiKeyMessages"];
            var ApiSecretMessages = ConfigurationManager.AppSettings["ApiSecretMessages"];
            var ApiKeyChatMessages = ConfigurationManager.AppSettings["ApiKeyChatMessages"];
            var ApiSecretChatMessages = ConfigurationManager.AppSettings["ApiSecretChatMessages"];
            var ApiKeyMarkUserRealName = ConfigurationManager.AppSettings["ApiKeyMarkUserRealName"];
            var ApiSecretMarkUserRealName = ConfigurationManager.AppSettings["ApiSecretMarkUserRealName"];
            var ApiKeyMarkUserVerified = ConfigurationManager.AppSettings["ApiKeyMarkUserVerified"];
            var ApiSecretMarkUserVerified = ConfigurationManager.AppSettings["ApiSecretMarkUserVerified"];
            var ApiKeyReleaseBitcoins = ConfigurationManager.AppSettings["ApiKeyReleaseBitcoins"];
            var ApiSecretReleaseBitcoins = ConfigurationManager.AppSettings["ApiSecretReleaseBitcoins"];
            var DiretoryPath = ConfigurationManager.AppSettings["DirectoryPath"];

            if (string.IsNullOrWhiteSpace(ApiNotificationskey) || string.IsNullOrWhiteSpace(ApiNotificationsSecret) || string.IsNullOrWhiteSpace(ApiMarkNotificationsKey) ||
                string.IsNullOrWhiteSpace(ApiMarkNotificationsSecret) || string.IsNullOrWhiteSpace(ApiKeyNewTrades) || string.IsNullOrWhiteSpace(ApiSecretNewTrades) || string.IsNullOrWhiteSpace(ApiKeyMessages) ||
                string.IsNullOrWhiteSpace(ApiSecretMessages) || string.IsNullOrWhiteSpace(ApiKeyChatMessages) || string.IsNullOrWhiteSpace(ApiSecretChatMessages) || string.IsNullOrWhiteSpace(ApiKeyMarkUserRealName) ||
                string.IsNullOrWhiteSpace(ApiSecretMarkUserRealName) || string.IsNullOrWhiteSpace(ApiKeyMarkUserVerified) || string.IsNullOrWhiteSpace(ApiSecretMarkUserVerified) ||
                string.IsNullOrWhiteSpace(ApiKeyReleaseBitcoins) || string.IsNullOrWhiteSpace(ApiSecretReleaseBitcoins) || string.IsNullOrWhiteSpace(DiretoryPath) || string.IsNullOrWhiteSpace(ApikeyGobalMessages)
                || string.IsNullOrWhiteSpace(ApiSecretGobalMessages))
                return true;
            else
                return false;
        }

        async private void ButtonNotificationsKeys_Click(object sender, RoutedEventArgs e)
        {
            ViewboxNotificationsKeys.Visibility = Visibility.Visible;
            var ApiKey = TextBoxNotificationsKey.Text;
            var ApiSecret = TextBoxNotificationsSecret.Text;
            try
            {
                Task ApikeyTask = Task.Run(() => ConfigSettings("ApikeyNotifications", ApiKey));
                Task ApiSecretTask = Task.Run(() => ConfigSettings("ApiSecretNotifications", ApiSecret));
                await Task.WhenAll(ApikeyTask, ApiSecretTask);
                ViewboxNotificationsKeys.Visibility = Visibility.Collapsed;
                WarnNotificationsKey.Visibility = Visibility.Collapsed;
                WarnNotificationsSecret.Visibility = Visibility.Collapsed;
            }
            catch (Exception ep)
            {
                ErrorLogging(ep);
                ViewboxNotificationsKeys.Visibility = Visibility.Collapsed;
            }
        }

        async private void ButtonGlobalMessages_Click(object sender, RoutedEventArgs e)
        {
            ViewboxGlobalMessages.Visibility = Visibility.Visible;
            var ApiKey = TextBoxGlobalMessagesKey.Text;
            var ApiSecret = TextBoxGlobalMessagesSecret.Text;
            try
            {
                Task ApikeyTask = Task.Run(() => ConfigSettings("ApikeyGobalMessages", ApiKey));
                Task ApiSecretTask = Task.Run(() => ConfigSettings("ApiSecretGobalMessages", ApiSecret));
                await Task.WhenAll(ApikeyTask, ApiSecretTask);
                ViewboxGlobalMessages.Visibility = Visibility.Collapsed;
                WarnGlobalMessagesKey.Visibility = Visibility.Collapsed;
                WarnGlobalMessagesSecret.Visibility = Visibility.Collapsed;
            }
            catch (Exception ep)
            {
                ErrorLogging(ep);
                ViewboxGlobalMessages.Visibility = Visibility.Collapsed;
            }
        }

        async private void ButtonNotificationsReadingKeys_Click(object sender, RoutedEventArgs e)
        {
            ViewboxNotificationsReadingKeys.Visibility = Visibility.Visible;
            var ApiKey = TextBoxNotificationsReadingKey.Text;
            var ApiSecret = TextBoxNotificationsReadingSecret.Text;
            try
            {
                Task ApikeyTask = Task.Run(() => ConfigSettings("ApiKeyMarkNotifications", ApiKey));
                Task ApiSecretTask = Task.Run(() => ConfigSettings("ApiSecretMarkNotifications", ApiSecret));
                await Task.WhenAll(ApikeyTask, ApiSecretTask);
                ViewboxNotificationsReadingKeys.Visibility = Visibility.Collapsed;
                WarnNotificationsReadingKey.Visibility = Visibility.Collapsed;
                WarnNotificationsReadingSecret.Visibility = Visibility.Collapsed;
            }
            catch (Exception ep)
            {
                ErrorLogging(ep);
                ViewboxNotificationsReadingKeys.Visibility = Visibility.Collapsed;
            }
        }

        async private void ButtonAccountINfoKeys_Click(object sender, RoutedEventArgs e)
        {
            ViewboxAccountINfoKeys.Visibility = Visibility.Visible;
            var ApiKey = TextBoxAccountINfoKey.Text;
            var ApiSecret = TextBoxAccountINfoSecret.Text;
            try
            {
                Task ApikeyTask = Task.Run(() => ConfigSettings("ApiKeyNewTrades", ApiKey));
                Task ApiSecretTask = Task.Run(() => ConfigSettings("ApiSecretNewTrades", ApiSecret));
                await Task.WhenAll(ApikeyTask, ApiSecretTask);
                ViewboxAccountINfoKeys.Visibility = Visibility.Collapsed;
                WarnAccountINfokey.Visibility = Visibility.Collapsed;
                WarnAccountINfoSecret.Visibility = Visibility.Collapsed;
            }
            catch (Exception ep)
            {
                ErrorLogging(ep);
                ViewboxAccountINfoKeys.Visibility = Visibility.Collapsed;
            }
        }

        async private void ButtonMessagesKeys_Click(object sender, RoutedEventArgs e)
        {
            ViewboxMessagesKeys.Visibility = Visibility.Visible;
            var ApiKey = TextBoxMessagesKeys.Text;
            var ApiSecret = TextboxChatMessagesSecret.Text;
            try
            {
                Task ApikeyTask = Task.Run(() => ConfigSettings("ApiKeyMessages", ApiKey));
                Task ApiSecretTask = Task.Run(() => ConfigSettings("ApiSecretMessages", ApiSecret));
                await Task.WhenAll(ApikeyTask, ApiSecretTask);
                ViewboxMessagesKeys.Visibility = Visibility.Collapsed;
                WarnMessageskey.Visibility = Visibility.Collapsed;
                WarnMessagessecret.Visibility = Visibility.Collapsed;
            }
            catch (Exception ep)
            {
                ErrorLogging(ep);
                ViewboxMessagesKeys.Visibility = Visibility.Collapsed;
            }
        }

        async private void ButtonChatMessagesKeys_Click(object sender, RoutedEventArgs e)
        {
            ViewboxChatMessagesKeys.Visibility = Visibility.Visible;
            var ApiKey = TextBoxChatMessagesKey.Text;
            var ApiSecret = TextboxChatMessagesSecret.Text;
            try
            {
                Task ApikeyTask = Task.Run(() => ConfigSettings("ApiKeyChatMessages", ApiKey));
                Task ApiSecretTask = Task.Run(() => ConfigSettings("ApiSecretChatMessages", ApiSecret));
                await Task.WhenAll(ApikeyTask, ApiSecretTask);
                ViewboxChatMessagesKeys.Visibility = Visibility.Collapsed;
                WarnChatMessagesKey.Visibility = Visibility.Collapsed;
                WarnChatMessagesSecret.Visibility = Visibility.Collapsed;
            }
            catch (Exception ep)
            {
                ErrorLogging(ep);
                ViewboxChatMessagesKeys.Visibility = Visibility.Collapsed;
            }
        }

        async private void ButtonRealNameapiKeys_Click(object sender, RoutedEventArgs e)
        {
            ViewboxRealNameapiKeys.Visibility = Visibility.Visible;
            var ApiKey = TextBoxRealNameapiKeys.Text;
            var ApiSecret = TextBoxRealNameapiSecret.Text;
            try
            {
                Task ApikeyTask = Task.Run(() => ConfigSettings("ApiKeyMarkUserRealName", ApiKey));
                Task ApiSecretTask = Task.Run(() => ConfigSettings("ApiSecretMarkUserRealName", ApiSecret));
                await Task.WhenAll(ApikeyTask, ApiSecretTask);
                ViewboxRealNameapiKeys.Visibility = Visibility.Collapsed;
                WarnRealNameapiKey.Visibility = Visibility.Collapsed;
                WarnRealNameapiSecret.Visibility = Visibility.Collapsed;
            }
            catch (Exception ep)
            {
                ErrorLogging(ep);
                ViewboxRealNameapiKeys.Visibility = Visibility.Collapsed;
            }
        }

        async private void ButtonVerifyingUserApiKeys_Click(object sender, RoutedEventArgs e)
        {
            ViewboxVerifyingUserApiKeys.Visibility = Visibility.Visible;
            var ApiKey = TextBoxVerifyingUserApiKeys.Text;
            var ApiSecret = TextBoxVerifyingUserApiSecret.Text;
            try
            {
                Task ApikeyTask = Task.Run(() => ConfigSettings("ApiKeyMarkUserVerified", ApiKey));
                Task ApiSecretTask = Task.Run(() => ConfigSettings("ApiSecretMarkUserVerified", ApiSecret));
                await Task.WhenAll(ApikeyTask, ApiSecretTask);
                ViewboxVerifyingUserApiKeys.Visibility = Visibility.Collapsed;
                WarnVerifyingUserApiKey.Visibility = Visibility.Collapsed;
                WarnVerifyingUserApiSecret.Visibility = Visibility.Collapsed;
            }
            catch (Exception ep)
            {
                ErrorLogging(ep);
                ViewboxVerifyingUserApiKeys.Visibility = Visibility.Collapsed;
            }
        }

        async private void ButtonReleaseBitcoinsKeys_Click(object sender, RoutedEventArgs e)
        {
            ViewboxReleaseBitcoinsKeys.Visibility = Visibility.Visible;
            var ApiKey = TextBoxReleaseBitcoinsKeys.Text;
            var ApiSecret = TextBoxReleaseBitcoinsSecret.Text;
            try
            {
                Task ApikeyTask = Task.Run(() => ConfigSettings("ApiKeyMarkUserVerified", ApiKey));
                Task ApiSecretTask = Task.Run(() => ConfigSettings("ApiSecretMarkUserVerified", ApiSecret));
                await Task.WhenAll(ApikeyTask, ApiSecretTask);
                ViewboxReleaseBitcoinsKeys.Visibility = Visibility.Collapsed;
                WarnReleaseBitcoinskey.Visibility = Visibility.Collapsed;
                WarnReleaseBitcoinsSecret.Visibility = Visibility.Collapsed;
            }
            catch (Exception ep)
            {
                ErrorLogging(ep);
                ViewboxReleaseBitcoinsKeys.Visibility = Visibility.Collapsed;
            }
        }

        async private void ButtonDirectoryFiles_Click(object sender, RoutedEventArgs e)
        {
            var chooseFolder = new Microsoft.Win32.SaveFileDialog();
            chooseFolder.InitialDirectory = @"F:\Downloads";
            chooseFolder.Title = "Select a Directory";
            chooseFolder.Filter = "Directory|*.this.directory";
            chooseFolder.FileName = "select";
            if (chooseFolder.ShowDialog() == true)
            {
                string diretory = chooseFolder.FileName;
                diretory = diretory.Replace("\\select.this.directory", "");
                diretory = diretory.Replace(".this.directory", "");
                if (!System.IO.Directory.Exists(diretory))
                {
                    System.IO.Directory.CreateDirectory(diretory);
                }
                TextBoxDirectoryFiles.Text = diretory;
                await Task.Run(() => ConfigSettings("DirectoryPath", diretory));
            }
        }

        async private void ButtonGridsTrades_Click(object sender, RoutedEventArgs e)
        {
            await Task.Run(() => Updategridstrades());
        }

        private void Updategridstrades()
        {
            this.Dispatcher.BeginInvoke((Action)(() =>
            {
                myDataGridSelling.Dispatcher.BeginInvoke((Action)(() =>
                {
                    myDataGridSelling.Items.Clear();
                }));
                myDataGridBuying.Dispatcher.BeginInvoke((Action)(() =>
                {
                    myDataGridBuying.Items.Clear();
                }));
            }));
            if (AllCurrentTrades != null)
            {
                List<RootObject> AllTrades = null;
                this.Dispatcher.Invoke(() =>
                {
                    AllTrades = AllCurrentTrades.data.contact_list;
                });
                decimal totalBtc = 0;
                foreach (var item in AllTrades)
                {
                    if (item.data.is_buying == true)
                    {
                        var Statustrade = "";
                        if (item.data.payment_completed_at != null && item.data.canceled_at == null && item.data.released_at == null)
                            Statustrade = "Payment completed";
                        else if (item.data.canceled_at != null)
                            Statustrade = "Canceled!";
                        else if (item.data.closed_at != null && item.data.canceled_at == null && item.data.released_at == null)
                            Statustrade = "Canceled!";
                        else if (item.data.released_at != null)
                            Statustrade = "Released!";
                        else
                            Statustrade = "Waiting Payment";
                        var newCostumer = new TableItemscs
                        {
                            Nickname = item.data.seller.username,
                            Amount = item.data.amount,
                            AmountBtc = item.data.amount_btc,
                            CreatedAt = item.data.created_at.ToString(),
                            ContactNumber = item.data.contact_id.ToString(),
                            PaymentMethod = item.data.advertisement.payment_method,
                            Status = Statustrade
                        };
                        this.Dispatcher.BeginInvoke((Action)(() =>
                        {
                            myDataGridBuying.Dispatcher.BeginInvoke((Action)(() =>
                            {
                                myDataGridBuying.Items.Add(newCostumer);
                            }));
                        }));
                        decimal.TryParse(item.data.amount_btc, out decimal itemAmountBtc);
                        decimal.TryParse(item.data.fee_btc, out decimal itemAmountfeeBTC);
                        totalBtc = totalBtc + itemAmountBtc + itemAmountfeeBTC;
                    }
                    else
                    {
                        //    Hyperlink newhYper = new Hyperlink();
                        //newhYper.Click += NewhYper_Click;
                        //newhYper.IsEnabled = true;
                        //newhYper.Inlines.Add(item.data.contact_id.ToString());
                        var Statustrade = "";
                        if (item.data.payment_completed_at != null && item.data.canceled_at == null && item.data.released_at == null)
                            Statustrade = "Payment completed";
                        else if (item.data.canceled_at != null)
                            Statustrade = "Canceled!";
                        else if (item.data.closed_at != null && item.data.canceled_at == null && item.data.released_at == null)
                            Statustrade = "Canceled!";
                        else if (item.data.released_at != null)
                            Statustrade = "Released!";
                        else
                            Statustrade = "Waiting Payment";
                        var newCostumer = new TableItemscs
                        {
                            Nickname = item.data.buyer.username.ToString(),
                            Amount = item.data.amount.ToString(),
                            AmountBtc = item.data.amount_btc.ToString(),
                            CreatedAt = item.data.created_at.ToString(),
                            ContactNumber = item.data.contact_id.ToString(),
                            PaymentMethod = item.data.advertisement.payment_method.ToString(),
                            Status = Statustrade
                        };
                        this.Dispatcher.BeginInvoke((Action)(() =>
                        {
                            myDataGridSelling.Dispatcher.BeginInvoke((Action)(() =>
                            {
                                myDataGridSelling.Items.Add(newCostumer);
                            }));
                        }));
                        decimal.TryParse(item.data.amount_btc, out decimal itemAmountBtc);
                        decimal.TryParse(item.data.fee_btc, out decimal itemAmountfeeBTC);
                        totalBtc = totalBtc + itemAmountBtc + itemAmountfeeBTC;
                    }
                }
                this.Dispatcher.BeginInvoke((Action)(() =>
                {
                    TextBlocktotalbitcoinsunderescrow.Dispatcher.BeginInvoke((Action)(() =>
                    {
                        TextBlocktotalbitcoinsunderescrow.Text = $"Your total amount of bitcoins under escrow is {totalBtc} BTC";
                        }));
                }));
            }
        }

        async private void ButtonGridAds_Click(object sender, RoutedEventArgs e)
        {
            await Task.Run(() => UpdateGridAds());
        }

        private void UpdateGridAds()
        {
            var APiKey = ConfigurationManager.AppSettings["ApiKeyNewTrades"];
            var ApiSecret = ConfigurationManager.AppSettings["ApiSecretNewTrades"];
            var client = new LocalBitcoinsAPI(APiKey, ApiSecret);
            myDataGridAds.Dispatcher.BeginInvoke((Action)(() =>
            {
                myDataGridAds.Items.Clear();
            }));
            var ads = client.GetOwnAds();
            AdsData myAds = JsonConvert.DeserializeObject<AdsData>(ads);
            if (myAds != null)
            {
                foreach (var item in myAds.data.ad_list)
                {
                    string Advisibilty = "Disabled";
                    if (item.data.visible == true)
                        Advisibilty = "Active";
                    var newRow = new TableAds
                    {
                        Adid = item.data.ad_id.ToString(),
                        AdStatus = Advisibilty,
                        AdEquation = item.data.price_equation,
                        AdInfo = item.data.trade_type.ToString() + " " + item.data.online_provider.ToString(),
                        AdPrice = item.data.temp_price.ToString() + item.data.currency.ToString(),
                        adCreatedat = item.data.created_at.ToString()
                    };
                    this.Dispatcher.BeginInvoke((Action)(() =>
                    {
                        myDataGridAds.Dispatcher.BeginInvoke((Action)(() =>
                        {
                            myDataGridAds.Items.Add(newRow);
                        }));
                        AdsUpdated = true;
                    }));
                }
            }
        }

        private void tabControlBTC_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tabControlBTC.Items.Count == 3)
            {
                var item = tabControlBTC.SelectedItem;
                if (item == null)
                    tabControlBTC.SelectedItem = HomeTab;
            }
        }

        async private void ButtonverificationAdduser_Click(object sender, RoutedEventArgs e)
        {
            SpinnerButtonverificationAdduser.Visibility = Visibility.Visible;
            var contacttrade = TextBoxVerificationContactid.Text;
            var email = TextBoxVerificationEmail.Text;
            if (string.IsNullOrWhiteSpace(contacttrade) || string.IsNullOrWhiteSpace(email))
            {
                WarnButtonverificationAdduser.Visibility = Visibility.Visible;
                WarnTextBlockverificationAdduser.Text = "Email or contact id empty!";
            }
            else
            {
               try
               {
                    await Task.Run(() => verifyuser(email, contacttrade));
                    TextBoxVerificationContactid.Clear();
                    TextBoxVerificationEmail.Clear();
               }
               catch (Exception ep)
               {
                   MainWindow.ErrorLogging(ep);
                   WarnButtonverificationAdduser.Visibility = Visibility.Visible;
                   WarnTextBlockverificationAdduser.Text = "User not verified!";
                }
            }
            SpinnerButtonverificationAdduser.Visibility = Visibility.Collapsed;
        }

        private void verifyuser(string email, string contactid)
        {
            var APiKey = ConfigurationManager.AppSettings["ApiKeyMarkUserVerified"];
            var ApiSecret = ConfigurationManager.AppSettings["ApiSecretMarkUserVerified"];
            var client = new LocalBitcoinsAPI(APiKey, ApiSecret);
            LocalBitcoinsDatabaseEntities _BaseDados = new LocalBitcoinsDatabaseEntities();
            var infocontact = client.GetContactInfo(contactid);
            RootObject InfoApicontact = JsonConvert.DeserializeObject<RootObject>(infocontact);
            var username = InfoApicontact.data.buyer.username;
            var paypalemail = email;
            bool TradeisSell = InfoApicontact.data.is_selling;
            try
            {
                using (_BaseDados)
                {
                    bool UserExists = _BaseDados.Usernames.Any(x => x.Username1 == username);
                    if (UserExists == true)
                    {
                        var UserData = _BaseDados.Usernames.FirstOrDefault(x => x.Username1 == username).UsernameInfo;
                        UserData.Email = paypalemail;
                        UserData.Verified = true;
                        UserData.Verification_date = DateTime.Now;
                    }
                    else if (UserExists == false)
                    {
                        int DataBaseCount = _BaseDados.Usernames.Count();
                        int usernameKey = DataBaseCount + 1;
                        var userRealName = "";
                        if (TradeisSell == true)
                            userRealName = InfoApicontact.data.buyer.real_name;
                        else
                            userRealName = null;
                        _BaseDados.Usernames.Add(new Username()
                        {
                            Username1 = username,
                            UsernameKey = usernameKey,
                            UsernameInfo = new UsernameInfo()
                            {
                                UsernameKey = usernameKey,
                                Email = paypalemail,
                                Verified = true,
                                Verification_date = DateTime.Now,
                                RealName = userRealName
                            }
                        });
                    }
                    _BaseDados.SaveChanges();
                    WarnButtonverificationAdduser.Dispatcher.BeginInvoke((Action)(() =>
                    {
                        WarnButtonverificationAdduser.Visibility = Visibility.Collapsed;
                    }));
                }
            }
            catch (Exception ep)
            {
                MainWindow.ErrorLogging(ep);
                WarnButtonverificationAdduser.Dispatcher.BeginInvoke((Action)(() =>
                {
                    WarnButtonverificationAdduser.Visibility = Visibility.Visible;
                }));
                WarnTextBlockverificationAdduser.Dispatcher.BeginInvoke((Action)(() =>
                {
                    WarnTextBlockverificationAdduser.Text = "User not verified!";
                }));
            }
        }

        private void ButtonVerification_Click(object sender, RoutedEventArgs e)
        {
            tabControlBTC.SelectedItem = VerificationTab;
        }

        async private void ButtonverificationGmailPhotos_Click(object sender, RoutedEventArgs e)
        {
            SpinnerButtonverificationgmailphotos.Visibility = Visibility.Visible;
            var email = TextBoxVerificationGmailphotos.Text;
            var contactid = TextBoxVerificationContactidGmail.Text;
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(contactid) || FilesverificationList.Count() == 0)
            {
                WarnButtonverificationGmailPhotos.Visibility = Visibility.Visible;
                WarnTextBlockverificationGmailphotos.Text = "Check all fields!";
            }
            else
            {
                try
                {
                    await Task.Run(() => gmailVerification(contactid, email));
                    FilesverificationList.Clear();
                    BordertextbuttonVerificationGmail.Content = FilesverificationList.Count().ToString();
                    TextBoxVerificationGmailphotos.Clear();
                    TextBoxVerificationContactidGmail.Clear();
                }
                catch (Exception ep)
                {
                    ErrorLogging(ep);
                    WarnButtonverificationGmailPhotos.Visibility = Visibility.Visible;
                    WarnTextBlockverificationGmailphotos.Text = "Error occured! Check logs!";
                }

            }
            SpinnerButtonverificationgmailphotos.Visibility = Visibility.Collapsed;
        }

        public string VerificationsEmailBody(string contactid, string realname, string Email, string username)
        {
            var name = realname;

            StringBuilder myString = new StringBuilder();
            myString.AppendLine($"Localbitcoins Contact {contactid}");
            myString.AppendLine();
            myString.AppendLine($"https://localbitcoins.com/accounts/profile/{username}");
            myString.AppendLine($"https://localbitcoins.com/request/online_sell_seller/{contactid}");
            myString.AppendLine($"{Email}");
            myString.AppendLine();
            myString.AppendLine($"Nome Real: {name}");

            return myString.ToString();
        }

        private void gmailVerification(string contactid, string PaypalEmail)
        {
            var tradenumber = contactid;
            var paypalemail = PaypalEmail;
            var APiKey = ConfigurationManager.AppSettings["ApiKeyMarkUserVerified"];
            var ApiSecret = ConfigurationManager.AppSettings["ApiSecretMarkUserVerified"];
            var client = new LocalBitcoinsAPI(APiKey, ApiSecret);
            try
            {
                var calluserinfo = client.GetContactInfo(tradenumber);
                RootObject infouserApi = JsonConvert.DeserializeObject<RootObject>(calluserinfo);
                var username = infouserApi.data.buyer.username;
                var realname = infouserApi.data.buyer.real_name;
                var fromAddress = new MailAddress("mr.niceonne@hotmail.com", "");
                var toAddress = new MailAddress("fialjose@gmail.com", "");
                const string fromPassword = "933897374pass";
                string subject = username;
                string body = VerificationsEmailBody(tradenumber, realname, paypalemail, username);
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
                    WarnButtonverificationGmailPhotos.Dispatcher.BeginInvoke((Action)(() =>
                    {
                        WarnButtonverificationGmailPhotos.Visibility = Visibility.Collapsed;
                    }));
                    MessageBox.Show("User sucessfully verified!");
                }
                catch (Exception ep)
                {
                    MainWindow.ErrorLogging(ep);
                    WarnButtonverificationGmailPhotos.Dispatcher.BeginInvoke((Action)(() =>
                    {
                        WarnButtonverificationGmailPhotos.Visibility = Visibility.Visible;
                    }));
                    WarnTextBlockverificationGmailphotos.Dispatcher.BeginInvoke((Action)(() =>
                    {
                        WarnTextBlockverificationGmailphotos.Text = "Photos not sent to e-mail! Check logs!";
                    }));
                }
            }
            catch (Exception ep)
            {
                ErrorLogging(ep);
                WarnButtonverificationGmailPhotos.Dispatcher.BeginInvoke((Action)(() =>
                {
                    WarnButtonverificationGmailPhotos.Visibility = Visibility.Visible;
                }));
                WarnTextBlockverificationGmailphotos.Dispatcher.BeginInvoke((Action)(() =>
                {
                    WarnTextBlockverificationGmailphotos.Text = "User info not retrieved! Check logs!";
                }));
            }
        }

        private List<string> FilesverificationList = new List<string>();

        private void ButtonChooseFileGmailverification_Click(object sender, RoutedEventArgs e)
        {
            int numberofFiles = FilesverificationList.Count();
            if (numberofFiles == 0)
            {
                System.Windows.Forms.OpenFileDialog fdlg = new System.Windows.Forms.OpenFileDialog();
                fdlg.Title = "Localbitcoins Verification Files";
                fdlg.InitialDirectory = @"F:\Downloads";
                fdlg.Filter = "All files (*.*)|*.*";
                fdlg.FilterIndex = 2;
                fdlg.Multiselect = true;
                fdlg.RestoreDirectory = true;

                if (fdlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    foreach (String file in fdlg.FileNames)
                    {
                        FilesverificationList.Add(file);
                    }
                }
                BordertextbuttonVerificationGmail.Content = FilesverificationList.Count().ToString();
            }
            else
            {
                FilesverificationList.Clear();
                BordertextbuttonVerificationGmail.Content = FilesverificationList.Count().ToString();
            }
        }

    }
}
