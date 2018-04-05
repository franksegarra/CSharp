using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Nap.Logger.Log4NetAppenders
{
    public static class Extensions
    {
        private static readonly HashSet<Type> NumericTypes = new HashSet<Type>
        {
            typeof(decimal),
            typeof(double),
            typeof(float),
            typeof(int),
            typeof(uint),
            typeof(long),
            typeof(ulong),
            typeof(short),
            typeof(ushort)
        };

        public static bool IsNumeric(this Type type)
        {
            return NumericTypes.Contains(type);
        }

        public static IDictionary ToDictionary(this object values)
        {
            var dict = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            if (values == null) return dict;

            foreach (PropertyDescriptor propertyDescriptor in TypeDescriptor.GetProperties(values))
            {
                var obj = propertyDescriptor.GetValue(values);
                dict.Add(propertyDescriptor.Name, obj);
            }

            return dict;
        }

        public static string TruncateMessage(this string message, int length)
        {
            return (message.Length > length)
                       ? message.Substring(0, length - 1)
                       : message;
        }

        public static bool ValidateJson(this string s)
        {
            try
            {
                JToken.Parse(s);
                return true;
            }
            catch (JsonReaderException)
            {
                return false;
            }
        }

        public static object ToJson(this string s)
        {
            return JsonConvert.DeserializeObject(s);
        }

        public static double ToUnixTimestamp(this DateTime d)
        {
            var duration = d.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0);

            return duration.TotalSeconds;
        }

        public static DateTime FromUnixTimestamp(this double d)
        {

            var datetime = new DateTime(1970, 1, 1, 0, 0, 0).AddMilliseconds(d * 1000).ToLocalTime();

            return datetime;
        }

    }
}