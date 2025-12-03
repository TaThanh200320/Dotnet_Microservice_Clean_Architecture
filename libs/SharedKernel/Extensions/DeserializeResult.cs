using System.Text.Json;

namespace SharedKernel.Extensions;

public record DeserializeResult<T>(T? Object, JsonSerializerOptions Options);