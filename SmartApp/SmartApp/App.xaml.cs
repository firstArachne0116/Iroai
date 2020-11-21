using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace SmartApp
{
    public partial class App : Application
    {
        public static int ScreenWidth;
        public static int ScreenHeight;

        public static ContentPage srcPage;
        public static ContentPage mainPage;

        public App()
        {
            InitializeComponent();

            MainPage = new MainPage();
            SqliteUtil.Current.CreateAllTablesAsync();
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
