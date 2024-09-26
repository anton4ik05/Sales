using System.Text.Json;

namespace ClientLibrary.Helpers;

public static class Serializations
{
    public static string SerializeObj<T>(T modelObject) => JsonSerializer.Serialize(modelObject);
    public static T? DeserializeObj<T>(string json) => JsonSerializer.Deserialize<T>(json);
    public static IList<T>? DeserializeList<T>(string json) => JsonSerializer.Deserialize<IList<T>>(json);
}