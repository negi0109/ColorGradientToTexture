using System;
using System.Collections.Generic;
using UnityEngine;

namespace Negi0109.ColorGradientToTexture
{
    [Serializable]
    public class ColorFilter
    {
        public enum Type
        {
            Step,
            Noise,
            Repeat,
            Scale
        }

        public Type type;
        public float value1;

        public float Evaluate(float v)
        {
            switch (type)
            {
                case Type.Step:
                    var step = 1f / (int)value1;
                    return (int)(v / step) * step;
                case Type.Noise:
                    return v + UnityEngine.Random.Range(0f, 1f) * value1;
                case Type.Repeat:
                    return v * value1 % 1f;
                case Type.Scale:
                    return v * value1;
                default:
                    return v;
            }
        }
    }
}
