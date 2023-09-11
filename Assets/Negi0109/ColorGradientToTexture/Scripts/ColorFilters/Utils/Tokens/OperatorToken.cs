using System;
using System.Linq.Expressions;

namespace Negi0109.ColorGradientToTexture.Filters.Formulas
{
    public class OperatorToken : Token
    {
        private class Operator
        {
            private readonly Func<Expression, Expression, Expression> func;
            public Expression GetExpression(Expression left, Expression right) => func(left, right);
            public readonly int priority;

            public Operator(int priority, Func<Expression, Expression, Expression> func)
            {
                this.func = func;
                this.priority = priority;
            }
        }

        private readonly Operator _operator;

        public OperatorToken(int begin, int end, char value) : base(begin, end)
        {
            this.value = value;
            _operator = value switch
            {
                '+' => new Operator(1, (l, r) => Expression.Add(l, r)),
                '-' => new Operator(1, (l, r) => Expression.Subtract(l, r)),
                '*' => new Operator(2, (l, r) => Expression.Multiply(l, r)),
                '/' => new Operator(2, (l, r) => Expression.Divide(l, r)),
                '%' => new Operator(2, (l, r) => Expression.Modulo(l, r)),
                '^' => new Operator(2, (l, r) => Expression.Power(l, r)),
                _ => throw new ParseException($"{value} is undefined identifier", begin, begin)
            };
        }

        public char value;
        public int Priority => _operator.priority;

        public Expression GetExpression(Expression v0, Expression v1)
            => ExpressionOptimizer.ReduceExpression(_operator.GetExpression(v0, v1));
    }
}
