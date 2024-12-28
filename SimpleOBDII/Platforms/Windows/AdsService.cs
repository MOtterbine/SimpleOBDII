using OS.OBDII.Interfaces;

namespace OS.OBDII.PartialClasses;

public partial class AdService : IAdService
{

    public AdService()
    {
        if (this.AdsAreActive)
        {
            // Init ads..
        }
    }
    public bool AdsAreActive => OS.OBDII.Constants.ADS_ARE_ACTIVE;
    public bool IsSpecialEdition => OS.OBDII.Constants.SPECIAL_EDITION;

    public Task PostAdTask { get; set; }

    protected int AdTripCount = 0;

    public string BannerAdUnitId { get; set; } = "";

    public bool DoAdPopup(bool force = false)
    {
        return false;
    }

    private ContentView GetContent()
    {
        if (this.AdsAreActive) return new OS.OBDII.Controls.ExampleBannerAd();
        // Return empty placeholder if ads are off
        return new ContentView(); 
    }

    public ContentView BannerAd => GetContent();

    public event EventHandler PopupClosed;

}