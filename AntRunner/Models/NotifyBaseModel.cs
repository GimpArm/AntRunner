using System.ComponentModel;
using System.Runtime.CompilerServices;
using AntRunner.Annotations;

namespace AntRunner.Models
{
    public abstract class NotifyBaseModel : object, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
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
    }
}
