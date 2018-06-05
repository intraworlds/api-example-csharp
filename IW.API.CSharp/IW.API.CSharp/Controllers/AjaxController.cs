namespace IW.API.CSharp.Controllers
{
    using System;
    using IW.API.CSharp.Core;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    public class AjaxController : Controller
    {
        // POST: Ajax
        [HttpPost]
        public string Index(string url, string username, string password, string method, string payload)
        {
            string[] splitUrl = url.Split(new string[] { "/rest/" }, StringSplitOptions.None);
            string baseUrl = splitUrl[0];
            TokenStorageInSession tokenStorage = new TokenStorageInSession(baseUrl, username, this.HttpContext);
            IApiAdapter adapter = new OAuthAdapter(baseUrl, username, password, tokenStorage);
            Core core = new Core(adapter);
            string response = core.GetResponse(url, method, payload);
            return response;
        }
    }
}