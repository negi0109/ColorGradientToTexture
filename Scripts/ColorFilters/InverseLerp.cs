using System;
using UnityEditor;
using UnityEngine;

namespace Negi0109.ColorGradientToTexture.Filters
{

    [System.Serializable]
    public class InverseLerp : ColorFilter
    {
        public Number min = new Number() { type = Number.Type.Min };
        public Number max = new Number() { type = Number.Type.Max };
        public bool clamped = true;

        public override void EvaluateAll(ref double[,] array)
        {
            Utils.ArraySegment2DBase<double> segment = new Utils.ArraySegment2D<double>(array);

            var minValue = min.GetValue(segment);
            var maxValue = max.GetValue(segment);

            segment.GetAll().SetValues(
                v => clamped ?
                    Math.Max(Math.Min((v - minValue) / (maxValue - minValue), 1.0), 0.0) :
                    (v - minValue) / (maxValue - minValue)
            );
        }

        public override bool Editor()
        {
            EditorGUIUtility.labelWidth = 60f;

            var updated = false;

            EditorGUILayout.BeginVertical();

            updated |= Watch(() => clamped, () => { clamped = EditorGUILayout.Toggle("Clamp01", clamped); });

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("min", new GUILayoutOption[] { GUILayout.Width(40) });

            updated |= min.Editor();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("max", new GUILayoutOption[] { GUILayout.Width(40) });
            updated |= max.Editor();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();

            EditorGUIUtility.labelWidth = 0;

            return updated;
        }
    }
}
