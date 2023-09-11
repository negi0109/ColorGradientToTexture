using UnityEditor;

namespace Negi0109.ColorGradientToTexture.Filters
{
    [System.Serializable]
    public class Threshold : ColorFilter
    {
        public double value;
        public override double Evaluate(double v) => v >= value ? 1 : 0;

        public override bool Editor()
        {
            return Watch(
                () => value,
                () => { value = EditorGUILayout.Slider("value", (float)value, 0f, 1f); }
            );
        }
    }
}
