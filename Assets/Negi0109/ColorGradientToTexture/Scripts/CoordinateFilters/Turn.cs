using UnityEngine;

namespace Negi0109.ColorGradientToTexture.CoordinateFilters
{
    [System.Serializable]
    public class Turn : CoordinateFilter
    {
        public override Vector2 Evaluate(Vector2 v) => new Vector2(v.y, v.x);
    }
}
