using UnityEditor;
using UnityEngine;

namespace Negi0109.ColorGradientToTexture.CoordinateFilters
{
    [System.Serializable]
    public class Reverse : CoordinateFilter
    {
        public enum Mode
        {
            Both, X, Y
        }
        public Mode mode = Mode.Both;

        public override Vector2 Evaluate(Vector2 v)
        {
            var tmp = v;
            if (mode == Mode.Both || mode == Mode.X) tmp.x = 1 - tmp.x;
            if (mode == Mode.Both || mode == Mode.Y) tmp.y = 1 - tmp.y;

            return tmp;
        }

        public override bool Editor() =>
            Watch(
                () => (int)mode,
                () => mode = (Mode)EditorGUILayout.EnumPopup(mode)
            );
    }
}
