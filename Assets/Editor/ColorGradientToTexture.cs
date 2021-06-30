using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using UnityEditor;

public class ColorGradientToTexture : EditorWindow
{
    private class ColorAxis
    {
        private AnimationCurve xCurve;
        private AnimationCurve yCurve;
        private float xDirection;

        private float[] xCurveValues;
        private float[] yCurveValues;

        public ColorAxis()
        {
            xCurve = AnimationCurve.Linear(0, 0, 1, 1);
            yCurve = AnimationCurve.Linear(0, 0, 1, 1);
            xDirection = 0;
            xCurveValues = new float[10];
            yCurveValues = new float[10];
        }

        public bool Editor (string label)
        {
            var updated = false;
            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(label, new GUILayoutOption[]{ GUILayout.Width(12) });

            var tmpXDirection = EditorGUILayout.Slider(xDirection, 0f, 1f);

            if (tmpXDirection != xDirection) {
                xDirection = tmpXDirection;
                updated = true;
            }

            EditorGUILayout.EndHorizontal();
            xCurve = EditorGUILayout.CurveField("x", xCurve);
            for (int i = 0; i < 10; i++) {
                var tmp = xCurve.Evaluate(i / 10f);
                if (xCurveValues[i] != tmp) updated = true;
                xCurveValues[i] = tmp;
            }

            yCurve = EditorGUILayout.CurveField("y", yCurve);
            for (int i = 0; i < 10; i++) {
                var tmp = yCurve.Evaluate(i / 10f);
                if (yCurveValues[i] != tmp) updated = true;
                yCurveValues[i] = tmp;
            }
            EditorGUILayout.EndVertical();

            return updated;
        }

        public float Evaluate (float x, float y)
            => xDirection * xCurve.Evaluate(x) + (1 - xDirection) * yCurve.Evaluate(y);
    }

    private static readonly int previewSize = 150;

    private int colorMode;
    private ColorAxis[] axes;
    private Vector2Int textureSize;
    private Texture2D tex;

    [MenuItem("Tools/ColorGradients/Bake texture")]
    static void OpenWindow()
    {
        Debug.Log("Open Window");
        var window = GetWindow<ColorGradientToTexture>();
        window.Initialize(new Vector2Int(256, 256));
        window.Show();
    }

    private void Initialize(Vector2Int textureSize)
    {
        this.textureSize = textureSize;
        this.tex = new Texture2D(textureSize.x, textureSize.y, TextureFormat.ARGB32, false);
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

        if (updated | GUILayout.Button("Preview reload")) {
            Debug.Log("Preview");
            Preview();
        }

        GUILayout.Box(tex);
        if (GUILayout.Button("Bake Texture")) {
            Debug.Log("bake texture");
        }
    }

    private void Preview()
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

                tex.SetPixel(x, y, color, 0);
            }
        }
        tex.Apply();
    }

    private void Bake()
    {

    }
}
