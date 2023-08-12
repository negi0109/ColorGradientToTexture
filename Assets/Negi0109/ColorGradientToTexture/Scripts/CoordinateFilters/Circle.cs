using UnityEngine;

namespace Negi0109.ColorGradientToTexture.CoordinateFilters
{
    [System.Serializable]
    public class Circle : CoordinateFilter
    {
        public override Vector2 Evaluate(Vector2 v)
        {
            var center = v - new Vector2(.5f, .5f);

            return new Vector2(
                Mathf.Min(center.magnitude, 1f),
                Mathf.Atan2(center.y, center.x) * Mathf.Rad2Deg / 360f + .5f
            );
        }
    }
}
