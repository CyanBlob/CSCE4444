using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace YouTubeAPI
{
    public class YouTubeAPICall
    {
        public static string[,] GetSubs(string userID)
        {
            HttpClient client = new HttpClient();

            string getSubsWithID = "https://www.googleapis.com/youtube/v3/subscriptions?part=Snippet%2CcontentDetails&channelId=" + userID + "&key=AIzaSyA-AZ4yYvaOPlGWb70p-V32n2StrmyFPiE";
            //string urlParameters = "?part=Snippet%2CcontentDetails&channelId=UCT3IDkrEU07il99Vcf15YYw&key=AIzaSyA-AZ4yYvaOPlGWb70p-V32n2StrmyFPiE";
            WebRequest subsRequest = WebRequest.Create(getSubsWithID);
            subsRequest.ContentType = "application/json; charset=utf-8";

            HttpWebResponse subsResponse = (HttpWebResponse)subsRequest.GetResponse();

            if (subsResponse.StatusCode == HttpStatusCode.OK)
            {
                /*Console.WriteLine("\nRequest succeeded and the requested information is in the response; description: {0}",
                                subsResponse.StatusDescription);*/
                string response;


                using (var sr = new StreamReader(subsResponse.GetResponseStream()))
                {
                    response = sr.ReadToEnd();
                }

                JObject jsonResponse = JObject.Parse(response);
                JArray items = (JArray)jsonResponse.SelectToken("items");
                int count = items.Count;

                string[,] subs = new string[count, 2];

                for (int x = 0; x < count; x++)
                {
                    subs[x, 0] = jsonResponse.SelectToken("items")[x].SelectToken("snippet").SelectToken("title").ToString();
                    subs[x, 1] = jsonResponse.SelectToken("items")[x].SelectToken("snippet").SelectToken("resourceId").SelectToken("channelId").ToString();
                    //Console.WriteLine(subs[x, 0] + " " + subs[x, 1]);
                }

                return (subs);
            }

            else
            {
                //Console.WriteLine("Something when wrong when reading the response description: {0}", subsResponse.StatusDescription);
            }
            //Console.ReadKey();
            return null;
        }

        public static string[,] GetVids(string[,] subs)
        {
            HttpClient client = new HttpClient();
            string[,] vids = new string[subs.Length / 2 * 3, 3];
            int count = 3;

            for (int i = 0; i < subs.Length / 2; i++)
            {
                string getSubsWithID = "https://www.googleapis.com/youtube/v3/search?key=AIzaSyA-AZ4yYvaOPlGWb70p-V32n2StrmyFPiE&channelId=" + subs[i, 1] + "&part=snippet,id&order=date&maxResults=" + count;
                // Console.WriteLine(getSubsWithID);
                // Console.ReadKey();
                WebRequest subsRequest = WebRequest.Create(getSubsWithID);
                subsRequest.ContentType = "application/json; charset=utf-8";

                HttpWebResponse subsResponse = (HttpWebResponse)subsRequest.GetResponse();

                if (subsResponse.StatusCode == HttpStatusCode.OK)
                {
                    /*Console.WriteLine("\nRequest succeeded and the requested information is in the response; description: {0}",
                                    subsResponse.StatusDescription);*/
                    string response;

                    using (var sr = new StreamReader(subsResponse.GetResponseStream()))
                    {
                        response = sr.ReadToEnd();
                    }

                    JObject jsonResponse = JObject.Parse(response);
                    JArray items = (JArray)jsonResponse.SelectToken("items");
                    //int count = items.Count;

                    for (int x = 0; x < count; x++)
                    {
                        vids[x + i * count, 0] = subs[i, 0];
                        vids[x + i * count, 1] = jsonResponse.SelectToken("items")[x].SelectToken("snippet").SelectToken("title").ToString();
                        vids[x + i * count, 2] = jsonResponse.SelectToken("items")[x].SelectToken("id").SelectToken("videoId").ToString();
                        //Console.WriteLine(vids[x + i * count, 0] + " " + vids[x + i * count, 1] + " " + vids[x + i * count, 2]);
                    }

                    Console.WriteLine();

                    //return (subs);
                }

                else
                {
                    //Console.WriteLine("Something when wrong when reading the response description: {0}", subsResponse.StatusDescription);
                }
                //Console.ReadKey();
                //return null;
            }
            for (int x = 0; x < subs.Length / 2 * 3; x++)
            {
                //Console.WriteLine(vids[x, 0] + " " + vids[x, 1] + " " + vids[x, 2]);
            }
            //Console.ReadKey();
            return (vids);
        }

        /*static void Main(string[] args)
        {
            string[,] subs = GetSubs();
            GetVids(subs);
        }*/
    }
}