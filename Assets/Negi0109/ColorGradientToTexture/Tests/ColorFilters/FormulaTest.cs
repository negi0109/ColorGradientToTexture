using System;
using NUnit.Framework;

namespace Negi0109.ColorGradientToTexture.Tests.ColorFilterTests
{
    public class FormulaTest
    {

        [TestCase("v", 1f, 2f, 3f, 4f)]
        [TestCase("x", 0f, 0f, 1f, 1f)]
        [TestCase("y", 0f, 1f, 0f, 1f)]
        [TestCase("0.1", 0.1f, 0.1f, 0.1f, 0.1f)]
        [TestCase("0.25", 0.25f, 0.25f, 0.25f, 0.25f)]
        [TestCase("3", 3f, 3f, 3f, 3f)]
        [TestCase("v+2", 3f, 4f, 5f, 6f)]
        [TestCase("v-2", -1f, 0f, 1f, 2f)]
        [TestCase("v*2", 2f, 4f, 6f, 8f)]
        [TestCase("v/2", 0.5f, 1f, 1.5f, 2f)]
        [TestCase("v+1*2", 3f, 4f, 5f, 6f)]
        [TestCase("(v+1)*2", 4f, 6f, 8f, 10f)]
        [TestCase("2*v+1", 3f, 5f, 7f, 9f)]
        public void Evaluate(string formula, float e1, float e2, float e3, float e4)
        {
            var filter = new Filters.Formula() { formula = formula };
            var ar = new float[,] { { 1f, 2f, }, { 3f, 4f } };
            filter.EvaluateAll(ref ar);

            Assert.That(ar, Is.EqualTo(new float[,] { { e1, e2 }, { e3, e4 } }));
        }

        [TestCase("2*(v+1", "No matching ')' for '('", 2)]
        public void ThrowParseError(string formula, string message, int location)
        {
            var exception = Assert.Throws<Filters.FormulaCompiler.ParseException>(
                () => Filters.FormulaCompiler.Compile(formula)
            );
            Assert.That(exception.Message, Is.EqualTo(message));
            Assert.That(exception.location, Is.EqualTo(location));
        }
    }
}