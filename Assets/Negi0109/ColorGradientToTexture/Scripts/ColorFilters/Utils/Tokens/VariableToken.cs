using System.Linq.Expressions;
using System.Collections.Generic;

namespace Negi0109.ColorGradientToTexture.Filters.Formulas
{
    public class VariableToken : MonomialToken
    {
        private readonly ParameterExpression _parameter;

        public VariableToken(string value, int begin, int end, Dictionary<string, ParameterExpression> allParams) : base(begin, end)
        {
            _parameter = allParams[value];
        }

        public override Expression GetExpression() => _parameter;
    }
}
