using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reminders
{
    class Program
    {
        static void Main()
        {
            DateTime date = DateTime.Now;
            List<Reminder> reminderList = new List<Reminder>(); 

            reminderList.Add(new Reminder(date.AddSeconds(1), "one"));
            reminderList.Add(new Reminder(date.AddSeconds(2), "two"));
            reminderList.Add(new Reminder(date.AddSeconds(3), "three"));
            reminderList.Add(new Reminder(date.AddSeconds(4), "four"));
            reminderList.Add(new Reminder(date.AddSeconds(5), "five"));
            reminderList.Add(new Reminder(date.AddSeconds(6), "six"));

            // don't necessarily need to store reminders
            new Reminder(date.AddSeconds(8), null);

            Console.ReadLine();
        }
    }
}
