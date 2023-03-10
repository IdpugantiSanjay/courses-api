using System.Text.Json;
using System.Text.Json.Serialization;

namespace CourseModule.Contracts.JsonConvertors;

public class CreateResponseJsonConvertor : JsonConverter<CourseResponse>
{
    public override CourseResponse? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var readerClone = reader;
        using var jsonDocument = JsonDocument.ParseValue(ref readerClone);
        if (!jsonDocument.RootElement.TryGetProperty("kind", out var kind)) throw new JsonException();

        return kind.GetString() switch
        {
            nameof(CourseResponse.Default) => JsonSerializer.Deserialize<CourseResponse.Default>(ref reader, options),
            nameof(CourseResponse.WithEntries) => JsonSerializer.Deserialize<CourseResponse.WithEntries>(ref reader, options),
            _ => null
        };
    }

    public override void Write(Utf8JsonWriter writer, CourseResponse value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}