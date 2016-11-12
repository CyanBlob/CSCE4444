using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;

//907747046009-b516vr064rad16tme955tijk588gmhik.apps.googleusercontent.com


//API Call to get subscribers. Format Below it
//https://www.googleapis.com/youtube/v3/subscriptions?part=Snippet%2CcontentDetails&channelId=UCT3IDkrEU07il99Vcf15YYw&key=AIzaSyA-AZ4yYvaOPlGWb70p-V32n2StrmyFPiE

//Format
//https://www.googleapis.com/youtube/v3/subscriptions?part=Snippet%2CcontentDetails&channelId=<ChannelID>&key=<APIKey>

//Actually, use this one. It doesn't require the youtube channel ID
//https://www.googleapis.com/youtube/v3/subscriptions?part=snippet&mine=true&key={YOUR_API_KEY}

//Return videos from channel by date
//https://www.googleapis.com/youtube/v3/search?key=<APIKey>&channelId=<channelID>&part=snippet,id&order=date&maxResults=20

namespace Google.Apis.YouTube.Samples
{
    internal class Search
    {
        [STAThread]
        static void Main(string[] args)
        {
            Console.WriteLine("YouTube Data API: Search");
            Console.WriteLine("========================");

            try
            {
                new Search().Run().Wait();
            }
            catch (AggregateException ex)
            {
                foreach (var e in ex.InnerExceptions)
                {
                    Console.WriteLine("Error: " + e.Message);
                }
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        private async Task Run()
        {
            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = "AIzaSyA-AZ4yYvaOPlGWb70p-V32n2StrmyFPiE",
                ApplicationName = this.GetType().ToString()
            });

            var searchListRequest = youtubeService.Search.List("snippet");
            searchListRequest.Q = Console.ReadLine();
            //searchListRequest.Q = "Google"; // Replace with your search term.
            searchListRequest.MaxResults = 20;

            // Call the search.list method to retrieve results matching the specified query term.
            var searchListResponse = await searchListRequest.ExecuteAsync();

            List<string> videos = new List<string>();
            List<string> channels = new List<string>();
            List<string> playlists = new List<string>();
            string[] IDs = new string[20];
            int counter = 0;

            // Add each result to the appropriate list, and then display the lists of
            // matching videos, channels, and playlists.
            foreach (var searchResult in searchListResponse.Items)
            {
                switch (searchResult.Id.Kind)
                {
                    case "youtube#video":
                        videos.Add(String.Format("{0} ({1})", searchResult.Snippet.Title, searchResult.Id.VideoId));
                        IDs[counter] = searchResult.Id.VideoId;
                        Console.WriteLine(String.Format("{0} ({1})", searchResult.Snippet.Title, searchResult.Id.VideoId));
                        Console.WriteLine("http://img.youtube.com/vi/" + IDs[counter] + "/0.jpg");
                        Console.WriteLine("https://www.youtube.com/embed/" + IDs[counter]);
                        Console.WriteLine();
                        counter++;
                        break;

                    case "youtube#channel":
                        channels.Add(String.Format("{0} ({1})", searchResult.Snippet.Title, searchResult.Id.ChannelId));
                        break;

                    case "youtube#playlist":
                        playlists.Add(String.Format("{0} ({1})", searchResult.Snippet.Title, searchResult.Id.PlaylistId));
                        break;
                }
            }

            /*Console.WriteLine(String.Format("Videos:\n{0}\n", string.Join("\n", videos)));
            Console.WriteLine(String.Format("Channels:\n{0}\n", string.Join("\n", channels)));
            Console.WriteLine(String.Format("Playlists:\n{0}\n", string.Join("\n", playlists)));
            for(int x = 0; x < 20; x++)
            {
                Console.WriteLine("http://img.youtube.com/vi/" +IDs[x] + "/0.jpg");
                Console.WriteLine("https://www.youtube.com/embed/" + IDs[x]);
            
            }*/
        }
    }
}