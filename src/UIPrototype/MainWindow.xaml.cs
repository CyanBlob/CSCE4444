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

        private ObservableCollection<weamyTwitchChannel> userLiveChannels;
        public ObservableCollection<weamyTwitchChannel> UserLiveChannels
        {
            get
            {
                if (userLiveChannels == null)
                {
                    userLiveChannels = new ObservableCollection<weamyTwitchChannel>();
                }
                return userLiveChannels;
            }
        }
        
        private List<weamyTwitchChannel> userChannels = new List<weamyTwitchChannel>();
        private const int tickTimer = 10;
        private string twitchUsername = "Monatrox";

        public MainWindow()
        {
            this.DataContext = this;
            InitializeComponent();
            string imageFilePath = System.IO.Path.GetTempPath() + "\\Weamy_" + "404_user_50x50.png" + ".jpeg";   // set image path for file
            
            UserLiveChannels.Add(new weamyTwitchChannel
            {
                displayName = "DisplayName",
                streamTitle = "Title that is really long and should probably end up wrapping for funzies",
                game = "Game",
                imageUrl = imageFilePath,
                url = "https://www.google.com"
            });

            System.Timers.Timer twitchTimer = new System.Timers.Timer(tickTimer * 1000);
            twitchTimer.Elapsed += checkLiveChannels;
            twitchTimer.Enabled = true;
        }

        private void combi_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void btnTwitch_Click(object sender, RoutedEventArgs e)
        {
            lstContent.ItemsSource = null;
            lstContent.ItemsSource = UserLiveChannels;
        }

        private void checkLiveChannels(object source, System.Timers.ElapsedEventArgs e)
        {
            TwitchAuthenticatedClient client = new TwitchAuthenticatedClient(CLIENT_ID, AUTH_ID);
            
            if (string.IsNullOrEmpty(twitchUsername) || client.GetChannel(twitchUsername) == null)
            {
                twitchUsername = "Monatrox";
            }
            
            PagingInfo pagingInfo = new PagingInfo { Page = 1 };
            TwitchList<FollowedChannel> followedChannels = client.GetFollowedChannels("Monatrox");
            List<Channel> newLiveChannels = new List<Channel>();
            int page = 1;
            while (followedChannels.List.Count != 0)
            {
                pagingInfo.Page = page++;
                followedChannels = client.GetFollowedChannels("Monatrox", pagingInfo); // temporarily don't care about someone else defining the username. 

                foreach (FollowedChannel f in followedChannels.List)
                {
                    if (client.IsLive(f.Channel.DisplayName))
                    {
                        newLiveChannels.Add(f.Channel);
                    }
                }
            }
            foreach (Channel c in newLiveChannels)
            {
                if (!userChannels.Any(channel => channel.displayName == c.DisplayName))
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
            userChannels = new List<weamyTwitchChannel>();
            userLiveChannels = new ObservableCollection<weamyTwitchChannel>();
            foreach (Channel c in newLiveChannels)
            {
                string image = c.Logo;
                int start = image.LastIndexOf("/") + 1;
                int length = image.LastIndexOf(".") - start;
                image = image.Substring(start, length);
                string imageFilePath = System.IO.Path.GetTempPath() + "\\Weamy_" + image + ".jpeg";   // set image path for file

                weamyTwitchChannel newChannel = new weamyTwitchChannel
                {
                    displayName = c.DisplayName,
                    streamTitle = c.Status,
                    game = c.Game,
                    imageUrl = imageFilePath,
                    url = c.Url
                };
                userChannels.Add(newChannel);
                UserLiveChannels.Add(newChannel);
            }
            lstContent.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate ()
            {
                lstContent.ItemsSource = null;
                lstContent.ItemsSource = UserLiveChannels;
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
            openInBrowser(lstContent.SelectedItem.ToString());
        }
    }
}
