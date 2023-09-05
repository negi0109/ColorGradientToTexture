using System.Linq.Expressions;

namespace Negi0109.ColorGradientToTexture.Filters.Formulas
{
    public class ConstantToken : MonomialToken
    {
        public ConstantToken(int begin, int end) : base(begin, end) { }

        public float value;
        public override Expression GetExpression() => Expression.Constant(value);
    }
}
