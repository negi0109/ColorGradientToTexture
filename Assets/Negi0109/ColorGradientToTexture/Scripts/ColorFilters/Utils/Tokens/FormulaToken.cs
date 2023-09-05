using System;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace Negi0109.ColorGradientToTexture.Filters.Formulas
{
    public class FormulaToken : MonomialToken
    {
        private readonly Token[] tokens;

        public FormulaToken(string text, int begin, int end, int offset, Dictionary<string, ParameterExpression> allParams) : base(begin, end)
        {
            tokens = FormulaTokenizer.Tokenize(text.Substring(begin, end - begin + 1), allParams, begin + offset);
        }

        public override Expression GetExpression() => FormulaCompiler.ParseExpression(new ArraySegment<Token>(tokens));
    }
}
