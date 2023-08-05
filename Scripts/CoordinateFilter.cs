using System;
using System.Collections.Generic;
using UnityEngine;

namespace Negi0109.ColorGradientToTexture
{
    [Serializable]
    public abstract class CoordinateFilter
    {
        public static CoordinateFilter DefaultFilter { get => new CoordinateFilters.Circle(); }
        public abstract Vector2 Evaluate(Vector2 pos);
        public virtual bool Editor() => false;

        public static bool Watch<T>(Func<T> value, Action action)
            where T : IEquatable<T>
        {
            var prevValue = value();

            action();

            return !prevValue.Equals(value());
        }
    }
}
