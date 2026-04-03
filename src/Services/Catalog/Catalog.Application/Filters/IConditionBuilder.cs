using System.Linq.Expressions;

namespace Catalog.Application.Filters;

public interface IConditionBuilder
{
    Expression BuildExpression(ParameterExpression parameter);
}