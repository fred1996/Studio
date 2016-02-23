using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Online.DbHelper.Model
{
    public class Roles
    {
        public int RoleID { get; set; }

        public string RoleName { get; set; }

        public string Description { get; set; }

        public int BizStatus { get; set; }

        public string CreateUser { get; set; }

        public DateTime CreateTime { get; set; }

        public int? PowerId { get; set; }

        public virtual List<UserRoles> UserRoleses { get; set; }

        public virtual List<Role_Permissions> RolePermissionses { get; set; } 
    }
}
