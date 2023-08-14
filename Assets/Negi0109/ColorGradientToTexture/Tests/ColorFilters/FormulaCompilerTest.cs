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
        }

        [TestCase("2*(v+1", "No matching ')' for '('", 2)]
        public void ThrowParseError(string formula, string message, int location)
        {
            var exception = Assert.Throws<Filters.FormulaCompiler.ParseException>(
                () => Filters.FormulaCompiler.GetExpression(formula)
            );
            Assert.That(exception.Message, Is.EqualTo(message));
            Assert.That(exception.location, Is.EqualTo(location));
        }
    }
}
