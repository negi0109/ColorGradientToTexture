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
        public float value;

        public float GetValue(Utils.ArraySegment2DBase<float> segment = null)
        {
            if (type != Type.Free && segment == null) throw new ArgumentException();

            switch (type)
            {
                case Type.Free:
                    return value; break;
                case Type.Avg:
                    return segment.GetAll().Average(); break;
                case Type.Min:
                    return segment.GetAll().Min(); break;
                case Type.Max:
                    return segment.GetAll().Max(); break;
                default: return 0;
            }
        }

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
                    () => { value = EditorGUILayout.FloatField("value", value); }
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
