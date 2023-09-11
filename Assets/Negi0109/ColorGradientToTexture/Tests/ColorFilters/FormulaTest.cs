using System;
using NUnit.Framework;

namespace Negi0109.ColorGradientToTexture.Tests.ColorFilterTests
{
    public class FormulaTest
    {

        [TestCase("v", 1, 2, 3, 4)]
        [TestCase("0.1", 0.1, 0.1, 0.1, 0.1)]
        [TestCase("0.25", 0.25, 0.25, 0.25, 0.25)]
        [TestCase("3", 3, 3, 3, 3)]
        [TestCase("v+2", 3, 4, 5, 6)]
        [TestCase("v-2", -1, 0, 1, 2)]
        [TestCase("v*2", 2, 4, 6, 8)]
        [TestCase("v/2", 0.5, 1, 1.5, 2)]
        [TestCase("v+1*2", 3, 4, 5, 6)]
        [TestCase("(v+1)*2", 4, 6, 8, 10)]
        [TestCase("2*v+1", 3, 5, 7, 9)]
        [TestCase("pow(v, 2)", 1, 4, 9, 16)]
        [TestCase("max(v, 2)", 2, 2, 3, 4)]
        [TestCase("min(v, 2)", 1, 2, 2, 2)]
        [TestCase("log(pow(2, v), 2)", 1, 2, 3, 4)]
        [TestCase("exp(v)", 2.71828182846, 7.38905609893, 20.0855369232, 54.5981500331)]
        [TestCase("floor(v+0.6)", 1, 2, 3, 4)]
        [TestCase("ceil(v+0.6)", 2, 3, 4, 5)]
        public void Evaluate(string formula, double e1, double e2, double e3, double e4)
        {
            var filter = new Filters.Formula() { formula = formula };
            var ar = new double[,] { { 1, 2, }, { 3, 4 } };
            filter.EvaluateAll(ref ar);

            Assert.That(ar, Is.EqualTo(new double[,] { { e1, e2 }, { e3, e4 } }).Within(0.000001f));
        }

        [TestCase("x", 0, 0.5, 1)]
        [TestCase("y", 0.5, 0.5, 0.5)]
        public void ParamX(string formula, double x1, double x2, double x3)
        {

            var filter = new Filters.Formula() { formula = formula };
            var ar = new double[,] { { 0 }, { 0 }, { 0 } };
            filter.EvaluateAll(ref ar);

            Assert.That(ar, Is.EqualTo(new double[,] { { x1 }, { x2 }, { x3 } }));
        }

        [TestCase("y", 0, 0.5, 1)]
        [TestCase("x", 0.5, 0.5, 0.5)]
        public void ParamY(string formula, double y1, double y2, double y3)
        {

            var filter = new Filters.Formula() { formula = formula };
            var ar = new double[,] { { 0, 0, 0 } };
            filter.EvaluateAll(ref ar);

            Assert.That(ar, Is.EqualTo(new double[,] { { y1, y2, y3 } }));
        }
    }
}
