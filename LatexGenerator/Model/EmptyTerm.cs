using System;

namespace LatexGenerator.Model
{
    internal class EmptyTerm : ITerm
    {
        public ITerm Parent { get; set; }

        public ITerm Replace(ITerm oldTerm, ITerm newTerm)
        {
            throw new InvalidOperationException("Method Replace of SimpleTerm cannot be called. Please create a new object instead");
        }

        public string ToLatexCode()
        {
            return "{}";
        }

        public override string ToString() => ToLatexCode();
    }
}
