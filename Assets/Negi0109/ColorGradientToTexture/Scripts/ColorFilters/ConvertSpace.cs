using UnityEditor;
using UnityEngine;

namespace Negi0109.ColorGradientToTexture.Filters
{
    [System.Serializable]
    public class ConvertSpace : ColorFilter
    {
        public enum ConvertType
        {
            GammaToLinear,
            LinearToGamma
        }

        public ConvertType type;
        public override float Evaluate(float v)
        {
            switch (type)
            {
                case ConvertType.GammaToLinear: return Mathf.GammaToLinearSpace(v);
                case ConvertType.LinearToGamma: return Mathf.LinearToGammaSpace(v);
                default: return v;
            }
        }

        public override bool Editor()
        {
            return Watch(
                () => (int)type,
                () => { type = (ConvertType)EditorGUILayout.EnumPopup(type); }
            );
        }
    }
}
