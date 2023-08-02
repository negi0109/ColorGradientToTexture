using System;
using UnityEditor;
using UnityEngine;

namespace Negi0109.ColorGradientToTexture
{
    public static class ColorFilterEditorUtils
    {
        public static bool IntField(string name, ref int value, Func<int, int> func = null)
        {
            int v = value;
            var updated = Watch(
                () => v,
                () =>
                {
                    v = EditorGUILayout.IntField("count", v);
                    if (func != null) v = func(v);
                }
            );
            value = v;

            return updated;
        }

        public static bool Watch<T>(Func<T> value, Action action)
            where T : IEquatable<T>
        {
            var prevValue = value();

            action();

            return !prevValue.Equals(value());
        }
    }
}
