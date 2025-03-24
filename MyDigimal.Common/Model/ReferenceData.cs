using System.Collections.Generic;

namespace MyDigimal.Common.Model
{
    public class ReferenceDataPoint
    {
        public ReferenceDataPoint(int key, string value, string description)
        {
            Key = key;
            Value = value;
            Description = description;
        }

        public int Key { get; } 
        public string Value { get; } 
        public string Description { get; } 
    }
}