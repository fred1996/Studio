using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Online.DbHelper.Model;

namespace Online.DbHelper.Mapping
{
    public class SysDictionariesMapping:EntityTypeConfiguration<SysDictionaries>
    {
        public SysDictionariesMapping()
        {
            ToTable("SysDictionaries");
            HasKey(t => t.SysDictID);
            Property(t => t.ParentDictID);
            Property(t => t.FiledName);
            Property(t => t.FiledValue);
            Property(t => t.Description);
            Property(t => t.OperatorName);
            Property(t => t.CreateTime);
            Property(t => t.UpdateTime);
        }
    }
}
