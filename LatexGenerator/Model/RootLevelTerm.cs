namespace LatexGenerator.Model
{
    internal class RootLevelTerm : ITerm
    {
        private ITerm _content;

        public ITerm Parent
        {
            get => this;
            set { }
        }

        public ITerm Content
        {
            get => _content;
            set
            {
                _content = value;
                _content.Parent = this;
            }
        }

        public RootLevelTerm()
        {
            Content = new EmptyTerm();
        }

        public ITerm Replace(ITerm oldTerm, ITerm newTerm)
        {
            if (oldTerm != Content)
                return Content.Replace(oldTerm, newTerm);

            Content = newTerm;
            return newTerm;
        }

        public string ToLatexCode()
        {
            return $"${Content.ToLatexCode()}$";
        }

        public override string ToString() => ToLatexCode();
    }
}
