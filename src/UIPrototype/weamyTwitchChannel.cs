using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UIPrototype
{
    public class weamyTwitchChannel
    {
        // I tried to think of a good reason for all these to be private. Couldn't think of one. So now they're public. |'~'|
        public string streamTitle { get; set; }
        public string game { get; set; }
        public string displayName { get; set; }
        public string imageUrl { get; set; }
        public string url { get; set; }
        public bool live { get; set; }
        public bool whiteListed { get; set; }
        public bool blackListed { get; set; }

        public override string ToString()
        {
            return url;
        }

        public static bool operator ==(weamyTwitchChannel a, weamyTwitchChannel b)
        {
            if (a.displayName == b.displayName)
            {
                return true;
            }
            return false;
        }

        public static bool operator !=(weamyTwitchChannel a, weamyTwitchChannel b)
        {
            if (a.displayName != b.displayName)
            {
                return true;
            }
            return false;
        }
    }
}
