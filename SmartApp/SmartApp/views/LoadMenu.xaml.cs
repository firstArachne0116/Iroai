using Acr.UserDialogs;
using Newtonsoft.Json;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using SmartApp.Model;
using SmartApp.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using AdMob.DependencyServices;

namespace SmartApp.views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoadMenu : PopupPage
    {
        private List<ToolbarItemModel> MenuItems { get; set; }

        private string _fullpath;
        public LoadMenu(string strpath)
        {
            InitializeComponent();
            _fullpath = strpath;
            DependencyService.Get<IAdInterstitial>().LoadAd();
            List<ToolbarItemModel> items = new List<ToolbarItemModel>();

            if (Device.RuntimePlatform == Device.iOS)
            {
                items = new List<ToolbarItemModel> {
                     new ToolbarItemModel { MenuText = "カメラ", Name = "Camera"},
                     new ToolbarItemModel { MenuText = "ライブラリ" , Name = "Library"},
                     new ToolbarItemModel { MenuText = "エクスポート" , Name = "Export"},
                     new ToolbarItemModel { MenuText = "インポート" , Name = "Import"}
             };
            }
            else if (Device.RuntimePlatform == Device.Android)
            {
                items = new List<ToolbarItemModel> {
                     new ToolbarItemModel { MenuText = "カメラ", Name = "Camera"},
                     new ToolbarItemModel { MenuText = "ライブラリ" , Name = "Library"},
                     new ToolbarItemModel { MenuText = "エクスポート" , Name = "Export"},
                     new ToolbarItemModel { MenuText = "インポート" , Name = "Import"}
             };
            }

            MenuItems = items;
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
            if (item.Name == "Camera")
            {
                await TakePhoto();
            }
            else if (item.Name == "Library")
            {
                await PickPhoto();
            }
            else if (item.Name == "Export")
            {
                await Navigation.PushModalAsync(new Export());
            }
            else if (item.Name == "Import")
            {
                await importPhoto();
            }
            CloseAllPopup();
        }

        // 撮影
        private async Task TakePhoto()
        {
            var media = CrossMedia.Current;

            // 権限チェック
            if (await CheckPermisson())
            {
                if (!media.IsCameraAvailable || !media.IsTakePhotoSupported)
                {
                    await DisplayAlert("", "カメラにアクセスできない", "確定");
                    return;
                }

                var file = await media.TakePhotoAsync(new StoreCameraMediaOptions());

                if (file == null)
                    return;

                ImageDetail imageDetail;
                if (App.srcPage != null)
                {
                    imageDetail = (ImageDetail)App.srcPage;
                    imageDetail.setImageDetail(file, -1);
                    imageDetail.reflash();
                }
                else
                {
                    imageDetail = new ImageDetail(file, -1);

                }

                App.Current.MainPage = imageDetail;
            }
        }

        private async Task PickPhoto()
        {
            var media = CrossMedia.Current;

            if (await CheckPermisson())
            {
                if (!media.IsPickPhotoSupported)
                {
                    await DisplayAlert("", "アルバムにアクセスする権限はありません", "確定");
                    return;
                }

                var file = await media.PickPhotoAsync();

                if (file == null)
                    return;

                ImageDetail imageDetail;
                if (App.srcPage != null)
                {
                    imageDetail = (ImageDetail)App.srcPage;
                    imageDetail.setImageDetail(file, -1);
                    imageDetail.reflash();
                }
                else
                {
                    imageDetail = new ImageDetail(file, -1);

                }

                App.Current.MainPage = imageDetail;
            }
        }

        private async Task importPhoto()
        {
            try
            {
                string[] fileType = {"application/zip" };
                SmartApp.FilePicker.Abstractions.FileData fileData = await SmartApp.FilePicker.CrossFilePicker.Current.PickFile(fileType);
                if (fileData == null)
                {
                    return; // user canceled file picking
                }
                DependencyService.Get<IAdInterstitial>().ShowAd();
                // do import
                // 1. get json and png(byte[])
                string pickedData = "";
                MemoryStream imageData = new MemoryStream();
                using (ZipArchive archive = new ZipArchive(fileData.GetStream(), ZipArchiveMode.Read))
                {
                    var entry1 = archive.GetEntry("data.json");
                    using (StreamReader reader = new StreamReader(entry1.Open()))
                    {
                        pickedData = reader.ReadLine();
                    }

                    var entry2 = archive.GetEntry("image.png");
                    using (Stream reader = entry2.Open())
                    {
                        await entry2.Open().CopyToAsync(imageData);
                        //await reader.CopyToAsync(imageData);
                    }
                }
                //2. json -> colorList
                ExportCSV importInfo = new ExportCSV();
                importInfo = JsonConvert.DeserializeObject<ExportCSV>(pickedData);
                List<ImageColorInfo> ImageColorInfoList = new List<ImageColorInfo>();
                foreach (ExportColor color in importInfo.ColorList)
                {
                    ImageColorInfo inColor = new ImageColorInfo();

                    inColor.XValue = color.XValue;
                    inColor.YValue = color.YValue;
                    inColor.RGBValue = color.RGBValue;
                    inColor.HEXValue = color.HEXValue;
                    inColor.HSLValue = color.HSLValue;
                    inColor.HSVValue = color.HSVValue;
                    inColor.CMYKValue = color.CMYKValue;
                    inColor.LABValue = color.LABValue;
                    inColor.MUNSELLValue = color.MUNSELLValue;
                    inColor.PCCSValue = color.PCCSValue;
                    inColor.JISValue = color.JISValue;
                    inColor.ScaledRatio = color.ScaledRatio;
                    inColor.XPis = color.XPis;
                    inColor.YPis = color.YPis;
                    inColor.DefX = color.DefX;
                    inColor.DefY = color.DefY;
                    ImageColorInfoList.Add(inColor);
                }
                
                //3. move to imagedetail page
                using (UserDialogs.Instance.Loading("Loading", null, null, true, MaskType.Black))
                {
                    ImageDetail imageDetail;
                    if (App.srcPage != null)
                    {
                        imageDetail = (ImageDetail)App.srcPage;
                        imageDetail.setImageDetail(imageData.ToArray(), ImageColorInfoList, "", "");
                        imageDetail.reload();
                    }
                    else
                    {
                        imageDetail = new ImageDetail(imageData.ToArray(), ImageColorInfoList, "", "");

                    }

                    App.Current.MainPage = imageDetail;

                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("Exception choosing file: " + ex.ToString());
            }
        }

        // 権限チェック
        private async Task<bool> CheckPermisson()
        {
            var permissons = CrossPermissions.Current;

            // Storage権限チェック
            var storageStatus = await permissons.CheckPermissionStatusAsync(Permission.Storage);

            if (storageStatus != PermissionStatus.Granted)
            {
                var results = await permissons.RequestPermissionsAsync(Permission.Storage);
                storageStatus = results[Permission.Storage];
            }

            return storageStatus == PermissionStatus.Granted;
        }
    }
}