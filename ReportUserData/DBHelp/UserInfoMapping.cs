using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Online.DbHelper.Model;

namespace Online.DbHelper.Mapping
{
    public class UserInfoMapping : EntityTypeConfiguration<i_User_Info>
    {
        public UserInfoMapping()
        {
            ToTable("i_User_Info");
            HasKey(t => t.i_User_ID);
            Property(t => t.i_User_Name);
            Property(t => t.i_User_Mail);
            Property(t => t.i_User_Pass);
            Property(t => t.i_User_RoleID);
            Property(t => t.i_User_SaleID);
            Property(t => t.i_User_SaleCompanyID);
            Property(t => t.i_User_CompanyID);
            Property(t => t.i_User_CompanyRoleID);
            Property(t => t.i_User_CustomCompanys);
            Property(t => t.i_User_ClassID);
            Property(t => t.i_User_TypeID);
            Property(t => t.i_User_NickName);
            Property(t => t.i_User_RealName);
            Property(t => t.i_User_Sex);
            Property(t => t.i_User_QQ);
            Property(t => t.i_User_Tel);
            Property(t => t.i_User_Add);
            Property(t => t.i_User_Birthday);
            Property(t => t.i_User_Profession);
            Property(t => t.i_User_Face);
            Property(t => t.i_User_About);
            Property(t => t.i_User_Faction);
            Property(t => t.i_User_Invest);
            Property(t => t.i_User_Era);
            Property(t => t.i_User_Label);
            Property(t => t.i_User_Tags);
            Property(t => t.i_User_Score);
            Property(t => t.i_User_Money);
            Property(t => t.i_User_LiveNess);
            Property(t => t.i_User_ViewNum);
            Property(t => t.i_User_RegTime);
            Property(t => t.i_User_LastLoginTime);
            Property(t => t.i_User_LoginIP);
            Property(t => t.i_User_IsLock);
            Property(t => t.i_User_MailValid_Key);
            Property(t => t.i_User_MailValid);
            Property(t => t.i_User_Question);
            Property(t => t.i_User_Answer);
            Property(t => t.i_User_OpenID);
            Property(t => t.i_User_UCID);
            Property(t => t.i_User_IsRobot);
            Property(t => t.i_User_RedBagNum);
            //Property(t => t.i_User_IncomeNum);
            Property(t => t.i_User_TelisTrue);

        }
    }
}
