using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Online.DbHelper.Model;

namespace Online.DbHelper.Mapping
{
    public class UsersMapping : EntityTypeConfiguration<Users>
    {
        public UsersMapping()
        {
            ToTable("Users");
            HasKey(t => t.RegisterTime);
            Property(t => t.UserID);

            Property(t => t.UserName);
            Property(t => t.Password);
            Property(t => t.Salt);
            Property(t => t.RealName);
            Property(t => t.Avatar);
            Property(t => t.Sex);
            Property(t => t.BirthDay);
            Property(t => t.Email);
            Property(t => t.IsEmailEffective);
            Property(t => t.QQ);
            Property(t => t.Weixin);
            Property(t => t.Weibo);
            Property(t => t.IDCard);
            Property(t => t.Telephone);
            Property(t => t.IsTelphoneEffective);
            Property(t => t.InvestmentType);
            Property(t => t.UserTags);
            Property(t => t.IsDeleted);
            Property(t => t.BizStatus);
            Property(t => t.CreateUser);
            Property(t => t.RegisterTime);
            Property(t => t.RegisterIP);
            Property(t => t.RegSource);
            Property(t => t.Owner);
            Property(t => t.Channel);
            Property(t => t.Device);
            Property(t => t.LastSignInTime);
            Property(t => t.LastSignOutTime);
            Property(t => t.LastSigninIP);
            Property(t => t.UpdateUser);
            Property(t => t.LastChangeTime);
            Property(t => t.OpenAccountApplyTime);
            Property(t => t.OpenAccountAuditTime);
            Property(t => t.OpenAccountAuditUser);
            Property(t => t.Field1);
            Property(t => t.Field2);
            Property(t => t.Field3);
            Property(t => t.Field4);
            Property(t => t.Field5);
            Property(t => t.Field6);
            Property(t => t.Field7);
            Property(t => t.Field8);
            Property(t => t.SexTheme);
            Property(t => t.Score);
            Property(t => t.RecommendCode);

         
        }
    }
}
