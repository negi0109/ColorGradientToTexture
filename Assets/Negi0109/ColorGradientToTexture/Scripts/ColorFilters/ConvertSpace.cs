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
        public override double Evaluate(double v)
         => type switch
         {
             ConvertType.GammaToLinear => (double)Mathf.GammaToLinearSpace((float)v),
             ConvertType.LinearToGamma => (double)Mathf.LinearToGammaSpace((float)v),
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
