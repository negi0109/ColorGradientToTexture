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
                colorGradient.axes[0].xCurve = curves.v0;
                colorGradient.axes[1].xCurve = curves.v1;
                colorGradient.axes[2].xCurve = curves.v2;

                editorWindow.Close();
            }

            position.y += LineHeight;

            if (colorGradient.axesCount == 2)
            {
                if (GUI.Button(IndentPosition(position), "y"))
                {
                    var curves = GetCurves();

                    colorGradient.shareColorMode = false;
                    colorGradient.axes[0].yCurve = curves.v0;
                    colorGradient.axes[1].yCurve = curves.v1;
                    colorGradient.axes[2].yCurve = curves.v2;

                    editorWindow.Close();
                }
                position.y += LineHeight;

                if (GUI.Button(IndentPosition(position), "x, y"))
                {
                    var curves = GetCurves();

                    colorGradient.shareColorMode = false;
                    colorGradient.axes[0].xCurve = curves.v0;
                    colorGradient.axes[1].xCurve = curves.v1;
                    colorGradient.axes[2].xCurve = curves.v2;

                    colorGradient.axes[0].yCurve = new AnimationCurve((Keyframe[])curves.v0.keys.Clone());
                    colorGradient.axes[1].yCurve = new AnimationCurve((Keyframe[])curves.v1.keys.Clone());
                    colorGradient.axes[2].yCurve = new AnimationCurve((Keyframe[])curves.v2.keys.Clone());

                    editorWindow.Close();
                }
                position.y += LineHeight;
            }
        }
        private (AnimationCurve v0, AnimationCurve v1, AnimationCurve v2) GetCurves()
        {
            (AnimationCurve v0, AnimationCurve v1, AnimationCurve v2) curves = (
                new AnimationCurve(),
                new AnimationCurve(),
                new AnimationCurve()
            );

            var modeName = ColorMode.names[colorGradient.colorMode];

            for (int i = 0; i < gradient.colorKeys.Length; i++)
            {
                GradientColorKey key = gradient.colorKeys[i];
                float v0 = 0, v1 = 0, v2 = 0;

                if (modeName.Equals("RGB"))
                {
                    v0 = key.color.r;
                    v1 = key.color.g;
                    v2 = key.color.b;
                }
                else if (modeName.Equals("HSV"))
                {
                    Color.RGBToHSV(key.color, out v0, out v1, out v2);
                }

                curves.v0.AddKey(new Keyframe(key.time, v0, 0, 0));
                curves.v1.AddKey(new Keyframe(key.time, v1, 0, 0));
                curves.v2.AddKey(new Keyframe(key.time, v2, 0, 0));
            }

            for (int i = 0; i < gradient.colorKeys.Length; i++)
            {
                AnimationUtility.SetKeyLeftTangentMode(curves.v0, i, mode);
                AnimationUtility.SetKeyLeftTangentMode(curves.v1, i, mode);
                AnimationUtility.SetKeyLeftTangentMode(curves.v2, i, mode);
                AnimationUtility.SetKeyRightTangentMode(curves.v0, i, mode);
                AnimationUtility.SetKeyRightTangentMode(curves.v1, i, mode);
                AnimationUtility.SetKeyRightTangentMode(curves.v2, i, mode);
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
