using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using UnityEditor;

namespace Negi0109.ColorGradientToTexture
{
    public class ColorGradientToTextureEditor : EditorWindow
    {
        private static readonly int previewSize = 150;

        public ColorGradient colorGradient;
        public Vector2Int textureSize;

        private ColorAxisEditor[] colorAxisEditors;
        private Texture2D previewTex;

        [MenuItem("Tools/ColorGradients")]
        public static void OpenWindow()
        {
            var window = GetWindow<ColorGradientToTextureEditor>();
            window.Initialize();
            window.Show();
        }

        private void Initialize()
        {
            textureSize = new Vector2Int(256, 256);
            colorGradient = new ColorGradient(0);

            InitializeEditor();
        }

        private void InitializeEditor()
        {
            previewTex = new Texture2D(previewSize, previewSize, TextureFormat.RGB565, false);
            colorAxisEditors = new ColorAxisEditor[colorGradient.Mode.size];
            for (int i = 0; i < colorGradient.Mode.size; i++)
            {
                colorAxisEditors[i] = new ColorAxisEditor(colorGradient.Mode.axisNames[i].ToString());
            }
        }

        private void OnGUI()
        {
            var tmp_colorMode = EditorGUILayout.Popup("color", colorGradient.colorMode, ColorMode.names);
            if (colorGradient.colorMode != tmp_colorMode)
            {
                colorGradient = new ColorGradient(tmp_colorMode);

                InitializeEditor();
            }

            var updated = false;
            if (previewTex == null || colorAxisEditors == null)
            {
                InitializeEditor();
                updated = true;
            }

            textureSize = EditorGUILayout.Vector2IntField("size", textureSize);
            textureSize.x = Math.Max(textureSize.x, 1);
            textureSize.y = Math.Max(textureSize.y, 1);

            for (int i = 0; i < colorGradient.Mode.size; i++)
                updated |= colorAxisEditors[i].Editor(colorGradient.axes[i]);


            if (updated)
            {
                colorGradient.SetTexturePixel(previewTex);
                previewTex.Apply();
            }

            GUILayout.Box(previewTex);
            if (GUILayout.Button("Bake Texture"))
                Bake();

        }

        private void Bake()
        {
            var filePath = EditorUtility.SaveFilePanelInProject("Save Asset", "default_name", "png", "");

            if (string.IsNullOrEmpty(filePath))
                return;

            var tex = new Texture2D(textureSize.x, textureSize.y, TextureFormat.ARGB32, false);

            colorGradient.SetTexturePixel(tex);

            var bytes = tex.EncodeToPNG();
            System.IO.File.WriteAllBytes(filePath, bytes);
            AssetDatabase.ImportAsset(filePath, ImportAssetOptions.ForceUpdate);
        }
    }
}
