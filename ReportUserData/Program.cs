using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Online.DbHelper.BLL;
using Online.DbHelper.Model;

namespace ReportUserData
{
    class Program
    {
        static void Main(string[] args)
        {
            InserData();
            Console.WriteLine("导入完成");
            Console.ReadLine();
        }


        public static void InserData()
        {
            try
            {
                var iusers = new List<i_User_Info>();
              
                int Updatecount = 0;
                int AddCount = 0;
                using (var db1 = new VinvContextBll())
                {
                    iusers = db1.i_User_Info.ToList();
                }
                using (var db = new NewUserContentBll())
                {
                    db.Database.CommandTimeout = 60*60*1000;
                    foreach (var item in iusers)
                    {
                        try
                        {
                            bool isUpdate = true;
                            Users dbmodel;
                            var entity = db.Userses.FirstOrDefault(t => t.Email == item.i_User_Mail);
                            if (entity == null)
                            {
                                dbmodel = new Users();
                                dbmodel.UserID = Convert.ToInt64(item.i_User_ID);
                                isUpdate = false;
                            }
                            else
                            {
                                dbmodel = entity;
                            }
                         
                            dbmodel.UserName =string.IsNullOrEmpty(item.i_User_NickName)?item.i_User_Name:item.i_User_NickName;
                            dbmodel.Salt = string.Empty;
                            dbmodel.Password = item.i_User_Pass;
                            dbmodel.RealName = item.i_User_RealName == null ? String.Empty : item.i_User_RealName;
                            dbmodel.Avatar = string.Empty;
                            dbmodel.Sex = item.i_User_Sex;
                            dbmodel.BirthDay =string.IsNullOrEmpty(item.i_User_Birthday) ? Convert.ToDateTime("1911-01-01") : Convert.ToDateTime(item.i_User_Birthday);
                            dbmodel.Email = item.i_User_Mail;
                            dbmodel.IsEmailEffective = !isUpdate ? false : dbmodel.IsEmailEffective;
                            dbmodel.QQ = item.i_User_QQ == null ? string.Empty : item.i_User_QQ;
                            dbmodel.Weixin = !isUpdate ? String.Empty : dbmodel.Weixin;
                            dbmodel.Weibo = !isUpdate ? String.Empty : dbmodel.Weibo;
                            dbmodel.IDCard = !isUpdate ? String.Empty : dbmodel.IDCard;
                            dbmodel.Telephone = item.i_User_Tel;
                            dbmodel.IsTelphoneEffective = !isUpdate ? false : dbmodel.IsTelphoneEffective; ;
                            dbmodel.InvestmentType = 0;
                            dbmodel.UserTags = String.Empty;
                            dbmodel.IsDeleted = false;
                            dbmodel.BizStatus = 1;
                            dbmodel.CreateUser = String.Empty;
                            dbmodel.RegisterTime = item.i_User_RegTime == null ? DateTime.Now : Convert.ToDateTime(item.i_User_RegTime);
                            dbmodel.RegisterIP = string.Empty;
                            dbmodel.RegSource = string.Empty;
                            dbmodel.Owner =   0;
                            dbmodel.Channel = 0;
                            dbmodel.Device = 0;
                            dbmodel.LastSignInTime = item.i_User_RegTime == null ? DateTime.Now : Convert.ToDateTime(item.i_User_RegTime);
                            dbmodel.LastSignOutTime = item.i_User_RegTime == null ? DateTime.Now : Convert.ToDateTime(item.i_User_RegTime);
                            dbmodel.LastSigninIP = item.i_User_LoginIP;
                            dbmodel.UpdateUser = string.Empty;
                            dbmodel.LastChangeTime = DateTime.Now;
                            dbmodel.OpenAccountApplyTime = DateTime.Now;
                            dbmodel.OpenAccountAuditTime = DateTime.Now;
                            dbmodel.OpenAccountAuditUser = String.Empty;
                            dbmodel.Field1 = String.Empty;
                            dbmodel.Field2 = String.Empty;
                            dbmodel.Field3 = String.Empty;
                            dbmodel.Field4 = String.Empty;
                            dbmodel.Field5 = String.Empty;
                            dbmodel.Field6 = String.Empty;
                            dbmodel.Field7 = String.Empty;
                            dbmodel.Field8 = String.Empty;
                            dbmodel.SexTheme = String.Empty;
                            dbmodel.RecommendCode = string.Empty;
                            dbmodel.Score = 0;
                            dbmodel.Owner =   0;
                            var oldrole = Enum.GetName(typeof(OldRole), item.i_User_RoleID);
                            var roleid = db.Roleses.FirstOrDefault(t => t.RoleName == oldrole);
                            if (isUpdate)
                            {
                                var userroles =
                                    db.UserRoleses.FirstOrDefault(t => t.UserId == dbmodel.UserID && t.RoleId == roleid.RoleID);
                                if (userroles != null)
                                {
                                    userroles.RoleId = roleid.RoleID;
                                    db.Entry(userroles).State = EntityState.Modified;
                                    db.SaveChanges();
                                }
                                else
                                {
                                    UserRoles userRoles = new UserRoles()
                                    {
                                        ProjectId = 1,//表示所有项目
                                        RoleId = roleid == null ? 0 : roleid.RoleID,//会员级别
                                        UserId = dbmodel.UserID,
                                        NickName = String.Empty,// reqModel.UserName,
                                    };
                                    db.UserRoleses.Add(userRoles);
                                    db.SaveChanges();

                                }
                                db.Entry(dbmodel).State = EntityState.Modified;
                                int count = db.SaveChanges();
                                if (count > 0) Updatecount++;
                            }
                            else
                            {
                                db.Userses.Add(dbmodel);
                                db.SaveChanges();

                                UserRoles userRoles = new UserRoles()
                                {
                                    ProjectId = 1,//表示所有项目
                                    RoleId = roleid == null ? 0 : roleid.RoleID,//会员级别
                                    UserId = dbmodel.UserID,
                                    NickName = String.Empty,// reqModel.UserName,
                                };
                                db.UserRoleses.Add(userRoles);
                                var count = db.SaveChanges();
                                if (count > 0) AddCount++;
                            }
                        }
                        catch(Exception ex)
                        {

                        }
                       
                    };
               
                }
                Console.WriteLine("总行数：" + iusers.Count);
                Console.WriteLine("新增行数：" + AddCount);
                Console.WriteLine("修改行数：" + Updatecount);
            }
            catch (Exception ex)
            {

                throw;
            }

        }
    }
}
