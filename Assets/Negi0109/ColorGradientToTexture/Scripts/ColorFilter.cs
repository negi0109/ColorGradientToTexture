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
            OneMinus,
            Cumulate,
        }

        public Type type = Type.Add;
        public float value1;
        public float value2;

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
                case Type.OneMinus:
                    return 1 - v;
                default:
                    return v;
            }
        }

        public void EvaluateValues(ref float[,] v)
        {
            var width = v.GetLength(0);
            var height = v.GetLength(1);

            switch (type)
            {
                case Type.Cumulate:
                    {
                        var type1 = (int)value1;
                        var type2 = (int)value2;
                        switch (type1)
                        {
                            case 0:
                                for (int y = 0; y < height; y++)
                                {
                                    for (int x = 1; x < width; x++)
                                    {
                                        v[x, y] = v[x - 1, y] + v[x, y];
                                    }

                                    var a = 1f;
                                    if (type2 == 0) a = v[width - 1, y];
                                    else if (type2 == 1) a = width;
                                    else if (type2 == 2) a = 1;

                                    if (a != 1f)
                                        for (int x = 0; x < width; x++)
                                            v[x, y] = v[x, y] / a;
                                }
                                break;
                            case 1:
                                for (int x = 0; x < width; x++)
                                {
                                    for (int y = 1; y < height; y++)
                                    {
                                        v[x, y] = v[x, y - 1] + v[x, y];
                                    }

                                    var a = 1f;
                                    if (type2 == 0) a = v[x, height - 1];
                                    else if (type2 == 1) a = height;
                                    else if (type2 == 2) a = 1;

                                    if (a != 1f)
                                        for (int y = 0; y < height; y++)
                                            v[x, y] = v[x, y] / a;
                                }
                                break;
                            case 2:
                                for (int y = 0; y < height; y++)
                                {
                                    for (int x = width - 2; x >= 0; x--)
                                    {
                                        v[x, y] = v[x + 1, y] + v[x, y];
                                    }

                                    var a = 1f;
                                    if (type2 == 0) a = v[0, y];
                                    else if (type2 == 1) a = width;
                                    else if (type2 == 2) a = 1;

                                    if (a != 1f)
                                        for (int x = 0; x < width; x++)
                                            v[x, y] = v[x, y] / a;
                                }
                                break;
                            case 3:
                                for (int x = 0; x < width; x++)
                                {
                                    for (int y = height - 2; y >= 0; y--)
                                    {
                                        v[x, y] = v[x, y + 1] + v[x, y];
                                    }

                                    var a = 1f;
                                    if (type2 == 0) a = v[x, 0];
                                    else if (type2 == 1) a = height;
                                    else if (type2 == 2) a = 1;

                                    if (a != 1f)
                                        for (int y = 0; y < height; y++)
                                            v[x, y] = v[x, y] / a;
                                }
                                break;
                        }
                    }
                    break;
            }
        }

        public void Evaluate(ref float[,] v)
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

            EvaluateValues(ref v);
        }
    }
}
