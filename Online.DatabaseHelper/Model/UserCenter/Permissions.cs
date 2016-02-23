using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Online.DbHelper.Model
{
    public class Permissions
    {
        public int PermissionID { get; set; }
        public string PName { get; set; }
        public string Description { get; set; }
        public bool IsRead { get; set; }
        public bool IsEdit { get; set; }
        public bool IsShow { get; set; }
        public bool IsAll { get; set; }

        public virtual List<Role_Permissions> RolePermissionses { get; set; }
    }
}
