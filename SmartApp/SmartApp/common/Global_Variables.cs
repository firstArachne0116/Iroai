using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace SmartApp.common
{
    public class Global_Variables
    {
        #region 保存リスト選択フォルダ
        public static readonly ObservableCollection<string> StoreFolderList = new ObservableCollection<string>
        {
          "風景フォルダ",
          "絵画の色彩フォルダ",
          "サンプルフォルダ",
          "風景フォルダ",
          "絵画の色彩フォルダ",
          "サンプルフォルダ",
          "風景フォルダ",
          "絵画の色彩フォルダ",
          "サンプルフォルダ"
        };
        #endregion

    }
}
