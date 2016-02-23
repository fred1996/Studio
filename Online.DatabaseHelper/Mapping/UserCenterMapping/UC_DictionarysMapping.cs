using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Online.DbHelper.Model;

namespace Online.DbHelper.Mapping
{
    public class UC_DictionarysMapping : EntityTypeConfiguration<UC_Dictionarys>
    {
        public UC_DictionarysMapping()
        {
            ToTable("UC_Dictionarys");
            HasKey(t => t.DictID);
            Property(t => t.CreateTime);
            Property(t => t.Description);
            Property(t => t.FiledBaseName);
            Property(t => t.FiledBaseValue);
            Property(t => t.OperatorName);
            Property(t => t.ParentDictID);
            Property(t => t.UpdateTime);
        }


    }
}
