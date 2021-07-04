﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using UnityEditor;

public class ColorGradientToTexture : EditorWindow
{
    [Serializable]
    public class ColorAxis
    {
        public AnimationCurve xCurve;
        public AnimationCurve yCurve;
        public float yDirection;

        private static int previewSize = 50;
        private Texture2D previewTex;
        private float[] xCurveValues;
        private float[] yCurveValues;

        public ColorAxis()
        {
            xCurve = AnimationCurve.Linear(0, 0, 1, 1);
            yCurve = AnimationCurve.Linear(0, 0, 1, 1);
            yDirection = 0;

            xCurveValues = new float[10];
            yCurveValues = new float[10];
        }

        public bool Editor (string label)
        {
            var updated = false;
            if (previewTex == null) {
                previewTex = new Texture2D(previewSize, previewSize, TextureFormat.ARGB32, false);
                xCurveValues = new float[10];
                yCurveValues = new float[10];
                updated = true;
            }
            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(label, new GUILayoutOption[]{ GUILayout.Width(12) });

            var tmpDirection = EditorGUILayout.Slider(yDirection, 0f, 1f);

            if (tmpDirection != yDirection) {
                yDirection = tmpDirection;
                updated = true;
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();

            GUILayout.Box(previewTex);
            EditorGUILayout.BeginVertical();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("x", new GUILayoutOption[]{ GUILayout.Width(12) });
            xCurve = EditorGUILayout.CurveField(xCurve);
            for (int i = 0; i < 10; i++) {
                var tmp = xCurve.Evaluate(i / 10f);
                if (xCurveValues[i] != tmp) updated = true;
                xCurveValues[i] = tmp;
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("y", new GUILayoutOption[]{ GUILayout.Width(12) });
            yCurve = EditorGUILayout.CurveField(yCurve);
            for (int i = 0; i < 10; i++) {
                var tmp = yCurve.Evaluate(i / 10f);
                if (yCurveValues[i] != tmp) updated = true;
                yCurveValues[i] = tmp;
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();

            if (updated) {
                for (int x = 0; x < previewSize; x++)
                {
                    for (int y = 0; y < previewSize; y++)
                    {
                        var xf = (float) x / previewSize;
                        var yf = (float) y / previewSize;
                        var v = Evaluate(xf, yf);
                        var color = new Color(v, v, v, 1);

                        previewTex.SetPixel(x, y, color, 0);
                    }
                }
                previewTex.Apply();
            }
            return updated;
        }

        public float Evaluate (float x, float y)
            => (1 - yDirection) * xCurve.Evaluate(x) + yDirection * yCurve.Evaluate(y);
    }

    private static readonly int previewSize = 150;

    public int colorMode;
    public ColorAxis[] axes;
    public Vector2Int textureSize;

    private Texture2D previewTex;

    [MenuItem("Tools/ColorGradients")]
    static void OpenWindow()
    {
        var window = GetWindow<ColorGradientToTexture>();
        window.Initialize(new Vector2Int(256, 256));
        window.Show();
    }

    private void Initialize(Vector2Int textureSize)
    {
        this.textureSize = textureSize;
        this.previewTex = new Texture2D(previewSize, previewSize, TextureFormat.ARGB32, false);
        axes = new ColorAxis[3];
        for (int i = 0; i < axes.Length; i++) {
            axes[i] = new ColorAxis();
        }
    }

    private void OnGUI()
    {
        var tmp_colorMode = EditorGUILayout.Popup("color", colorMode, new string[]{ "RGB", "HSV" });
        if (colorMode != tmp_colorMode) {
            colorMode = tmp_colorMode;

            axes = new ColorAxis[3];
            for (int i = 0; i < axes.Length; i++) {
                axes[i] = new ColorAxis();
            }
        }

        var updated = false;
        if (previewTex == null) {
            previewTex = new Texture2D(previewSize, previewSize, TextureFormat.RGB565, false);
            updated = true;
        }
        textureSize = EditorGUILayout.Vector2IntField("size", textureSize);

        if (colorMode == 0) {
            updated |= axes[0].Editor("r");
            updated |= axes[1].Editor("g");
            updated |= axes[2].Editor("b");
        } else {
            updated |= axes[0].Editor("h");
            updated |= axes[1].Editor("s");
            updated |= axes[2].Editor("v");
        }

        if (updated) {
            ReloadPreview();
        }

        GUILayout.Box(previewTex);
        if (GUILayout.Button("Bake Texture")) {
            Debug.Log("bake texture");
            Bake();
        }
    }

    private void ReloadPreview()
    {
        //fill with with the animation curve data
        for (int x = 0; x < previewSize; x++)
        {
            for (int y = 0; y < previewSize; y++)
            {
                var xf = (float) x / previewSize;
                var yf = (float) y / previewSize;
                Color color;
                if (colorMode == 0) {
                    var r = axes[0].Evaluate(xf, yf);
                    var g = axes[1].Evaluate(xf, yf);
                    var b = axes[2].Evaluate(xf, yf);
                    color = new Color(r, g, b);
                } else {
                    var h = axes[0].Evaluate(xf, yf);
                    var s = axes[1].Evaluate(xf, yf);
                    var v = axes[2].Evaluate(xf, yf);
                    color = Color.HSVToRGB(h, s, v);
                }

                previewTex.SetPixel(x, y, color, 0);
            }
        }
        previewTex.Apply();
    }

    private void Bake()
    {
        var filePath = EditorUtility.SaveFilePanelInProject("Save Asset", "default_name", "png", "");

        if (string.IsNullOrEmpty(filePath)) return;

        var tex = new Texture2D(textureSize.x, textureSize.y, TextureFormat.ARGB32, false);

        //fill with with the animation curve data
        for (int x = 0; x < textureSize.x; x++)
        {
            for (int y = 0; y < textureSize.y; y++)
            {
                var xf = (float) x / textureSize.x;
                var yf = (float) y / textureSize.y;
                Color color;
                if (colorMode == 0) {
                    var r = axes[0].Evaluate(xf, yf);
                    var g = axes[1].Evaluate(xf, yf);
                    var b = axes[2].Evaluate(xf, yf);
                    color = new Color(r, g, b);
                } else {
                    var h = axes[0].Evaluate(xf, yf);
                    var s = axes[1].Evaluate(xf, yf);
                    var v = axes[2].Evaluate(xf, yf);
                    color = Color.HSVToRGB(h, s, v);
                }

                tex.SetPixel(x, y, color, 0);
            }
        }

        var bytes = tex.EncodeToPNG();
		System.IO.File.WriteAllBytes (filePath, bytes);
        AssetDatabase.ImportAsset (filePath, ImportAssetOptions.ForceUpdate);
    }
}
