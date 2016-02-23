using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Online.DbHelper.Model
{
   public class SysDictionaries
    {
        /// <summary>
        /// ID
        /// </summary>
        public Int64 SysDictID { get; set; }

        /// <summary>
        /// 父ID
        /// </summary>
        public Int64 ParentDictID { get; set; }

        /// <summary>
        /// key值
        /// </summary>
        public string FiledName { get; set; }

        /// <summary>
        ///value值
        /// </summary>
        public string FiledValue { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 操作人
        /// </summary>
        public string OperatorName { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateTime { get; set; }
    }
}
