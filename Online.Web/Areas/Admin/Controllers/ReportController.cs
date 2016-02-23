using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Online.DbHelper.Model;
using Online.Web.DAL;
using Online.DbHelper.Common;
using PagedList;
using PagedList.Mvc;
using Online.Web.Help;
using Online.Web.Areas.Admin.Models;
using System.Reflection;

namespace Online.Web.Areas.Admin.Controllers
{
    [AdminAuthorize]
    public class ReportController : BaseController
    {
        // GET: Admin/Report
        public ActionResult Index(string searchString, string reservation, string endreservation, int page = 1)
        {
            IEnumerable<Users> result = UserSource.Userses.Where(x => !x.IsDeleted).ToList();
            int s = 0;
            if (!String.IsNullOrEmpty(searchString))
            {
                if (int.TryParse(searchString, out s))
                {
                    result = result.Where(x => !x.IsDeleted && (x.UserName.Contains(searchString) || x.Telephone.Contains(searchString) || x.Email.Contains(searchString) || x.UserID == s)).ToList();
                    ViewBag.CurrentFilter = searchString;
                }
                else
                {
                    result = result.Where(x => !x.IsDeleted && (x.UserName.Contains(searchString) || x.Telephone.Contains(searchString) || x.Email.Contains(searchString))).ToList();
                    ViewBag.CurrentFilter = searchString;
                }
            }
            //else
            //{
            //    result = UserSource.Userses.Where(x => !x.IsDeleted).ToList();
            //    ViewBag.CurrentFilter = searchString;
            //}
            if (!String.IsNullOrEmpty(reservation) && !String.IsNullOrEmpty(endreservation))
            {
                DateTime begintime = Convert.ToDateTime(reservation);
                DateTime endtime = Convert.ToDateTime(endreservation);
                result = result.Where(x => x.RegisterTime >= begintime && x.RegisterTime <= endtime);
                //result = result.Where(x => DateTime.Compare(Convert.ToDateTime(x.RegisterTime.ToString("yyyy-MM-dd hh:mm")), begintime) >= 0&& DateTime.Compare(Convert.ToDateTime(x.RegisterTime.ToString("yyyy-MM-dd hh:mm")), endtime) <= 0);
                ViewBag.reservation = begintime;
                ViewBag.endreservation = endtime;
            }
            ViewBag.Menus = ReadMenu.Instance.Menues;
            return View(result.OrderByDescending(x => x.RegisterTime).ToPagedList(page, 10));
        }

        public ActionResult EditIndex(long userid)
        {
            var user = UserSource.UserRoleses.Where(t => t.UserId == userid).ToList();
            var list = UserSource.Roleses.Where(x => x.PowerId < 90).OrderBy(r => r.PowerId);
            ViewBag.roleList = list;
            return View(user);
        }

        /// <summary>
        /// 编辑注册用户密码和角色
        /// </summary>
        /// <param name="roles"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EdidIndexSave(FormCollection formcol, string password, long? UserId)
        {
            try
            {
                if (formcol.GetValues("roles") != null)
                {
                    string strRoles = formcol.GetValue("roles").AttemptedValue;
                    string[] lsroles = strRoles.Split(',');
                    if (lsroles.Count() > 0)
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
                if (!String.IsNullOrEmpty(password))
                {
                    var newpwd = UntilHelper.GetMd5HashCode(password);
                    var user = UserSource.Userses.FirstOrDefault(t => t.UserID == UserId);
                    user.Password = newpwd;
                    UserSource.SaveChanges();
                }
                //var model = UserSource.UserRoleses.Where(x => x.AutoId == roles.AutoId).FirstOrDefault();
                //model.RoleId = roles.RoleId;
                //if (!string.IsNullOrEmpty(password))
                //{
                //    var newpwd = UntilHelper.GetMd5HashCode(password);
                //    var user = UserSource.Userses.FirstOrDefault(t => t.UserID == roles.UserId);
                //    user.Password = newpwd;
                //}
                //UserSource.SaveChanges();
                AddUpdateSettingLog("每日注册", "用户信息修改操作", 120);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Write("每日注册量Error:" + ex.Message, LogMessageType.Admin, GetType(), MethodBase.GetCurrentMethod().Name);
                throw;
            }
        }
    }
}