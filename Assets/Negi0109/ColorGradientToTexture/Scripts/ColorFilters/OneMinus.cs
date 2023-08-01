using UnityEditor;
using UnityEngine;

namespace Negi0109.ColorGradientToTexture.Filters {

    [System.Serializable]
    public class OneMinus : ColorFilter {
        public override float Evaluate(float v) => 1 - v;

        public override bool Editor() => false;
    }

}
