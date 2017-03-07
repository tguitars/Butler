using System;
using System.Windows.Input;

namespace Butler
{
    public class RelayCommand<T> : ICommand
    {
        private readonly Func<T, bool> _canExecute;
        private readonly Action<T> _execute;

        public RelayCommand(Action<T> execute)
            : this(execute, obj => true)
        {
        }

        public RelayCommand(Action<T> execute, Func<T, bool> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");
            _execute = execute;
            _canExecute = canExecute;
        }

        #region ICommand Members

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute((T) parameter);
        }

        public void Execute(object parameter)
        {
            var val = parameter;
            if (parameter != null && parameter.GetType() != typeof (T) && parameter is IConvertible)
                val = Convert.ChangeType(parameter, typeof (T), null);
            if (CanExecute(val))
            {
                if (val == null)
                    _execute(default(T));
                else
                    _execute((T) val);
            }
        }

        #endregion // ICommand Members
    }

    public class RelayCommand : ICommand
    {
        private readonly Func<bool> _canExecute;
        private readonly Action _execute;

        public RelayCommand(Action execute)
            : this(execute, () => true)
        {
        }

        public RelayCommand(Action execute, Func<bool> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");
            _execute = execute;
            _canExecute = canExecute;
        }

        #region ICommand Members

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter) // parameter ignored 
        {
            return _canExecute == null || _canExecute();
        }

        public void Execute(object parameter) // parameter ignored 
        {
            if (CanExecute(parameter))
                _execute();
        }

        #endregion // ICommand Members
    }
}