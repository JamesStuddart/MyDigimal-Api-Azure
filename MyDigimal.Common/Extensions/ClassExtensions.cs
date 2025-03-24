using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace MyDigimal.Common.Extensions
{
    public static class ClassExtensions
    {
        public static List<string> GenerateListOfProperties(this Type type)
        {
            var listOfProperties = type.GetProperties();
            return (from prop in listOfProperties let attributes = prop.GetCustomAttributes(typeof(DescriptionAttribute), false)
                where attributes.Length <= 0 || (attributes[0] as DescriptionAttribute)?.Description != "ignore" select prop.Name).ToList();
        }
        
        public static IEnumerable<(string valueName, string oldValue, string newValue)> FindDifferences<T>(this T oldObject, T newObject)
            where T : class
        {
            var differences = new List<(string, string, string)>();

            var parentType = oldObject.GetType();

            void CompareObject(object obj1, object obj2, MemberInfo info)
            {
                if (obj1 != obj2)
                {
                    differences.Add((info.Name, $"{obj1}", $"{obj2}"));
                }
            }

            foreach (var property in parentType.GetProperties())
            {
                var value1 = property.GetValue(oldObject, null);
                var value2 = property.GetValue(newObject, null);

                if (property.PropertyType == typeof(string))
                {
                    if (string.IsNullOrEmpty(value1 as string) != string.IsNullOrEmpty(value2 as string))
                    {
                        CompareObject(value1, value2, property);
                    }
                }
                else if (property.PropertyType.IsEnum)
                {
                    if (value1 != null)
                    {
                        value1 = (int) value1;
                    }
                    
                    if (value2 != null)
                    {
                        value2 = (int) value2;
                    }
                    
                    CompareObject(value1, value2, property);
                }
                else if (property.PropertyType.IsPrimitive )
                {
                    CompareObject(value1, value2, property);
                }
            }
            
            return differences;
        }
    }
}