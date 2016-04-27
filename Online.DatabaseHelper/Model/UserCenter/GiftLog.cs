using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Online.DbHelper.Model.UserCenter
{
    public class GiftLog
    {
        public long LogId { get; set; }

        public long UserId { get; set; }

        public string UserName { get; set; }
        public long? ToUserId { get; set; }
        public string ToUserName { get; set; }

        public string GiftName { get; set; }

        public long GiftNum { get; set; }

        public DateTime CreateTime { get; set; }
    }
}
