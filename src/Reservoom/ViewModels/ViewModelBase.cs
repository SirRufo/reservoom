using ReactiveUI;

namespace Reservoom.ViewModels
{
    public abstract class ViewModelBase : ReactiveObject
    {
        public virtual void Dispose() { }
    }
}
