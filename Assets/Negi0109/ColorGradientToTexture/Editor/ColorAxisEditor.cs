using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using UnityEditor;

namespace Negi0109.ColorGradientToTexture
{
    public class ColorAxisEditor
    {
        private readonly string label;
        private static readonly int previewSize = 50;
        private static readonly int cachedSize = 10;
        private Texture2D previewTex;
        private float[] xCurveValues;
        private float[] yCurveValues;

        public ColorAxisEditor(string label)
        {
            this.label = label;
        }

        public bool Editor(ColorAxis axis)
        {
            var updated = false;
            if (previewTex == null)
            {
                previewTex = new Texture2D(previewSize, previewSize, TextureFormat.ARGB32, false);
                xCurveValues = new float[cachedSize];
                yCurveValues = new float[cachedSize];
                updated = true;
            }

            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(label, new GUILayoutOption[] { GUILayout.Width(12) });

            var tmpDirection = EditorGUILayout.Slider(axis.yDirection, 0f, 1f);

            if (tmpDirection != axis.yDirection)
            {
                axis.yDirection = tmpDirection;
                updated = true;
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();

            GUILayout.Box(previewTex);
            EditorGUILayout.BeginVertical();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("x", new GUILayoutOption[] { GUILayout.Width(12) });
            axis.xCurve = EditorGUILayout.CurveField(axis.xCurve);
            for (int i = 0; i < cachedSize; i++)
            {
                var tmp = axis.xCurve.Evaluate(i / (float)cachedSize);
                if (xCurveValues[i] != tmp) updated = true;
                xCurveValues[i] = tmp;
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("y", new GUILayoutOption[] { GUILayout.Width(12) });
            axis.yCurve = EditorGUILayout.CurveField(axis.yCurve);
            for (int i = 0; i < cachedSize; i++)
            {
                var tmp = axis.yCurve.Evaluate(i / (float)cachedSize);
                if (yCurveValues[i] != tmp) updated = true;
                yCurveValues[i] = tmp;
            }
            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("Add Coordinate"))
            {
                axis.coordinateFilters.Add(new CoordinateFilter());
                updated = true;
            }

            var removeCoordinateFilters = new List<CoordinateFilter>();
            foreach (var filter in axis.coordinateFilters)
            {
                EditorGUILayout.BeginHorizontal();
                updated |= CoordinateFilterEditor.Editor(filter);
                if (GUILayout.Button("x", new GUILayoutOption[] { GUILayout.Width(20) }))
                    removeCoordinateFilters.Add(filter);
                EditorGUILayout.EndHorizontal();
            }

            foreach (var removeFilter in removeCoordinateFilters)
            {
                axis.coordinateFilters.Remove(removeFilter);
                updated = true;
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("Add Filter"))
            {
                axis.colorFilters.Add(new ColorFilter());
                updated = true;
            }

            var removeColorFilters = new List<ColorFilter>();
            foreach (var filter in axis.colorFilters)
            {
                EditorGUILayout.BeginHorizontal();
                updated |= ColorFilterEditor.Editor(filter);
                if (GUILayout.Button("x", new GUILayoutOption[] { GUILayout.Width(20) }))
                    removeColorFilters.Add(filter);
                EditorGUILayout.EndHorizontal();
            }

            foreach (var removeFilter in removeColorFilters)
            {
                axis.colorFilters.Remove(removeFilter);
                updated = true;
            }

            EditorGUILayout.EndVertical();

            if (updated)
            {
                for (int x = 0; x < previewSize; x++)
                {
                    for (int y = 0; y < previewSize; y++)
                    {
                        var xf = (float)x / previewSize;
                        var yf = (float)y / previewSize;
                        var v = axis.Evaluate(xf, yf);
                        var color = new Color(v, v, v, 1);

                        previewTex.SetPixel(x, y, color, 0);
                    }
                }
                previewTex.Apply();
            }
            return updated;
        }
    }
}
