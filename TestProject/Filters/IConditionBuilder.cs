using System.Linq.Expressions;

namespace TestProject.Filters;

public interface IConditionBuilder
{
    Expression BuildExpression(ParameterExpression parameter);
}