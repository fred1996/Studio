using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Online.DbHelper.Model;

namespace Online.DbHelper.Mapping
{
    public class SysConfigsMapping : EntityTypeConfiguration<SysConfigs>
    {
        public SysConfigsMapping()
        {
            ToTable("SysConfigs");
            HasKey(t => t.SysConfigID);
            Property(t => t.RoomId);
            Property(t => t.IsAllowPost);
            Property(t => t.IsAllowTouristPost);
            Property(t => t.IsShowUserList);
            Property(t => t.IsUploadFile);
            Property(t => t.UploadFileSize);
            Property(t => t.IsSaveGuestMsg);
            Property(t => t.IsOpenReg);
            Property(t => t.IsVerifyPhone);
            Property(t => t.IsCheckMsg);
            Property(t => t.IsFilterMsg);
            Property(t => t.ServiceQQs);
            Property(t => t.IsLock);
            Property(t => t.LoginPwd);
            HasOptional(t => t.LiveRooms).WithMany(t => t.SysConfigses).HasForeignKey(t => t.RoomId);
        }
    }
}
