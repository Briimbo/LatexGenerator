using System;

namespace LatexGenerator.Model
{
    internal class Subscript : ITerm
    {
        private ITerm _baseTerm;
        private ITerm _sub;

        public ITerm Parent { get; set; }
        public ITerm BaseTerm
        {
            get => _baseTerm;
            private set
            {
                _baseTerm = value;
                _baseTerm.Parent = this;
            }
        }

        public ITerm Sub
        {
            get => _sub;
            private set
            {
                _sub = value;
                _sub.Parent = this;
            }
        }


        public Subscript(ITerm baseTerm, ITerm sub)
        {
            BaseTerm = baseTerm;
            Sub = sub;
        }

        public ITerm Replace(ITerm oldTerm, ITerm newTerm)
        {
            if (newTerm is EmptyTerm)
                return DissolveSubscript(oldTerm);
            if (oldTerm == Sub)
                Sub = newTerm;
            else if (oldTerm == BaseTerm)
                BaseTerm = newTerm;
            else
                throw new InvalidOperationException("Argument oldTerm does not match an existing Term in this component");

            return newTerm;
        }

        private ITerm DissolveSubscript(ITerm obsoleteTerm)
        {
            if (obsoleteTerm == Sub)
                return Parent.Replace(this, BaseTerm);
            if (obsoleteTerm == BaseTerm)
                return Parent.Replace(this, Sub);
            throw new InvalidOperationException("Argument obsoleteTerm does not match an existing Term in this component");
        }

        public string ToLatexCode()
        {
            if (BaseTerm is Exponent)
                return $"{BaseTerm.ToLatexCode()}_{{{Sub.ToLatexCode()}}}";
            else
                return $"{{{BaseTerm.ToLatexCode()}}}_{{{Sub.ToLatexCode()}}}";
        }

        public override string ToString() => ToLatexCode();
    }
}
