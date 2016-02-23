using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Online.DbHelper.Model;

namespace Online.DbHelper.Mapping
{
   public class UserBlackListMapping:EntityTypeConfiguration<UserBlackList>
    {
       public UserBlackListMapping()
       {
           ToTable("User_BlackList");
           HasKey(t => t.BlackListId);
           Property(t => t.UserId);
           Property(t => t.ClientIp);
           Property(t => t.RoomId);
           Property(t => t.OperateName);
           Property(t => t.CreateTime);
           Property(t => t.Type);
           Property(t => t.UserName);
       }
    }
}
