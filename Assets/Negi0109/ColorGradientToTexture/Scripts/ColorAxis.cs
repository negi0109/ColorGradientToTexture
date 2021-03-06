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
        public List<CoordinateFilter> coordinateFilters;

        public ColorAxis()
        {
            xCurve = AnimationCurve.Linear(0, 0, 1, 1);
            yCurve = AnimationCurve.Linear(0, 0, 1, 1);
            yDirection = 0;
            colorFilters = new List<ColorFilter>(0);
            coordinateFilters = new List<CoordinateFilter>(0);
        }

        public float Evaluate(float x, float y)
        {
            var pos = new Vector2(x, y);
            foreach (var filter in coordinateFilters)
                pos = filter.Evaluate(pos);

            float v = (1 - yDirection) * xCurve.Evaluate(pos.x) + yDirection * yCurve.Evaluate(pos.y);

            foreach (var filter in colorFilters)
                v = filter.Evaluate(v);

            return v;
        }
    }
}
