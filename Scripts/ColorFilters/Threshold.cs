using UnityEditor;

namespace Negi0109.ColorGradientToTexture.Filters
{
    [System.Serializable]
    public class Threshold : ColorFilter
    {
        public float value;
        public override float Evaluate(float v) => v >= value ? 1 : 0;

        public override bool Editor()
        {
            return Watch(
                () => value,
                () => { value = EditorGUILayout.Slider("value", value, 0f, 1f); }
            );
        }
    }
}
