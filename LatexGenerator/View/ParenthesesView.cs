using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using LatexGenerator.Model;

namespace LatexGenerator.View
{
    internal sealed class ParenthesesView : TermView
    {
        private TextBlock OpenParantheses { get; }
        private TextBlock CloseParantheses { get; }
        private TermView InnerPanel { get; }

        public ParenthesesView(Parentheses p, ITerm selectedTerm, TermView parentTermView)
        {
            Model = p;
            ParentTermView = parentTermView;
            Orientation = Orientation.Horizontal;
            HorizontalAlignment = HorizontalAlignment.Center;
            VerticalAlignment = VerticalAlignment.Center;

            OpenParantheses = new TextBlock {Text = " (", VerticalAlignment = VerticalAlignment.Center};
            CloseParantheses = new TextBlock {Text = ") ", VerticalAlignment = VerticalAlignment.Center};
            InnerPanel = TermDispatcher.DispatchTermIntoView(p.InnerTerm, selectedTerm, this);

            Children.Add(OpenParantheses);
            Children.Add(InnerPanel);
            Children.Add(CloseParantheses);

            if (p == selectedTerm)
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
            return InnerPanel;
        }

        public override TermView NavigateDown(TermView sender)
        {
            return ParentTermView;
        }

        public override void Select()
        {
            OpenParantheses.Foreground = Brushes.Red;
            CloseParantheses.Foreground = Brushes.Red;
            InnerPanel.Deselect();
        }

        public override void Deselect()
        {
            OpenParantheses.Foreground = Brushes.Black;
            CloseParantheses.Foreground = Brushes.Black;
            //InnerPanel.Deselect();
        }

        public override TermView FindTermView(ITerm term)
        {
            return Model == term ? this : InnerPanel.FindTermView(term);
        }
    }
}
