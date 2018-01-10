using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace DatamaxPlugin
{
    public class EnumName
    {
        #region Instance implementation

        private readonly Type _enumType;
        private static readonly Hashtable StringValues = new Hashtable();
        private static readonly Dictionary<EnumAttribute, Enum> EnumValues = new Dictionary<EnumAttribute, Enum>();

        public EnumName(Type enumType)
        {
            if (!enumType.IsEnum)
            {
                throw new ArgumentException(String.Format("Supplied type must be an Enum.  Type was {0}",enumType));
            }

            _enumType = enumType;

            //if (EnumValues.Count == 0)
            //{
            //    MemberInfo[] mis = enumType.GetMembers(BindingFlags.Public | BindingFlags.Static);

            //    foreach (MemberInfo mi in mis)
            //    {
            //        EnumAttribute sa = (EnumAttribute)mi.GetCustomAttributes(typeof(EnumAttribute), false)[0];
            //    }
            //}
        }

        public Enum GetEnum(string value)
        {
            EnumAttribute sva = new EnumAttribute(value);
            return EnumValues[sva];
        }

        public bool ContainsKey(string value)
        {
            bool result = false;
            EnumAttribute sva = new EnumAttribute(value);
            if (EnumValues.ContainsKey(sva))
            {
                result = true;
            }
            return result;
        }

        /// <summary>
        /// Gets the string value associated with the given enum value.
        /// </summary>
        /// <param name="valueName">Name of the enum value.</param>
        /// <returns>String Value</returns>
        public string GetStringValue(string valueName)
        {
            Enum enumType;
            string stringValue = null;
            try
            {
                enumType = (Enum)Enum.Parse(_enumType, valueName);
                stringValue = GetStringValue(enumType);
            }
            catch
            {
                // ignored
            }

            return stringValue;
        }

        /// <summary>
        /// Gets the string values associated with the enum.
        /// </summary>
        /// <returns>String value array</returns>
        public Array GetStringValues()
        {
            ArrayList values = new ArrayList();
            foreach (FieldInfo fi in _enumType.GetFields())
            {
                EnumAttribute[] attrs = fi.GetCustomAttributes(typeof(EnumAttribute), false) as EnumAttribute[];
                if (attrs != null && attrs.Length > 0)
                {
                    values.Add(attrs[0].Value);
                }
            }

            return values.ToArray();
        }

        /// <summary>
        /// Gets the values as a 'bindable' list datasource.
        /// </summary>
        /// <returns>IList for data binding</returns>
        public IList GetListValues()
        {
            Type underlyingType = Enum.GetUnderlyingType(_enumType);
            ArrayList values = new ArrayList();
            //Look for our string value associated with fields in this enum
            foreach (FieldInfo fi in _enumType.GetFields())
            {
                //Check for our custom attribute
                EnumAttribute[] attrs = fi.GetCustomAttributes(typeof(EnumAttribute), false) as EnumAttribute[];
                if (attrs != null && attrs.Length > 0)
                {
                    values.Add(new DictionaryEntry(Convert.ChangeType(Enum.Parse(_enumType, fi.Name), underlyingType), attrs[0].Value));
                }
            }

            return values;
        }

        /// <summary>
        /// Return the existence of the given string value within the enum.
        /// </summary>
        /// <param name="stringValue">String value.</param>
        /// <returns>Existence of the string value</returns>
        public bool IsStringDefined(string stringValue)
        {
            return Parse(_enumType, stringValue) != null;
        }

        /// <summary>
        /// Return the existence of the given string value within the enum.
        /// </summary>
        /// <param name="stringValue">String value.</param>
        /// <param name="ignoreCase">Denotes whether to conduct a case-insensitive match on the supplied string value</param>
        /// <returns>Existence of the string value</returns>
        public bool IsStringDefined(string stringValue, bool ignoreCase)
        {
            return Parse(_enumType, stringValue, ignoreCase) != null;
        }

        /// <summary>
        /// Gets the underlying enum type for this instance.
        /// </summary>
        /// <value></value>
        public Type EnumType
        {
            get { return _enumType; }
        }

        #endregion

        #region Static implementation

        /// <summary>
        /// Gets a string value for a particular enum value.
        /// </summary>
        /// <param name="value">Value.</param>
        /// <returns>String Value associated via a <see cref="EnumAttribute"/> attribute, or null if not found.</returns>
        public static string GetStringValue(Enum value)
        {
            string output = null;
            Type type = value.GetType();

            if (StringValues.ContainsKey(value))
            {
                EnumAttribute enumAttribute = StringValues[value] as EnumAttribute;
                if (enumAttribute != null) output = enumAttribute.Value;
            }
            else
            {
                //Look for our 'StringValueAttribute' in the field's custom attributes
                FieldInfo fi = type.GetField(value.ToString());
                EnumAttribute[] attrs =
                    fi.GetCustomAttributes(typeof(EnumAttribute), false) as EnumAttribute[];
                if (attrs != null && attrs.Length > 0)
                {
                    StringValues.Add(value, attrs[0]);
                    output = attrs[0].Value;
                }

            }
            return output;
        }

        //public void GetEnum(string enumName)
        //{
        //    Enum result;
        //    string output = null;
        //    //Type type = value.GetType();

        //    if (_stringValues.ContainsKey(enumName))
        //        output = (_stringValues[enumName] as StringValueAttribute).Value;
        //    else
        //    {
        //        //Look for our 'StringValueAttribute' in the field's custom attributes
        //        //FieldInfo fi = type.GetField(value.ToString());
        //        //StringValueAttribute[] attrs =
        //        //    fi.GetCustomAttributes(typeof(StringValueAttribute), false) as StringValueAttribute[];
        //        //if (attrs.Length > 0)
        //        //{
        //        //    _stringValues.Add(value, attrs[0]);
        //        //    output = attrs[0].Value;
        //        //}

        //    }
        //    //return result;
        //}
        
        /// <summary>
        /// Parses the supplied enum and string value to find an associated enum value (case sensitive).
        /// </summary>
        /// <param name="type">Type.</param>
        /// <param name="stringValue">String value.</param>
        /// <returns>Enum value associated with the string value, or null if not found.</returns>
        public static object Parse(Type type, string stringValue)
        {
            return Parse(type, stringValue, false);
        }

        /// <summary>
        /// Parses the supplied enum and string value to find an associated enum value.
        /// </summary>
        /// <param name="type">Type.</param>
        /// <param name="stringValue">String value.</param>
        /// <param name="ignoreCase">Denotes whether to conduct a case-insensitive match on the supplied string value</param>
        /// <returns>Enum value associated with the string value, or null if not found.</returns>
        public static object Parse(Type type, string stringValue, bool ignoreCase)
        {
            object output = null;
            string enumStringValue = null;

            if (!type.IsEnum)
                throw new ArgumentException(String.Format("Supplied type must be an Enum.  Type was {0}", type));

            //Look for our string value associated with fields in this enum
            foreach (FieldInfo fi in type.GetFields())
            {
                //Check for our custom attribute
                EnumAttribute[] attrs = fi.GetCustomAttributes(typeof(EnumAttribute), false) as EnumAttribute[];
                if (attrs != null && attrs.Length > 0)
                    enumStringValue = attrs[0].Value;

                //Check for equality then select actual enum value.
                if (string.Compare(enumStringValue, stringValue, ignoreCase) == 0)
                {
                    output = Enum.Parse(type, fi.Name);
                    break;
                }
            }

            return output;
        }

        /// <summary>
        /// Return the existence of the given string value within the enum.
        /// </summary>
        /// <param name="stringValue">String value.</param>
        /// <param name="enumType">Type of enum</param>
        /// <returns>Existence of the string value</returns>
        public static bool IsStringDefined(Type enumType, string stringValue)
        {
            return Parse(enumType, stringValue) != null;
        }

        /// <summary>
        /// Return the existence of the given string value within the enum.
        /// </summary>
        /// <param name="stringValue">String value.</param>
        /// <param name="enumType">Type of enum</param>
        /// <param name="ignoreCase">Denotes whether to conduct a case-insensitive match on the supplied string value</param>
        /// <returns>Existence of the string value</returns>
        public static bool IsStringDefined(Type enumType, string stringValue, bool ignoreCase)
        {
            return Parse(enumType, stringValue, ignoreCase) != null;
        }

        #endregion
    }
}