using Acr.UserDialogs;
using Plugin.Media;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Rg.Plugins.Popup.Extensions;
using SmartApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SmartApp.views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class StoreListDetail : ContentPage
    {

        public StoreListDetail()
        {
            InitializeComponent();

            MyListView.ItemTapped += MyListView_ItemTapped;

        }

        public StoreListDetail(String ReturnTitle) : this()
        {
            RetrunName.Text = ReturnTitle;
        }

        protected override void OnAppearing()
        {

            base.OnAppearing();

            GetImageInfoAsync();
        }

        // Add an event to notify when things are updated
        public event EventHandler<EventArgs> OperationCompeleted;

        protected override void OnDisappearing()
        {

            base.OnDisappearing();

            MyListView.ItemsSource = new List<NewsListItemModel>();
        }

        public async void DeleteAction_Clicked(object sender, EventArgs e)
        {
            bool confirmAsync = await UserDialogs.Instance.ConfirmAsync("レコードを削除しますが？", "ｉｒｏａｉリスト", "確定", "キャンセル");
            if (confirmAsync)
            {
                var mi = ((MenuItem)sender);
                string _ImageSrc = (mi.CommandParameter as NewsListItemModel).ImageSrc;
                int _CateLogId = (mi.CommandParameter as NewsListItemModel).CateLogId;
                List<IamgeSaveInfo> IamgeSaveInfoList = await SqliteUtil.Current.QueryIamgeSaveInfoByImagePath(_ImageSrc);
                var imageIdGroupList = IamgeSaveInfoList
                    .Where(x => x.CateLogId == _CateLogId)
                        .GroupBy(x => new { x.ImageID, x.CateLogId })
                        .Select(group => new
                        {
                            Keys = group.Key
                        }).ToList();

                int retIamgeSaveInfo = await SqliteUtil.Current.DeleteIamgeSaveInfoByImageID(imageIdGroupList[0].Keys.ImageID.ToString());
                int retImageColorInfo = await SqliteUtil.Current.DeleteImageColorInfoByImageID(imageIdGroupList[0].Keys.ImageID.ToString());
                int retCataLogInfo = await SqliteUtil.Current.DeleteCataLogInfoByCateLogId(imageIdGroupList[0].Keys.CateLogId.ToString());

                if (retIamgeSaveInfo == 1 && retImageColorInfo > 0 && retCataLogInfo == 1)
                {
                    await DisplayAlert("", "保存リストから削除しました。", "OK");
                    //refresh the data
                    GetImageInfoAsync();
                }
                else
                {
                    await DisplayAlert("", "レコード削除に失敗しました。", "OK");
                }
            }
        }

        public async void MyListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var media = CrossMedia.Current;

            // 権限チェック
            if (await CheckPermisson())
            {
                if (!media.IsTakePhotoSupported)
                {
                    await DisplayAlert("", "ファイルは読み込みできない。", "OK");
                    return;
                }

                string _ImageSrc = (MyListView.SelectedItem as NewsListItemModel).ImageSrc;
                int _CateLogId = (MyListView.SelectedItem as NewsListItemModel).CateLogId;

                List<IamgeSaveInfo> IamgeSaveInfoList = await SqliteUtil.Current.QueryIamgeSaveInfoByCateLogId(_CateLogId);
 
                var imageIdGroupList = IamgeSaveInfoList
                        .Where(x => x.CateLogId == _CateLogId)
                        .GroupBy(x => new { x.ImageID, x.CateLogId, x.Imagebuffer })
                        .Select(group => new
                        {
                            Keys = group.Key
                        }).ToList();

                List<ImageColorInfo> ImageColorInfoList = await SqliteUtil.Current.QueryImageColorInfoByImageID(imageIdGroupList[0].Keys.ImageID);
                CataLogInfo cataLogInfo = await SqliteUtil.Current.QueryCataLog(_CateLogId);

                //FileStream _infoStream = new FileStream(_ImageSrc, FileMode.Open, FileAccess.Read);

                //await Navigation.PushModalAsync(new ImageDetail(Convert.FromBase64String(IamgeSaveInfoList[0].Imagebuffer), ImageColorInfoList, _ImageSrc));
                EventArgsEx eventargs = new EventArgsEx();
                eventargs.ByteArray = Convert.FromBase64String(IamgeSaveInfoList[0].Imagebuffer);
                eventargs.InputList = ImageColorInfoList;
                eventargs.Filepath = _ImageSrc;
                if (cataLogInfo != null)
                {
                    eventargs.TitleName = cataLogInfo.Title;
                }
                await Navigation.PopModalAsync();
                OperationCompeleted?.Invoke(this, eventargs);
                

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

        private async void btnOpen_Clicked(object sender, EventArgs e)
        {
            //await Navigation.PushModalAsync(new StoreListDetail());
        }

        private async void btnRead_Clicked(object sender, EventArgs e)
        {

            await Navigation.PushPopupAsync(new LoadMenu(string.Empty));
        }

        private void btnHome_Clicked(object sender, EventArgs e)
        {
            //if (App.srcPage != null)
            //{
            //    await Navigation.PushModalAsync(App.srcPage);
            //}
            //else
            //{
            //    await Navigation.PushModalAsync(new MainPage());
            //}
            // Do something to change newObj
            //EventArgs eventargs = new EventArgsEx();

            //OperationCompeleted?.Invoke(this, eventargs);
            //Navigation.PopModalAsync();
            if (App.Current.MainPage != App.mainPage)
            {
                App.Current.MainPage = App.mainPage;
            }
            else
            {
                Navigation.PopModalAsync();
            }
            ;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
        }

        private async void btnCommon_Clicked(object sender, EventArgs e)
        {
            //await Navigation.PushPopupAsync(new SMSMenu());
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
                // await DisplayAlert("", "インメージが無し、ご確定してください。", "確定");
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
                    await DisplayAlert("", "インメージ画が無し、ご確定してください。", "OK");
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
                    await DisplayAlert("", "インメージタイトルが無し、ご確定してください。", "OK");
                    await Navigation.PushModalAsync(new MainPage());
                    return;
                }

                //newsListItemModel.ImageSrc = "picDemo.PNG";
                //newsListItemModel.Title = "風景タイトル";

                newsListItemModel.Title = imageTitleGroupList[imageTitleGroupList.Count - 1].Keys.Title.ToString();
                newsListItemModel.CateLogId = imageTitleGroupList[imageTitleGroupList.Count - 1].Keys.CateLogId;
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

        private void RetrunName_Clicked(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }
    }
}