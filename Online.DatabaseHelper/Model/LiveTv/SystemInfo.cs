using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Online.DbHelper.Model
{
    public class SystemInfo
    {
        /// <summary>
        /// ID
        /// </summary>
        public Int64 SysInfoID { get; set; }

        /// <summary>
        /// 房间ID
        /// </summary>
        public Int32? RoomID { get; set; }

        /// <summary>
        /// 消息标题
        /// </summary>
        public string InfoTitle { get; set; }

        /// <summary>
        ///消息类型
        /// </summary>
        public Int32 InfoType { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        public string InfoContent { get; set; }

        /// <summary>
        /// 图片
        /// </summary>
        public string ImgUrl { get; set; }

        /// <summary>
        /// 源链接
        /// </summary>
        public string SourceLink { get; set; }

        /// <summary>
        ///权重
        /// </summary>
        public Int32 InfoWeight { get; set; }

        /// <summary>
        /// 发送时间
        /// </summary>
        public DateTime SendTime { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string CreateUser { get; set; }

        public virtual LiveRooms LiveRooms { get; set; }

    }
}
