using System;
using System.Collections;
using System.Configuration;
using System.Data.Common;
using System.Data.Entity.Infrastructure;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Online.DbHelper.BLL;
using Online.DbHelper.Common;
using Online.DbHelper.Model;
using Online.Web.DAL;
using Online.Web.Help;
using Online.Web.Models;

namespace Online.Web.Controllers
{
    [SiteAuthorize]
    public class AccountController : BaseController
    {
        public AccountController()
        {
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        public ActionResult LoginOut()
        {
            SaveUserId(0);
            ClearCookie();
            return View("Login");
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            try
            {
                if (!ModelState.IsValid || !model.Code.Equals(Session["VerifyCode"].ToString(), StringComparison.OrdinalIgnoreCase))
                {
                    if (model.Code == null || !model.Code.Equals(Session["VerifyCode"].ToString(), StringComparison.OrdinalIgnoreCase))
                        ViewBag.Message = "验证码错误";
                    return View(model);
                }
                // 这不会计入到为执行帐户锁定而统计的登录失败次数中
                // 若要在多次输入错误密码的情况下触发帐户锁定，请更改为 shouldLockout: true
                var result = CheckLogin(model);
                if (result) return RedirectToLocal(returnUrl);
                ViewBag.Message = "用户名或者密码错误，请重新输入";
                //Response.Write("<script>alert('用户名或者密码错误，请重新输入')</script>");
                return View(model);
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteError(ex, GetType(), MethodBase.GetCurrentMethod().Name);
                throw;
            }
        }
 
        public bool CheckLogin(LoginViewModel model)
        {
            var user = UserSource.Userses.FirstOrDefault(t => (t.UserName == model.Email || t.Email == model.Email) && !t.IsDeleted);
            if (user == null) return false;
            var password = UntilHelper.GetMd5HashCode(model.Password);
            if (password == user.Password)
            {
                SaveUserId(user.UserID);
                return true;
            }
            return false;
        }

        [AllowAnonymous]
        public ActionResult Register()
        {
            var model = new UserRegisterViewModel() { Message = string.Empty,Token = UntilHelper.Token};
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(UserRegisterViewModel model)
        {
            try
            {
                string message = string.Empty;
                if (!ModelState.IsValid || !CheckRegister(model, out message))
                {
                    model.Message = message;
                    return View(model);
                }
                // 如果我们进行到这一步时某个地方出错，则重新显示表单
                var result = AddUser(model);
                if (result)
                {
                    return RedirectToAction("Index", "Home"); //View("Login");
                }
                model.Message = "注册失败";
                return View(model);
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteError(ex, GetType(), MethodBase.GetCurrentMethod().Name);
                throw;
            }
        }

        private bool AddUser(UserRegisterViewModel model)
        {
            try
            {
                string recommendCode;
                do
                {
                    recommendCode = UntilHelper.CreateRandomCode(6);
                } while (UserSource.Userses.Any(x => x.RecommendCode == recommendCode));
                string salt = Guid.NewGuid().ToString().Replace("-", "");
                var user = new Users();
                user.UserName = model.UserName;
                user.Password = UntilHelper.GetMd5HashCode(model.Password);
                user.Email = model.Email;
                user.Salt = salt;
                user.Avatar = "";
                user.Telephone = model.Telephone;
                user.BirthDay = Convert.ToDateTime("1900-01-01 00:00:00");
                user.IsEmailEffective = false;
                user.QQ = string.IsNullOrEmpty(model.QQ) ? "" : model.QQ;
                user.Weixin = "";
                user.LastChangeTime = DateTime.Now;
                user.RecommendCode = recommendCode;
                user.RegisterTime = DateTime.Now;
                user.BizStatus = 1;
                user.Owner = ProjectOwner;
                user.Channel = 1;
                user.Device = 1;
                return UserRegister(user);
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteError(ex, GetType(), MethodBase.GetCurrentMethod().Name);
                throw;
            }
        }

        private bool UserRegister(Users user)
        {
            try
            {
                UserSource.Userses.Add(user);
                UserSource.SaveChanges();
                UserRoles userRoles = new UserRoles()
                {
                    ProjectId = 0,//表示所有项目
                    RoleId = Convert.ToInt32(ConfigurationManager.AppSettings["DefaultRoleLevel"]),//会员级别
                    UserId = user.UserID,
                    NickName = String.Empty,// reqModel.UserName,
                };
                UserSource.UserRoleses.Add(userRoles);
                UserSource.SaveChanges();
                SaveUserId(user.UserID);
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        private bool CheckRegister(UserRegisterViewModel model, out string message)
        {
            message = string.Empty;
            var email = UserSource.Userses.FirstOrDefault(t => t.Email == model.Email && !t.IsDeleted);
            if (email != null)
            {
                message = "这个邮箱已经注册 请登陆";
                return false;
            }
            var user = UserSource.Userses.FirstOrDefault(t => t.UserName == model.UserName && !t.IsDeleted);
            if (user != null)
            {
                message = "用户名重复 请重新输入";
                return false;
            }
            var mobile = UserSource.Userses.FirstOrDefault(t => t.Telephone == model.Telephone && !t.IsDeleted);
            if (mobile != null)
            {
                message = "这个手机号己经注册 请登陆";
                return false;
            }
            if (Request.Cookies["RegisterCode"]==null||  (model.Telephone+ model.VerifyPhoneCode) != Request.Cookies["RegisterCode"].Value)
            {
                message = "验证码输入错误，请重新输入！";
                return false;
            }
            if (!model.RedChecked)
            {
                message = "请先勾选并阅读风险披露！";
                return false;
            }
            return true;
        }

        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            return View("ConfirmEmail");
        }

        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            var model = new ForgotPasswordViewModel() { Message = String.Empty };
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            try
            {
                model.Message = String.Empty;
                if (!ModelState.IsValid)
                {
                    return View(model);
                }
                var user = UserSource.Userses.FirstOrDefault(t => t.Email == model.Email && t.UserName == model.UserName);
                if (user == null)
                {
                    model.Message = "错误的用户名或者邮箱";
                    return View(model);
                }
                AccountHandler.Current.FindPassword(model.UserName, model.Email);
                Response.Write("<script>alert('邮件已发送，请立即查收!')</script>");
                return View("Login");
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteError(ex, GetType(), MethodBase.GetCurrentMethod().Name);
                throw;
            }
        }

        #region 个人中心
        public ActionResult Index()
        {
            try
            {
                if (Users == null) return RedirectToAction("Login");
                ViewBag.UserName = Users.UserName;
                return View(Users);
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteError(ex, GetType(), MethodBase.GetCurrentMethod().Name);
                throw;
            }
        }

        public ActionResult UserTags()
        {
            try
            {
                ViewBag.UserName = Users.UserName;
                var viewmodel = new UserTagsViewModel();
                //todo:登录完成之后获取当前用的用户标签
                viewmodel.List = UserSource.UcDictionaryses.Where(t => t.FiledBaseName == "UserTagKey").ToList();
                viewmodel.Tags = Users.UserTags;
                return View(viewmodel);
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteError(ex, GetType(), MethodBase.GetCurrentMethod().Name);
                throw;
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult UserTags(string tags)
        {
            try
            {
                var user = AccountHandler.Current.UpdateTags(tags, UserId);
                var viewmodel = new UserTagsViewModel();
                viewmodel.List = UserSource.UcDictionaryses.Where(t => t.FiledBaseName == "UserTagKey").ToList();
                viewmodel.Tags = user.UserTags;
                return View(viewmodel);
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteError(ex, GetType(), MethodBase.GetCurrentMethod().Name);
                throw;
            }
        }

        public ActionResult UserBase()
        {
            try
            {
                ViewBag.UserName = Users.UserName;
                return View(Users);
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteError(ex, GetType(), MethodBase.GetCurrentMethod().Name);
                throw;
            }
        }

        [HttpPost]
        public ActionResult UserBase(UserBaseViewModel model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.UserName))
                {
                    Response.Write("<script>alert('昵称不能为空，请重新输入')</script>");
                    return View(Users);
                }
                if (UserSource.Userses.Any(t => t.UserName == model.UserName && t.UserID != Users.UserID))
                {
                    Response.Write("<script>alert('此昵称已被占用，请重新输入')</script>");
                    return View(Users);
                }
                return View(Update(model));
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteError(ex, GetType(), MethodBase.GetCurrentMethod().Name);
                throw;
            }
        }

        private readonly int pageSize = 3;

        public ActionResult UserAddress()
        {
            ViewBag.UserName = Users.UserName;
            return View();
        }

        [HttpPost]
        public JsonResult UserAddress(int pageindex)
        {
            try
            {
                var q = UserSource.UserAddresses.Where(t => t.UserID == UserId);
                var count = q.Count();
                var list = q.OrderBy(t => t.AddressID).Skip(pageSize * (pageindex - 1)).Take(pageSize).ToList();
                int pageSum = 0;
                if (count % pageSize == 0)
                {
                    pageSum = count / pageSize;
                }
                else
                {
                    pageSum = (count / pageSize) + 1;
                }
                return Json(new { status = true, pageSum, data = list }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteError(ex, GetType(), MethodBase.GetCurrentMethod().Name);
                throw;
            }
        }

        [HttpPost]
        public JsonResult AddressOperation(UserAddressViewModel model)
        {
            try
            {
                if (AddressInsert(model))
                {
                    return Json(new { msg = "操作成功", status = true }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { msg = "操作失败", status = false }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteError(ex, GetType(), MethodBase.GetCurrentMethod().Name);
                throw;
            }
        }

        [HttpPost]
        public JsonResult GetAddress(int addressid)
        {
            try
            {
                var model = UserSource.UserAddresses.FirstOrDefault(t => t.AddressID == addressid);
                return Json(new { data = model }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteError(ex, GetType(), MethodBase.GetCurrentMethod().Name);
                throw;
            }
        }

        [HttpPost]
        public JsonResult AddressDel(int addressid)
        {
            try
            {
                var result = AccountHandler.Current.AddressDel(addressid) > 0;
                return Json(new { status = result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteError(ex, GetType(), MethodBase.GetCurrentMethod().Name);
                throw;
            }
        }

        private bool AddressInsert(UserAddressViewModel model)
        {
            var address = new UserAddress()
            {
                UserID = UserId,
                Email = model.Email,
                Telephone = model.Telephone,
                Country = "中国",
                Area = model.Area,
                City = model.City,
                Province = model.Province,
                DetailInfo = model.DetailInfo
            };
            if (model.AddressID != 0) //修改
            {
                address.AddressID = model.AddressID;
                return AccountHandler.Current.AddressUpdate(address) > 0;
            }
            else                      //新增
            {
                return AccountHandler.Current.AddressInsert(address) > 0;
            }
        }

        private Users Update(UserBaseViewModel model)
        {
            var savaurl = SaveFile();
            var users = new Users
            {
                UserID = UserId,
                Avatar = model.Avatar ?? "",
                RealName = model.RealName ?? "",
                QQ = model.QQ ?? "",
                Weixin = model.Weixin ?? "",
                Sex = model.Sex,
                InvestmentType = model.InvestmentType,
                UserName = model.UserName
            };
            if (savaurl != "") users.Avatar = savaurl;
            return AccountHandler.Current.UpdateUser(users);
        }

        [HttpPost]
        [AllowAnonymous]
        public JsonResult UploadFile()
        {
            try
            {
                var savaurl = SaveFile();
                return Json(savaurl, "text/html");
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteError(ex, GetType(), MethodBase.GetCurrentMethod().Name);
                throw;
            }
        }

        private string SaveFile()
        {
            var savaurl = string.Empty;
            if (Request.Files.Count > 0)
            {

                var file = Request.Files[0];
                if (file == null || !UntilHelper.IsUploadImage(Path.GetExtension(file.FileName))) return string.Empty;
                var newName = DateTime.Now.ToString("yyyyMMddHHmmss") + Path.GetExtension(file.FileName);
                if (!Directory.Exists(Server.MapPath("~/Image/uploadfiles/users/")))
                {
                    Directory.CreateDirectory(Server.MapPath("~/Image/uploadfiles/users/"));
                }
                savaurl = "../Image/uploadfiles/users/" + newName;
                var filepath = Path.Combine(Server.MapPath("~/Image/uploadfiles/users/"), newName);
                //保存文件
                file.SaveAs(filepath);
                try
                {

                    var url = GetUrlPath(filepath);
                    LogHelper.Instance.WriteInformation("SaveFile url is " + url + " path is " + filepath);
                    NotifyDownloadFile(url, "/Image/uploadfiles/users/" + newName);

                }
                catch (Exception ex)
                {
                    LogHelper.Instance.WriteError(ex, GetType(), MethodBase.GetCurrentMethod().Name);
                }

            }
            return savaurl;
        }

        private void NotifyDownloadFile(string url, string path)
        {
            if (!string.IsNullOrEmpty(NotifyWebUrl))
            {
                NotifyService(NotifyWebUrl + "Home/DownloadFile?url=" + url + "&path=" + path);
            }
        }

        public ActionResult VerifyTel()
        {
            try
            {
                ViewBag.UserName = Users.UserName;
                ViewBag.isverify = Users.IsTelphoneEffective;
                ViewBag.Tel = Users.Telephone.Substring(0, 3) + "****" + Users.Telephone.Substring(7);
                ViewBag.Token = UntilHelper.Token;
                return View();
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteError(ex, GetType(), MethodBase.GetCurrentMethod().Name);
                throw;
            }
        }

        [HttpPost]
        public JsonResult Verify()
        {
            return Json(new { status = AccountHandler.Current.VerifyTel(UserId) });
        }

        public ActionResult VerifyEmail()
        {
            try
            {
                ViewBag.UserName = Users.UserName;
                ViewBag.isverify = Users.IsEmailEffective;
                ViewBag.Email = Users.Email;
                return View();
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteError(ex, GetType(), MethodBase.GetCurrentMethod().Name);
                throw;
            }
        }

        [HttpPost]
        public JsonResult EmailVerify()
        {
            try
            {
                return Json(new { status = AccountHandler.Current.VerifyEmail(UserId) });
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteError(ex, GetType(), MethodBase.GetCurrentMethod().Name);
                throw;
            }
        }

        public JsonResult SendEmialCode()
        {
            try
            {
                string code = UntilHelper.CreateRandomCode(6);
                var result = MailHelp.SendEmailCode(Users.Email, code);
                if (result)
                {
                    HttpCookie cookie = new HttpCookie("emailcode", code);
                    DateTime dt = DateTime.Now;
                    TimeSpan ts = new TimeSpan(0, 1, 0, 0, 0);//过期时间为1天
                    cookie.Expires = dt.Add(ts);//设置过期时间             
                    Response.AppendCookie(cookie);
                }
                return Json(new { status = result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteError(ex, GetType(), MethodBase.GetCurrentMethod().Name);
                throw;
            }
        }
        [AllowAnonymous]
        public ActionResult SendSmsByPhone(string phone, string token)
        {
            try
            {
                if (string.IsNullOrEmpty(token) || !UntilHelper.Token.Equals(token, StringComparison.OrdinalIgnoreCase))
                    return Json(false, JsonRequestBehavior.AllowGet);
                if (string.IsNullOrEmpty(phone)) return Json(false, JsonRequestBehavior.AllowGet);
                string code = UntilHelper.CreateRandomCode(6);
                var msgcontent = $"您的验证码是：{code}。请不要把验证码泄露给其他人。如非本人操作，可不用理会！";
                var resut = UntilHelper.SendSms(phone, msgcontent);
                if (resut)
                {
                    HttpCookie cookie = new HttpCookie("RegisterCode", phone + code);
                    DateTime dt = DateTime.Now;
                    TimeSpan ts = new TimeSpan(0, 1, 0, 0, 0); //过期时间为1天
                    cookie.Expires = dt.Add(ts); //设置过期时间             
                    Response.AppendCookie(cookie);
                }
                return Json(resut, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteError(ex, GetType(), MethodBase.GetCurrentMethod().Name);
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        [AllowAnonymous]
        public ActionResult SendSMS(string phone, string token)
        {
            try
            {
                if (string.IsNullOrEmpty(token) || !UntilHelper.Token.Equals(token, StringComparison.OrdinalIgnoreCase))
                    return Json(false, JsonRequestBehavior.AllowGet);
                if (string.IsNullOrEmpty(Users.Telephone)) return Json(false, JsonRequestBehavior.AllowGet);
                string code = UntilHelper.CreateRandomCode(6);
                var msgcontent = $"您的验证码是：{code}。请不要把验证码泄露给其他人。如非本人操作，可不用理会！";
                var resut = UntilHelper.SendSms(Users.Telephone, msgcontent);
                if (resut)
                {
                    HttpCookie cookie = new HttpCookie("code", code);
                    DateTime dt = DateTime.Now;
                    TimeSpan ts = new TimeSpan(0, 1, 0, 0, 0); //过期时间为1天
                    cookie.Expires = dt.Add(ts); //设置过期时间             
                    Response.AppendCookie(cookie);   
                }
                return Json(resut, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteError(ex, GetType(), MethodBase.GetCurrentMethod().Name);
                throw;
            }
        }


        public ActionResult ChangePassword()
        {
            try
            {
                ViewBag.Tel = Users.Telephone.Substring(0, 3) + "****" + Users.Telephone.Substring(7);
                ViewBag.UserName = Users.UserName;
                ViewBag.Token = UntilHelper.Token;
                return View();
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteError(ex, GetType(), MethodBase.GetCurrentMethod().Name);
                throw;
            }
        }


        [HttpPost]
        public JsonResult ChangePassword(string password)
        {
            try
            {
                return Json(new { status = AccountHandler.Current.ChangePassword(UserId, password) }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteError(ex, GetType(), MethodBase.GetCurrentMethod().Name);
                throw;
            }
        }

        #endregion

        #region 帮助程序

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }


        #endregion
    }
}