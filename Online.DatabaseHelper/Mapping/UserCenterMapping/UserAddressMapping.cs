using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Online.DbHelper.Model;

namespace Online.DbHelper.Mapping
{
    public class UserAddressMapping : EntityTypeConfiguration<UserAddress>
    {
        public UserAddressMapping()
        {
            ToTable("User_Address");
            HasKey(t => t.AddressID);
            Property(t => t.UserID);
            Property(t => t.Area);
            Property(t => t.City);
            Property(t => t.Country);
            Property(t => t.DetailInfo);
            Property(t => t.Email);
            Property(t => t.Province);
            Property(t => t.Telephone);
            HasOptional(t => t.User).WithMany(t => t.UserAddress).HasForeignKey(t => t.UserID);
        }
    }
}
