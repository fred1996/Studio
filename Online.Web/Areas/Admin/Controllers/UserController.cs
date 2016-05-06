using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Online.DbHelper.Model;
using Online.Web.DAL;
using PagedList;
using PagedList.Mvc;
using Aspose.Cells;
using System.IO;
using Online.Web.Help;
using System.Data.Entity;
using Online.DbHelper.Common;
using System.Reflection;
using Online.Web.Areas.Admin.Models;
using System.Diagnostics;

namespace Online.Web.Areas.Admin.Controllers
{
    [AdminAuthorize]
    public class UserController : BaseController
    {

        public ActionResult UserIndex(string searchString, int page = 1)
        {
            try
            {
                IEnumerable<Users> result;
                if (!String.IsNullOrEmpty(searchString))
                {
                    result = UserSource.Userses.Where(x => x.UserName.Contains(searchString) || x.Email.Contains(searchString) || x.Telephone.Contains(searchString) && !x.IsDeleted);
                    ViewBag.CurrentFilter = searchString;
                }
                else
                {
                    result = UserSource.Userses.Where(x => !x.IsDeleted).ToList();
                }
                ViewBag.Page = page;
                ViewBag.Menus = ReadMenu.Instance.Menues;
                //List<Users> users = UserSource.Userses.Where(x => !x.IsDeleted).ToList();
                return View(result.OrderByDescending(x => x.RegisterTime).ToPagedList(page, 10));
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpPost]
        public ActionResult CheckEmail(string email = "", string username = "")
        {
            var Mail = UserSource.Userses.FirstOrDefault(t => t.Email == email && !t.IsDeleted);
            if (Mail != null)
            {
                return Json("T");
            }
            var user = UserSource.Userses.FirstOrDefault(t => t.UserName == username && !t.IsDeleted);
            if (user != null)
            {
                return Json("G");
            }
            return Json("F");
        }

        public ActionResult AddUserInfo()
        {
            return View();
        }
        public ActionResult SaveAddUserInfo(Users userc)
        {
            try
            {
                string salt = Guid.NewGuid().ToString().Replace("-", "");
                var user = new Users();
                user.UserName = userc.UserName;
                user.Password = UntilHelper.GetMd5HashCode(userc.Password);
                user.Email = userc.Email;
                user.Salt = salt;
                user.Avatar = "";
                user.Telephone = "";
                user.BirthDay = Convert.ToDateTime("1900-01-01 00:00:00");
                user.IsEmailEffective = false;
                user.QQ = "";
                user.Weixin = "";
                user.LastChangeTime = DateTime.Now;
                user.RecommendCode = "";
                user.RegisterTime = DateTime.Now;
                user.BizStatus = 1;
                user.Owner = ProjectOwner;
                user.Channel = 1;
                user.Device = 1;
                UserSource.Userses.Add(user);
                UserSource.SaveChanges();
                UserRoles userRoles = new UserRoles()
                {
                    ProjectId = 0,//表示所有项目
                    RoleId = 1,//会员级别
                    UserId = user.UserID,
                    NickName = String.Empty,// reqModel.UserName,
                };
                UserSource.UserRoleses.Add(userRoles);
                UserSource.SaveChanges();

                GiftHandler.Instance.RegistAddGift(user);
                AddUpdateSettingLog("用户编辑", "用户管理中用户信息添加", 123);
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Write("添加用户Error:" + ex.Message, LogMessageType.Admin, GetType(), MethodBase.GetCurrentMethod().Name);
                throw;
            }
            return RedirectToAction("UserIndex");

        }
        /// <summary>
        /// 用户编辑视图
        /// </summary>
        /// <returns></returns>
        public ActionResult UserEdit(long id)
        {
            try
            {
                var user = UserSource.Userses.FirstOrDefault(x => x.UserID == id);
                return View(user);
            }
            catch (Exception)
            {
                return View("/UserIndex");
                throw;
            }
        }
        /// <summary>
        /// 用户资料详情
        /// </summary>
        /// <returns></returns>
        public ActionResult UserDetail(long id)
        {
            try
            {
                var user = UserSource.Userses.FirstOrDefault(x => x.UserID == id);
                ViewBag.Menus = ReadMenu.Instance.Menues;
                return View(user);
            }
            catch (Exception)
            {
                return View("/UserIndex");
                throw;
            }
        }
        public ActionResult AddUserKey()
        {
            return View();
        }
        public ActionResult AddUserKeySave(UC_Dictionarys uc)
        {
            try
            {
                uc.Description = uc.Description != null ? uc.Description : "";
                uc.FiledBaseName = uc.FiledBaseName != null ? uc.FiledBaseName : "";
                uc.FiledBaseValue = uc.FiledBaseValue != null ? uc.FiledBaseValue : "";
                uc.CreateTime = DateTime.Now;
                uc.OperatorName =CrmUsers.UserName;
                uc.UpdateTime = DateTime.Now;
                uc.ParentDictID = 0;
                UserSource.UcDictionaryses.Add(uc);
                UserSource.SaveChanges();
                AddUpdateSettingLog("用户字典", "用户字典管理数据添加操作", 121);
                return RedirectToAction("UserKeys");
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Write("用户字典添加Error:" + ex.Message, LogMessageType.Admin, GetType(), MethodBase.GetCurrentMethod().Name);
                throw;
            }
        }

        public ActionResult EditUserRole(long id)
        {
            List<Roles> ress = UserSource.Roleses.Where(t => t.BizStatus == 1).ToList();
            ViewBag.Roles = ress;
            var user_role = UserSource.UserRoleses.Where(x => x.UserId == id).ToList();
            return View(user_role);
        }

        /// <summary>
        /// 用户黑名单管理
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public ActionResult BlackList(int page = 1)
        {
            //IEnumerable<Users> result;
            //if (!String.IsNullOrEmpty(searchString))
            //{
            //    result = UserSource.Userses.Where(x => x.UserName.Contains(searchString) || x.Email.Contains(searchString) || x.Telephone.Contains(searchString) && !x.IsDeleted);
            //}
            //else
            //{
            //    result = UserSource.Userses.Where(x => !x.IsDeleted).ToList();
            //}
            ////List<Users> users = UserSource.Userses.Where(x => !x.IsDeleted).ToList();
            //return View(result.OrderByDescending(x => x.RegisterTime).ToPagedList(page, 10));
            IEnumerable<UserBlackList> result;
            result = UserSource.UserBlackLists.ToList();
            ViewBag.Menus = ReadMenu.Instance.Menues;
            return View(result.OrderByDescending(x => x.CreateTime).ToPagedList(page, 10));
        }

        /// <summary>
        /// 用户地址管理
        /// </summary>
        /// <returns></returns>
        public ActionResult UserAddress(string searchString, int page = 1)
        {
            try
            {
                IEnumerable<UserAddress> result;
                if (!String.IsNullOrEmpty(searchString))
                {
                    result = UserSource.UserAddresses.Where(x => x.Email.Contains(searchString) || x.Telephone.Contains(searchString)).ToList();
                }
                else
                {
                    result = UserSource.UserAddresses.OrderByDescending(x => x.AddressID).ToList();
                }
                ViewBag.Menus = ReadMenu.Instance.Menues;
                //var s = from ud in UserSource.UserAddresses join uu in UserSource.Userses on ud.UserID equals uu.UserID select new UserAddress {UserName="" };
                return View(result.OrderByDescending(x => x.AddressID).ToPagedList(page, 10));
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ActionResult UserKeys(string searchString, int page = 1)
        {
            try
            {
                IEnumerable<UC_Dictionarys> result;
                if (!String.IsNullOrEmpty(searchString))
                {
                    result = UserSource.UcDictionaryses.Where(x => x.FiledBaseValue.Contains(searchString) || x.FiledBaseName.Contains(searchString)).ToList();
                }
                else
                {
                    result = UserSource.UcDictionaryses.OrderByDescending(x => x.DictID).ToList();
                }
                ViewBag.Menus = ReadMenu.Instance.Menues;
                //var s = from ud in UserSource.UserAddresses join uu in UserSource.Userses on ud.UserID equals uu.UserID select new UserAddress {UserName="" };
                return View(result.OrderByDescending(x => x.DictID).ToPagedList(page, 10));
            }
            catch (Exception)
            {
                throw;
            }
        }
        public ActionResult RemoveUkeyDel(long did)
        {
            try
            {
                var ukey = UserSource.UcDictionaryses.Where(x => x.DictID == did).FirstOrDefault();
                if (ukey != null)
                {
                    UserSource.UcDictionaryses.Remove(ukey);
                    UserSource.SaveChanges();
                    AddUpdateSettingLog("用户字典", "用户字典管理数据删除操作", 108);
                }
                return RedirectToAction("UserKeys");
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Write("用户字典删除Error:" + ex.Message, LogMessageType.Admin, GetType(), MethodBase.GetCurrentMethod().Name);
                throw;
            }
        }
        public ActionResult EditUserKey(long did)
        {
            try
            {
                var ukey = UserSource.UcDictionaryses.Where(x => x.DictID == did).FirstOrDefault();
                return View(ukey);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public ActionResult EditUserKeySave(UC_Dictionarys uc)
        {
            try
            {
                var modeladdress = UserSource.UcDictionaryses.FirstOrDefault(x => x.DictID == uc.DictID);
                if (modeladdress != null)
                {
                    modeladdress.FiledBaseName = uc.FiledBaseName != null ? uc.FiledBaseName : "";
                    modeladdress.FiledBaseValue = uc.FiledBaseValue != null ? uc.FiledBaseValue : "";
                    modeladdress.Description = uc.Description != null ? uc.Description : "";
                    UserSource.Entry(modeladdress).State = EntityState.Modified;
                    UserSource.SaveChanges();
                    AddUpdateSettingLog("用户字典", "用户字典管理数据编辑操作", 107);
                }
                return RedirectToAction("UserKeys");
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Write("用户字典Error:" + ex.Message, LogMessageType.Admin, GetType(), MethodBase.GetCurrentMethod().Name);
                throw;
            }
        }
        public ActionResult EditUserAddress(long id)
        {
            try
            {
                var modeladdress = UserSource.UserAddresses.FirstOrDefault(x => x.AddressID == id);
                if (modeladdress != null)
                {
                    return View(modeladdress);
                }
                return View(modeladdress);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpPost]
        public ActionResult EditUserAddress(UserAddress address)
        {
            try
            {
                var modeladdress = UserSource.UserAddresses.FirstOrDefault(x => x.AddressID == address.AddressID);
                if (modeladdress != null)
                {
                    modeladdress.Country = address.Country != null ? address.Country : "";
                    modeladdress.Province = address.Province != null ? address.Province : "";
                    modeladdress.City = address.City != null ? address.City : "";
                    modeladdress.Area = address.Area != null ? address.Area : "";
                    modeladdress.Email = address.Email != null ? address.Email : "";
                    modeladdress.Telephone = address.Telephone != null ? address.Telephone : "";
                    modeladdress.DetailInfo = address.DetailInfo != null ? address.DetailInfo : "";
                    UserSource.Entry(modeladdress).State = EntityState.Modified;
                    UserSource.SaveChanges();
                    AddUpdateSettingLog("用户地址管理", "用户地址管理数据编辑操作", 106);
                }
                return RedirectToAction("UserAddress");
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Write("用户地址管理Error:" + ex.Message, LogMessageType.Admin, GetType(), MethodBase.GetCurrentMethod().Name);
                throw;
            }
        }
        public ActionResult UserAddressRemove(long address)
        {
            try
            {
                var addressmodel = UserSource.UserAddresses.FirstOrDefault(x => x.AddressID == address);
                if (addressmodel != null)
                {
                    UserSource.UserAddresses.Remove(addressmodel);
                    UserSource.SaveChanges();
                    AddUpdateSettingLog("用户地址管理", "用户地址管理数据删除操作", 105);

                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Write("用户黑名单删除Error:" + ex.Message, LogMessageType.Admin, GetType(), MethodBase.GetCurrentMethod().Name);
                throw;
            }
            return RedirectToAction("UserAddress");
        }
        public ActionResult RemoveBlackList(long id)
        {
            try
            {
                var user = UserSource.UserBlackLists.FirstOrDefault(x => x.BlackListId == id);
                if (user != null)
                {
                    UserSource.UserBlackLists.Remove(user);
                    UserSource.SaveChanges();
                    AddUpdateSettingLog("用户黑名单删除", "用户黑名单数据删除操作", 104);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Write("用户黑名单删除Error:" + ex.Message, LogMessageType.Admin, GetType(), MethodBase.GetCurrentMethod().Name);
                throw;
            }
            return RedirectToAction("BlackList");
        }
        [HttpPost]
        public ActionResult EditUserRole(FormCollection formcol, long UserId)
        {
            try
            {
                if (formcol.GetValues("roles") != null)
                {
                    string strRoles = formcol.GetValue("roles").AttemptedValue;
                    string[] lsroles = strRoles.Split(',');
                    if (lsroles.Any())
                    {
                        List<UserRoles> roues = UserSource.UserRoleses.Where(x => x.UserId == UserId).ToList();
                        UserSource.UserRoleses.RemoveRange(roues);
                        UserSource.SaveChanges();
                    }
                    foreach (var item in lsroles)
                    {
                        var model = new UserRoles
                        {
                            UserId = UserId,
                            ProjectId = 1,
                            RoleId = Convert.ToInt32(item)
                        };
                        UserSource.UserRoleses.Add(model);
                    }
                    UserSource.SaveChanges();
                }
                AddUpdateSettingLog("用户角色编辑", "用户管理中用户角色编辑", 103);
                return RedirectToAction("UserIndex");
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Write("用户角色编辑Error:" + ex.Message, LogMessageType.Admin, GetType(), MethodBase.GetCurrentMethod().Name);
                throw;
            }
        }
        [HttpPost]
        public ActionResult UserEdit(Users user)
        {
            try
            {
                var usermodel = UserSource.Userses.FirstOrDefault(x => x.UserID == user.UserID);
                usermodel.UserName = user.UserName;
                if (!Guid.Equals(usermodel.Password, user.Password))
                {
                    usermodel.Password = UntilHelper.GetMd5HashCode(user.Password);
                }
                usermodel.RealName = user.RealName != null ? user.RealName : "";
                usermodel.IDCard = user.IDCard != null ? user.IDCard : "";
                usermodel.Sex = user.Sex;
                usermodel.QQ = user.QQ != null ? user.QQ : "";
                usermodel.Weixin = user.Weixin != null ? user.Weixin : "";
                usermodel.IsDeleted = user.IsDeleted;
                //手动修改实体的状态
                UserSource.Entry(usermodel).State = EntityState.Modified;
                UserSource.SaveChanges();
                AddUpdateSettingLog("用户编辑", "用户管理中用户信息编辑", 102);
                return RedirectToAction("UserIndex");
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Write("用户编辑Error:" + ex.Message, LogMessageType.Admin, GetType(), MethodBase.GetCurrentMethod().Name);
                return RedirectToAction("UserIndex");
                throw;
            }
        }
        /// <summary>
        /// 根据ID删除用户
        /// </summary>
        /// <returns>1-删除成功,0-删除失败</returns>
        public ActionResult RemoveUserInfo(long id)
        {
            try
            {
                var user = UserSource.Userses.FirstOrDefault(x => x.UserID == id);
                if (user != null)
                {
                    user.IsDeleted = true;
                    UserSource.SaveChanges();
                    //LogHelper.Instance.Write("用户列表管理数据删除操作！", LogMessageType.Admin, GetType(), MethodBase.GetCurrentMethod().Name);
                    AddUpdateSettingLog("删除用户", "后台根据用户ID删除用户信息", 100);
                    return Json(1, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Write("删除用户Error:" + ex.Message, LogMessageType.Admin, GetType(), MethodBase.GetCurrentMethod().Name);
                return Json(0, JsonRequestBehavior.AllowGet);
                throw;
            }
        }
        /// <summary>
        /// 导出当前用
        /// </summary>
        /// <returns></returns>
        public FileResult ExportUserExcel(string searchString, int page = 1)
        {
            try
            {
                IEnumerable<Users> result;
                if (!String.IsNullOrEmpty(searchString))
                {
                    result = UserSource.Userses.Where(x => x.UserName.Contains(searchString) || x.Email.Contains(searchString) || x.Telephone.Contains(searchString)).OrderByDescending(x => x.RegisterTime).ToPagedList(page, 10);
                }
                else
                {
                    result = UserSource.Userses.Where(x => !x.IsDeleted).OrderByDescending(x => x.RegisterTime).ToPagedList(page, 10);
                }
                string path = PrepareFolder() + "Export.xls";
                Aspose.Cells.Workbook workbook = new Aspose.Cells.Workbook();   //实例化
                                                                                //workbook.Open(path);
                Aspose.Cells.Worksheet sheet = workbook.Worksheets[0];
                Cells cells = sheet.Cells;//单元格
                var s = sheet.Cells.Rows.Count;
                //清空之前的数据
                //for (int t = 2; t < s; t++)
                //{
                //    sheet.Cells.DeleteRows(t, s, true);
                //}
                //添加标头
                cells[0, 0].PutValue("ID");
                cells[0, 1].PutValue("邮箱");
                cells[0, 2].PutValue("用户名");
                cells[0, 3].PutValue("性别");
                cells[0, 4].PutValue("手机");
                cells[0, 5].PutValue("QQ");
                cells[0, 6].PutValue("注册时间");
                Aspose.Cells.Style style3 = workbook.Styles[workbook.Styles.Add()];//新增样式 
                style3.HorizontalAlignment = TextAlignmentType.Center;//文字居中 

                int j = 1;
                foreach (var item in result)
                {
                    cells[j, 0].PutValue(item.UserID);
                    cells[j, 1].PutValue(item.Email);
                    cells[j, 2].PutValue(item.UserName);
                    cells[j, 3].PutValue(item.Sex == 0 ? "男" : "女");
                    cells[j, 4].PutValue(item.Telephone);
                    cells[j, 5].PutValue(item.QQ);
                    cells[j, 6].PutValue(item.RegisterTime.ToString("yyyy-MM-dd hh:mm:ss"));
                    j++;
                }
                workbook.Save(path, SaveFormat.Auto);
                var fileName = Server.MapPath("~/Image/Excel/Export.xls");
                AddUpdateSettingLog("导出数据", "导出当前页用户数据", 101);
                return File(fileName, "application/ms-excel", "" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".xls");
                //FileStream fs = new FileStream(fileName, FileMode.Open);
                //byte[] bytes = new byte[(int)fs.Length];
                //fs.Read(bytes, 0, bytes.Length);
                //fs.Close();
                //Response.Charset = "UTF-8";
                //Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
                //Response.ContentType = "application/octet-stream";
                //Response.AddHeader("Content-Disposition", "attachment; filename=" + Server.UrlEncode(fileName));
                //Response.BinaryWrite(bytes);
                //Response.Flush();
                //Response.End();
                //return new EmptyResult();
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Write("导出数据Error:" + ex.Message, LogMessageType.Admin, GetType(), MethodBase.GetCurrentMethod().Name);
                throw;
            }
        }
        string PrepareFolder()
        {
            string path = Server.MapPath("~/Image/Excel/");
            string filepath = Server.MapPath("~/Image/Excel/Export.xls");
            if (System.IO.File.Exists(filepath)) System.IO.File.Delete(filepath);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return path;
        }
        ///// <summary>
        ///// 标题{删除/添加/修改}
        ///// 描述
        ///// 类型后台100依次相加
        ///// </summary>
        ///// <param name="title"></param>
        ///// <param name="description"></param>
        ///// <param name="type"></param>
        //private void AddUpdateSettingLog(string title, string description, int type)
        //{
        //    var entity = new UserActionLog();
        //    entity.UserId = UserId;
        //    entity.Title = title;
        //    entity.Description = description;
        //    entity.Type = type;
        //    entity.ProjectId = ProjectId;
        //    entity.UserIp = ClientIp;
        //    entity.CreateTime = DateTime.Now;
        //    entity.RoomId = RoomId;
        //    DataQueueDal.Instance.Add(entity);
        //}

        #region 用户助理

        public ActionResult UserRelationAssistantIndex(int page = 1, string condition = "")
        {
            ViewBag.Menus = ReadMenu.Instance.Menues;
            var list = UserSource.UserRelationAssistants.Where(t => !t.IsDelete);
            if (!string.IsNullOrEmpty(condition))
                list = list.Where(t => t.Assistant.UserName.Contains(condition) || t.UserName.Contains(condition));
            var pagelist = list.OrderByDescending(t => t.CreateDateTime).ToPagedList(page, 10);
            ViewBag.Codition = condition;
            return View(pagelist);
        }

        public ActionResult UserRelationAssistantEdit(Guid id)
        {
            var roles = UserSource.Roleses.FirstOrDefault(t => t.RoleName == "助理");
            var list = roles.UserRoleses.Select(t => t.UserId).OrderByDescending(r => r);
            var assistants = UserSource.Userses.Where(t => list.Contains(t.UserID)).Select(t => new SelectListItem()
            {
                Text = t.UserName,
                Value = t.UserID.ToString()
            });

            var model = UserSource.UserRelationAssistants.FirstOrDefault(t => t.Id == id);
            ViewBag.Assistants = assistants;
            return View(model);
        }

        public ActionResult UserRelationAssistantEditSave(UserRelationAssistant model)
        {
            var relation = UserSource.UserRelationAssistants.FirstOrDefault(t => t.Id == model.Id);
            relation.UserName = model.UserName == null ? "" : model.UserName;
            var userid = 0;
            relation.UserId = int.TryParse(model.UserId.ToString(), out userid) ? model.UserId : 0;
            relation.AssistantId = model.AssistantId;
            UserSource.SaveChanges();
            AddUpdateSettingLog("用户助理管理", "修改用户助理关系", 122);
            return RedirectToAction("UserRelationAssistantIndex");
        }

        public ActionResult UserRelationAssistantCreate()
        {
            var roles = UserSource.Roleses.FirstOrDefault(t => t.PowerId == 85);
            var list = roles.UserRoleses.Select(t => t.UserId).OrderByDescending(r => r);
            var assistants = UserSource.Userses.Where(t => list.Contains(t.UserID)).Select(t => new SelectListItem()
            {
                Text = t.UserName,
                Value = t.UserID.ToString()
            });
            ViewBag.Assistants = assistants;
            return View();
        }

        public ActionResult UserRelationAssistantCreateSave(UserRelationAssistant model)
        {
            var userid = 0;
            model.UserId = int.TryParse(model.UserId.ToString(), out userid) ? model.UserId : 0;
            model.CreateDateTime = DateTime.Now;
            model.UserName = model.UserName == null ? "" : model.UserName;
            model.Id = Guid.NewGuid();
            UserSource.UserRelationAssistants.Add(model);
            UserSource.SaveChanges();
            AddUpdateSettingLog("用户助理管理", "新增用户助理关系", 123);
            return RedirectToAction("UserRelationAssistantIndex");
        }

        public ActionResult UserRelationAssistantRemove(Guid id)
        {
            var model = UserSource.UserRelationAssistants.FirstOrDefault(t => t.Id == id);
            UserSource.UserRelationAssistants.Remove(model);
            UserSource.SaveChanges();
            AddUpdateSettingLog("用户助理管理", "删除用户助理关系", 124);
            return RedirectToAction("UserRelationAssistantIndex");
        }

        #endregion
    }
}