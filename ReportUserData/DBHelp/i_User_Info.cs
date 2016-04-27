using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Online.DbHelper.Model
{
   public class i_User_Info
    {
       public int i_User_ID { get; set; }

       public string i_User_Name { get; set; }

       public string i_User_Mail { get; set; }

        public string i_User_Pass { get; set; }

       public int? i_User_RoleID { get; set; }
       public int? i_User_SaleID { get; set; }

       public int? i_User_SaleCompanyID { get; set; }

       public int? i_User_CompanyID { get; set; }
       public int? i_User_CompanyRoleID { get; set; }
       public string i_User_CustomCompanys { get; set; }
       public int? i_User_ClassID { get; set; }

       public int? i_User_TypeID { get; set; }
       public string i_User_NickName { get; set; }
       public string i_User_RealName { get; set; }

       public int i_User_Sex { get; set; }

       public string i_User_QQ { get; set; }

       public string i_User_Tel { get; set; }

       public string i_User_Add { get; set; }
       public string i_User_Birthday { get; set; }

       public int i_User_Profession { get; set; }
       public string i_User_Face { get; set; }

       public string i_User_About { get; set; }

       public int? i_User_Faction { get; set; }

       public string i_User_Invest { get; set; }

       public int i_User_Era { get; set; }

       public string i_User_Label { get; set; }
       public string i_User_Tags { get; set; }

       public long i_User_Score { get; set; }

       public long i_User_Money { get; set; }

       public int? i_User_LiveNess { get; set; }

       public long i_User_ViewNum { get; set; }

       public DateTime? i_User_RegTime { get; set; }

       public DateTime? i_User_LastLoginTime { get; set; }

       public string i_User_LoginIP { get; set; }

       public int? i_User_IsLock { get; set; }
       public string i_User_MailValid_Key { get; set; }

       public int? i_User_MailValid { get; set; }

       public string i_User_Question { get; set; }

       public string i_User_Answer { get; set; }

       public string i_User_OpenID { get; set; }


       public string i_User_UCID { get; set; }

       public int? i_User_IsRobot { get; set; }
       public int? i_User_RedBagNum { get; set; }

       //public long i_User_IncomeNum { get; set; }

       public int? i_User_TelisTrue { get; set; }
    }
}
