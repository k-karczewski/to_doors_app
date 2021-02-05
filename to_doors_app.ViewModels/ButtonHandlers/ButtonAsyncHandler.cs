using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace to_doors_app.ViewModels.ButtonHandlers
{
    public class ButtonAsyncHandler : ICommand
    {
        private Func<Task> _action;
        private Func<bool> _canExecute;

        public event EventHandler CanExecuteChanged;

        public ButtonAsyncHandler(Func<Task> action, Func<bool> canExecute)
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