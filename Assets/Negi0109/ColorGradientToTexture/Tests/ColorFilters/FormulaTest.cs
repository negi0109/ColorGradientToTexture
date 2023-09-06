using System;
using NUnit.Framework;

namespace Negi0109.ColorGradientToTexture.Tests.ColorFilterTests
{
    public class FormulaTest
    {

        [TestCase("v", 1f, 2f, 3f, 4f)]
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
        [TestCase("pow(v, 2)", 1f, 4f, 9f, 16f)]
        [TestCase("max(v, 2)", 2f, 2f, 3f, 4f)]
        [TestCase("min(v, 2)", 1f, 2f, 2f, 2f)]
        [TestCase("log(pow(2, v), 2)", 1f, 2f, 3f, 4f)]
        [TestCase("exp(v)", 2.71828182846f, 7.38905609893f, 20.0855369232f, 54.5981500331f)]
        [TestCase("floor(v+0.5)", 1f, 2f, 3f, 4f)]
        [TestCase("ceil(v+0.5)", 2f, 3f, 4f, 5f)]
        public void Evaluate(string formula, float e1, float e2, float e3, float e4)
        {
            var filter = new Filters.Formula() { formula = formula };
            var ar = new float[,] { { 1f, 2f, }, { 3f, 4f } };
            filter.EvaluateAll(ref ar);

            Assert.That(ar, Is.EqualTo(new float[,] { { e1, e2 }, { e3, e4 } }));
        }

        [TestCase("x", 0f, 0.5f, 1f)]
        [TestCase("y", 0.5f, 0.5f, 0.5f)]
        public void ParamX(string formula, float x1, float x2, float x3)
        {

            var filter = new Filters.Formula() { formula = formula };
            var ar = new float[,] { { 0f }, { 0f }, { 0f } };
            filter.EvaluateAll(ref ar);

            Assert.That(ar, Is.EqualTo(new float[,] { { x1 }, { x2 }, { x3 } }));
        }

        [TestCase("y", 0f, 0.5f, 1f)]
        [TestCase("x", 0.5f, 0.5f, 0.5f)]
        public void ParamY(string formula, float y1, float y2, float y3)
        {

            var filter = new Filters.Formula() { formula = formula };
            var ar = new float[,] { { 0f, 0f, 0f } };
            filter.EvaluateAll(ref ar);

            Assert.That(ar, Is.EqualTo(new float[,] { { y1, y2, y3 } }));
        }
    }
}
