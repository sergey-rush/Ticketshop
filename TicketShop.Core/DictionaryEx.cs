using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;

namespace TicketShop.Core
{
    public static class DictionaryEx
    {
        public static NameValueCollection ToNameValueCollection<TKey, TValue>(this IDictionary<TKey, TValue> dict)
        {
            var nameValueCollection = new NameValueCollection();

            foreach (var kvp in dict)
            {
                if (kvp.Value != null)
                {
                    string value = kvp.Value.ToString();
                    nameValueCollection.Add(kvp.Key.ToString(), value);
                }
            }

            return nameValueCollection;
        }

        /// <summary>
        /// Converts IDictionary to a class fields, usage:MyClass someObject = dictionary.ToObject<MyClass>();
        /// where MyClass is a class with corresponding fields of the IDictionary keys
        /// </summary>
        /// <typeparam name="T">MyClass</typeparam>
        /// <param name="source">IDictionary</param>
        /// <returns>MyClass</returns>
        public static T ToObject<T>(this IDictionary<string, object> source) where T : class, new()
        {
            T someObject = new T();
            Type someObjectType = someObject.GetType();

            foreach (KeyValuePair<string, object> item in source)
            {
                someObjectType.GetProperty(item.Key).SetValue(someObject, item.Value, null);
            }

            return someObject;
        }

        /// <summary>
        /// Converts fields of a class to IDictionary<string, object>
        /// Example of usage:
        /// IDictionary<string, object> objectBackToDictionary = someObject.AsDictionary();
        /// </summary>
        /// <param name="source">Set of class fields</param>
        /// <param name="bindingAttr">BindingFlags</param>
        /// <returns>IDictionary<string, object></returns>
        public static IDictionary<string, object> AsDictionary(this object source, BindingFlags bindingAttr = BindingFlags.Public | BindingFlags.Instance)
        {
            return source.GetType().GetProperties(bindingAttr).ToDictionary
                (
                    propInfo => propInfo.Name,
                    propInfo => propInfo.GetValue(source, null)
                );

        }
    }
}