using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;


namespace YouTubeAPI
{
    public class APICall
    {
        static void Main(string[] args)
        {
            HttpClient client = new HttpClient();
            string getSubsWithoutID = "https://www.googleapis.com/youtube/v3/subscriptions?part=snippet&mine=true&key=AIzaSyA-AZ4yYvaOPlGWb70p-V32n2StrmyFPiE";


            var cookieJar = new CookieContainer();
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(getSubsWithoutID);

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string Response = reader.ReadToEnd();
            response.Close();
            Console.Write(Response);
        }
    }
}
