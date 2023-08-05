using UnityEngine;

namespace Negi0109.ColorGradientToTexture.Filters
{
    [System.Serializable]
    public class Repeat : ColorFilter
    {
        public int count = 1;

        public override float Evaluate(float v) => v * count % 1f;

        public override bool Editor() => ColorFilterEditorUtils.IntField("count", ref count, v => Mathf.Max(v, 1));
    }
}
