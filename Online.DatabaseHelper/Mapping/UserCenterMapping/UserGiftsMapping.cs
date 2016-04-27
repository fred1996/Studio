using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Online.DbHelper.Model;
using Online.DbHelper.Model.UserCenter;

namespace Online.DbHelper.Mapping.UserCenterMapping
{
    public class UserGiftsMapping : EntityTypeConfiguration<UserGifts>
    {
        public UserGiftsMapping()
        {
            ToTable("User_Gifts");
            HasKey(t => t.Id);
            Property(t => t.UserId);
            Property(t => t.GiftId);
            Property(t => t.GiftName);
            Property(t => t.GiftNum);
            HasOptional(t => t.User).WithMany(t => t.UserGifts).HasForeignKey(t => t.UserId);
            HasOptional(t => t.Gift).WithMany(t => t.UserGifts).HasForeignKey(t => t.GiftId);
        }
    }
}
