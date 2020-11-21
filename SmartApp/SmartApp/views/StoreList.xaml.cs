using SmartApp.Model;
using SmartApp.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SmartApp.views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class StoreList : ContentPage
    {
        public StoreList()
        {
            InitializeComponent();
        }

        public StoreList(string folder) : this()
        {
            GetImageInfoByFolder(folder);
            MyListView.ItemTapped += MyListView_ItemTapped;
        }

        public async void MyListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            string _ImageSrc = (MyListView.SelectedItem as NewsListItemModel).ImageSrc;
            List<IamgeSaveInfo> IamgeSaveInfoList = await SqliteUtil.Current.QueryIamgeSaveInfoByImagePath(_ImageSrc);
            var imageIdGroupList = IamgeSaveInfoList
                    .GroupBy(x => new { x.ImageID, x.CateLogId,x.Imagebuffer })
                    .Select(group => new
                    {
                        Keys = group.Key
                    }).ToList();

            List<ImageColorInfo> ImageColorInfoList = await SqliteUtil.Current.QueryImageColorInfoByImageID(imageIdGroupList[0].Keys.ImageID);

            FileStream _infoStream = new FileStream(_ImageSrc, FileMode.Open, FileAccess.Read);

            //await Navigation.PushModalAsync(new ImageDetail(Convert.FromBase64String(IamgeSaveInfoList[0].Imagebuffer), ImageColorInfoList, _ImageSrc));
        }

        private void btn_Return(object sender, EventArgs e)
        {
            DisplayAlert("", "実装待ち...", "確定");
            //Navigation.PushModalAsync(new MainStoreList(0));
        }

        private void btn_SaveImageInfo(object sender, EventArgs e)
        {
            Navigation.PushModalAsync(new MainPage());
        }

        public async void GetImageInfoByFolder(string folder)
        {
            List<CataLogInfo> CataLogInfoList = await SqliteUtil.Current.QueryCataLogsByFolder(folder);
            if (CataLogInfoList == null)
                return;

            if (CataLogInfoList.Count == 0)
            {
                await DisplayAlert("", "インメージが無し、ご確定してください。", "確定");
                //await Navigation.PushModalAsync(new MainStoreList(0));
                return;
            }

            List<NewsListItemModel> _NewsListItemModelLists = new List<NewsListItemModel>();

            var titleGroupList = CataLogInfoList
                    .GroupBy(x => new { x.Title,x.CateLogId })
                    .Select(group => new
                    {
                        Keys = group.Key
                    }).ToList();

            foreach (var item in titleGroupList)
            {
                List<IamgeSaveInfo> IamgeSaveInfoList = await SqliteUtil.Current.QueryIamgeSaveInfoByCateLogId(item.Keys.CateLogId);
                NewsListItemModel newsListItemModel = new NewsListItemModel();
                var imagePathGroupList = IamgeSaveInfoList
                    .GroupBy(x => new { x.ImagePath, x.ImageID })
                    .Select(group => new
                    {
                        Keys = group.Key
                    }).ToList();

                foreach (var _item in imagePathGroupList)
                {
                    newsListItemModel.Title = item.Keys.Title;
                    SaveListName.Text = folder;
                    newsListItemModel.ImageSrc = _item.Keys.ImagePath;
                    List<ImageColorInfo> ImageColorListByImageID = await SqliteUtil.Current.QueryImageColorInfoByImageID(Convert.ToInt32(_item.Keys.ImageID));

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

                    _NewsListItemModelLists.Add(newsListItemModel);
                }
            }

            MyListView.ItemsSource = _NewsListItemModelLists;
        }

    }
}