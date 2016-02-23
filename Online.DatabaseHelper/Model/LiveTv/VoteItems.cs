using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Online.DbHelper.Model
{
    public class VoteItems
    {
        public int VoteItemID { get; set; }

        public int? UserVoteColumID { get; set; }

        public int VoteUserID { get; set; }

        public DateTime VoteTime { get; set; }

        public virtual UserVoteColum UserVoteColum { get; set; }
    }
}
