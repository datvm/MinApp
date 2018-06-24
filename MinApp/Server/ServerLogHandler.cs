using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinApp.Server
{

    public delegate void ServerLogEventHandler(object sender, ServerLogEventArgs e);

    public class ServerLogEventArgs : EventArgs
    {

        public ServerLogScope Scope { get; set; }
        public string Message { get; set; }
        public Exception Exception { get; set; }

    }

    public enum ServerLogScope
    {
        Verbose = 3,
        Information = 2,
        Warning = 1,
        Error = 0,
        Critical = -1,
    }

}
