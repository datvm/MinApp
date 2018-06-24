using MinApp.Server;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MinApp.Actions
{

    public class ApiController
    {

        public HttpListenerContext Context { get; internal set; }
        public IServer Server { get; internal set; }

        private NameValueCollection parametersField;
        public NameValueCollection Parameters
        {
            get
            {
                if (this.parametersField == null)
                {
                    this.parametersField = this.ParseQueryString();
                }

                return this.parametersField;
            }
        }

        protected async Task<string> GetBodyString()
        {
            using (var streamReader = new StreamReader(this.Context.Request.InputStream))
            {
                return await streamReader.ReadToEndAsync();
            }
        }

        protected async Task<T> GetJsonBody<T>()
        {
            var rawJson = await this.GetBodyString();
            return JsonConvert.DeserializeObject<T>(rawJson);
        }

        protected NameValueCollection ParseQueryString()
        {
            var requestUrl = this.Context.Request.Url;
            return HttpUtility.ParseQueryString(requestUrl.Query);
        }

        #region Helper Result Methods

        public StatusCodeActionResult Ok()
        {
            return this.StatusCode(HttpStatusCode.OK);
        }

        public StatusCodeActionResult NotFound()
        {
            return this.StatusCode(HttpStatusCode.NotFound);
        }

        public StatusCodeActionResult BadRequest()
        {
            return this.StatusCode(HttpStatusCode.BadRequest);
        }

        public StatusCodeActionResult StatusCode(HttpStatusCode statusCode)
        {
            return new StatusCodeActionResult(statusCode);
        }

        public JsonActionResult Json(object obj)
        {
            return new JsonActionResult(obj);
        }
        public JsonActionResult<T> Json<T>(T obj)
        {
            return new JsonActionResult<T>(obj);
        }

        public StringActionResult String(string response)
        {
            return new StringActionResult(response);
        }

        public StringActionResult String(object response)
        {
            return new StringActionResult(response.ToString());
        }

        public FileActionResult File(string filePath, string contentType = null)
        {
            return new FileActionResult(filePath, contentType);
        }

        public FileActionResult File(byte[] fileData, string contentType)
        {
            return new FileActionResult(fileData, contentType);
        }

        #endregion

    }

}
