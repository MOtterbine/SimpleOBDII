using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace OS.OBDII.ViewModels;

public delegate Task<bool> RequestPopup(string title, string prompt, bool isYesNo = true, string okText = "Ok", string cancelText = "Cancel");


public class ViewModelBase : INotifyPropertyChanged
{

    protected bool SetProperty<T>(ref T backingStore, T value,
    [CallerMemberName] string propertyName = "",
    Action onChanged = null)
    {
        if (EqualityComparer<T>.Default.Equals(backingStore, value))
            return false;

        backingStore = value;
        onChanged?.Invoke();
        OnPropertyChanged(propertyName);
        return true;
    }

    /// <summary>
    /// Disable entire screen
    /// </summary>
    public bool IsBusy
    {
        get => isBusy;
        set
        {
            SetProperty(ref isBusy, value);
            OnPropertyChanged("InputIsBlocked");
        }
    }
    protected bool isBusy = false;


    public virtual bool IsCommunicating
    {
        get { return isCommunicating; }
        set
        {
            SetProperty(ref isCommunicating, value);
            OnPropertyChanged("CanSelectUserPID");

        }
    }
    protected bool isCommunicating = false;


    public string StatusDescription
    {
        get => statusDescription;
        protected set => SetProperty(ref statusDescription, value);
    }
    private string statusDescription = string.Empty;

    public string Title
    {
        get => title;
        protected set
        {
            SetProperty(ref title, value);
        }
    }
    private string title = string.Empty;


    public void SetTitle(string title)
    {
        Title = title;
    }
    public void InvokeUpdate(string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    #region INotifyPropertyChanged
    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        var changed = PropertyChanged;
        if (changed == null)
            return;

        changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    #endregion


}
