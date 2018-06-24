using MinApp.Demo.Backend;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MinApp.Demo.ServerOnly
{
    class Program
    {

        [STAThread]
        static void Main(string[] args)
        {
            AppCore.RunInUiThread = action =>
            {
                Console.WriteLine("Running Thread: " + Thread.CurrentThread.ManagedThreadId);
                action();
            };

            AppCore.Startup();

            AppCore.Server.Log += (sender, e) =>
            {
                if (e.Scope < Server.ServerLogScope.Verbose)
                {
                    Console.WriteLine($"[{e.Scope.ToString()}] {e.Message}");
                }
            };

            Console.WriteLine("Server listening at: " + AppCore.Server.RootUri.AbsoluteUri);

            var run = true;
            do
            {
                Console.WriteLine();
                Console.WriteLine("Commands:");
                Console.WriteLine("  s: open server Url");
                Console.WriteLine("  c: shutdown");
                Console.WriteLine("-------------------------");
                Console.Write("> ");

                var input = Console.ReadLine().Split(' ');
                var command = input[0];
                switch (command.ToLower())
                {
                    case "s":
                        Process.Start(AppCore.Server.RootUri.AbsoluteUri);
                        break;
                    case "c":
                        run = false;
                        break;
                    default:
                        Console.WriteLine("Unknown command.");
                        break;
                }
            } while (run);

            AppCore.Shutdown();
        }

    }
}
