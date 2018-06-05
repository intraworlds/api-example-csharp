namespace IW.API.CSharp.Core
{
    using System.IO;
    using System.Net;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public class Core
    {
        private IApiAdapter apiAdapter;

        /// <summary>
        /// Initializes a new instance of the <see cref="Core"/> class.
        /// </summary>
        /// <param name="apiAdapter"> adapter, that can communicate with api </param>
        public Core(IApiAdapter apiAdapter)
        {
            this.apiAdapter = apiAdapter;
        }

        /**
        * <summary>Function GetResponse gets formatted response from api adapter.</summary>
        * <param name="url"> url of api </param>
        * <param name="method"> method we are using </param>
        * <param name="payload"> payload provided for api </param>
        * <returns>string response - json formatted response with time, response and response code</returns>
        */
        public string GetResponse(string url, string method, string payload)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            long duration;
            int responseCode = 0;
            string response = null;

            try
            {
                HttpWebResponse httpResponse = this.apiAdapter.SendRequest(url, payload, method);
                responseCode = (int)httpResponse.StatusCode;
                StreamReader sr = new StreamReader(httpResponse.GetResponseStream());
                response = sr.ReadToEnd();
                sr.Close();
            }
            catch (System.Net.WebException e)
            {
                if (e.Status == WebExceptionStatus.ProtocolError)
                {
                    var webResponse = e.Response as HttpWebResponse;
                    if (webResponse != null)
                    {
                        responseCode = (int)webResponse.StatusCode;
                        response = webResponse.ToString();
                    }
                }

                response = e.Message;
            }

            watch.Stop();
            duration = watch.ElapsedMilliseconds;
            return this.FormatResponse(duration, response, responseCode);
        }

        /**
        * <summary>Function FormatResponse formats response, time and response code into json.</summary>
        * <param name="totalTime"> meaured time the response took to arrive </param>
        * <param name="response"> response in json </param>
        * <param name="responseCode"> code of response </param>
        * <returns>string response - json formatted response with time, response and response code</returns>
        */
        private string FormatResponse(long totalTime, string response, int responseCode)
        {
            string timeAndResponse = "{\"time\":" + totalTime + ", \"Response Code\": " + responseCode + ", \"Response\": ";
            if (this.IsValidJson(response))
            {
                timeAndResponse += response + "}";
            }
            else
            {
                timeAndResponse += "\"" + response + "\"}";
            }

            return timeAndResponse;
        }

        /// <summary>
        ///  checks if parameter is valid json string
        /// </summary>
        /// <param name="stringValue"> String that we need to know if it is json</param>
        /// <returns>bool</returns>
        private bool IsValidJson(string stringValue)
        {
            if (string.IsNullOrWhiteSpace(stringValue))
            {
                return false;
            }

            var value = stringValue.Trim();

            if ((value.StartsWith("{") && value.EndsWith("}")) ||
                (value.StartsWith("[") && value.EndsWith("]")))
            {
                try
                {
                    var obj = JToken.Parse(value);
                    return true;
                }
                catch (JsonReaderException)
                {
                    return false;
                }
            }

            return false;
        }
    }
}