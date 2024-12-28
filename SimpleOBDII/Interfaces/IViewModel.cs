

using OS.OBDII.Models;
using OS.OBDII.ViewModels;

namespace OS.OBDII.Interfaces
{
    public interface IViewModel
    {
        void Back();
        void Start();
        void Stop();
        bool IsBusy { get; set; }
        void CloseCommService();
        event ViewModelEvent ModelEvent;
        event RequestPopup NeedYesNoPopup;

    }
    public interface IEditableViewModel : IViewModel
    {
        void Edit(object editObject);
    }
}
