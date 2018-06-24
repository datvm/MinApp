using MinApp.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinApp.Demo.Backend
{

    public static class AppCore
    {

        public static Action<Action> RunInUiThread { get; set; }

        public static IServer Server { get; private set; } 
        public static Uri RootUri
        {
            get
            {
                return Server.RootUri;
            }
        }

        public static void Startup()
        {
            // Use another folder if needed (for example, release version require pre-compile)
            // During dev, and even release, try to use absolute path from the Assembly in case they are at other places
            // In my case, the running assembly (MinApp.Demo.ServerOnly) is in another folder
            // So I need to use path from the MinApp.Demo.Backend folder
            // In Release project, you should have the Build to copy the end folder (and run script, for example, Node, Babel etc)
#if DEBUG
            var uiFolder = @".\..\..\..\MinApp.Demo.Backend\UIDev";
#else
            var uiFolder = "UI";
#endif

            Server = new MvcServer(uiFolder);

            // Stay at a fixed port while debugging
#if DEBUG
            Server.Port = 2112;
#endif

            Server.Start();
        }

        public static void Shutdown()
        {
            Server.Dispose();
        }

    }

}
