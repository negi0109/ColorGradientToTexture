using System.Linq.Expressions;

namespace Negi0109.ColorGradientToTexture.Filters.Formulas
{
    public abstract class MonomialToken : Token
    {
        protected MonomialToken(int begin, int end) : base(begin, end) { }

        public abstract Expression GetExpression();
    }
}
