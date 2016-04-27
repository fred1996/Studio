using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Online.DbHelper.Help;
using Online.DbHelper.Model.UserCenter;

namespace Online.DbHelper.Model
{
    public class Users
    {
        public Users()
        {
            UserName = string.Empty;
            Password = string.Empty;
            Salt = string.Empty;
            RealName = string.Empty;
            Avatar = string.Empty;
            Email = string.Empty;
            QQ = string.Empty;
            Weixin = string.Empty;
            Weibo = string.Empty;
            IDCard = string.Empty;
            Telephone = string.Empty;
            UserTags = string.Empty;
            CreateUser = string.Empty;
            RegisterIP = string.Empty;
            RegSource = string.Empty;
            LastSigninIP = string.Empty;
            UpdateUser = string.Empty;
            OpenAccountAuditUser = string.Empty;
            BirthDay = DbUntilHelper.GetDefaultDateTime();
            RegisterTime = DbUntilHelper.GetDefaultDateTime();
            LastSignInTime = DbUntilHelper.GetDefaultDateTime();
            LastSignOutTime = DbUntilHelper.GetDefaultDateTime();
            LastChangeTime = DbUntilHelper.GetDefaultDateTime();
            OpenAccountApplyTime = DbUntilHelper.GetDefaultDateTime();
            OpenAccountAuditTime = DbUntilHelper.GetDefaultDateTime();
            Field1 = string.Empty;
            Field2 = string.Empty;
            Field3 = string.Empty;
            Field4 = string.Empty;
            Field5 = string.Empty;
            Field6 = string.Empty;
            Field7 = string.Empty;
            Field8 = string.Empty;
            SexTheme = string.Empty;
            Score = 0;
            RecommendCode = string.Empty;
        }


        public long UserID { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string Salt { get; set; }

        public string RealName { get; set; }

        public string Avatar { get; set; }

        public int Sex { get; set; }

        public DateTime BirthDay { get; set; }

        public string Email { get; set; }

        public bool IsEmailEffective { get; set; }

        public string QQ { get; set; }

        public string Weixin { get; set; }

        public string Weibo { get; set; }

        public string IDCard { get; set; }

        public string Telephone { get; set; }

        public bool IsTelphoneEffective { get; set; }

        public int InvestmentType { get; set; }

        public string UserTags { get; set; }

        public bool IsDeleted { get; set; }

        public int BizStatus { get; set; }

        public string CreateUser { get; set; }

        public DateTime RegisterTime { get; set; }

        public string RegisterIP { get; set; }

        public string RegSource { get; set; }

        public int Owner { get; set; }

        public int Channel { get; set; }

        public int Device { get; set; }

        public DateTime LastSignInTime { get; set; }

        public DateTime LastSignOutTime { get; set; }

        public string LastSigninIP { get; set; }

        public string UpdateUser { get; set; }

        public DateTime LastChangeTime { get; set; }

        public DateTime OpenAccountApplyTime { get; set; }

        public DateTime OpenAccountAuditTime { get; set; }

        public string OpenAccountAuditUser { get; set; }

        public string Field1 { get; set; }

        public string Field2 { get; set; }

        public string Field3 { get; set; }

        public string Field4 { get; set; }

        public string Field5 { get; set; }

        public string Field6 { get; set; }

        public string Field7 { get; set; }

        public string Field8 { get; set; }

        public string SexTheme { get; set; }

        public int Score { get; set; }

        public string RecommendCode { get; set; }
        /// <summary>
        /// 连续登陆次数
        /// </summary>
        public int ContinueCount { get; set; }

        public virtual List<UserRoles> UserRoleses { get; set; }
        public virtual List<UserAddress> UserAddress { get; set; }
        public virtual List<UserActionLog> UserActionlog { get; set; }
        public virtual List<UserRelationAssistant> UserRelationAssistants { get; set; }

        public virtual List<UserGifts> UserGifts { get; set; }
 

    }
}
