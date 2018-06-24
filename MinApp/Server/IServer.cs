using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinApp.Server
{

    public interface IServer : IDisposable
    {

        string HostName { get; set; }
        int Port { get; set; }
        Uri RootUri { get; }

        bool IsRunning { get; }
        bool IsDisposed { get; }
        
        bool DisableCache { get; set; }

        void Start();
        void Stop();

        event ServerLogEventHandler Log;

    }

}
