using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using MyDigimal.Common.Model;

namespace MyDigimal.Common.Extensions
{
    
    /// <summary> Enum Extension Methods </summary>
    /// <typeparam name="T"> type of Enum </typeparam>
    public class Enum<T> where T : struct, IConvertible
    {
        public static T GetByDescription(string description, T defaultValue)
        {
            var enumType = typeof(T);
            if(!enumType.IsEnum) throw new InvalidOperationException();
            foreach(var field in enumType.GetFields())
            {
                if(Attribute.GetCustomAttribute(field,
                    typeof(DescriptionAttribute)) is DescriptionAttribute attribute)
                {
                    if(attribute.Description == description)
                        return (T)field.GetValue(null);
                }
                else
                {
                    if(field.Name == description)
                        return (T)field.GetValue(null);
                }
            }
            
            return defaultValue;
        }
    }

    /// <summary> Enum Extension Methods </summary>
    public static class EnumExtensions
    {
        public static IDictionary<int, string> ToDictionary(this Enum value) 
        => Enum.GetValues(value.GetType()).Cast<object>().OrderBy(x=> x).ToDictionary(Convert.ToInt32, item => item.ToString());

        public static IEnumerable<KeyValuePair<int, string>> ToList(this Enum value)
            => value.ToDictionary().Select(x => KeyValuePair.Create(x.Key, x.Value));
        
        public static IEnumerable<ReferenceDataPoint> ToDescriptiveList(this Enum value)
            => value.ToDictionary().Select(x => new ReferenceDataPoint(x.Key, x.Value, value.Description()));
              
        public static IEnumerable<ReferenceDataPoint> ToDescriptiveList<T>(
            this IEnumerable<T> values) where T : Enum
        =>  values?.Select(x =>
                new ReferenceDataPoint(Convert.ToInt32(x), Enum.GetName(typeof(T), x), x.Description()));
        
        public static string Description(this Enum value)
        {
            var enumType = value.GetType();
            if(!enumType.IsEnum) throw new InvalidOperationException();
            
            foreach(var field in enumType.GetFields())
            {
                if(Attribute.GetCustomAttribute(field,
                    typeof(DescriptionAttribute)) is DescriptionAttribute attribute)
                {
                    return attribute.Description;
                }
            }
            
            return string.Empty;
        }
    }
}