using System;
using System.Collections.Generic;
using LatexGenerator.Model;
using LatexGenerator.View;

namespace LatexGenerator
{
    internal class Controller
    {
        private readonly MainWindow _window;
        private ITerm _currentSelection;

        public ITerm CurrentSelection
        {
            get => _currentSelection ?? FullEquation;
            private set => _currentSelection = value ?? FullEquation;
        }

        public RootLevelTerm FullEquation { get; }

        public Controller(MainWindow window)
        {
            _window = window;
            FullEquation = new RootLevelTerm();
            CurrentSelection = FullEquation.Content;
        }

        #region AddInteractions

        public enum InteractionAddType
        {
            AddExponentForBase,
            AddBaseForExponent,
            AddSubscriptForBase,
            AddBaseForSubscript,
            AddFractionBelow,
            AddMultiplication,
            AddParentheses,
            AddSimpleTerm,
            AddLeftMultiplication,
            AddFractionOnTop,
            AddUnlimitedMultiplication,
            AddLeftUnlimitedMultiplication
        }

        public void DispatchAddInteraction(InteractionAddType type, object payload)
        {
            
            DispatchAddInteraction(type, CurrentSelection, payload);
            _window.NotifyTermChanged(FullEquation);
        }

        private ITerm DispatchAddInteraction(InteractionAddType type, ITerm baseEquation, object payload)
        {
            if (payload is null)
                payload = new EmptyTerm();

            var result = type switch
            {
                InteractionAddType.AddExponentForBase => DispatchAddExponent(baseEquation, payload, true),
                InteractionAddType.AddBaseForExponent => DispatchAddExponent(baseEquation, payload, false),
                InteractionAddType.AddSubscriptForBase => DispatchAddSubscript(baseEquation, payload, true),
                InteractionAddType.AddBaseForSubscript => DispatchAddSubscript(baseEquation, payload, false),
                InteractionAddType.AddFractionBelow => DispatchAddFraction(baseEquation, payload as ITerm, false),
                InteractionAddType.AddFractionOnTop => DispatchAddFraction(baseEquation, payload as ITerm, true),
                InteractionAddType.AddUnlimitedMultiplication => DispatchAddUnlimitedMultiplication(baseEquation, payload as ITerm, CurrentSelection),
                InteractionAddType.AddLeftUnlimitedMultiplication => DispatchAddLeftUnlimitedMultiplication(baseEquation, payload as ITerm, CurrentSelection),
                InteractionAddType.AddParentheses => DispatchAddParentheses(baseEquation),
                InteractionAddType.AddSimpleTerm => DispatchAddSimpleTerm(baseEquation, payload),
                _ => throw new NotImplementedException($"InteractionAddType argument \"type\" '{type}' has no associated Interaction")
            };
            return result;
        }

        private Multiplication DispatchAddLeftUnlimitedMultiplication(ITerm baseEquation, ITerm payload, ITerm desiredRightNeighbor)
        {
            if (!(baseEquation.Parent is Multiplication m))
                return DispatchAddUnlimitedMultiplication(baseEquation, payload, null);

            var idx = m.Children.IndexOf(desiredRightNeighbor);
            var leftNeighbor = idx <= 0 ? null : m.Children[idx - 1];

            return DispatchAddUnlimitedMultiplication(baseEquation, payload, leftNeighbor);
        }

        private Multiplication DispatchAddUnlimitedMultiplication(ITerm baseEquation, ITerm payload, ITerm desiredLeftNeighbor)
        {
            var parent = baseEquation.Parent;
            var result = Multiplication.GetOrCreate(baseEquation, payload, desiredLeftNeighbor);
            
            if (!(parent is Multiplication))
                parent.Replace(baseEquation, result);

            CurrentSelection = payload;
            return result;
        }

        private Exponent DispatchAddExponent(ITerm baseEquation, object payload, bool payloadAsExponent)
        {
            if (payload is string s)
                payload = new SimpleTerm(s);

            var parent = baseEquation.Parent;
            var validPayload = payload is ITerm t ? t : new EmptyTerm();
            var result = payloadAsExponent ? new Exponent(baseEquation, validPayload) : new Exponent(validPayload, baseEquation);
            parent.Replace(baseEquation, result);

            CurrentSelection = payloadAsExponent ? result.Power : result.BaseTerm;

            return result;
        }
        private Subscript DispatchAddSubscript(ITerm baseEquation, object payload, bool payloadAsSubscript)
        {
            if (payload is string s)
                payload = new SimpleTerm(s);

            var parent = baseEquation.Parent;
            var validPayload = payload is ITerm t ? t : new EmptyTerm();
            var result = payloadAsSubscript ? new Subscript(baseEquation, validPayload) : new Subscript(validPayload, baseEquation);
            parent.Replace(baseEquation, result);

            CurrentSelection = payloadAsSubscript ? result.Sub : result.BaseTerm;

            return result;
        }

