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
        public float yWeight;

        [SerializeReference]
        public List<ColorFilter> colorFilters;
        [SerializeReference]
        public List<CoordinateFilter> coordinateFilters;
        public readonly int axesCount = 0;

        private bool dirty = false;
        public bool Dirty
        {
            get => dirty;
            set => dirty = value;
        }

        public ColorAxis(int axesCount)
        {
            this.axesCount = axesCount;
            xCurve = AnimationCurve.Linear(0, 0, 1, 1);
            yCurve = AnimationCurve.Linear(0, 0, 1, 1);
            yWeight = 0;
            colorFilters = new List<ColorFilter>(0);
            coordinateFilters = new List<CoordinateFilter>(0);
        }

        public double Evaluate(float x, float y)
        {
            var pos = new Vector2(x, y);
            foreach (var filter in coordinateFilters)
                pos = filter.Evaluate(pos);

            double v = axesCount == 1 ? xCurve.Evaluate(pos.x) : (1 - yWeight) * xCurve.Evaluate(pos.x) + yWeight * yCurve.Evaluate(pos.y);

            foreach (var filter in colorFilters)
                v = filter.Evaluate(v);

            return v;
        }

        public double[,] Evaluate(int width, int height)
        {
            var v = new double[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var pos = new Vector2((float)x / width, (float)y / height);

                    foreach (var filter in coordinateFilters)
                        pos = filter.Evaluate(pos);
                    v[x, y] = axesCount == 1 ? xCurve.Evaluate(pos.x) : (1 - yWeight) * xCurve.Evaluate(pos.x) + yWeight * yCurve.Evaluate(pos.y);
                }
            }

            foreach (var filter in colorFilters)
                filter.EvaluateAll(ref v);

            return v;
        }
    }
}
