using System.Linq.Expressions;

namespace SharedKernel.Extensions.Expressions;

public record MemberExpressionResult(Expression NullCheck, Expression Member);