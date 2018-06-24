using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MinApp.Routes
{

    [AttributeUsage(AttributeTargets.Method)]
    public class RouteAttribute : Attribute
    {

        public string Route { get; set; }

        public RouteAttribute(string route)
        {
            this.Route = route;
        }

    }

    [AttributeUsage(AttributeTargets.Method)]
    public class HttpVerbAttribute : Attribute
    {
        public string Verb { get; set; }

        public HttpVerbAttribute(string verb)
        {
            this.Verb = verb;
        }

        public HttpVerbAttribute(HttpVerb verb)
        {
            this.Verb = verb.ToString();
        }

    }

    [AttributeUsage(AttributeTargets.Method)]
    public class HttpGetAttribute : HttpVerbAttribute
    {
        public HttpGetAttribute() : base(HttpVerb.GET) { }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class HttpPostAttribute : HttpVerbAttribute
    {
        public HttpPostAttribute() : base(HttpVerb.POST) { }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class HttpPutAttribute : HttpVerbAttribute
    {
        public HttpPutAttribute() : base(HttpVerb.PUT) { }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class HttpDeleteAttribute : HttpVerbAttribute
    {
        public HttpDeleteAttribute() : base(HttpVerb.DELETE) { }
    }

}
