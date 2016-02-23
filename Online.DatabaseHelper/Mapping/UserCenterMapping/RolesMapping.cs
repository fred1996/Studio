using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Online.DbHelper.Model;

namespace Online.DbHelper.Mapping
{
    public class RolesMapping : EntityTypeConfiguration<Roles>
    {
        public RolesMapping()
        {
            ToTable("Roles");
            HasKey(t => t.RoleID);
            Property(t => t.RoleName);
            Property(t => t.Description);
            Property(t => t.BizStatus);
            Property(t => t.CreateUser);
            Property(t => t.CreateTime);
            Property(t => t.PowerId);
            HasMany(t => t.UserRoleses).WithOptional(t => t.Roles).HasForeignKey(t => t.RoleId);
            HasMany(t => t.RolePermissionses).WithOptional(t => t.Roles).HasForeignKey(t => t.RoleID);
        }
    }
}
