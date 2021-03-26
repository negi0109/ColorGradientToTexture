using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;

public class ColorGradientToTexture : EditorWindow
{
    private enum ColorType {
        RGB, HSV,
    }

    private class ColorAxis {
        public AnimationCurve curve;
        public int direction;

        public ColorAxis() {
            curve = new AnimationCurve();
            direction = 0;
        }

        public void Editor (string label) {
            direction = EditorGUILayout.Popup(direction, new string[]{ "x", "y" });
            curve = EditorGUILayout.CurveField(label, curve);
        }

        public float Evaluate (float x, float y) {
            switch (direction) {
                case 0: return curve.Evaluate(x);
                case 1: return curve.Evaluate(y);
                default: return 0;
            }
        }
    }

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

    private void Initialize(Vector2Int textureSize) {
        this.textureSize = textureSize;
        this.tex = new Texture2D(textureSize.x, textureSize.y, TextureFormat.ARGB32, false);
        axes = new ColorAxis[3];
        for (int i = 0; i < axes.Length; i++) {
            axes[i] = new ColorAxis();
        }
    }

    private void OnGUI()
    {
        EditorGUILayout.Popup("color", 0, new string[]{ "RGB" });

        textureSize = EditorGUILayout.Vector2IntField("size", textureSize);
        axes[0].Editor("r");
        axes[1].Editor("g");
        axes[2].Editor("b");

        if (GUILayout.Button("Preview reload")) {
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

                var r = axes[0].Evaluate(xf, yf);
                var g = axes[1].Evaluate(xf, yf);
                var b = axes[2].Evaluate(xf, yf);

                tex.SetPixel(x, y, new Color(r, g, b, 1), 0);
            }
        }
        tex.Apply();
    }

    private void Bake()
    {

    }
}
