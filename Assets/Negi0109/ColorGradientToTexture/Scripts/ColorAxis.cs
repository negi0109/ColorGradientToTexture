using System;
using System.Collections.Generic;
using UnityEngine;

namespace Negi0109.ColorGradientToTexture
{
    [Serializable]
    public class ColorAxis
    {
        public AnimationCurve xCurve;
        public AnimationCurve yCurve;
        public float yDirection;
        public List<ColorFilter> colorFilters;

        public ColorAxis()
        {
            xCurve = AnimationCurve.Linear(0, 0, 1, 1);
            yCurve = AnimationCurve.Linear(0, 0, 1, 1);
            yDirection = 0;
            colorFilters = new List<ColorFilter>(0);
        }

        public float Evaluate(float x, float y)
        {
            float v = (1 - yDirection) * xCurve.Evaluate(x) + yDirection * yCurve.Evaluate(y);

            foreach (var filter in colorFilters)
                v = filter.Evaluate(v);

            return v;
        }
    }
}
