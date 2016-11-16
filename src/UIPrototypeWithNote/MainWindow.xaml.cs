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

using System.Diagnostics;

using Weamy.Notes;

namespace UIPrototype
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        NoteList notesList;

        public MainWindow()
        {
            notesList = new NoteList();
            notesList.loadFromFile("blah");
            InitializeComponent();
        }

        private void combi_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Note_New_Click(object sender, RoutedEventArgs e)
        {
            NewNote notewin = new NewNote();
            notewin.Show();
        }
    }
}
