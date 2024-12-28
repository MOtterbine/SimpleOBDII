using OS.OBDII.Interfaces;
using static Android.Provider.Settings;


namespace OS.OBDII.PartialClasses;

public partial class AdService : IAdService
{

    public AdService()
        {
        }
        public bool AdsAreActive => OS.OBDII.Constants.ADS_ARE_ACTIVE;
        public bool IsSpecialEdition => OS.OBDII.Constants.SPECIAL_EDITION;
        public Task PostAdTask { get; set; }

        protected int AdTripCount = 0;

        public string BannerAdUnitId => OS.OBDII.Constants.BANNER_AD_ID;

        public bool DoAdPopup(bool force = false)
        {
            return false;
        }

        private ContentView GetContent()
        {
#if ADS_SUPPORTED
        if (this.AdsAreActive)
            {
                return new OS.OBDII.Controls.AdMobView() { AdUnitId = OS.OBDII.Constants.BANNER_AD_ID };
            }
#endif
            // Return empty placeholder if ads are off
            return new ContentView();

        }

        public ContentView BannerAd => GetContent();

        public event EventHandler PopupClosed;

}