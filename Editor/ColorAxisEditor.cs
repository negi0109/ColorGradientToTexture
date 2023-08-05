using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Negi0109.ColorGradientToTexture
{
    public class ColorAxisEditor
    {
        private readonly string label;
        private static readonly int previewSize = 50;
        private static readonly int coordinatePreviewSize = 20;
        private Texture2D previewTex;
        private Texture2D coordinatePreviewTex;

        public ColorAxisEditor(string label)
        {
            this.label = label;
        }

        public bool Editor(ColorGradient colorGradient, ColorAxis axis)
        {
            var updated = false;
            var coordinateUpdated = false;

            if (previewTex == null)
            {
                previewTex = new Texture2D(previewSize, previewSize, TextureFormat.ARGB32, false);
                coordinatePreviewTex = new Texture2D(coordinatePreviewSize, coordinatePreviewSize, TextureFormat.RG16, false);
                coordinateUpdated = updated = true;
            }

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField(label, new GUILayoutOption[] { GUILayout.Width(12) });

            if (axis.axesCount >= 2)
            {
                axis.yWeight = EditorGUILayout.Slider(axis.yWeight, 0f, 1f);
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();

            GUILayout.Box(previewTex);
            EditorGUILayout.BeginVertical();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("x", new GUILayoutOption[] { GUILayout.Width(12) });
            axis.xCurve = EditorGUILayout.CurveField(axis.xCurve);

            EditorGUILayout.EndHorizontal();

            if (axis.axesCount >= 2)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("y", new GUILayoutOption[] { GUILayout.Width(12) });
                axis.yCurve = EditorGUILayout.CurveField(axis.yCurve);

                EditorGUILayout.EndHorizontal();
            }

            if (EditorGUI.EndChangeCheck())
            {
                updated = true;
            }

            EditorGUILayout.BeginHorizontal();
            GUILayout.Box(coordinatePreviewTex);

            EditorGUILayout.BeginVertical();
            if (GUILayout.Button("Add Coordinate"))
            {
                axis.coordinateFilters.Add(new CoordinateFilter());
                coordinateUpdated = true;
            }

            var addCoordinateFilterIndex = -1;
            var removeCoordinateFilters = new List<CoordinateFilter>();
            foreach (var filter in axis.coordinateFilters)
            {
                EditorGUILayout.BeginHorizontal();
                coordinateUpdated |= CoordinateFilterEditor.Editor(filter);
                if (GUILayout.Button("x", new GUILayoutOption[] { GUILayout.Width(20) }))
                    removeCoordinateFilters.Add(filter);
                if (GUILayout.Button("+", new GUILayoutOption[] { GUILayout.Width(20) }))
                    addCoordinateFilterIndex = axis.coordinateFilters.IndexOf(filter);
                EditorGUILayout.EndHorizontal();
            }

            foreach (var removeFilter in removeCoordinateFilters)
            {
                axis.coordinateFilters.Remove(removeFilter);
                coordinateUpdated = true;
            }

            if (addCoordinateFilterIndex >= 0)
            {
                axis.coordinateFilters.Insert(addCoordinateFilterIndex, new CoordinateFilter());
                coordinateUpdated = true;
            }

            updated |= coordinateUpdated;

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("Add Filter"))
            {
                axis.colorFilters.Add(ColorFilter.DefaultFilter);
                updated = true;
            }

            var addColorFilterIndex = -1;
            var removeColorFilters = new List<ColorFilter>();

            var colorFilterClasses = System.Reflection.Assembly.GetAssembly(typeof(ColorFilter))
                .GetTypes()
                .Where(x => x.IsSubclassOf(typeof(ColorFilter)) && !x.IsAbstract);

            var colorFilterClassDictionary = colorFilterClasses.ToDictionary(v => v.Name);
            var colorFilterClassNameArray = colorFilterClasses.Select(v => v.Name).ToArray();

            // Debug.Log(string.Join( ", ", colorFilterClassNameArray));

            axis.colorFilters = axis.colorFilters.Select(filter =>
            {
                var current = filter;

                EditorGUILayout.BeginHorizontal();
                {
                    var currentClassIndex = Array.IndexOf(colorFilterClassNameArray, filter.GetType().Name);
                    var nextClassIndex = EditorGUILayout.Popup(currentClassIndex, colorFilterClassNameArray);

                    if (nextClassIndex != currentClassIndex)
                    {
                        updated |= true;

                        var nextFilter = (ColorFilter)Activator.CreateInstance(colorFilterClassDictionary[colorFilterClassNameArray[nextClassIndex]]);
                        current = axis.colorFilters[axis.colorFilters.IndexOf(filter)] = nextFilter;
                    }
                }

                updated |= current.Editor();

                // filter
                if (GUILayout.Button("x", new GUILayoutOption[] { GUILayout.Width(20) }))
                    removeColorFilters.Add(current);
                if (GUILayout.Button("+", new GUILayoutOption[] { GUILayout.Width(20) }))
                    addColorFilterIndex = axis.colorFilters.IndexOf(current);
                EditorGUILayout.EndHorizontal();

                return current;
            }).ToList();

            foreach (var removeFilter in removeColorFilters)
            {
                axis.colorFilters.Remove(removeFilter);
                updated = true;
            }

            if (addColorFilterIndex >= 0)
            {
                axis.colorFilters.Insert(addColorFilterIndex, ColorFilter.DefaultFilter);
                updated = true;
            }

            EditorGUILayout.EndVertical();

            if (updated)
            {
                if (colorGradient.axesCount == 2)
                {
                    var axisValue = axis.Evaluate(previewSize, previewSize);

                    for (int x = 0; x < previewSize; x++)
                    {
                        for (int y = 0; y < previewSize; y++)
                        {
                            var v = axisValue[x, y];
                            var color = new Color(v, v, v, 1);

                            previewTex.SetPixel(x, y, color, 0);
                        }
                    }
                }
                else
                {
                    var axisValue = axis.Evaluate(previewSize, 1);

                    for (int x = 0; x < previewSize; x++)
                    {
                        for (int y = 0; y < previewSize; y++)
                        {
                            var v = axisValue[x, 0];
                            var color = new Color(v, v, v, 1);

                            previewTex.SetPixel(x, y, color, 0);
                        }
                    }

                    for (int x = 0; x < previewSize; x++)
                    {
                        var v = (int)(axisValue[x, 0] * previewSize);
                        if (v >= 0 && v < previewSize)
                        {
                            previewTex.SetPixel(x, v, Color.green, 0);
                        }
                    }
                }

                previewTex.Apply();
            }

            if (coordinateUpdated)
            {
                for (int x = 0; x < coordinatePreviewSize; x++)
                {
                    for (int y = 0; y < coordinatePreviewSize; y++)
                    {
                        var pos = new Vector2((float)x / coordinatePreviewSize, (float)y / coordinatePreviewSize);

                        foreach (var filter in axis.coordinateFilters)
                        {
                            pos = filter.Evaluate(pos);
                        }

                        var color = new Color(pos.x, pos.y, 0, 1);

                        coordinatePreviewTex.SetPixel(x, y, color, 0);
                    }
                }
                coordinatePreviewTex.Apply();
            }
            return updated;
        }
    }
}
