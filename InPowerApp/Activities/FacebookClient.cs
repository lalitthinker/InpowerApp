using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace InPowerApp.Activities
{
    class FacebookClient
    {

        private const string GraphUri = "https://graph.facebook.com/";
        private const string AuthUri = "https://www.facebook.com/dialog/oauth";
        private const string RedirectUri = "https://www.facebook.com/connect/login_success.html";

        private readonly string _accessToken;

        public FacebookClient()
        {

        }

        public FacebookClient(string accessToken)
        {
            _accessToken = accessToken;

        }
        //DELETE
        public async Task<String> DeleteTaskAsync(string path)
        {

            var parser = new Parser(GraphUri, path, null, _accessToken);
            var httpClient = new HttpClient();

            var response = await httpClient.DeleteAsync(parser.Url);
            return await response.Content.ReadAsStringAsync();
        }
        //POST
        public async Task<String> PostTaskAsync(string path, object parameters)
        {

            var parser = new Parser(GraphUri, path, parameters, _accessToken);
            var httpClient = new HttpClient();

            var response = await httpClient.PostAsync(parser.Path, new StringContent(parser.Query));
            return await response.Content.ReadAsStringAsync();
        }

        //GET
        public async Task<String> GetTaskAsync(string path, object parameters)
        {

            var parser = new Parser(GraphUri, path, parameters, _accessToken);

            var httpClient = new HttpClient();
            return await httpClient.GetStringAsync(parser.Url);
        }

        //GET
        public async Task<String> GetTaskAsync(string path)
        {

            return await GetTaskAsync(path, null);
        }


       
        public Uri GetLoginUrl(string appId, string scope)
        {
            var parser = new Parser(AuthUri, "",
                new
                {
                    client_id = appId,
                    display = "touch",
                    redirect_uri = RedirectUri,
                    response_type = "token",
                    scope = scope.Replace(",", "%2C")
                }
                , null);
            return new Uri(parser.Url);
        }

   
        public bool TryParseOAuthCallbackUrl(Uri uri, out FacebookOAuthResult oauthResult)
        {

            oauthResult = new FacebookOAuthResult();

          
            var query = "";
            if (!string.IsNullOrEmpty(uri.Fragment) && uri.Fragment != "#_=_")
            {
                query = uri.Fragment.Substring(1);
            }
            else if (!string.IsNullOrEmpty(uri.Query))
            {
                query = uri.Query.Substring(1);
            }

         
            var param = new Dictionary<string, string>();
            foreach (var p in query.Split(new[] { '&' }, StringSplitOptions.RemoveEmptyEntries))
            {
                var tmp = p.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                if (tmp.Length == 2)
                {
                    var key = tmp[0].Trim();
                    var val = tmp[1].Trim();
                    param.Add(key, val);
                }
            }

           
            if (param.ContainsKey("access_token"))
            {
                oauthResult.AccessToken = param["access_token"];
                oauthResult.IsSuccess = true;
                return true;
            }

          
            if (param.ContainsKey("code") || (param.ContainsKey("error") && param.ContainsKey("error_description")))
                return true;

           
            return false;

        }

    
        class Parser
        {
            readonly Dictionary<string, string> _query = new Dictionary<string, string>();
            private readonly String _path = "";
            private readonly String _basePath = "";


         
            public Parser(string basePath, string path, object o, string accessToken)
            {

                _basePath = basePath;

            
                var query = "";
                if (path.IndexOf("?") != -1)
                {
                    var tmp = path.Split('?');
                    _path = tmp[0];
                    query = tmp[1];
                }
                else
                {
                    _path = path;
                }
                foreach (var l in query.Split(new[] { '&' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    Append(l);
                }
            
                if (o != null)
                {
                    var str = o.ToString();
                    str = str.TrimStart('{').TrimEnd('}');
                    foreach (var l in str.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        Append(l);
                    }
                }
            
                if (!String.IsNullOrEmpty(accessToken))
                {
                    _query.Add("access_token", accessToken);
                }
            }

        
            public String Path
            {
                get
                {
                    return string.Format("{0}{1}", _basePath, _path);
                }
            }
          
            public string Query
            {
                get
                {
                    var sb = new StringBuilder();
                    foreach (var a in _query)
                    {
                        if (sb.Length != 0)
                        {
                            sb.Append("&");
                        }
                        sb.Append(string.Format("{0}={1}", a.Key, a.Value));
                    }
                    return sb.ToString();
                }
            }

         
            public string Url
            {
                get
                {
                    return string.Format("{0}?{1}", Path, Query);

                }
            }

            void Append(string str)
            {
                var tmp = str.Split(new[] { '=' }, 2, StringSplitOptions.RemoveEmptyEntries);
                _query.Add(tmp[0].Trim(), tmp[1].Trim());
            }
        }


    }

  
    class FacebookOAuthResult
    {
        public bool IsSuccess { get; set; }
        public String AccessToken { get; set; }
    }
}