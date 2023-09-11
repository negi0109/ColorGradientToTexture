using System;
using UnityEditor;
using System.Linq;

namespace Negi0109.ColorGradientToTexture.Filters
{
    [System.Serializable]
    public class Number
    {
        public enum Type { Free, Avg, Min, Max }

        public Type type;
        public double value;

        public double GetValue(Utils.ArraySegment2DBase<double> segment)
        => type switch
        {
            Type.Free => value,
            Type.Avg => segment.GetAll().Average(),
            Type.Min => segment.GetAll().Min(),
            Type.Max => segment.GetAll().Max(),
            _ => 0
        };

        public bool Editor()
        {
            var updated = Watch(
                () => (int)type,
                () => { type = (Type)EditorGUILayout.EnumPopup(type); }
            );
            if (type == Type.Free)
            {
                updated |= Watch(
                    () => value,
                    () => { value = EditorGUILayout.DoubleField("value", value); }
                );
            }
            return updated;
        }

        private static bool Watch<T>(Func<T> value, Action action)
        where T : IEquatable<T>
        {
            var prevValue = value();

            action();

            return !prevValue.Equals(value());
        }
    }
}
