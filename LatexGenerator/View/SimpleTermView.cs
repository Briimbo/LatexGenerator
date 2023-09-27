using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using LatexGenerator.Model;

namespace LatexGenerator.View
{
    internal sealed class SimpleTermView : TermView
    {
        private TextBlock Content { get; }

        public SimpleTermView(SimpleTerm l, ITerm selectedTerm, TermView parentTermView)
        {
            Model = l;
            ParentTermView = parentTermView;
            Orientation = Orientation.Horizontal;
            HorizontalAlignment = HorizontalAlignment.Center;
            VerticalAlignment = VerticalAlignment.Center;

            Content = new TextBlock {Text = l.Symbol.ToString()};
            Children.Add(Content);

            if (l == selectedTerm)
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
            return ParentTermView.NavigateUp(this);
        }

        public override TermView NavigateDown(TermView sender)
        {
            return ParentTermView.NavigateDown(this);
        }

        public override void Select()
        {
            Content.Foreground = Brushes.Red;
        }

        public override void Deselect()
        {
            Content.Foreground = Brushes.Black;
        }

        public override TermView FindTermView(ITerm term)
        {
            return Model == term ? this : null;
        }
    }
}
