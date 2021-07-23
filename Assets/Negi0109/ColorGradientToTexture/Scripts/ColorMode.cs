using System;
using UnityEngine;

namespace Negi0109.ColorGradientToTexture
{
    [Serializable]
    public class ColorMode
    {
        public static readonly ColorMode[] modes = new ColorMode[] {
            new ColorMode(
                "RGB", 3, (float[] values) => new Color(values[0], values[1], values[2])
            ),
            new ColorMode(
                "HSV", 3, (float[] values) => Color.HSVToRGB(values[0], values[1], values[2])
            ),
        };

        public string name;
        public int size;
        public Func<float[], Color> func;

        public ColorMode(string name, int size, Func<float[], Color> func)
        {
            this.name = name;
            this.size = size;
            this.func = func;
        }
    }
}
