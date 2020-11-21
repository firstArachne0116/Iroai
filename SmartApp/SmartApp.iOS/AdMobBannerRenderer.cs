using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using UIKit;
using CoreGraphics;
using SmartApp.Controls;
using SmartApp.Controls.iOS.Renderers;
using Google.MobileAds;

[assembly: ExportRenderer(typeof(AdMobBanner), typeof(AdMobBannerRenderer))]
namespace SmartApp.Controls.iOS.Renderers
{
#pragma warning disable CS0618 // 型またはメンバーが旧型式です
    public class AdMobBannerRenderer : ViewRenderer
    {

        const string adUnitID = "ca-app-pub-3940256099942544/2934735716"; // バナー

        BannerView adMobBanner;
        bool viewOnScreen;
        UIViewController viewCtrl;

        protected override void OnElementChanged(ElementChangedEventArgs<View> e)
        {

            base.OnElementChanged(e);

            if (e.NewElement == null)
                return;

            if (e.OldElement == null)
            {

                foreach (UIWindow v in UIApplication.SharedApplication.Windows)
                {
                    if (v.RootViewController != null)
                    {
                        viewCtrl = v.RootViewController;
                        break;
                    }
                }

                //またはこれでも取得可能の場合があります。
                if (viewCtrl == null)
                {
                    viewCtrl = UIApplication.SharedApplication.KeyWindow.RootViewController;
                }

                adMobBanner = new BannerView(AdSizeCons.Banner, new CGPoint(-10, 0))
                {

                    AdUnitID = adUnitID,
                    RootViewController = viewCtrl
                };

                adMobBanner.AdReceived += (sender, args) =>
                {
                    if (!viewOnScreen) AddSubview(adMobBanner);
                    viewOnScreen = true;
                };

                adMobBanner.LoadRequest(Request.GetDefaultRequest());
                SetNativeControl(adMobBanner);
            }
        }
    }
#pragma warning restore CS0618 // 型またはメンバーが旧型式です
}