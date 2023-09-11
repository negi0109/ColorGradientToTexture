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

        public override void EvaluateAll(ref double[,] array)
        {
            Utils.ArraySegment2DBase<double> _segment = new Utils.ArraySegment2D<double>(array);

            if (direction == Direction.Y01 || direction == Direction.Y10) _segment = _segment.Dimension(1);
            if (direction == Direction.X10 || direction == Direction.Y10) _segment = _segment.Backward(true);

            foreach (var line in _segment.GetLines())
            {
                var last = .0;
                line.SetValues(v => last = last + v);

                if (division == Division.Max) line.SetValues(v => v / last);
                else if (division == Division.Volume) line.SetValues(v => v / line.Count);
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
