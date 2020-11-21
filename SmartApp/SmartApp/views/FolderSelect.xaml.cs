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

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SmartApp.views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FolderSelect : ContentPage
    {
        private string _fullpath;
        public FolderSelect(string filepath)
        {
            InitializeComponent();
            _fullpath = filepath;
            GetFolderInfo();
        }

        public void GetFolderInfo()
        {
            List<string> pathList = DependencyService.Get<IGetFileLst>().GetFoderList();

            if (pathList == null)
                return;

            if (pathList.Count == 0)
            {
                DisplayAlert("", "エキスポートフォルダがありません。", "確定");
            }

            List<FullPathInfo> ExportFathList = new List<FullPathInfo>();

            foreach (var item in pathList)
            {
                FullPathInfo folderInfo = new FullPathInfo();

                folderInfo.FullPath = item;
                folderInfo.Folder = item.Substring(item.LastIndexOf("/") + 1);


                ExportFathList.Add(folderInfo);
            }

            using (UserDialogs.Instance.Loading("Loading", null, null, true, MaskType.Black))
            {
                MyListView.ItemsSource = ExportFathList;
            }
        }

        private void btnRetun(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_fullpath))
            {
                Navigation.PushModalAsync(new MainPage());
            }
            else
            {
                Navigation.PushModalAsync(new ImageDetail(DependencyService.Get<IData>().ReadBinary(_fullpath), -1, _fullpath));
            }

        }


        private void btnSelected(object sender, EventArgs e)
        {
            if (MyListView.SelectedItem == null)
            {
                DisplayAlert("", "エキスポートフォルダを選択してください。", "確定");
            }
            else
            {
                Navigation.PushModalAsync(new Export());
            }
        }
    }
}