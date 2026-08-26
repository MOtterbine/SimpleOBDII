
namespace OS.OBDII;

public partial class AppShell : Shell
{

    public AppShell()
    {
        InitializeComponent();
    }

    public void InvokeBackPressed()
    {
        OnBackButtonPressed();
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
    }

}


