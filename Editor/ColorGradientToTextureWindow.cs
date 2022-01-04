using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using UnityEditor;

namespace Negi0109.ColorGradientToTexture
{
    public class ColorGradientToTextureWindow : EditorWindow
    {
        private static readonly int previewSize = 150;

        public ColorGradient colorGradient;

        private ColorAxisEditor[] colorAxisEditors;
        private Texture2D previewTex;
        private RenderTexture renderTexture;

        [MenuItem("Tools/ColorGradients")]
        public static void OpenWindow()
        {
            var window = GetWindow<ColorGradientToTextureWindow>();
            window.Initialize();
            window.Show();
        }

        private void Initialize()
        {
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
            var tmp_axesCount = EditorGUILayout.Toggle("y", colorGradient.axesCount == 2) ? 2 : 1;

            if (colorGradient.colorMode != tmp_colorMode || colorGradient.axesCount != tmp_axesCount)
            {
                colorGradient = new ColorGradient(tmp_colorMode, tmp_axesCount);

                InitializeEditor();
            }

            var updated = false;

            if (previewTex == null || colorAxisEditors == null)
            {
                colorGradient = new ColorGradient(tmp_colorMode);
                InitializeEditor();
                updated = true;
            }

            if (colorGradient.axesCount == 2)
            {
                colorGradient.textureSize = EditorGUILayout.Vector2IntField("size", colorGradient.textureSize);
                colorGradient.textureSize.x = Math.Max(colorGradient.textureSize.x, 1);
                colorGradient.textureSize.y = Math.Max(colorGradient.textureSize.y, 1);
            }
            else
            {
                var width = EditorGUILayout.IntField("size", colorGradient.textureSize.x);
                colorGradient.textureSize.x = Math.Max(width, 1);
                colorGradient.textureSize.y = 1;
            }

            for (int i = 0; i < colorGradient.Mode.size; i++)
                updated |= colorAxisEditors[i].Editor(colorGradient.axes[i]);

            if (GUILayout.Button("Bake Texture"))
                Bake();

            EditorGUILayout.LabelField("Preview(RenderTexture)");
            var tmpRenderTexture = (RenderTexture)EditorGUILayout.ObjectField(renderTexture, typeof(RenderTexture), false);
            updated |= renderTexture != tmpRenderTexture;
            renderTexture = tmpRenderTexture;

            if (updated && renderTexture == null)
            {
                colorGradient.SetTexturePixel(previewTex);
                previewTex.Apply();
            }

            if (updated && renderTexture != null)
            {
                var previewRenderTex = new Texture2D(colorGradient.textureSize.x, colorGradient.textureSize.y, TextureFormat.ARGB32, false);
                colorGradient.SetTexturePixel(previewRenderTex);
                previewRenderTex.Apply();
                Graphics.Blit(previewRenderTex, renderTexture);
            }

            if (renderTexture == null)
                GUILayout.Box(previewTex);
            else
                GUILayout.Box(renderTexture);
        }

        private void Bake()
        {
            var filePath = EditorUtility.SaveFilePanelInProject("Save Asset", "default_name", "png", "");

            if (string.IsNullOrEmpty(filePath))
                return;

            var tex = new Texture2D(colorGradient.textureSize.x, colorGradient.textureSize.y, TextureFormat.ARGB32, false);

            colorGradient.SetTexturePixel(tex);

            var bytes = tex.EncodeToPNG();
            System.IO.File.WriteAllBytes(filePath, bytes);
            AssetDatabase.ImportAsset(filePath, ImportAssetOptions.ForceUpdate);
        }
    }
}
