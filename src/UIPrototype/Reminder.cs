﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Xml;

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

                new WeamyNotifications.Notification
                {
                    APP_ID = "Weamy",
                    title = "Reminder",
                    text = reminderText,
                    //text2 = c.Status,
                    //imageUrl = c.Logo ?? "http://www-cdn.jtvnw.net/images/xarth/404_user_50x50.png",    // null coalescing for when stream has no logo
                    //activatedCallbackFunction = openInBrowser,
                    //Url = c.Url,
                }.makeToast();

                //There are a few different template toast notifications. This one happens to have 3 lines. 
                /*XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText04);

                XmlNodeList stringElements = toastXml.GetElementsByTagName("text");
                stringElements[0].AppendChild(toastXml.CreateTextNode(reminderText));
                //stringElements[1].AppendChild(toastXml.CreateTextNode("The way I was trying before was stupid"));
                //stringElements[2].AppendChild(toastXml.CreateTextNode("This is literally less than 10 lines of code..."));
                ToastNotification toast = new ToastNotification(toastXml);
                ToastNotificationManager.CreateToastNotifier("MyAppID_0001").Show(toast);*/

                timer.Dispose();
            }
            else
            {
            }
        }

    }
}