using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity.ModelConfiguration;
using System.Text;
using System.Threading.Tasks;
using Online.DbHelper.Model;
namespace Online.DbHelper.Mapping
{
    public class UserVoteColumMapping : EntityTypeConfiguration<UserVoteColum>
    {
        public UserVoteColumMapping()
        {
            ToTable("UserVoteColum");
            HasKey(t => t.ID);
            Property(t => t.VoteID);
            Property(t => t.Columname);
            Property(t => t.VoteCount);
            HasOptional(t => t.UserVote).WithMany(t => t.UserVoteColums).HasForeignKey(t => t.VoteID);
            HasMany(t => t.VoteItemses).WithOptional(t => t.UserVoteColum).HasForeignKey(t => t.UserVoteColumID);
        }
    }
}
