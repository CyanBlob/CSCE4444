using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Weamy
{
    namespace Notes
    {
        /// <summary>
        /// A class to encapsulate a List of notes along with
        /// Ancillary methods such as adding/removing notes
        /// and loading data from file and moving to list
        /// </summary>
        public class NoteList
        {
            /// <summary>
            /// Default Constructor initializes an empty list
            /// </summary>
            public NoteList()
            {
                notes = new List<Note> { };
            }

            /// <summary>
            /// add note to list
            /// </summary>
            /// <param name="title">blah blip</param>
            /// <param name="body"></param>
            public void add(String title, String body)
            {

            }
            
            /// <summary>
            /// Clear notes list
            /// And fill list with contents of data file (XML???)
            /// </summary>
            /// <param name="fileName"></param>
            public void loadFromFile(String fileName)
            {
                notes.Clear();

            }

            /// <summary>
            /// Removes a note from list
            /// </summary>
            public void remove()
            {

            }

            List<Note> notes;
        }
    }
}

