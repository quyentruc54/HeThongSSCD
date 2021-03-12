using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace NovaAlert.Common.Utils
{
    public static class EnumEx
    {
        public static Dictionary<int, string> ToDictionary(this Enum @enum)
        {
            var type = @enum.GetType();
            return Enum.GetValues(type).Cast<int>().ToDictionary(e => e, e => Enum.GetName(type, e));
        }

        public static string GetEnumDescription(Enum value)
        {
            if (value == null)
            {
                return null;
            }

            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(
                typeof(DescriptionAttribute),
                false);

            if (attributes != null && attributes.Length > 0)
            {
                return attributes[0].Description;
            }

            return value.ToString();            
        }

        public static List<KeyValuePair<T, string>> EnumToListOfKeyValuePairs<T>(Type en)
        {
            if (!en.IsEnum)
            {
                throw new InvalidOperationException("Need an enum type here.");
            }

            var list = new List<KeyValuePair<T, string>>();
            foreach (var val in Enum.GetValues(en))
            {
                list.Add(new KeyValuePair<T, string>((T)val, GetEnumDescription((Enum)val)));
            }

            return list;
        }
    }
}
