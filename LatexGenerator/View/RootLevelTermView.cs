using LatexGenerator.Model;

namespace LatexGenerator.View
{
    internal class RootLevelTermView : TermView
    {
        private TermView _content;

        public TermView Content
        {
            get => _content;
            set
            {
                _content = value;
                Model = value.Model;
            }
        }

        public RootLevelTermView()
        {
            ParentTermView = this;
        }

        public override TermView NavigateLeft(TermView sender)
        {
            return sender == Content ? Content : Content.NavigateLeft(this);
        }

        public override TermView NavigateRight(TermView sender)
        {
            return sender == Content ? Content : Content.NavigateRight(this);
        }

        public override TermView NavigateUp(TermView sender)
        {
            return sender == Content ? Content : Content.NavigateUp(this);
        }

        public override TermView NavigateDown(TermView sender)
        {
            return sender == Content ? Content : Content.NavigateDown(this);
        }

        public override void Select()
        {
            Content.Select();
        }

        public override void Deselect()
        {
            Content.Deselect();
        }

        public override TermView FindTermView(ITerm term)
        {
            return _content.FindTermView(term);
        }
    }
}
