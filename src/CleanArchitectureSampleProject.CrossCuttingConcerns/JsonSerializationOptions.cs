using Newtonsoft.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CleanArchitectureSampleProject.CrossCuttingConcerns;

public static class JsonSerializationOptions
{
    public static JsonSerializerSettings RemoveInfiniteLoop => new JsonSerializerSettings
    {
        PreserveReferencesHandling = PreserveReferencesHandling.Objects,
        Formatting = Formatting.Indented
    };

    public static JsonSerializerOptions TextJsonRemoveInfiniteLoop => new JsonSerializerOptions
    {
        ReferenceHandler = ReferenceHandler.Preserve,
        WriteIndented = true,
    };
}

public static class Json
{
    public static string SerializeObjectWithoutReferenceLoop(object? value) => JsonConvert.SerializeObject(value, JsonSerializationOptions.RemoveInfiniteLoop);
    public static string SerializeWithoutReferenceLoop(object? value) => System.Text.Json.JsonSerializer.Serialize(value, JsonSerializationOptions.TextJsonRemoveInfiniteLoop);
}
