using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace SmartApp.Model
{
    [Table("IamgeSaveInfo")]
    /// <summary>
    /// 文件保存信息
    /// </summary>
    public class IamgeSaveInfo
    {
        /// <summary>
        /// 目录ID
        /// </summary>
        [PrimaryKey, AutoIncrement]
        public int CateLogId { get; set; }

        /// <summary>
        /// 文件ID
        /// </summary>
        public int ImageID { get; set; }

        /// <summary>
        /// imagepath
        /// </summary>
        public string ImagePath { get; set; }


        public string Imagebuffer { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime Createdatetime { get; set; }

    }
}
