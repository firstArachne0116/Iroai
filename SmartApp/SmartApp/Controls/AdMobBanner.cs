using System;
using Xamarin.Forms;

namespace SmartApp.Controls
{
    public class AdMobBanner : ContentView
    {
        public AdMobBanner()
        {
        }  
    }


}

namespace AdMob.DependencyServices
{
    public interface IAdInterstitial
    {
        void LoadAd();
        void ShowAd();
    }
}
