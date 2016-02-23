using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Online.DbHelper.Model
{
  public  class UC_Dictionarys
    {

        public int DictID { get; set; }
        public int ParentDictID { get; set; }
        public string FiledBaseName { get; set; }
        public string FiledBaseValue { get; set; }
        public string Description { get; set; }
        public string OperatorName { get; set; }
        public System.DateTime CreateTime { get; set; }
        public System.DateTime UpdateTime { get; set; }
    }
}
