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
                    array[x, y] = compiled(array[x, y], (float)x, (float)y);
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
                var tokens = Tokenize(formula).ToArray();

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
                    var token = tokens.Array[tokens.Offset];

                    if (token is ConstantToken)
                    {
                        return Expression.Constant(((ConstantToken)token).value);
                    }
                    else if (token is VariableToken)
                    {
                        switch (((VariableToken)token).value)
                        {
                            case "v": return vParams;
                            case "x": return xParams;
                            case "y": return yParams;
                        }
                    }
                }
                else
                {
                    var lowPriority = 1000;
                    var lowPriorityOperatorIndex = 0;

                    for (int i = tokens.Count - 1; i >= 0; i--)
                    {
                        if (tokens.Array[tokens.Offset + i] is OperatorToken)
                        {
                            var token = tokens.Array[tokens.Offset + i] as OperatorToken;
                            if (token.Priority < lowPriority)
                            {
                                lowPriorityOperatorIndex = i;
                                lowPriority = token.Priority;
                            }
                        }
                    }
                    Debug.Log(string.Join(",", tokens.Select(v => v.ToString())));
                    Debug.Log($"{tokens.Count} {lowPriorityOperatorIndex} {tokens.Array[tokens.Offset + lowPriorityOperatorIndex]}");

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

            public class Token
            {
                public enum Type { Operator, Function, Constant, Variable }
                public Type type;
            }

            public class FunctionToken : Token { public string func; }
            public class VariableToken : Token { public string value; }
            public class ConstantToken : Token { public float value; }
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

            public static List<Token> Tokenize(string text)
            {
                List<Token> tokens = new List<Token>();

                for (var i = 0; i < text.Length; i++)
                {
                    Debug.Log($"{i}: {text[i]}");
                    switch (text[i])
                    {
                        case '(':
                            tokens.Add(new OperatorToken() { value = OperatorToken.Operator.LeftBracket });
                            break;
                        case ')':
                            tokens.Add(new OperatorToken() { value = OperatorToken.Operator.RightBracket });
                            break;
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
                        case '0':
                        case '1':
                        case '2':
                        case '3':
                        case '4':
                        case '5':
                        case '6':
                        case '7':
                        case '8':
                        case '9':
                            {
                                GetFloat(text, i, out int length, out float value);
                                Debug.Log($"{i} {length}: {value}");
                                tokens.Add(new ConstantToken() { value = value });
                            }
                            break;
                        default:
                            if (text[i] == ' ') continue;
                            {
                                var index = text.IndexOfAny(new char[] {
                                    '(', ')', '+', '-', '*', '/', '%'
                                }, i);
                                var tokenLength = 0;
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

                return tokens;
            }

            // text = 0.333
            // begin = 0
            // value = 0.333f
            // length = 5
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
