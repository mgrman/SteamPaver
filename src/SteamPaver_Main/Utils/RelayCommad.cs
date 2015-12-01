using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SteamPaver_Main
{
    public class AsyncRelayCommand : ICommand
    {
        private Func<Task> asyncExecute;
        private Task asyncExecutingTask;
        private bool singleInstance;
        
        private Func<bool> canExecute;

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public AsyncRelayCommand(Func<Task> execute, Func<bool> canExecute = null, bool singleInstance = false)
        {
            this.asyncExecute = execute;
            this.canExecute = canExecute;
            this.singleInstance = singleInstance;
        }


        public bool CanExecute(object parameter)
        {
            return (this.canExecute == null || this.canExecute()) && (!singleInstance || asyncExecutingTask==null) ;
        }

        public async void Execute(object parameter)
        {
            asyncExecutingTask = this.asyncExecute();
            await asyncExecutingTask;
            asyncExecutingTask = null;
            CommandManager.InvalidateRequerySuggested();
        }

        public static Lazy<AsyncRelayCommand> Lazy(Func<Task> execute, Func<bool> canExecute = null, bool singleInstance = false)
        {
            return new Lazy<AsyncRelayCommand>(() => new AsyncRelayCommand(execute, canExecute, singleInstance));
        }
        
    }
    public class RelayCommand : ICommand
    {
        private Action execute;
        private Func<bool> canExecute;

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }


        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return this.canExecute == null || this.canExecute();
        }

        public void Execute(object parameter)
        {
            this.execute();

        }


        public static Lazy<RelayCommand> Lazy(Action execute, Func<bool> canExecute = null)
        {
            return new Lazy<RelayCommand>(() => new RelayCommand(execute, canExecute));
        }
    }

    public class RelayCommand<T> : ICommand
    {
        private Action<T> execute;
        private Func<T, bool> canExecute;

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }


        public RelayCommand(Action<T> execute, Func<T, bool> canExecute = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            if (!(parameter is T))
                return false;

            return this.canExecute == null || this.canExecute((T)parameter);
        }

        public void Execute(object parameter)
        {
            if (!(parameter is T))
                return;

            this.execute((T)parameter);
        }
    }
}
