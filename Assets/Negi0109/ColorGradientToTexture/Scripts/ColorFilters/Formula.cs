using UnityEditor;
using UnityEngine;
using System;
using System.Linq.Expressions;
using System.Linq;
using System.Collections.Generic;

namespace Negi0109.ColorGradientToTexture.Filters
{
    [System.Serializable]
    public class Formula : ColorFilter
    {
        public string formula = "";
        private string compiledFormula;
        private Func<float, float, float, float> compiled;

        public override void EvaluateAll(ref float[,] array)
        {
            Compile();

            for (int x = 0; x < array.GetLength(0); x++)
            {
                for (int y = 0; y < array.GetLength(1); y++)
                {
                    array[x, y] = compiled(array[x, y], x, y);
                }
            }
        }

        public override bool Editor()
        {
            return Watch(
                () => formula,
                () => { formula = EditorGUILayout.TextField("formula", formula); }
            );
        }

        public void Compile()
        {
            if (formula == compiledFormula) return;

            compiled = FormulaCompiler.Compile(formula);
            compiledFormula = formula;
        }


        public static class FormulaCompiler
        {
            private static List<string> allParams = new List<string> { "v", "x", "y" };
            private static ParameterExpression vParams = Expression.Parameter(typeof(float), "v");
            private static ParameterExpression xParams = Expression.Parameter(typeof(float), "x");
            private static ParameterExpression yParams = Expression.Parameter(typeof(float), "y");

            public static Func<float, float, float, float> Compile(string formula)
            {
                var tokens = Tokenize(formula);

                if (tokens.Length == 0) return (float v, float x, float y) => v;
                else if (tokens.Length > 0)
                {
                    var body = ParseExpression(new ArraySegment<Token>(tokens));
                    var lambda = Expression.Lambda<Func<float, float, float, float>>(body, vParams, xParams, yParams);

                    return lambda.Compile();
                }

                return (float v, float x, float y) => v;
            }

            public static Expression ParseExpression(ArraySegment<Token> tokens)
            {
                if (tokens.Count == 1)
                {
                    if (tokens.Array[tokens.Offset] is MonomialToken token)
                    {
                        return token.GetExpression();
                    }
                }
                else
                {
                    var lowPriority = 1000;
                    var lowPriorityOperatorIndex = 0;

                    for (int i = tokens.Count - 1; i >= 0; i--)
                    {
                        if (tokens.Array[tokens.Offset + i] is OperatorToken token)
                        {
                            if (token.Priority < lowPriority)
                            {
                                lowPriorityOperatorIndex = i;
                                lowPriority = token.Priority;
                            }
                        }
                    }

                    return ((OperatorToken)tokens.Array[tokens.Offset + lowPriorityOperatorIndex]).GetExpression(
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

                return Expression.Constant(1f);
            }

            public class Token { }

            public abstract class MonomialToken : Token
            {
                public abstract Expression GetExpression();
            }

            public class FunctionToken : Token { public string func; }
            public class VariableToken : MonomialToken
            {

                public string value;
                public override Expression GetExpression()
                    => value switch
                    {
                        "x" => xParams,
                        "y" => yParams,
                        "v" => vParams,
                        _ => vParams
                    };
            }
            public class ConstantToken : MonomialToken
            {
                public float value;
                public override Expression GetExpression() => Expression.Constant(value);
            }
            public class FormulaToken : MonomialToken
            {
                public Token[] tokens;
                public override Expression GetExpression() => ParseExpression(new ArraySegment<Token>(tokens));
            }
            public class OperatorToken : Token
            {
                public Operator value;
                public enum Operator { Add, Subtract, Multiply, Divide, LeftBracket, RightBracket }
                public int Priority =>
                    value switch
                    {
                        Operator.Add => 1,
                        Operator.Subtract => 1,
                        Operator.Multiply => 2,
                        Operator.Divide => 2,
                        _ => 0
                    };

                public Expression GetExpression(Expression v0, Expression v1) =>
                    value switch
                    {
                        Operator.Add => Expression.Add(v0, v1),
                        Operator.Subtract => Expression.Subtract(v0, v1),
                        Operator.Multiply => Expression.Multiply(v0, v1),
                        Operator.Divide => Expression.Divide(v0, v1),
                        _ => Expression.Constant(1f)
                    };
            }

            public static Token[] Tokenize(string text)
            {
                List<Token> tokens = new List<Token>();

                for (var i = 0; i < text.Length; i++)
                {
                    switch (text[i])
                    {
                        case '(':
                            {
                                int nest = 1;

                                for (var j = i + 1; j < text.Length; j++)
                                {
                                    var chr = text[j];
                                    if (chr == '(') nest++;
                                    if (chr == ')') nest--;
                                    if (nest == 0)
                                    {
                                        tokens.Add(new FormulaToken() { tokens = Tokenize(text.Substring(i + 1, j - i - 1)) });
                                        i = j;
                                        break;
                                    }
                                }
                                break;
                            }
                        case ')': break;
                        case '+':
                            tokens.Add(new OperatorToken() { value = OperatorToken.Operator.Add });
                            break;
                        case '-':
                            tokens.Add(new OperatorToken() { value = OperatorToken.Operator.Subtract });
                            break;
                        case '*':
                            tokens.Add(new OperatorToken() { value = OperatorToken.Operator.Multiply });
                            break;
                        case '/':
                            tokens.Add(new OperatorToken() { value = OperatorToken.Operator.Divide });
                            break;
                        case ' ': break;
                        default:
                            if (char.IsDigit(text[i]))
                            {
                                GetFloat(text, i, out int length, out float value);
                                i += length - 1;
                                tokens.Add(new ConstantToken() { value = value });
                            }
                            else
                            {
                                var index = text.IndexOfAny(new char[] {
                                    '(', ')', '+', '-', '*', '/', '%'
                                }, i);
                                int tokenLength;
                                if (index == -1) tokenLength = text.Length - i;
                                else tokenLength = index - i;

                                // 12345
                                var token = text.Substring(i, tokenLength);
                                if (allParams.Contains(token))
                                {
                                    tokens.Add(new VariableToken() { value = token });
                                }
                                i += tokenLength - 1;
                            }
                            break;
                    }
                }

                return tokens.ToArray();
            }

            public static void GetFloat(string text, int begin, out int length, out float value)
            {
                for (var i = text.Length - begin; i >= 1; i--)
                {
                    if (float.TryParse(text.Substring(begin, i), out float parsed))
                    {
                        length = i;
                        value = parsed;

                        return;
                    }
                }
                throw new Exception();
            }
        }
    }
}
