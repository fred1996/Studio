using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Online.DbHelper.Model.UserCenter
{
   public class UserGifts
    {
       public long Id { get; set; }

       public long? UserId { get; set; }

       public int? GiftId { get; set; }

       public string GiftName { get; set; }

       public long GiftNum { get; set; }

        public virtual Users User { get; set; }

        public virtual Gift Gift { get; set; }
    }
}
