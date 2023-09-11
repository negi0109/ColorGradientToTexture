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
            protected static Expression GetExpressionUsingMethod(Type type, string name, (FormulaToken token, Expression expression)[] args)
            {
                var method = type.GetMethod(name, Enumerable.Repeat(typeof(double), args.Length).ToArray());
                var allConstants = true;
                var values = new object[args.Length];

                for (int i = 0; i < args.Length; i++)
                    if (args[i].expression is ConstantExpression constant)
                        values[i] = constant.Value;
                    else allConstants = false;

                if (allConstants) return Expression.Constant(method.Invoke(null, values));
                else return Expression.Call(method, args.Select(a => a.expression));
            }

            public abstract Expression GetExpression(FunctionToken token, (FormulaToken token, Expression expression)[] args);
            protected static Exception GetArgumentException(FunctionToken token)
                => new ParseException(
                    $"different number of arguments",
                    token.begin,
                    token.end
                );
        }

        private class MaxFunction : Function
        {
            public override Expression GetExpression(FunctionToken token, (FormulaToken token, Expression expression)[] args)
            {
                if (args.Length >= 2) return GetExpressionUsingMethod(typeof(Math), "Max", args);
                else throw GetArgumentException(token);
            }
        }

        private class MinFunction : Function
        {
            public override Expression GetExpression(FunctionToken token, (FormulaToken token, Expression expression)[] args)
            {
                if (args.Length >= 2) return GetExpressionUsingMethod(typeof(Math), "Min", args);
                else throw GetArgumentException(token);
            }
        }

        private class AbsFunction : MethodCallFunction { public AbsFunction() : base(typeof(Math), "Abs", args => args.Length == 1) { } }
        private class PowFunction : MethodCallFunction { public PowFunction() : base(typeof(Math), "Pow", args => args.Length == 2) { } }
        private class LogFunction : MethodCallFunction { public LogFunction() : base(typeof(Math), "Log", args => args.Length == 1 || args.Length == 2) { } }
        private class ExpFunction : MethodCallFunction { public ExpFunction() : base(typeof(Math), "Exp", args => args.Length == 1) { } }
        private class FloorFunction : MethodCallFunction { public FloorFunction() : base(typeof(Math), "Floor", args => args.Length == 1) { } }
        private class CeilFunction : MethodCallFunction { public CeilFunction() : base(typeof(Math), "Ceiling", args => args.Length == 1) { } }
        private class SinFunction : MethodCallFunction { public SinFunction() : base(typeof(Math), "Sin", args => args.Length == 1) { } }
        private class AsinFunction : MethodCallFunction { public AsinFunction() : base(typeof(Math), "Asin", args => args.Length == 1) { } }
        private class CosFunction : MethodCallFunction { public CosFunction() : base(typeof(Math), "Cos", args => args.Length == 1) { } }
        private class AcosFunction : MethodCallFunction { public AcosFunction() : base(typeof(Math), "Acos", args => args.Length == 1) { } }
        private class TanFunction : MethodCallFunction { public TanFunction() : base(typeof(Math), "Tan", args => args.Length == 1) { } }
        private class ClampFunction : Function
        {
            public override Expression GetExpression(FunctionToken token, (FormulaToken token, Expression expression)[] args)
            {
                if (args.Length == 1)
                {
                    return GetExpressionUsingMethod(typeof(Math), "Max", new (FormulaToken token, Expression expression)[] {
                        (null, GetExpressionUsingMethod(typeof(Math), "Min", new (FormulaToken token, Expression expression)[] {
                            args[0], (null, Expression.Constant(1.0))
                        })), (null, Expression.Constant(0.0))
                    });
                }
                else if (args.Length == 3)
                {
                    return GetExpressionUsingMethod(typeof(Math), "Max", new (FormulaToken token, Expression expression)[] {
                        (null, GetExpressionUsingMethod(typeof(Math), "Min", new (FormulaToken token, Expression expression)[] {
                            args[0], args[2]
                        })), args[1]
                    });
                }
                else throw GetArgumentException(token);
            }
        }

        private class AtanFunction : Function
        {
            public override Expression GetExpression(FunctionToken token, (FormulaToken token, Expression expression)[] args)
            {
                if (args.Length == 2) return GetExpressionUsingMethod(typeof(Math), "Atan2", args);
                else if (args.Length == 1) return GetExpressionUsingMethod(typeof(Math), "Atan", args);
                else throw GetArgumentException(token);
            }
        }

        private class MethodCallFunction : Function
        {
            private readonly Type _type;
            private readonly string _method;
            private readonly Func<(FormulaToken token, Expression expression)[], bool> _argValidate;

            public MethodCallFunction(Type type, string method, Func<(FormulaToken token, Expression expression)[], bool> argValidate)
            {
                _type = type;
                _method = method;
                _argValidate = argValidate;
            }

            public override Expression GetExpression(FunctionToken token, (FormulaToken token, Expression expression)[] args)
            {
                if (_argValidate(args)) return GetExpressionUsingMethod(_type, _method, args);
                else throw GetArgumentException(token);
            }
        }


        private readonly string _functionName;
        private readonly Function _function;
        private readonly FormulaToken[] argTokens;

        public override Expression GetExpression()
        {
            var args = new (FormulaToken token, Expression expression)[argTokens.Length];

            for (int i = 0; i < args.Length; i++)
            {
                args[i] = (
                    argTokens[i],
                    ExpressionOptimizer.PostProcessingExpression(argTokens[i].GetExpression())
                );
            }

            return _function.GetExpression(this, args);
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
                "sin" => new SinFunction(),
                "asin" => new AsinFunction(),
                "cos" => new CosFunction(),
                "acos" => new AcosFunction(),
                "tan" => new TanFunction(),
                "atan" => new AtanFunction(),
                "abs" => new AbsFunction(),
                "clamp" => new ClampFunction(),
                _ => throw new ParseException($"{functionName} is undefined identifier", begin, begin + functionName.Length - 1)
            };

            var beginArgumentIndex = 0;
            var argList = new List<FormulaToken>();

            for (int i = 0; i < argsBody.Length; i++)
            {
                if (argsBody[i] == ',')
                {
                    argList.Add(new FormulaToken(argsBody, beginArgumentIndex, i - 1, argsOffset, allParams));
                    beginArgumentIndex = i + 1;
                }
            }
            argList.Add(new FormulaToken(argsBody, beginArgumentIndex, argsBody.Length - 1, argsOffset, allParams));
            argTokens = argList.ToArray();
        }
    }
}
