using System;
using System.Collections.Generic;

namespace SharedKernel.Common.Messages;

public class MessageDictionary
{
    public string Message { get; set; }

    public Dictionary<string, string> Translation { get; set; }

    public string? NegativeMessage { get; set; }

    public MessageType Type { get; set; }

    public KeyValuePair<string, string>? Preposition { get; set; }

    public MessageDictionary(
        string message,
        Dictionary<string, string> translation,
        MessageType type,
        string? negativeMessage = null,
        KeyValuePair<string, string>? preposition = null
    )
    {
        Message = message ?? throw new ArgumentNullException(nameof(message));
        Translation = translation ?? throw new ArgumentNullException(nameof(translation));
        NegativeMessage = negativeMessage;
        Type = type;
        Preposition = preposition;
    }
}