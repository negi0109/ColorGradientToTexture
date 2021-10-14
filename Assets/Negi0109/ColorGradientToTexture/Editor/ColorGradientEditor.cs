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

            for (int i = 0; i < colorGradient.Mode.size; i++)
                updated |= colorAxisEditors[i].Editor(colorGradient.axes[i]);

            updated |= sizeUpdated;

            if (sizeUpdated) colorGradient.texture.Resize(colorGradient.textureSize.x, colorGradient.textureSize.y, TextureFormat.ARGB32, false);

            if (updated)
            {
                colorGradient.SetTexturePixel(colorGradient.texture);
                colorGradient.texture.Apply();

                EditorUtility.SetDirty(colorGradient);
            }
        }
        public override bool HasPreviewGUI() => true;
        public override void OnPreviewGUI(Rect r, GUIStyle background)
        {
            var colorGradient = target as ColorGradient;
            if (colorGradient == null || colorGradient.texture == null) return;
            GUI.DrawTexture(r, colorGradient.texture);
        }
    }
}
