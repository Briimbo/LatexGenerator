using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using LatexGenerator.Model;

namespace LatexGenerator.View
{
    internal sealed class ExponentView : TermView
    {
        private TermView BaseView { get; }
        private TermView PowerView { get; }
        public double PowerViewWidth { get => PowerView.ActualWidth; }
        private bool _resized;

        public ExponentView(Exponent e, ITerm selectedTerm, TermView parentTermView)
        {
            Model = e;
            ParentTermView = parentTermView;
            Orientation = Orientation.Horizontal;
            VerticalAlignment = VerticalAlignment.Center;
            HorizontalAlignment = HorizontalAlignment.Center;

            BaseView = TermDispatcher.DispatchTermIntoView(e.BaseTerm, selectedTerm, this);
            PowerView = TermDispatcher.DispatchTermIntoView(e.Power, selectedTerm, this);
            TextElement.SetFontSize(PowerView, 9);
            DockPanel.SetDock(PowerView, Dock.Top);

            var container = new DockPanel();
            container.Children.Add(BaseView);
            container.Children.Add(PowerView);

            Children.Add(container);

            container.SizeChanged += Container_SizeChanged;

            if (e == selectedTerm)
                Select();
        }

        private void Container_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (_resized)
                return;
            var horizontalShift = 0d;
            if ((Model as Exponent).BaseTerm is Subscript)
            {
                horizontalShift = -(BaseView as SubscriptView).SubViewWidth;
                PowerView.HorizontalAlignment = HorizontalAlignment.Left;
            }
            var verticalShift = e.NewSize.Height / 3.5 + BaseView.ActualHeight / 2;
            PowerView.Margin = new Thickness(horizontalShift, 0, 0, verticalShift);
            _resized = true;
        }

        public override TermView NavigateLeft(TermView sender)
        {
            return sender != BaseView ? BaseView : ParentTermView.NavigateLeft(this);
        }

        public override TermView NavigateRight(TermView sender)
        {
            return sender != PowerView ? PowerView : ParentTermView.NavigateRight(this);
        }

        public override TermView NavigateUp(TermView sender)
        {
            return sender != PowerView ? PowerView : ParentTermView.NavigateUp(this);
        }

        public override TermView NavigateDown(TermView sender)
        {
            return sender != BaseView ? BaseView : ParentTermView.NavigateDown(this);
        }

        public override void Select()
        {
            BaseView.Select();
            PowerView.Select();
        }

        public override void Deselect()
        {
            BaseView.Deselect();
            PowerView.Deselect();
        }

        public override TermView FindTermView(ITerm term)
        {
            if (Model == term)
                return this;

            var baseFindResult = BaseView.FindTermView(term);
            return baseFindResult ?? PowerView.FindTermView(term);
        }
    }
}
