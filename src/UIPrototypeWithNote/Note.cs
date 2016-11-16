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
                private Sources _src;
                private Uri _srcUri;

                /// <summary>
                /// Enumeration represents possible sources of note
                /// None means note was created new from app
                /// Other sources mean a quick note was created from feed content
                /// </summary>
                public enum Sources {
                    /// <summary>
                    /// Note was created new in-app
                    /// </summary>
                    None,
                    /// <summary>
                    /// Source content came from Facebook
                    /// </summary>
                    Facebook,
                    /// <summary>
                    /// Source content came from Youtube
                    /// </summary>
                    YouTube,
                    /// <summary>
                    /// Source content came from Twitch
                    /// </summary>
                    Twitch
                }

                /// <summary>
                /// Default Constructor for Note
                /// </summary>
                public Note()
                {
                    _body = "";
                    _title = "";
                    _src = Sources.None;
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

                /// <summary>
                /// Get/Setter for private member _title
                /// </summary>
                /// <returns></returns>
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

