using UnityEngine;

namespace Negi0109.ColorGradientToTexture.Filters
{
    [System.Serializable]
    public class Repeat : ColorFilter
    {
        public int count = 1;

        public override double Evaluate(double v) => v * count % 1;

        public override bool Editor() => ColorFilterEditorUtils.IntField("count", ref count, v => Mathf.Max(v, 1));
    }
}
