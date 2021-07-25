using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Negi0109.ColorGradientToTexture
{
    public static class ColorFilterEditor
    {
        public static bool Editor(ColorFilter filter)
        {
            bool updated = false;
            var changedType = (ColorFilter.Type)EditorGUILayout.EnumPopup(filter.type);

            if (changedType != filter.type) updated = true;
            filter.type = changedType;

            switch (filter.type)
            {
                case ColorFilter.Type.Step:
                    {
                        var changedValue1 = Mathf.Max(EditorGUILayout.IntField("step", (int)filter.value1), 1);
                        if (changedValue1 != (int)filter.value1) updated = true;

                        filter.value1 = changedValue1;
                    }
                    break;
                case ColorFilter.Type.Noise:
                    {
                        var changedValue1 = EditorGUILayout.Slider("weight", filter.value1, -1f, 1f);
                        if (changedValue1 != filter.value1) updated = true;

                        filter.value1 = changedValue1;
                    }
                    break;
                case ColorFilter.Type.Repeat:
                    {
                        var changedValue1 = Math.Max(EditorGUILayout.IntField("count", (int)filter.value1), 1);
                        if (changedValue1 != filter.value1) updated = true;

                        filter.value1 = changedValue1;
                    }
                    break;
                case ColorFilter.Type.Scale:
                    {
                        var changedValue1 = Mathf.Max(EditorGUILayout.FloatField("weight", filter.value1), 0);
                        if (changedValue1 != filter.value1) updated = true;

                        filter.value1 = changedValue1;
                    }
                    break;
                case ColorFilter.Type.Threshold:
                    {
                        var changedValue1 = EditorGUILayout.Slider("weight", filter.value1, -0f, 1f);
                        if (changedValue1 != filter.value1) updated = true;

                        filter.value1 = changedValue1;
                    }
                    break;
            }

            return updated;
        }
    }
}
