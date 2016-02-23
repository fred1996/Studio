using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Online.DbHelper.Model
{
  public  class LiveTVs
    {
        /// <summary>
        /// ID
        /// </summary>
        public Int32 LiveTVID { get; set; }

        /// <summary>
        /// 直播室名称
        /// </summary>
        public string TVName { get; set; }

        /// <summary>
        /// logo
        /// </summary>
        public string LogoUrl { get; set; }

        /// <summary>
        /// logo Title
        /// </summary>
        public string LogoTitle { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public Int32 BizStatus { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string CreateUser { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 修改者
        /// </summary>
        public string UpdateUser { get; set; }

        /// <summary>
        /// 最后一次修改时间 
        /// </summary>
        public DateTime LastChangeTime { get; set; }

        ///// <summary>
        ///// 直播室主题
        ///// </summary>
        //public string AdminMsgBackgroundStyle { get; set; }
    }
}
