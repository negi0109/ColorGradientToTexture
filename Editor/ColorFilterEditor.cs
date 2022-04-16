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
                case ColorFilter.Type.Add:
                    {
                        var changedValue1 = EditorGUILayout.FloatField("value", filter.value1);
                        if (changedValue1 != filter.value1) updated = true;

                        filter.value1 = changedValue1;
                    }
                    break;
                case ColorFilter.Type.LinearToGamma:
                case ColorFilter.Type.GammaToLinear:
                    break;
                case ColorFilter.Type.Cumulate:
                    {
                        EditorGUILayout.BeginVertical();
                        EditorGUIUtility.labelWidth = 60f;
                        var changedValue1 = EditorGUILayout.Popup("Direction", (int)filter.value1, new string[] {
                            "X 0 -> 1",
                            "Y 0 -> 1",
                            "X 0 <- 1",
                            "Y 0 <- 1"
                        });
                        if (changedValue1 != filter.value1) updated = true;

                        filter.value1 = changedValue1;

                        var changedValue2 = EditorGUILayout.Popup("Division", (int)filter.value2, new string[][] {
                            new string[]{ "Last (X1)", "Width", "One" },
                            new string[]{ "Last (Y1)", "Height", "One" },
                            new string[]{ "Last (X0)", "Width", "One" },
                            new string[]{ "Last (Y0)", "Height", "One" }
                        }[changedValue1]);
                        if (changedValue2 != filter.value2) updated = true;

                        filter.value2 = changedValue2;
                        EditorGUIUtility.labelWidth = 0;
                        EditorGUILayout.EndVertical();
                    }
                    break;
            }

            return updated;
        }
    }
}
