using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace Butler
{
    /// <summary>
    ///     Interaction logic for FloatingWindow.xaml
    /// </summary>
    public partial class FloatingWindow
    {
        public FloatingWindow()
        {
            InitializeComponent();
        }

        private void MenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
            //Application.Current.Dispatcher.BeginInvokeShutdown(DispatcherPriority.Normal);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.G && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                GoToWindow.Instance.Owner = this;
                GoToWindow.Instance.Top = Top + Height;
                GoToWindow.Instance.Left = Left - 20; //hard code , todo
                GoToWindow.Instance.Show();
            }
            base.OnKeyDown(e);
        }

        protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.DataBind, new Action(() =>
            {
                if (!MainWindow.Instance.IsVisible)
                {
                    MainWindow.Instance.Show();
                }

                if (MainWindow.Instance.WindowState == WindowState.Minimized)
                {
                    MainWindow.Instance.WindowState = WindowState.Normal;
                }

                MainWindow.Instance.Activate();
            }));
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
            base.OnMouseDown(e);
        }
    }
}