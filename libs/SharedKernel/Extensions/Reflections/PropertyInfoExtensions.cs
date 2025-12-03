using System;
using System.Collections;
using System.Linq.Expressions;
using System.Reflection;
using SharedKernel.Exceptions;

namespace SharedKernel.Extensions.Reflections;

public static class PropertyInfoExtensions
{
    public static bool IsArrayGenericType(this PropertyInfo propertyInfo)
    {
        return propertyInfo.PropertyType.IsArrayGenericType();
    }

    public static bool IsArrayGenericType(this Type type)
    {
        if (type.IsGenericType && typeof(IEnumerable).IsAssignableFrom(type) && type.GetGenericArguments()[0].IsUserDefineType())
        {
            return true;
        }

        return false;
    }

    public static PropertyInfo GetNestedPropertyInfo(this Type type, string propertyName)
    {
        string[] array = propertyName.Trim().Split('.');
        PropertyInfo propertyInfo = null;
        string[] array2 = array;
        foreach (string text in array2)
        {
            propertyInfo = type.GetProperty(text.Trim(), BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public) ?? throw new NotFoundException(text, text);
            type = (propertyInfo.IsArrayGenericType() ? propertyInfo.PropertyType.GetGenericArguments()[0] : propertyInfo.PropertyType);
        }

        return propertyInfo;
    }

    public static object? GetNestedPropertyValue(this Type type, string propertyName, object target)
    {
        string[] array = propertyName.Trim().Split('.');
        object obj = target;
        string[] array2 = array;
        foreach (string text in array2)
        {
            if (obj == null)
            {
                break;
            }

            PropertyInfo propertyInfo = type.GetProperty(text.Trim(), BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public) ?? throw new NotFoundException(text, text);
            type = (propertyInfo.IsArrayGenericType() ? propertyInfo.PropertyType.GetGenericArguments()[0] : propertyInfo.PropertyType);
            obj = propertyInfo.GetValue(obj, null);
        }

        return obj;
    }

    public static bool IsNestedPropertyValid(this Type type, string propertyName)
    {
        string[] array = propertyName.Trim().Split('.');
        foreach (string text in array)
        {
            PropertyInfo property = type.GetProperty(text.Trim(), BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);
            if (property == null)
            {
                return false;
            }

            type = (property.IsArrayGenericType() ? property.PropertyType.GetGenericArguments()[0] : property.PropertyType);
        }

        return true;
    }

    public static bool IsUserDefineType(this PropertyInfo? propertyInfo)
    {
        return (propertyInfo?.PropertyType).IsUserDefineType();
    }

    public static bool IsUserDefineType(this Type? type)
    {
        if (type == null)
        {
            return false;
        }

        if ((object)type != null && type.IsClass)
        {
            if ((object)type == null)
            {
                return false;
            }

            return type.FullName?.StartsWith("System.") == false;
        }

        return false;
    }

    public static string GetValue<T>(this T obj, Expression<Func<T, object>> expression)
    {
        return expression.ToPropertyInfo().GetValue(obj, null)?.ToString() ?? string.Empty;
    }

    public static PropertyInfo ToPropertyInfo(this Expression expression)
    {
        if (expression == null)
        {
            throw new ArgumentException("Expression must be not null.");
        }

        LambdaExpression lambdaExpression = (expression as LambdaExpression) ?? throw new ArgumentException($"Can not parse {expression} to LambdaExpression");
        return (PropertyInfo)(lambdaExpression.Body.NodeType switch
        {
            ExpressionType.Convert => ((UnaryExpression)lambdaExpression.Body).Operand as MemberExpression,
            ExpressionType.MemberAccess => lambdaExpression.Body as MemberExpression,
            _ => throw new Exception("Expression Type is not support"),
        }).Member;
    }

    public static bool IsNullable(this Type type)
    {
        if (!type.IsValueType)
        {
            return true;
        }

        if (Nullable.GetUnderlyingType(type) != null)
        {
            return true;
        }

        return false;
    }
}