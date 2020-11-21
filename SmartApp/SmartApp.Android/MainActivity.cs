using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Plugin.CurrentActivity;
using Plugin.Permissions;
using Acr.UserDialogs;
using Xamarin.Forms;

namespace SmartApp.Droid
{
    [Activity(Label = "SmartApp", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);           

            // 初始化 Current Activity Plugin
            CrossCurrentActivity.Current.Init(this, savedInstanceState);
            UserDialogs.Init(this);
            Rg.Plugins.Popup.Popup.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

            App.ScreenWidth = (int)Resources.DisplayMetrics.WidthPixels; // real pixels 
            App.ScreenHeight = (int)Resources.DisplayMetrics.HeightPixels; // real pixels 

            App.ScreenWidth = (int)(Resources.DisplayMetrics.WidthPixels / Resources.DisplayMetrics.Density); // device independent pixels 
            App.ScreenHeight = (int)(Resources.DisplayMetrics.HeightPixels / Resources.DisplayMetrics.Density); // device independent pixels
            DependencyService.Register<ScreenshotService>();


            DependencyService.Get<ScreenshotService>().SetActivity(this);


            //DependencyService.Register<AdmobView>();
            //DependencyService.Get<AdmobView>().Init();

            LoadApplication(new App());
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Android.Content.PM.Permission[] grantResults)
        {
            var current = PermissionsImplementation.Current;

            current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}