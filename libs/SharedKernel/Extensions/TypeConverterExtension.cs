using System;
using System.Collections;
using System.Globalization;
using deniszykov.TypeConversion;
using SharedKernel.Extensions.Reflections;

namespace SharedKernel.Extensions;

public static class TypeConverterExtension
{
    public static object? ConvertTo(this object? input, Type targetType)
    {
        if (input == null)
        {
            return null;
        }

        Type type = input.GetType();
        if (targetType.IsAssignableFrom(type) || type.IsUserDefineType() || type.IsArrayGenericType() || (type != typeof(string) && typeof(IEnumerable).IsAssignableFrom(type)))
        {
            return input;
        }

        Type type2 = Nullable.GetUnderlyingType(targetType) ?? targetType;
        if (type2 == typeof(Ulid))
        {
            return (input == null) ? Ulid.Empty : Ulid.Parse(input.ToString());
        }

        if (type2 == typeof(DateOnly))
        {
            if (input is string text)
            {
                string text2 = text.Trim();
                if (DateOnly.TryParse(text2, out var result))
                {
                    return result;
                }

                if (DateTimeOffset.TryParse(text2, out var result2))
                {
                    return DateOnly.FromDateTime(result2.Date);
                }

                if (DateTime.TryParse(text2, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.AssumeLocal, out var result3))
                {
                    return DateOnly.FromDateTime(result3);
                }

                if (long.TryParse(text2, out var result4))
                {
                    return DateOnly.FromDateTime(((text2.Length >= 13) ? DateTimeOffset.FromUnixTimeMilliseconds(result4) : DateTimeOffset.FromUnixTimeSeconds(result4)).Date);
                }

                throw new InvalidCastException($"Cannot convert '{input}' to DateOnly.");
            }

            if (input is DateOnly dateOnly)
            {
                return dateOnly;
            }

            if (input is DateTime dateTime)
            {
                return DateOnly.FromDateTime(dateTime);
            }

            if (input is DateTimeOffset dateTimeOffset)
            {
                return DateOnly.FromDateTime(dateTimeOffset.Date);
            }

            if (input is long num)
            {
                return DateOnly.FromDateTime(((num >= 1000000000000L) ? DateTimeOffset.FromUnixTimeMilliseconds(num) : DateTimeOffset.FromUnixTimeSeconds(num)).UtcDateTime);
            }

            if (input is int num2)
            {
                return DateOnly.FromDateTime(DateTimeOffset.FromUnixTimeSeconds(num2).UtcDateTime);
            }

            throw new InvalidCastException($"Cannot convert '{input}' to DateOnly.");
        }

        if (type2 == typeof(DateTime))
        {
            if (input is string text3)
            {
                if (long.TryParse(text3, out var result5))
                {
                    return ((text3.Length >= 13) ? DateTimeOffset.FromUnixTimeMilliseconds(result5) : DateTimeOffset.FromUnixTimeSeconds(result5)).UtcDateTime;
                }

                return Convert.ChangeType(input, typeof(DateTime));
            }

            if (input is DateTime dateTime2)
            {
                return dateTime2;
            }

            if (input is DateTimeOffset dateTimeOffset2)
            {
                return dateTimeOffset2.UtcDateTime;
            }

            if (input is DateOnly dateOnly2)
            {
                return dateOnly2.ToDateTime(TimeOnly.MinValue, DateTimeKind.Local);
            }

            if (input is long num3)
            {
                return ((num3 >= 1000000000000L) ? DateTimeOffset.FromUnixTimeMilliseconds(num3) : DateTimeOffset.FromUnixTimeSeconds(num3)).UtcDateTime;
            }

            if (input is int num4)
            {
                return DateTimeOffset.FromUnixTimeSeconds(num4).UtcDateTime;
            }

            throw new InvalidCastException($"Cannot convert '{input.GetType()}' to DateTime.");
        }

        if (type2 == typeof(DateTimeOffset))
        {
            if (input is string text4 && long.TryParse(text4, out var result6))
            {
                return (text4.Length >= 13) ? DateTimeOffset.FromUnixTimeMilliseconds(result6) : DateTimeOffset.FromUnixTimeSeconds(result6);
            }

            if (input is DateOnly dateOnly3)
            {
                return new DateTimeOffset(dateOnly3.ToDateTime(TimeOnly.MinValue, DateTimeKind.Local));
            }

            if (input is long num5)
            {
                return (num5 >= 1000000000000L) ? DateTimeOffset.FromUnixTimeMilliseconds(num5) : DateTimeOffset.FromUnixTimeSeconds(num5);
            }

            if (input is int num6)
            {
                return DateTimeOffset.FromUnixTimeSeconds(num6);
            }

            throw new InvalidCastException($"Cannot convert '{input.GetType()}' to DateTimeOffset.");
        }

        return new TypeConversionProvider().Convert(typeof(object), targetType, input);
    }
}