using System;
using System.Linq;
using UnityEngine;

namespace Negi0109.ColorGradientToTexture
{
    [Serializable]
    public class ColorMode
    {
        public static readonly ColorMode[] modes = new ColorMode[] {
            new ColorMode(
                "RGB", 3, (double[] values) => new Color((float)values[0], (float)values[1], (float)values[2])
            ),
            new ColorMode(
                "HSV", 3, (double[] values) => Color.HSVToRGB((float)values[0], (float)values[1], (float)values[2])
            ),
            new ColorMode(
                "Grayscale", 1, (double[] values) => new Color((float)values[0], (float)values[0], (float)values[0]), "V"
            ),
        };
        public static string[] names = modes.Select(v => v.name).ToArray();

        public string name;
        public string axisNames;
        public int size;
        public Func<double[], Color> func;

        public ColorMode(string name, int size, Func<double[], Color> func, string axisNames = null)
        {
            this.name = name;
            this.size = size;
            this.func = func;
            this.axisNames = axisNames ?? this.name;
        }
    }
}
