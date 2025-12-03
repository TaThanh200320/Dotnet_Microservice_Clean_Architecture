using CaseConverter;

namespace SharedKernel.Common.Messages;

public static class ErrorMessage
{
    public static readonly Dictionary<MessageType, MessageDictionary> ErrorMessages = new Dictionary<MessageType, MessageDictionary>
    {
        {
            MessageType.MaximumLength,
            new MessageDictionary("too-long", new Dictionary<string, string>
            {
                {
                    LanguageType.En.ToString(),
                    "too long"
                },
                {
                    LanguageType.Vi.ToString(),
                    "quá dài"
                }
            }, MessageType.MaximumLength)
        },
        {
            MessageType.MinimumLength,
            new MessageDictionary("too-short", new Dictionary<string, string>
            {
                {
                    LanguageType.En.ToString(),
                    "too short"
                },
                {
                    LanguageType.Vi.ToString(),
                    "quá ngắn"
                }
            }, MessageType.MinimumLength)
        },
        {
            MessageType.Valid,
            new MessageDictionary(MessageType.Valid.ToString().ToKebabCase(), new Dictionary<string, string>
            {
                {
                    LanguageType.En.ToString(),
                    "valid"
                },
                {
                    LanguageType.Vi.ToString(),
                    "hợp lệ"
                }
            }, MessageType.Valid, "invalid", new KeyValuePair<string, string>("for", "cho"))
        },
        {
            MessageType.Found,
            new MessageDictionary(MessageType.Found.ToString().ToKebabCase(), new Dictionary<string, string>
            {
                {
                    LanguageType.En.ToString(),
                    "found"
                },
                {
                    LanguageType.Vi.ToString(),
                    "tìm thấy"
                }
            }, MessageType.Found)
        },
        {
            MessageType.Existence,
            new MessageDictionary(MessageType.Existence.ToString().ToKebabCase(), new Dictionary<string, string>
            {
                {
                    LanguageType.En.ToString(),
                    MessageType.Existence.ToString().ToKebabCase()
                },
                {
                    LanguageType.Vi.ToString(),
                    "tồn tại"
                }
            }, MessageType.Existence)
        },
        {
            MessageType.Correct,
            new MessageDictionary(MessageType.Correct.ToString().ToKebabCase(), new Dictionary<string, string>
            {
                {
                    LanguageType.En.ToString(),
                    MessageType.Correct.ToString().ToKebabCase()
                },
                {
                    LanguageType.Vi.ToString(),
                    "đúng"
                }
            }, MessageType.Correct, "incorrect")
        },
        {
            MessageType.Active,
            new MessageDictionary(MessageType.Active.ToString().ToKebabCase(), new Dictionary<string, string>
            {
                {
                    LanguageType.En.ToString(),
                    "active"
                },
                {
                    LanguageType.Vi.ToString(),
                    "hoạt động"
                }
            }, MessageType.Active, "inactive")
        },
        {
            MessageType.AmongTheAllowedOptions,
            new MessageDictionary(MessageType.AmongTheAllowedOptions.ToString().ToKebabCase(), new Dictionary<string, string>
            {
                {
                    LanguageType.En.ToString(),
                    "among the allowed options"
                },
                {
                    LanguageType.Vi.ToString(),
                    "nằm trong lựa chọn cho phép"
                }
            }, MessageType.AmongTheAllowedOptions)
        },
        {
            MessageType.GreaterThan,
            new MessageDictionary(MessageType.GreaterThan.ToString().ToKebabCase(), new Dictionary<string, string>
            {
                {
                    LanguageType.En.ToString(),
                    "greater than"
                },
                {
                    LanguageType.Vi.ToString(),
                    "lớn hơn"
                }
            }, MessageType.GreaterThan)
        },
        {
            MessageType.GreaterThanEqual,
            new MessageDictionary(MessageType.GreaterThanEqual.ToString().ToKebabCase(), new Dictionary<string, string>
            {
                {
                    LanguageType.En.ToString(),
                    "greater than or equal"
                },
                {
                    LanguageType.Vi.ToString(),
                    "lớn hơn hoặc bằng"
                }
            }, MessageType.GreaterThanEqual)
        },
        {
            MessageType.LessThan,
            new MessageDictionary(MessageType.LessThan.ToString().ToKebabCase(), new Dictionary<string, string>
            {
                {
                    LanguageType.En.ToString(),
                    "less than"
                },
                {
                    LanguageType.Vi.ToString(),
                    "nhỏ hơn"
                }
            }, MessageType.LessThan)
        },
        {
            MessageType.LessThanEqual,
            new MessageDictionary(MessageType.LessThanEqual.ToString().ToKebabCase(), new Dictionary<string, string>
            {
                {
                    LanguageType.En.ToString(),
                    "less than or equal"
                },
                {
                    LanguageType.Vi.ToString(),
                    "nhỏ hơn hoặc bằng"
                }
            }, MessageType.LessThanEqual)
        },
        {
            MessageType.Null,
            new MessageDictionary(MessageType.Null.ToString().ToKebabCase(), new Dictionary<string, string>
            {
                {
                    LanguageType.En.ToString(),
                    "null"
                },
                {
                    LanguageType.Vi.ToString(),
                    "rỗng"
                }
            }, MessageType.Null)
        },
        {
            MessageType.Empty,
            new MessageDictionary(MessageType.Empty.ToString().ToKebabCase(), new Dictionary<string, string>
            {
                {
                    LanguageType.En.ToString(),
                    "empty"
                },
                {
                    LanguageType.Vi.ToString(),
                    "trống"
                }
            }, MessageType.Empty)
        },
        {
            MessageType.Unique,
            new MessageDictionary(MessageType.Unique.ToString().ToKebabCase(), new Dictionary<string, string>
            {
                {
                    LanguageType.En.ToString(),
                    "unique"
                },
                {
                    LanguageType.Vi.ToString(),
                    "là duy nhất"
                }
            }, MessageType.Unique)
        },
        {
            MessageType.Strong,
            new MessageDictionary(MessageType.Strong.ToString().ToKebabCase(), new Dictionary<string, string>
            {
                {
                    LanguageType.En.ToString(),
                    "strong enough"
                },
                {
                    LanguageType.Vi.ToString(),
                    "đủ mạnh"
                }
            }, MessageType.Strong, "weak")
        },
        {
            MessageType.Expired,
            new MessageDictionary(MessageType.Expired.ToString().ToKebabCase(), new Dictionary<string, string>
            {
                {
                    LanguageType.En.ToString(),
                    "Expired"
                },
                {
                    LanguageType.Vi.ToString(),
                    "Quá hạn"
                }
            }, MessageType.Expired)
        },
        {
            MessageType.Redundant,
            new MessageDictionary(MessageType.Redundant.ToString().ToKebabCase(), new Dictionary<string, string>
            {
                {
                    LanguageType.En.ToString(),
                    "Redundant"
                },
                {
                    LanguageType.Vi.ToString(),
                    "Dư thừa"
                }
            }, MessageType.Redundant)
        },
        {
            MessageType.Missing,
            new MessageDictionary(MessageType.Missing.ToString().ToKebabCase(), new Dictionary<string, string>
            {
                {
                    LanguageType.En.ToString(),
                    "missing"
                },
                {
                    LanguageType.Vi.ToString(),
                    "thiếu"
                }
            }, MessageType.Missing)
        },
        {
            MessageType.Identical,
            new MessageDictionary(MessageType.Identical.ToString().ToKebabCase(), new Dictionary<string, string>
            {
                {
                    LanguageType.En.ToString(),
                    "Identical"
                },
                {
                    LanguageType.Vi.ToString(),
                    "Giống"
                }
            }, MessageType.Identical, null, new KeyValuePair<string, string>("to", "với"))
        }
    };
}