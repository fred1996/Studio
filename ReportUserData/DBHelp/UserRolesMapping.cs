using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Online.DbHelper.Model;

namespace Online.DbHelper.Mapping
{
    public class UserRolesMapping:EntityTypeConfiguration<UserRoles>
    {
        public UserRolesMapping()
        {
            ToTable("User_Roles");
            HasKey(t => t.AutoId);
            Property(t => t.UserId);
            Property(t => t.ProjectId);
            Property(t => t.RoleId);
            Property(t => t.NickName);
 
        }
    }
}
