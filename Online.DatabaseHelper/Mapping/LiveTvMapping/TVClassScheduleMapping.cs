using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Online.DbHelper.Model;

namespace Online.DbHelper.Mapping
{
    public class TVClassScheduleMapping : EntityTypeConfiguration<TVClassSchedule>
    {
        public TVClassScheduleMapping()
        {
            ToTable("TVClassSchedule");
            HasKey(t => t.SCId);
            Property(t => t.LiveRoomId);
            Property(t => t.Teacher);
            Property(t => t.TNickName);
            Property(t => t.HomeUrl);
            Property(t => t.liveStartTime);
            Property(t => t.liveEndTime);
            Property(t => t.EffectiveStartTime);
            Property(t => t.EffectiveEndTime);
            HasOptional(t => t.LiveRooms).WithMany(t => t.TvClassSchedules).HasForeignKey(t => t.LiveRoomId);
        }
    }
}
