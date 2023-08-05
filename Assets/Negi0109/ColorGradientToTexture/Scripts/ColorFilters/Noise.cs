using System;
using UnityEditor;
using UnityEngine;

namespace Negi0109.ColorGradientToTexture.Filters
{
    [System.Serializable]
    public class Noise : ColorFilter
    {

        public float weight = 0.5f;
        public int seed = new System.Random().Next();

        public override void EvaluateAll(ref float[,] v)
        {
            var all = new Utils.ArraySeeker<float>(v).GetAll();
            var r = new System.Random(seed);

            for (int i = 0; i < all.GetLength(); i++)
            {
                all[i] += weight * r.Next() / int.MaxValue;
            }
        }

        public override bool Editor()
        {
            EditorGUILayout.BeginVertical();
            EditorGUIUtility.labelWidth = 60f;

            var updated = Watch(
                () => weight,
                () => { weight = EditorGUILayout.Slider("weight", weight, 0f, 1f); }
            );
            updated |= Watch(
                () => (int)seed,
                () => { seed = EditorGUILayout.IntField("seed", seed); }
            );

            EditorGUIUtility.labelWidth = 0;
            EditorGUILayout.EndVertical();

            return updated;
        }
    }
}
