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

    public class DefaultRouteTable : IRouteTable
    {
        private static readonly Type ApiControllerType = typeof(ApiController);
        private static readonly Type[] EmptyTypeArray = new Type[0];

        private RouteDictionary Routes { get; set; }

        public DefaultRouteTable()
        {
            this.Routes = new RouteDictionary();
        }

        public void AddController<T>()
            where T : ApiController
        {
            this.ParseController(typeof(T));
        }

        public void LoadAttributeRoutes()
        {
            var assembly = Assembly.GetEntryAssembly();

            this.ParseAssembly(assembly);
        }

        public MvcRouteInfo Resolve(HttpListenerContext context)
        {
            var path = context.GetPath();
            var verb = context.Request.HttpMethod;

            return this.Routes.Match(path, verb);
        }

        private void ParseAssembly(Assembly assembly)
        {
            var controllers = assembly.DefinedTypes
                .Where(q => q.IsSubclassOf(ApiControllerType))
                .ToList();

            foreach (var controller in controllers)
            {
                this.ParseController(controller);
            }
        }

        private void ParseController(Type controllerType)
        {
            var methods = controllerType.GetMethods();

            foreach (var method in methods)
            {
                // Must be decorated with RouteAttribute
                var routeAttribute = method.GetCustomAttribute<RouteAttribute>();

                if (routeAttribute == null)
                {
                    continue;
                }

                // Check if there is any verb
                var verbAttribute = method.GetCustomAttribute<HttpVerbAttribute>();

                var routeInfo = new MvcRouteInfo(
                    routeAttribute.Route, verbAttribute?.Verb,
                    controllerType, method);

                this.Routes.Add(routeInfo);
            }
        }

    }

    internal class RouteDictionary
    {

        /// <summary>
        /// A Dictionary that lookup by Url.
        /// </summary>
        private Dictionary<string, List<MvcRouteInfo>> routes;

        public RouteDictionary()
        {
            this.routes = new Dictionary<string, List<MvcRouteInfo>>(
                StringComparer.OrdinalIgnoreCase);
        }

        public void Add(MvcRouteInfo routeInfo)
        {
            if (!this.routes.TryGetValue(routeInfo.Url, out var urlRouteList))
            {
                urlRouteList = new List<MvcRouteInfo>();
                this.routes.Add(routeInfo.Url, urlRouteList);
            }

            // Validate
            if (urlRouteList.Any(q => q.Verb == routeInfo.Verb))
            {
                throw new ArgumentException("There is already another route with this Url and Verb");
            }

            urlRouteList.Add(routeInfo);
        }

        public MvcRouteInfo Match(string url, string verb)
        {
            if (!this.routes.TryGetValue(url, out var urlRouteList))
            {
                return null;
            }

            // Prioritize any verb-specific one
            return urlRouteList
                .Where(q =>
                    string.Equals(q.Verb, verb, StringComparison.OrdinalIgnoreCase) ||
                    q.Verb == null)
                .OrderBy(q => q.Verb == null)
                .FirstOrDefault();
        }

    }

}
