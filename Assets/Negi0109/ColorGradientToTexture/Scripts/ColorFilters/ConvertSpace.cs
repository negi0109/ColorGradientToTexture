using System.Runtime.InteropServices;
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
         => type switch
         {
             ConvertType.GammaToLinear => Mathf.GammaToLinearSpace(v),
             ConvertType.LinearToGamma => Mathf.LinearToGammaSpace(v),
             _ => v
         };

        public override bool Editor()
        {
            return Watch(
                () => (int)type,
                () => { type = (ConvertType)EditorGUILayout.EnumPopup(type); }
            );
        }
    }
}
