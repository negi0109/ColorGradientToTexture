using UnityEditor;

namespace Negi0109.ColorGradientToTexture.Filters {

    [System.Serializable]
    public class Add : ColorFilter {
        public float value;
        public override float Evaluate(float v) => v + value;

        public override bool Editor()
        {
            bool updated = false;
            var changedValue = EditorGUILayout.FloatField("value", value);
            if (changedValue != value) updated = true;

            value = changedValue;

            return updated;
        }
    }

}
