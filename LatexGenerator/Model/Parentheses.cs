using System;

namespace LatexGenerator.Model
{
    internal class Parentheses : ITerm
    {
        private ITerm _innerTerm;
        public ITerm Parent { get; set; } = null;

        public ITerm InnerTerm
        {
            get => _innerTerm;
            private set
            {
                _innerTerm = value;
                _innerTerm.Parent = this;
            }
        }

        public Parentheses(ITerm innerTerm)
        {
            InnerTerm = innerTerm;
        }

        public ITerm Replace(ITerm oldTerm, ITerm newTerm)
        {
            if (oldTerm == InnerTerm)
                InnerTerm = newTerm;
            else
                throw new InvalidOperationException("Argument oldTerm does not match an existing Term in this component");

            return newTerm;
        }

        public string ToLatexCode()
        {
            return $"({InnerTerm.ToLatexCode()})";
        }
        public override string ToString() => ToLatexCode();
    }
}
