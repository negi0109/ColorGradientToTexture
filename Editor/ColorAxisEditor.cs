using System;
using System.Collections.Generic;
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


            // CoordinateFilter
            {
                axis.coordinateFilters = axis.coordinateFilters.SelectMany((value) =>
                {
                    var result = new List<CoordinateFilter>();
                    EditorGUILayout.BeginHorizontal();

                    coordinateUpdated |= PopupSubClass(ref value);
                    coordinateUpdated |= value.Editor();

                    if (GUILayout.Button("x", new GUILayoutOption[] { GUILayout.Width(20) }))
                        updated = true;
                    else result.Add(value);
                    if (GUILayout.Button("+", new GUILayoutOption[] { GUILayout.Width(20) })) result.Add(CoordinateFilter.DefaultFilter);
                    EditorGUILayout.EndHorizontal();

                    return result;
                }).ToList();

                if (GUILayout.Button("Add Coordinate"))
                {
                    axis.coordinateFilters.Add(CoordinateFilter.DefaultFilter);
                    coordinateUpdated = true;
                }

                updated |= coordinateUpdated;
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();

            // ColorFilter
            {
                axis.colorFilters = axis.colorFilters.SelectMany((value) =>
                {
                    var result = new List<ColorFilter>();
                    EditorGUILayout.BeginHorizontal();

                    updated |= PopupSubClass(ref value);
                    updated |= value.Editor();

                    if (GUILayout.Button("x", new GUILayoutOption[] { GUILayout.Width(20) }))
                        updated = true;
                    else result.Add(value);

                    if (GUILayout.Button("+", new GUILayoutOption[] { GUILayout.Width(20) })) result.Add(ColorFilter.DefaultFilter);

                    EditorGUILayout.EndHorizontal();

                    return result;
                }).ToList();

                if (GUILayout.Button("Add Filter"))
                {
                    axis.colorFilters.Add(ColorFilter.DefaultFilter);
                    updated = true;
                }
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
                            var color = new Color((float)v, (float)v, (float)v, 1);

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
                            var color = new Color((float)v, (float)v, (float)v, 1);

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

        public bool PopupSubClass<T>(ref T value)
        {
            var classes = System.Reflection.Assembly.GetAssembly(typeof(T))
                .GetTypes()
                .Where(x => x.IsSubclassOf(typeof(T)) && !x.IsAbstract).ToList();
            var classNameArray = classes.Select(v => v.Name).ToArray();


            var currentClassIndex = Array.IndexOf(classNameArray, value.GetType().Name);
            var nextClassIndex = EditorGUILayout.Popup(currentClassIndex, classNameArray);
            if (currentClassIndex == nextClassIndex) return false;

            value = (T)Activator.CreateInstance(classes[nextClassIndex]);
            return true;
        }
    }
}
