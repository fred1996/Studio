using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Online.DbHelper.Model;

namespace Online.DbHelper.Mapping
{
    public class SysChatMsgsMapping:EntityTypeConfiguration<SysChatMsgs>
    {
        public SysChatMsgsMapping()
        {
            ToTable("SysChatMsgs");
            HasKey(t => t.ChatID);
            Property(t => t.FromUserID);
            Property(t => t.FromUserName);
            Property(t => t.ToUserID);
            Property(t => t.ToUserName);
            Property(t => t.RoomID);
            Property(t => t.MsgContent);
            Property(t => t.MsgType);
            Property(t => t.IsCheck);
            Property(t => t.FilePath);
            Property(t => t.SendTime);
            Property(t => t.ClientIp);
            Property(t => t.UpdateTime);
            Property(t => t.OperatorName);
        }
    }
}
