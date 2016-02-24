using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Online.DbHelper.BLL;
using Online.DbHelper.Model;
using Online.Web.DAL;
using Online.Web.Help;
using Online.Web.Models;
using System.Data.Entity;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Online.DbHelper.Common;

namespace Online.Web.Controllers
{
    public class HomeController : BaseController
    {
        public async Task<ActionResult> Index()
        {
            ViewBag.Title = LiveRooms == null ? "" : LiveRooms.RoomName;
            var userName = GetUserNameFromCookie();
            bool isUserInBlackList = false;
            var isInBlackList = await UserSource.UserBlackLists.AnyAsync(t => t.ClientIp == ClientIp && t.RoomId == RoomId && t.Type == 1);
            if (!string.IsNullOrEmpty(userName))
            {
                isUserInBlackList = await UserSource.UserBlackLists.AnyAsync(t => t.Type == 2 && t.UserName == userName);
            }
            if (isInBlackList || isUserInBlackList)
                return View("Invalid");
            return View();
        }

        public ActionResult UserVote()
        {
            return View();
        }

        public ActionResult Invalid()
        {
            return View();
        }

        public ActionResult Announcement()
        {
            return View();
        }

        public ActionResult ImageManage()
        {
            return View();
        }

        public ActionResult Playfigure()
        {
            return View();
        }

        public ActionResult GetValidateCode(string time)
        {
            try
            {
                string code = UntilHelper.CreateRandomCode(4);
                Session["VerifyCode"] = code;
                byte[] bytes = UntilHelper.CreateImage(code);
                return File(bytes, @"image/jpeg");
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteError(ex, GetType(), MethodBase.GetCurrentMethod().Name);
                throw ex;
            }
        }

