using System;
using System.Linq;
using UnityEngine;

namespace Negi0109.ColorGradientToTexture
{
    [Serializable]
    public class ColorGradient
    {
        public readonly int colorMode;
        public ColorAxis[] axes;
        public ColorMode Mode { get => ColorMode.modes[colorMode]; }

        public ColorGradient(int colorMode)
        {
            this.colorMode = colorMode;
            this.axes = new ColorAxis[Mode.size];
            for (int i = 0; i < Mode.size; i++)
                this.axes[i] = new ColorAxis();
        }

        public Color GetColor(float x, float y)
        {
            float[] values = new float[Mode.size];

            for (int i = 0; i < values.Length; i++)
                values[i] = axes[i].Evaluate(x, y);

            return Mode.func(values);
        }

        public void SetTexturePixel(Texture2D tex)
        {
            for (int x = 0; x < tex.width; x++)
                for (int y = 0; y < tex.height; y++)
                    tex.SetPixel(x, y, Mode.func(
                        axes.Select((v) => v.Evaluate(
                            (float)x / tex.width,
                            (float)y / tex.height
                        )).ToArray()
                    ), 0);
        }
    }
}
