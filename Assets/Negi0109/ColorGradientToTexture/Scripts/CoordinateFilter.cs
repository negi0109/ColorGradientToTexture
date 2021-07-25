using System;
using System.Collections.Generic;
using UnityEngine;

namespace Negi0109.ColorGradientToTexture
{
    [Serializable]
    public class CoordinateFilter
    {
        public enum Type
        {
            Circle,
        }

        public Type type;

        public Vector2 Evaluate(Vector2 pos)
        {
            switch (type)
            {
                case Type.Circle:
                    var center = pos - new Vector2(.5f, .5f);
                    return new Vector2(
                        Mathf.Min(center.magnitude, 1f),
                        Mathf.Atan2(center.y, center.x) * Mathf.Rad2Deg / 360f + .5f
                    );
            }

            return pos;
        }
    }
}
