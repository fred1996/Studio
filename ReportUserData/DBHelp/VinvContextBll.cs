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
   public class VinvContextBll : DbContext
    {
        public VinvContextBll()
           : base("Vinv")
        {

        }

        public DbSet<i_User_Info> i_User_Info { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new UserInfoMapping());
           
        }
    }
}
