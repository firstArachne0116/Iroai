using Xamarin.Forms;
using SmartApp.Droid;
using SmartApp.Interface;
using Android.Gms.Ads;
using System.Threading.Tasks;
using Android.Widget;
using AdMob.DependencyServices;
using AdMob.Droid.DependencyServices;

[assembly: Dependency(typeof(AdInterstitial_Droid))]  
namespace AdMob.Droid.DependencyServices
{
    public class AdInterstitial_Droid : IAdInterstitial
    {
        InterstitialAd interstitialAd;

        public AdInterstitial_Droid()
        {
            interstitialAd = new InterstitialAd(Android.App.Application.Context);

            // TODO: change this id to your admob id  
            interstitialAd.AdUnitId = "ca-app-pub-3940256099942544/1033173712";
            LoadAd();
        }

        public void LoadAd()
        {
            var requestbuilder = new AdRequest.Builder();
            interstitialAd.LoadAd(requestbuilder.Build());
        }

        public void ShowAd()
        {
            if (interstitialAd.IsLoaded)
                interstitialAd.Show();

            LoadAd();
        }
    }
}
