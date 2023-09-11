using UnityEngine;

namespace Negi0109.ColorGradientToTexture.Filters
{
    [System.Serializable]
    public class Step : ColorFilter
    {
        public int step = 1;

        public override double Evaluate(double v) => (int)(v * step) * ((double)1 / step);

        public override bool Editor() => ColorFilterEditorUtils.IntField("step", ref step, v => Mathf.Max(v, 1));
    }
}
