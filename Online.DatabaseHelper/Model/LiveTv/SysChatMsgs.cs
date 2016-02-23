using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Online.DbHelper.Model
{
    public class SysChatMsgs
    {
        public Int64 ChatID { get; set; }

        public string  FromUserID { get; set; }

        public string FromUserName { get; set; }

        public string  ToUserID { get; set; }

        public string ToUserName { get; set; }

        public Int32 RoomID { get; set; }

        public string MsgContent { get; set; }

        /// <summary>
        /// LOV：0：其他；1表示上线；2表示下线；3表示管理员发送的消息；4.普通消息；5.我的消息; 100.私聊消息
        /// </summary>
        public Int32 MsgType { get; set; }

        public bool IsCheck { get; set; }

        public string FilePath { get; set; }

        public DateTime SendTime { get; set; }
    }
}
