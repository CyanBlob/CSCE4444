using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.ObjectModel;

namespace Weamy
{
    namespace Notes
    {
        /// <summary>
        /// A class to encapsulate a List of notes along with
        /// Ancillary methods such as adding/removing notes
        /// and loading data from file and moving to list
        /// </summary>
        /// 
        [Serializable]
        public class NoteList
        {
            /// <summary>
            /// Default Constructor initializes an empty list
            /// </summary>
            public NoteList()
            {
                notes = new ObservableCollection<Note> { };
            }

            //
            /// <summary>
            /// add note to list
            /// </summary>
            /// <param name="title">blah blip</param>
            /// <param name="body"></param>
            public void add(String title, String body)
            {
                Note newNote = new Note(title, body);
                notes.Insert(0, newNote);
            }

            /// <summary>
            /// Clear notes list
            /// And fill list with contents of local XML file
            /// </summary>
            /// <param name="fileName"></param>
            public void loadFromFile(String fileName = "notes.xml")
            {
                if (File.Exists(fileName))
                {
                    try
                    {
                        notes.Clear();
                        XmlSerializer cereal = new XmlSerializer(typeof(ObservableCollection<Note>));
                        FileStream fileStream = new FileStream(fileName, FileMode.Open);
                        notes = (ObservableCollection<Note>)cereal.Deserialize(fileStream);
                        fileStream.Close();
                    }
                    catch
                    {

                    }
                }
            }


            /// <summary>
            /// Clear notes list
            /// And fill list with contents of data file (XML???)
            /// </summary>
            /// <param name="fileName"></param>
            public void saveToFile(String fileName = "notes.xml")
            {
                XmlSerializer cereal = new XmlSerializer(typeof(ObservableCollection<Note>));
                TextWriter fileStream = new StreamWriter(fileName);
                cereal.Serialize(fileStream, notes);
                fileStream.Close();
            }
            public List<Note> notesDeCereal;
            public ObservableCollection<Note> notes;
        }
    }
}

