using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Online.DbHelper.Model.UserCenter;

namespace Online.DbHelper.Mapping.UserCenterMapping
{
    public class GiftMapping : EntityTypeConfiguration<Gift>
    {
        public GiftMapping()
        {
            ToTable("Gift");
            HasKey(t => t.GiftId);
            Property(t => t.GiftName);
            Property(t => t.GiftType);
            Property(t => t.GiftUnit);
            Property(t => t.GiftCount);
            Property(t => t.CreateTime);
            HasMany(t => t.UserGifts).WithOptional(t => t.Gift).HasForeignKey(t => t.GiftId);
        }
    }
}
