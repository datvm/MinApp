using Microsoft.VisualStudio.TestTools.UnitTesting;
using MinApp.Actions;
using MinApp.Server;
using MinApp.Test.TestData;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinApp.Test.Server
{

    [TestClass]
    public class MvcServerTests
    {

        private static IServer Server;
        private static RestClient RestClient;

        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            TestUtils.SetEntryAssembly();
            
            Server = new MvcServer("");

            Server.Log += (sender, e) =>
            {
                Debug.WriteLine($"{e.Scope}: {e.Message}");

                if (e.Exception != null)
                {
                    Debug.WriteLine(e.Exception.ToString());
                }
            };

            Server.Start();

            RestClient = new RestClient(Server.RootUri);
        }

        [TestMethod]
        public void TestHelloWorld()
        {
            var request = new RestRequest("test/hello-world", Method.GET);
            var response = RestClient.Execute(request);

            Assert.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual("text/plain", response.ContentType);
            Assert.AreEqual("Hello World", response.Content);
        }

        [TestMethod]
        public void TestEnsureNoCache()
        {
            var request = new RestRequest("test/hello-world", Method.GET);
            var response = RestClient.Execute(request);

            Assert.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual(
                "no-cache",
                response.Headers
                    .FirstOrDefault(q => q.Name == "Cache-Control").Value);
        }

        [TestMethod]
        public void TestParams()
        {
            const string content = "Hi";
            const int id = 30;
            const decimal value = 1.534M;

            var request = new RestRequest("test/params", Method.GET)
                .AddQueryParameter("content", content)
                .AddQueryParameter("id", id.ToString())
                .AddQueryParameter("value", value.ToString());
            var response = RestClient.Execute(request);

            Assert.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual($"{content} {id} {value}", response.Content);
        }

        [TestMethod]
        public void TestVoid()
        {
            var request = new RestRequest("test/void", Method.GET);
            var response = RestClient.Execute(request);

            Assert.AreEqual(System.Net.HttpStatusCode.NoContent, response.StatusCode);
        }

        [TestMethod]
        public void TestAsync()
        {
            const int id = 30;

            var request = new RestRequest("test/async", Method.GET)
                .AddQueryParameter("id", id.ToString());
            var response = RestClient.Execute(request);

            Assert.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual(id.ToString(), response.Content);
        }

        [TestMethod]
        public void TestAsyncEmpty()
        {
            var request = new RestRequest("test/async-empty", Method.GET);
            var response = RestClient.Execute(request);

            Assert.AreEqual(System.Net.HttpStatusCode.NoContent, response.StatusCode);
        }

        [TestMethod]
        public void TestFile()
        {
            var testFilePath = TestUtils.TestFilePath;
            var testFileContentType = TestUtils.TestFileContentType;

            var request = new RestRequest("test/file", Method.GET)
                .AddQueryParameter("fileName", testFilePath);
            var response = RestClient.Execute(request);

            Assert.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual(testFileContentType, response.ContentType);

            var responseBytes = response.RawBytes;
            var testFile = File.ReadAllBytes(testFilePath);

            CollectionAssert.AreEqual(testFile, responseBytes);
        }

        [TestMethod]
        public void TestJson()
        {
            const string content = "Hello";

            var request = new RestRequest("test/json", Method.GET)
                .AddQueryParameter("content", content);
            var response = RestClient.Execute(request);

            Assert.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual("application/json", response.ContentType);

            var json = JsonConvert.DeserializeObject<JObject>(response.Content);
            Assert.AreEqual(content, json["Content"].Value<string>());
        }

        [TestMethod]
        public void TestHttpVerb1()
        {
            const string content = "Hello";

            var request = new RestRequest("test/http-verb", Method.GET)
                .AddQueryParameter("content", content);
            var response = RestClient.Execute(request);

            Assert.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual("text/plain", response.ContentType);
            Assert.AreEqual(content, response.Content);
        }

        [TestMethod]
        public void TestHttpVerb2()
        {
            const string content = "Hello";

            var request = new RestRequest("test/http-verb", Method.POST)
                .AddQueryParameter("content", content);
            var response = RestClient.Execute(request);

            Assert.AreEqual(System.Net.HttpStatusCode.NotFound, response.StatusCode);
        }

        [TestMethod]
        public void TestWildcardHttpVerb1()
        {
            var request = new RestRequest("test/http-wildcard-verb", Method.GET);
            var response = RestClient.Execute(request);

            Assert.AreEqual(System.Net.HttpStatusCode.NoContent, response.StatusCode);
        }

        [TestMethod]
        public void TestWildcardHttpVerb2()
        {
            const string content = "Hello";

            var request = new RestRequest("test/http-wildcard-verb", Method.POST)
                .AddQueryParameter("content", content);
            var response = RestClient.Execute(request);

            Assert.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual("text/plain", response.ContentType);
            Assert.AreEqual(content, response.Content);
        }

        [TestMethod]
        public void TestBody()
        {
            const string content = "Hello";

            var request = new RestRequest("test/body", Method.POST)
                .AddParameter("text/plain", content, ParameterType.RequestBody);
            var response = RestClient.Execute(request);

            Assert.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual("text/plain", response.ContentType);
            Assert.AreEqual(content, response.Content);
        }

        [TestMethod]
        public void TestBodyJson()
        {
            const string content = "Hello";

            var request = new RestRequest("test/body-json", Method.POST)
                .AddJsonBody(new { content, });
            var response = RestClient.Execute(request);

            Assert.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual("application/json", response.ContentType);

            var parsedJson = JsonConvert.DeserializeObject<JObject>(response.Content);

            Assert.AreEqual(content, parsedJson["content"].Value<string>());
        }

        [ClassCleanup]
        public static void CleanUp()
        {
            Server.Dispose();
        }

    }

}
