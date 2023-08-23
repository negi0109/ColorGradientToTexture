using System;
using System.Linq.Expressions;
using System.Linq;
using System.Collections.Generic;

namespace Negi0109.ColorGradientToTexture.Filters.Formulas
{
    public static class FormulaTokenizer
    {
        public static Token[] Tokenize(string text, Dictionary<string, ParameterExpression> allParams, int offset = 0)
        {
            List<Token> tokens = new List<Token>();

            for (var i = 0; i < text.Length; i++)
            {
                switch (text[i])
                {
                    case '(':
                        if (TryGetPairBracket(text, i, out int index))
                        {
                            tokens.Add(new FormulaToken(text, i, index, offset, allParams));
                            i = index;
                        }
                        else throw new ParseException($"No matching ')' for '('", i);
                        break;
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
                        else if (TryGetIdentifier(text, i, out string identifier, out int tokenLength))
                        {
                            if (allParams.ContainsKey(identifier))
                            {
                                tokens.Add(new VariableToken(identifier, i + offset, i + tokenLength - 1 + offset, allParams));
                            }
                            else
                            {
                                throw new ParseException($"{identifier} is undefined identifier", i + offset, i + offset + tokenLength - 1);
                            }

                            i += tokenLength - 1;
                        }
                        break;
                }
            }

            return tokens.ToArray();
        }

        public static bool TryGetIdentifier(string text, int begin, out string identifier, out int tokenLength)
        {
            identifier = "";

            var index = text.IndexOfAny(new char[] {
                '(', ')', '+', '-', '*', '/', '%', ' '
            }, begin);

            if (index == -1) tokenLength = text.Length - begin;
            else tokenLength = index - begin;

            if (tokenLength <= 0) return false;

            identifier = text.Substring(begin, tokenLength);

            return true;
        }

        public static bool TryGetPairBracket(string text, int begin, out int index)
        {
            int nest = 1;
            index = -1;

            for (var i = begin + 1; i < text.Length; i++)
            {
                var chr = text[i];
                if (chr == '(') nest++;
                if (chr == ')') nest--;
                if (nest == 0)
                {
                    index = i;
                    return true;
                }
            }

            return false;
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