        public ActionResult AddUserVoteItem(int voteid, int votecolumid, int votecount)
        {
            try
            {
                if (UserId == 0 || User == null) return Json("Y", JsonRequestBehavior.AllowGet);
                var vote = DataSource.UserVotes.Where(x => x.VoteID == voteid).ToList();
                var colum = DataSource.UserVoteColums.Where(x => x.VoteID == voteid);
                if (!vote[0].IsVoteMulti)
                {
                    foreach (UserVoteColum item in colum)
                    {
                        var items = DataSource.VoteItemses.Count(x => x.UserVoteColumID == item.ID && x.VoteUserID == UserId);
                        if (items > 0)
                        {
                            return Json("Q", JsonRequestBehavior.AllowGet);
                        }
                    }
                }
                var count = DataSource.VoteItemses.Count(t => t.VoteUserID == Users.UserID && t.UserVoteColumID == votecolumid);
                if (count >= votecount)
                    return Json("C", JsonRequestBehavior.AllowGet);
                var exist = DataSource.UserVoteColums.FirstOrDefault(t => t.ID == votecolumid);
                if (exist == null) throw new Exception("投票项不存在！");
                exist.VoteCount = exist.VoteCount + 1;
                var entity = new VoteItems
                {
                    UserVoteColumID = votecolumid,
                    VoteUserID = Convert.ToInt32(UserId),
                    VoteTime = DateTime.Now,
                };
                DataSource.VoteItemses.Add(entity);
                DataSource.SaveChanges();
                return Json("T", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteError(ex, GetType(), MethodBase.GetCurrentMethod().Name);
                throw ex;
            }
        }

        public ActionResult SaveDesktop(string url, string title)
        {
            try
            {
                string fileName = Request.Browser.Browser == "InternetExplorer" || Request.Browser.Browser == "IE" ? HttpUtility.UrlEncode(title + ".url") : HttpUtility.HtmlEncode(title + ".url");
                string testStr = "[{000214A0-0000-0000-C000-000000000046}] \n\r Prop3=19,2 \n\r [InternetShortcut] \n\r URL=" + url + " \n\r";
                Response.Charset = "utf-8";
                var byteArry = System.Text.Encoding.Default.GetBytes(testStr);
                Response.ContentType = "application/octet-stream";
                Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
                Response.BinaryWrite(byteArry);
                Response.Flush();
                Response.End();
                return new EmptyResult();
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteError(ex, GetType(), MethodBase.GetCurrentMethod().Name);
                throw ex;
            }
        }

        public ActionResult GetLiveTvList()
        {
            try
            {
                IEnumerable<SysTVColumns> q;
                if (LiveRooms == null || LiveRooms.SysTvColumnses == null)
                {
                    q = DataSource.SysTvColumnses.Where(t => t.RoomID == RoomId && t.ItemType == 1);
                }
                else
                {
                    q = LiveRooms.SysTvColumnses.Where(t => t.RoomID == RoomId && t.ItemType == 1);
                }
                var entitylist = q.Select(t => new
                {
                    ItemImgUrl = "../../Image/" + t.ItemImgUrl,
                    t.ItemLink,
                    t.ItemName,
                    t.ItemTitle
                }).ToList();
                return Json(entitylist, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteError(ex, GetType(), MethodBase.GetCurrentMethod().Name);
                throw ex;
            }
        }

        public ActionResult QueryBasicKnowledges()
        {
            try
            {
                IEnumerable<SysTVColumns> q;
                if (LiveRooms == null || LiveRooms.SysTvColumnses == null)
                {
                    q = DataSource.SysTvColumnses.Where(t => t.RoomID == RoomId && t.ItemType == 8);
                }
                else
                {
                    q = LiveRooms.SysTvColumnses.Where(t => t.RoomID == RoomId && t.ItemType == 8);
                }
                var entitylist = q.Select(t => new
                {
                    t.ItemLink,
                    t.ItemTitle,
                    t.SysTVColumnID
                }).ToList();
                return Json(entitylist, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteError(ex, GetType(), MethodBase.GetCurrentMethod().Name);
                throw ex;
            }
        }

        public JsonResult QueryDisclaimer()
        {
            try
            {
                SysTVColumns entity;
                if (LiveRooms?.SysTvColumnses == null)
                {
                    entity = DataSource.SysTvColumnses.FirstOrDefault(t => t.RoomID == RoomId && t.ItemType == 6);
                }
                else
                {
                    entity = LiveRooms.SysTvColumnses.FirstOrDefault(t => t.ItemType == 6);
                }
                return Json(entity != null ? entity.ISummary : "", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteError(ex, GetType(), MethodBase.GetCurrentMethod().Name);
                throw ex;
            }

        }

        public ActionResult QueryIntelligentTradings()
        {
            try
            {
                IEnumerable<SysTVColumns> q;
                if (LiveRooms?.SysTvColumnses == null)
                {
                    q = DataSource.SysTvColumnses.Where(t => t.RoomID == RoomId && t.ItemType == 9);
                }
                else
                {
                    q = LiveRooms.SysTvColumnses.Where(t => t.ItemType == 9);
                }
                var entitylist = q.Select(t => new
                {
                    t.ItemLink,
                    t.ItemTitle,
                    t.SysTVColumnID
                }).ToList();
                return Json(entitylist, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteError(ex, GetType(), MethodBase.GetCurrentMethod().Name);
                throw ex;
            }
        }

        public ActionResult QueryVips()
        {
            try
            {
                IEnumerable<SysTVColumns> q;
                if (LiveRooms?.SysTvColumnses == null)
                {
                    q = DataSource.SysTvColumnses.Where(t => t.RoomID == RoomId && t.ItemType == 4);
                }
                else
                {
                    q = LiveRooms.SysTvColumnses.Where(t => t.ItemType == 4);
                }
                var entitylist = q
                    .Select(t => new
                    {
                        ItemImgUrl = "../../Image/" + t.ItemImgUrl,
                        t.ItemLink,
                        t.ItemName,
                        t.ISummary
                    }).ToList();
                return Json(entitylist, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteError(ex, GetType(), MethodBase.GetCurrentMethod().Name);
                throw ex;
            }
        }

        public ActionResult QueryAdvancedTechnology()
        {
            try
            {
                IEnumerable<SysTVColumns> q;
                if (LiveRooms == null || LiveRooms.SysTvColumnses == null)
                {
                    q = DataSource.SysTvColumnses.Where(t => t.RoomID == RoomId && t.ItemType == 7);
                }
                else
                {
                    q = LiveRooms.SysTvColumnses.Where(t => t.ItemType == 7);
                }
                var entitylist = q
                    .Select(t => new
                    {
                        t.ItemLink,
                        t.ItemTitle,
                        t.SysTVColumnID
                    });
                return Json(entitylist, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteError(ex, GetType(), MethodBase.GetCurrentMethod().Name);
                throw ex;
            }
        }

        public ActionResult QueryMsgs()
        {
            try
            {
                IEnumerable<MessageInfo> entityList;
                if (Users != null && Roleses != null && Roleses.Any(t => t.PowerId >= (int)UserRoleEnum.XUGUAN))
                    entityList = MessageCache.Instance.GetTop(1000);
                else
                {
                    entityList = MessageCache.Instance.GetCheckedTop(20);
                }
                return Json(entityList.OrderBy(t => t.createTime), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteError(ex, GetType(), MethodBase.GetCurrentMethod().Name);
                throw ex;
            }
        }
        [AllowAnonymous]
        public ActionResult Logout()
        {
            try
            {
                SaveUserLogout();
                SaveUserId(0);
                ClearCookie();
                RedisClienHelper.Hash_Remove<UserOnlineInfo>("ONLINE_Admin_USERS_" + RoomId, UserId.ToString());
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteError(ex, GetType(), MethodBase.GetCurrentMethod().Name);
                throw ex;
            }
        }

        private void SaveUserLogout()
        {
            try
            {
                Users.LastSignOutTime = DateTime.Now;
                Users.LastChangeTime = DateTime.Now;
                UserSource.SaveChanges();
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteError(ex, GetType(), MethodBase.GetCurrentMethod().Name);
            }

        }

        public Users CheckLogin(string userName, string password)
        {
            var user = UserSource.Userses.FirstOrDefault(t => (t.UserName == userName || t.Email == userName || t.Telephone == userName) && !t.IsDeleted);
            if (user == null) return null;
            var md5Password = UntilHelper.GetMd5HashCode(password);
            if (md5Password == user.Password)
            {
                SaveUserId(user.UserID);
                SaveUserLogin(user);
                return user;
            }
            return null;
        }

        private void SaveUserLogin(Users user)
        {
            try
            {
                user.LastSigninIP = ClientIp;
                user.LastSignInTime = DateTime.Now;
                user.LastChangeTime = DateTime.Now;
                UserSource.SaveChanges();
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteError(ex, GetType(), MethodBase.GetCurrentMethod().Name);
            }

        }

        private void AddUserRole(Users user)
        {
            var userRole = new UserRoles
            {
                UserId = user.UserID,
                ProjectId = ProjectId,
                RoleId = 1,
            };
            UserSource.UserRoleses.Add(userRole);
            UserSource.SaveChanges();
        }

        private Users SaveUser(string email, string nickName, string phone, string password, string qq,string fromUrl)
        {
            var user = new Users();
            user.Salt = string.Empty;//Guid.NewGuid().ToString().Replace("-", "").ToLower();
            user.Email = email;
            user.UserName = nickName;
            user.Password = UntilHelper.GetMd5HashCode(password);
            user.Telephone = phone;
            user.RegisterIP = ClientIp;
            user.RegSource = Enum.GetName(typeof(UserOwnerEnum), ProjectOwner);
            user.Owner = ProjectOwner;
            user.RecommendCode = "";
            user.QQ = string.IsNullOrEmpty(qq) ? "" : qq;
            user.Channel = 1;
            user.Device = 1;
            user.LastChangeTime = DateTime.Now;
            user.RegisterTime = DateTime.Now;
            user.RegSource = fromUrl??"";
            UserSource.Userses.Add(user);
            UserSource.SaveChanges();
            return user;
        }

        private bool CheckUserName(string nickName, string email, string phone)
        {
            var isUserName = UserSource.Userses.Any(t => t.UserName == nickName && !t.IsDeleted);
            if (isUserName)
                throw new Exception("用户名重复");
            var isEmail = UserSource.Userses.Any(t => t.Email == email && !t.IsDeleted);
            if (isEmail)
                throw new Exception("邮箱名重复");
            var isPhone = UserSource.Userses.Any(t => t.Telephone == phone && !t.IsDeleted);
            if (isPhone)
                throw new Exception("手机号重复");
            return true;
        }

        public ActionResult Register(string email, string nickName, string phone, string password, string verifyCode, string qq,string fromUrl)
        {
            try
            {
                CheckUserName(nickName, email, phone);
                var user = SaveUser(email, nickName, phone, password, qq,fromUrl);
                AddUserRole(user);
                var result = new
                {
                    RoleList = new[] { new
                    {
                        RoleID = 1,
                        RoleName = "会员"

                    }},
                    user.UserID,
                    user.UserName,
                };
                SaveUserId(user.UserID);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteError(ex, GetType(), MethodBase.GetCurrentMethod().Name);
                throw ex;
            }
        }

        private bool IsInBlackList(string userName)
        {
            return UserSource.UserBlackLists.Any(t => t.Type == 2 && t.UserName == userName);
        }

        private void AddXuGuanToRedis(Users user)
        {
            UserOnlineInfo cacheuser = new UserOnlineInfo();
            cacheuser.uid = user.UserID;
            cacheuser.from = user.UserName;
            var max = user.UserRoleses.Where(t => t.Roles.PowerId < 1000).Max(t => t.Roles.PowerId);
            if (max != null)
            {
                cacheuser.rid = max.Value;
            }
            cacheuser.roomid = RoomId;
            if (cacheuser.rid >= (int)UserRoleEnum.XUGUAN)
            {
                RedisClienHelper.Hash_Remove<UserOnlineInfo>("ONLINE_Admin_USERS_" + RoomId, user.UserID.ToString());
                RedisClienHelper.Hash_Set<UserOnlineInfo>("ONLINE_Admin_USERS_" + RoomId, user.UserID.ToString(), cacheuser);
            }
        }

        [AllowAnonymous]
        public ActionResult UserLogin(string userName, string password)
        {
            try
            {
                var user = CheckLogin(userName, password);
                if (user != null)
                {
                    if (IsInBlackList(user.UserName)) return Json(2, JsonRequestBehavior.AllowGet);
                    AddXuGuanToRedis(user);
                    var result = new
                    {
                        RoleList = user.UserRoleses.Where(t => t.Roles.PowerId < 1000).Select(t => new
                        {
                            RoleID = t.Roles.PowerId,
                            t.Roles.RoleName,
                            t.NickName
                        }),
                        user.UserID,
                        user.UserName,
                    };
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(1, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteError(ex, GetType(), MethodBase.GetCurrentMethod().Name);
                return Json(1, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult LoadRoomInfo()
        {
            try
            {
                AddUserLoginLog();
                LiveRooms entity;
                if (LiveRooms == null)
                    entity = DataSource.LiveRoomses.First(t => t.RoomID == RoomId);
                else
                {
                    entity = LiveRooms;
                }
                var roomInfo = new
                {
                    RoomId,
                    entity.RoomName,
                    entity.IsDeleted,
                    entity.BizStatus,
                    entity.IsPrivateChat,
                    entity.IsShowAdminMsg,
                };
                var conf = new
                {
                    SysConfigs.IsAllowPost,
                    SysConfigs.IsAllowTouristPost,
                    SysConfigs.IsUploadFile,
                    SysConfigs.IsOpenReg,
                    SysConfigs.IsCheckMsg,
                    SysConfigs.IsFilterMsg,
                    FilterWords = WordFilters,
                    SysConfigs.ServiceQQs,
                    Token = UntilHelper.Token,
                };
                if (UserId > 0 && Users != null)
                {
                    var user = new
                    {
                        RoleList = Users.UserRoleses.Where(t => t.Roles.PowerId < 1000).Select(t => new
                        {
                            RoleID = t.Roles.PowerId,
                            t.Roles.RoleName,
                            t.NickName
                        }),
                        Users.UserID,
                        Users.UserName,
                    };
                    return Json(new { Entity = roomInfo, Conf = conf, User = user }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { Entity = roomInfo, Conf = conf, }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteError(ex, GetType(), MethodBase.GetCurrentMethod().Name);
                throw ex;
            }
        }

        private void AddUserLoginLog()
        {
            try
            {
                var entity = new UserActionLog();
                entity.UserId = UserId;
                entity.Title = "进入直播室";
                entity.Description = LiveRooms == null ? "" : LiveRooms.RoomName;
                entity.Type = 8;
                entity.ProjectId = ProjectId;
                entity.UserIp = ClientIp;
                entity.CreateTime = DateTime.Now;
                entity.RoomId = RoomId;
                DataQueueDal.Instance.Add(entity);
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteError(ex, GetType(), MethodBase.GetCurrentMethod().Name);

            }

        }

        public ActionResult CachingMsgList(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) throw new Exception("消息不能为空");
            if (input.Contains("??????")) throw new Exception("乱码"); ;

            try
            {
                MessageInfo entity = JsonConvert.DeserializeObject<MessageInfo>(input);
                var item = SaveMessage(entity);
                entity.ChatID = item.ChatID;
                NotifyCachingMsgList(entity);
                MessageCache.Instance.AddMessage(entity);

                return Json(entity.ChatID, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteError(ex, GetType(), MethodBase.GetCurrentMethod().Name);
                throw ex;
            }
        }

        public ActionResult AddMessage(string input)
        {
            try
            {
                var entity = JsonConvert.DeserializeObject<MessageInfo>(input);
                MessageCache.Instance.AddMessage(entity);
                return Json(entity.ChatID, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteError(ex, GetType(), MethodBase.GetCurrentMethod().Name);
                throw ex;
            }
        }

        private void NotifyCachingMsgList(MessageInfo entity)
        {
            if (!string.IsNullOrEmpty(NotifyWebUrl))
            {
                var input = JsonConvert.SerializeObject(entity);
                NotifyService(NotifyWebUrl + "Home/AddMessage?input=" + input);
            }
        }

        private SysChatMsgs SaveMessage(MessageInfo item)
        {
            var entity = new SysChatMsgs();
            if (item != null)
            {
                entity.FromUserID = item.uid.ToString();
                entity.FromUserName = item.from;
                entity.ToUserID = item.touid.ToString();
                entity.ToUserName = item.to;
                entity.RoomID = RoomId;
                entity.MsgContent = item.msg;
                entity.MsgType = item.msgtype;
                entity.IsCheck = item.ischeck == 1;
                entity.FilePath = string.IsNullOrWhiteSpace(item.postfile) ? string.Empty : item.postfile;
                entity.SendTime = DateTime.Now;
            }
            DataSource.SysChatMsgses.Add(entity);
            DataSource.SaveChanges();
            return entity;
        }

        /// <summary>
        /// 根据socketid删除下线用户
        /// </summary>
        /// <returns></returns>
        public ActionResult CorrectionOnlineUserCount(string socketids)
        {
            try
            {
                if (string.IsNullOrEmpty(socketids)) return Json(true, JsonRequestBehavior.AllowGet);
                //var user = UserSource.Userses.FirstOrDefault(t => t.UserName == socketids);
                //if (user != null && user.UserRoleses.Any(t => t.RoleId >= 9))
                //    RedisClienHelper.Hash_Remove<UserOnlineInfo>("ONLINE_Admin_USERS_" + RoomId, user.UserID.ToString());
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteError(ex, GetType(), MethodBase.GetCurrentMethod().Name);
                //throw ex;
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        private void NotifyCheckMsgitem(long chartId)
        {
            if (!string.IsNullOrEmpty(NotifyWebUrl))
            {

                NotifyService(NotifyWebUrl + "Home/CheckMsgitem?chartId=" + chartId);
            }
        }

        public ActionResult CheckMsgitem(long chartId)
        {
            try
            {
                if (chartId > 0)
                {
                    var first = MessageCache.Instance.FirstOrDefault(chartId);
                    if (first == null)
                    {
                        throw new Exception("无效数据或已损坏");
                    }
                    if (first.ischeck == 0)
                    {
                        first.ischeck = 1;
                        var result = MessageCache.Instance.SetChecked(first.ChatID);
                        if (result)
                        {
                            var entity = DataSource.SysChatMsgses.FirstOrDefault(t => t.ChatID == first.ChatID);
                            if (entity != null)
                            {
                                entity.IsCheck = true;
                                DataSource.SaveChanges();
                            }
                            NotifyCheckMsgitem(chartId);
                            return Json(first, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        throw new Exception("此消息已审核");
                    }
                }
                throw new Exception("无效数据或已损坏");
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteError(ex, GetType(), MethodBase.GetCurrentMethod().Name);
                throw ex;
            }
        }

        public ActionResult LoadSystemInfos()
        {
            try
            {
                var results = SystemInfos.Select(t => new
                {
                    t.InfoType,
                    SendTime = t.SendTime.ToString("yyyy-MM-dd hh:mm:ss"),
                    t.InfoContent,
                });
                return Json(results, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteError(ex, GetType(), MethodBase.GetCurrentMethod().Name);
                throw ex;
            }
        }

        public ActionResult ChangeUserRole(string userName, int roleId)
        {
            try
            {
                var adminulist = RedisClienHelper.Hash_GetAll<UserOnlineInfo>("ONLINE_Admin_USERS_" + RoomId);
                if (adminulist != null && adminulist.Count > 0)
                {
                    var adcurlist = adminulist.ToList().Where(p => p.uid == UserId).ToList();
                    foreach (UserOnlineInfo item in adcurlist)
                    {
                        RedisClienHelper.Hash_Remove<UserOnlineInfo>("ONLINE_Admin_USERS_" + RoomId, item.from);
                    }
                }
                if (roleId >= (Int32)UserRoleEnum.XUGUAN)
                {
                    UserOnlineInfo adminuser = new UserOnlineInfo();
                    adminuser.uid = UserId;
                    adminuser.from = userName;
                    adminuser.rid = roleId;
                    adminuser.roomid = RoomId;
                    RedisClienHelper.Hash_Set<UserOnlineInfo>("ONLINE_Admin_USERS_" + RoomId, userName, adminuser);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteError(ex, GetType(), MethodBase.GetCurrentMethod().Name);
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult HasValExitsts(string value, string type)
        {
            try
            {
                Users exist = null;
                if (type.Equals("UserName", StringComparison.OrdinalIgnoreCase))
                    exist = UserSource.Userses.FirstOrDefault(t => t.UserName == value);
                if (type.Equals("Email", StringComparison.OrdinalIgnoreCase))
                    exist = UserSource.Userses.FirstOrDefault(t => t.Email == value);
                if (type.Equals("Telephone", StringComparison.OrdinalIgnoreCase))
                    exist = UserSource.Userses.FirstOrDefault(t => t.Telephone == value);
                if (exist != null)
                    return Json(new { IsSuccess = false, Message = "此[" + value + "]已经注册" }, JsonRequestBehavior.AllowGet);
                return Json(new { IsSuccess = true, Message = "" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteError(ex, GetType(), MethodBase.GetCurrentMethod().Name);
                throw ex;
            }
        }

        public ActionResult ValidateCode(string code)
        {
            if (Session["VerifyCode"] != null && code != null && code.Equals(Session["VerifyCode"].ToString(), StringComparison.OrdinalIgnoreCase))
            {
                return Json(new { IsSuccess = true, Message = "" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { IsSuccess = false, Message = "验证码不正确" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult JoinBlackList(string userId, string userName)
        {
            try
            {
                var id = Convert.ToInt64(userId);
                var user = UserSource.Userses.FirstOrDefault(t => t.UserID == id);
                if ((user == null || user.UserRoleses.Any(t => t.Roles.PowerId >= 100)) && string.IsNullOrEmpty(userName))
                {
                    throw new Exception("添加黑名单失败");
                }
                var entity = new UserBlackList
                {
                    UserId = Convert.ToInt64(id),
                    ClientIp = user != null ? user.LastSigninIP : "",
                    RoomId = RoomId,
                    OperateName = Users.UserName,
                    CreateTime = DateTime.Now,
                    Type = string.IsNullOrEmpty(userName) ? 1 : 2,
                    UserName = userName
                };
                UserSource.UserBlackLists.Add(entity);
                UserSource.SaveChanges();
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteError(ex, GetType(), MethodBase.GetCurrentMethod().Name);
                throw new Exception("添加黑名单失败");
            }
        }

        public ActionResult DelBlackList(string userId)
        {
            try
            {
                var id = Convert.ToInt64(userId);
                var user = UserSource.UserBlackLists.FirstOrDefault(t => t.UserId == id);
                if (user == null)
                {
                    throw new Exception("删除黑名单失败");
                }

                UserSource.UserBlackLists.Remove(user);
                UserSource.SaveChanges();
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteError(ex, GetType(), MethodBase.GetCurrentMethod().Name);
                throw new Exception("添加黑名单失败");
            }
        }

        public ActionResult GetUserSexTheme()
        {
            try
            {
                var obj = new
                {
                    userAdmin = Users != null && Users.UserRoleses != null && Users.UserRoleses.Any(t => t.RoleId <= 11 && t.RoleId >= 9),
                    userstarlo = Users != null,
                    theme = Users != null ? Users.SexTheme : "theme_default.css"
                };
                return Json(obj, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteError(ex, GetType(), MethodBase.GetCurrentMethod().Name);
                throw ex;
            }
        }

        private void AddUpdateSettingLog(string title, string description, int type)
        {
            var entity = new UserActionLog();
            entity.UserId = UserId;
            entity.Title = title;
            entity.Description = description;
            entity.Type = type;
            entity.ProjectId = ProjectId;
            entity.UserIp = ClientIp;
            entity.CreateTime = DateTime.Now;
            entity.RoomId = RoomId;
            DataQueueDal.Instance.Add(entity);
        }

        public ActionResult UpdateUserSextheme(string style)
        {
            var user = UserSource.Userses.FirstOrDefault(t => t.UserID == UserId);
            if (user != null)
            {
                user.SexTheme = style;
                UserSource.SaveChanges();
                AddUpdateSettingLog("更改主题", style, 9);
            }
            else
            {
                throw new Exception("请先登陆");
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdateServerConfigQQ(string serviceQQs)
        {
            try
            {
                var entityList = DataSource.SysConfigses.Where(t => t.RoomId == RoomId).ToList();
                foreach (var entity in entityList)
                {
                    if (string.IsNullOrEmpty(entity.ServiceQQs))
                    {
                        entity.ServiceQQs = serviceQQs;
                    }
                    else
                    {
                        entity.ServiceQQs = entity.ServiceQQs + ";" + serviceQQs;
                    }
                }
                DataSource.SaveChanges();
                AddUpdateSettingLog("新增QQ", serviceQQs, 10);
                RefreshLiveRoom();
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteError(ex, GetType(), MethodBase.GetCurrentMethod().Name);
                throw ex;
            }
        }
        [HttpPost]
        public ActionResult ServerConfigQQ()
        {
            var entity = DataSource.SysConfigses.FirstOrDefault(t => t.RoomId == RoomId);
            return Json(entity.ServiceQQs, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult UpdateConfigQQ(string serviceQQs)
        {
            try
            {
                var entityList = DataSource.SysConfigses.FirstOrDefault(t => t.RoomId == RoomId);
                entityList.ServiceQQs = serviceQQs;
                DataSource.SaveChanges();
                AddUpdateSettingLog("更新QQ", serviceQQs, 10);
                RefreshLiveRoom();
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteError(ex, GetType(), MethodBase.GetCurrentMethod().Name);
                throw ex;
            }
        }

        public ActionResult GetIsUserVotes()
        {
            try
            {
                var date = DateTime.Now.Date;
                var result = DataSource.UserVotes.Where(t =>
                 t.RoomID == RoomId && !t.IsDeleted && t.VoteBeginTime <= date &&
                 t.VoteEndTime >= date).Any(t => t.UserVoteColums.All(c => c.VoteItemses.All(a => a.VoteUserID != UserId)));

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteError(ex, GetType(), MethodBase.GetCurrentMethod().Name);
                throw ex;
            }
        }

        public ActionResult GetUserVotes()
        {
            try
            {
                var date = DateTime.Now.Date;
                var voteList = DataSource.UserVotes.Where(t => t.RoomID == RoomId && !t.IsDeleted && t.VoteBeginTime <= date && t.VoteEndTime >= date).ToList();
                var results = voteList.Select(x => new
                {
                    x.VoteTitle,
                    x.OptCount,
                    x.VoteCount,
                    x.VoteID,
                    x.CreateUser,
                    UserVoteColums = x.UserVoteColums.Select(t => new
                    {
                        t.ID,
                        t.Columname,
                        t.VoteCount
                    })
                });
                return Json(results, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteError(ex, GetType(), MethodBase.GetCurrentMethod().Name);
                throw ex;
            }
        }

        public ActionResult GetSysteminfoFlyImage()
        {
            try
            {
                var entity = SystemInfos.Where(t => t.RoomID == RoomId && t.InfoType == 4).OrderByDescending(t => t.SendTime).FirstOrDefault();
                return Json(entity == null ? "" : entity.ImgUrl, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteError(ex, GetType(), MethodBase.GetCurrentMethod().Name);
                throw ex;
            }
        }

        public ActionResult SaveSystemInfo(string content)
        {
            try
            {
                var entity = new SystemInfo
                {
                    CreateUser = Users.UserName,
                    RoomID = RoomId,
                    InfoTitle = "",
                    InfoType = 3,
                    InfoContent = content,
                    ImgUrl = "",
                    SourceLink = "",
                    InfoWeight = 0,
                    SendTime = DateTime.Now,
                };
                DataSource.SystemInfos.Add(entity);
                DataSource.SaveChanges();
                AddUpdateSettingLog("新增飞屏", content, 14);
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteError(ex, GetType(), MethodBase.GetCurrentMethod().Name);
                throw ex;
            }
        }

        public ActionResult GetSettingVis()
        {
            try
            {
                var result = new
                {
                    userAdmin = Users != null && Users.UserRoleses != null && Users.UserRoleses.Any(t => t.RoleId <= 11 && t.RoleId >= 9),
                    theme = Users != null ? Users.SexTheme : "theme_default.css"
                };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteError(ex, GetType(), MethodBase.GetCurrentMethod().Name);
                throw ex;
            }
        }

        public ActionResult SetTheme(string style)
        {
            try
            {
                int i = 0;
                if (Users != null)
                {
                    using (var db = new UserContextBll())
                    {
                        Users users = db.Userses.FirstOrDefault(t => t.UserID == Users.UserID);
                        users.SexTheme = style;
                        i = db.SaveChanges();
                    }
                }
                var result = new { start = i };
                RefreshLiveRoom();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteError(ex, GetType(), MethodBase.GetCurrentMethod().Name);
                throw ex;
            }
        }

        public ActionResult GetAnnouncement()
        {
            try
            {
                DataSource.Configuration.ProxyCreationEnabled = false;
                var list = DataSource.SystemInfos.Where(t => t.RoomID == RoomId && t.InfoType == 1);
                return Json(list, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteError(ex, GetType(), MethodBase.GetCurrentMethod().Name);
                throw ex;
            }
        }

        public ActionResult SetAnnouncement(int nid, string title, string content, string link)
        {
            try
            {
                SystemInfo info = DataSource.SystemInfos.FirstOrDefault(t => t.SysInfoID == nid);
                if (info != null)
                {
                    info.InfoContent = content;
                    info.InfoTitle = title;
                    info.SourceLink = link;
                    DataSource.SaveChanges();
                    RefreshLiveRoom();
                    AddUpdateSettingLog("更改公告", content, 11);
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteError(ex, GetType(), MethodBase.GetCurrentMethod().Name);
                throw ex;
            }
            return Json(false, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddAnnouncement(string title, string content, string link)
        {
            try
            {
                using (var db = new DataContextBll())
                {
                    SystemInfo info = new SystemInfo()
                    {
                        CreateUser = Users != null ? Users.UserName : "",
                        RoomID = RoomId,
                        InfoContent = content,
                        InfoTitle = title,
                        SourceLink = link,
                        ImgUrl = "",
                        SendTime = DateTime.Now,
                        InfoWeight = 0,
                        InfoType = 1
                    };
                    db.SystemInfos.Add(info);
                    db.SaveChanges();
                    RefreshLiveRoom();
                    AddUpdateSettingLog("新增公告", content, 11);
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteError(ex, GetType(), MethodBase.GetCurrentMethod().Name);
                throw ex;
            }
        }

        public ActionResult DelAnnouncement(int sid)
        {
            try
            {
                var s = DataSource.SystemInfos.FirstOrDefault(t => t.SysInfoID == sid);
                DataSource.SystemInfos.Remove(s);
                DataSource.SaveChanges();
                RefreshLiveRoom();
                AddUpdateSettingLog("删除公告", sid.ToString(), 11);
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteError(ex, GetType(), MethodBase.GetCurrentMethod().Name);
                throw ex;
            }
        }

        public ActionResult GetSysColums()
        {
            try
            {
                DataSource.Configuration.ProxyCreationEnabled = false;
                var list = DataSource.SysTvColumnses.Where(x => x.RoomID == RoomId && x.ItemType == 5).ToList();
                return Json(list, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteError(ex, GetType(), MethodBase.GetCurrentMethod().Name);
                throw ex;
            }
        }
        public ActionResult SetSyscolums(string title, string content, string link, string coulmname, string imgurl, int id)
        {
            try
            {
                content = HttpUtility.UrlDecode(content);
                SysTVColumns tm = DataSource.SysTvColumnses.FirstOrDefault(t => t.SysTVColumnID == id);
                if (tm != null)
                {
                    tm.ItemTitle = title;
                    tm.ISummary = content;
                    tm.ItemImgUrl = imgurl;
                    tm.ItemLink = link;
                    tm.ItemName = coulmname;
                    DataSource.SaveChanges();
                    RefreshLiveRoom();
                    AddUpdateSettingLog("更改专题活动", content, 12);
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteError(ex, GetType(), MethodBase.GetCurrentMethod().Name);
                throw ex;
            }
            return Json(false, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DownloadFile(string url, string path)
        {
            try
            {
                LogHelper.Instance.WriteInformation("DownloadFile url is " + url + " path is " + path);
                using (var client = new WebClient())
                {
                    client.DownloadFile(url, Server.MapPath(path));
                }
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult UploadFile()
        {
            try
            {
                var savaurl = string.Empty;
                if (Request.Files.Count > 0)
                {
                    var file = Request.Files[0];
                    if (file != null && file.ContentLength > 0 && UntilHelper.IsUploadImage(Path.GetExtension(file.FileName)))
                    {
                        string fileName = DateTime.Now.ToString("yyyyMMddHHmmssffff") + "-" + Path.GetFileName(file.FileName);

                        var imageUrl = Path.Combine(Server.MapPath("/Image/uploadfiles/"), fileName);
                        savaurl = "/Image/uploadfiles/" + fileName;

                        file.SaveAs(imageUrl);
                        try
                        {
                            var url = GetUrlPath(imageUrl);
                            LogHelper.Instance.WriteInformation("UploadFile url is " + url + " path is " + imageUrl);
                            NotifyDownloadFile(url, savaurl);
                        }
                        catch (Exception ex)
                        {
                            LogHelper.Instance.WriteError(ex, GetType(), MethodBase.GetCurrentMethod().Name);
                        }
                    }
                }
                return Json(savaurl, "text/html");
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteError(ex, GetType(), MethodBase.GetCurrentMethod().Name);
                throw ex;
            }

        }

        private void NotifyDownloadFile(string url, string path)
        {
            if (!string.IsNullOrEmpty(NotifyWebUrl))
            {
                NotifyService(NotifyWebUrl + "Home/DownloadFile?url=" + url + "&path=" + path);
            }
        }

        public ActionResult AddImagemange(string content, string link, string coulmname, string imgurl)
        {
            try
            {
                string name = Users != null ? Users.UserName : "";
                using (var db = new DataContextBll())
                {
                    SysTVColumns info = new SysTVColumns()
                    {
                        CreateUser = name,
                        ItemTitle = Users != null ? Users.UserName : "",
                        ISummary = content,
                        ItemLink = link,
                        ItemImgUrl = imgurl,
                        ItemType = 5,
                        RoomID = RoomId,
                        CreateTime = DateTime.Now,
                        ItemName = coulmname
                    };
                    db.SysTvColumnses.Add(info);
                    db.SaveChanges();
                    RefreshLiveRoom();
                    AddUpdateSettingLog("新增专题活动", content, 12);
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteError(ex, GetType(), MethodBase.GetCurrentMethod().Name);
                return Json(false, JsonRequestBehavior.AllowGet);

            }
        }
        public ActionResult DelImagemange(int sid)
        {
            try
            {
                var s = DataSource.SysTvColumnses.FirstOrDefault(t => t.SysTVColumnID == sid);
                DataSource.SysTvColumnses.Remove(s);
                DataSource.SaveChanges();
                RefreshLiveRoom();
                AddUpdateSettingLog("删除专题活动", sid.ToString(), 12);
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteError(ex, GetType(), MethodBase.GetCurrentMethod().Name);
                throw ex;
            }
        }

        public ActionResult GetSysteminfoplayfigure()
        {
            try
            {
                DataSource.Configuration.ProxyCreationEnabled = false;
                var list = DataSource.SystemInfos.Where(x => x.RoomID == RoomId && x.InfoType == 4).ToList();
                return Json(list, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteError(ex, GetType(), MethodBase.GetCurrentMethod().Name);
                throw ex;
            }
        }

        public ActionResult DelSystminfoplayigure(int sid)
        {
            try
            {
                var s = DataSource.SystemInfos.FirstOrDefault(t => t.SysInfoID == sid);
                DataSource.SystemInfos.Remove(s);
                DataSource.SaveChanges();
                RefreshLiveRoom();
                AddUpdateSettingLog("删除图片", sid.ToString(), 13);
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteError(ex, GetType(), MethodBase.GetCurrentMethod().Name);
                throw ex;
            }
        }

        public ActionResult AddSteminfoplayfigure(string url)
        {
            try
            {
                string name = Users != null ? Users.UserName : "";
                SystemInfo info = new SystemInfo()
                {
                    RoomID = RoomId,
                    CreateUser = name,
                    ImgUrl = url,
                    InfoContent = "",
                    InfoType = 4,
                    InfoTitle = "",
                    InfoWeight = 0,
                    SendTime = DateTime.Now,
                    SourceLink = ""
                };
                DataSource.SystemInfos.Add(info);
                DataSource.SaveChanges();
                RefreshLiveRoom();
                AddUpdateSettingLog("新增图片", url, 13);
                return Json(true, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteError(ex, GetType(), MethodBase.GetCurrentMethod().Name);
                throw ex;
            }
        }


        public ActionResult RemoveMessage(long chartId)
        {
            try
            {
                var entity = MessageCache.Instance.FirstOrDefault(chartId);
                if (entity != null)
                {
                    MessageCache.Instance.RemoveMessage(chartId);
                    NotifyRemoveMessage(chartId);
                }
                return Json(entity?.ischeck ?? 1, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteError(ex, GetType(), MethodBase.GetCurrentMethod().Name);
                throw ex;
            }
        }

        private void NotifyRemoveMessage(long chartId)
        {
            if (!string.IsNullOrEmpty(NotifyWebUrl))
            {
                NotifyService(NotifyWebUrl + "Home/RemoveMessage?chartId=" + chartId);
            }
        }

        [AllowAnonymous]
        public ActionResult WebIndex()
        {
            return View();
        }

        public ActionResult UploadSchedule()
        {
            return View();
        }
        public ActionResult ShowScheduleImg()
        {
            //var room = DataSource.LiveRoomses.FirstOrDefault(t => t.RoomID == RoomId);
            var schedule = DataSource.TvClassSchedules.Where(t => t.LiveRoomId == RoomId).ToList();
            return View(schedule);
        }

        public ActionResult AddScheduleImg(string url)
        {
            try
            {
                bool isExists = false;
                var schedule = DataSource.TvClassSchedules.FirstOrDefault(t => t.LiveRoomId == RoomId);
                if (schedule != null)//如果存在 删除原有的文件
                {
                    isExists = true;
                    var imageUrl = Server.MapPath(schedule.HomeUrl);
                    if (System.IO.File.Exists(imageUrl))
                    {
                        System.IO.File.Delete(imageUrl);
                    }
                }
                else
                {
                    schedule = new TVClassSchedule();
                }
                schedule.LiveRoomId = RoomId;
                schedule.HomeUrl = url;
                schedule.Teacher = string.Empty;
                schedule.TNickName = string.Empty;
                schedule.liveStartTime = string.Empty;
                schedule.liveEndTime = string.Empty;
                schedule.EffectiveEndTime = DateTime.Now;
                schedule.EffectiveStartTime = DateTime.Now;
                if (isExists)
                {
                    DataSource.Entry(schedule).State = EntityState.Modified;
                }
                else
                {
                    DataSource.TvClassSchedules.Add(schedule);
                }
                DataSource.SaveChanges();
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteError(ex, GetType(), MethodBase.GetCurrentMethod().Name);
                throw ex;
            }

        }

        public ActionResult Download()
        {
            return View();
        }
        public ActionResult About()
        {
            return View();
        }

        public ActionResult ColumnDetail(int id)
        {
            var entity = DataSource.SysTvColumnses.FirstOrDefault(t => t.SysTVColumnID == id);
            if (entity == null)
                return Invalid();

            if ((entity.ItemType == 7 && (Users == null || Users.UserRoleses.Any(t => t.Roles.PowerId < 40)))
                || (entity.ItemType == 8 && (Users == null || Users.UserRoleses.Any(t => t.Roles.PowerId < 1)))
                    || (entity.ItemType == 9 && (Users == null || Users.UserRoleses.Any(t => t.Roles.PowerId < 60))))

                entity.ISummary = "<div style=\"font-size:18px;text-align : center; \">对不起，你没有权限，详情请联系助理。</div>";
            return View(entity);
        }

        public ActionResult GetRandomQQ()
        {
            try
            {
                var qqList = SysConfigs.ServiceQQs.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                var index = new Random().Next(qqList.Length);
                return Json(qqList[index - 1].Split('-')[0], JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult QueryNewsFlashs()
        {
            try
            {
                IEnumerable<SysTVColumns> q;
                if (LiveRooms?.SysTvColumnses == null)
                {
                    q = DataSource.SysTvColumnses.Where(t => t.RoomID == RoomId && t.ItemType == 10);
                }
                else
                {
                    q = LiveRooms.SysTvColumnses.Where(t => t.ItemType == 10);
                }
                var entitylist = q.Select(t => new
                {
                    t.ItemLink,
                    t.ItemTitle,
                    t.SysTVColumnID
                }).ToList();
                return Json(entitylist, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteError(ex, GetType(), MethodBase.GetCurrentMethod().Name);
                throw ex;
            }
        }

        public ActionResult QueryComment()
        {
            try
            {
                IEnumerable<SysTVColumns> q;
                if (LiveRooms?.SysTvColumnses == null)
                {
                    q = DataSource.SysTvColumnses.Where(t => t.RoomID == RoomId && t.ItemType == 11);
                }
                else
                {
                    q = LiveRooms.SysTvColumnses.Where(t => t.ItemType == 11);
                }
                var entitylist = q.Select(t => new
                {
                    t.ItemTitle,
                    //t.CreateTime,
                    t.CreateTime,
                    t.SysTVColumnID
                }).ToList();
                return Json(entitylist, JsonRequestBehavior.AllowGet);
                //return JsonDate(entitylist);
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteError(ex, GetType(), MethodBase.GetCurrentMethod().Name);
                throw ex;
            }
        }

        public ActionResult QueryArticleInfo(int id)
        {
            try
            {
               var result = DataSource.SysTvColumnses.FirstOrDefault(t => t.SysTVColumnID == id);
                var entitylist =  new
                {
                    result.ItemTitle,
                    result.ItemName,
                    result.CreateUser,
                    result.CreateTime,
                    result.ISummary,
                    result.SysTVColumnID
                } ;
                return Json(entitylist, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteError(ex, GetType(), MethodBase.GetCurrentMethod().Name);
                throw ex;
            }

        }
 
        public ActionResult SingleService()
        {
            return View();
        }

        public ActionResult TeacherIntroduce()
        {
            return View();
        }
    }
}