using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Online.DbHelper.Model
{
    public class UserRoles
    {
        public int AutoId { get; set; }

        public long? UserId { get; set; }

        public int ProjectId { get; set; }

        public int? RoleId { get; set; }

        public string NickName { get; set; }

        public virtual Users Users { get; set; }

        public virtual Roles Roles { get; set; }
    }
}
