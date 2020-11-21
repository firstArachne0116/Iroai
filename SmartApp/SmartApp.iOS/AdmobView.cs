using Xamarin.Forms;
using SmartApp.iOS;
using SmartApp.Interface;
using System.Threading.Tasks;
using AdMob.iOS.DependencyServices;
using AdMob.DependencyServices;
using Google.MobileAds;
using UIKit;

[assembly: Dependency(typeof(AdInterstitial_iOS))]
namespace AdMob.iOS.DependencyServices
{
    public class AdInterstitial_iOS : IAdInterstitial
    {
        Interstitial interstitial;

        public AdInterstitial_iOS()
        {
            LoadAd();
            interstitial.ScreenDismissed += (s, e) => LoadAd();
        }

        public void LoadAd()
        {
            // TODO: change this id to your admob id    
            interstitial = new Interstitial("ca-app-pub-3940256099942544/1033173712");

            var request = Request.GetDefaultRequest();
            request.TestDevices = new string[] { "ca-app-pub-3940256099942544/4411468910", "GADSimulator" };
            interstitial.LoadRequest(request);
        }

        public void ShowAd()
        {
            if (interstitial.IsReady)
            {
                var viewController = GetVisibleViewController();
                interstitial.PresentFromRootViewController(viewController);
            }
        }
        UIViewController GetVisibleViewController()
        {
            var rootController = UIApplication.SharedApplication.KeyWindow.RootViewController;

            if (rootController.PresentedViewController == null)
                return rootController;

            if (rootController.PresentedViewController is UINavigationController)
            {
                return ((UINavigationController)rootController.PresentedViewController).VisibleViewController;
            }

            if (rootController.PresentedViewController is UITabBarController)
            {
                return ((UITabBarController)rootController.PresentedViewController).SelectedViewController;
            }

            return rootController.PresentedViewController;
        }
    }
}