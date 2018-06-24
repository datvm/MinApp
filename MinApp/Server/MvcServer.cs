using MinApp.Actions;
using MinApp.Routes;
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

    public class MvcServer : BaseServer
    {

        public string FileFolder { get; set; }
        public bool FileFallback { get; set; } = false;
        public IRouteTable RouteTable { get; set; }

        public bool DisableCache { get; set; } = true;

        public MvcServer() { }

        public MvcServer(string fileFolder) : this()
        {
            this.FileFallback = true;
            this.FileFolder = fileFolder;

            this.RouteTable = new DefaultRouteTable();
            this.RouteTable.LoadAttributeRoutes();
        }

        protected override async Task ProcessRequestAsync(HttpListenerContext context)
        {
            var route = this.RouteTable.Resolve(context);
            IActionResult actionResult = null;

            if (route == null && this.FileFallback)
            {
                actionResult = this.FallbackToFile(context);
            }
            else
            {
                var controllerInstance = route.ControllerConstructor
                    .Invoke(MinAppUtils.EmptyObjectArray) as ApiController;

                controllerInstance.Server = this;
                controllerInstance.Context = context;

                var methodParams = this.ParseParameters(controllerInstance, route);
                var methodResult = route.Method.Invoke(controllerInstance, methodParams);

                if (route.IsAsync)
                {
                    var resultTask = methodResult as Task;
                    await resultTask;

                    if (route.AsyncReturnType == null)
                    {
                        actionResult = StatusCodeActionResult.NoContent();
                    }
                    else
                    {
                        actionResult = ((dynamic) methodResult).Result;
                    }
                }
                else
                {
                    if (route.ReturnType == typeof(void))
                    {
                        actionResult = StatusCodeActionResult.NoContent();
                    }
                }
            }

            if (actionResult == null)
            {
                actionResult = StatusCodeActionResult.NotFound();
            }

            if (this.DisableCache)
            {
                context.Response.AddHeader("Cache-Control", "no-cache");
            }

            actionResult.WriteResponse(context);
        }

        protected FileActionResult FallbackToFile(HttpListenerContext context)
        {
            var path = context.GetPath();

            var filePath = Path.Combine(this.FileFolder, path);
            if (File.Exists(filePath))
            {
                return new FileActionResult(filePath);
            }
            else
            {
                return null;
            }
        }

        protected object[] ParseParameters(ApiController controller, MvcRouteInfo routeInfo)
        {
            var result = new object[routeInfo.Parameters.Length];

            for (int i = 0; i < routeInfo.Parameters.Length; i++)
            {
                var param = routeInfo.Parameters[i];

                var name = param.Name;
                var value = controller.Parameters[name];

                if (value != null)
                {
                    try
                    {
                        result[i] = Convert.ChangeType(value, param.ParameterType);
                    }
                    catch (Exception) { }
                }
            }

            return result;
        }

    }

}
