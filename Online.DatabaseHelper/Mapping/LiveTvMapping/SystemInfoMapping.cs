using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Online.DbHelper.Model;

namespace Online.DbHelper.Mapping
{
    public class SystemInfoMapping:EntityTypeConfiguration<SystemInfo>
    {
        public SystemInfoMapping()
        {
            ToTable("SystemInfo");
            HasKey(t => t.SysInfoID);
            Property(t => t.RoomID);
            Property(t => t.InfoTitle);
            Property(t => t.InfoType);
            Property(t => t.InfoContent);
            Property(t => t.ImgUrl);
            Property(t => t.SourceLink);
            Property(t => t.InfoWeight);
            Property(t => t.SendTime);
            Property(t => t.CreateUser);
            HasOptional(t => t.LiveRooms).WithMany(t => t.SystemInfos).HasForeignKey(t => t.RoomID);
        }
    }
}
