namespace IW.API.CSharp.Tests
{
    using System;
    using System.IO;
    using System.Net;
    using IW.API.CSharp.Core;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using NUnit.Framework;

    [TestFixture]
    public class CoreTest
    {
        private Core core;

        [SetUp]
        public void Initialize()
        {
            this.core = new Core(new TestAdapter());
        }

        [Test]
        public void Test_getResponse_success()
        {
            this.core = new Core(new TestAdapter());
            string response = this.core.GetResponse("good", string.Empty, string.Empty);
            JObject o = JObject.Parse(response);
            string jsonResponse = (string)JsonConvert.SerializeObject(o["Response"]);
            var parsedTime = o.Value<int>("time");
            var statusCode = o.Value<int>("Response Code");

            Assert.AreEqual(200, statusCode);
            Assert.AreEqual("{\"api\":\"good\"}", jsonResponse);
            Assert.IsInstanceOf(Type.GetType("System.Int32"), parsedTime);
        }

        [Test]
        public void Test_getResponse_failure()
        {
            string response = this.core.GetResponse("bad", string.Empty, string.Empty);
            JObject o = JObject.Parse(response);
            string jsonResponse = (string)JsonConvert.SerializeObject(o["Response"]);
            var statusCode = o.Value<int>("Response Code");
            var parsedTime = o.Value<int>("time");

            Assert.AreEqual("{\"api\":\"bad\"}", jsonResponse);
            Assert.AreNotEqual(200, statusCode);
            Assert.IsInstanceOf(Type.GetType("System.Int32"), parsedTime);
        }

        private class TestAdapter : IApiAdapter
        {
            public HttpWebResponse SendRequest(string url, string payload, string method)
            {
                if (url == "good")
                {
                    HttpWebResponse response = new HttpWebResponseMock();

                    return response;
                }
                else
                {
                    throw new System.Net.WebException("{\"api\":\"bad\"}");
                }
            }
        }

        private class HttpWebResponseMock : HttpWebResponse
        {
            public override HttpStatusCode StatusCode { get; }

            public override string StatusDescription { get; }

            public HttpWebResponseMock()
            {
                this.StatusCode = HttpStatusCode.OK;
                this.StatusDescription = "Response is good.";
            }

            public override Stream GetResponseStream()
            {
                string s = "{\"api\":\"good\"}";
                var stream = new MemoryStream();
                var writer = new StreamWriter(stream);
                writer.Write(s);
                writer.Flush();
                stream.Position = 0;
                return stream;
            }
        }
    }
}
