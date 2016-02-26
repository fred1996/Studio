using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Online.DbHelper.BLL;
using Online.DbHelper.Common;
using Online.DbHelper.Model;
using Online.Web.Help;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;

namespace Online.Web.DAL
{
    public class BaseController : Controller
    {
        private DataContextBll _dataSource;

        public DataContextBll DataSource
        {
            get
            {
                if (_dataSource == null)
                    _dataSource = new DataContextBll();
                return _dataSource;
            }
        }

        private UserContextBll _userSource;

        public UserContextBll UserSource
        {
            get
            {
                if (_userSource == null)
                    _userSource = new UserContextBll();
                return _userSource;
            }
        }

        private long _userId;

        public long UserId
        {
            get
            {
                if (_userId == 0)
                {
                    _userId = GetUserId();
                }
                return _userId;
            }
        }

        private long _crmUserId;

        public long CrmUserId
        {
            get
            {
                if (_crmUserId == 0)
                {
                    _crmUserId = GetCrmUserId();
                }
                return _crmUserId;
            }
        }

        private Users _crmUsers;

        public Users CrmUsers
        {
            get
            {
                if (_crmUsers == null && CrmUserId > 0)
                    _crmUsers = UserSource.Userses.FirstOrDefault(t => t.UserID == CrmUserId);
                return _crmUsers;
            }
        }

        private Users _users;

        public Users Users
        {
            get
            {
                if (_users == null && UserId > 0)
                    _users = UserSource.Userses.FirstOrDefault(t => t.UserID == UserId);

                return _users;
            }
        }

        private IEnumerable<Roles> _roleses;

        public IEnumerable<Roles> Roleses
        {
            get
            {
                if (_roleses == null)
                    _roleses = Users.UserRoleses.Select(t => t.Roles).Where(t => t.PowerId < 1000);
                return _roleses;
            }
        }

        private static LiveRooms _liveRooms;

        public static LiveRooms LiveRooms
        {
            get
            {
                try
                {
                    if (_liveRooms == null)
                    {
                        _liveRooms = ContextFactory.DataSource.LiveRoomses.FirstOrDefault(t => t.RoomID == RoomId);
                    }
                }
                catch (Exception ex)
                {
                    _liveRooms = null;
                }
                return _liveRooms;
            }
            set { _liveRooms = value; }
        }

        protected void RefreshLiveRoom(bool isNotify = true)
        {
            LiveRooms = DataSource.LiveRoomses.FirstOrDefault(t => t.RoomID == RoomId);
            if (isNotify)
                NotifyWebRefresh();
        }

        protected void NotifyWebRefresh()
        {
            if (!string.IsNullOrEmpty(NotifyWebUrl))
            {
                NotifyService(NotifyWebUrl + "Api/UpdateSysConfig?isNotify=" + false);
            }
        }

