namespace IW.API.CSharp.Core
{
    using System.Net;
    using System.Text;

    /// <summary>
    /// OAuthAdapter is a class that calls functions from class Manager below and returns response from API.
    /// </summary>
    public class OAuthAdapter : IApiAdapter
    {
        private Manager oAuth;
        private string baseUrl;
        private string username;
        private string password;
        private TokenStorageInSession storage;

        /// <summary>
        /// Initializes a new instance of the <see cref="OAuthAdapter"/> class.
        /// Constructor for storing base url, consumer key, consumer secret, and storage for storing tokens in Session.
        /// </summary>
        /// <param name="baseUrl"> base url of your request. For Example: https://net.intraworlds.com/yourname </param>
        /// <param name="username">Consumer key</param>
        /// <param name="password">Consumer secret</param>
        /// <param name="storage">storage for tokens in Session</param>
        public OAuthAdapter(string baseUrl, string username, string password, TokenStorageInSession storage)
        {
            this.baseUrl = baseUrl;
            this.username = username;
            this.password = password;
            this.storage = storage;
            this.oAuth = new Manager();
            this.oAuth["consumer_key"] = this.username;
            this.oAuth["consumer_secret"] = this.password;
        }

        /**
        * <summary>Method that fetches response from intraworlds REST API from payload
         * and chosen method. Returns response in JSON if there was no error or throws
         * an Exception when an error occurs</summary>
        * <param name="url"> url for api request </param>
        * <param name="payload"> payload for api request </param>
        * <param name="method"> method for api request </param>
        * <returns>string representation of response</returns>
        * <exception cref="System.Net.WebException">Thrown when bad credentials or bad url is provided.</exception>
        */
        public HttpWebResponse SendRequest(string url, string payload, string method)
        {
            return this.SendRequestInner(url, payload, method);
        }

        private HttpWebResponse SendRequestInner(string url, string payload, string method, bool forceAccessTokenRenewal = false)
        {
            this.InitAccessToken(url, forceAccessTokenRenewal);
            if (payload == string.Empty)
            {
                payload = null;
            }

            return this.GetResponse(url, method, payload);
        }

        /// <summary>
        /// Requests token and token secret if it is not stored in TokenStorageInSession,
        /// else retrieves token from Session.
        /// </summary>
        /// <param name="url">Full url of your request</param>
        /// <param name="forceAccessTokenRenewal">boolean - indicates if token should be renewed by requesting another token</param>
        private void InitAccessToken(string url, bool forceAccessTokenRenewal = false)
        {
            string requestTokenUrl = this.baseUrl + "/remoteapi/oauth/request-token?auth=1";
            string accessTokenUrl = this.baseUrl + "/remoteapi/oauth/access-token";

            var retrievedToken = this.storage.RetrieveToken();
            var retrievedSecret = this.storage.RetrieveTokenSecret();

            if (retrievedToken == null || retrievedSecret == null || forceAccessTokenRenewal == true)
            {
                this.oAuth.AcquireRequestToken(requestTokenUrl, "POST");
                var accessResponse = this.oAuth.AcquireAccessToken(accessTokenUrl, "POST");
                this.storage.StoreToken(this.oAuth["token"], this.oAuth["token_secret"]);
            }
            else
            {
                this.oAuth["token"] = retrievedToken;
                this.oAuth["token_secret"] = retrievedSecret;
            }
        }

        /// <summary>
        /// Fetches and returns HttpWebResponse or throws WebException.
        /// </summary>
        /// <param name="url">Full url of yur request</param>
        /// <param name="method">Request method ("GET", "POST", "PUT", "DELETE")</param>
        /// <param name="payload">Payload in Json</param>
        /// <param name="forceAccessTokenRenewal">boolean value that tells us if we need to renew access token</param>
        /// <exception cref="System.Net.WebException">Thrown when bad credentials or bad url is provided.</exception>
        /// <returns>HttpWebResponse</returns>
        private HttpWebResponse GetResponse(string url, string method, string payload, bool forceAccessTokenRenewal = false)
        {
            string authzHeader = this.oAuth.GenerateAuthzHeader(url, method);
            HttpWebResponse response;

            var request = (HttpWebRequest)WebRequest.Create(url);

            request.Method = method; // method format: "GET" (its a string)

            request.PreAuthenticate = true;

            request.AllowWriteStreamBuffering = true;
            request.Headers.Add("Authorization", authzHeader);
            request.ContentType = "application/json";
            request.Accept = "application/json";

            if (payload != null)
            {
                var data = Encoding.ASCII.GetBytes(payload);
                var newStream = request.GetRequestStream();
                newStream.Write(data, 0, data.Length);
            }

            // get the response
            try
            {
                return (HttpWebResponse)request.GetResponse();
            }
            catch (WebException e)
            {
                if (e.Status == WebExceptionStatus.ProtocolError)
                {
                    response = e.Response as HttpWebResponse;

                    if (response != null && response.StatusCode == HttpStatusCode.Unauthorized && forceAccessTokenRenewal == false)
                    {
                        return this.SendRequestInner(url, payload, method, true);
                    }
                    else
                    {
                        throw e;
                    }
                }
                else
                {
                    throw e;
                }
            }
        }
    }
}