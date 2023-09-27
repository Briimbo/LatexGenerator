using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using LatexGenerator.Model;

namespace LatexGenerator.View
{
    internal sealed class FractionView : TermView
    {
        private Separator Separator { get; }
        private TermView NumeratorView { get; }
        private TermView DenominatorView{ get; }
        

            public FractionView(Fraction f, ITerm selectedTerm, TermView parentTermView)
        {
            Model = f;
            ParentTermView = parentTermView;
            Orientation = Orientation.Vertical;
            HorizontalAlignment = HorizontalAlignment.Center;
            VerticalAlignment = VerticalAlignment.Center;

            NumeratorView = TermDispatcher.DispatchTermIntoView(f.Numerator, selectedTerm, this);
            DenominatorView = TermDispatcher.DispatchTermIntoView(f.Denominator, selectedTerm, this);
            Separator = new Separator {Margin = new Thickness(0, 0, 0, 0)};

            Children.Add(NumeratorView);
            Children.Add(Separator);
            Children.Add(DenominatorView);

            Margin = new Thickness(3, 0, 3, 0);

            if (f == selectedTerm)
                Select();
        }

        public override TermView NavigateLeft(TermView sender)
        {
            return ParentTermView.NavigateLeft(this);
        }

        public override TermView NavigateRight(TermView sender)
        {
            return ParentTermView.NavigateRight(this);
        }

        public override TermView NavigateUp(TermView sender)
        {
            return sender != NumeratorView ? NumeratorView : ParentTermView.NavigateUp(this);
        }

        public override TermView NavigateDown(TermView sender)
        {
            return sender != DenominatorView ? DenominatorView : ParentTermView.NavigateDown(this);
        }

        public override void Select()
        {
            NumeratorView.Select();
            Separator.Background = Brushes.Red;
            DenominatorView.Select();
        }

        public override void Deselect()
        {
            NumeratorView.Deselect();
            Separator.Background = Brushes.Black;
            DenominatorView.Deselect();
        }

        public override TermView FindTermView(ITerm term)
        {
            if (Model == term)
                return this;

            var numeratorResult = NumeratorView.FindTermView(term);
            return numeratorResult ?? DenominatorView.FindTermView(term);
        }
    }
}
