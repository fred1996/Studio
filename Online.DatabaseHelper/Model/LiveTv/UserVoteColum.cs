using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Online.DbHelper.Model
{
    [Serializable]
    public class UserVoteColum
    {
        public int ID { get; set; }
        public int? VoteID { get; set; }
        public string Columname { get; set; }
        public int VoteCount { get; set; }

        public virtual UserVote UserVote { get; set; }

        public virtual List<VoteItems> VoteItemses { get; set; } 
    }
}
