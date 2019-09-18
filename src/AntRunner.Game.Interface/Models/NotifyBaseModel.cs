using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;

namespace AntRunner.Game.Interface.Models
{
    public abstract class NotifyBaseModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetValue<T>(ref T property, T value, [CallerMemberName] string name = "")
        {
            if (Equals(property, value)) return false;

            property = value;

            RaisePropertyChanged(name);
            return true;
        }

        protected void RunOnUIThread(Action action)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(action));
        }
    }
}
