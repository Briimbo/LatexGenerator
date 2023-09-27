using System;
using System.Collections.Generic;
using System.Linq;

namespace LatexGenerator.Model
{
    internal class Multiplication : ITerm
    {
        public List<ITerm> Children { get; } = new List<ITerm>();

        public ITerm Parent { get; set; }

        public static Multiplication GetOrCreate(ITerm oldTerm, ITerm newTerm, ITerm desiredLeftNeighbor)
        {
            Multiplication result;
            if (oldTerm is Multiplication m) // => oldTerm.Parent is not a Multiplication
                result = m;
            else if (oldTerm.Parent is Multiplication p)
                result = p;
            else
                return new Multiplication(oldTerm, newTerm) {Parent = oldTerm.Parent};

            if (desiredLeftNeighbor == result)
                desiredLeftNeighbor = result.Children.Last();

            result.AddTerm(newTerm, desiredLeftNeighbor);
            return result;
        }

        private Multiplication(ITerm leftTerm, ITerm rightTerm)
        {
            if (rightTerm is null)
                throw new ArgumentNullException(nameof(rightTerm));

            Children.Add(leftTerm);
            Children.Add(rightTerm);
            leftTerm.Parent = this;
            rightTerm.Parent = this;
        }

        public void AddTerm(ITerm newTerm, ITerm desiredLeftNeighbor)
        {
            var idx = Children.IndexOf(desiredLeftNeighbor);
            Children.Insert(idx + 1, newTerm);
            newTerm.Parent = this;
        }

        public ITerm Replace(ITerm oldTerm, ITerm newTerm)
        {
            if (oldTerm == this && newTerm is EmptyTerm)
                return Parent.Replace(this, newTerm);

            var idx = Children.IndexOf(oldTerm);
            if (idx < 0)
                return ReplaceInChildren(oldTerm, newTerm);

            if (newTerm is EmptyTerm)
                return DeleteOldTerm(oldTerm, idx);

            if (newTerm is Multiplication m)
            {
                Children.Remove(oldTerm);
                Children.InsertRange(idx, m.Children);
                Children.ForEach(c => c.Parent = this);
                return m.Children.Last();
            }
            
            Children[idx] = newTerm;
            newTerm.Parent = this;
            
            return newTerm;
        }

        private ITerm ReplaceInChildren(ITerm oldTerm, ITerm newTerm)
        {
            foreach (var child in Children)
            {
                try
                {
                    return child.Replace(oldTerm, newTerm);
                }
                catch (InvalidOperationException)
                {
                }
            }
            throw new InvalidOperationException("Argument oldTerm does not match an existing Term in this component");
        }

        private ITerm DeleteOldTerm(ITerm oldTerm, int idx)
        {
            Children.Remove(oldTerm);
            if (Children.Count == 1)
                return Parent.Replace(this, Children[0]);
            if (Children.Count == 0) // should never actually occur
                return Parent.Replace(this, new EmptyTerm());
            return idx > 0 ? Children[idx - 1] : Children[0];
        }

        public string ToLatexCode()
        {
            return string.Join(" ", Children.Select(c => c.ToLatexCode()));
        }

        public override string ToString() => ToLatexCode();
    }
}
