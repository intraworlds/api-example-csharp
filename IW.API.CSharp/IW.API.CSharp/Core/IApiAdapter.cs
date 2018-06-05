namespace IW.API.CSharp.Core
{
    using System.Net;

    /**
     * <summary>Interface for adapter for communicating with api.</summary>
     */
    public interface IApiAdapter
    {
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
        HttpWebResponse SendRequest(string url, string payload, string method);
    }
}