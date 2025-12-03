using System;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace SharedKernel.Extensions;

public class SerializerExtension
{
    public static SerializeResult Serialize(object data, Action<JsonSerializerOptions>? optionalOptions = null)
    {
        JsonSerializerOptions options = Options(optionalOptions);
        return new SerializeResult(JsonSerializer.Serialize(data, options), options);
    }

    public static DeserializeResult<T?> Deserialize<T>(string json, Action<JsonSerializerOptions>? optionalOptions = null)
    {
        JsonSerializerOptions options = Options(optionalOptions);
        return new DeserializeResult<T?>(JsonSerializer.Deserialize<T>(json, options), options);
    }

    public static JsonSerializerOptions Options(Action<JsonSerializerOptions>? optionalOptions = null)
    {
        JsonSerializerOptions jsonSerializerOptions = CreateOptions();
        optionalOptions?.Invoke(jsonSerializerOptions);
        return jsonSerializerOptions;
    }

    private static JsonSerializerOptions CreateOptions()
    {
        return new JsonSerializerOptions
        {
            AllowTrailingCommas = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };
    }
}