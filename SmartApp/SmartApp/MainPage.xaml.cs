using Plugin.Media;
using Plugin.Media.Abstractions;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using SmartApp.views;
using SmartApp.ViewModels;
using Rg.Plugins.Popup.Services;
using Rg.Plugins.Popup.Extensions;
using SmartApp.Model;
using SmartApp.Controls;

namespace SmartApp
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            SizeChanged += MainPageSizeChanged;
            App.mainPage = this;
        }

        void MainPageSizeChanged(object sender, EventArgs e)
        {
            //logoImage.WidthRequest = 319;
            //logoImage.HeightRequest = 270;
            //image.WidthRequest = Math.Min(this.Width, 400);
            //btnCamera.WidthRequest = Math.Min(this.Width, 400);
            //btnPick.WidthRequest = Math.Min(this.Width, 400);
            //btnSave.WidthRequest = Math.Min(this.Width, 400);
        }

        private async void btnCamera_Click(object sender, EventArgs e)
        {
            await TakePhoto();
        }
        
        private async void btnPick_Click(object sender, EventArgs e)
        {
            await PickPhoto();
        }

        private async void btnStore_Click(object sender, EventArgs e)
        {
            List<ImageColorInfo> ImageColorList = await SqliteUtil.Current.QueryImageColorInfo();

            if (ImageColorList == null)
                return;

            if (ImageColorList.Count == 0)
            {
                await DisplayAlert("", "保存データはありません。", "確定");
            }
            else
            {
                StoreListDetail storeListDetail = new StoreListDetail("ＴＯＰ画面");
                // Subscribe to the event when things are updated
                storeListDetail.OperationCompeleted += StoreListDetail_OperationCompleted;
                await Navigation.PushModalAsync(storeListDetail);

            }
        }

        private void StoreListDetail_OperationCompleted(object sender, EventArgs e)
        {
            // Unsubscribe to the event to prevent memory leak
            (sender as StoreListDetail).OperationCompeleted -= StoreListDetail_OperationCompleted;

            EventArgsEx ex = (EventArgsEx)e;

            if (ex.Filepath != null)
            { 
                //Navigation.PushModalAsync(new ImageDetail(ex.byteArray, ex.inputList, ex.filepath));
                var imageDetail = new ImageDetail(ex.ByteArray, ex.InputList, ex.Filepath, ex.TitleName);
                App.Current.MainPage = imageDetail;
            }

        }


        private async void btnHelp_Clicked(object sender, EventArgs e)
        {
            // bool checkalert;
            // checkalert = await DisplayAlert("", "アプリから移動します。\nよろしいですか？", "はい", "キャンセル");
            // if (checkalert) {

            await Navigation.PushPopupAsync(new TopMenu());
            
            // }
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
