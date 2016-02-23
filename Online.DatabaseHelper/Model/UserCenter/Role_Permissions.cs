using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Online.DbHelper.Model
{
    public class Role_Permissions
    {
        public long Role_PermissionID { get; set; }
        public int? RoleID { get; set; }
        public int? PermissionID { get; set; }

        public virtual Permissions Permissions { get; set; }

        public virtual Roles Roles { get; set; }
    }
}
