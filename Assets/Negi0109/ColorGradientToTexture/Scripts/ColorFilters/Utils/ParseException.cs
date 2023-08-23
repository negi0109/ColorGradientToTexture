using System;

namespace Negi0109.ColorGradientToTexture.Filters.Formulas
{
    public class ParseException : Exception
    {
        public readonly int begin;
        public readonly int end;

        public ParseException(string message, int location) : base(message)
        {
            begin = location;
            end = location;
        }
        public ParseException(string message, int location, int end) : base(message)
        {
            begin = location;
            this.end = end;
        }
    }
}
