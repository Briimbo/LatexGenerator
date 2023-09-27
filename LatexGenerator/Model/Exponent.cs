using System;

namespace LatexGenerator.Model
{
    internal class Exponent : ITerm
    {
        private ITerm _baseTerm;
        private ITerm _power;
        public ITerm Parent { get; set; } = null;

        public ITerm BaseTerm
        {
            get => _baseTerm;
            private set
            {
                _baseTerm = value;
                _baseTerm.Parent = this;
            }
        }

        public ITerm Power
        {
            get => _power;
            private set
            {
                _power = value;
                _power.Parent = this;
            }
        }

        public Exponent(ITerm baseTerm, ITerm power)
        {
            BaseTerm = baseTerm;
            Power = power;
        }

        public ITerm Replace(ITerm oldTerm, ITerm newTerm)
        {
            if (newTerm is EmptyTerm)
                return DissolveExponentiation(oldTerm);
            if (oldTerm == Power)
                Power = newTerm;
            else if (oldTerm == BaseTerm)
                BaseTerm = newTerm;
            else
                throw new InvalidOperationException("Argument oldTerm does not match an existing Term in this component");

            return newTerm;
        }

        private ITerm DissolveExponentiation(ITerm obsoleteTerm)
        {
            if (obsoleteTerm == Power)
                return Parent.Replace(this, BaseTerm);
            if (obsoleteTerm == BaseTerm)
                return Parent.Replace(this, Power);
            throw new InvalidOperationException("Argument obsoleteTerm does not match an existing Term in this component");
        }

        public string ToLatexCode()
        {
            if (BaseTerm is Subscript)
                return $"{BaseTerm.ToLatexCode()}^{{{Power.ToLatexCode()}}}";
            else
                return $"{{{BaseTerm.ToLatexCode()}}}^{{{Power.ToLatexCode()}}}";
        }

        public override string ToString() => ToLatexCode();
    }
}
