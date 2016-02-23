using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Online.DbHelper.Model
{
    public class UserActionLog
    {
        public long UserActionLogId { get; set; }
        public long? UserId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Type { get; set; }
        public int ProjectId { get; set; }
        public string UserIp { get; set; }
        public int? RoomId { get; set; }
        public DateTime CreateTime { get; set; }
        public virtual Users User { get; set; }
    }
}
