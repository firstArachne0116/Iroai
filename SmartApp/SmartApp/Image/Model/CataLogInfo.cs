using SQLite;
using System;

namespace SmartApp.Model
{
    [Table("CataLogInfo")]
    /// <summary>
    /// カテゴリ情報
    /// </summary>
    public class CataLogInfo
    {
        /// <summary>
        /// キー、ID
        /// </summary>
        [PrimaryKey]
        public int CateLogId { get; set; }

        /// <summary>
        /// 目录名称
        /// </summary>
        public string Folder { get; set; }

        /// <summary>
        /// 目录标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDatetime { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
    }
}
