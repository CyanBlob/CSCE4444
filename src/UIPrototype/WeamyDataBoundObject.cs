using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UIPrototype
{
    public class WeamyDataBoundObject
    {
        // I tried to think of a good reason for all these to be private. Couldn't think of one. So now they're public. |'~'|
        public string title { get; set; }
        public string textLine1 { get; set; }
        public string textLine2 { get; set; }
        public string imagePath { get; set; }       // Can be url or absolute file path
        public string callbackUrl { get; set; }     // For opening in browser when clicked
        public bool whiteListed { get; set; }       // Not implemented
        public bool blackListed { get; set; }       // Not implemented

        // For easy use as a button in a list box
        public override string ToString()
        {
            return callbackUrl;
        }

        // Comparing callbackUrl is probably the best way to test equality
        public static bool operator ==(WeamyDataBoundObject a, WeamyDataBoundObject b)
        {
            if (a.title == b.title)
            {
                return true;
            }
            return false;
        }

        // Comparing callbackUrl is probably the best way to test equality
        public static bool operator !=(WeamyDataBoundObject a, WeamyDataBoundObject b)
        {
            if (a.title != b.title)
            {
                return true;
            }
            return false;
        }
    }
}
