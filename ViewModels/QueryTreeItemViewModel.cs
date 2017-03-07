using System.Collections.ObjectModel;

namespace Butler
{
    public class QueryTreeItemViewModel : ViewModelBase
    {
        private ObservableCollection<QueryTreeItemViewModel> _children;
        private string _content;
        private string _name;

        public QueryTreeItemViewModel()
        {
            _children = new ObservableCollection<QueryTreeItemViewModel>();
            _content = "";
            _name = "";
        }

        public bool HasData
        {
            get { return !string.IsNullOrWhiteSpace(Content); }
        }

        public ObservableCollection<QueryTreeItemViewModel> Children
        {
            get { return _children; }
            set
            {
                _children = value;
                RaisePropertyChanged("Children");
            }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                RaisePropertyChanged("Name");
            }
        }

        public string Content
        {
            get { return _content; }
            set
            {
                _content = value;
                RaisePropertyChanged("Content");
                RaisePropertyChanged("HasData");
            }
        }
    }
}