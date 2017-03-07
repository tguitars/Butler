using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using Butler.Properties;
using MahApps.Metro;

namespace Butler
{
    public class SettingViewModel : ViewModelBase
    {
        public SettingViewModel()
        {
            AccentColors = ThemeManager.Accents
                .Select(a => new ThemeModel {Name = a.Name, ColorBrush = a.Resources["AccentColorBrush"] as Brush})
                .ToList();

            AppThemes = ThemeManager.AppThemes
                .Select(
                    a =>
                        new ThemeModel
                        {
                            Name = a.Name,
                            BorderColorBrush = a.Resources["BlackColorBrush"] as Brush,
                            ColorBrush = a.Resources["WhiteColorBrush"] as Brush
                        })
                .ToList();
        }

        public List<ThemeModel> AccentColors { get; set; }

        public string CurrentColor
        {
            get { return Settings.Default.Color; }
            set
            {
                if (value == Settings.Default.Color) return;
                Settings.Default.Color = value;
                Settings.Default.Save();
                ChangeAppStyle();
                RaisePropertyChanged("CurrentColor");
            }
        }

        public List<ThemeModel> AppThemes { get; set; }

        public string Project
        {
            get { return Settings.Default.Project; }
            set
            {
                if (value == Settings.Default.Project) return;
                Settings.Default.Project = value;
                Settings.Default.Save();
                RaisePropertyChanged("Project");
            }
        }

        public string CurrentTheme
        {
            get { return Settings.Default.Theme; }
            set
            {
                if (value == Settings.Default.Theme) return;
                Settings.Default.Theme = value;
                Settings.Default.Save();
                ChangeAppStyle();
                RaisePropertyChanged("CurrentTheme");
            }
        }

        public static void ChangeAppStyle()
        {
            var theme = ThemeManager.DetectAppStyle(MainWindow.Instance);

            if (theme == null || theme.Item1.Name != Settings.Default.Theme ||
                theme.Item2.Name != Settings.Default.Color)
            {
                ThemeManager.ChangeAppStyle(MainWindow.Instance,
                    ThemeManager.GetAccent(Settings.Default.Color),
                    ThemeManager.GetAppTheme(Settings.Default.Theme));
            }
        }
    }
}