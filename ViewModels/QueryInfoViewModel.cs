using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

namespace Butler
{
    public class QueryInfoViewModel : ViewModelBase
    {
        private RelayCommand<object> _addCommand;
        private bool _canMoveDown;
        private bool _canMoveUp;
        private ObservableCollection<QueryGridItemViewModel> _gridItems;
        private bool _isDirty;
        private RelayCommand<object> _moveDownCommand;
        private RelayCommand<object> _moveUpCommand;
        private RelayCommand<object> _removeCommand;
        private AsyncRelayCommand _saveCommand;
        private QueryGridItemViewModel _selectedGridItem;
        private QueryTreeItemViewModel _selectedTreeItem;
        private ObservableCollection<QueryTreeItemViewModel> _treeItems;

        public QueryInfoViewModel()
        {
            _treeItems = new ObservableCollection<QueryTreeItemViewModel>();

            Repository.Instance.AfterLoadQueryHierarchy += (sender, e) => { DataBind(); };
            GridItems =
                new ObservableCollection<QueryGridItemViewModel>(
                    QueryProvider.Instance.Data.Select(d => new QueryGridItemViewModel(d)).ToList());

            DataBind();
        }

        public ObservableCollection<QueryGridItemViewModel> GridItems
        {
            get { return _gridItems; }
            set
            {
                _gridItems = value;
                RaisePropertyChanged("GridItems");
            }
        }

        public QueryGridItemViewModel SelectedGridItem
        {
            get { return _selectedGridItem; }
            set
            {
                _selectedGridItem = value;
                RaisePropertyChanged("SelectedGridItem");
                var index = GridItems.IndexOf(_selectedGridItem);
                CanMoveUp = index > 0;
                CanMoveDown = index + 1 < GridItems.Count();
            }
        }

        public QueryTreeItemViewModel SelectedTreeItem
        {
            get { return _selectedTreeItem; }
            set
            {
                _selectedTreeItem = value;
                RaisePropertyChanged("SelectedTreeItem");
            }
        }

        public ObservableCollection<QueryTreeItemViewModel> TreeItems
        {
            get { return _treeItems; }
            set
            {
                _treeItems = value;
                RaisePropertyChanged("TreeItems");
            }
        }

        public bool IsDirty
        {
            get { return _isDirty; }
            private set
            {
                _isDirty = value;
                RaisePropertyChanged("IsDirty");
            }
        }

        public bool CanMoveUp
        {
            get { return _canMoveUp; }
            private set
            {
                _canMoveUp = value;
                RaisePropertyChanged("CanMoveUp");
            }
        }

        public bool CanMoveDown
        {
            get { return _canMoveDown; }
            private set
            {
                _canMoveDown = value;
                RaisePropertyChanged("CanMoveDown");
            }
        }

        public RelayCommand<object> AddCommand
        {
            get
            {
                if (_addCommand == null)
                {
                    _addCommand = new RelayCommand<object>(
                        obj =>
                        {
                            var item = obj as QueryTreeItemViewModel;
                            if (item != null)
                            {
                                if (!string.IsNullOrWhiteSpace(item.Content))
                                {
                                    var qvm = new QueryGridItemViewModel(item);
                                    if (!GridItems.Contains(qvm) && GridItems.Count() < 7)
                                    {
                                        IsDirty = true;
                                        GridItems.Add(qvm);
                                    }
                                }
                            }
                        });
                }

                return _addCommand;
            }
        }

        public RelayCommand<object> RemoveCommand
        {
            get
            {
                if (_removeCommand == null)
                {
                    _removeCommand = new RelayCommand<object>(
                        obj =>
                        {
                            var item = obj as QueryGridItemViewModel;
                            if (item != null)
                            {
                                var qi = GridItems.FirstOrDefault(i => i.Name == item.Name && i.CanDelete);
                                if (qi != null)
                                {
                                    IsDirty = true;
                                    GridItems.Remove(qi);
                                }
                            }
                        });
                }

                return _removeCommand;
            }
        }

        public RelayCommand<object> MoveUpCommand
        {
            get
            {
                if (_moveUpCommand == null)
                {
                    _moveUpCommand = new RelayCommand<object>(
                        obj =>
                        {
                            var item = obj as QueryGridItemViewModel;
                            if (item != null)
                            {
                                var index = GridItems.IndexOf(item);
                                GridItems.Remove(item);
                                GridItems.Insert(index - 1, item);
                                SelectedGridItem = item;
                                IsDirty = true;
                            }
                        });
                }
                return _moveUpCommand;
            }
        }

        public RelayCommand<object> MoveDownCommand
        {
            get
            {
                if (_moveDownCommand == null)
                {
                    _moveDownCommand = new RelayCommand<object>(
                        obj =>
                        {
                            var item = obj as QueryGridItemViewModel;
                            if (item != null)
                            {
                                var index = GridItems.IndexOf(item);
                                GridItems.Remove(item);
                                GridItems.Insert(index + 1, item);
                                SelectedGridItem = item;
                                IsDirty = true;
                            }
                        });
                }
                return _moveDownCommand;
            }
        }

        public AsyncRelayCommand SaveCommand
        {
            get
            {
                if (_saveCommand == null)
                {
                    _saveCommand = new AsyncRelayCommand(
                        () =>
                        {
                            IsDirty = false;
                            QueryProvider.Instance.Data = GridItems.Select(d => new QueryInfoModel(d)).ToList();
                            QueryProvider.Instance.Save();
                        });
                }

                return _saveCommand;
            }
        }

        private void DataBind()
        {
#if DEBUG
            var sw = Stopwatch.StartNew();
#endif

            TreeItems = new ObservableCollection<QueryTreeItemViewModel>(Repository.Instance.WorkItemQueries);
#if DEBUG
            sw.Stop();

            Console.WriteLine("QueryInfoViewModel.DataBind: {0}", sw.Elapsed.TotalMilliseconds);
#endif
        }
    }
}