using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Online.DbHelper.Model;

namespace Online.DbHelper.Mapping
{
    public class LiveRoomsMapping:EntityTypeConfiguration<LiveRooms>
    {
        public LiveRoomsMapping()
        {
            ToTable("LiveRooms");
            HasKey(t => t.RoomID);
            Property(t => t.RoomName);
            Property(t => t.LogoUrl);
            Property(t => t.LogoTitle);
            Property(t => t.RType);
            Property(t => t.LimitUserNum);
            Property(t => t.IsPrivateChat);
            Property(t => t.IsMainRoom);
            Property(t => t.AnchorName);
            Property(t => t.giftNum);
            Property(t => t.LiveTVID);
            Property(t => t.TransitionYYRoomID);
            Property(t => t.IsShowAdminMsg);
            Property(t => t.AdminMsgBackgroundStyle);
            Property(t => t.LiveStartTime);
            Property(t => t.LiveEndTime);
            Property(t => t.WelcomeUrl);
            Property(t => t.WelcomeText);
            Property(t => t.RoomTags);
            Property(t => t.IsDeleted);
            Property(t => t.BizStatus);
            Property(t => t.CreateUser);
            Property(t => t.CreateTime);
            Property(t => t.UpdateUser);
            Property(t => t.LastChangeTime);

            HasMany(t => t.SysConfigses).WithOptional(t => t.LiveRooms).HasForeignKey(t => t.RoomId);
            HasMany(t => t.SystemInfos).WithOptional(t => t.LiveRooms).HasForeignKey(t => t.RoomID);
            HasMany(t => t.SysTvColumnses).WithOptional(t => t.LiveRooms).HasForeignKey(t => t.RoomID);
            HasMany(t => t.TvClassSchedules).WithOptional(t => t.LiveRooms).HasForeignKey(t => t.LiveRoomId);
        }
    }
}
