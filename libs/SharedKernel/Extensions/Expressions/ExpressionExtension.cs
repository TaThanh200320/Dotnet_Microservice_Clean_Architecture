using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using SharedKernel.Extensions.Reflections;

namespace SharedKernel.Extensions.Expressions;

public static class ExpressionExtension
{
    public static Expression MemberExpression<T>(this Expression expression, string propertyPath, bool isNullCheck = false)
    {
        Type type = typeof(T);
        string[] array = propertyPath.Trim().Split('.', StringSplitOptions.TrimEntries);
        Expression expression2 = expression;
        Expression expression3 = null;
        string[] array2 = array;
        foreach (string text in array2)
        {
            PropertyInfo nestedPropertyInfo = type.GetNestedPropertyInfo(text);
            try
            {
                expression2 = Expression.PropertyOrField(expression2, text);
            }
            catch (ArgumentException)
            {
                expression2 = Expression.MakeMemberAccess(expression2, nestedPropertyInfo);
            }

            if (isNullCheck)
            {
                expression3 = GenerateOrderNullCheckExpression(expression2, expression3);
            }

            type = nestedPropertyInfo.PropertyType;
        }

        if (expression3 != null)
        {
            return Expression.Condition(expression3, Expression.Default(expression2.Type), expression2);
        }

        return expression2;
    }

    public static Expression MemberExpression(this Expression expression, Type entityType, string propertyPath)
    {
        Type type = entityType;
        string[] array = propertyPath.Trim().Split('.', StringSplitOptions.TrimEntries);
        Expression expression2 = expression;
        string[] array2 = array;
        foreach (string text in array2)
        {
            PropertyInfo nestedPropertyInfo = type.GetNestedPropertyInfo(text);
            try
            {
                expression2 = Expression.PropertyOrField(expression2, text);
            }
            catch (ArgumentException)
            {
                expression2 = Expression.MakeMemberAccess(expression2, nestedPropertyInfo);
            }

            type = nestedPropertyInfo.PropertyType;
        }

        return expression2;
    }

    public static MemberExpressionResult MemberExpressionNullCheck(this Expression expression, Type entityType, string propertyPath)
    {
        Type type = entityType;
        string[] array = propertyPath.Trim().Split('.', StringSplitOptions.TrimEntries);
        Expression expression2 = expression;
        Expression expression3 = null;
        string[] array2 = array;
        foreach (string text in array2)
        {
            PropertyInfo nestedPropertyInfo = type.GetNestedPropertyInfo(text);
            try
            {
                expression2 = Expression.PropertyOrField(expression2, text);
            }
            catch (ArgumentException)
            {
                expression2 = Expression.MakeMemberAccess(expression2, nestedPropertyInfo);
            }

            expression3 = GenerateNullCheckExpression(expression2, expression3);
            type = nestedPropertyInfo.PropertyType;
        }

        return new MemberExpressionResult(expression3, expression2);
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

    public static string ToStringProperty(this Expression expression)
    {
        if (expression == null)
        {
            throw new ArgumentException("Expression must be not null.");
        }

        LambdaExpression lambdaExpression = (expression as LambdaExpression) ?? throw new ArgumentException($"Can not parse {expression} to LambdaExpression");
        Stack<string> stack = new Stack<string>();
        MemberExpression memberExpression;
        switch (lambdaExpression.Body.NodeType)
        {
            case ExpressionType.Convert:
            case ExpressionType.ConvertChecked:
                memberExpression = (lambdaExpression.Body as UnaryExpression)?.Operand as MemberExpression;
                break;
            case ExpressionType.MemberAccess:
                memberExpression = lambdaExpression.Body as MemberExpression;
                break;
            default:
                throw new Exception("Expression Type is not support");
        }

        for (MemberExpression memberExpression2 = memberExpression; memberExpression2 != null; memberExpression2 = memberExpression2.Expression as MemberExpression)
        {
            stack.Push(memberExpression2.Member.Name);
        }

        int num = 0;
        string[] array = new string[stack.Count];
        foreach (string item in stack)
        {
            array[num] = item;
            num++;
        }

        return string.Join(".", array);
    }

    public static Type GetMemberExpressionType(this MemberExpression expression)
    {
        if (expression == null)
        {
            throw new ArgumentException("Expression must be not null.");
        }

        if (expression == null)
        {
            throw new ArgumentException($"Can not parse {expression} to MemberExpression");
        }

        MemberInfo member = expression.Member;
        if (!(member is PropertyInfo propertyInfo))
        {
            if (member is FieldInfo fieldInfo)
            {
                return fieldInfo.FieldType;
            }

            throw new ArgumentException($"{expression.Member} neither a property nor a field ");
        }

        return propertyInfo.PropertyType;
    }

    private static BinaryExpression GenerateOrderNullCheckExpression(Expression propertyValue, Expression nullCheckExpression)
    {
        if (nullCheckExpression != null)
        {
            return Expression.OrElse(nullCheckExpression, Expression.Equal(propertyValue, Expression.Default(propertyValue.Type)));
        }

        return Expression.Equal(propertyValue, Expression.Default(propertyValue.Type));
    }

    private static Expression GenerateNullCheckExpression(Expression propertyValue, Expression nullCheckExpression)
    {
        Expression expression = Expression.Not(Expression.Equal(propertyValue, Expression.Constant(null, propertyValue.Type)));
        if (nullCheckExpression != null)
        {
            return Expression.AndAlso(nullCheckExpression, expression);
        }

        return expression;
    }
}