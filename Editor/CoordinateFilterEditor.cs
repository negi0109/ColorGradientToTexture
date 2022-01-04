using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Negi0109.ColorGradientToTexture
{
    public static class CoordinateFilterEditor
    {
        private static readonly string[] reverseModes = new string[] { "x", "y", "both" };

        public static bool Editor(CoordinateFilter filter)
        {
            bool updated = false;
            var changedType = (CoordinateFilter.Type)EditorGUILayout.Popup((int)filter.type, CoordinateFilter.types);

            if (changedType != filter.type) updated = true;
            filter.type = changedType;

            switch (filter.type)
            {
                case CoordinateFilter.Type.Circle:
                    break;
                case CoordinateFilter.Type.Tiling:
                    {
                        var changedValue = EditorGUILayout.Vector2Field("", new Vector2(filter.value1, filter.value2));
                        changedValue = new Vector2(Mathf.Max(changedValue.x, 0), Mathf.Max(changedValue.y, 0));
                        if (changedValue.x != filter.value1) updated = true;
                        if (changedValue.y != filter.value2) updated = true;

                        filter.value1 = changedValue.x;
                        filter.value2 = changedValue.y;
                    }
                    break;
                case CoordinateFilter.Type.Offset:
                    {
                        var changedValue = EditorGUILayout.Vector2Field("", new Vector2(filter.value1, filter.value2));
                        changedValue = new Vector2(Mathf.Min(Mathf.Max(changedValue.x, 0), 1), Mathf.Min(Mathf.Max(changedValue.y, 0), 1));
                        if (changedValue.x != filter.value1) updated = true;
                        if (changedValue.y != filter.value2) updated = true;

                        filter.value1 = changedValue.x;
                        filter.value2 = changedValue.y;
                    }
                    break;
                case CoordinateFilter.Type.Reverse:
                    {
                        int changedValue = EditorGUILayout.Popup((int)filter.value1, reverseModes);
                        if (changedValue != (int)filter.value1) updated = true;

                        filter.value1 = changedValue;
                    }
                    break;
            }

            return updated;
        }
    }
}
