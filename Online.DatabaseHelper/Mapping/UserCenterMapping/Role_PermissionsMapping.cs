using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Online.DbHelper.Model;

namespace Online.DbHelper.Mapping
{
    public class Role_PermissionsMapping : EntityTypeConfiguration<Role_Permissions>
    {
        public Role_PermissionsMapping()
        {
            ToTable("Role_Permissions");
            HasKey(t => t.Role_PermissionID);
            Property(t => t.RoleID);
            Property(t => t.PermissionID);
            HasOptional(t => t.Permissions).WithMany(t => t.RolePermissionses).HasForeignKey(t => t.PermissionID);
            HasOptional(t => t.Roles).WithMany(t => t.RolePermissionses).HasForeignKey(t => t.RoleID);
        }
    }
}
