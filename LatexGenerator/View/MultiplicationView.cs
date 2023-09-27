using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using LatexGenerator.Model;

namespace LatexGenerator.View
{
    internal sealed class MultiplicationView : TermView
    {
        private readonly List<TermView> _children;

        public MultiplicationView(Multiplication m, ITerm selectedTerm, TermView parentTermView)
        {
            Model = m;
            ParentTermView = parentTermView;
            Orientation = Orientation.Horizontal;
            VerticalAlignment = VerticalAlignment.Center;
            HorizontalAlignment = HorizontalAlignment.Center;

            _children = m.Children.Select(t => TermDispatcher.DispatchTermIntoView(t, selectedTerm, this)).ToList();
            _children.ForEach(c => Children.Add(c));

            if (m == selectedTerm)
                Select();
        }

        public override TermView NavigateLeft(TermView sender)
        {
            var indexOfSender = _children.IndexOf(sender);
            if (indexOfSender == -1)
                return _children.First();
            return indexOfSender == 0 ? ParentTermView.NavigateLeft(this) : _children[indexOfSender - 1];
        }

        public override TermView NavigateRight(TermView sender)
        {
            var indexOfSender = _children.IndexOf(sender);
            if (indexOfSender == -1)
                return _children.Last();
            return indexOfSender == _children.Count - 1 ? ParentTermView.NavigateRight(this) : _children[indexOfSender + 1];
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
            _children.ForEach(c => c.Select());
        }

        public override void Deselect()
        {
            _children.ForEach(c => c.Deselect());
        }

        public override TermView FindTermView(ITerm term)
        {
            return Model == term 
                ? this 
                : _children.Select(child => child.FindTermView(term)).FirstOrDefault(findResult => findResult != null);
        }
    }
}
