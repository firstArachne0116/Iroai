using Acr.UserDialogs;
using Microsoft.AppCenter.Crashes;
using Rg.Plugins.Popup.Extensions;
using SmartApp.common;
using SmartApp.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SmartApp.views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MainStoreList : ContentPage
	{
        ArrayList arrayList = new ArrayList();

        private string strFolderName;

        private string _ImagePath;
        //保存1，打开0
        private int _flag;

        List<ImageColorInfo> _listinfo = new List<ImageColorInfo>();
        //public int _imageId;
        private ObservableCollection<string> Items { get; set; }

        public MainStoreList()
		{
			InitializeComponent();
            Items = Global_Variables.StoreFolderList;
            MyListView.ItemsSource = Items;
        }

        public MainStoreList(int flag) : this()
        {
            _flag = flag;
        }

        public MainStoreList(List<ImageColorInfo> listinfo, int flag, string imagePath) : this()
        {
            arrayList.AddRange(listinfo);
            lock (arrayList.SyncRoot)
            {
                for (int k = 0; k < arrayList.Count; k++)
                {
                    _listinfo.Add(arrayList[k] as ImageColorInfo);
                }

                _flag = flag;
                _ImagePath = imagePath;
            }
        }

        public async void OnSaveTitle(string title)
        {
            try
            {
                int maxmaxImageId = -1;

                //search the max imageId from table [ImageColorInfo] 
                List<ImageColorInfo> ImageIdList = await SqliteUtil.Current.QueryImageColorInfo();
                if (ImageIdList == null)
                    return;

                if (ImageIdList.Count > 0)
                {
                    maxmaxImageId = ImageIdList[ImageIdList.Count - 1].ImageID;
                }

                maxmaxImageId++;

                foreach (var imageColorItem in _listinfo)
                {
                    ImageColorInfo imageColorInfo = new ImageColorInfo();
                    imageColorInfo.ImageID = maxmaxImageId;
                    imageColorInfo.HEXValue = imageColorItem.HEXValue;
                    imageColorInfo.RGBValue = imageColorItem.RGBValue;
                    imageColorInfo.HSLValue = imageColorItem.HSLValue;
                    imageColorInfo.HSVValue = imageColorItem.HSVValue;
                    imageColorInfo.CMYKValue = imageColorItem.CMYKValue;
                    imageColorInfo.LABValue = imageColorItem.LABValue;
                    imageColorInfo.MUNSELLValue = imageColorItem.MUNSELLValue;
                    imageColorInfo.PCCSValue = imageColorItem.PCCSValue;
                    imageColorInfo.XValue = imageColorItem.XValue;
                    imageColorInfo.YValue = imageColorItem.YValue;
                    int retImageColorInfo = await SqliteUtil.Current.InsertImageColorInfo(imageColorInfo);
                }

                //insert into table [IamgeSaveInfo] 
                IamgeSaveInfo imageinfo = new IamgeSaveInfo();
                //int _imageId = _listinfo[0].ImageID;
                imageinfo.ImageID = maxmaxImageId;
                imageinfo.ImagePath = _ImagePath;
                imageinfo.Createdatetime = DateTime.Now;
                int retIamgeSaveInfo = await SqliteUtil.Current.InsertImageInfo(imageinfo);

                List<IamgeSaveInfo> IamgeSaveInfoList = await SqliteUtil.Current.QueryIamgeSaveInfoByImageID(maxmaxImageId);
                var CateLogIdGroupList = IamgeSaveInfoList
                    .GroupBy(x => new { x.CateLogId })
                    .Select(group => new
                    {
                        Keys = group.Key
                    }).ToList();

                List<CataLogInfo> cataList = await SqliteUtil.Current.QueryCataLogsByFolder(strFolderName);
                var CateLogIdTitleGroupList = cataList
                    .GroupBy(x => new { x.Title })
                    .Select(group => new
                    {
                        Keys = group.Key
                    }).ToList();

                //foreach (var title in CateLogIdTitleGroupList)
                //{
                //    if (strTitleName == title.Keys.Title)
                //    {
                //        await DisplayAlert("提示", "同じ保存タイトル名、もう一度選択してください。", "確定");
                //        await Navigation.PushModalAsync(new MainStoreList(List < ImageColorInfo > listinfo, int flag, string imagePath));
                //        return;
                //    }
                //}

                CataLogInfo catalog = new CataLogInfo();
                catalog.CateLogId = CateLogIdGroupList[CateLogIdGroupList.Count - 1].Keys.CateLogId;
                catalog.Folder = strFolderName;
                catalog.Title = title;
                catalog.CreateDatetime = DateTime.Now;
                catalog.Description = "風景";

                int ret = await SqliteUtil.Current.InsertCataLog(catalog);
                if (ret == 1)
                {
                    await DisplayAlert("", "保存しました。", "確定");
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        public async void MyListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            strFolderName = MyListView.SelectedItem.ToString();

            if (_flag == 1) 
            {
                PromptResult promptResult = await UserDialogs.Instance.PromptAsync("保存タイトル名", "ｉｒｏａｉリスト", "保存", "キャンセル", "", InputType.Name);
                if (promptResult.Ok)
                {
                    string sTitle = promptResult.Text;

                    if (!string.IsNullOrWhiteSpace(sTitle))
                    {
                        OnSaveTitle(sTitle);
                    }
                    else
                    {
                        await DisplayAlert("", "タイトル無しできません。", "確定");
                    }
                }
            }
            else
            {
                await Navigation.PushModalAsync(new StoreList(MyListView.SelectedItem.ToString()));
            }
        }

        private void btnReturnHome_Click(object sender, EventArgs e)
        {
            Navigation.PushModalAsync(new ImageDetail());
        }

        private void btnRead_Click(object sender, EventArgs e)
        {
            Navigation.PushModalAsync(new StoreListDetail());
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            DisplayAlert("", "実装待ち...", "確定");
        }

        private async void btnCommon_Click(object sender, EventArgs e)
        {
            //await Navigation.PushPopupAsync(new SMSMenu());
        }

        private async void btnHelp_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushPopupAsync(new TopMenu());
        }
    }
}