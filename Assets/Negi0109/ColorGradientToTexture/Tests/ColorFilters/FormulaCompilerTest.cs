using System;
using NUnit.Framework;
using System.Linq.Expressions;

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
        public void EvaluateNoArgs(string formula, float e)
        {
            var body = Filters.FormulaCompiler.GetExpression(formula);
            var lambda = Expression.Lambda<Func<float>>(body);
            var func = lambda.Compile();

            Assert.That(func(), Is.EqualTo(e));
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
        public void ThrowParseError(string formula, string message, int begin, int end)
        {
            var exception = Assert.Throws<Filters.FormulaCompiler.ParseException>(
                () => Filters.FormulaCompiler.GetExpression(formula)
            );
            Assert.That(exception.Message, Is.EqualTo(message));
            Assert.That(exception.begin, Is.EqualTo(begin));
            Assert.That(exception.end, Is.EqualTo(end));
        }
    }
}
