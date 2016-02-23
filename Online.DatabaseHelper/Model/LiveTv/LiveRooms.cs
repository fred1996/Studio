using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Online.DbHelper.Model
{
    public class LiveRooms
    {
        /// <summary>
        /// ID
        /// </summary>
        public Int32 RoomID { get; set; }

        /// <summary>
        /// 房间名称
        /// </summary>
        public string RoomName { get; set; }

        /// <summary>
        /// 房间logo
        /// </summary>
        public string LogoUrl { get; set; }

        /// <summary>
        ///房间logo title
        /// </summary>
        public string LogoTitle { get; set; }

        /// <summary>
        /// 房间类型
        /// </summary>
        public Int32 RType { get; set; }

        /// <summary>
        /// 用户限制数
        /// </summary>
        public Int32? LimitUserNum { get; set; }

        /// <summary>
        /// 是否允许私聊
        /// </summary>
        public bool IsPrivateChat { get; set; }

        /// <summary>
        /// 是否是主房间
        /// </summary>
        public bool IsMainRoom { get; set; }

        /// <summary>
        /// 主持人
        /// </summary>
        public string AnchorName { get; set; }

        /// <summary>
        /// 礼物数量 
        /// </summary>
        public Int32 giftNum { get; set; }

        /// <summary>
        /// 直播室ID 
        /// </summary>
        public Int32 LiveTVID { get; set; }

        /// <summary>
        /// 转移YY房间ID 
        /// </summary>
        public string TransitionYYRoomID { get; set; }

        /// <summary>
        /// 是否显示管理员消息 
        /// </summary>
        public bool IsShowAdminMsg { get; set; }

        /// <summary>
        /// 管理员消息背景色 
        /// </summary>
        public string AdminMsgBackgroundStyle { get; set; }

        /// <summary>
        /// 直播开始时间 
        /// </summary>
        public DateTime LiveStartTime { get; set; }

        /// <summary>
        /// 直播结束时间 
        /// </summary>
        public DateTime LiveEndTime { get; set; }

        /// <summary>
        /// 直播室弹图 
        /// </summary>
        public string WelcomeUrl { get; set; }

        /// <summary>
        /// 欢迎语 
        /// </summary>
        public string WelcomeText { get; set; }

        /// <summary>
        /// 房间标签 
        /// </summary>
        public string RoomTags { get; set; }

        /// <summary>
        /// 是否删除  0：未删除，1：删除，默认为0
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// 状态  0:禁用；1：启用
        /// </summary>
        public Int32 BizStatus { get; set; }

        /// <summary>
        /// 创建者 
        /// </summary>
        public string CreateUser { get; set; }

        /// <summary>
        /// 创建时间 
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 更新者 
        /// </summary>
        public string UpdateUser { get; set; }

        /// <summary>
        /// 修改时间  
        /// </summary>
        public DateTime LastChangeTime { get; set; }

        public virtual List<SysConfigs> SysConfigses { get; set; } 

        public virtual List<SystemInfo> SystemInfos { get; set; }

        public virtual List<SysTVColumns> SysTvColumnses { get; set; }

        public virtual List<TVClassSchedule> TvClassSchedules { get; set; }
    }
}
