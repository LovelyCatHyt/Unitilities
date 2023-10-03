using Newtonsoft.Json;

namespace Unitilities.Serialization
{
    public class JsonSerializer : ITextSerializer
    {
        public static JsonSerializer Instance { get; } = new();

        public string FileExtension => "json";

        public string SerializeToText(object obj)
        {
            return JsonConvert.SerializeObject(obj, Formatting.Indented);
        }
        public T DeserializeFromText<T>(string text) where T : new()
        {
            return JsonConvert.DeserializeObject<T>(text);
        }
    }
}
