using Reminders;
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
using System.Windows.Shapes;

namespace Notifications
{
    /// <summary>
    /// Interaction logic for ReminderPopUp.xaml
    /// </summary>
    public partial class ReminderPopUp : Window
    {
        public ReminderPopUp()
        {
            InitializeComponent();
        }


        private void ScheduleReminder(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}
