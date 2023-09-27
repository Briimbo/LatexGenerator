using System;
using LatexGenerator.Model;
using LatexGenerator.View;

namespace LatexGenerator
{
    internal class TermDispatcher
    {
        public static RootLevelTermView DispatchTermIntoView(RootLevelTerm term, ITerm selectedTerm)
        { 
            var rootLevelTermView = new RootLevelTermView();
            var result = DispatchTermIntoView(term.Content, selectedTerm, rootLevelTermView);
            rootLevelTermView.Content = result;
            return rootLevelTermView;
        }

        public static TermView DispatchTermIntoView(ITerm term, ITerm selectedTerm, TermView parentTermView)
        {
            return term switch
            {
                EmptyTerm e => new EmptyTermView(e, selectedTerm, parentTermView),
                Exponent e => new ExponentView(e, selectedTerm, parentTermView),
                Fraction f => new FractionView(f, selectedTerm, parentTermView),
                Multiplication m => new MultiplicationView(m, selectedTerm, parentTermView),
                Parentheses p => new ParenthesesView(p, selectedTerm, parentTermView),
                SimpleTerm s => new SimpleTermView(s, selectedTerm, parentTermView),
                Subscript s => new SubscriptView(s, selectedTerm, parentTermView),
                RootLevelTerm r => DispatchTermIntoView(r.Content, selectedTerm, parentTermView),
                _ => throw new NotImplementedException($"Unknown ITerm type '{term?.GetType().Name}'")
            };
        }
    }
}