        public static bool NotifyService(string url)
        {
            try
            {
                var client = new WebClient();
                client.Encoding = Encoding.UTF8;
                var str = client.DownloadString(url);
                client.Dispose();
                return Convert.ToBoolean(str);
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        private static SysConfigs _sysConfigs;

        public static SysConfigs SysConfigs
        {
            get
            {
                if ((LiveRooms?.SysConfigses == null))
                {
                    using (var db = new DataContextBll())
                    {
                        var entity = db.SysConfigses.FirstOrDefault(t => t.RoomId == RoomId);
                        if (_sysConfigs == null)
                            _sysConfigs = entity;
                    }
                }
                else
                {
                    _sysConfigs = LiveRooms.SysConfigses.FirstOrDefault();
                    if (_sysConfigs == null)
                    {
                        using (var db = new DataContextBll())
                        {
                            _sysConfigs = db.SysConfigses.FirstOrDefault(t => t.RoomId == RoomId);
                        }
                    }
                }

                return _sysConfigs;
            }
        }

        private static List<SystemInfo> _systemInfos;

        public static List<SystemInfo> SystemInfos
        {
            get
            {

                if (LiveRooms?.SystemInfos == null)
                {
                    var entity = ContextFactory.DataSource.SystemInfos.Where(t => t.RoomID == RoomId).ToList();
                    if (_systemInfos == null)
                        _systemInfos = entity;
                }
                else
                    _systemInfos = LiveRooms.SystemInfos;
                return _systemInfos;
            }
            set { _systemInfos = value; }
        }

        private static List<string> _wordFilters;

        public static List<string> WordFilters
        {
            get
            {
                if (_wordFilters == null)
                    _wordFilters = ContextFactory.DataSource.SysDictionarieses.Where(t => t.FiledName == "WordFilterKey").Select(t => t.FiledValue).ToList();
                return _wordFilters;
            }
            set { _wordFilters = null; }
        }

        private static int _roomId;

        protected static int RoomId
        {
            get
            {
                if (_roomId == 0)
                    _roomId = Convert.ToInt32(ConfigurationManager.AppSettings["RoomId"]);
                return _roomId;
            }
        }

        protected static int ProjectOwner = Convert.ToInt32(ConfigurationManager.AppSettings["ProjectOwner"]);

        protected static int ProjectId = Convert.ToInt32(ConfigurationManager.AppSettings["ProjectID"]);

        protected string ClientIp
        {
            get { return GetRealIpAddress(HttpContext.Request); }
        }

        protected void SaveUserId(long userId)
        {
            Session["UserId"] = userId;
            SetCookie(userId);
        }

        protected void SaveCrmUserId(long userId)
        {
            Session["CrmUserId"] = userId;
            SetCrmCookie(userId);
        }


        protected string GetRealIpAddress(HttpRequestBase request)
        {
            return string.IsNullOrEmpty(request.Headers.Get("X-Real-IP"))
                ? request.UserHostAddress
                : request.Headers["X-Real-IP"];
        }
        protected long GetCrmUserId()
        {
            try
            {
                var result = Session["CrmUserId"];
                if (result != null)
                    return Convert.ToInt64(result);
                return 0;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        protected long GetUserId()
        {
            try
            {
                var result = Session["UserId"];
                if (result != null)
                    return Convert.ToInt64(result);
                var userId = GetUserIdFromCookie();
                if (userId > 0)
                {
                    Session["UserId"] = userId;
                }
                return 0;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        private long GetUserIdFromCookie()
        {
            try
            {
                var cookie = Request.Cookies["UserId"];
                if (cookie != null)
                {
                    if (string.IsNullOrEmpty(cookie.Value))
                        return 0;
                    return Convert.ToInt64(cookie.Value);
                }
            }
            catch (Exception ex)
            {
            }
            return 0;
        }

        protected void ClearCookie()
        {

            HttpCookie cookie = Request.Cookies["UserId"];
            if (cookie != null)
            {
                cookie.Expires = DateTime.Now.AddDays(-2);
                Response.Cookies.Set(cookie);

            }
        }

        private void SetCookie(long userId)
        {
            HttpCookie newcookie = new HttpCookie("UserId");
            newcookie.Value = userId.ToString();
            newcookie.Expires = DateTime.Now.AddDays(365);
            Response.AppendCookie(newcookie);

        }
        private void SetCrmCookie(long userId)
        {
            HttpCookie newcookie = new HttpCookie("CrmUserId");
            newcookie.Value = userId.ToString();
            newcookie.Expires = DateTime.Now.AddDays(365);
            Response.AppendCookie(newcookie);

        }
        private static readonly string NotifyWebUrl = ConfigurationManager.AppSettings["NotifyWebUrl"];

        private static List<string> _notifyWebUrlList = new List<string>();

        protected static List<string> NotifyWebUrlList
        {
            get
            {
                if (!_notifyWebUrlList.Any() && !string.IsNullOrEmpty(NotifyWebUrl))
                    foreach (string url in NotifyWebUrl.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        if (string.IsNullOrEmpty(url))
                            continue;
                        if (!url.EndsWith("/"))
                            _notifyWebUrlList.Add(url + "/");
                        else
                            _notifyWebUrlList.Add(url);
                    }
                return _notifyWebUrlList;
            }
        }

        protected string GetUrlPath(string imagesurl1)
        {
            try
            {
                string tmpRootDir = Server.MapPath(System.Web.HttpContext.Current.Request.ApplicationPath);//获取程序根目录
                string imagesurl2 = imagesurl1.Replace(tmpRootDir, ""); //转换成相对路径
                imagesurl2 = imagesurl2.Replace(@"\", @"/");

                return "http://" + Request.Url.Authority + "/" + imagesurl2;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }

        }
        public void AddUpdateSettingLog(string title, string description, int type)
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

        protected string GetUserNameFromCookie()
        {
            try
            {
                var cookie = Request.Cookies["UserName"];
                if (cookie != null)
                {
                    if (string.IsNullOrEmpty(cookie.Value))
                        return string.Empty;
                    return WebUtility.HtmlDecode(cookie.Value);
                }
            }
            catch (Exception ex)
            {
            }
            return string.Empty;
        }
        /// <summary>
        /// 返回处理过时间的json
        /// </summary>
        /// <param name="Data"></param>
        /// <returns></returns>
        protected ContentResult JsonDate(object Data)
        {
            var timeConverter = new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" };

            return Content(JsonConvert.SerializeObject(Data, Formatting.Indented, timeConverter));
        }
    }
}