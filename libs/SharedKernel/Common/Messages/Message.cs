using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using CaseConverter;
using SharedKernel.Extensions;

namespace SharedKernel.Common.Messages;

public class Message<T> where T : class
{
    private readonly Dictionary<MessageType, MessageDictionary> Messages = ErrorMessage.ErrorMessages;

    public string EntityName { get; }

    public string? PropertyName { get; internal set; } = string.Empty;

    public string ObjectName { get; internal set; } = string.Empty;

    public string? Additions { get; internal set; }

    public CustomMessage? CustomMessage { get; internal set; }

    public MessageType Type { get; internal set; }

    public bool IsNegative { get; internal set; }

    public string EnglishTranslatedMessage { get; internal set; } = string.Empty;

    public string VietnameseTranslatedMessage { get; internal set; } = string.Empty;

    public Message(string? entityName = null)
    {
        EntityName = string.IsNullOrWhiteSpace(entityName) ? typeof(T).Name : entityName;
    }

    public MessageResult BuildMessage()
    {
        string text = EntityName.ToKebabCase();
        if (!string.IsNullOrWhiteSpace(PropertyName))
        {
            text = text + "_" + PropertyName.ToKebabCase();
        }

        int num = 1;
        List<string> list = new List<string>(num);
        CollectionsMarshal.SetCount(list, num);
        Span<string> span = CollectionsMarshal.AsSpan(list);
        int num2 = 0;
        span[num2] = text;
        num2++;
        List<string> list2 = list;
        string message = CustomMessage?.Message?.ToKebabCase() ?? Messages.GetValueOrDefault(Type)?.Message;
        string text2 = CustomMessage?.NegativeMessage ?? Messages.GetValueOrDefault(Type)?.NegativeMessage;
        string item = ((IsNegative && !string.IsNullOrWhiteSpace(text2)) ? text2 : BuildMainRawMessage(IsNegative, message));
        list2.Add(item);
        if (!string.IsNullOrWhiteSpace(ObjectName))
        {
            list2.Add(ObjectName.ToKebabCase());
        }

        string en = ((!string.IsNullOrWhiteSpace(EnglishTranslatedMessage)) ? EnglishTranslatedMessage : Translate(LanguageType.En));
        string vi = ((!string.IsNullOrWhiteSpace(VietnameseTranslatedMessage)) ? VietnameseTranslatedMessage : Translate(LanguageType.Vi));
        return new MessageResult
        {
            Message = string.Join("_", list2).ToLower(),
            En = en,
            Vi = vi
        };
    }

    private string Translate(LanguageType languageType)
    {
        string path = Path.Join(Directory.GetCurrentDirectory(), "Resources");
        Dictionary<string, ResourceResult> dictionary = ResourceExtension.ReadResxFile(languageType switch
        {
            LanguageType.Vi => Path.Join(path, "Translations", "Message.vi.resx"),
            LanguageType.En => Path.Join(path, "Translations", "Message.en.resx"),
            _ => string.Empty,
        }) ?? new Dictionary<string, ResourceResult>();
        ResourceResult valueOrDefault = dictionary.GetValueOrDefault(PropertyName);
        string text = valueOrDefault?.Value ?? string.Empty;
        string text2 = dictionary.GetValueOrDefault(EntityName)?.Value ?? string.Empty;
        string text3 = dictionary.GetValueOrDefault(ObjectName)?.Value ?? string.Empty;
        string text4 = string.Empty;
        MessageDictionary messageDictionary = ((Type == (MessageType)0) ? null : Messages.GetValueOrDefault(Type));
        string viTranslation = null;
        if (languageType == LanguageType.Vi)
        {
            string[] array = ((string.IsNullOrWhiteSpace(Additions) ? valueOrDefault?.Comment?.Trim()?.Split(",") : Additions?.Trim()?.Split(","))?.FirstOrDefault(delegate (string x)
            {
                string[] array3 = x.Split("=");
                return array3.Length != 0 && array3[0] == "ViToBeTranslation";
            }))?.Split("=");
            viTranslation = ((array != null && array.Length != 0) ? array[1] : null);
        }

        string text5 = BuildMainTranslationMessage(IsNegative, CustomMessage?.NegativeMessage ?? messageDictionary?.NegativeMessage, CustomMessage?.CustomMessageTranslations[languageType.ToString()] ?? messageDictionary?.Translation[languageType.ToString()], languageType, viTranslation);
        CustomMessage? customMessage = CustomMessage;
        if ((((object)customMessage != null && customMessage.Preposition.HasValue) || (messageDictionary != null && messageDictionary.Preposition.HasValue)) && !string.IsNullOrWhiteSpace(text3))
        {
            text4 = ((languageType != LanguageType.En) ? (CustomMessage?.Preposition?.Value ?? messageDictionary?.Preposition?.Value ?? "") : (CustomMessage?.Preposition?.Key ?? messageDictionary?.Preposition?.Key ?? ""));
        }

        string text6 = string.Empty;
        if (languageType == LanguageType.En)
        {
            string[] array2 = valueOrDefault?.Comment?.Trim()?.Split(",");
            text6 = ((array2 != null && array2.Any(delegate (string x)
            {
                string[] array3 = x.Split("=");
                return array3[0] == "IsPlural" && array3[1] == "true";
            })) ? "are" : "is");
        }

        string text7 = (string.IsNullOrWhiteSpace(text) ? string.Empty : ((languageType == LanguageType.En) ? "of" : "của"));
        int num = 7;
        List<string> list = new List<string>(num);
        CollectionsMarshal.SetCount(list, num);
        Span<string> span = CollectionsMarshal.AsSpan(list);
        int num2 = 0;
        span[num2] = text;
        num2++;
        span[num2] = text7;
        num2++;
        span[num2] = text2;
        num2++;
        span[num2] = text6;
        num2++;
        span[num2] = text5;
        num2++;
        span[num2] = text4;
        num2++;
        span[num2] = text3;
        num2++;
        IReadOnlyCollection<string> values = list.FindAll((string word) => !string.IsNullOrWhiteSpace(word));
        return string.Join(' ', values);
    }

    private static string? BuildMainRawMessage(bool isNegative, string? message)
    {
        if (!isNegative)
        {
            return message;
        }

        string text = (string.IsNullOrWhiteSpace(message) ? string.Empty : ("_" + message));
        return "not" + text;
    }

    private static string? BuildMainTranslationMessage(bool isNegative, string? negativeMessage, string? message, LanguageType languageType, string? viTranslation = null)
    {
        if (languageType == LanguageType.En && !string.IsNullOrWhiteSpace(negativeMessage))
        {
            return negativeMessage;
        }

        if (!isNegative)
        {
            return message;
        }

        string text = ((languageType == LanguageType.En) ? "not" : (viTranslation ?? "không"));
        return string.Join(" ", new string[2] { text, message }.Where((string item) => !string.IsNullOrEmpty(item)));
    }
}