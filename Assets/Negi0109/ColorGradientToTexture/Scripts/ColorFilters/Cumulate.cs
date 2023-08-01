using UnityEditor;
using UnityEngine;

namespace Negi0109.ColorGradientToTexture.Filters {

    [System.Serializable]
    public class Cumulate : ColorFilter {
        public enum Direction {
            [InspectorName("X 0 -> 1")] X01,
            [InspectorName("Y 0 -> 1")] Y01,
            [InspectorName("X 0 <- 1")] X10,
            [InspectorName("Y 0 <- 1")] Y10
        }

        public enum Division {
            Max,
            Volume,
            One,
        }

        public Direction direction;
        public Division division;

        public override void Evaluate(ref float[,] v)
        {
            var width = v.GetLength(0);
            var height = v.GetLength(1);

            switch (direction)
            {
                case Direction.X01:
                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 1; x < width; x++)
                        {
                            v[x, y] = v[x - 1, y] + v[x, y];
                        }

                        var a = 1f;
                        if (division == Division.Max) a = v[width - 1, y];
                        else if (division == Division.Volume) a = width;
                        else if (division == Division.One) a = 1;

                        if (a != 1f)
                            for (int x = 0; x < width; x++)
                                v[x, y] = v[x, y] / a;
                    }
                    break;
                case Direction.Y01:
                    for (int x = 0; x < width; x++)
                    {
                        for (int y = 1; y < height; y++)
                        {
                            v[x, y] = v[x, y - 1] + v[x, y];
                        }

                        var a = 1f;
                        if (division == Division.Max) a = v[x, height - 1];
                        else if (division == Division.Volume) a = height;
                        else if (division == Division.One) a = 1;

                        if (a != 1f)
                            for (int y = 0; y < height; y++)
                                v[x, y] = v[x, y] / a;
                    }
                    break;
                case Direction.X10:
                    for (int y = 0; y < height; y++)
                    {
                        for (int x = width - 2; x >= 0; x--)
                        {
                            v[x, y] = v[x + 1, y] + v[x, y];
                        }

                        var a = 1f;
                        if (division == Division.Max) a = v[0, y];
                        else if (division == Division.Volume) a = width;
                        else if (division == Division.One) a = 1;

                        if (a != 1f)
                            for (int x = 0; x < width; x++)
                                v[x, y] = v[x, y] / a;
                    }
                    break;
                case Direction.Y10:
                    for (int x = 0; x < width; x++)
                    {
                        for (int y = height - 2; y >= 0; y--)
                        {
                            v[x, y] = v[x, y + 1] + v[x, y];
                        }

                        var a = 1f;
                        if (division == Division.Max) a = v[x, 0];
                        else if (division == Division.Volume) a = height;
                        else if (division == Division.One) a = 1;

                        if (a != 1f)
                            for (int y = 0; y < height; y++)
                                v[x, y] = v[x, y] / a;
                    }
                    break;
            }
        }

        public override bool Editor() {
            EditorGUILayout.BeginVertical();
            EditorGUIUtility.labelWidth = 60f;

            var updated = Watch(
                () => (int) direction,
                () => { direction = (Direction) EditorGUILayout.EnumPopup(direction); }
            );
            updated |= Watch(
                () => (int) division,
                () => {
                    division = (Division) EditorGUILayout.Popup(
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
