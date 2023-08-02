using UnityEditor;
using UnityEngine;

namespace Negi0109.ColorGradientToTexture.Filters
{
    [System.Serializable]
    public class Scale : ColorFilter
    {
        public float value = 1f;

        public override float Evaluate(float v) => v * value;

        public override bool Editor()
        {
            return Watch(
                () => value,
                () => { value = Mathf.Max(EditorGUILayout.FloatField("weight", value), 0); }
            );
        }
    }
}
