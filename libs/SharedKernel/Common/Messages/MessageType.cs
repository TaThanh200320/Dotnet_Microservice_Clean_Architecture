namespace SharedKernel.Common.Messages;

public enum MessageType
{
    MaximumLength = 1,
    MinimumLength,
    Valid,
    Found,
    Existence,
    Correct,
    Active,
    AmongTheAllowedOptions,
    GreaterThan,
    GreaterThanEqual,
    LessThan,
    LessThanEqual,
    Empty,
    Null,
    Unique,
    Strong,
    Expired,
    Redundant,
    Missing,
    Identical
}