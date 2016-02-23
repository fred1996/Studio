using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Online.DbHelper.Model
{
    [Serializable]
   public class SysConfigs
    {
        /// <summary>
        /// ID
        /// </summary>
        public int SysConfigID { get; set; }

        /// <summary>
        /// 房间ID
        /// </summary>
        public int? RoomId { get; set; }

        /// <summary>
        /// 是否禁言
        /// </summary>
        public bool IsAllowPost { get; set; }

        /// <summary>
        ///是否允许游客发言
        /// </summary>
        public bool IsAllowTouristPost { get; set; }

        /// <summary>
        /// 是否显示用户列表
        /// </summary>
        public bool IsShowUserList { get; set; }

        /// <summary>
        /// 是否允许上传附件
        /// </summary>
        public bool IsUploadFile { get; set; }

        /// <summary>
        /// 附件体积限制
        /// </summary>
        public Int32 UploadFileSize { get; set; }

        /// <summary>
        /// 是否保存游客消息
        /// </summary>
        public bool IsSaveGuestMsg { get; set; }

        /// <summary>
        /// 是否开放注册
        /// </summary>
        public bool IsOpenReg { get; set; }

        /// <summary>
        /// 是否开启手机验证
        /// </summary>
        public bool IsVerifyPhone { get; set; }

        /// <summary>
        /// 是否开启消息审核
        /// </summary>
        public bool IsCheckMsg { get; set; }

        /// <summary>
        /// 是否开启消息过滤
        /// </summary>
        public bool IsFilterMsg { get; set; }

        /// <summary>
        /// 客服QQ
        /// </summary>
        public string ServiceQQs { get; set; }

        /// <summary>
        /// 是否锁定房间
        /// </summary>
        public bool IsLock { get; set; }

        /// <summary>
        /// 房间密码
        /// </summary>
        public string LoginPwd { get; set; }

        public virtual LiveRooms LiveRooms { get; set; }
    }
}
