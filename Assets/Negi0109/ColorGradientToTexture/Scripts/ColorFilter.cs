using System;
using UnityEditor;

namespace Negi0109.ColorGradientToTexture {
    [System.Serializable]
    public abstract class ColorFilter {
        public static ColorFilter DefaultFilter { get => new Filters.Scale(); }

        public virtual float Evaluate(float v) => v;
        public virtual void Evaluate(ref float[,] v)
        {
            var width = v.GetLength(0);
            var height = v.GetLength(1);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    v[x, y] = Evaluate(v[x, y]);
                }
            }
        }

        public abstract bool Editor();

        public static bool Watch<T>(Func<T> value, Action action)
            where T : IEquatable<T>
        {
            var prevValue = value();

            action();

            return !prevValue.Equals(value());
        }
    }
}
