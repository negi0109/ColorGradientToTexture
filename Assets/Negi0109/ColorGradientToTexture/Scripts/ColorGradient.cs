using System;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif
namespace Negi0109.ColorGradientToTexture
{
    [Serializable]
    public class ColorGradient : ScriptableObject
    {
        public int colorMode;

        public ColorAxis[] axes;
        public ColorMode Mode { get => ColorMode.modes[colorMode]; }
        public int axesCount = 2;
        public Vector2Int textureSize;
        public Texture2D texture;

        public ColorGradient(int colorMode, int axesCount = 2)
        {
            this.colorMode = colorMode;
            this.axesCount = axesCount;

            SetAxes();
        }

        public void SetAxes()
        {
            this.axes = new ColorAxis[Mode.size];

            for (int i = 0; i < Mode.size; i++)
                this.axes[i] = new ColorAxis(axesCount);
        }

        public Color GetColor(float x, float y)
        {
            float[] values = new float[Mode.size];

            for (int i = 0; i < values.Length; i++)
                values[i] = axes[i].Evaluate(x, y);

            return Mode.func(values);
        }

        public void SetTexturePixel(Texture2D tex)
        {
            for (int x = 0; x < tex.width; x++)
                for (int y = 0; y < tex.height; y++)
                    tex.SetPixel(x, y, Mode.func(
                        axes.Select((v) => v.Evaluate(
                            (float)x / tex.width,
                            (float)y / tex.height
                        )).ToArray()
                    ), 0);
        }

#if UNITY_EDITOR
        [MenuItem("Assets/Create/ColorGradient")]
        public static void Create()
        {
            var parent = CreateInstance<ColorGradient>();
            parent.textureSize = new Vector2Int(256, 256);
            parent.axesCount = 2;

            parent.SetAxes();

            AssetDatabase.CreateAsset(parent, "Assets/ColorGradient.asset");

            var texture = new Texture2D(parent.textureSize.x, parent.textureSize.y, TextureFormat.ARGB32, false);
            parent.texture = texture;

            AssetDatabase.AddObjectToAsset(texture, parent);
            AssetDatabase.SaveAssets();

            EditorGUIUtility.PingObject(parent);
        }
#endif
    }
}
