using MinApp.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MinApp.Routes
{

    public interface IRouteTable
    {

        void AddController<T>() where T : ApiController;
        void LoadAttributeRoutes(Assembly assembly);

        MvcRouteInfo Resolve(HttpListenerContext context);

    }

}
