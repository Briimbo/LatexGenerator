using LatexGenerator.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace LatexGenerator.View
{
    internal sealed class SubscriptView : TermView
    {
        private TermView BaseView { get; }
        private TermView SubView { get; }
        public double SubViewWidth { get => SubView.ActualWidth; }
        private bool _resized;

        public SubscriptView(Subscript s, ITerm selectedTerm, TermView parentTermView)
        {
            Model = s;
            ParentTermView = parentTermView;
            Orientation = Orientation.Horizontal;
            VerticalAlignment = VerticalAlignment.Center;
            HorizontalAlignment = HorizontalAlignment.Center;

            BaseView = TermDispatcher.DispatchTermIntoView(s.BaseTerm, selectedTerm, this);
            SubView = TermDispatcher.DispatchTermIntoView(s.Sub, selectedTerm, this);
            TextElement.SetFontSize(SubView, 9);
            DockPanel.SetDock(SubView, Dock.Bottom);

            var container = new DockPanel();
            container.Children.Add(BaseView);
            container.Children.Add(SubView);

            Children.Add(container);

            container.SizeChanged += Container_SizeChanged;

            if (s == selectedTerm)
                Select();
        }

        private void Container_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (_resized)
                return;
            var horizontalShift = 0d;
            if ((Model as Subscript).BaseTerm is Exponent)
            {
                horizontalShift = -(BaseView as ExponentView).PowerViewWidth;
                SubView.HorizontalAlignment = HorizontalAlignment.Left;
            }
            var verticalShift = e.NewSize.Height / 3.5 + BaseView.ActualHeight / 2;
            SubView.Margin = new Thickness(horizontalShift, verticalShift, 0, 0);
            _resized = true;
        }

        public override void Deselect()
        {
            BaseView.Deselect();
            SubView.Deselect();
        }

        public override TermView FindTermView(ITerm term)
        {
            if (Model == term)
                return this;

            var baseFindResult = BaseView.FindTermView(term);
            return baseFindResult ?? SubView.FindTermView(term);
        }

        public override TermView NavigateDown(TermView sender)
        {
            return sender != SubView ? SubView : ParentTermView.NavigateDown(this);
        }

        public override TermView NavigateLeft(TermView sender)
        {
            return sender != BaseView ? BaseView : ParentTermView.NavigateLeft(this);
        }

        public override TermView NavigateRight(TermView sender)
        {
            return sender != SubView ? SubView : ParentTermView.NavigateRight(this);
        }

        public override TermView NavigateUp(TermView sender)
        {
            return sender != BaseView ? BaseView : ParentTermView.NavigateUp(this);
        }

        public override void Select()
        {
            BaseView.Select();
            SubView.Select();
        }
    }
}
