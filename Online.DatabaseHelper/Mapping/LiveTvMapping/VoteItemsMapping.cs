using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Online.DbHelper.Model;

namespace Online.DbHelper.Mapping
{
    public class VoteItemsMapping:EntityTypeConfiguration<VoteItems>
    {
        public VoteItemsMapping()
        {
            ToTable("VoteItems");
            HasKey(t => t.VoteItemID);
            Property(t => t.VoteUserID);
            Property(t => t.UserVoteColumID);
            Property(t => t.VoteTime);
            HasOptional(t => t.UserVoteColum).WithMany(t => t.VoteItemses).HasForeignKey(t => t.UserVoteColumID);
        }
    }
}
