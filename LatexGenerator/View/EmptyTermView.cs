using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using LatexGenerator.Model;


namespace LatexGenerator.View
{
    internal sealed class EmptyTermView : TermView
    {
        private Rectangle Content { get; }

        public EmptyTermView(EmptyTerm e, ITerm selectedTerm, TermView parentTermView)
        {
            Model = e;
            ParentTermView = parentTermView;
            Orientation = Orientation.Horizontal;
            HorizontalAlignment = HorizontalAlignment.Center;
            VerticalAlignment = VerticalAlignment.Center;

            Content = new Rectangle {Height = 12, Width = 12, StrokeDashArray = new DoubleCollection(new[] {3.7}), Stroke = Brushes.Black};
            var border = new Border {Padding = new Thickness(2, 2, 2, 2), Child = Content}; // use border for padding
            Children.Add(border);

            if (e == selectedTerm)
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
            Content.Stroke = Brushes.Red;
            Content.Fill = new SolidColorBrush(Color.FromArgb(20, 255, 0, 0));
        }

        public override void Deselect()
        {
            Content.Fill = Brushes.Transparent;
            Content.Stroke = Brushes.Black;
        }

        public override TermView FindTermView(ITerm term)
        {
            return Model == term ? this : null;
        }
    }
}
