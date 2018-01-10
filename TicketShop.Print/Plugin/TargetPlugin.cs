using System;
using System.Collections.Generic;
using TicketShop.Core;

namespace TicketShop.Print.Plugin
{
    public abstract class TargetPlugin:IPlugin
    {
        protected static readonly Dictionary<string, Delegate> Methods = new Dictionary<string, Delegate>();
        protected static readonly Dictionary<string, Type> TypeList = new Dictionary<string, Type>();
        
        private static PluginDiscovery _pluginDiscovery;
        public static TargetPlugin Instance 
        {
            get
            {
                if (_pluginDiscovery == null)
                {
                    _pluginDiscovery = new PluginDiscovery();
                }
                return _pluginDiscovery;
            }
        }

        public abstract bool Init();
        protected IPlugin Plugin;
        public static object InvokeMethod(Argument argument)
        {
            return Methods[argument.MethodName].DynamicInvoke(argument.ParamsObjects);
        }

        public static Argument GetParams(Argument argument)
        {
            argument.Parameters = Methods[argument.MethodName].Method.GetParameters();
            argument.ReturnType = Methods[argument.MethodName].Method.ReturnType;
            return argument;
        }

        public static Type GetReturnType(string method)
        {
            return Methods[method].Method.ReturnType;
        }

        public string GetName()
        {
            throw new NotImplementedException();
        }

        public void SetIp(string ip)
        {
            Argument argument = new Argument()
            {
                MethodName = "SetIp", ParamsObjects = new object[]{ip}
            };

            InvokeMethod(argument);
        }

        public bool IsReady()
        {
            throw new NotImplementedException();
        }

        public bool Print(byte[] bytes)
        {
            Argument argument = new Argument()
            {
                MethodName = "Print",
                ParamsObjects = new object[] { bytes }
            };

            return (bool)InvokeMethod(argument);
        }

        public Dictionary<string, object> RunQuery(string clause)
        {
            Argument argument = new Argument()
            {
                MethodName = "RunQuery",
                ParamsObjects = new object[] { clause }
            };

            return (Dictionary<string, object>)InvokeMethod(argument);
        }

        public bool ValidateDevice(string printerMac)
        {
            Argument argument = new Argument()
            {
                MethodName = "ValidateDevice",
                ParamsObjects = new object[] { printerMac }
            };

            return (bool)InvokeMethod(argument);
        }

        public Dictionary<string, object> GetSettings()
        {
            Argument argument = new Argument()
            {
                MethodName = "Setup",
                ParamsObjects = new object[] {  }
            };
            return (Dictionary<string, object>)InvokeMethod(argument);
        }

        public void Calibrate()
        {
            Argument argument = new Argument()
            {
                MethodName = "Calibrate",
                ParamsObjects = new object[] { }
            };
            InvokeMethod(argument);
        }

        public void Setup()
        {
            Argument argument = new Argument()
            {
                MethodName = "Setup",
                ParamsObjects = new object[] { }
            };
            InvokeMethod(argument);
        }
    }
}