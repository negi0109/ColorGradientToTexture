using UnityEditor;
using UnityEngine;


namespace Negi0109.ColorGradientToTexture
{
    public class UnityGradientToCurveWindow : PopupWindowContent
    {
        public ColorGradient colorGradient;
        public Gradient gradient;
        public AnimationUtility.TangentMode mode;

        public UnityGradientToCurveWindow(ColorGradient colorGradient)
        {
            this.colorGradient = colorGradient;
            this.gradient = new Gradient();
            this.mode = AnimationUtility.TangentMode.Linear;
        }

        public override void OnGUI(Rect rect)
        {
            var position = MarginRect(rect, 5);
            position.height = EditorGUIUtility.singleLineHeight;
            position.y += EditorGUIUtility.standardVerticalSpacing;

            EditorGUI.LabelField(position, "Unity Gradient");
            position.y += LineHeight;
            EditorGUI.GradientField(IndentPosition(position), gradient);
            position.y += LineHeight;

            EditorGUI.LabelField(position, "Tangent Mode");
            position.y += LineHeight;
            mode = (AnimationUtility.TangentMode)EditorGUI.EnumPopup(IndentPosition(position), mode);
            position.y += LineHeight;

            EditorGUI.LabelField(position, "import");
            position.y += LineHeight;

            if (GUI.Button(IndentPosition(position), "x"))
            {
                var curves = GetCurves();

                colorGradient.shareColorMode = false;
                colorGradient.axes[0].xCurve = curves.r;
                colorGradient.axes[1].xCurve = curves.g;
                colorGradient.axes[2].xCurve = curves.b;

                editorWindow.Close();
            }

            position.y += LineHeight;

            if (colorGradient.axesCount == 2)
            {
                if (GUI.Button(IndentPosition(position), "y"))
                {
                    var curves = GetCurves();

                    colorGradient.shareColorMode = false;
                    colorGradient.axes[0].yCurve = curves.r;
                    colorGradient.axes[1].yCurve = curves.g;
                    colorGradient.axes[2].yCurve = curves.b;

                    editorWindow.Close();
                }
                position.y += LineHeight;

                if (GUI.Button(IndentPosition(position), "x, y"))
                {
                    var curves = GetCurves();

                    colorGradient.shareColorMode = false;
                    colorGradient.axes[0].xCurve = curves.r;
                    colorGradient.axes[1].xCurve = curves.g;
                    colorGradient.axes[2].xCurve = curves.b;

                    colorGradient.axes[0].yCurve = new AnimationCurve((Keyframe[])curves.r.keys.Clone());
                    colorGradient.axes[1].yCurve = new AnimationCurve((Keyframe[])curves.g.keys.Clone());
                    colorGradient.axes[2].yCurve = new AnimationCurve((Keyframe[])curves.b.keys.Clone());

                    editorWindow.Close();
                }
                position.y += LineHeight;
            }
        }
        private (AnimationCurve r, AnimationCurve g, AnimationCurve b) GetCurves()
        {
            (AnimationCurve r, AnimationCurve g, AnimationCurve b) curves = (
                new AnimationCurve(),
                new AnimationCurve(),
                new AnimationCurve()
            );

            for (int i = 0; i < gradient.colorKeys.Length; i++)
            {
                GradientColorKey key = gradient.colorKeys[i];
                curves.r.AddKey(new Keyframe(key.time, key.color.r, 0, 0));
                curves.g.AddKey(new Keyframe(key.time, key.color.g, 0, 0));
                curves.b.AddKey(new Keyframe(key.time, key.color.b, 0, 0));
            }

            for (int i = 0; i < gradient.colorKeys.Length; i++)
            {
                AnimationUtility.SetKeyLeftTangentMode(curves.r, i, mode);
                AnimationUtility.SetKeyLeftTangentMode(curves.g, i, mode);
                AnimationUtility.SetKeyLeftTangentMode(curves.b, i, mode);
                AnimationUtility.SetKeyRightTangentMode(curves.r, i, mode);
                AnimationUtility.SetKeyRightTangentMode(curves.g, i, mode);
                AnimationUtility.SetKeyRightTangentMode(curves.b, i, mode);
            }

            return curves;
        }

        private float LineHeight { get => EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing; }

        private Rect IndentPosition(Rect rect, float margin = 12)
        {
            var position = new Rect(rect);
            position.x += margin;
            position.width -= margin;

            return position;
        }

        private Rect MarginRect(Rect rect, float margin)
        {
            var rect2 = new Rect(rect);
            rect2.x += margin;
            rect2.y += margin;
            rect2.width -= margin * 2;

            return rect2;
        }
    }
}
