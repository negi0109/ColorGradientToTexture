using UnityEngine;

namespace Negi0109.ColorGradientToTexture.Filters
{
    [System.Serializable]
    public class Step : ColorFilter
    {
        public int step = 1;

        public override float Evaluate(float v) => (int)(v / (1f / step)) * (1f / step);

        public override bool Editor() => ColorFilterEditorUtils.IntField("step", ref step, v => Mathf.Max(v, 1));
    }
}
