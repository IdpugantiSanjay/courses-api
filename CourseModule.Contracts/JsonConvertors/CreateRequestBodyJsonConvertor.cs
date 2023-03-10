using System.Text.Json;
using System.Text.Json.Serialization;

namespace CourseModule.Contracts.JsonConvertors;

//https://stackoverflow.com/a/69561978

public class CreateRequestBodyJsonConvertor : JsonConverter<CreateRequestBody>
{
    public override CreateRequestBody? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var readerClone = reader;
        using var jsonDocument = JsonDocument.ParseValue(ref readerClone);
        if (!jsonDocument.RootElement.TryGetProperty("type", out var typeProperty)) throw new JsonException();

        return typeProperty.GetString()?.ToLower() switch
        {
            "default" => JsonSerializer.Deserialize<CreateRequestBody.Default>(ref reader, options),
            "playlist" => JsonSerializer.Deserialize<CreateRequestBody.Playlist>(ref reader, options),
            _ => null
        };
    }

    public override void Write(Utf8JsonWriter writer, CreateRequestBody value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}