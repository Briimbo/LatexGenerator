using System.Windows.Controls;
using LatexGenerator.Model;

namespace LatexGenerator.View
{
    internal abstract class TermView : StackPanel
    {
        protected TermView ParentTermView { get; set; }
        public ITerm Model { get; set; }

        public abstract TermView NavigateLeft(TermView sender);
        public abstract TermView NavigateRight(TermView sender);
        public abstract TermView NavigateUp(TermView sender);
        public abstract TermView NavigateDown(TermView sender);
        public abstract void Select();
        public abstract void Deselect();
        public abstract TermView FindTermView(ITerm term);

        public override string ToString() => Model.ToLatexCode();
    }
}
