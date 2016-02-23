using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Online.DbHelper.Model;

namespace Online.DbHelper.Mapping
{
    public class SysTVColumnsMapping:EntityTypeConfiguration<SysTVColumns>
    {
        public SysTVColumnsMapping()
        {
            ToTable("SysTVColumns");
            HasKey(t => t.SysTVColumnID);
            Property(t => t.RoomID);
            Property(t => t.ItemTitle);
            Property(t => t.ItemName);
            Property(t => t.ISummary);
            Property(t => t.ItemImgUrl);
            Property(t => t.ItemLink);
            Property(t => t.ItemType);
            Property(t => t.CreateUser);
            Property(t => t.CreateTime);
            HasOptional(t => t.LiveRooms).WithMany(t => t.SysTvColumnses).HasForeignKey(t => t.RoomID);
        }
    }
}
