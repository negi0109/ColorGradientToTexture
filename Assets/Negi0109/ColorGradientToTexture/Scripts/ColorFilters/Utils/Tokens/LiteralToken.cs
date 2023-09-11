using System.Linq.Expressions;

namespace Negi0109.ColorGradientToTexture.Filters.Formulas
{
    public class LiteralToken : MonomialToken
    {
        public LiteralToken(int begin, int end) : base(begin, end) { }

        public double value;
        public override Expression GetExpression() => Expression.Constant(value);
    }
}
