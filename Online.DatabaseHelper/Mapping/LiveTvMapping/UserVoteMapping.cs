using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Online.DbHelper.Model;

namespace Online.DbHelper.Mapping
{
    public class UserVoteMapping:EntityTypeConfiguration<UserVote>
    {
        public UserVoteMapping()
        {
            ToTable("UserVote");
            HasKey(t => t.VoteID);
            Property(t => t.RoomID);
            Property(t => t.VoteTitle);
            Property(t => t.OptCount);
            Property(t => t.VoteCount);
            Property(t => t.IsVoteMulti);
            Property(t => t.IsAnonymous);
            Property(t => t.IsViewResult);
            Property(t => t.VoteSummary);
            Property(t => t.VoteEndTime);
            Property(t => t.IsDeleted);
            Property(t => t.CreateUser);
            Property(t => t.CreateTime);
            Property(t => t.VoteBeginTime);
            HasMany(t => t.UserVoteColums).WithOptional(t => t.UserVote).HasForeignKey(t => t.VoteID);
        }
    }
}
