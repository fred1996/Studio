using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Online.Web.Models
{
    public class UserOnlineInfo
    {
        public Int32 roomid { get; set; }

        public Int64 uid { get; set; }

        public string from { get; set; }

        public Int32 rid { get; set; }

        public string socketid { get; set; }
    }
    public class UserOnline
    {
        public long UserId { get; set; }
        public string UserName { get; set; }
        public int RoleId { get; set; }
        public DateTime CreateDateTime { get; set; }
    }
}