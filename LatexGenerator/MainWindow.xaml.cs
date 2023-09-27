using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using LatexGenerator.Model;
using LatexGenerator.View;
using Microsoft.VisualBasic.FileIO;

namespace LatexGenerator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Controller Controller { get; }
        internal RootLevelTermView RootLevelTermView { get; private set; }

        public MainWindow()
        {
            Controller = new Controller(this);
            InitializeComponent();
            InitializeButtons();
            CharacterInput.TextChanged += OnCharacterInputTextChanged;
            NotifyTermChanged(Controller.FullEquation);
        }

        private void InitializeButtons()
        {
            var basicSymbols = ParseBasicSymbols(); // Dictionary for symbols that can be displayed as a string with a simple Latex command
            foreach (var symbol in basicSymbols)
            {
                var button = new Button { Content = symbol.Key };
                button.Click += (sender, eventArgs) => Controller.DispatchAddInteraction(Controller.InteractionAddType.AddSimpleTerm, symbol);
                SymbolButtonsContainer.Children.Add(button);
            }

            FractionBelowButton.Click += (sender, eventArgs) => Controller.DispatchAddInteraction(Controller.InteractionAddType.AddFractionBelow, null);
            FractionOnTopButton.Click += (sender, eventArgs) => Controller.DispatchAddInteraction(Controller.InteractionAddType.AddFractionOnTop, null);

            SquaredButton.Click += (sender, eventArgs) => Controller.DispatchAddInteraction(Controller.InteractionAddType.AddExponentForBase, "2");

            CubedButton.Click += (sender, eventArgs) => Controller.DispatchAddInteraction(Controller.InteractionAddType.AddExponentForBase, "3");

            ExponentForBaseButton.Click += (sender, eventArgs) => Controller.DispatchAddInteraction(Controller.InteractionAddType.AddExponentForBase, null);
            BaseForExponentButton.Click += (sender, eventArgs) => Controller.DispatchAddInteraction(Controller.InteractionAddType.AddBaseForExponent, null);

            SubscriptForBaseButton.Click += (sender, eventArgs) => Controller.DispatchAddInteraction(Controller.InteractionAddType.AddSubscriptForBase, null);
            BaseForSubscriptButton.Click += (sender, eventArgs) => Controller.DispatchAddInteraction(Controller.InteractionAddType.AddBaseForSubscript, null);

            ParenthesesButton.Click += (sender, eventArgs) => Controller.DispatchAddInteraction(Controller.InteractionAddType.AddParentheses, null);

            MultiplyRightButton.Click += (sender, eventArgs) => Controller.DispatchAddInteraction(Controller.InteractionAddType.AddUnlimitedMultiplication, null);
            MultiplyLeftButton.Click += (sender, eventArgs) => Controller.DispatchAddInteraction(Controller.InteractionAddType.AddLeftUnlimitedMultiplication, null);

            InsertCustomButton.Click += OnInsertCustomSymbols;
        }

        private Dictionary<string, string> ParseBasicSymbols()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = Array.Find(assembly.GetManifestResourceNames(), name => name.EndsWith("BasicButtons.csv"));
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                var parser = new TextFieldParser(reader);
                parser.SetDelimiters(";");
                parser.TextFieldType = FieldType.Delimited;

                var parsedSymbols = new Dictionary<string, string>();
                while (!parser.EndOfData)
                {
                    var fields = parser.ReadFields();
                    parsedSymbols.Add(fields[0], fields[1]);
                }

                return parsedSymbols;
            }
        }

        internal void NotifyTermChanged(RootLevelTerm newTerm)
        {
            foreach (UIElement resultsChild in Results.Children)
            {
                if (Grid.GetRow(resultsChild) != 1 || Grid.GetColumn(resultsChild) != 0) continue;
                Results.Children.Remove(resultsChild);
                break;
            }

            RootLevelTermView = TermDispatcher.DispatchTermIntoView(newTerm, Controller.CurrentSelection);
            var newTermUiElement = new Border
            {
                Margin = new Thickness(5,5,5,5),
                BorderBrush = Brushes.Red,
                BorderThickness = new Thickness(1,1,1,1)
            };
            Grid.SetRow(newTermUiElement, 1);
            Grid.SetColumn(newTermUiElement, 0);
            newTermUiElement.Child = RootLevelTermView.Content;
            Results.Children.Add(newTermUiElement);
            LatexEquation.Text = newTerm.ToLatexCode();
        }

        private void OnNavigateLeft(object sender, RoutedEventArgs e)
        {
            Navigate(Controller.NavigationDirection.Left);
        }

        private void OnNavigateRight(object sender, RoutedEventArgs e)
        {
            Navigate(Controller.NavigationDirection.Right);
        }

        private void OnNavigateUp(object sender, RoutedEventArgs e)
        {
            Navigate(Controller.NavigationDirection.Up);
        }

        private void OnNavigateDown(object sender, RoutedEventArgs e)
        {
            Navigate(Controller.NavigationDirection.Down);
        }

        private void Navigate(Controller.NavigationDirection direction)
        {
            Controller.DispatchNavigation(RootLevelTermView, direction);
        }

        private void OnDelete(object sender, RoutedEventArgs e)
        {
            Controller.DispatchDelete();
        }

        private void OnInsertCustomSymbols(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(LatexCodeInput.Text) || string.IsNullOrEmpty(CharacterInput.Text))
            {
                ShowError(); 
                return;
            }

            Controller.DispatchAddInteraction(Controller.InteractionAddType.AddSimpleTerm, new KeyValuePair<string, string>(CharacterInput.Text, LatexCodeInput.Text));
            CharacterInput.Text = "";
            LatexCodeInput.Text = "";
            _characterInputOldValue = "";
        }

        private void ShowError()
        {
            MessageBox.Show("Symbol and Latex-Code need to be provided!", "Input incomplete", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private string _characterInputOldValue = "";

        private void OnCharacterInputTextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(LatexCodeInput.Text) || LatexCodeInput.Text == _characterInputOldValue)
                LatexCodeInput.Text = CharacterInput.Text;

            _characterInputOldValue = CharacterInput.Text;
        }
    }
}
