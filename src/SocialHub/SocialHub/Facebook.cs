using System;
using System.IO;
using System.Net;
using System.Text;
using System.Dynamic;
using System.Diagnostics;
//using Mono.WebBrowser;
using System.Windows.Forms;
using System.Threading;

static class FacebookHelpers
{
    static Uri uri;

        static void GetFacebookUserData(string code)
        {
            // Exchange the code for an access token
            //Uri targetUri = new Uri("https://graph.facebook.com/oauth/access_token?client_id=" + "739835922850732" + "&client_secret=" + "c4231582c8498df58a1d60a34c93089d" + "&redirect_uri=https://www.facebook.com/connect/login_success.html");
            Uri targetUri = new Uri("https://www.facebook.com/v2.7/dialog/oauth?client_id=739835922850732&redirect_uri=https://www.facebook.com/connect/login_success.html");
            Console.WriteLine(targetUri);
            HttpWebRequest at = (HttpWebRequest)HttpWebRequest.Create(targetUri);

            System.IO.StreamReader str = new System.IO.StreamReader(at.GetResponse().GetResponseStream());
            string token = str.ReadToEnd().ToString().Replace("access_token=", "");

            // Split the access token and expiration from the single string
            string[] combined = token.Split('&');
            string accessToken = combined[0];

            Console.WriteLine(accessToken);

            // Exchange the code for an extended access token
            Uri eatTargetUri = new Uri("https://www.facebook.com/v2.7/dialog/oauth?client_id=739835922850732" + "&fb_exchange_dtoken=" + accessToken);
            HttpWebRequest eat = (HttpWebRequest)HttpWebRequest.Create(eatTargetUri);

            StreamReader eatStr = new StreamReader(eat.GetResponse().GetResponseStream());
            string eatToken = eatStr.ReadToEnd().ToString().Replace("access_token=", "");

            // Split the access token and expiration from the single string
            string[] eatWords = eatToken.Split('&');
            string extendedAccessToken = eatWords[0];

            // Request the Facebook user information
            Uri targetUserUri = new Uri("https://graph.facebook.com/me?fields=first_name,last_name,gender,locale,link&access_token=" + accessToken);
            HttpWebRequest user = (HttpWebRequest)HttpWebRequest.Create(targetUserUri);

            // Read the returned JSON object response
            StreamReader userInfo = new StreamReader(user.GetResponse().GetResponseStream());
            string jsonResponse = string.Empty;
            jsonResponse = userInfo.ReadToEnd();

            /*// Deserialize and convert the JSON object to the Facebook.User object type
            JavaScriptSerializer sr = new JavaScriptSerializer();
            string jsondata = jsonResponse;
            Facebook.User converted = sr.Deserialize<Facebook.User>(jsondata);

            // Write the user data to a List
            List<Facebook.User> currentUser = new List<Facebook.User>();
            currentUser.Add(converted);

            // Return the current Facebook user
            Console.Log(currentUser + "; " + accessToken);
            //return currentUser;*/
            Console.WriteLine(accessToken);
        }
    public static void GrtUrl(string url)
    {
        HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
        webRequest.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1; .NET CLR 2.0.50727; .NET4.0C; .NET4.0E)";
        webRequest.AllowAutoRedirect = false;  // IMPORTANT

