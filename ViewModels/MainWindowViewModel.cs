using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Threading;

namespace Butler
{
    public class MainWindowViewModel : ViewModelBase
    {
        private ObservableCollection<TabItemViewModel> _items = new ObservableCollection<TabItemViewModel>();
        private int _totalWorkItemsCount;

        public MainWindowViewModel()
        {
            _totalWorkItemsCount = 0;
            Items = new ObservableCollection<TabItemViewModel>();

            QueryProvider.Instance.Changed += (sender, e) =>
            {
                LoadTabControls();

                BackgroundRunner.Run(() => { Repository.Instance.Refresh(); });
            };

            Repository.Instance.AfterLoadWorkItems +=
                (sender, e) => { new AsyncRelayCommand(DataBind, null).Execute(null); };

            LoadTabControls();

            new AsyncRelayCommand(DataBind, null).Execute(null);
        }

        public int TotalWorkItemsCount
        {
            get { return _totalWorkItemsCount; }
            private set
            {
                _totalWorkItemsCount = value;
                RaisePropertyChanged("TotalWorkItemsCount");
            }
        }

        public ObservableCollection<TabItemViewModel> Items
        {
            get { return _items; }
            private set
            {
                _items = value;
                RaisePropertyChanged("Items");
            }
        }

        private void LoadTabControls()
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.DataBind,
                new Action(
                    () =>
                    {
                        Items.Clear();

                        QueryProvider.Instance.Data.ForEach(
                            queryInfo =>
                            {
                                Items.Add(new TabItemViewModel(queryInfo.Name, new WorkItemViewModel(queryInfo.Content)));
                            });
                    }));
        }

        private void DataBind()
        {
#if DEBUG
            var sw = Stopwatch.StartNew();
#endif

            TotalWorkItemsCount = Repository.Instance.GetWorkItemCountInFloating(0)
                                  + Repository.Instance.GetWorkItemCountInFloating(1)
                                  + Repository.Instance.GetWorkItemCountInFloating(2);
#if DEBUG
            sw.Stop();

            Console.WriteLine("MainWindowViewModel.DataBind: {0}", sw.Elapsed.TotalMilliseconds);
#endif
        }
    }
}