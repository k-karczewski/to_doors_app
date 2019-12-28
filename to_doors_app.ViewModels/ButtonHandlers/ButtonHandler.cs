using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace to_doors_app.ViewModels.ButtonHandlers
{
    public class ButtonHandler : ICommand
    {
        private Action _action;
        private Func<bool> _canExecute;

        public event EventHandler CanExecuteChanged;

        public ButtonHandler(Action action, Func<bool> canExecute)
        {
            _action = action;
            _canExecute = canExecute;
        }

        public void Execute(object parameter)
        {
            _action.Invoke();
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute.Invoke();
        }

        public void InvokeCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
