using Newtonsoft.Json;

namespace CleanArchitectureSampleProject.CrossCuttingConcerns;

public static class JsonSerializationOptions
{
    public static JsonSerializerSettings RemoveInfiniteLoop => new JsonSerializerSettings
    {
        PreserveReferencesHandling = PreserveReferencesHandling.Objects,
        Formatting = Formatting.Indented
    };    
}

public static class Json
{
    public static string SerializeObjectWithoutReferenceLoop(object? value) => JsonConvert.SerializeObject(value, JsonSerializationOptions.RemoveInfiniteLoop);
}