        private Fraction DispatchAddFraction(ITerm baseEquation, ITerm payload, bool baseEquationAsDenominator)
        {
            var parent = baseEquation.Parent;
            var result = baseEquationAsDenominator ? new Fraction(payload, baseEquation) : new Fraction(baseEquation, payload);
            parent.Replace(baseEquation, result);

            CurrentSelection = baseEquationAsDenominator ? result.Numerator : result.Denominator;

            return result;
        }

        private Parentheses DispatchAddParentheses(ITerm baseEquation)
        {
            var parent = baseEquation.Parent;
            var result = new Parentheses(baseEquation);
            parent.Replace(baseEquation, result);

            CurrentSelection = result;
            return result;
        }

        private ITerm DispatchAddSimpleTerm(ITerm baseEquation, object payload)
        {
            var result = payload is KeyValuePair<string, string> pair ? new SimpleTerm(pair) : (ITerm) new EmptyTerm();
            if (baseEquation == FullEquation)
            {
                FullEquation.Replace(baseEquation, result);
                CurrentSelection = result;
            }
            else if (baseEquation is EmptyTerm)
            {
                baseEquation.Parent.Replace(baseEquation, result);
                CurrentSelection = result;
            }
            else
            {
                var parent = baseEquation.Parent;
                //result = DispatchAddInteraction(InteractionAddType.AddMultiplication, baseEquation, result);
                result = DispatchAddInteraction(InteractionAddType.AddUnlimitedMultiplication, baseEquation, result);
                //result.Parent = parent;
            }

            return result;
        }
        #endregion

        #region NavigationInteractions

        public enum NavigationDirection
        {
            Left, Right, Up, Down
        }

        public void DispatchNavigation(TermView rootTermView, NavigationDirection direction)
        {
            rootTermView.Deselect();
            var currentView = rootTermView.FindTermView(CurrentSelection);

            var selectedView = direction switch
            {
                NavigationDirection.Left => DispatchLeftNavigation(currentView),
                NavigationDirection.Right => DispatchRightNavigation(currentView),
                NavigationDirection.Up => DispatchUpNavigation(currentView),
                NavigationDirection.Down => DispatchDownNavigation(currentView),
                _ => throw new NotImplementedException($"Unknown direction '{direction}'")
            };
            CurrentSelection = selectedView.Model;
            selectedView.Select();
        }

        private TermView DispatchLeftNavigation(TermView rootTermView)
        {
            return rootTermView.NavigateLeft(null);
        }

        private TermView DispatchRightNavigation(TermView rootTermView)
        {
            return rootTermView.NavigateRight(null);
        }

        private TermView DispatchUpNavigation(TermView rootTermView)
        {
            return rootTermView.NavigateUp(null);
        }

        private TermView DispatchDownNavigation(TermView rootTermView)
        {
            return rootTermView.NavigateDown(null);
        }


        #endregion

        public void DispatchDelete()
        {
            if (CurrentSelection is Parentheses parentheses)
            {
                parentheses.Parent.Replace(parentheses, parentheses.InnerTerm);
                CurrentSelection = parentheses.InnerTerm;
                _window.NotifyTermChanged(FullEquation);
                return;
            }

            CurrentSelection = CurrentSelection.Parent switch
            {
                Exponent e => e.Replace(CurrentSelection, new EmptyTerm()),
                Fraction f => f.Replace(CurrentSelection, new EmptyTerm()),
                Multiplication m => m.Replace(CurrentSelection, new EmptyTerm()),
                Parentheses p => p.Replace(p, new EmptyTerm()),
                Subscript s => s.Replace(CurrentSelection, new EmptyTerm()),
                RootLevelTerm r => r.Replace(CurrentSelection, new EmptyTerm()),
                _ => throw new InvalidOperationException($"{CurrentSelection.Parent.GetType()} cannot be a parent of an element")
            };

            _window.NotifyTermChanged(FullEquation);
        }
    }
}
