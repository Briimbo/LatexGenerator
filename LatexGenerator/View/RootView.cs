using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using LatexGenerator.Model;

namespace LatexGenerator.View
{
    internal sealed class RootView : TermView
    {
        private TermView RadicandView { get; }
        private TermView IndexView { get; }
        private bool _resized;

        public RootView(Root r, ITerm selectedTerm, TermView parentTermView)
        {
            Model = r;
            ParentTermView = parentTermView;
            Orientation = Orientation.Horizontal;
            VerticalAlignment = VerticalAlignment.Center;
            HorizontalAlignment = HorizontalAlignment.Center;

            RadicandView = TermDispatcher.DispatchTermIntoView(r.Radicand, selectedTerm, this);
            IndexView = TermDispatcher.DispatchTermIntoView(r.Index, selectedTerm, this);
            TextElement.SetFontSize(IndexView, 9);
            DockPanel.SetDock(IndexView, Dock.Top);

            var container = new DockPanel();
            container.Children.Add(RadicandView);
            container.Children.Add(IndexView);

            Children.Add(container);

            container.SizeChanged += Container_SizeChanged;

            if (r == selectedTerm)
                Select();
        }

        private void Container_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (_resized)
                return;
            var verticalShift = e.NewSize.Height / 3.5 + RadicandView.ActualHeight / 2;
            IndexView.Margin = new Thickness(0, 0, 0, verticalShift);
            _resized = true;
        }

        public override TermView NavigateLeft(TermView sender)
        {
            return sender != RadicandView ? RadicandView : ParentTermView.NavigateLeft(this);
        }

        public override TermView NavigateRight(TermView sender)
        {
            return sender != IndexView ? IndexView : ParentTermView.NavigateRight(this);
        }

        public override TermView NavigateUp(TermView sender)
        {
            return sender != IndexView ? IndexView : ParentTermView.NavigateUp(this);
        }

        public override TermView NavigateDown(TermView sender)
        {
            return sender != RadicandView ? RadicandView : ParentTermView.NavigateDown(this);
        }

        public override void Select()
        {
            RadicandView.Select();
            IndexView.Select();
        }

        public override void Deselect()
        {
            RadicandView.Deselect();
            IndexView.Deselect();
        }

        public override TermView FindTermView(ITerm term)
        {
            if (Model == term)
                return this;

            var radicandFindResult = RadicandView.FindTermView(term);
            return radicandFindResult ?? IndexView.FindTermView(term);
        }
    }
}
