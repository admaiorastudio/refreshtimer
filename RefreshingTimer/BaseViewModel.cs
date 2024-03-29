﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace RefreshingTimer
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        #region Costants and Fields

        private bool _isBusy;
        
        private Dictionary<string, ICommand> _commands;

        #endregion

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Properties

        public bool IsBusy
        {
            get => _isBusy;
            set { SetProperty(ref _isBusy, value); }
        }        


        #endregion

        #region Constructors

        public BaseViewModel()
        {
            _commands = new Dictionary<string, ICommand>();
        }

        #endregion

        #region Event Raising Methods

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (propertyName == null)
                return;

            if (propertyName == String.Empty)
                return;

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region Methods

        protected bool SetProperty<T>(ref T backingStore, T value,
            string chainedPropertyName = "",
            [CallerMemberName]string propertyName = "",
            Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            OnPropertyChanged(chainedPropertyName);            
            return true;
        }

        protected ICommand RegisterCommand(Action execute, [CallerMemberName]string commandId = "")
        {
            if(!_commands.ContainsKey(commandId))
                _commands[commandId] = new Command(execute);

            return _commands[commandId];
        }

        protected ICommand RegisterCommand(Action<object> execute, [CallerMemberName]string commandId = "")
        {
            if (!_commands.ContainsKey(commandId))
                _commands[commandId] = new Command(execute);

            return _commands[commandId];
        }

        protected ICommand RegisterCommandTask(Func<Task> execute, [CallerMemberName]string commandId = "")
        {
            if (!_commands.ContainsKey(commandId))
                _commands[commandId] = new Command(async () => await execute());

            return _commands[commandId];
        }

        #endregion
    }    
}
