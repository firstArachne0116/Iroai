using Acr.UserDialogs;
using Plugin.Media;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Rg.Plugins.Popup.Extensions;
using SmartApp.Interface;
using SmartApp.Model;
using SmartApp.Util;
using SmartApp.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using AdMob.DependencyServices;
using SmartApp.Controls;

namespace SmartApp.views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Export : ContentPage
    {
        // private bool adshow = false;
        public Export()
        {
            InitializeComponent();
            DependencyService.Get<IAdInterstitial>().LoadAd();
            MyListView.ItemTapped += MyListView_ItemTapped;
            RetrunName.Text = "戻る";
            GetImageInfoAsync();
        }

        private void btn_Return(object sender, EventArgs e)
        {
            // if (adshow)
            // {
            //     DependencyService.Get<IAdInterstitial>().ShowAd();
            // }
            Navigation.PopModalAsync();
        }

        private async void btnHelp_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushPopupAsync(new TopMenu());
        }
        public async void GetImageInfoAsync()
        {
            List<ImageColorInfo> ImageColorList = await SqliteUtil.Current.QueryImageColorInfo();

            if (ImageColorList == null)
                return;

            if (ImageColorList.Count == 0)
            {
                // await DisplayAlert("提示", "インメージが無し、ご確定してください。", "確定");
            }

            List<NewsListItemModel> NewsListItemModelLists = new List<NewsListItemModel>();

            var imageIdGroupList = ImageColorList
                    .GroupBy(x => new { x.ImageID })
                    .Select(group => new
                    {
                        Keys = group.Key
                    }).ToList();

            foreach (var item in imageIdGroupList)
            {
                int imageId = item.Keys.ImageID;
                List<ImageColorInfo> ImageColorListByImageID = await SqliteUtil.Current.QueryImageColorInfoByImageID(Convert.ToInt32(imageId));
                List<IamgeSaveInfo> IamgeSaveInfoList = await SqliteUtil.Current.QueryIamgeSaveInfoByImageID(imageId);
                NewsListItemModel newsListItemModel = new NewsListItemModel();

                var imagePathGroupList = IamgeSaveInfoList
                    .GroupBy(x => new { x.ImagePath, x.CateLogId, x.Imagebuffer })
                    .Select(group => new
                    {
                        Keys = group.Key
                    }).ToList();
                if (imagePathGroupList.Count == 0)
                {
                    await DisplayAlert("", "インメージ画が無し、ご確定してください。", "確定");
                    await Navigation.PushModalAsync(new MainPage());
                    return;
                }
                newsListItemModel.ImageSrc = imagePathGroupList[imagePathGroupList.Count - 1].Keys.ImagePath;
                newsListItemModel.Imagebyte = imagePathGroupList[imagePathGroupList.Count - 1].Keys.Imagebuffer;
                int iCateLogId = imagePathGroupList[imagePathGroupList.Count - 1].Keys.CateLogId;
                List<CataLogInfo> CateLogIdList = await SqliteUtil.Current.QueryCataLogInfoByCateLogId(iCateLogId);
                var imageTitleGroupList = CateLogIdList
                    .GroupBy(x => new { x.CateLogId, x.Title, x.Folder })
                    .Select(group => new
                    {
                        Keys = group.Key
                    }).ToList();

                if (imageTitleGroupList.Count == 0)
                {
                    await DisplayAlert("", "インメージタイトルが無し、ご確定してください。", "確定");
                    await Navigation.PushModalAsync(new MainPage());
                    return;
                }

                //newsListItemModel.ImageSrc = "picDemo.PNG";
                //newsListItemModel.Title = "風景タイトル";

                newsListItemModel.Title = imageTitleGroupList[imageTitleGroupList.Count - 1].Keys.Title.ToString();
                newsListItemModel.CateLogId = (int)imageTitleGroupList[imageTitleGroupList.Count - 1].Keys.CateLogId;
                string strFolderName = imageTitleGroupList[imageTitleGroupList.Count - 1].Keys.Folder.ToString();
                //SaveListName.Text = strFolderName;

                newsListItemModel.pickcolor01 = "Transparent";
                newsListItemModel.pickcolor02 = "Transparent";
                newsListItemModel.pickcolor03 = "Transparent";
                newsListItemModel.pickcolor04 = "Transparent";
                newsListItemModel.pickcolor05 = "Transparent";
                newsListItemModel.pickcolor06 = "Transparent";
                newsListItemModel.pickcolor07 = "Transparent";
                newsListItemModel.pickcolor08 = "Transparent";
                newsListItemModel.pickcolor09 = "Transparent";
                newsListItemModel.pickcolor10 = "Transparent";

                for (int i = 0; i < ImageColorListByImageID.Count; i++)
                {
                    switch (i)
                    {
                        case 0:
                            newsListItemModel.pickcolor01 = ImageColorListByImageID[i].HEXValue;
                            break;
                        case 1:
                            newsListItemModel.pickcolor02 = ImageColorListByImageID[i].HEXValue;
                            break;
                        case 2:
                            newsListItemModel.pickcolor03 = ImageColorListByImageID[i].HEXValue;
                            break;
                        case 3:
                            newsListItemModel.pickcolor04 = ImageColorListByImageID[i].HEXValue;
                            break;
                        case 4:
                            newsListItemModel.pickcolor05 = ImageColorListByImageID[i].HEXValue;
                            break;
                        case 5:
                            newsListItemModel.pickcolor06 = ImageColorListByImageID[i].HEXValue;
                            break;
                        case 6:
                            newsListItemModel.pickcolor07 = ImageColorListByImageID[i].HEXValue;
                            break;
                        case 7:
                            newsListItemModel.pickcolor08 = ImageColorListByImageID[i].HEXValue;
                            break;
                        case 8:
                            newsListItemModel.pickcolor09 = ImageColorListByImageID[i].HEXValue;
                            break;
                        case 9:
                            newsListItemModel.pickcolor10 = ImageColorListByImageID[i].HEXValue;
                            break;
                    }
                }

                NewsListItemModelLists.Add(newsListItemModel);
            }

            using (UserDialogs.Instance.Loading("Loading", null, null, true, MaskType.Black))
            {
                await Task.Delay(1000);
                MyListView.ItemsSource = NewsListItemModelLists;
            }
        }

        public async void MyListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {

            if (Device.RuntimePlatform == Device.Android)
            {
                string answer = await DisplayActionSheet("色彩情報をエクスポートしますか？", "No", null, "端末へ保存", "クラウドへ保存");
                // adshow = false;
                if (answer == "端末へ保存")
                {
                    await ExportImg(1);
                    DependencyService.Get<IAdInterstitial>().ShowAd();
                    // adshow = true;

                }
                else if (answer == "クラウドへ保存")
                {
                    await ExportImg(2);
                }

            }
            else
            {
                Func<int, Task> action1 = AlertNoPermitionMsg;
                Func<int, Task> action2 = ExportImg;

                await DependencyService.Get<IAlertEx>().AlertMsg("色彩情報をエクスポートしますか？", "", "端末へ保存", "クラウドへ保存", "NO", action1, action2);

            }

        }

        public async Task AlertNoPermitionMsg(int i)
        {
            await DisplayAlert("", "ローカルディレクトリのアクセス権限がありません。", "OK");
        }


        public async Task ExportImg(int i)
        {
            int _cateLogId = (MyListView.SelectedItem as NewsListItemModel).CateLogId;
            List<IamgeSaveInfo> IamgeSaveInfoList = await SqliteUtil.Current.QueryIamgeSaveInfoByCateLogId(_cateLogId);
            var imageIdGroupList = IamgeSaveInfoList
                    .GroupBy(x => new { x.ImageID, x.CateLogId, x.Imagebuffer })
                    .Select(group => new
                    {
                        Keys = group.Key
                    }).ToList();

            List<CataLogInfo> cataloginfo = await SqliteUtil.Current.QueryCataLogInfoByCateLogId(imageIdGroupList[0].Keys.CateLogId);

            List<ImageColorInfo> ImageColorInfoList = await SqliteUtil.Current.QueryImageColorInfoByImageID(imageIdGroupList[0].Keys.ImageID);

            ExportCSV exportinof = new ExportCSV();

            exportinof.Title = cataloginfo[0].Title;
            exportinof.Folder = cataloginfo[0].Folder;
            exportinof.ImagePath = IamgeSaveInfoList[0].ImagePath;

            exportinof.ColorList = new List<ExportColor>();

            foreach (ImageColorInfo color in ImageColorInfoList)
            {
                ExportColor exportcolor = new ExportColor();

                exportcolor.XValue = color.XValue;
                exportcolor.YValue = color.YValue;
                exportcolor.RGBValue = color.RGBValue;
                exportcolor.HEXValue = color.HEXValue;
                exportcolor.HSLValue = color.HSLValue;
                exportcolor.HSVValue = color.HSVValue;
                exportcolor.CMYKValue = color.CMYKValue;
                exportcolor.LABValue = color.LABValue;
                exportcolor.MUNSELLValue = color.MUNSELLValue;
                exportcolor.PCCSValue = color.PCCSValue;
                exportcolor.JISValue = color.JISValue;
                exportcolor.ScaledRatio = color.ScaledRatio;
                exportcolor.XPis = color.XPis;
                exportcolor.YPis = color.YPis;
                exportcolor.DefX = color.DefX;
                exportcolor.DefY = color.DefY;

                exportinof.ColorList.Add(exportcolor);
            }

            var json = JsonConvert.SerializeObject(exportinof);

            string strreturnpath = string.Empty;

            if (i == 2)
            {
                await DependencyService.Get<IPhotoLibrary>().ShareAsync(exportinof.Title, json, System.Convert.FromBase64String(imageIdGroupList[0].Keys.Imagebuffer));
            }
            else
            {
                int result = await DependencyService.Get<IPhotoLibrary>().SaveAsync(exportinof.Title, json, System.Convert.FromBase64String(imageIdGroupList[0].Keys.Imagebuffer));
                
            }

        }
    }
}