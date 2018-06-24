using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MinApp.Actions
{

    public class StatusCodeActionResult : IActionResult
    {

        public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.OK;

        public StatusCodeActionResult() { }

        public StatusCodeActionResult(HttpStatusCode statusCode) : this()
        {
            this.StatusCode = statusCode;
        }

        public virtual void WriteResponse(HttpListenerContext context)
        {
            context.Response.StatusCode = (int)this.StatusCode;
        }

        public static StatusCodeActionResult NoContent()
        {
            return new StatusCodeActionResult(HttpStatusCode.NoContent);
        }

        public static StatusCodeActionResult NotFound()
        {
            return new StatusCodeActionResult(HttpStatusCode.NotFound);
        }

    }

    public class StringActionResult : StatusCodeActionResult, IActionResult<string>
    {

        public string Response { get; set; } = null;

        public StringActionResult() { }

        public StringActionResult(string response) : this()
        {
            this.Response = response;
        }

        public override void WriteResponse(HttpListenerContext context)
        {
            base.WriteResponse(context);

            context.Response.ContentType = "text/plain";
            using (var streamWriter = new StreamWriter(context.Response.OutputStream))
            {
                streamWriter.Write(this.Response);
            }
        }

    }

    public class JsonActionResult : StatusCodeActionResult, IActionResult<object>
    {

        public object Response { get; set; }

        public JsonActionResult() { }

        public JsonActionResult(object response) : this()
        {
            this.Response = response;
        }

        public JsonActionResult(HttpStatusCode code, object response) : this()
        {
            this.StatusCode = code;
            this.Response = response;
        }

        public override void WriteResponse(HttpListenerContext context)
        {
            base.WriteResponse(context);

            context.Response.ContentType = "application/json";
            using (var streamWriter = new StreamWriter(context.Response.OutputStream))
            {
                streamWriter.Write(JsonConvert.SerializeObject(this.Response));
            }
        }

    }

    public class JsonActionResult<T> : JsonActionResult, IActionResult<T>
    {

        T IActionResult<T>.Response => (T)base.Response;

        public JsonActionResult() { }

        public JsonActionResult(T response) : base(response) { }

        public JsonActionResult(HttpStatusCode code, T response) : base(code, response) { }


    }

}
