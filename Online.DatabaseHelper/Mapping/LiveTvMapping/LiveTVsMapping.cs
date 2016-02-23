using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Online.DbHelper.Model;

namespace Online.DbHelper.Mapping
{
    public class LiveTVsMapping:EntityTypeConfiguration<LiveTVs>
    {
        public LiveTVsMapping()
        {
            ToTable("LiveTVs");
            HasKey(t => t.LiveTVID);
            Property(t => t.TVName);
            Property(t => t.LogoUrl);
            Property(t => t.LogoTitle);
            Property(t => t.IsDeleted);
            Property(t => t.BizStatus);
            Property(t => t.CreateUser);
            Property(t => t.CreateTime);
            Property(t => t.UpdateUser);
            Property(t => t.LastChangeTime);
        }
    }
}
