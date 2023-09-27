using System;

namespace LatexGenerator.Model
{
    internal class Fraction : ITerm
    {
        private ITerm _numerator;
        private ITerm _denominator;
        public ITerm Parent { get; set; } = null;

        public ITerm Numerator
        {
            get => _numerator;
            private set
            {
                _numerator = value;
                _numerator.Parent = this;
            }
        }

        public ITerm Denominator
        {
            get => _denominator;
            private set
            {
                _denominator = value;
                _denominator.Parent = this;
            }
        }

        public Fraction(ITerm numerator, ITerm denominator)
        {
            Numerator = numerator;
            Numerator.Parent = this;
            Denominator = denominator;
            Denominator.Parent = this;
        }

        public ITerm Replace(ITerm oldTerm, ITerm newTerm)
        {
            if (oldTerm is EmptyTerm && newTerm is EmptyTerm)
                return DissolveFraction(oldTerm);
            if (oldTerm == Numerator)
                Numerator = newTerm;
            else if (oldTerm == Denominator)
                Denominator = newTerm;
            else
                throw new InvalidOperationException("Argument oldTerm does not match an existing Term in this component");
            return newTerm;
        }

        private ITerm DissolveFraction(ITerm obsoleteTerm)
        {
            if (obsoleteTerm == Numerator)
                return Parent.Replace(this, Denominator);
            if (obsoleteTerm == Denominator)
                return Parent.Replace(this, Numerator);

            throw new InvalidOperationException("Argument obsoleteTerm does not match an existing Term in this component");
        }

        public string ToLatexCode()
        {
            return $"\\frac{{{Numerator.ToLatexCode()}}}{{{Denominator.ToLatexCode()}}}";
        }

        public override string ToString() => ToLatexCode();
    }
}
