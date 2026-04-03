using System.Linq.Expressions;

namespace Vehicle.Application.Filters;

public interface IConditionBuilder
{
    Expression BuildExpression(ParameterExpression parameter);
}