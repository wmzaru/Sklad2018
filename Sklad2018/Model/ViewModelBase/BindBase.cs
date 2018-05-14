using System.ComponentModel;
using System.Runtime.CompilerServices;
using Sklad2018.Annotations;

namespace Sklad2018.Model.ViewModelBase
{
    public class BindBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void RaicePropertyCahnged([CallerMemberName] string property = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        protected virtual void SetProperty<T>(ref T member, T val, [CallerMemberName] string propertyName = null)
        {
            if (object.Equals(member, val)) return;
            member = val;
            RaicePropertyCahnged(propertyName);
        }
    }
}