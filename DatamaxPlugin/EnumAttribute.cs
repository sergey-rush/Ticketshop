using System;

namespace DatamaxPlugin
{
    public class EnumAttribute : Attribute
    {
        private string _value;
        public EnumAttribute(string value)
        {
            _value = value;
        }

        public string Value
        {
            get { return _value; }
        }
    }
}