using System;
using System.Collections.Generic;
using System.Linq;

namespace POSIDigitalPrinter.Utils
{
    public static class EnumUtil
    {
        public static IEnumerable<T> GetValues<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }

        public static T EnumFromString<T>(string value)
        {
            return (T)Enum.Parse(typeof(T), value);
        }
    }
}