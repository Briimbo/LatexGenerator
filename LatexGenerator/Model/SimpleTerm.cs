using System;
using System.Collections.Generic;

namespace LatexGenerator.Model
{
    /// <summary>
    /// Models a term that is not further nested
    /// </summary>
    internal class SimpleTerm : ITerm
    {
        public ITerm Parent { get; set; } = null;
        private string LatexCode { get; }
        public string Symbol { get; }

        public SimpleTerm(string latexCode, string symbol)
        {
            LatexCode = latexCode;
            Symbol = symbol;
        }

        public SimpleTerm(string symbol) : this(symbol, symbol)
        {
        }

        public SimpleTerm(KeyValuePair<string, string> symbolWithLatex) : this(symbolWithLatex.Value, symbolWithLatex.Key)
        {
        }


        public ITerm Replace(ITerm oldTerm, ITerm newTerm)
        {
            throw new InvalidOperationException("Method Replace of SimpleTerm cannot be called. Please create a new object instead");
        }

        public string ToLatexCode()
        {
            return LatexCode;
        }

        public override string ToString() => ToLatexCode();
    }
}
