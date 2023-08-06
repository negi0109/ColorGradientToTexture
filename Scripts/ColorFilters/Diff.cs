using UnityEditor;
using UnityEngine;
using System.Linq;

namespace Negi0109.ColorGradientToTexture.Filters
{
    [System.Serializable]
    public class Diff : ColorFilter
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
            One,
        }

        public Direction direction;
        public Division division;

        public override void EvaluateAll(ref float[,] array)
        {
            Utils.ArraySegment2DBase<float> _segment = new Utils.ArraySegment2D<float>(array);

            if (direction == Direction.Y01 || direction == Direction.Y10) _segment = _segment.Dimension(1);
            if (direction == Direction.X10 || direction == Direction.Y10) _segment = _segment.Backward(true);

            foreach (var line in _segment.Backward().GetLines())
            {
                line.SetValues((v, i) => v - (i + 1 < line.Count ? line[i + 1] : 0));

                if (division == Division.Max)
                {
                    var max = line.Max();
                    line.SetValues(v => v / max);
                }
                else if (division == Division.One) { }
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
                            new string[]{ "Max", "Width", "One" },
                            new string[]{ "Max", "Height", "One" },
                            new string[]{ "Max", "Width", "One" },
                            new string[]{ "Max", "Height", "One" }
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
