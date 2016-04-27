using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Online.DbHelper.Model;

namespace Online.DbHelper.Mapping
{
    public class UserActionLogMapping:EntityTypeConfiguration<UserActionLog>
    {
        public UserActionLogMapping()
        {
            ToTable("User_ActionLog");
            HasKey(t => t.UserActionLogId);
            Property(t => t.UserId);
            Property(t => t.Title);
            Property(t => t.Description);
            Property(t => t.Type);
            Property(t => t.ProjectId);
            Property(t => t.UserIp);
            Property(t => t.CreateTime);
            Property(t => t.RoomId);
            Property(t => t.FromUrl);
            Property(t => t.CurrentUrl);
            Property(t => t.UserName);
            HasOptional(t => t.User).WithMany(t => t.UserActionlog).HasForeignKey(t=>t.UserId);
        }
    }
}
