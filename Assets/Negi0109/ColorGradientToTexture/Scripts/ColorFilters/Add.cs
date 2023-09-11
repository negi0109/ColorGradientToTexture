using UnityEditor;

namespace Negi0109.ColorGradientToTexture.Filters
{

    [System.Serializable]
    public class Add : ColorFilter
    {
        public double value;
        public override double Evaluate(double v) => v + value;

        public override bool Editor()
        {
            return Watch(
                () => value,
                () => { value = EditorGUILayout.DoubleField("value", value); }
            );
        }
    }
}
