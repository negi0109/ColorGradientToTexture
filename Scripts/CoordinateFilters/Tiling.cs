using UnityEditor;
using UnityEngine;

namespace Negi0109.ColorGradientToTexture.CoordinateFilters
{
    [System.Serializable]
    public class Tiling : CoordinateFilter
    {
        public Vector2 value = Vector2.one;

        public override Vector2 Evaluate(Vector2 v) => new Vector2(v.x * value.x % 1, v.y * value.y % 1);

        public override bool Editor() =>
            Watch(
                () => value,
                () => value = EditorGUILayout.Vector2Field("", value)
            );
    }
}
