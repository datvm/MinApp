using MinApp.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MinApp
{
    public class ServerException : Exception
    {

        public IServer Server { get; internal set; }

        public ServerException()
        {
        }

        public ServerException(string message) : base(message)
        {
        }

        public ServerException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ServerException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

    }
}
