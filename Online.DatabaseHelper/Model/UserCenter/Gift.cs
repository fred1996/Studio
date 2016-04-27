using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Online.DbHelper.Model.UserCenter
{
   public class Gift
    {
       public int GiftId { get; set; }

       public string GiftName { get; set; }

       public int GiftType { get; set; }

       public string GiftUnit { get; set; }
        /// <summary>
        /// 礼物最大个数，默认为0（无限制）
        /// </summary>
       public long GiftCount { get; set; }

       public DateTime CreateTime { get; set; }

        public virtual List<UserGifts> UserGifts { get; set; }
    }
}
