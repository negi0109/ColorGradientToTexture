using UnityEditor;

namespace Negi0109.ColorGradientToTexture.Filters
{
    [System.Serializable]
    public class Noise : ColorFilter
    {

        public float weight = 0.5f;
        public int seed = new System.Random().Next();

        public override void EvaluateAll(ref double[,] array)
        {
            var r = new System.Random(seed);

            Utils.ArraySegment2DUtils.SetValues(array, v => v + weight * r.Next() / int.MaxValue);
        }

        public override bool Editor()
        {
            EditorGUILayout.BeginVertical();
            EditorGUIUtility.labelWidth = 60f;

            var updated = Watch(
                () => weight,
                () => { weight = EditorGUILayout.Slider("weight", weight, 0f, 1f); }
            );
            updated |= ColorFilterEditorUtils.IntField("seed", ref seed);

            EditorGUIUtility.labelWidth = 0;
            EditorGUILayout.EndVertical();

            return updated;
        }
    }
}
