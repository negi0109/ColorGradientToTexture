using System;
using System.Collections.Generic;
using UnityEngine;

namespace Negi0109.ColorGradientToTexture
{
    [Serializable]
    public class CoordinateFilter
    {
        public static string[] types = new string[] {
            "Circle",
            "Tiling",
            "Offset",
            "Reverse",
            "X <-> Y",
        };

        public enum Type
        {
            Circle,
            Tiling,
            Offset,
            Reverse,
            Turn,
        }

        public Type type;
        public float value1;
        public float value2;

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
                case Type.Tiling:
                    return new Vector2(pos.x * value1 % 1, pos.y * value2 % 1);
                case Type.Offset:
                    return new Vector2((pos.x + value1) % 1, (pos.y + value2) % 1);
                case Type.Reverse:
                    var a = (int)value1;
                    var x = a == 0 || a == 2;
                    var y = a == 1 || a == 2;

                    return new Vector2(x ? 1 - pos.x : pos.x, y ? 1 - pos.y : pos.y);
                case Type.Turn:
                    return new Vector2(pos.y, pos.x);
            }

            return pos;
        }
    }
}
