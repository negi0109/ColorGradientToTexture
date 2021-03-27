using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using UnityEditor;

public class ColorGradientToTexture : EditorWindow
{
    private class ColorAxis {
        public AnimationCurve curve;
        public int direction;

        private float[] values;

        public ColorAxis()
        {
            curve = AnimationCurve.Linear(0, 0, 1, 1);
            direction = 0;
            values = new float[10];
        }

        public bool Editor (string label)
        {
            var updated = false;
            var tmp_direction = EditorGUILayout.Popup(direction, new string[]{ "x", "y" });

            if (tmp_direction != direction) {
                direction = tmp_direction;
                updated = true;
            }

            curve = EditorGUILayout.CurveField(label, curve);
            for (int i = 0; i < 10; i++) {
                var tmp = curve.Evaluate(i / 10f);
                if (values[i] != tmp) updated = true;
                values[i] = tmp;
            }

            return updated;
        }

        public float Evaluate (float x, float y)
        {
            switch (direction) {
                case 0: return curve.Evaluate(x);
                case 1: return curve.Evaluate(y);
                default: return 0;
            }
        }
    }

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
        var tmp_textureSize = EditorGUILayout.Vector2IntField("size", textureSize);
        if (tmp_textureSize.x != textureSize.x || tmp_textureSize.y != textureSize.y) {
            textureSize = tmp_textureSize;
            tex = new Texture2D(textureSize.x, textureSize.y,TextureFormat.ARGB32, false);
            updated = true;
        }

        if (colorMode == 0) {
            updated |= axes[0].Editor("r");
            updated |= axes[1].Editor("g");
            updated |= axes[2].Editor("b");
        } else {
            updated |= axes[0].Editor("h");
            updated |= axes[1].Editor("s");
            updated |= axes[2].Editor("v");
        }

        if (updated || GUILayout.Button("Preview reload")) {
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
        for (int x = 0; x < tex.width; x++)
        {
            for (int y = 0; y < tex.height; y++)
            {
                var xf = (float) x / tex.width;
                var yf = (float) y / tex.height;
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
