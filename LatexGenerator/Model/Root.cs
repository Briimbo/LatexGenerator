using System;
using System.Collections.Generic;
using System.Text;

namespace LatexGenerator.Model
{
    internal class Root : ITerm
    {
        private ITerm _radicand;
        private ITerm _index;
        public ITerm Parent { get; set; } = null;

        public ITerm Radicand
        {
            get => _radicand;
            private set
            {
                _radicand = value;
                _radicand.Parent = this;
            }
        }

        public ITerm Index
        {
            get => _index;
            private set
            {
                _index = value;
                _index.Parent = this;
            }
        }

        public Root(ITerm radicand, ITerm index)
        {
            Radicand = radicand;
            Radicand.Parent = this;
            Index = index;
            Index.Parent = this;
        }

        public ITerm Replace(ITerm oldTerm, ITerm newTerm)
        {
            if (newTerm is EmptyTerm)
                return DissolveRoot(oldTerm);
            if (oldTerm == Index)
                Index = newTerm;
            else if (oldTerm == Radicand)
                Radicand = newTerm;
            else
                throw new InvalidOperationException("Argument oldTerm does not match an existing Term in this component");

            return newTerm;
        }

        private ITerm DissolveRoot(ITerm obsoleteTerm)
        {
            if (obsoleteTerm == Index)
                return Parent.Replace(this, Radicand);
            if (obsoleteTerm == Radicand)
                return Parent.Replace(this, Index);
            throw new InvalidOperationException("Argument obsoleteTerm does not match an existing Term in this component");
        }

        public string ToLatexCode()
        {
            return $"\\sqrt[{Index.ToLatexCode()}]{{{Radicand.ToLatexCode()}}}";
        }

        public override string ToString() => ToLatexCode();
    }
}
