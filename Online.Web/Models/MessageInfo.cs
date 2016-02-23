using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Online.Web.Models
{
    public class MessageInfo
    {
        public Int64 ChatID { get; set; }
        public Int32 roomid { get; set; }

        public Int32 uid { get; set; }

        public string from { get; set; }

        public Int32 touid { get; set; }

        public string to { get; set; }

        public Int32 roleid { get; set; }

        public string rolename { get; set; }

        public string msg { get; set; }

        public string postfile { get; set; }

        /// <summary>
        /// LOV：0：其他；1表示上线；2表示下线；3表示管理员发送的消息；4.普通消息；5.我的消息
        /// </summary>
        public Int32 msgtype { get; set; }

        public Int32 ischeck { get; set; }

        public bool isOVerMaxMsgCount { get; set; }

        public string sendtime { get; set; }

        public DateTime createTime { get; set; }
    }
}