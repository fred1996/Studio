using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Online.DbHelper.Mapping;
using Online.DbHelper.Mapping.UserCenterMapping;
using Online.DbHelper.Model;
using Online.DbHelper.Model.UserCenter;

namespace Online.DbHelper.BLL
{
    public class UserContextBll : DbContext
    {
        public UserContextBll()
           : base("UserConnection")
        {

        }

        public DbSet<Users> Userses { get; set; }
        public DbSet<UserRoles> UserRoleses { get; set; }
        public DbSet<Roles> Roleses { get; set; }
        public DbSet<UserBlackList> UserBlackLists { get; set; }
        public DbSet<UC_Dictionarys> UcDictionaryses { get; set; }

        public DbSet<UserAddress> UserAddresses { get; set; }

        public DbSet<UserActionLog> UserActionLogs { get; set; }

        public DbSet<Permissions> Permissions { get; set; }

        public DbSet<Role_Permissions> Role_Permissions { get; set; }

        public DbSet<UserRelationAssistant> UserRelationAssistants { get; set; }

        public DbSet<UserGifts> UserGiftses { get; set; }

        public DbSet<Gift> Gifts { get; set; }

        public DbSet<GiftLog> GiftLogs { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new UsersMapping());
            modelBuilder.Configurations.Add(new UserRolesMapping());
            modelBuilder.Configurations.Add(new RolesMapping());
            modelBuilder.Configurations.Add(new UserAddressMapping());
            modelBuilder.Configurations.Add(new UC_DictionarysMapping());
            modelBuilder.Configurations.Add(new UserBlackListMapping());

            modelBuilder.Configurations.Add(new UserActionLogMapping());
            modelBuilder.Configurations.Add(new PermissionsMapping());
            modelBuilder.Configurations.Add(new Role_PermissionsMapping());
            modelBuilder.Configurations.Add(new UserRelationAssistantMapping());
            modelBuilder.Configurations.Add(new UserGiftsMapping());
            modelBuilder.Configurations.Add(new GiftMapping());
            modelBuilder.Configurations.Add(new GiftLogMapping());
        }
    }
}
