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
            Scale,
            Threshold,
            Add,
            LinearToGamma,
            GammaToLinear,
        }

        public Type type = Type.Add;
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
                case Type.Threshold:
                    return v >= value1 ? 1 : 0;
                case Type.Add:
                    return v + value1;
                case Type.GammaToLinear:
                    return Mathf.GammaToLinearSpace(v);
                case Type.LinearToGamma:
                    return Mathf.LinearToGammaSpace(v);
                default:
                    return v;
            }
        }
    }
}
