using System.Data.Entity;
using Online.DbHelper.Mapping;
using Online.DbHelper.Model;

namespace Online.DbHelper.BLL
{
    public class DataContextBll : DbContext
    {
        public DataContextBll()
           : base("DefaultConnection")
        {

        }

        public DbSet<LiveRooms> LiveRoomses { get; set; }
        public DbSet<LiveTVs> LiveTVses { get; set; }
     
        public DbSet<SysChatMsgs> SysChatMsgses { get; set; }
        public DbSet<SysConfigs> SysConfigses { get; set; }
        public DbSet<SysDictionaries> SysDictionarieses { get; set; }
        public DbSet<SystemInfo> SystemInfos { get; set; }
        public DbSet<SysTVColumns> SysTvColumnses { get; set; }
        public DbSet<TVClassSchedule> TvClassSchedules { get; set; }
       
        public DbSet<UserVote> UserVotes { get; set; }
        public DbSet<UserVoteColum> UserVoteColums { get; set; }
        public DbSet<VoteItems> VoteItemses { get; set; }
        public DbSet<LogMessage> LogMessages { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new LiveRoomsMapping());
            modelBuilder.Configurations.Add(new LiveTVsMapping());
         
            modelBuilder.Configurations.Add(new SysChatMsgsMapping());
            modelBuilder.Configurations.Add(new SysConfigsMapping());
            modelBuilder.Configurations.Add(new SysDictionariesMapping());
            modelBuilder.Configurations.Add(new SystemInfoMapping());
            modelBuilder.Configurations.Add(new SysTVColumnsMapping());
            modelBuilder.Configurations.Add(new TVClassScheduleMapping());
           
            modelBuilder.Configurations.Add(new UserVoteMapping());
            modelBuilder.Configurations.Add(new VoteItemsMapping());
            modelBuilder.Configurations.Add(new UserVoteColumMapping());
            modelBuilder.Configurations.Add(new LogMessageMapping());
        }
    }
}
