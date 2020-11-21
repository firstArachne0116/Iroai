using SmartApp.Controls;
using SmartApp.Controls.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Android.Content;

[assembly: ExportRenderer(typeof(AdMobBanner), typeof(AdMobBannerRenderer))]
namespace SmartApp.Controls.Droid.Renderers
{
#pragma warning disable CS0618 // 型またはメンバーが旧型式です
    public class AdMobBannerRenderer : ViewRenderer<AdMobBanner, Android.Gms.Ads.AdView>
    {

        const string adUnitID = "ca-app-pub-3940256099942544/6300978111"; // バナー

        protected override void OnElementChanged(ElementChangedEventArgs<AdMobBanner> e)
        {

            base.OnElementChanged(e);

            if (Control == null)
            {
                var adMobBanner = new Android.Gms.Ads.AdView(Forms.Context);
                adMobBanner.AdSize = Android.Gms.Ads.AdSize.Banner;
                adMobBanner.AdUnitId = adUnitID;

                var requestbuilder = new Android.Gms.Ads.AdRequest.Builder();
                adMobBanner.LoadAd(requestbuilder.Build());

                SetNativeControl(adMobBanner);
            }
        }
    }
#pragma warning restore CS0618 // 型またはメンバーが旧型式です
}
