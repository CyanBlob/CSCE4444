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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WeamyNotifications;
using TwitchCSharp.Clients;
using TwitchCSharp.Enums;
using TwitchCSharp.Helpers;
using TwitchCSharp.Models;
using YouTubeAPI;

using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Windows.Interop;
using Reminders;
using TwitchTester;
using Notifications;
using Weamy.Notes;

namespace UIPrototype
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        // being global hotkey code
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        private const int HOTKEY_ID = 9000;

        private static IntPtr _windowHandle;
        private static HwndSource _source;

        protected override void OnSourceInitialized(EventArgs e)
        {
            //base.OnSourceInitialized(e);

            _windowHandle = new WindowInteropHelper(this).Handle;
            _source = HwndSource.FromHwnd(_windowHandle);
            _source.AddHook(HwndHook);

            // for now, just register addition hotkeys here
            // be sure to add a statement for each hotkey in HwndHook ( if (vkey == KeyCodes.x) {...} )
            RegisterHotKey(_windowHandle, HOTKEY_ID, KeyCodes.CONTROL + KeyCodes.ALT, KeyCodes.R); //CTRL + ALT + S
            RegisterHotKey(_windowHandle, HOTKEY_ID, KeyCodes.CONTROL + KeyCodes.ALT, KeyCodes.S); //CTRL + ALT + S
            RegisterHotKey(_windowHandle, HOTKEY_ID, KeyCodes.CONTROL + KeyCodes.ALT, KeyCodes.T); //CTRL + ALT + T
            RegisterHotKey(_windowHandle, HOTKEY_ID, KeyCodes.CONTROL + KeyCodes.ALT, KeyCodes.N); //CTRL + ALT + N
        }

        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_HOTKEY = 0x0312;
            switch (msg)
            {
                case WM_HOTKEY:
                    switch (wParam.ToInt32())
                    {
                        case HOTKEY_ID:
                            int vkey = (((int)lParam >> 16) & 0xFFFF);
                            if (vkey == KeyCodes.T || vkey == KeyCodes.S || vkey == KeyCodes.R)
                            {
                                createReminder(null, null);
                            }
                            else if(vkey == KeyCodes.N)
                            {
                                btnNote_Click(null, null);
                            }            

                            handled = true;
                            break;
                    }
                    break;
            }
            return IntPtr.Zero;
        }

        private void createReminder(object sender, RoutedEventArgs e)
        {
            bool reminderPopUpResult;
            ReminderPopUp reminderPopUp = new ReminderPopUp();
            reminderPopUpResult = (bool)reminderPopUp.ShowDialog();

            if (reminderPopUpResult == true)
            {
                DateTime today = DateTime.Now;
                //string dateTime = today.Month + "/" + today.Day + "/" + today.Year + " " +
                    //today.Hour + reminderPopUp.HoursText.GetLineText(0) + ":" + today.Minute + reminderPopUp.MinutesText.GetLineText(0) + ":" + today.Second + reminderPopUp.SecondsText.GetLineText(0) + ".00";
                double hours = Double.Parse(reminderPopUp.HoursText.GetLineText(0));
                double minutes = Double.Parse(reminderPopUp.MinutesText.GetLineText(0));
                double seconds = Double.Parse(reminderPopUp.SecondsText.GetLineText(0));
                DateTime reminderDate = DateTime.Now.AddHours(hours).AddMinutes(minutes).AddSeconds(seconds);
                //Console.WriteLine(dateTime);

                //DateTime reminderDate = Convert.ToDateTime(dateTime);

                /*double seconds = (
                    Double.Parse(reminderPopUp.HoursText.GetLineText(0)) * 3600 +
                    Double.Parse(reminderPopUp.MinutesText.GetLineText(0)) * 60 +
                    Double.Parse(reminderPopUp.SecondsText.GetLineText(0)));*/

                string reminderText = "";
                reminderText = reminderPopUp.NotificationText.Text;

                //for (i = 0; i < reminderPopUp.NotificationText.LineCount; i++)
                //{
                //    reminderText += reminderPopUp.NotificationText.GetLineText(i);
                //}

                //new Reminder(DateTime.Now.AddSeconds(seconds), reminderText);
                new Reminder(reminderDate, reminderText);
                //tblock.Text += "Notification set for: " + DateTime.Now.AddSeconds(seconds) + Environment.NewLine;

                //tblock.Text += reminderPopUp.NotificationText.GetLineText(0);
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            _source.RemoveHook(HwndHook);
            UnregisterHotKey(_windowHandle, HOTKEY_ID);
            base.OnClosed(e);
        }
        //end global hotkey code

        // DO NOT CHANGE THESE, THEY ARE REQUIRED FOR THE TWITCH API TO WORK //
        private const string CLIENT_ID = "s4fxyi0repqgclpqgnnd1siicn7qjxe;";
        private const string AUTH_ID = "y8rv59wxnof94ocjbx09z6bbuageue";
        // ----------------------------------------------------------------- //
        //Twitch
        private ObservableCollection<WeamyDataBoundObject> userLiveChannels;
        public ObservableCollection<WeamyDataBoundObject> UserLiveChannels
        {
            get
            {
                if (userLiveChannels == null)
                {
                    userLiveChannels = new ObservableCollection<WeamyDataBoundObject>();
                }
                return userLiveChannels;
            }
        }

        private List<WeamyDataBoundObject> userChannels = new List<WeamyDataBoundObject>();

        //YouTube
        private ObservableCollection<WeamyDataBoundObject> youTubeVids;
        public ObservableCollection<WeamyDataBoundObject> YouTubeVids
        {
            get
            {
                if (youTubeVids == null)
                {
                    youTubeVids = new ObservableCollection<WeamyDataBoundObject>();
                }
                return youTubeVids;
            }
        }

        private List<WeamyDataBoundObject> YTVids = new List<WeamyDataBoundObject>();

        System.Timers.Timer twitchTimer;
        private int tickTimer = 15;
        private string twitchUsername = "Monatrox";
        //private string YouTubeUsername = "UCOZ6Q4QpkGZz5LYOUKs6g8A";
        private string YouTubeUsername = "UCT3IDkrEU07il99Vcf15YYw";
        private bool enableTwitchNotifications = false;
        private bool enableYoutubeNotifications = false;

        private CallbackBrowser twitchBrowser = CallbackBrowser.Default;
        private CallbackBrowser youtubeBrowser = CallbackBrowser.Default;

        public MainWindow()
        {
            this.DataContext = this;
            InitializeComponent();
            txtTwitchUsername.Text = twitchUsername;
            txtYoutubeUsername.Text = YouTubeUsername;
            cbxTwitchBrowser.SelectedIndex = 0;
            cbxYouTubeBrowser.SelectedIndex = 0;
            string imageFilePath = System.IO.Path.GetTempPath() + "\\Weamy_" + "404_user_50x50.png" + ".jpeg";   // set image path for file

            UserLiveChannels.Add(new WeamyDataBoundObject
            {
                title = "Updating Live Twitch Streams",
                textLine1 = "It takes a while",
                textLine2 = "The list will update automatically when it's finished",
                imagePath = "http://www-cdn.jtvnw.net/images/xarth/404_user_50x50.png",
                callbackUrl = "https://twitch.tv"
            });

            twitchTimer = new System.Timers.Timer(1000); // initally run after 1 second. Still takes time to make the web request in the first place (a lot of time :< )
            twitchTimer.Elapsed += updateYouTube;
            twitchTimer.Elapsed += checkLiveChannels;
            twitchTimer.Enabled = true;

            ((App)Application.Current).notes = new NoteList();
            ((App)Application.Current).notes.loadFromFile("notes.xml");
            lstNotes.ItemsSource = ((App)Application.Current).notes.notes;
        }

        private void combi_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void btnTwitch_Click(object sender, RoutedEventArgs e)
        {
            lstContent.ItemsSource = null;
            lstContent.ItemsSource = UserLiveChannels;
            YouTubeContent.ItemsSource = null;
            YouTubeContent.ItemsSource = YouTubeVids;
        }

        private void checkLiveChannels(object source, System.Timers.ElapsedEventArgs e)
        {
            TwitchAuthenticatedClient client = new TwitchAuthenticatedClient(CLIENT_ID, AUTH_ID);
            twitchTimer.Interval = tickTimer * 1000;        // milliseconds -- also this is the simple way to do this using System.Timers.Timer 
            twitchTimer.Enabled = false;                    // disable until updated first time

            if (string.IsNullOrEmpty(twitchUsername) || client.GetChannel(twitchUsername) == null)
            {
                twitchUsername = "Monatrox";
            }

            PagingInfo pagingInfo = new PagingInfo { Page = 1 };
            TwitchList<FollowedChannel> followedChannels = client.GetFollowedChannels(twitchUsername);
            List<Channel> newLiveChannels = new List<Channel>();
            int page = 1;
            while (followedChannels.List.Count != 0)
            {
                pagingInfo.Page = page++;
                followedChannels = client.GetFollowedChannels(twitchUsername, pagingInfo); // temporarily don't care about someone else defining the username. 

                foreach (FollowedChannel f in followedChannels.List)
                {
                    if (client.IsLive(f.Channel.DisplayName))
                    {
                        newLiveChannels.Add(f.Channel);
                    }
                }
            }

            if (enableTwitchNotifications)
            {
                foreach (Channel c in newLiveChannels)
                {
                    if (!userChannels.Any(channel => channel.callbackUrl == c.Url))
                    {
                        new WeamyNotifications.Notification
                        {
                            APP_ID = "Weamy",
                            title = c.DisplayName,
                            text = c.Game,
                            text2 = c.Status,
                            imageUrl = c.Logo ?? "http://www-cdn.jtvnw.net/images/xarth/404_user_50x50.png",    // null coalescing for when stream has no logo
                            activatedCallbackFunction = openInBrowser,
                            callbackBrowser = twitchBrowser,
                            Url = c.Url,
                        }.makeToast();
                    }
                }
            }

            userChannels = new List<WeamyDataBoundObject>();
            userLiveChannels = new ObservableCollection<WeamyDataBoundObject>();
            foreach (Channel c in newLiveChannels)
            {
                string image = c.Logo ?? "http://www-cdn.jtvnw.net/images/xarth/404_user_50x50.png";

                WeamyDataBoundObject newChannel = new WeamyDataBoundObject
                {
                    title = c.DisplayName,
                    textLine1 = c.Game,
                    textLine2 = c.Status,
                    imagePath = image,
                    callbackUrl = c.Url
                };
                userChannels.Add(newChannel);
                UserLiveChannels.Add(newChannel);
            }
            lstContent.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate ()
            {
                lstContent.ItemsSource = null;
                lstContent.ItemsSource = UserLiveChannels;
                if (!twitchTimer.Enabled)
                {
                    twitchTimer.Enabled = true;
                }
            }));

        }

        private void updateYouTube(object source, System.Timers.ElapsedEventArgs e)
        {
            string[,] subs = YouTubeAPICall.GetSubs(YouTubeUsername);
            string[,] vids = YouTubeAPICall.GetVids(subs);
            List<WeamyDataBoundObject> newYTVids = new List<WeamyDataBoundObject>();
            
            for (int x = 0; x < subs.Length / 2 * 3; x++)
            {
                if (string.IsNullOrEmpty(vids[x, 2])) continue;
                string imageFilePath = "http://img.youtube.com/vi/" + vids[x, 2] + "/0.jpg";
                string URL = "https://www.youtube.com/watch?v=" + vids[x, 2];
                
                WeamyDataBoundObject newVid = new WeamyDataBoundObject
                {
                    title = vids[x, 0],
                    textLine1 = vids[x, 1],
                    imagePath = imageFilePath,
                    callbackUrl = URL
                };
                newYTVids.Add(newVid);

                if (enableYoutubeNotifications)
                {
                    if (!YTVids.Any(vid => vid.callbackUrl == URL))
                    {
                        new WeamyNotifications.Notification
                        {
                            APP_ID = "Weamy",
                            title = vids[x, 0],
                            text = vids[x, 1],
                            imageId = vids[x, 2],
                            imageUrl = imageFilePath,
                            activatedCallbackFunction = openInBrowser,
                            callbackBrowser = youtubeBrowser,
                            Url = URL,
                        }.makeToast();
                    }
                }
            }

            YTVids = new List<WeamyDataBoundObject>(newYTVids);
            youTubeVids = new ObservableCollection<WeamyDataBoundObject>(YTVids);

            YouTubeContent.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate ()
            {
                YouTubeContent.ItemsSource = null;
                YouTubeContent.ItemsSource = YouTubeVids;
                if (!twitchTimer.Enabled)
                {
                    twitchTimer.Enabled = true;
                }
            }));
        }

        private void updateGridContent()
        {

        }

        private static void openInBrowser(string url, CallbackBrowser b = CallbackBrowser.Default)
        {
            switch (b)
            {
                case CallbackBrowser.Chrome:
                    System.Diagnostics.Process.Start("chrome", url);
                    break;
                case CallbackBrowser.Firefox:
                    System.Diagnostics.Process.Start("firefox", url);
                    break;
                case CallbackBrowser.IE:
                    System.Diagnostics.Process.Start("iexplore", url);
                    break;
                default:
                    System.Diagnostics.Process.Start(url);
                    break;
            }
        }

        private void ListBoxItem_Selected(object sender, RoutedEventArgs e)
        {

        }

        private void lstContent_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                openInBrowser(lstContent.SelectedItem.ToString(), (CallbackBrowser)cbxTwitchBrowser.SelectedValue);
                lstContent.SelectedItem = null;
            }
            catch (Exception ex)
            {

            }
        }

        private void YouTubeContent_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                openInBrowser(YouTubeContent.SelectedItem.ToString(), (CallbackBrowser)cbxYouTubeBrowser.SelectedValue);
                YouTubeContent.SelectedItem = null;
            }
            catch (Exception ex)
            {

            }
        }

        private void btnSettingsClick(object sender, RoutedEventArgs e)
        {
            if (pnlSettings.Visibility == Visibility.Collapsed)
            {
                pnlMainFeed.Visibility = Visibility.Collapsed;
                pnlSettings.Visibility = Visibility.Visible;
            }
            else
            {
                pnlSettings.Visibility = Visibility.Collapsed;
                pnlMainFeed.Visibility = Visibility.Visible;
            }
        }

        private void btnSaveSettings_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtTwitchUsername.Text))
            {
                twitchUsername = txtTwitchUsername.Text;
            }
            if (!string.IsNullOrEmpty(txtYoutubeUsername.Text))
            {
                YouTubeUsername = txtYoutubeUsername.Text;
            }
            twitchBrowser = (CallbackBrowser)cbxTwitchBrowser.SelectedValue;
            youtubeBrowser = (CallbackBrowser)cbxYouTubeBrowser.SelectedValue;
            
            tickTimer = (int)Math.Floor(sldUpdateRate.Value);
        }

        private void btnEnableYoutubeNotifications_Click(object sender, RoutedEventArgs e)
        {
            if (enableYoutubeNotifications == false)
            {
                btnEnableYoutubeNotifications.Foreground = Brushes.Black;
                btnEnableYoutubeNotifications.Background = Brushes.LightGreen;
                btnEnableYoutubeNotifications.Content = "Enabled";
                enableYoutubeNotifications = true;
            }
            else
            {
                btnEnableYoutubeNotifications.Foreground = Brushes.DarkGray;
                btnEnableYoutubeNotifications.Background = Brushes.Gray;
                btnEnableYoutubeNotifications.Content = "Disabled";
                enableYoutubeNotifications = false;
            }
        }

        private void btnEnableTwitchNotifications_Click(object sender, RoutedEventArgs e)
        {
            if (enableTwitchNotifications == false)
            {
                btnEnableTwitchNotifications.Foreground = Brushes.Black;
                btnEnableTwitchNotifications.Background = Brushes.LightGreen;
                btnEnableTwitchNotifications.Content = "Enabled";
                enableTwitchNotifications = true;
            }
            else
            {
                btnEnableTwitchNotifications.Foreground = Brushes.DarkGray;
                btnEnableTwitchNotifications.Background = Brushes.Gray;
                btnEnableTwitchNotifications.Content = "Disabled";
                enableTwitchNotifications = false;
            }
        }

        private void btnNote_Click(object sender, RoutedEventArgs e)
        {
            NewNote notewin = new NewNote();
            notewin.Show();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ((App)Application.Current).notes.saveToFile("notes.xml");
        }


        private void btnNoteEdit_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as FrameworkElement).DataContext;
            int ind = lstNotes.Items.IndexOf(item);
            NewNote notewin = new NewNote();
            notewin.noteIndex = ind;
            notewin.txtTitle.Text = ((App)Application.Current).notes.notes[ind].title;
            notewin.txtBody.Text = ((App)Application.Current).notes.notes[ind].body;
            notewin.Show();
        }

        private void btnNoteDelete_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as FrameworkElement).DataContext;
            int ind = lstNotes.Items.IndexOf(item);
            ((App)Application.Current).notes.notes.RemoveAt(ind);
        }

    }
}