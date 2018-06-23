using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MinApp.Server
{

    public class MvcServer : IServer
    {
        private const int BufferSize = 1 * 1024 * 1024; // 1MB

        public string FileFolder { get; set; }
        public bool FileFallback { get; set; } = false;

        public string HostName { get; set; } = "localhost";
        public int Port { get; set; }

        public bool IsRunning { get; protected set; }
        public bool IsDisposed { get; protected set; }

        public Uri RootUri
        {
            get
            {
                return new Uri($"http://localhost:{this.Port}/");
            }
        }

        private HttpListener HttpListener { get; set; } = new HttpListener();
        protected CancellationTokenSource ServerCancelTokenSource { get; set; } = new CancellationTokenSource();

        protected string ServerPath { get; set; }

        public MvcServer()
        {
            this.ServerPath = Path.GetFileName(this.GetType().Assembly.Location);
        }

        public MvcServer(string fileFolder) : this()
        {
            this.FileFallback = true;
            this.FileFolder = fileFolder;
        }

        public void Start()
        {
            if (this.IsRunning)
            {
                throw new InvalidOperationException("The Server is already running");
            }

            if (this.IsDisposed)
            {
                throw new InvalidOperationException("The Server is Disposed");
            }

            this.IsRunning = true;

            if (this.Port == 0)
            {
                this.Port = this.FindFreeTcpPort();
            }

            Task.Run(() =>
            {
                this.Process();
            });
        }

        private void Process()
        {
            this.ServerCancelTokenSource = new CancellationTokenSource();

            var token = this.ServerCancelTokenSource.Token;

            Task.Run(async () =>
            {
                while (!token.IsCancellationRequested)
                {
                    try
                    {
                        var context = this.HttpListener.GetContext();
                        await this.ProcessRequest(context);
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }, token);
        }

        protected virtual async Task ProcessRequest(HttpListenerContext context)
        {
            var relativePath = context.Request.Url.AbsolutePath.Substring(1);
        }

        public void Stop()
        {

        }

        public string CombineServerPath(params string[] paths)
        {
            return Path.Combine(this.ServerPath, Path.Combine(paths));
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        protected int FindFreeTcpPort()
        {
            TcpListener l = new TcpListener(IPAddress.Loopback, 0);
            l.Start();
            int port = ((IPEndPoint)l.LocalEndpoint).Port;
            l.Stop();

            return port;
        }

        
    }

}
