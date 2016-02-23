using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Online.DbHelper.Model
{
    public class SysTVColumns
    {
        /// <summary>
        /// ID
        /// </summary>
        public Int32 SysTVColumnID { get; set; }

        /// <summary>
        /// 房间ID
        /// </summary>
        public Int32? RoomID { get; set; }

        /// <summary>
        /// 栏目标题
        /// </summary>
        public string ItemTitle { get; set; }

        /// <summary>
        ///标目名称
        /// </summary>
        public string ItemName { get; set; }

        [AllowHtml]
        /// <summary>
        /// 简介
        /// </summary>
        public string ISummary { get; set; }

        /// <summary>
        /// 栏目图片地址
        /// </summary>
        public string ItemImgUrl { get; set; }

        /// <summary>
        /// 栏目链接
        /// </summary>
        public string ItemLink { get; set; }

        /// <summary>
        ///栏目类型
        /// </summary>
        public Int32 ItemType { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string CreateUser { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        public virtual LiveRooms LiveRooms { get; set; }
    }
}
