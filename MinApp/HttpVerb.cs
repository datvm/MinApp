using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinApp
{
    public static class HttpVerbs
    {

        public const string Get = "GET";
        public const string Head = "HEAD";
        public const string Post = "POST";
        public const string Put = "PUT";
        public const string Delete = "DELETE";
        public const string Connect = "CONNECT";
        public const string Trace = "TRACE";
        public const string Patch = "PATCH";

    }

    public enum HttpVerb
    {
        GET,
        HEAD,
        POST,
        PUT,
        DELETE,
        CONNECT,
        TRACE,
        PATCH,
    }
}
