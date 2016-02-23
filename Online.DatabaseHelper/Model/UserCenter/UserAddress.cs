using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Online.DbHelper.Model
{
   public class UserAddress
    {
        public long AddressID { get; set; }
        public long? UserID { get; set; }
        public string Country { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        public string Area { get; set; }
        public string Email { get; set; }        
        public string Telephone { get; set; }
        public string DetailInfo { get; set; }
        public virtual Users User { get; set; }
    }
}
