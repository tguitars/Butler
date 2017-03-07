using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Butler
{
    /// <summary>
    ///     Interaction logic for GoToWindow.xaml
    /// </summary>
    public partial class GoToWindow
    {
        private static readonly Lazy<GoToWindow> Lazy = new Lazy<GoToWindow>(() => new GoToWindow());

        public GoToWindow()
        {
            InitializeComponent();
        }

        public static GoToWindow Instance
        {
            get { return Lazy.Value; }
        }


        private void UIElement_OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (bool.Parse(e.NewValue.ToString()))
            {
                var textBox = sender as TextBox;
                if (textBox != null)
                {
                    textBox.Focus();
                    textBox.Text = GetNum(Clipboard.GetText());
                }
            }
        }

        private void TextBox_OnKeyDown(object sender, KeyEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null)
            {
                if (e.Key == Key.Enter)
                {
                    int id;
                    if (!string.IsNullOrWhiteSpace(textBox.Text) && int.TryParse(textBox.Text, out id))
                    {
                        BackgroundRunner.Run(() => Repository.OpenWorkItem(id));
                    }
                    Visibility = Visibility.Collapsed;
                }

                if (e.Key == Key.Escape)
                {
                    Visibility = Visibility.Collapsed;
                }

                e.Handled = !IsNumberKey(e.Key) && !IsDelOrBackspaceOrTabKey(e.Key);
            }
        }

        private void TextBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null)
            {
                textBox.Text = GetNum(textBox.Text);
            }
        }

        private string GetNum(string str)
        {
            int n;
            var ret = "";
            if (int.TryParse(str, out n))
            {
                ret = n.ToString();
            }
            return ret;
        }

        private static bool IsDelOrBackspaceOrTabKey(Key inKey)
        {
            return inKey == Key.Delete || inKey == Key.Back || inKey == Key.Tab;
        }

        private static bool IsNumberKey(Key inKey)
        {
            if (inKey < Key.D0 || inKey > Key.D9)
            {
                if (inKey < Key.NumPad0 || inKey > Key.NumPad9)
                {
                    return false;
                }
            }
            return true;
        }
    }
}