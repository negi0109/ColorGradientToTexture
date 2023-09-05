using UnityEngine;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace Negi0109.ColorGradientToTexture.Filters.Formulas
{
    public class FunctionToken : MonomialToken
    {
        private abstract class Function
        {
            public abstract Expression GetExpression(FunctionToken token, (FormulaToken token, Expression expression, float value)[] args, bool allConstants);
        }

        private class PowFunction : Function
        {
            public override Expression GetExpression(FunctionToken token, (FormulaToken token, Expression expression, float value)[] args, bool allConstants)
            {
                if (args.Length == 2)
                {
                    if (allConstants) return Expression.Constant(Mathf.Pow(args[0].value, args[1].value));
                    else return Expression.Call(typeof(Mathf).GetMethod("Pow"), args[0].expression, args[1].expression);
                }
                else
                {
                    throw new ParseException(
                        $"different number of arguments",
                        token.begin,
                        token.end
                    );
                }
            }
        }

        private class MaxFunction : Function
        {
            public override Expression GetExpression(FunctionToken token, (FormulaToken token, Expression expression, float value)[] args, bool allConstants)
            {
                if (args.Length == 2)
                {
                    if (allConstants) return Expression.Constant(Mathf.Max(args[0].value, args[1].value));
                    else return Expression.Call(
                        typeof(Mathf).GetMethod("Max", new[] { typeof(float), typeof(float) }),
                        args[0].expression,
                        args[1].expression
                    );
                }
                else
                {
                    throw new ParseException(
                        $"different number of arguments",
                        token.begin,
                        token.end
                    );
                }
            }
        }

        private class MinFunction : Function
        {
            public override Expression GetExpression(FunctionToken token, (FormulaToken token, Expression expression, float value)[] args, bool allConstants)
            {
                if (args.Length == 2)
                {
                    if (allConstants) return Expression.Constant(Mathf.Min(args[0].value, args[1].value));
                    else return Expression.Call(
                        typeof(Mathf).GetMethod("Min", new[] { typeof(float), typeof(float) }),
                        args[0].expression,
                        args[1].expression
                    );
                }
                else
                {
                    throw new ParseException(
                        $"different number of arguments",
                        token.begin,
                        token.end
                    );
                }
            }
        }

        private class LogFunction : Function
        {
            public override Expression GetExpression(FunctionToken token, (FormulaToken token, Expression expression, float value)[] args, bool allConstants)
            {
                if (args.Length == 2)
                {
                    if (allConstants) return Expression.Constant(Mathf.Log(args[0].value, args[1].value));
                    else return Expression.Call(
                        typeof(Mathf).GetMethod("Log", new[] { typeof(float), typeof(float) }),
                        args[0].expression,
                        args[1].expression
                    );
                }
                else if (args.Length == 1)
                {
                    if (allConstants) return Expression.Constant(Mathf.Log(args[0].value));
                    else return Expression.Call(
                        typeof(Mathf).GetMethod("Log", new[] { typeof(float) }),
                        args[0].expression
                    );
                }
                else
                {
                    throw new ParseException(
                        $"different number of arguments",
                        token.begin,
                        token.end
                    );
                }
            }
        }

        private class ExpFunction : Function
        {
            public override Expression GetExpression(FunctionToken token, (FormulaToken token, Expression expression, float value)[] args, bool allConstants)
            {
                if (args.Length == 1)
                {
                    if (allConstants) return Expression.Constant(Mathf.Exp(args[0].value));
                    else return Expression.Call(
                        typeof(Mathf).GetMethod("Exp", new[] { typeof(float) }),
                        args[0].expression
                    );
                }
                else
                {
                    throw new ParseException(
                        $"different number of arguments",
                        token.begin,
                        token.end
                    );
                }
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
