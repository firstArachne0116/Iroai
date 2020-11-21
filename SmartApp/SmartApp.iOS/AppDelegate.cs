using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using Photos;
using UIKit;
using SmartApp.iOS;
using Xamarin.Forms;

namespace SmartApp.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            Rg.Plugins.Popup.Popup.Init();
            global::Xamarin.Forms.Forms.Init();
            LoadApplication(new App());

            PHPhotoLibrary.RequestAuthorization(status =>
            {
                switch (status)
                {
                    case PHAuthorizationStatus.Authorized:
                        // Add code do run if user authorized permission, if needed.
                        break;
                    case PHAuthorizationStatus.Denied:
                        // Add code do run if user denied permission, if needed.
                        break;
                    case PHAuthorizationStatus.Restricted:
                        // Add code do run if user restricted permission, if needed.
                        break;
                    default:
                        break;
                }
            });

            App.ScreenWidth = (int)UIScreen.MainScreen.Bounds.Width;
            App.ScreenHeight = (int)UIScreen.MainScreen.Bounds.Height;

            return base.FinishedLaunching(app, options);
        }

        [Export("window")]
        public UIWindow GetWindow()
        {
            UIWindow returnWindow = null;

            foreach (UIWindow v in UIApplication.SharedApplication.Windows)
            {
                if (v != null)
                {
                    returnWindow = v;
                    break;
                }

            }

            if (returnWindow == null)
            {
                returnWindow = UIApplication.SharedApplication.KeyWindow;

            }

            return returnWindow;
        }
    }
}
