using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace TicketShop.Print.Plugin
{
    internal class DelegateTypeFactory
    {
        private readonly ModuleBuilder _moduleBuilder;
        /// <summary>
        /// Caution: creates the same delegate type over and over.
        /// </summary>
        public DelegateTypeFactory()
        {
            var assembly = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("TicketShop.PrintFactory"), AssemblyBuilderAccess.RunAndCollect);
            _moduleBuilder = assembly.DefineDynamicModule("TicketShop.PrintFactory");
        }

        public Type CreateDelegateType(MethodInfo method)
        {
            string nameBase = string.Format("{0}{1}", method.DeclaringType.Name, method.Name);
            string name = GetUniqueName(nameBase);

            var typeBuilder = _moduleBuilder.DefineType(name, TypeAttributes.Sealed | TypeAttributes.Public, typeof(MulticastDelegate));

            ConstructorBuilder constructor = typeBuilder.DefineConstructor(MethodAttributes.RTSpecialName | MethodAttributes.HideBySig | MethodAttributes.Public,
                CallingConventions.Standard, new[] { typeof(object), typeof(IntPtr) });
            constructor.SetImplementationFlags(MethodImplAttributes.CodeTypeMask);

            ParameterInfo[] parameters = method.GetParameters();

            MethodBuilder invokeMethod = typeBuilder.DefineMethod("Invoke", MethodAttributes.HideBySig | MethodAttributes.Virtual | MethodAttributes.Public,
                method.ReturnType, parameters.Select(p => p.ParameterType).ToArray());
            invokeMethod.SetImplementationFlags(MethodImplAttributes.CodeTypeMask);

            for (int i = 0; i < parameters.Length; i++)
            {
                var parameter = parameters[i];
                invokeMethod.DefineParameter(i + 1, ParameterAttributes.None, parameter.Name);
            }

            return typeBuilder.CreateType();
        }

        private string GetUniqueName(string nameBase)
        {
            int number = 2;
            string name = nameBase;
            while (_moduleBuilder.GetType(name) != null)
                name = nameBase + number++;
            return name;
        }
    }
}