using System.Collections.Generic;

namespace SharedKernel.Common.Messages;

public record CustomMessage(string Message, Dictionary<string, string> CustomMessageTranslations, string? NegativeMessage = null, KeyValuePair<string, string>? Preposition = null);