using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Online.DbHelper.BLL;
using Online.DbHelper.Model;
using Online.Web.Help;
using ServiceStack;

namespace Online.Web.DAL
{
    public class AccountHandler
    {

        public static AccountHandler Current
        {
            get { return new AccountHandler(); }
        }

        public Users UpdateUser(Users model)
        {
            using (var db = new UserContextBll())
            {
                var entity = db.Userses.FirstOrDefault(t => t.UserID == model.UserID);
                entity.Avatar = model.Avatar;
                entity.RealName = model.RealName;
                entity.QQ = model.QQ;
                entity.Weixin = model.Weixin;
                entity.Sex = model.Sex;
                entity.InvestmentType = model.InvestmentType;
                entity.UserName = model.UserName;
                db.SaveChanges();
                return entity;
            }

        }
 
        public Users UpdateTags(string tags, long userid)
        {
            using (var db = new UserContextBll())
            {
                var entity = db.Userses.FirstOrDefault(t => t.UserID == userid);
                entity.UserTags = tags;
                db.SaveChanges();
                return entity;
            }
        }

        public int AddressInsert(UserAddress address)
        {
            using (var db = new UserContextBll())
            {
                db.UserAddresses.Add(address);
                return db.SaveChanges();
            }
        }

        public int AddressUpdate(UserAddress address)
        {
            using (var db = new UserContextBll())
            {
                var model = db.UserAddresses.FirstOrDefault(t => t.AddressID == address.AddressID);
                model.Email = address.Email;
                model.Telephone = address.Telephone;
                model.Province = address.Province;
                model.City = address.City;
                model.Area = address.Area;
                model.DetailInfo = address.DetailInfo;
                return db.SaveChanges();
            }
        }

        public int AddressDel(int addressid)
        {
            using (var db = new UserContextBll())
            {
                var model = db.UserAddresses.FirstOrDefault(t => t.AddressID == addressid);
                db.UserAddresses.Remove(model);
                return db.SaveChanges();
            }
        }

        public bool VerifyTel(long userid)
        {
            using (var db = new UserContextBll())
            {
                var entity = db.Userses.FirstOrDefault(t => t.UserID == userid);
                entity.IsTelphoneEffective = true;
                db.Entry(entity).State = EntityState.Modified;
                var result = db.SaveChanges();
                return result > 0;
            }
        }

        public bool VerifyEmail(long userid)
        {
            using (var db = new UserContextBll())
            {
                var entity = db.Userses.FirstOrDefault(t => t.UserID == userid);
                entity.IsEmailEffective = true;
                db.Entry(entity).State = EntityState.Modified;
                var result = db.SaveChanges();
                return result > 0;
            }
        }

        public bool ChangePassword(long userid, string password)
        {
            using (var db = new UserContextBll())
            {
                var entity = db.Userses.FirstOrDefault(t => t.UserID == userid);
                var salt = Guid.NewGuid().ToString().Replace("-", "");
                var newword = UntilHelper.GetMd5HashCode(password);
                entity.Salt = salt;
                entity.Password = newword;
                db.Entry(entity).State = EntityState.Modified;
                var result = db.SaveChanges();
                return result > 0;
            }
        }

        public bool FindPassword(string userName, string email)
        {
            int password = new Random().Next(100000, 999999);
            //密码盐
            string salt = Guid.NewGuid().ToString().Replace("-", "");
            //密码盐+用户输入的密码，进行MD5加密
            string strPassword = UntilHelper.GetMd5HashCode(password.ToString());
            //异步发送邮件
            Task.Factory.StartNew(() =>
            {
                MailHelp.FindPassword(email, password.ToString());
            });
            using (var db = new UserContextBll())
            {
                var model = db.Userses.FirstOrDefault(t => t.UserName == userName && t.Email == email);
                model.Salt = salt;
                model.Password = strPassword;
                db.Entry(model).State = EntityState.Modified;
                var result = db.SaveChanges();
                return result > 0;
            }
        }

        public bool UserRegister(Users user)
        {
            using (var db = new UserContextBll())
            {
                DbConnection con = ((IObjectContextAdapter)db).ObjectContext.Connection;
                con.Open();
                using (var tran = con.BeginTransaction())
                {
                    try
                    {
                        db.Userses.Add(user);
                        db.SaveChanges();
                        UserRoles userRoles = new UserRoles()
                        {
                            ProjectId = 0,//表示所有项目
                            RoleId = Convert.ToInt32(ConfigurationManager.AppSettings["DefaultRoleLevel"]),//会员级别
                            UserId = user.UserID,
                            NickName = String.Empty,// reqModel.UserName,
                        };
                        db.UserRoleses.Add(userRoles);
                        db.SaveChanges();
                        tran.Commit();
                    }
                    catch (Exception exception)
                    {
                        tran.Rollback();
                        return false;
                    }

                }
                return true;
            }


        }
    }

}
