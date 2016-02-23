using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Online.DbHelper.Model;

namespace Online.DbHelper.Mapping
{
    public class UserRelationAssistantMapping:EntityTypeConfiguration<UserRelationAssistant>
    {
        public UserRelationAssistantMapping()
        {
            ToTable("UserRelationAssistant");
            HasKey(t => t.Id);
            Property(t => t.CreateDateTime);
            Property(t => t.UserId);
            Property(t => t.AssistantId);
            Property(t => t.IsDelete);
            Property(t => t.UserName);
      
            HasOptional(t => t.Assistant).WithMany(t => t.UserRelationAssistants).HasForeignKey(t => t.AssistantId);
        }
    }
}
