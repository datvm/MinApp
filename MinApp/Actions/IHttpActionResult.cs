using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MinApp.Actions
{

    public interface IActionResult
    {
        HttpStatusCode StatusCode { get; }
        void WriteResponse(HttpListenerContext context);
    }

    public interface IActionResult<T> : IActionResult
    {
        T Response { get; }
    }

}
