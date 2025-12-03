using System;
using System.Linq.Expressions;
using SharedKernel.Extensions.Expressions;

namespace SharedKernel.Common.Messages;

public static class Messenger
{
    public static Message<T> Create<T>(string? objectName = null) where T : class
    {
        return new Message<T>(objectName);
    }

    public static Message<T> Property<T>(this Message<T> message, Expression<Func<T, object>> prop) where T : class
    {
        message.PropertyName = prop.ToStringProperty();
        return message;
    }

    public static Message<T> Property<T>(this Message<T> message, string propertyName) where T : class
    {
        message.PropertyName = propertyName;
        return message;
    }

    public static Message<T> CustomProperty<T>(this Message<T> message, string propertyName, string? additions = null) where T : class
    {
        message.PropertyName = propertyName;
        message.Additions = additions;
        return message;
    }

    public static Message<T> Negative<T>(this Message<T> message, bool isNegative = true) where T : class
    {
        message.IsNegative = isNegative;
        return message;
    }

    public static Message<T> Object<T>(this Message<T> message, string name) where T : class
    {
        message.ObjectName = name;
        return message;
    }

    public static Message<T> Message<T>(this Message<T> message, CustomMessage customMessage) where T : class
    {
        message.CustomMessage = customMessage;
        return message;
    }

    public static Message<T> Message<T>(this Message<T> message, MessageType type) where T : class
    {
        message.Type = type;
        return message;
    }

    public static Message<T> EnglishTranslation<T>(this Message<T> message, string englishMessage) where T : class
    {
        message.EnglishTranslatedMessage = englishMessage;
        return message;
    }

    public static Message<T> VietnameseTranslation<T>(this Message<T> message, string vietnameseMessage) where T : class
    {
        message.VietnameseTranslatedMessage = vietnameseMessage;
        return message;
    }

    public static MessageResult Build<T>(this Message<T> message) where T : class
    {
        return message.BuildMessage();
    }
}