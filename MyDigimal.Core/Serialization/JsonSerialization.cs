using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace MyDigimal.Core.Serialization;

public static class JsonSerialization
{
    private static readonly JsonSerializerSettings Settings = new()
    {
        ContractResolver = new CamelCasePropertyNamesContractResolver(),
        NullValueHandling = NullValueHandling.Ignore
    };

    public static string Serialize(object obj) => JsonConvert.SerializeObject(obj, Settings);
}