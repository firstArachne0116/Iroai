using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using SkiaSharp;
using SmartApp.Interface;
using SmartApp.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SmartApp.views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SMSMenu : PopupPage
    {
        public SMSMenu()
        {
            InitializeComponent();            
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }

        protected override void OnAppearingAnimationBegin()
        {
            base.OnAppearingAnimationBegin();
        }

        private void OnCloseButtonTapped(object sender, EventArgs e)
        {
            CloseAllPopup();
        }

        protected override bool OnBackgroundClicked()
        {
            CloseAllPopup();
            return false;
        }

        private async void CloseAllPopup()
        {
            await PopupNavigation.Instance.PopAllAsync();
        }
    }
}