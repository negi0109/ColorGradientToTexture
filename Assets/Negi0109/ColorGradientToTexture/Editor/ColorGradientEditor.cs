using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using UnityEditor;

namespace Negi0109.ColorGradientToTexture
{
    [CustomEditor(typeof(ColorGradient))]
    public class ColorGradientEditor : Editor
    {
        private ColorAxisEditor[] colorAxisEditors;

        public void InitializeEditor()
        {
            var colorGradient = target as ColorGradient;

            colorAxisEditors = new ColorAxisEditor[colorGradient.Mode.size];
            for (int i = 0; i < colorGradient.Mode.size; i++)
            {
                colorAxisEditors[i] = new ColorAxisEditor(colorGradient.Mode.axisNames[i].ToString());
            }

            if (colorGradient.shareColorMode)
            {
                var xCurve = colorGradient.axes[0].xCurve;
                var yCurve = colorGradient.axes[0].yCurve;
                for (int i = 1; i < colorGradient.axes.Length; i++)
                {
                    colorGradient.axes[i].xCurve = xCurve;
                    colorGradient.axes[i].yCurve = yCurve;
                }
            }
        }
        public void OnEnable()
        {
            InitializeEditor();
        }

        public override void OnInspectorGUI()
        {
            var colorGradient = target as ColorGradient;

            var tmp_colorMode = EditorGUILayout.Popup("color", colorGradient.colorMode, ColorMode.names);
            var tmp_axesCount = EditorGUILayout.Toggle("y", colorGradient.axesCount == 2) ? 2 : 1;
            var sizeUpdated = false;
            var updated = false;

            if (colorGradient.colorMode != tmp_colorMode || colorGradient.axesCount != tmp_axesCount)
            {
                if (colorGradient.axesCount != tmp_axesCount)
                {
                    colorGradient.textureSize = new Vector2Int(
                        colorGradient.textureSize.x,
                        tmp_axesCount == 2 ? colorGradient.textureSize.x : 1
                    );
                }

                colorGradient.colorMode = tmp_colorMode;
                colorGradient.axesCount = tmp_axesCount;
                colorGradient.SetAxes();
                InitializeEditor();

                sizeUpdated = true;
            }

            var modeName = ColorMode.names[colorGradient.colorMode];

            if (colorGradient.axesCount == 2)
            {
                var tmpSize = EditorGUILayout.Vector2IntField("size", colorGradient.textureSize);
                sizeUpdated |= colorGradient.textureSize.x != tmpSize.x || colorGradient.textureSize.y != tmpSize.y;

                colorGradient.textureSize.x = Math.Max(tmpSize.x, 1);
                colorGradient.textureSize.y = Math.Max(tmpSize.y, 1);

            }
            else
            {
                var tmpSize = EditorGUILayout.IntField("size", colorGradient.textureSize.x);
                sizeUpdated |= colorGradient.textureSize.x != tmpSize || colorGradient.textureSize.y != 1;

                colorGradient.textureSize.x = Math.Max(tmpSize, 1);
                colorGradient.textureSize.y = 1;
            }

            if (modeName.Equals("RGB") || modeName.Equals("HSV"))
            {
                var rect = EditorGUILayout.GetControlRect();

                if (GUI.Button(rect, "import UnityGradient"))
                {
                    PopupWindow.Show(rect, new UnityGradientToCurveWindow(colorGradient));
                }
            }

            #region ShareColorMode

            var tmp_shareColorMode = EditorGUILayout.Toggle("share color mode", colorGradient.shareColorMode);
            if (tmp_shareColorMode != colorGradient.shareColorMode)
            {
                colorGradient.shareColorMode = tmp_shareColorMode;
                updated |= true;
                if (colorGradient.shareColorMode)
                {
                    var xCurve = colorGradient.axes[0].xCurve;
                    var yCurve = colorGradient.axes[0].yCurve;
                    for (int i = 1; i < colorGradient.axes.Length; i++)
                    {
                        colorGradient.axes[i].xCurve = xCurve;
                        colorGradient.axes[i].yCurve = yCurve;
                    }
                }
                else
                {
                    for (int i = 0; i < colorGradient.axes.Length; i++)
                    {
                        colorGradient.axes[i].xCurve = CloneAnimationCurve(colorGradient.axes[i].xCurve);
                        if (colorGradient.axesCount == 2)
                            colorGradient.axes[i].yCurve = CloneAnimationCurve(colorGradient.axes[i].yCurve);
                    }
                }
            }

            #endregion

            for (int i = 0; i < colorGradient.Mode.size; i++)
                updated |= colorAxisEditors[i].Editor(colorGradient, colorGradient.axes[i]);

            updated |= sizeUpdated;

            if (sizeUpdated)
            {
                colorGradient.texture.Resize(colorGradient.textureSize.x, colorGradient.textureSize.y, TextureFormat.ARGB32, false);

                if (colorGradient.sprite != null)
                {
                    var so = new SerializedObject(colorGradient.sprite);
                    var rect = so.FindProperty("m_Rect");
                    rect.rectValue = new Rect(0, 0, colorGradient.textureSize.x, colorGradient.textureSize.y);
                    so.ApplyModifiedProperties();
                }
            }

            if (colorGradient.sprite == null)
            {
                if (GUILayout.Toggle(false, "Sprite"))
                {
                    var sprite = Sprite.Create(
                        colorGradient.texture,
                        new Rect(0, 0, colorGradient.textureSize.x, colorGradient.textureSize.y),
                        new Vector2(0.5f, 0.5f)
                    );
                    sprite.name = "sprite";
                    colorGradient.sprite = sprite;

                    AssetDatabase.AddObjectToAsset(sprite, colorGradient);
                    AssetDatabase.SaveAssets();
                }
            }
            else
            {
                if (!GUILayout.Toggle(true, "Sprite"))
                {
                    AssetDatabase.RemoveObjectFromAsset(colorGradient.sprite);
                    colorGradient.sprite = null;

                    AssetDatabase.SaveAssets();
                }
            }

            if (updated)
            {
                colorGradient.SetTexturePixel(colorGradient.texture);
                colorGradient.texture.Apply();

                EditorUtility.SetDirty(colorGradient);
            }

            if (GUILayout.Button("Bake Texture")) Bake(colorGradient);

        }
        public override bool HasPreviewGUI() => true;
        public override void OnPreviewGUI(Rect r, GUIStyle background)
        {
            var colorGradient = target as ColorGradient;
            if (colorGradient == null || colorGradient.texture == null) return;
            GUI.DrawTexture(r, colorGradient.texture);
        }

        private AnimationCurve CloneAnimationCurve(AnimationCurve curve)
         => new AnimationCurve((Keyframe[])curve.keys.Clone());

        private void Bake(ColorGradient colorGradient)
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
