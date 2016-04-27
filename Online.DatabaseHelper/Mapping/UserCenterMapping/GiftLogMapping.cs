using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Online.DbHelper.Model.UserCenter;

namespace Online.DbHelper.Mapping.UserCenterMapping
{
    public class GiftLogMapping : EntityTypeConfiguration<GiftLog>
    {
        public GiftLogMapping()
        {
            ToTable("Gift_Log");
            HasKey(t => t.LogId);
            Property(t => t.UserId);
            Property(t => t.UserName);
            Property(t => t.ToUserId);
            Property(t => t.ToUserName);
            Property(t => t.GiftName);
            Property(t => t.GiftNum);
            Property(t => t.CreateTime);
        }
    }
}
