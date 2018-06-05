namespace IW.API.CSharp.Core
{
    using Microsoft.AspNetCore.Http;

    public class TokenStorageInSession
    {
        private string uniqueAccessTokenKey;
        private string uniqueAccessTokenSecretKey;
        private HttpContext context;

        /**
         * <summary>
         * Initializes a new instance of the <see cref="TokenStorageInSession"/> class.
         *  Constructor for setting token from user's url and consumer key.
         * </summary>
         * <param name="baseUrl"> string baseUrl     user's url</param>
         * <param name="consumerKey"> user's consumer key</param>
         * <param name="context"> HttpContext context - context that has Session in it</param>
         */
        public TokenStorageInSession(string baseUrl, string consumerKey, HttpContext context)
        {
            this.uniqueAccessTokenKey = "OAUTH_TOKEN_" + baseUrl + consumerKey;
            this.uniqueAccessTokenSecretKey = "OAUTH_TOKEN_SECRET_" + baseUrl + consumerKey;
            this.context = context;
        }

        /**
        * <summary>Method for storing token in SESSION.</summary>
        * <param name="token"> token to store</param>
        * <param name="tokenSecret"> token secret to store</param>
        */
        public void StoreToken(string token, string tokenSecret)
        {
            this.context.Session.SetString(this.uniqueAccessTokenKey, token);
            this.context.Session.SetString(this.uniqueAccessTokenSecretKey, tokenSecret);
        }

        /**
        * <summary>Method for retrieving token from SESSION.</summary>
        * <returns> stored token</returns>
        */
        public string RetrieveToken()
        {
            if (this.context.Session.GetString(this.uniqueAccessTokenKey) == null)
            {
                return null;
            }

            string token = this.context.Session.GetString(this.uniqueAccessTokenKey);
            return token;
        }
        public string RetrieveTokenSecret()
        {
            if (this.context.Session.GetString(this.uniqueAccessTokenSecretKey) == null)
            {
                return null;
            }

            string token = this.context.Session.GetString(this.uniqueAccessTokenSecretKey);
            return token;
        }

    }
}