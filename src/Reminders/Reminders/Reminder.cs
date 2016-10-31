using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reminders
{
    public class Reminder
    {
        public Reminder(DateTime date)
        {
            Console.WriteLine("Creating reminder!");
            Console.WriteLine("The date is: " + date.ToString());

            while(DateTime.Now < date){};

            Console.WriteLine("REMINDER!");
        }

    }
}
