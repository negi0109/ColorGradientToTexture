using UnityEditor;
using UnityEngine;
using System;

namespace Negi0109.ColorGradientToTexture.Filters
{
    [System.Serializable]
    public class Scale : ColorFilter
    {
        public double value = 1;

        public override double Evaluate(double v) => v * value;

        public override bool Editor()
        {
            return Watch(
                () => value,
                () => { value = Math.Max(EditorGUILayout.DoubleField("weight", value), 0); }
            );
        }
    }
}
