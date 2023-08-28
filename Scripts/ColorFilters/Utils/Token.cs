using UnityEngine;
using System;
using System.Linq.Expressions;
using System.Linq;
using System.Collections.Generic;

namespace Negi0109.ColorGradientToTexture.Filters.Formulas
{
    public class Token
    {
        public readonly int begin;
        public readonly int end = -1;

        public Token(int begin, int end) { this.begin = begin; this.end = end; }
    }

    public abstract class MonomialToken : Token
    {
        protected MonomialToken(int begin, int end) : base(begin, end) { }

        public abstract Expression GetExpression();
    }

    public class VariableToken : MonomialToken
    {

        private string _value;
        private ParameterExpression _parameter;

        public VariableToken(string value, int begin, int end, Dictionary<string, ParameterExpression> allParams) : base(begin, end)
        {
            _value = value;
            _parameter = allParams[value];
        }

        public override Expression GetExpression() => _parameter;
    }

    public class ConstantToken : MonomialToken
    {
        public ConstantToken(int begin, int end) : base(begin, end) { }

        public float value;
        public override Expression GetExpression() => Expression.Constant(value);
    }

    public class FormulaToken : MonomialToken
    {
        public FormulaToken(string text, int begin, int end, int offset, Dictionary<string, ParameterExpression> allParams) : base(begin, end)
        {
            tokens = FormulaTokenizer.Tokenize(text.Substring(begin + 1, end - begin - 1), allParams, begin + 1 + offset);
        }

        private Token[] tokens;
        public override Expression GetExpression() => FormulaCompiler.ParseExpression(new ArraySegment<Token>(tokens));
    }

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
                _ => throw new ParseException($"{value} is undefined identifier", begin, begin)
            };
        }

        public char value;
        public int Priority => _operator.priority;

        public Expression GetExpression(Expression v0, Expression v1)
            => ExpressionOptimizer.ReduceExpression(_operator.GetExpression(v0, v1));
    }
}
