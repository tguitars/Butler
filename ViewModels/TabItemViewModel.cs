namespace Butler
{
    public class TabItemViewModel : ViewModelBase
    {
        private WorkItemViewModel _dataContext;
        private string _tabName;

        public TabItemViewModel(string tabName, WorkItemViewModel dataContext)
        {
            DataContext = dataContext;
            TabName = tabName;
        }

        public string TabName
        {
            get { return _tabName; }
            private set
            {
                _tabName = value;
                RaisePropertyChanged("TabName");
            }
        }

        public WorkItemViewModel DataContext
        {
            get { return _dataContext; }
            private set
            {
                _dataContext = value;
                RaisePropertyChanged("DataContext");
            }
        }
    }
}