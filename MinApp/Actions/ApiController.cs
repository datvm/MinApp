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

        public IActionResult Ok()
        {
            return this.StatusCode(HttpStatusCode.OK);
        }

        public IActionResult NotFound()
        {
            return this.StatusCode(HttpStatusCode.NotFound);
        }

        public IActionResult BadRequest()
        {
            return this.StatusCode(HttpStatusCode.BadRequest);
        }

        public IActionResult StatusCode(HttpStatusCode statusCode)
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

        #endregion

    }

}
