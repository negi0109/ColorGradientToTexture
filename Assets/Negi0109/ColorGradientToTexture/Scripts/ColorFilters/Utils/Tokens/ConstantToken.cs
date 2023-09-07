using System;
using System.Linq.Expressions;
using UnityEngine;

namespace Negi0109.ColorGradientToTexture.Filters.Formulas
{
    public class ConstantToken : MonomialToken
    {
        private ConstantToken(int begin, int end) : base(begin, end) { }

        private float _value;
        public override Expression GetExpression() => Expression.Constant(_value);

        public static bool TryGetConstantToken(int begin, int end, string name, out ConstantToken result)
        {
            result = name switch
            {
                "pi" => new ConstantToken(begin, end) { _value = Mathf.PI },
                "tau" => new ConstantToken(begin, end) { _value = Mathf.PI * 2 },
                "e" => new ConstantToken(begin, end) { _value = (float)Math.E },
                _ => null
            };

            return result != null;
        }
    }
}
