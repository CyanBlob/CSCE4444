using System;
using System.IO;
using System.Net;
using System.Text;
using System.Dynamic;
using System.Diagnostics;
using System.Windows.Forms;
using System.Threading;

static class FacebookHelpers
{
    static Uri uri;
    private static string accessToken;

    /// <summary>
    /// creates a FaceBook login URL using the apps Application ID and extra permissions (can be null)
    /// </summary>
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

    /// <summary>
    /// tests authenticating a user with FaceBook by opening a WebBrowser
    /// prints an access token and a JSON response with the user's friend count
    /// </summary>
	public static void FacebookTest()
	{
        string permissions = "public_profile, user_friends, email, user_about_me, user_birthday, user_events, user_likes, user_location, user_photos, user_posts, user_relationships, user_relationship_details, rsvp_event, user_tagged_places, pages_show_list, ";

        uri = GenerateLoginUrl("739835922850732", permissions);

        //Console.WriteLine(uri.ToString());
        //MyWebRequest myRequest = new MyWebRequest(url);
        //HttpWebRequest myHttpWebRequest = (HttpWebRequest)WebRequest.Create(uri); 
        //myHttpWebRequest.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1; .NET CLR 2.0.50727; .NET4.0C; .NET4.0E)";
        //HttpWebResponse myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();

        // create a new thread to authenticate from a WebBrowser
        var t = new Thread(() => Authenticate(uri));
        t.SetApartmentState(ApartmentState.STA);
        t.Start();

        // wait for the thread to complete, then move on
        //TODO: Handle case where we don't receive an access token
        t.Join();

        Console.WriteLine("Access token is: " + accessToken);

        // API call to read friend count of user
        string friendCountRequestURL = "https://graph.facebook.com/v2.8/me?fields=friends&access_token=";
        WebRequest friendCountRequest = WebRequest.Create(friendCountRequestURL + accessToken);

        Console.WriteLine("\nFriend query:\n" + friendCountRequestURL + accessToken);
        friendCountRequest.ContentType = "application/json; charset=utf-8";

        HttpWebResponse friendCountResponse = (HttpWebResponse)friendCountRequest.GetResponse();

        if(friendCountResponse.StatusCode == HttpStatusCode.OK)
        {
            Console.WriteLine("\nRequest succeeded and the requested information is in the response; description: {0}",
                            friendCountResponse.StatusDescription);
            string response;

            using (var sr = new StreamReader(friendCountResponse.GetResponseStream()))
            {
                response = sr.ReadToEnd();
            }

            Console.WriteLine("FRIEND RESPONSE:\n" + response);
        }
   
        else
        {
            Console.WriteLine("Something when wrong when reading the responsel description: {0}",
                            friendCountResponse.StatusDescription);
        }

        // keep console open
        Console.ReadLine();
	}

    /// <summary>
    /// creates a WebBrowser to authenticate (via OAuth2) a user. Takes a Uri as an argument. Assigns global string accessToken. Does not yet handle failures
    /// </summary>
    public static void Authenticate(Uri uri)
    {
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

        browser.Navigate(uri);
        //"https://www.facebook.com/dialog/oauth?client_id=739835922850732&redirect_uri=https:%2F%2Fwww.facebook.com%2Fconnect%2Flogin_success.html&response_type=token&display=popup"
        Application.Run(form);

        
        //Application.Exit();
        //Application.ExitThread();
        //form.Close();
        
    }

    /// <summary>
    /// callback that's triggered when the WebBrowser finishes loading a page
    /// set as callback with browser.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(browser_DocumentCompleted);
    /// </summary>
    private static void browser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
    {
        //Console.WriteLine("Document Completed");
    }

    /// <summary>
    /// callback that's triggered when the WebBrowser navigates
    /// browser.Navigated += new WebBrowserNavigatedEventHandler(browser_Navigated);
    /// assigns global string accessToken
    /// </summary>
    private static void browser_Navigated(object sender, WebBrowserNavigatedEventArgs e)
    {
        // whenever the browser navigates to a new url, try parsing the url.
        // the url may be the result of OAuth 2.0 authentication.
        //Console.WriteLine("Navigated callback");
        var fb = new Facebook.FacebookClient();
        Facebook.FacebookOAuthResult oauthResult;
        if (fb.TryParseOAuthCallbackUrl(e.Url, out oauthResult))
        {
            // the url is the result of OAuth 2.0 authentication
            if (oauthResult.IsSuccess)
            {
                accessToken = oauthResult.AccessToken;
                //Console.WriteLine(accessToken);
                WebBrowser browser = (WebBrowser) sender;

                // kill the WebBrowser
                browser.Parent.Dispose();
                return;
                //browser.Hide();

            }
            else
            {
                var errorDescription = oauthResult.ErrorDescription;
                var errorReason = oauthResult.ErrorReason;
            }
        }
        else
        {
            // the url is NOT the result of OAuth 2.0 authentication.
            Console.WriteLine("FAIL");
        }

    }
}
