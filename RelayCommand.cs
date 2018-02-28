using System;
using System.Windows.Input;

namespace TestMahApps
{
    public class RelayCommand : ICommand
    {
        private Predicate<object> canExecuteEvaluator;
        private Action<object> methodToExecute;

        public RelayCommand(Action<object> methodToExecute, Predicate<object> canExecute)
        {
            if (methodToExecute == null) throw new ArgumentNullException("execute");

            this.methodToExecute = methodToExecute;
            canExecuteEvaluator = canExecute;
        }

        public RelayCommand(Action<object> methodToExecute)
            : this(methodToExecute, null) { }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            if (canExecuteEvaluator != null)
                return canExecuteEvaluator(parameter);
            return true; // if there is no can execute default to true
        }

        public void Execute(object parameter)
        {
            this.methodToExecute.Invoke(parameter);
        }
    }

    public class RelayCommand<T> : ICommand
    {
        private Predicate<T> canExecuteEvaluator;
        private Action<T> methodToExecute;

        public RelayCommand(Action<T> methodToExecute, Predicate<T> canExecute)
        {
            if (methodToExecute == null) throw new ArgumentNullException("execute");

            this.methodToExecute = methodToExecute;
            canExecuteEvaluator = canExecute;
        }

        public RelayCommand(Action<T> methodToExecute)
            : this(methodToExecute, null) { }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            if (canExecuteEvaluator != null)
                return canExecuteEvaluator((T)parameter);
            return true; // if there is no can execute default to true
        }

        public void Execute(object parameter)
        {
            this.methodToExecute.Invoke((T)parameter);
        }
    }
}
