namespace Butler
{
    public class QueryGridItemViewModel : ViewModelBase
    {
        private bool _canDelete;
        private string _content;
        private string _name;

        public QueryGridItemViewModel(QueryInfoModel qim)
        {
            CanDelete = qim.CanDelete;
            Name = qim.Name;
            Content = qim.Content;
        }

        public QueryGridItemViewModel(QueryTreeItemViewModel qtivm)
        {
            CanDelete = true;
            Name = qtivm.Name;
            Content = qtivm.Content;
        }

        public string Content
        {
            get { return _content; }
            private set
            {
                _content = value;
                RaisePropertyChanged("Content");
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

        public bool CanDelete
        {
            get { return _canDelete; }
            private set
            {
                _canDelete = value;
                RaisePropertyChanged("CanDelete");
            }
        }

        public override bool Equals(object obj)
        {
            var qgivm = obj as QueryGridItemViewModel;
            if (qgivm != null)
            {
                return qgivm.Name.Equals(Name);
            }
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}