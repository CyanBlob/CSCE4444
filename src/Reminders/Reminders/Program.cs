using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reminders
{
    class Program
    {
        static void Main(string[] args)
        {
            DateTime date = DateTime.Now;
            Reminder reminder = new Reminder(date.AddSeconds(60));
            Console.ReadLine();
        }
    }
}
