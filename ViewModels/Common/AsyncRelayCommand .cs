using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Butler
{
    public interface IAsyncRelayCommand : ICommand
    {
        Task ExecuteAsync(object parameter);
    }

    public interface IAsyncRelayCommand<T> : ICommand
    {
        Task ExecuteAsync(T parameter);
    }

    public class AsyncRelayCommand : IAsyncRelayCommand
    {
        private readonly Func<bool> _canExecute;
        private readonly Action _execute;

        public AsyncRelayCommand(Action execute)
            : this(execute, () => true)
        {
        }

        public AsyncRelayCommand(Action execute, Func<bool> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");

            _execute = execute;
            _canExecute = canExecute;
        }

        #region IAsyncRelayCommand

        #region ICommand

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute();
        }

        public async void Execute(object parameter)
        {
            if (!CanExecute(parameter))
                return;
            await ExecuteAsync(parameter);
        }

        #endregion

        public async Task ExecuteAsync(object parameter)
        {
            await Task.Run(_execute);
        }

        #endregion
    }

    public class AsyncRelayCommand<T> : IAsyncRelayCommand<T>
    {
        private readonly Func<T, bool> _canExecute;
        private readonly Action<T> _execute;

        public AsyncRelayCommand(Action<T> execute)
            : this(execute, obj => true)
        {
        }

        public AsyncRelayCommand(Action<T> execute, Func<T, bool> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");

            _execute = execute;
            _canExecute = canExecute;
        }

        public async Task ExecuteAsync(T parameter)
        {
            await Task.Run(() => _execute(parameter));
        }

        #region ICommand

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute((T) parameter);
        }

        public async void Execute(object parameter)
        {
            var val = parameter;

            if (parameter != null && parameter.GetType() != typeof (T) && parameter is IConvertible)
                val = Convert.ChangeType(parameter, typeof (T), null);

            if (!CanExecute(val))
                return;
            T value;
            if (val == null)
                value = default(T);
            else
                value = (T) val;
            await ExecuteAsync(value);
        }

        #endregion
    }
}