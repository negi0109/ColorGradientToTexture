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
    private AnimationCurve rCurve;
    private AnimationCurve gCurve;
    private AnimationCurve bCurve;

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
        rCurve = new AnimationCurve();
        gCurve = new AnimationCurve();
        bCurve = new AnimationCurve();
    }

    private void OnGUI()
    {
        EditorGUILayout.Popup("color", 0, new string[]{ "RGB" });
        textureSize = EditorGUILayout.Vector2IntField("size", textureSize);
        EditorGUILayout.LabelField("r");
        EditorGUILayout.Popup(0, new string[]{ "x" });
        rCurve = EditorGUILayout.CurveField(rCurve);
        EditorGUILayout.LabelField("g");
        EditorGUILayout.Popup(0, new string[]{ "x" });
        gCurve = EditorGUILayout.CurveField(gCurve);
        EditorGUILayout.LabelField("b");
        EditorGUILayout.Popup(0, new string[]{ "y" });
        bCurve = EditorGUILayout.CurveField(bCurve);

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
            var r = rCurve.Evaluate((float)x / tex.width);
            var g = gCurve.Evaluate((float)x / tex.width);
            for (int y = 0; y < tex.height; y++)
            {
                var b = bCurve.Evaluate((float)y / tex.height);

                tex.SetPixel(x, y, new Color(r, g, b, 1), 0);
            }
        }
        tex.Apply();
    }

    private void Bake()
    {

    }
}
