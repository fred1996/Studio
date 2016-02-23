using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Online.DbHelper.Model;

namespace Online.DbHelper.Mapping
{
   public class LogMessageMapping:EntityTypeConfiguration<LogMessage>
    {
        public LogMessageMapping()
        {
            ToTable("LogMessage");
            HasKey(t => t.Id);
            Property(t => t.ClassType).HasMaxLength(500);
            Property(t => t.CreateDateTime);
            Property(t => t.MessageType);
            Property(t => t.MethodName);
            Property(t => t.Text).HasColumnType(SqlDbType.Text.ToString()).IsMaxLength();
        }
    }
}
