using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Windows.Data.Xml.Dom;
using Windows.Foundation;
using Windows.UI.Notifications;

namespace WeamyNotifications
{
    public enum ToastType { text, image }
    public enum CallbackBrowser { Default, Chrome, Firefox, IE }
    public class Notification
    {
        public string APP_ID { get; set; }
        public string imageUrl { get; set; }
        public string text { get; set; }
        public string text2 { get; set; }
        public string title { get; set; }
        public string imageId { get; set; } // deprecated -- not recommended to use -- basically just an override for automated file naming process
        public string Url { get; set; }
        public CallbackBrowser callbackBrowser = CallbackBrowser.Default;
        public Action<string, CallbackBrowser> activatedCallbackFunction { get; set; }

        public void makeToast()
        {
            try
            {
                if (!string.IsNullOrEmpty(imageUrl))    // fairly different process required for toasts with images
                {
                    int start = imageUrl.LastIndexOf("/") + 1;
                    int length = imageUrl.LastIndexOf(".") - start;
                    if (string.IsNullOrEmpty(imageId))  // make sure that imageId is not passed as override
                    {
                        imageId = imageUrl.Substring(start, length);
                    }
                    string imageFilePath = System.IO.Path.GetTempPath() + "\\Weamy_" + imageId + ".jpeg";   // set image path for file

                    int curLine = 0;
                    XmlDocument toastXml;
                    XmlNodeList stringElements;

                    // Set appropriate xml template for toast notification and insert text elements
                    if (text2 != null)
                    {
                        if (title != null)
                        {
                            toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastImageAndText04);
                            stringElements = toastXml.GetElementsByTagName("text");
                            stringElements[curLine++].AppendChild(toastXml.CreateTextNode(title));
                            stringElements[curLine++].AppendChild(toastXml.CreateTextNode(text));
                            stringElements[curLine++].AppendChild(toastXml.CreateTextNode(text2));
                        }
                        else
                        {
                            toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastImageAndText01);
                            text += "\n" + text2; // template 1 is just 1 string wrapped across multiple lines. Not sure if \n even works, but this might be fine.
                            stringElements = toastXml.GetElementsByTagName("text");
                            stringElements[curLine].AppendChild(toastXml.CreateTextNode(text));
                        }
                    }
                    else
                    {
                        if (title != null)
                        {
                            toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastImageAndText02);
                            stringElements = toastXml.GetElementsByTagName("text");
                            stringElements[curLine++].AppendChild(toastXml.CreateTextNode(title));
                            stringElements[curLine++].AppendChild(toastXml.CreateTextNode(text));
                        }
                        else
                        {
                            toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastImageAndText01);
                            stringElements = toastXml.GetElementsByTagName("text");
                            stringElements[curLine].AppendChild(toastXml.CreateTextNode(text));
                        }
                    }
                    ToastNotification toast = new ToastNotification(toastXml);

                    // callback still technically indev. Needs to be a bit more generic.
                    if (activatedCallbackFunction != null)  
                    {
                        TypedEventHandler<ToastNotification, object> activatedEventHandler = (sender, e) =>
                        {
                            if (!string.IsNullOrEmpty(Url))
                            {
                                activatedCallbackFunction(Url, callbackBrowser);
                            }
                            else throw (new Exception("No URL provided for callback"));
                        };
                        toast.Activated += activatedEventHandler;
                    }

                    // Download image if the image doesn't already exist in temp folder. Requires web client. Else image already exists for use.
                    if (!File.Exists(imageFilePath))
                    {
                        WebClient client = new WebClient();
                        client.OpenReadCompleted += (s, imageStream) =>
                        {
                            System.Drawing.Image image = System.Drawing.Image.FromStream(imageStream.Result);
                            image.Save(imageFilePath, System.Drawing.Imaging.ImageFormat.Jpeg);

                            XmlNodeList imageElements = toastXml.GetElementsByTagName("image");
                            imageElements[0].Attributes.GetNamedItem("src").NodeValue = imageFilePath;

                            ToastNotificationManager.CreateToastNotifier(APP_ID).Show(toast);
                        };
                        client.OpenReadAsync(new Uri(imageUrl));
                    }
                    else
                    {
                        XmlNodeList imageElements = toastXml.GetElementsByTagName("image");
                        imageElements[0].Attributes.GetNamedItem("src").NodeValue = imageFilePath;

                        ToastNotificationManager.CreateToastNotifier(APP_ID).Show(toast);
                    }
                }
                else
                {
                    int curLine = 0;
                    XmlDocument toastXml;
                    XmlNodeList stringElements;

                    if (text2 != null)
                    {
                        if (title != null)
                        {
                            toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText04);
                            stringElements = toastXml.GetElementsByTagName("text");
                            stringElements[curLine++].AppendChild(toastXml.CreateTextNode(title));
                            stringElements[curLine++].AppendChild(toastXml.CreateTextNode(text));
                            stringElements[curLine++].AppendChild(toastXml.CreateTextNode(text2));
                        }
                        else
                        {
                            toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText01);
                            text += "\n" + text2; // template 1 is just 1 string wrapped across multiple lines. Not sure if \n even works, but this might be fine.
                            stringElements = toastXml.GetElementsByTagName("text");
                            stringElements[curLine].AppendChild(toastXml.CreateTextNode(text));
                        }
                    }
                    else
                    {
                        if (title != null)
                        {
                            toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02);
                            stringElements = toastXml.GetElementsByTagName("text");
                            stringElements[curLine++].AppendChild(toastXml.CreateTextNode(title));
                            stringElements[curLine++].AppendChild(toastXml.CreateTextNode(text));
                        }
                        else
                        {
                            toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText01);
                            stringElements = toastXml.GetElementsByTagName("text");
                            stringElements[curLine].AppendChild(toastXml.CreateTextNode(text));
                        }
                    }

                    ToastNotification toast = new ToastNotification(toastXml);
                    if (activatedCallbackFunction != null)
                    {
                        TypedEventHandler<ToastNotification, object> activatedEventHandler = (sender, e) =>
                        {
                            if (Url != null)
                            {
                                activatedCallbackFunction(Url, callbackBrowser);
                            }
                            else throw (new Exception("No URL provided for callback"));
                        };
                        toast.Activated += activatedEventHandler;
                    }
                    ToastNotificationManager.CreateToastNotifier(APP_ID).Show(toast);
                }
            }
            catch (Exception e)
            {
                // Nothing to do atm, we don't really have an error logging system. Might just remove this try catch at some point. 
            }
        }
    }
}
