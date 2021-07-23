using System;
using System.Collections.Generic;
using UnityEngine;

namespace Negi0109.ColorGradientToTexture
{
    [Serializable]
    public class ColorAxis
    {
        public AnimationCurve xCurve = AnimationCurve.Linear(0, 0, 1, 1);
        public AnimationCurve yCurve = AnimationCurve.Linear(0, 0, 1, 1);
        public float yDirection = 0;
        public List<ColorFilter> colorFilters = new List<ColorFilter>(0);

        public float Evaluate (float x, float y)
        {
            float v = (1 - yDirection) * xCurve.Evaluate(x) + yDirection * yCurve.Evaluate(y);

            foreach (var filter in colorFilters)
                v = filter.Evaluate(x, y, v);

            return v;
        }
    }
}
