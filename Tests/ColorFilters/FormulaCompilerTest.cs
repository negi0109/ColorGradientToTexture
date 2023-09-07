using System;
using NUnit.Framework;
using System.Linq.Expressions;
using Negi0109.ColorGradientToTexture.Filters.Formulas;

namespace Negi0109.ColorGradientToTexture.Tests.ColorFilterTests
{
    public class FormulaCompilerTest
    {

        [TestCase("0.1", 0.1f)]
        [TestCase("0.25", 0.25f)]
        [TestCase("3", 3f)]
        [TestCase("1+2", 3f)]
        [TestCase("2-2", 0f)]
        [TestCase("3*2", 6f)]
        [TestCase("3/2", 1.5f)]
        [TestCase("3+1*2", 5f)]
        [TestCase("(2+1)*2", 6f)]
        [TestCase("2*3+1", 7f)]
        [TestCase("pow(2, 3)", 8f)]
        [TestCase("sin(0)", 0f)]
        [TestCase("sin(3.1415926535*0.5)", 1f)]
        [TestCase("cos(0)", 1f)]
        [TestCase("cos(3.1415926535*0.5)", 0f)]
        [TestCase("tan(0)", 0f)]
        [TestCase("pi", 3.1415926535f)]
        [TestCase("tau", 3.1415926535f * 2)]
        [TestCase("e", 2.718281828459f)]
        public void EvaluateNoArgs(string formula, float e)
        {
            var body = FormulaCompiler.GetExpression(formula);
            var lambda = Expression.Lambda<Func<float>>(body);
            var func = lambda.Compile();

            Assert.That(func(), Is.EqualTo(e).Within(0.000001f));
            Assert.That(body, Is.InstanceOf(typeof(ConstantExpression)), "Compiler optimization");
        }

        [TestCase("2*2+hoge", "hoge is undefined identifier", 4, 7)]
        [TestCase("2*)+3", "No matching '(' for ')'", 2, 2)]
        [TestCase("2*(v+1", "No matching ')' for '('", 2, 2)]
        [TestCase("2*3+", "right operand of '+' is nothing", 3, 3)]
        [TestCase("+2*3", "left operand of '+' is nothing", 0, 0)]
        [TestCase("10*(+2*3)", "left operand of '+' is nothing", 4, 4)]
        [TestCase("3*10 3", "operator is nothing", 2, 5)]
        [TestCase("3?10", "? is undefined identifier", 1, 1)]
        [TestCase("hoge(3*2)", "hoge is undefined identifier", 0, 3)]
        public void ThrowParseError(string formula, string message, int begin, int end)
        {
            var exception = Assert.Throws<ParseException>(
                () => FormulaCompiler.GetExpression(formula)
            );
            Assert.That(exception.Message, Is.EqualTo(message));
            Assert.That(exception.begin, Is.EqualTo(begin));
            Assert.That(exception.end, Is.EqualTo(end));
        }

        [TestCase("3-v", "(3 - v)", TestName = "減算は加算に変更するが未確定な式の場合最後に減算に戻す")]
        [TestCase("v-3", "(v + -3)", TestName = "減算は加算に変更")]
        [TestCase("v/4", "(v * 0.25)", TestName = "除算は乗算に変更")]
        [TestCase("4/v", "(4 / v)", TestName = "除算は乗算に変更するが未確定な式の場合最後に除算に戻す")]
        [TestCase("3+2", "5", TestName = "定数のみの式は事前に計算")]
        [TestCase("(3+2)+v", "(5 + v)", TestName = "定数のみの項は事前に計算")]
        [TestCase("v+(3+2)", "(v + 5)", TestName = "定数のみの項は事前に計算")]
        [TestCase("(v+3)+2", "(v + 5)", TestName = "可換で左項が未確定な式の場合定数な項でまとめる")]
        [TestCase("2+(v+3)", "(v + 5)", TestName = "可換で右項が未確定な式の場合定数な項でまとめる")]
        [TestCase("(v+2)+v", "((v + v) + 2)", TestName = "可換で左項が未確定な式の場合未確定な項でまとめる")]
        [TestCase("v+(v+2)", "((v + v) + 2)", TestName = "可換で右項が未確定な式の場合未確定な項でまとめる")]
        [TestCase("(v+3)+(2+v)", "((v + v) + 5)", TestName = "可換で双方の項が未確定な式場合未確定な項でまとめる")]
        public void Optimize(string formula, string expected)
        {
            var expression = FormulaCompiler.GetExpression(formula);

            Assert.That(expression.ToString(), Is.EqualTo(expected));
        }
    }
}
