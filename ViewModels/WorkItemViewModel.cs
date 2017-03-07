using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Threading;

namespace Butler
{
    public class WorkItemViewModel : ViewModelBase
    {
        private readonly string _query;
        private Visibility _actionVisibility;
        private AsyncRelayCommand<string> _gotoWorkItemCommand;
        private WorkItemModel _selectedWorkItem;

        private ObservableCollection<WorkItemModel> _workItems;

        public WorkItemViewModel(string query)
        {
            _workItems = new ObservableCollection<WorkItemModel>();
            _query = query;
            _actionVisibility = Visibility.Visible;

            Repository.Instance.BeforeLoadWorkItems +=
                (sender, e) =>
                {
                    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.DataBind,
                        new Action(() => { ActionsVisible = Visibility.Visible; }));
                };

            Repository.Instance.AfterLoadWorkItems2 += (sender, e) =>
            {
                new AsyncRelayCommand(() =>
                {
                    DataBind();
                    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.DataBind,
                        new Action(() => { ActionsVisible = Visibility.Collapsed; }));
                }, null).Execute(null);
            };

            new AsyncRelayCommand(() =>
            {
                DataBind();
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.DataBind,
                    new Action(() => { ActionsVisible = Visibility.Collapsed; }));
            }, null).Execute(null);
        }

        public ObservableCollection<WorkItemModel> WorkItems
        {
            get { return _workItems; }
            private set
            {
                _workItems = value;
                RaisePropertyChanged("WorkItems");
            }
        }

        public WorkItemModel SelectedWorkItem
        {
            get { return _selectedWorkItem; }
            private set
            {
                _selectedWorkItem = value;
                RaisePropertyChanged("SelectedWorkItem");
            }
        }


        public AsyncRelayCommand<string> GotoWorkItemCommand
        {
            get
            {
                return _gotoWorkItemCommand ??
                       (_gotoWorkItemCommand =
                           new AsyncRelayCommand<string>(Repository.OpenWorkItem));
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

        private void DataBind()
        {
#if DEBUG
            var sw = System.Diagnostics.Stopwatch.StartNew();
#endif

            WorkItems = new ObservableCollection<WorkItemModel>(Repository.Instance.GetWorkItemModels(_query));
#if DEBUG
            sw.Stop();

            Console.WriteLine("WorkItemViewModel.DataBind: {0}", sw.Elapsed.TotalMilliseconds);
#endif
        }
    }
}