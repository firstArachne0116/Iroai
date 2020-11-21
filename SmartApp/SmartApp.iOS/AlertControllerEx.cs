using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Foundation;
using UIKit;

using SmartApp.iOS;
using SmartApp;
using System.IO;
using SmartApp.Interface;
using System.Threading.Tasks;

[assembly: Xamarin.Forms.Dependency(typeof(AlertControllerEx))]//DirectoryAndroid
namespace SmartApp.iOS
{

    public class AlertControllerEx : UIViewController,IDisposable,IUISpringLoadedInteractionSupporting,IAlertEx
    {
        public delegate void methodAction1();
        public delegate void methodAction2();
        bool IUISpringLoadedInteractionSupporting.SpringLoaded { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string returnValue = "";

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

        public async Task AlertMsg(string msg, string title, string actionName1, string actionName2, string actionNameCancel, Func<int, Task> action1, Func<int, Task> action2)
        {
            var alertEx = UIAlertController.Create(title, msg, UIAlertControllerStyle.Alert);
            alertEx.AddAction(UIAlertAction.Create(actionNameCancel, UIAlertActionStyle.Cancel, null));
            alertEx.AddAction(UIAlertAction.Create(actionName1, UIAlertActionStyle.Default, action => { action1(1); }));
            alertEx.AddAction(UIAlertAction.Create(actionName2, UIAlertActionStyle.Default, action => { action2(1); }));

            var vc = GetVisibleViewController();
            if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
            {
                if (alertEx.PopoverPresentationController != null)
                {
                    alertEx.PopoverPresentationController.SourceView = vc.View;
                }
            }

            await  vc.PresentViewControllerAsync(alertEx, true);

    
        }
    }
}