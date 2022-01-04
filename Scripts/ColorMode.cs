using System;
using UnityEngine;
using System.Linq;

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
            new ColorMode(
                "Grayscale", 1, (float[] values) => new Color(values[0], values[0], values[0]), "V"
            ),
        };
        public static string[] names = modes.Select(v => v.name).ToArray();

        public string name;
        public string axisNames;
        public int size;
        public Func<float[], Color> func;

        public ColorMode(string name, int size, Func<float[], Color> func, string axisNames = null)
        {
            this.name = name;
            this.size = size;
            this.func = func;
            this.axisNames = axisNames ?? this.name;
        }
    }
}
