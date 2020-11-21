using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Foundation;
using SmartApp.iOS;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using SmartApp.Interface;
using System.Threading.Tasks;

[assembly: Dependency(typeof(ShareClass))]
namespace SmartApp.iOS
{
    public class ShareClass : IShare
    {
        // MUST BE CALLED FROM THE UI THREAD
        public async Task Show(string title, string message, string filePath)
        {
            // need custmize for share app 
            var items = new NSObject[] { NSObject.FromObject(title), NSUrl.FromFilename(filePath) };

            var activityController = new UIActivityViewController(items, null);
            var vc = GetVisibleViewController();

            NSString[] excludedActivityTypes = null;

            if (excludedActivityTypes != null && excludedActivityTypes.Length > 0)
                activityController.ExcludedActivityTypes = excludedActivityTypes;

            if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
            {
                if (activityController.PopoverPresentationController != null)
                {
                    activityController.PopoverPresentationController.SourceView = vc.View;
                }
            }
            await vc.PresentViewControllerAsync(activityController, true);
        }

        public async Task Show(string title, string message, byte[] fileArray)
        {

        }

        UIViewController GetVisibleViewController()
        {
            var rootController = UIApplication.SharedApplication.KeyWindow.RootViewController;

            while (rootController.PresentedViewController != null)
            {
                if (rootController.PresentedViewController is UINavigationController)
                {
                    return ((UINavigationController)rootController.PresentedViewController).TopViewController;
                }

                if (rootController.PresentedViewController is UITabBarController)
                {
                    return ((UITabBarController)rootController.PresentedViewController).SelectedViewController;
                }

                rootController = rootController.PresentedViewController;
            }

            return rootController;
        }

    }
}