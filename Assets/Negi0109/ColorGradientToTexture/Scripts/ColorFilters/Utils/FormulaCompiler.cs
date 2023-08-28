using UnityEngine;
using System;
using System.Linq.Expressions;
using System.Linq;
using System.Collections.Generic;

namespace Negi0109.ColorGradientToTexture.Filters.Formulas
{
    public static class FormulaCompiler
    {
        static Dictionary<string, ParameterExpression> allParams = new Dictionary<string, ParameterExpression>()
        {
            { "x", Expression.Parameter(typeof(float), "x") },
            { "y", Expression.Parameter(typeof(float), "y") },
            { "v", Expression.Parameter(typeof(float), "v") }
        };

        public static Func<float, float, float, float> Compile(string formula)
        {

            var body = GetExpression(formula);
            var lambda = Expression.Lambda<Func<float, float, float, float>>(
                body, allParams["v"], allParams["x"], allParams["y"]
            );

            return lambda.Compile();
        }

        public static Expression GetExpression(string formula)
        {
            var tokens = FormulaTokenizer.Tokenize(formula, allParams);
            if (tokens.Length == 0) throw new ParseException("code is empty", -1);

            return ExpressionOptimizer.PostProcessingExpression(ParseExpression(new ArraySegment<Token>(tokens)));
        }

        public static Expression ParseExpression(ArraySegment<Token> tokens)
        {
            if (tokens.Count == 1)
            {
                if (tokens.Array[tokens.Offset] is MonomialToken token)
                {
                    return token.GetExpression();
                }

                throw new ParseException(
                    "operator is nothing",
                    tokens.Array[tokens.Offset].begin,
                    tokens.Array[tokens.Offset].end
                );
            }
            else
            {
                var lowPriority = 1000;
                var lowPriorityOperatorIndex = -1;

                for (int i = tokens.Count - 1; i >= 0; i--)
                {
                    if (tokens.Array[tokens.Offset + i] is OperatorToken token && token.Priority < lowPriority)
                    {
                        lowPriorityOperatorIndex = i;
                        lowPriority = token.Priority;
                    }
                }

                if (lowPriorityOperatorIndex == -1)
                {
                    throw new ParseException(
                        "operator is nothing",
                        tokens.Array[tokens.Offset].begin,
                        tokens.Array[tokens.Offset + tokens.Count - 1].end
                    );
                }

                var operatorToken = (OperatorToken)tokens.Array[tokens.Offset + lowPriorityOperatorIndex];


                if (lowPriorityOperatorIndex == tokens.Count - 1)
                    throw new ParseException($"right operand of '{operatorToken.value}' is nothing", operatorToken.begin);

                if (lowPriorityOperatorIndex == 0)
                    throw new ParseException($"left operand of '{operatorToken.value}' is nothing", operatorToken.begin);

                return operatorToken.GetExpression(
                    ParseExpression(new ArraySegment<Token>(
                        tokens.Array, tokens.Offset, lowPriorityOperatorIndex
                    )),
                    ParseExpression(new ArraySegment<Token>(
                        tokens.Array,
                        tokens.Offset + lowPriorityOperatorIndex + 1,
                        tokens.Count - lowPriorityOperatorIndex - 1
                    ))
                );
            }
        }
    }
}
