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

namespace UIPrototype
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        

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
        private bool enableTwitchNotifications = false;
        private bool enableYoutubeNotifications = false;

        public MainWindow()
        {
            this.DataContext = this;
            InitializeComponent();
            txtTwitchUsername.Text = twitchUsername;
            string imageFilePath = System.IO.Path.GetTempPath() + "\\Weamy_" + "404_user_50x50.png" + ".jpeg";   // set image path for file

            UserLiveChannels.Add(new WeamyDataBoundObject
            {
                title = "Updating Live Twitch Streams",
                textLine1 = "It takes a while",
                textLine2 = "The list will update automatically when it's finished",
                imagePath = imageFilePath,
                callbackUrl = "https://www.google.com"
            });

            twitchTimer = new System.Timers.Timer(1000); // initally run after 1 second. Still takes time to make the web request in the first place (a lot of time :< )
            twitchTimer.Elapsed += checkLiveChannels;
            twitchTimer.Elapsed += updateYouTube;
            twitchTimer.Enabled = true;

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
                int start = image.LastIndexOf("/") + 1;
                int length = image.LastIndexOf(".") - start;
                image = image.Substring(start, length);
                string imageFilePath = System.IO.Path.GetTempPath() + "\\Weamy_" + image + ".jpeg";   // set image path for file

                WeamyDataBoundObject newChannel = new WeamyDataBoundObject
                {
                    title = c.DisplayName,
                    textLine1 = c.Game,
                    textLine2 = c.Status,
                    imagePath = imageFilePath,
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
            //YouTubeVids.Clear();
            string[,] subs = YouTubeAPICall.GetSubs();
            string[,] vids = YouTubeAPICall.GetVids(subs);
            Debug.WriteLine("-------------------------------------------");

            YTVids = new List<WeamyDataBoundObject>();
            youTubeVids = new ObservableCollection<WeamyDataBoundObject>();
            for (int x = 0; x < subs.Length / 2 * 3; x++)
            {
                //YouTubeContent. += vids[x, 0] + " - " + vids[x, 1] + " - " + vids[x, 2] + "\n";
                Console.WriteLine(vids[x, 0] + " " + vids[x, 1] + " " + vids[x, 2]);
            
                string imageFilePath = "http://img.youtube.com/vi/" + vids[x,2] + "/0.jpg";
                string URL = "https://www.youtube.com/watch?v=" + vids[x,2];
                WeamyDataBoundObject newVid = new WeamyDataBoundObject
                {
                    title = vids[x, 0],
                    textLine1 = vids[x, 1],
                    textLine2 = "",
                    imagePath = imageFilePath,
                    callbackUrl = URL
                };
                YTVids.Add(newVid);
                YouTubeVids.Add(newVid);
            }

            if (enableYoutubeNotifications)
            {
                //Handle Youtube Notification Logic Here
            }

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
                openInBrowser(lstContent.SelectedItem.ToString());
                lstContent.SelectedItem = null;
            }
            catch(Exception ex)
            {

            }
        }
        private void YouTubeContent_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                openInBrowser(YouTubeContent.SelectedItem.ToString());
                YouTubeContent.SelectedItem = null;
            }
            catch (Exception ex)
            {

            }
        }

        private void btnSettingsClick(object sender, RoutedEventArgs e)
        {
            if(pnlSettings.Visibility == Visibility.Collapsed)
            {
                YouTubeContent.Visibility = Visibility.Collapsed;
                pnlSettings.Visibility = Visibility.Visible;
            }
            else
            {
                pnlSettings.Visibility = Visibility.Collapsed;
                YouTubeContent.Visibility = Visibility.Visible;
            }
        }

        private void btnSaveSettings_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtTwitchUsername.Text))
            {
                twitchUsername = txtTwitchUsername.Text;
            }

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
    }
}
