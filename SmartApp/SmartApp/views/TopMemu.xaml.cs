using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using SmartApp.ViewModels;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Plugin.Connectivity;

namespace SmartApp.views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TopMenu : PopupPage
    {
        private List<ToolbarItemModel> MenuItems { get; set; }
        private int isConnected = 0;
        public TopMenu()
        {
            InitializeComponent();

            CrossConnectivity.Current.ConnectivityChanged += Current_ConnectivityChanged;
            var items = new List<ToolbarItemModel> {
                     new ToolbarItemModel { Image = "Help.PNG", MenuText = "iroaiについて", Name = "USER"},
                     new ToolbarItemModel { Image = "read.PNG", MenuText = "使い方" , Name = "HELP"}
                     // new ToolbarItemModel { Image = "save.PNG", MenuText = "著作権", Name = "AUTHOR" }
             };
            MenuItems = items;
        }
        private void Current_ConnectivityChanged(object sender, Plugin.Connectivity.Abstractions.ConnectivityChangedEventArgs e)
        {
            if (!e.IsConnected)
            {
                isConnected = 1;
            }
            else
            {
                isConnected = 2;
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            SecondaryToolbarListView.ItemsSource = MenuItems;
            SecondaryToolbarListView.HeightRequest = MenuItems.Count * 26;
            SecondaryToolbarListView.ItemTapped += ListView_ItemClick;
        }

        protected override void OnAppearingAnimationBegin()
        {
            base.OnAppearingAnimationBegin();
            CloseImage.Rotation = 45;
            MainLayout.RaiseChild(CloseImage);
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

        private async void ListView_ItemClick(object sender, ItemTappedEventArgs e)
        {
            ToolbarItemModel item = (ToolbarItemModel)SecondaryToolbarListView.SelectedItem;
            string url = "https://iroai.app/";
            if (item.Name == "USER")
            {
                url = "https://iroai.app/users/";
            }
            else if (item.Name == "HELP")
            {
                url = "https://iroai.app/manual/";
            }
            else if (item.Name == "AUTHOR")
            {
                url = "https://iroai.app/copyright/";
            }
            if (isConnected == 2 || isConnected == 0 && CrossConnectivity.IsSupported && CrossConnectivity.Current.IsConnected)
            {
                bool checkalert;
                checkalert = await DisplayAlert("", "アプリから移動します\nよろしいですか？", "はい", "キャンセル");
                if (checkalert)
                {
                    await Navigation.PushModalAsync(new InAppBrowserXaml(url));
                }
            }
            else
            {
                await DisplayAlert("", "オフラインのためサイトが表示されません。", "はい");
            }
            CloseAllPopup();
        }
    }
}