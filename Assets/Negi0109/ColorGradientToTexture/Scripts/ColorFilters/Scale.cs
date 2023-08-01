using UnityEngine;
using UnityEditor;

namespace Negi0109.ColorGradientToTexture.Filters {

    [System.Serializable]
    public class Scale : ColorFilter {
        public float value = 1f;

        public override float Evaluate(float v) => v * value;

        public override bool Editor()
        {
            bool updated = false;
            var changedValue = Mathf.Max(EditorGUILayout.FloatField("weight", value), 0);
            if (changedValue != value) updated = true;

            value = changedValue;

            return updated;
        }
    }

}
