using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MinApp.Routes
{

    public class MvcRouteInfo
    {

        public string Url { get; set; }
        public string Verb { get; set; }
        public Type ControllerType { get; set; }
        public MethodInfo Method { get; set; }

        public ConstructorInfo ControllerConstructor { get; private set; }

        public Type ReturnType { get; private set; }
        public bool IsAsync { get; private set; }
        public Type AsyncReturnType { get; private set; }
        public ParameterInfo[] Parameters { get; private set; }

        public MvcRouteInfo(string url, string verb, Type controllerType, MethodInfo method)
        {
            this.Url = url;
            this.Verb = verb;
            this.ControllerType = controllerType;
            this.Method = method;

            this.ControllerConstructor = this.ControllerType
                .GetConstructor(MinAppUtils.EmptyTypeArray);

            this.ReturnType = method.ReturnType;
            this.Parameters = method.GetParameters();

            this.IsAsync =
                this.ReturnType == typeof(Task) ||
                this.ReturnType.IsSubclassOf(typeof(Task));
            if (this.ReturnType.IsSubclassOfRawGeneric(typeof(Task<>)))
            {
                this.AsyncReturnType = this.ReturnType.GetGenericArguments()[0];
            }
        }

    }

}
