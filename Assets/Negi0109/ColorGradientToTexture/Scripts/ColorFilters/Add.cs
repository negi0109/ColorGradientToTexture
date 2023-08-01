using UnityEditor;

namespace Negi0109.ColorGradientToTexture.Filters {

    [System.Serializable]
    public class Add : ColorFilter {
        public float value;
        public override float Evaluate(float v) => v + value;

        public override bool Editor()
        {
            return Watch(
                () => value,
                () => { value = EditorGUILayout.FloatField("value", value); }
            );
        }
    }

}
