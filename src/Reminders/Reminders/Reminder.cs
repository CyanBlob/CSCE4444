using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Reminders
{
    public class Reminder
    {
        private DateTime reminderDate;
        private Timer timer;
        private string reminderText;

        public Reminder(DateTime date, string text)
        {
            reminderDate = date;

            if (text == null)
            {
                text = "";
            }

            reminderText = text;

            Console.WriteLine("Creating reminder for: " + date.ToString());

            TimerCallback callback = new TimerCallback(datePassed);
            timer = new Timer(callback, null, 0, 1000);
        }

        private void datePassed(Object o)
        {
            if (DateTime.Now > reminderDate)
            {
                Console.WriteLine(DateTime.Now.ToString() + " " + reminderText);
                timer.Dispose();
            }
            else
            {
            }
        }

    }
}
