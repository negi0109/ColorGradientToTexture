using UnityEditor;
using UnityEngine;

namespace Negi0109.ColorGradientToTexture.Filters
{
    [System.Serializable]
    public class Cumulate : ColorFilter
    {
        public enum Direction
        {
            [InspectorName("X 0 -> 1")] X01,
            [InspectorName("Y 0 -> 1")] Y01,
            [InspectorName("X 0 <- 1")] X10,
            [InspectorName("Y 0 <- 1")] Y10
        }

        public enum Division
        {
            Max,
            Volume,
            One,
        }

        public Direction direction;
        public Division division;

        public override void EvaluateAll(ref float[,] v)
        {
            Utils.ArraySeeker<float> _seeker;

            switch (direction)
            {
                case Direction.X01:
                    _seeker = new Utils.ArraySeeker<float>(v, 0, false);
                    break;
                case Direction.Y01:
                    _seeker = new Utils.ArraySeeker<float>(v, 1, false);
                    break;
                case Direction.X10:
                    _seeker = new Utils.ArraySeeker<float>(v, 0, true);
                    break;
                case Direction.Y10:
                    _seeker = new Utils.ArraySeeker<float>(v, 1, true);
                    break;
                default:
                    _seeker = new Utils.ArraySeeker<float>(v, 0, false);
                    break;
            }

            foreach (var line in _seeker)
            {
                var length = line.GetLength();
                for (int i = 1; i < length; i++)
                    line[i] = line[i - 1] + line[i];

                var a = 1f;
                if (division == Division.Max) a = line[length - 1];
                else if (division == Division.Volume) a = length;
                else if (division == Division.One) a = 1;

                for (int i = 0; i < length; i++)
                    line[i] = line[i] / a;
            }
        }

        public override bool Editor()
        {
            EditorGUILayout.BeginVertical();
            EditorGUIUtility.labelWidth = 60f;

            var updated = Watch(
                () => (int)direction,
                () => { direction = (Direction)EditorGUILayout.EnumPopup(direction); }
            );
            updated |= Watch(
                () => (int)division,
                () =>
                {
                    division = (Division)EditorGUILayout.Popup(
                        (int)division,
                        new string[][] {
                            new string[]{ "Last (X1)", "Width", "One" },
                            new string[]{ "Last (Y1)", "Height", "One" },
                            new string[]{ "Last (X0)", "Width", "One" },
                            new string[]{ "Last (Y0)", "Height", "One" }
                        }[(int)direction]
                    );
                }
            );

            EditorGUIUtility.labelWidth = 0;
            EditorGUILayout.EndVertical();

            return updated;
        }
    }
}
