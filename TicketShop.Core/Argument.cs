using System;
using System.Collections.Specialized;
using System.Reflection;

namespace TicketShop.Core
{
    public class Argument
    {
        public string MethodName { get; set; }
        public object[] ParamsObjects { get; set; }
        public NameValueCollection Nvc { get; set; }
        public Type ReturnType { get; set; }
        public Delegate Delegate { get; set; }
        public ParameterInfo[] Parameters { get; set; } 
    }
}