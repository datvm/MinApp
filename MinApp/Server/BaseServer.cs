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

    public abstract class BaseServer : IServer
    {

        protected string Scheme { get; set; } = "http";
        public string HostName { get; set; } = "localhost";
        public int Port { get; set; }

        public bool DisableCache { get; set; } = true;

        public bool IsRunning { get; protected set; }
        public bool IsDisposed { get; protected set; }

        public IList<KeyValuePair<string, string>> CustomHeaders { get; private set; } = new List<KeyValuePair<string, string>>();

        public Uri RootUri
        {
            get
            {
                return new Uri($"{this.Scheme}://{this.HostName}:{this.Port}/");
            }
        }

        public event ServerLogEventHandler Log;

        protected HttpListener HttpListener { get; set; }
        protected CancellationTokenSource ServerCancelTokenSource { get; set; }

        protected string ServerPath { get; set; }

        public BaseServer()
        {
            this.ServerPath = Path.GetFileName(this.GetType().Assembly.Location);
        }

        public virtual void Start()
        {
            this.WriteVerbose("Server start requested.");

            if (this.IsRunning)
            {
                const string message = "The Server is already running";
                this.WriteError(message);
                throw new InvalidOperationException(message);
            }

            if (this.IsDisposed)
            {
                const string message = "The Server is Disposed";
                this.WriteError(message);
                throw new InvalidOperationException(message);
            }

            this.IsRunning = true;

            if (this.Port == 0)
            {
                this.WriteVerbose("No port specified. Auto-acquiring a port.");
                this.Port = this.FindFreeTcpPort();
                this.WriteVerbose($"Found port: {this.Port}.");
            }

            this.ServerCancelTokenSource = new CancellationTokenSource();

            var absoluteRootUri = this.RootUri.AbsoluteUri;
            this.HttpListener = new HttpListener();
            this.HttpListener.Prefixes.Add(absoluteRootUri);
            this.HttpListener.Start();
            this.WriteInfo("Server listening on " + absoluteRootUri);

            Task.Run(() =>
            {
                try
                {
                    this.Process();
                }
                catch (Exception ex)
                {
                    this.WriteCriticalException(ex);
                }

            });

            this.WriteVerbose("Server start successfully.");
        }

        private void Process()
        {
            try
            {
                this.WriteVerbose("Server Process Thread started: " +
                Thread.CurrentThread.ManagedThreadId);

                var token = this.ServerCancelTokenSource.Token;

                Task.Run(async () =>
                {
                    try
                    {
                        this.WriteVerbose("Listening to Requests on Thread " +
                        Thread.CurrentThread.ManagedThreadId);

                        while (!token.IsCancellationRequested)
                        {
                            try
                            {
                                var context = this.HttpListener.GetContext();
                                this.WriteInfo(context.Request.Url.AbsoluteUri);
                                this.WriteVerbose("Request received, processing...");

                                if (!token.IsCancellationRequested)
                                {
                                    try
                                    {
                                        if (this.DisableCache)
                                        {
                                            context.Response.AddHeader("Cache-Control", "no-cache");
                                        }

                                        foreach (var header in this.CustomHeaders)
                                        {
                                            context.Response.AddHeader(header.Key, header.Value);
                                        }

                                        await this.ProcessRequestAsync(context);
                                    }
                                    catch (Exception ex)
                                    {
                                        this.WriteLog("An unhandled server error occured.", ServerLogScope.Warning, ex);
                                        context.Response.StatusCode = 500;

                                        using (var streamWriter = new StreamWriter(context.Response.OutputStream))
                                        {
                                            streamWriter.WriteLine(ex.ToString());
                                        }
                                    }
                                    finally
                                    {
                                        context.Response.Close();
                                    }
                                }
                                else
                                {
                                    this.WriteVerbose("Request cancelled because server was requested to stop.");
                                }
                            }
                            catch (Exception ex)
                            {
                                this.WriteException(ex);
                            }
                        }

                        this.WriteVerbose("Stopped listening to request");
                    }
                    catch (Exception ex)
                    {
                        this.WriteCriticalException(ex);
                    }
                }, token);
            }
            catch (Exception ex)
            {
                this.WriteCriticalException(ex);
            }
        }

        protected abstract Task ProcessRequestAsync(HttpListenerContext context);

        public virtual void Stop()
        {
            try
            {
                this.WriteVerbose("Server stop requested");

                this.ServerCancelTokenSource.Cancel();
                this.HttpListener.Abort();
                this.WriteVerbose("Stopped server listening.");

                this.IsRunning = false;

                this.WriteInfo("Server stopped");
            }
            catch (Exception ex)
            {
                this.WriteException(ex);
            }
        }

        public virtual string CombineServerPath(params string[] paths)
        {
            return Path.Combine(this.ServerPath, Path.Combine(paths));
        }

        public void Dispose()
        {
            this.IsDisposed = true;
            this.Stop();
        }

        protected int FindFreeTcpPort()
        {
            TcpListener l = new TcpListener(IPAddress.Loopback, 0);
            l.Start();
            int port = ((IPEndPoint)l.LocalEndpoint).Port;
            l.Stop();

            return port;
        }

        #region Logging

        protected void WriteLog(string message, ServerLogScope scope = ServerLogScope.Information)
        {
            this.WriteLog(message, scope, null);
        }

        protected void WriteLog(string message, ServerLogScope scope, Exception exception)
        {
            this.Log?.Invoke(this, new ServerLogEventArgs()
            {
                Message = message,
                Scope = scope,
                Exception = exception,
            });
        }

        protected void WriteVerbose(string message)
        {
            this.WriteLog(message, ServerLogScope.Verbose);
        }

        protected void WriteInfo(string message)
        {
            this.WriteLog(message, ServerLogScope.Information);
        }

        protected void WriteError(string message)
        {
            this.WriteLog(message, ServerLogScope.Error);
        }

        protected void WriteException(Exception exception)
        {
            this.WriteLog(exception.Message, ServerLogScope.Error, exception);
        }

        protected void WriteCriticalException(Exception exception, bool throwAfterLog = true)
        {
            this.WriteLog(exception.Message, ServerLogScope.Critical, exception);

            if (throwAfterLog)
            {
                throw exception;
            }
        }

        #endregion

    }

}
