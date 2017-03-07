using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using Butler.Views.Window;
using MahApps.Metro.Controls;

namespace Butler
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private static readonly Lazy<MainWindow> Lazy = new Lazy<MainWindow>(() => new MainWindow());

        private MainWindow()
        {
            InitializeComponent();
        }

        public static MainWindow Instance
        {
            get { return Lazy.Value; }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
            Visibility = Visibility.Collapsed;
        }

        protected override void OnContentRendered(EventArgs e)
        {
            SettingViewModel.ChangeAppStyle();
            base.OnContentRendered(e);
        }

        private void ThemeSettings_Click(object sender, RoutedEventArgs e)
        {
            OpenFlyout(0);
        }

        private void AdvancedSettings_Click(object sender, RoutedEventArgs e)
        {
            OpenFlyout(1);
        }

        private void OpenFlyout(int index)
        {
            for (var i = 0; i < Flyouts.Items.Count; i++)
            {
                var flyout = Flyouts.Items[i] as Flyout;
                if (flyout != null)
                {
                    if (i == index)
                    {
                        flyout.IsOpen = !flyout.IsOpen;
                    }
                    else
                    {
                        flyout.IsOpen = false;
                    }
                }
            }
        }

        private void Project_OnClick(object sender, RoutedEventArgs e)
        {
            using (var tpp = TfsProxy.GetTeamProjectPicker())
            {
                var result = tpp.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK && tpp.SelectedProjects != null &&
                    tpp.SelectedProjects.Length > 0)
                {
                    var btn = sender as Button;
                    btn.Content = tpp.SelectedProjects[0].Name;
                    var uri = tpp.SelectedTeamProjectCollection.Uri;
                }
            }
        }

        private void Test_OnClick(object sender, RoutedEventArgs e)
        {
            new Window1().ShowDialog();
        }
    }
}