using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Online.DbHelper.Model
{
    public class UserBlackList
    {
        public long BlackListId { get; set; }

        public long? UserId { get; set; }

        public string ClientIp { get; set; }

        public int RoomId { get; set; }

        public string OperateName { get; set; }

        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 1:封IP,2:禁用户
        /// </summary>
        public int? Type { get; set; }

        public string UserName { get; set; }

    }
}
