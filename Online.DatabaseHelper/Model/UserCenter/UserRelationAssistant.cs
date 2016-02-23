using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Online.DbHelper.Model
{
   public  class UserRelationAssistant
    {
        public Guid Id { get; set; }
        public DateTime CreateDateTime { get; set; }
        public bool IsDelete { get; set; }

        public long? UserId { get; set; }
        public string UserName { get; set; }
        public long? AssistantId { get; set; }
        public virtual Users Assistant { get; set; }
 
    }
}
