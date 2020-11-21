using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SmartApp.views
{
	public partial class InAppBrowserXaml : ContentPage
    {
        public InAppBrowserXaml(string URL)
        {
            InitializeComponent();
            webView.Source = URL;
        }
    }
}