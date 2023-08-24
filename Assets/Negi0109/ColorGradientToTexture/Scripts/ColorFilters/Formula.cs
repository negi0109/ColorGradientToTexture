using UnityEditor;
using UnityEngine;
using System;

namespace Negi0109.ColorGradientToTexture.Filters
{
    [Serializable]
    public class Formula : ColorFilter
    {
        public string formula = "";
        private string compiledFormula;
        private Func<float, float, float, float> compiled = (v, x, y) => v;
        private Exception _exception = null;

        public override void EvaluateAll(ref float[,] array)
        {
            Compile();

            for (int x = 0; x < array.GetLength(0); x++)
            {
                for (int y = 0; y < array.GetLength(1); y++)
                {
                    array[x, y] = compiled(
                        array[x, y],
                        GetNormalValue(x, array.GetLength(0)),
                        GetNormalValue(y, array.GetLength(1))
                    );
                }
            }
        }
        private float GetNormalValue(int v, int size)
            => size == 1 ? 0.5f : (float)v / (size - 1);

        public override bool Editor()
        {
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("=", new GUILayoutOption[] { GUILayout.Width(12) });

            var updated = Watch(
                () => formula,
                () => { formula = EditorGUILayout.TextField(formula); }
            );
            EditorGUILayout.EndHorizontal();

            Compile();

            if (_exception != null)
            {
                var style = new GUIStyle(EditorStyles.label);

                var labelStyle = new GUIStyle(EditorStyles.label)
                {
                    richText = true,
                    wordWrap = true
                };
                if (_exception is Formulas.ParseException parseException)
                {
                    EditorGUILayout.LabelField(DisplayParseException(formula, parseException), labelStyle);
                }
                else
                {
                    EditorGUILayout.LabelField($"<color=red>*** Unconfirmed error ***</color>\n{_exception.Message}", labelStyle);
                }
            }
            EditorGUILayout.EndVertical();

            return updated;
        }

        public void Compile()
        {
            if (formula == compiledFormula) return;

            try
            {
                compiled = Formulas.FormulaCompiler.Compile(formula);
                _exception = null;
            }
            catch (Exception e)
            {
                _exception = e;
            }
            finally
            {
                compiledFormula = formula;
            }
        }

        public string DisplayParseException(string errorFormula, Formulas.ParseException exception)
        {
            if (exception.begin < 0) return exception.Message;

            var begin = exception.begin;
            var end = exception.end + 1;
            var length = end - begin;

            return $"{errorFormula.Substring(0, begin)} <b><i><color=red>{errorFormula.Substring(begin, length)}</color></i></b> {errorFormula.Substring(end)}\n{exception.Message}";
        }
    }
}
