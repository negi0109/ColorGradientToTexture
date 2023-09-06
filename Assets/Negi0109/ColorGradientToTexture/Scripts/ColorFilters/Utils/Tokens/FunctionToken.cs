using UnityEngine;
using System.Linq.Expressions;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Negi0109.ColorGradientToTexture.Filters.Formulas
{
    public class FunctionToken : MonomialToken
    {
        private abstract class Function
        {
            protected static Expression GetExpressionUsingMethod(Type type, string name, (FormulaToken token, Expression expression, float value)[] args, bool allConstants)
            {
                var method = type.GetMethod(name, Enumerable.Repeat(typeof(float), args.Length).ToArray());

                if (allConstants) return Expression.Constant(method.Invoke(null, args.Select(a => (object)a.value).ToArray()));
                else return Expression.Call(method, args.Select(a => a.expression));
            }

            public abstract Expression GetExpression(FunctionToken token, (FormulaToken token, Expression expression, float value)[] args, bool allConstants);
            protected static Exception GetArgumentException(FunctionToken token)
                => new ParseException(
                    $"different number of arguments",
                    token.begin,
                    token.end
                );
        }

        private class PowFunction : Function
        {
            public override Expression GetExpression(FunctionToken token, (FormulaToken token, Expression expression, float value)[] args, bool allConstants)
            {
                if (args.Length == 2)
                {
                    return GetExpressionUsingMethod(typeof(Mathf), "Pow", args, allConstants);
                }
                else throw GetArgumentException(token);
            }
        }

        private class MaxFunction : Function
        {
            public override Expression GetExpression(FunctionToken token, (FormulaToken token, Expression expression, float value)[] args, bool allConstants)
            {
                if (args.Length == 2) return GetExpressionUsingMethod(typeof(Mathf), "Max", args, allConstants);
                else throw GetArgumentException(token);
            }
        }

        private class MinFunction : Function
        {
            public override Expression GetExpression(FunctionToken token, (FormulaToken token, Expression expression, float value)[] args, bool allConstants)
            {
                if (args.Length == 2) return GetExpressionUsingMethod(typeof(Mathf), "Min", args, allConstants);
                else throw GetArgumentException(token);
            }
        }

        private class LogFunction : Function
        {
            public override Expression GetExpression(FunctionToken token, (FormulaToken token, Expression expression, float value)[] args, bool allConstants)
            {
                if (args.Length == 1 || args.Length == 2) return GetExpressionUsingMethod(typeof(Mathf), "Log", args, allConstants);
                else throw GetArgumentException(token);
            }
        }

        private class ExpFunction : Function
        {
            public override Expression GetExpression(FunctionToken token, (FormulaToken token, Expression expression, float value)[] args, bool allConstants)
            {
                if (args.Length == 1) return GetExpressionUsingMethod(typeof(Mathf), "Exp", args, allConstants);
                else throw GetArgumentException(token);
            }
        }

        private class FloorFunction : Function
        {
            public override Expression GetExpression(FunctionToken token, (FormulaToken token, Expression expression, float value)[] args, bool allConstants)
            {
                if (args.Length == 1) return GetExpressionUsingMethod(typeof(Mathf), "Floor", args, allConstants);
                else throw GetArgumentException(token);
            }
        }

        private class CeilFunction : Function
        {
            public override Expression GetExpression(FunctionToken token, (FormulaToken token, Expression expression, float value)[] args, bool allConstants)
            {
                if (args.Length == 1) return GetExpressionUsingMethod(typeof(Mathf), "Ceil", args, allConstants);
                else throw GetArgumentException(token);
            }
        }


        private readonly string _functionName;
        private readonly Function _function;
        private readonly FormulaToken[] argTokens;

        public override Expression GetExpression()
        {
            var args = new (FormulaToken token, Expression expression, float value)[argTokens.Length];
            for (int i = 0; i < args.Length; i++)
            {
                args[i] = (
                    argTokens[i],
                    ExpressionOptimizer.PostProcessingExpression(
                        argTokens[i].GetExpression()
                    ),
                    0f
                );
            }

            var allConstants = true;
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].expression is ConstantExpression constantExpression)
                {
                    args[i].value = (float)constantExpression.Value;
                }
                else
                {
                    allConstants = false;
                }
            }

            return _function.GetExpression(this, args, allConstants);
        }

        public FunctionToken(string functionName, string argsBody, int argsOffset, int begin, int end, Dictionary<string, ParameterExpression> allParams) : base(begin, end)
        {
            _functionName = functionName;
            _function = _functionName switch
            {
                "pow" => new PowFunction(),
                "max" => new MaxFunction(),
                "min" => new MinFunction(),
                "log" => new LogFunction(),
                "ln" => new LogFunction(),
                "exp" => new ExpFunction(),
                "floor" => new FloorFunction(),
                "ceil" => new CeilFunction(),
                _ => throw new ParseException(
                    $"{functionName} is undefined identifier",
                    begin,
                    begin + functionName.Length - 1
                )
            };

            var beginArgumentIndex = 0;
            var argList = new List<FormulaToken>();

            for (int i = 0; i < argsBody.Length; i++)
            {
                if (argsBody[i] == ',')
                {
                    argList.Add(
                        new FormulaToken(
                            argsBody, beginArgumentIndex, i - 1, argsOffset, allParams
                        )
                    );
                    beginArgumentIndex = i + 1;
                }
            }
            argList.Add(
                new FormulaToken(
                    argsBody, beginArgumentIndex, argsBody.Length - 1, argsOffset, allParams
                )
            );
            argTokens = argList.ToArray();
        }
    }
}
