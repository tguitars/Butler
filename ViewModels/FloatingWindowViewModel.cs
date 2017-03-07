using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Threading;

namespace Butler
{
    public class FloatingWindowViewModel : ViewModelBase
    {
        private Visibility _actionVisibility;
        private bool _enableRefresh;
        private int _firstCount;
        private bool _isIndeterminate;
        private AsyncRelayCommand _refreshCommand;
        private int _secondCount;
        private int _thirdCount;

        public FloatingWindowViewModel()
        {
            _firstCount = 0;
            _secondCount = 0;
            _thirdCount = 0;

            _isIndeterminate = true;
            _actionVisibility = Visibility.Visible;
            _enableRefresh = false;

            Repository.Instance.BeforeLoadWorkItems += (sender, e) =>
            {
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.DataBind, new Action(() =>
                {
                    ActionsVisible = Visibility.Visible;
                    IsIndeterminate = true;
                    EnableRefresh = false;
                }));
            };

            Repository.Instance.AfterLoadWorkItems += (sender, e) =>
            {
                new AsyncRelayCommand(() =>
                {
                    DataBind();
                    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.DataBind, new Action(() =>
                    {
                        IsIndeterminate = false;
                        ActionsVisible = Visibility.Collapsed;
                        EnableRefresh = true;
                    }));
                }, null).Execute(null);
            };
        }

        public bool EnableRefresh
        {
            get { return _enableRefresh; }
            private set
            {
                _enableRefresh = value;
                RaisePropertyChanged("EnableRefresh");
            }
        }

        public bool IsIndeterminate
        {
            get { return _isIndeterminate; }
            private set
            {
                _isIndeterminate = value;
                RaisePropertyChanged("IsIndeterminate");
            }
        }

        public Visibility ActionsVisible
        {
            get { return _actionVisibility; }
            private set
            {
                _actionVisibility = value;
                RaisePropertyChanged("ActionsVisible");
            }
        }

        public AsyncRelayCommand RefreshCommand
        {
            get
            {
                return _refreshCommand ??
                       (_refreshCommand = new AsyncRelayCommand(() => Repository.Instance.Refresh()));
            }
        }

        public int FirstCount
        {
            get { return _firstCount; }
            private set
            {
                _firstCount = value;
                RaisePropertyChanged("FirstCount");
            }
        }

        public int SecondCount
        {
            get { return _secondCount; }
            private set
            {
                _secondCount = value;
                RaisePropertyChanged("SecondCount");
            }
        }

        public int ThirdCount
        {
            get { return _thirdCount; }
            private set
            {
                _thirdCount = value;
                RaisePropertyChanged("ThirdCount");
            }
        }

        private void DataBind()
        {
#if DEBUG
            var sw = Stopwatch.StartNew();
#endif
            FirstCount = Repository.Instance.GetWorkItemCountInFloating(0);
            SecondCount = Repository.Instance.GetWorkItemCountInFloating(1);
            ThirdCount = Repository.Instance.GetWorkItemCountInFloating(2);
#if DEBUG
            sw.Stop();

            Console.WriteLine("FloatingWindowViewModel.DataBind: {0}", sw.Elapsed.TotalMilliseconds);
#endif
        }
    }
}