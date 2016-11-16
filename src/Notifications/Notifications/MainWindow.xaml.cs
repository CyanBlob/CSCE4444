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
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
using Reminders;
using YouTubeAPI;
using System.Windows.Interop;
using TwitchTester;
using System.Diagnostics;

namespace Notifications
{
    /// <summary>
    /// Very simple notification thingy
    /// </summary>
    public partial class MainWindow : Window
    {
        // being global hotkey code
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        private const int HOTKEY_ID = 9000;

        private IntPtr _windowHandle;
        private HwndSource _source;
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            _windowHandle = new WindowInteropHelper(this).Handle;
            _source = HwndSource.FromHwnd(_windowHandle);
            _source.AddHook(HwndHook);

            RegisterHotKey(_windowHandle, HOTKEY_ID, KeyCodes.CONTROL + KeyCodes.ALT, KeyCodes.T); //CTRL + ALT + T
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
                            if (vkey == KeyCodes.T)
                            {
                                double seconds = (
                                    Double.Parse(HoursText.GetLineText(0)) * 3600 +
                                    Double.Parse(MinutesText.GetLineText(0)) * 60 +
                                    Double.Parse(SecondsText.GetLineText(0)));

                                string reminderText = "";
                                int i;

                                for (i = 0; i < NotificationText.LineCount; i++)
                                {
                                    reminderText += NotificationText.GetLineText(i);
                                }

                                new Reminder(DateTime.Now.AddSeconds(seconds), NotificationText.GetLineText(0).ToString());
                                tblock.Text += "Notification set for: " + DateTime.Now.AddSeconds(seconds) + Environment.NewLine;
                            }
                            handled = true;
                            break;
                    }
                    break;
            }
            return IntPtr.Zero;
        }

        protected override void OnClosed(EventArgs e)
        {
            _source.RemoveHook(HwndHook);
            UnregisterHotKey(_windowHandle, HOTKEY_ID);
            base.OnClosed(e);
        }
        //end global hotkey code

        public MainWindow()
        {
            InitializeComponent();

            // schedule example notifications
            DateTime date = DateTime.Now;
            List<Reminder> reminderList = new List<Reminder>();

            //reminderList.Add(new Reminder(date.AddSeconds(1), "First notification"));

            // don't necessarily need to store reminders
            //new Reminder(date.AddSeconds(10), "Second notification");

            string[,] subs = YouTubeAPICall.GetSubs();
            string[,] vids = YouTubeAPICall.GetVids(subs);
            Debug.WriteLine("-------------------------------------------");
            for (int x = 0; x < subs.Length / 2 * 3; x++)
            {
                YouTubeBlock.Text += vids[x, 0] + " - " + vids[x, 1] + " - " + vids[x, 2] + "\n";
                //Console.WriteLine(vids[x, 0] + " " + vids[x, 1] + " " + vids[x, 2]);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //There are a few different template toast notifications. This one happens to have 3 lines. 
            XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText04);

            XmlNodeList stringElements = toastXml.GetElementsByTagName("text");
            stringElements[0].AppendChild(toastXml.CreateTextNode("Notifications Work!"));
            stringElements[1].AppendChild(toastXml.CreateTextNode("The way I was trying before was stupid"));
            stringElements[2].AppendChild(toastXml.CreateTextNode("This is literally less than 10 lines of code..."));
            ToastNotification toast = new ToastNotification(toastXml);
            ToastNotificationManager.CreateToastNotifier("MyAppID_0001").Show(toast);
        }
    }
}
