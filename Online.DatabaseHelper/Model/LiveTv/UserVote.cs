using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Online.DbHelper.Model
{
    [Serializable]
    public class UserVote
    {
        public UserVote()
        {
            UserVoteColums = new List<UserVoteColum>();
        }
        public int VoteID { get; set; }
        public int RoomID { get; set; }
        public string VoteTitle { get; set; }
        public int OptCount { get; set; }
        public int VoteCount { get; set; }
        public bool IsVoteMulti { get; set; }
        public bool IsAnonymous { get; set; }
        public bool IsViewResult { get; set; }
        public string VoteSummary { get; set; }
        public DateTime VoteEndTime { get; set; }
        public bool IsDeleted { get; set; }
        public string CreateUser { get; set; }
        public DateTime VoteBeginTime { get; set; }
        public DateTime CreateTime { get; set; }

        public virtual List<UserVoteColum> UserVoteColums { get; set; } 
    }
}
