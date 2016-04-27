using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Online.DbHelper.Mapping;
using Online.DbHelper.Model;

namespace Online.DbHelper.BLL
{
    public class NewUserContentBll : DbContext
    {

        public NewUserContentBll()
           : base("UserConnection")
        {

        }

        public DbSet<Users> Userses { get; set; }
        public DbSet<UserRoles> UserRoleses { get; set; }
        public DbSet<Roles> Roleses { get; set; }
 

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new UsersMapping());
            modelBuilder.Configurations.Add(new UserRolesMapping());
            modelBuilder.Configurations.Add(new RolesMapping());
    
        }
    }
}
