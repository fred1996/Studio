using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Online.DbHelper.Model;

namespace Online.DbHelper.Mapping
{
    public class PermissionsMapping : EntityTypeConfiguration<Permissions>
    {
        public PermissionsMapping()
        {
            ToTable("Permissions");
            HasKey(t => t.PermissionID);
            Property(t => t.Description);
            Property(t => t.IsAll);
            Property(t => t.IsEdit);
            Property(t => t.IsRead);
            Property(t => t.IsShow);
            Property(t => t.PName);
            HasMany(t => t.RolePermissionses).WithOptional(t => t.Permissions).HasForeignKey(t => t.PermissionID);
        }
    }
}
