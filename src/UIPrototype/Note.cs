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
            ///  Class represents a single note (from various possible sources, or no sources...created in-app)
            /// </summary>
            public class Note
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
                }
            }

        }
        }
    }

