namespace Negi0109.ColorGradientToTexture.Filters
{
    [System.Serializable]
    public class OneMinus : ColorFilter
    {
        public override double Evaluate(double v) => 1 - v;

        public override bool Editor() => false;
    }
}