        webRequest.Timeout = 10000;           // timeout 10s
        webRequest.Method = "HEAD";
        // Get the response ...
        HttpWebResponse webResponse;
        using (webResponse = (HttpWebResponse)webRequest.GetResponse())
        {
            // Now look to see if it's a redirect
            if ((int)webResponse.StatusCode >= 300 && (int)webResponse.StatusCode <= 399)
            {
                string uriString = webResponse.Headers["Location"];
                Console.WriteLine("Redirect to " + uriString ?? "NULL");
                webResponse.Close(); // don't forget to close it - or bad things happen!
            }

        }

    }

    public static string RedirectPath(string url)
{
    StringBuilder sb = new StringBuilder();
    string location = string.Copy(url);
    while (!string.IsNullOrWhiteSpace(location))
    {
        sb.AppendLine(location); // you can also use 'Append'
        HttpWebRequest request = HttpWebRequest.CreateHttp(location);
        request.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1; .NET CLR 2.0.50727; .NET4.0C; .NET4.0E)";
        request.AllowAutoRedirect = false;
        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
        {
            location = response.GetResponseHeader("Location");
        }
    }
    return sb.ToString();
}

    public class MyWebRequest
        {
            private WebRequest request;
            private Stream dataStream;
 
            private string status;
 
            public String Status
            {
                get
                {
                    return status;
                }
                set
                {
                    status = value;
                }
            }
 
            public MyWebRequest(string url)
            {
                // Create a request using a URL that can receive a post.
 
                request = WebRequest.Create(url);
            }
 
            public MyWebRequest(string url, string method)
                : this(url)
            {
 
                if (method.Equals("GET") || method.Equals("POST"))
                {
                    // Set the Method property of the request to POST.
                    request.Method = method;
                }
                else
                {
                    throw new System.Exception("Invalid Method Type");
                }
            }
 
            public MyWebRequest(string url, string method, string data)
                : this(url, method)
            {
 
                // Create POST data and convert it to a byte array.
                string postData = data;
                byte[] byteArray = Encoding.UTF8.GetBytes(postData);
 
                // Set the ContentType property of the WebRequest.
                request.ContentType = "application/x-www-form-urlencoded";
 
                // Set the ContentLength property of the WebRequest.
                request.ContentLength = byteArray.Length;
 
                // Get the request stream.
                dataStream = request.GetRequestStream();
 
                // Write the data to the request stream.
                dataStream.Write(byteArray, 0, byteArray.Length);
 
                // Close the Stream object.
                dataStream.Close();
 
            }
 
            public string GetResponse()
            {
                // Get the original response.
                WebResponse response = request.GetResponse();
 
                this.Status = ((HttpWebResponse)response).StatusDescription;
 
                // Get the stream containing all content returned by the requested server.
                dataStream = response.GetResponseStream();
 
                // Open the stream using a StreamReader for easy access.
                StreamReader reader = new StreamReader(dataStream);
 
                // Read the content fully up to the end.
                string responseFromServer = reader.ReadToEnd();
 
                // Clean up the streams.
                reader.Close();
                dataStream.Close();
                response.Close();
 
                return responseFromServer;
            }
 
        }

    private static Uri GenerateLoginUrl(string appId, string extendedPermissions)
{
    // for .net 3.5
    // var parameters = new Dictionary<string,object>
    // parameters["client_id"] = appId;
    dynamic parameters = new ExpandoObject();
    parameters.client_id = appId;
    parameters.redirect_uri = "https://www.facebook.com/connect/login_success.html";

    // The requested response: an access token (token), an authorization code (code), or both (code token).
    parameters.response_type = "token";

    // list of additional display modes can be found at http://developers.facebook.com/docs/reference/dialogs/#display
    parameters.display = "popup";

    // add the 'scope' parameter only if we have extendedPermissions.
    if (!string.IsNullOrWhiteSpace(extendedPermissions))
        parameters.scope = extendedPermissions;

    // generate the login url
    var fb = new Facebook.FacebookClient();
    return fb.GetLoginUrl(parameters);
}

	public static void Test()
	{
		string url = "https://www.facebook.com/v2.7/dialog/oauth?client_id=739835922850732&redirect_uri=https://www.facebook.com/connect/login_success.html";
		
    /*// Create a request for the URL. 		
    WebRequest request = WebRequest.Create (url);
    // If required by the server, set the credentials.
    request.Credentials = CredentialCache.DefaultCredentials;
    // Get the response.
    HttpWebResponse response = (HttpWebResponse)request.GetResponse ();
    // Display the status.
    Console.WriteLine (response.StatusDescription);
    // Get the stream containing content returned by the server.
    Stream dataStream = response.GetResponseStream ();
    // Open the stream using a StreamReader for easy access.
    StreamReader reader = new StreamReader (dataStream);
    // Read the content.
    string responseFromServer = reader.ReadToEnd ();
    // Display the content.
    Console.WriteLine (responseFromServer);
    // Cleanup the streams and the response.
    reader.Close ();
    dataStream.Close ();
    response.Close ();*/

    /*Uri myUri = new Uri(url);
    // Create a 'HttpWebRequest' object for the specified url. 
    HttpWebRequest myHttpWebRequest = (HttpWebRequest)WebRequest.Create(myUri); 
    myHttpWebRequest.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1; .NET CLR 2.0.50727; .NET4.0C; .NET4.0E)";
    // Send the request and wait for response.
    HttpWebResponse myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse(); 
    if (myHttpWebResponse.StatusCode == HttpStatusCode.OK)
         Console.WriteLine("\nRequest succeeded and the requested information is in the response ,Description : {0}",
                    myHttpWebResponse.StatusDescription);
    if (myUri.Equals(myHttpWebResponse.ResponseUri))
    Console.WriteLine("\nThe Request Uri was not redirected by the server");
    else
    Console.WriteLine("\nThe Request Uri was redirected to :{0}",myHttpWebResponse.ResponseUri);
    // Release resources of response object.
    myHttpWebResponse.Close(); */


    /*Process proc = new Process ();
    proc.StartInfo.UseShellExecute = true;
    proc.StartInfo.FileName = url;
    proc.Start ();*/


    /*WebRequest request = WebRequest.Create(url);
    WebResponse response = request.GetResponse();
    Console.WriteLine(response.ResponseUri);

    GrtUrl(url);*/

    //MyWebRequest myRequest = new MyWebRequest(url);
    /*HttpWebRequest myHttpWebRequest = (HttpWebRequest)WebRequest.Create(url); 
    myHttpWebRequest.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1; .NET CLR 2.0.50727; .NET4.0C; .NET4.0E)";
 
    /*string accessToken =myHttpWebRequest.GetResponse().ToString().Split('&')[0];
    accessToken = accessToken.Split('=')[1];
    HttpWebResponse myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse(); 

    Console.WriteLine(myHttpWebResponse.ResponseUri);*/

    Console.WriteLine("\n\n\n");
    //Console.WriteLine(RedirectPath(url));

    Console.WriteLine("\n\n\n");
    //GetFacebookUserData(null);
    
    uri = GenerateLoginUrl("739835922850732", null);

    Console.WriteLine(uri.ToString());
    //MyWebRequest myRequest = new MyWebRequest(url);
    //HttpWebRequest myHttpWebRequest = (HttpWebRequest)WebRequest.Create(uri); 
    //myHttpWebRequest.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1; .NET CLR 2.0.50727; .NET4.0C; .NET4.0E)";
    //HttpWebResponse myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();

    var t = new Thread(whatever);
    t.SetApartmentState(ApartmentState.STA);
    t.Start();
    Console.ReadLine();
    
    

    //if (myHttpWebResponse.StatusCode == HttpStatusCode.OK)
   


	}


    public static void whatever()
    {
        Console.WriteLine("Function is working!");
        WebBrowser browser = new WebBrowser();
        browser.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(browser_DocumentCompleted);
        browser.Navigated += new WebBrowserNavigatedEventHandler(browser_Navigated);
        browser.Dock = DockStyle.Fill;
        browser.Name = "webBrowser";
        browser.ScrollBarsEnabled = false;
        browser.TabIndex = 0;
        
        Form form = new Form();
        form.WindowState = FormWindowState.Maximized;
        form.Controls.Add(browser);
        form.Name = "Browser";
        browser.AllowNavigation = true;
        
        browser.Navigate(new Uri("https://www.facebook.com/dialog/oauth?client_id=739835922850732&redirect_uri=https:%2F%2Fwww.facebook.com%2Fconnect%2Flogin_success.html&response_type=token&display=popup")); //= new Uri("https://www.facebook.com/dialog/oauth?client_id=739835922850732&redirect_uri=https:%2F%2Fwww.facebook.com%2Fconnect%2Flogin_success.html&response_type=token&display=popup");
        Application.Run(form);
        
        Thread.Sleep(2500);
        browser.Navigate(new Uri("https://google.com"));
        
        //browser.Navigate(uri);
    }
    private static void browser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
    {
        Console.WriteLine("Document Completed");
    }

    private static void browser_Navigated(object sender, WebBrowserNavigatedEventArgs e)
    {
        // whenever the browser navigates to a new url, try parsing the url.
        // the url may be the result of OAuth 2.0 authentication.
        Console.WriteLine("Navigated callback");
        var fb = new Facebook.FacebookClient();
        Facebook.FacebookOAuthResult oauthResult;
        if (fb.TryParseOAuthCallbackUrl(e.Url, out oauthResult))
        {
            // The url is the result of OAuth 2.0 authentication
            if (oauthResult.IsSuccess)
            {
                var accesstoken = oauthResult.AccessToken;
                Console.WriteLine(accesstoken);
            }
            else
            {
                var errorDescription = oauthResult.ErrorDescription;
                var errorReason = oauthResult.ErrorReason;
            }
        }
        else
        {
            // The url is NOT the result of OAuth 2.0 authentication.
            Console.WriteLine("FAIL");
        }

    }
}
