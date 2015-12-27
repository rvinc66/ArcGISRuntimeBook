using System;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;

namespace MvvmHelper
{
    public class RelayCommandAsync : ICommand
    {
        private readonly RelayCommand _internalCommand;

        private readonly Func<Task> _executeMethod;

        public event EventHandler CanExecuteChanged
        {
            add { _internalCommand.CanExecuteChanged += value; }
            remove { _internalCommand.CanExecuteChanged -= value; }
        }

        public RelayCommandAsync(Func<Task> executeMethod, Func<bool> canExecuteMethod)
        {
            if (executeMethod == null)
            {
                throw new ArgumentNullException("executeMethod");
            }

            _executeMethod = executeMethod;
            _internalCommand = new RelayCommand(() => { }, canExecuteMethod);
        }

        public RelayCommandAsync(Func<Task> executeMethod) : this(executeMethod, () => true) { }

        public Task ExecuteAsync(object parameter)
        {
            return _executeMethod();
        }

        void ICommand.Execute(object parameter)
        {
            ExecuteAsync(parameter);
        }

        public bool CanExecute(object parameter)
        {
            return _internalCommand.CanExecute(parameter);
        }

        public void RaiseCanExecuteChanged()
        {
            _internalCommand.RaiseCanExecuteChanged();
        }
    }

    public class RelayCommandAsync<T> : ICommand
    {
        private readonly RelayCommand<T> _internalCommand;
        private readonly Func<T, Task> _executeMethod;

        public event EventHandler CanExecuteChanged
        {
            add { _internalCommand.CanExecuteChanged += value; }
            remove { _internalCommand.CanExecuteChanged -= value; }
        }

        public RelayCommandAsync(Func<T, Task> executeMethod, Func<T, bool> canExecuteMethod)
        {
            if (executeMethod == null)
            {
                throw new ArgumentNullException("executeMethod");
            }

            _executeMethod = executeMethod;
            _internalCommand = new RelayCommand<T>(_ => { }, canExecuteMethod);
        }

        public RelayCommandAsync(Func<T, Task> executeMethod) : this(executeMethod, _ => true) { }

        void ICommand.Execute(object parameter)
        {
            if (parameter is T)
            {
                ExecuteAsync((T)parameter);
            }

            else throw new ArgumentException("Parameter should be of type " + typeof(T));
        }

        public Task ExecuteAsync(T parameter)
        {
            return _executeMethod(parameter);
        }

        public bool CanExecute(object parameter)
        {
            return _internalCommand.CanExecute(parameter);
        }

        public void RaiseCanExecuteChanged()
        {
            _internalCommand.RaiseCanExecuteChanged();
        }
    }
}