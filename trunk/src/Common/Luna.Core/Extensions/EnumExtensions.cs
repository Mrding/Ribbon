using System.Collections.Generic;
using System;
using System.ComponentModel;
using System.Reflection;
using System.Linq;

namespace Luna.Core.Extensions
{
    public static class EnumExtensions
    {
        private static readonly Dictionary<Type, Dictionary<Enum, string>> EnumTypeCaches;
        static EnumExtensions()
        {
            EnumTypeCaches = new Dictionary<Type, Dictionary<Enum, string>>();


        }


        public static string GetDescription(this Type type, Enum value)
        {
            if (!EnumTypeCaches.ContainsKey(type))
            {
                EnumTypeCaches[type] = new Dictionary<Enum, string>();
            }

            if(!EnumTypeCaches[type].ContainsKey(value))
            {
                var name = Enum.GetName(type, value);
                if (!string.IsNullOrEmpty(name))
                {
                    var field = type.GetField(name);
                    if (field != null)
                    {
                        var attr = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
                        if (attr != null)
                        {
                            EnumTypeCaches[type][value] = attr.Description;
                            return attr.Description;
                        }
                           
                    }
                }
            }

            return EnumTypeCaches[type][value];
        }
    }
}