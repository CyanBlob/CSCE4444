using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.ComponentModel;

namespace Weamy
    {
        namespace Notes
        {
            /// <summary>
            ///  Class represents a single note (from various possible sources, or no sources...created in-app)
            /// </summary>
            public class Note: INotifyPropertyChanged
            {
                
                private string _title;
                private string _body;

                /// <summary>
                /// Default Constructor for Note
                /// </summary>
                public Note()
                {
                    _body = "";
                    _title = "";
                }

                /// <summary>
                /// Default Constructor for Note
                /// </summary>
                public Note(String title, String body)
                {
                    _body = body;
                    _title = title;
            }

            public event PropertyChangedEventHandler PropertyChanged;

            private void onPropertyChanged(object sender, string propertyName)
            {
                if (this.PropertyChanged != null)
                {
                    PropertyChanged(sender, new PropertyChangedEventArgs(propertyName));
                }
            }

            /// <summary>
            /// Get/Setter for private member _body
            /// </summary>
            /// <returns></returns>
            public String body
            {
                get
                {
                    return this._body;
                }
                set
                {
                    this._body = value;
                    onPropertyChanged(this, "Body");
                }
            }

            //    /// <summary>
            //    /// Get/Setter for private member _title
            //    /// </summary>
            //    /// <returns></returns>
            public String title
            {
                get
                {
                    return this._title;
                }
                set
                {
                    this._title = value;
                    onPropertyChanged(this, "Title");
                }
            }

        }
        }
    }

