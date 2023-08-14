using UnityEditor;
using UnityEngine;
using System;

namespace Negi0109.ColorGradientToTexture.Filters
{
    [System.Serializable]
    public class Formula : ColorFilter
    {
        public string formula = "";
        private string compiledFormula;
        private Func<float, float, float, float> compiled;

        public override void EvaluateAll(ref float[,] array)
        {
            Compile();

            for (int x = 0; x < array.GetLength(0); x++)
            {
                for (int y = 0; y < array.GetLength(1); y++)
                {
                    array[x, y] = compiled(array[x, y], x, y);
                }
            }
        }

        public override bool Editor()
        {
            return Watch(
                () => formula,
                () => { formula = EditorGUILayout.TextField("formula", formula); }
            );
        }

        public void Compile()
        {
            if (formula == compiledFormula) return;

            compiled = FormulaCompiler.Compile(formula);
            compiledFormula = formula;
        }
    }
}
