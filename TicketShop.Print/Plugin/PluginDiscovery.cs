using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using TicketShop.Core;
using TicketShop.Print.Base;

namespace TicketShop.Print.Plugin
{
    public class PluginDiscovery : TargetPlugin
    {
        public override bool Init()
        {
            if (Methods.Count == 0)
            {
                string path = Path.Combine(Config.Current.Directory, Config.Current.Keys["PluginFolder"]);
                if (Directory.Exists(path))
                {
                    string[] plugins = Directory.GetFiles(path).Where(file => file.ToLower().EndsWith(".dll")).ToArray();
                    if (plugins.Any())
                    {
                        foreach (string name in plugins)
                        {
                            Assembly assembly = Assembly.LoadFrom(name);
                            IEnumerable<Type> types = assembly.GetExportedTypes();
                            foreach (Type type in types)
                            {
                                Type interfaceType = type.GetInterface("IPlugin");
                                if (interfaceType != null)
                                {
                                    CreateInstance(type);
                                    break; // just one plugin can be loaded
                                }
                            }
                        }
                    }
                }
            }
            return Methods.Count > 0;
        }

        private void CreateInstance(Type type)
        {
            object instance = Activator.CreateInstance(type, true);
            MethodInfo[] methodArray = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            DelegateTypeFactory delegateTypeFactory=new DelegateTypeFactory();
            foreach (MethodInfo mi in methodArray)
            {
                Delegate del = Delegate.CreateDelegate(delegateTypeFactory.CreateDelegateType(mi), instance, mi);
                Methods.Add(mi.Name, del);
            }
        }

        //public IPlugin SelectedPrinter { get; set; }
    }
}