using UnityEditor;
using UnityEngine;
using System;
using System.Linq.Expressions;
using System.Linq;
using System.Collections.Generic;

namespace Negi0109.ColorGradientToTexture.Filters
{
    public static class FormulaCompiler
    {
        private static List<string> allParams = new List<string> { "v", "x", "y" };
        private static ParameterExpression vParams = Expression.Parameter(typeof(float), "v");
        private static ParameterExpression xParams = Expression.Parameter(typeof(float), "x");
        private static ParameterExpression yParams = Expression.Parameter(typeof(float), "y");

        public static Func<float, float, float, float> Compile(string formula)
        {

            var body = GetExpression(formula);
            var lambda = Expression.Lambda<Func<float, float, float, float>>(body, vParams, xParams, yParams);

            return lambda.Compile();
        }

        public static Expression GetExpression(string formula)
        {
            var tokens = Tokenize(formula);
            if (tokens.Length == 0) throw new ParseException("code is empty", -1);

            return ParseExpression(new ArraySegment<Token>(tokens));
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

            public string value;

            public VariableToken(int begin, int end) : base(begin, end) { }

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
            public ConstantToken(int begin, int end) : base(begin, end) { }

            public float value;
            public override Expression GetExpression() => Expression.Constant(value);
        }

        public class FormulaToken : MonomialToken
        {
            public FormulaToken(int begin, int end) : base(begin, end) { }

            public Token[] tokens;
            public override Expression GetExpression() => ParseExpression(new ArraySegment<Token>(tokens));
        }

        public class OperatorToken : Token
        {
            public OperatorToken(int begin, int end) : base(begin, end) { }

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

            public Expression GetExpression(Expression v0, Expression v1)
            {
                if (v0 is ConstantExpression v0c && v1 is ConstantExpression v1c)
                {
                    var v0f = (float)v0c.Value;
                    var v1f = (float)v1c.Value;
                    return value switch
                    {
                        Operator.Add => Expression.Constant(v0f + v1f),
                        Operator.Subtract => Expression.Constant(v0f - v1f),
                        Operator.Multiply => Expression.Constant(v0f * v1f),
                        Operator.Divide => Expression.Constant(v0f / v1f),
                        _ => Expression.Constant(1f)
                    };
                }

                return value switch
                {
                    Operator.Add => Expression.Add(v0, v1),
                    Operator.Subtract => Expression.Subtract(v0, v1),
                    Operator.Multiply => Expression.Multiply(v0, v1),
                    Operator.Divide => Expression.Divide(v0, v1),
                    _ => Expression.Constant(1f)
                };
            }
        }

        [Serializable]
        public class ParseException : Exception
        {
            public readonly int begin;
            public readonly int end;

            public ParseException(string message, int location) : base(message)
            {
                this.begin = location;
                this.end = location;
            }
            public ParseException(string message, int location, int end) : base(message)
            {
                this.begin = location;
                this.end = end;
            }
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
                                    tokens.Add(new FormulaToken(i, j) { tokens = Tokenize(text.Substring(i + 1, j - i - 1)) });
                                    i = j;
                                    break;
                                }

                                if (j == text.Length - 1)
                                    throw new ParseException($"No matching ')' for '('", i);
                            }
                            break;
                        }
                    case ')': throw new ParseException($"No matching '(' for ')'", i);
                    case '+':
                        tokens.Add(new OperatorToken(i, i) { value = OperatorToken.Operator.Add });
                        break;
                    case '-':
                        tokens.Add(new OperatorToken(i, i) { value = OperatorToken.Operator.Subtract });
                        break;
                    case '*':
                        tokens.Add(new OperatorToken(i, i) { value = OperatorToken.Operator.Multiply });
                        break;
                    case '/':
                        tokens.Add(new OperatorToken(i, i) { value = OperatorToken.Operator.Divide });
                        break;
                    case ' ': break;
                    default:
                        if (char.IsDigit(text[i]))
                        {
                            GetFloat(text, i, out int length, out float value);
                            i += length - 1;
                            tokens.Add(new ConstantToken(i, i + length - 1) { value = value });
                        }
                        else
                        {
                            var index = text.IndexOfAny(new char[] {
                                    '(', ')', '+', '-', '*', '/', '%'
                                }, i);
                            int tokenLength;
                            if (index == -1) tokenLength = text.Length - i;
                            else tokenLength = index - i;

                            tokenLength = Mathf.Max(tokenLength, 0);
                            var token = text.Substring(i, tokenLength);
                            if (allParams.Contains(token))
                            {
                                tokens.Add(new VariableToken(i, i + tokenLength - 1) { value = token });
                            }
                            else
                            {
                                throw new ParseException($"{token} is undefined identifier", i, i + tokenLength - 1);
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
                if (text[begin + i - 1] == ' ') continue;
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
