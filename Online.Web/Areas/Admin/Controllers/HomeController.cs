using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Online.Web.Areas.Admin.Models;
using Online.Web.DAL;
using Online.Web.Help;

namespace Online.Web.Areas.Admin.Controllers
{
    [AdminAuthorize]
    public class HomeController : BaseController
    {
        
        public ActionResult Index()
        {
            ViewBag.Menus = ReadMenu.Instance.Menues;
            return View();
        }

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            var model = new LoginViewModel();
            return View(model);
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = CheckLogin(model);
            if (result) return RedirectToLocal(returnUrl);
            return View(model);
        }

        private bool CheckLogin(LoginViewModel model)
        {
            var user = UserSource.Userses.FirstOrDefault(t => (t.UserName == model.UserName || t.Email == model.UserName) && !t.IsDeleted);
            if (user == null)
            {
                model.Message = "用户名或密码不正确";
                return false;
            }
            var password = UntilHelper.GetMd5HashCode(model.Password);
            if (password == user.Password)
            {
                var roleList = UserSource.UserRoleses.Where(t => t.UserId == user.UserID).ToList();
                if (roleList.FirstOrDefault(t => t.Roles.RoleName == "管理员") == null) //不是管理员
                {
                    //根据角色获取权限
                    var entity = roleList.FirstOrDefault(t => t.Roles.PowerId >= 1000);
                    if (entity != null)
                    {
                        var roleid = entity.RoleId;
                        var permissions = UserSource.Role_Permissions.Where(t => t.RoleID == roleid).ToList();
                        ReadMenu.Instance.GetMenu(string.Join(",", permissions.Select(t => t.Permissions.PName).ToArray()).Split(',').ToList());
                    }
                }
                else
                {
                    ReadMenu.Instance.GetMenu(new List<string>());
                }
                if (ReadMenu.Instance.Menues.Count == 0)
                {
                    model.Message = "您没有权限登陆！";
                    return false;
                }
                else
                {
                    SaveCrmUserId(user.UserID);
                    return true;
                }
            }
            else
            {
                model.Message = "用户名或密码不正确";
                return false;
            }
        }


        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        public ActionResult LoginOut()
        {
            ClearCookie();
            Session["UserId"] = 0;
            return RedirectToAction("Login");
        }
    }
}