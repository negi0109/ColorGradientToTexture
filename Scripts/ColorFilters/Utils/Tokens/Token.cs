namespace Negi0109.ColorGradientToTexture.Filters.Formulas
{
    public class Token
    {
        public readonly int begin;
        public readonly int end = -1;

        public Token(int begin, int end) { this.begin = begin; this.end = end; }
    }
}
