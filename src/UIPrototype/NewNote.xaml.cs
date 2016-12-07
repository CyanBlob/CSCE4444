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
using System.Windows.Markup;

namespace UIPrototype
{
    /// <summary>
    /// Interaction logic for NewNote.xaml
    /// </summary>
    
    public partial class NewNote : Window
    {
        public NewNote()
        {
            InitializeComponent();
        }

        private void save_Click(object sender, RoutedEventArgs e)
        {
            //Weamy.Notes.NoteList.notes.Add(new Weamy.Notes.Note("blah", "blippity blah"));
            Close();
        }
    }
}
