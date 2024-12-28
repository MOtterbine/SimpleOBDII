using System;
using System.Threading.Tasks;

namespace OS.OBDII.Interfaces
{
    public interface IAdService
    {

        /// <summary>
        /// How many actions, clicks etc to allow before showing popup ad
        /// </summary>
        // int AdTripCount { get; }
        /// <summary>
        /// Enables or disables ads
        /// </summary>
        bool IsSpecialEdition { get; }
        /// <summary>
        /// Enables or disables ads
        /// </summary>
        bool AdsAreActive { get; }
        string BannerAdUnitId { get; }
        /// <summary>
        /// XAML for ad
        /// </summary>
        ContentView BannerAd { get; }
        /// <summary>
        /// Action to carry out once the user closes ad popup
        /// </summary>
        Task PostAdTask { get; set; }

        event EventHandler PopupClosed;
        /// <summary>
        /// Starts an interstitial ad (full page popup) that shall call PopupClosed when user closes
        /// </summary>
        /// <param name="force">Calls the popup and resets the trip count</param>
        bool DoAdPopup(bool force = false);


    }
}
