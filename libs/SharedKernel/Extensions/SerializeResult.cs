using System.Text.Json;

namespace SharedKernel.Extensions;

public record SerializeResult(string StringJson, JsonSerializerOptions Options);