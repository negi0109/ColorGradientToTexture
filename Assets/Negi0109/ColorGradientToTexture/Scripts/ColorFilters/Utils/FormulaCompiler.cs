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
                    if (tokens.Array[tokens.Offset + i] is OperatorToken token)
                    {
                        if (token.Priority < lowPriority)
                        {
                            lowPriorityOperatorIndex = i;
                            lowPriority = token.Priority;
                        }
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
            private abstract class Operator
            {
                public abstract Expression GetExpression(Expression left, Expression right);
                public abstract int Priority { get; }
            }

            private class AddOperator : Operator
            {
                public override int Priority => 1;
                public override Expression GetExpression(Expression left, Expression right)
                {
                    if (left is ConstantExpression lc && right is ConstantExpression rc)
                        return Expression.Constant((float)lc.Value + (float)rc.Value);
                    return Expression.Add(left, right);
                }
            }

            private class SubtractOperator : Operator
            {
                public override int Priority => 1;
                public override Expression GetExpression(Expression left, Expression right)
                {
                    if (left is ConstantExpression lc && right is ConstantExpression rc)
                        return Expression.Constant((float)lc.Value - (float)rc.Value);
                    return Expression.Subtract(left, right);
                }
            }

            private class MultiplyOperator : Operator
            {
                public override int Priority => 2;
                public override Expression GetExpression(Expression left, Expression right)
                {
                    if (left is ConstantExpression lc && right is ConstantExpression rc)
                        return Expression.Constant((float)lc.Value * (float)rc.Value);
                    return Expression.Multiply(left, right);
                }
            }

            private class DivideOperator : Operator
            {
                public override int Priority => 2;
                public override Expression GetExpression(Expression left, Expression right)
                {
                    if (left is ConstantExpression lc && right is ConstantExpression rc)
                        return Expression.Constant((float)lc.Value / (float)rc.Value);
                    return Expression.Divide(left, right);
                }
            }

            private Operator GetOperator() =>
                value switch
                {
                    '+' => new AddOperator(),
                    '-' => new SubtractOperator(),
                    '*' => new MultiplyOperator(),
                    '/' => new DivideOperator(),
                    _ => throw new ParseException($"{value} is undefined identifier", begin, begin)
                };

            public OperatorToken(int begin, int end, char value) : base(begin, end)
            {
                this.value = value;
                _ = Priority;
            }

            public char value;
            public int Priority => GetOperator().Priority;

            public Expression GetExpression(Expression v0, Expression v1)
                => GetOperator().GetExpression(v0, v1);
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

        public static Token[] Tokenize(string text, int offset = 0)
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
                                    tokens.Add(new FormulaToken(i, j) { tokens = Tokenize(text.Substring(i + 1, j - i - 1), i + 1 + offset) });
                                    i = j;
                                    break;
                                }

                                if (j == text.Length - 1)
                                    throw new ParseException($"No matching ')' for '('", i);
                            }
                            break;
                        }
                    case ')': throw new ParseException($"No matching '(' for ')'", i);
                    case ' ': break;
                    default:
                        if (char.IsDigit(text[i]))
                        {
                            GetFloat(text, i, out int length, out float value);
                            tokens.Add(new ConstantToken(offset + i, offset + i + length - 1) { value = value });
                            i += length - 1;
                        }
                        else if ("!\"#$%&'()=-^¥[@:]_/.;,<>+*?_}{`~|}".Contains(text[i]))
                        {
                            tokens.Add(new OperatorToken(i + offset, i + offset, text[i]));
                        }
                        else
                        {
                            var index = text.IndexOfAny(new char[] {
                                    '(', ')', '+', '-', '*', '/', '%', ' '
                                }, i);
                            int tokenLength;
                            if (index == -1) tokenLength = text.Length - i;
                            else tokenLength = index - i;

                            tokenLength = Mathf.Max(tokenLength, 0);
                            var token = text.Substring(i, tokenLength);
                            if (allParams.Contains(token))
                            {
                                tokens.Add(new VariableToken(i + offset, i + tokenLength - 1 + offset) { value = token });
                            }
                            else
                            {
                                throw new ParseException($"{token} is undefined identifier", i + offset, i + offset + tokenLength - 1);
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
