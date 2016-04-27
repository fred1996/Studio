using System;
using System.Collections.Generic;
using System.Configuration;
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
using Online.Web.Help;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using ServiceStack;
using System.Timers;
using System.IO;

namespace Online.Web.DAL
{
    public class BaseController : Controller
    {
        protected const Int32 CACHE_MAX_COUNT = 500;
        private static string redisHost = ConfigurationManager.AppSettings["RedisServerHosts"];
        public BaseController()
        {
            RedisClienHelper.Init(new string[] { redisHost }, new string[] { redisHost });
        }
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
            if (NotifyWebUrlList.Any())
            {
                NotifyWebUrlList.ForEach(t =>
                {
                    NotifyService(t + "Api/UpdateSysConfig?isNotify=" + false);
                });
            }
        }

        public bool NotifyService(string url)
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
                LogHelper.Instance.WriteError(ex.Message + ex.StackTrace + url, GetType(), MethodBase.GetCurrentMethod().Name);
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
                    return WebUtility.UrlDecode(cookie.Value);
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
        /// <summary>
        /// 过滤标记
        /// </summary>
        /// <param name="NoHTML">包括HTML，脚本，数据库关键字，特殊字符的源码 </param>
        /// <returns>已经去除标记后的文字</returns>
        public static string HTMLFilter(string Htmlstring)
        {
            if (Htmlstring == null)
            {
                return string.Empty;
            }
            else
            {
                //删 除脚本
                Htmlstring = Regex.Replace(Htmlstring, @"<script[^>]*?>.*?</script>", "", RegexOptions.IgnoreCase);
                //删 除HTML
                //Htmlstring = Regex.Replace(Htmlstring, @"<(.[^>]*)>", "", RegexOptions.IgnoreCase);
                //Htmlstring = Regex.Replace(Htmlstring, @"([\r\n])[\s]+", "", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, @"-->", "", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, @"<!--.*", "", RegexOptions.IgnoreCase);

                Htmlstring = Regex.Replace(Htmlstring, @"&(quot|#34);", "\"", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, @"&(amp|#38);", "&", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, @"&(lt|#60);", "<", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, @"&(gt|#62);", ">", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, @"&(nbsp|#160);", " ", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, @"&(iexcl|#161);", "\xa1", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, @"&(cent|#162);", "\xa2", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, @"&(pound|#163);", "\xa3", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, @"&(copy|#169);", "\xa9", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, @"&#(\d+);", "", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, "xp_cmdshell", "", RegexOptions.IgnoreCase);

                //删 除与数据库相关的词 
                // Htmlstring = Regex.Replace(Htmlstring, "select", "", RegexOptions.IgnoreCase);
                // Htmlstring = Regex.Replace(Htmlstring, "insert", "", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, "delete from", "", RegexOptions.IgnoreCase);
                // Htmlstring = Regex.Replace(Htmlstring, "count''", "", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, "drop table", "", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, "truncate", "", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, "asc", "", RegexOptions.IgnoreCase);
                //Htmlstring = Regex.Replace(Htmlstring, "mid", "", RegexOptions.IgnoreCase);
                // Htmlstring = Regex.Replace(Htmlstring, "char", "", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, "xp_cmdshell", "", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, "exec master", "", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, "net localgroup administrators", "", RegexOptions.IgnoreCase);
                //Htmlstring = Regex.Replace(Htmlstring, "and", "", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, "net user", "", RegexOptions.IgnoreCase);
                //Htmlstring = Regex.Replace(Htmlstring, "or", "", RegexOptions.IgnoreCase);
                //Htmlstring = Regex.Replace(Htmlstring, "net", "", RegexOptions.IgnoreCase);
                //Htmlstring =  Regex.Replace(Htmlstring,"*", "", RegexOptions.IgnoreCase);
                //Htmlstring =  Regex.Replace(Htmlstring,"-", "", RegexOptions.IgnoreCase);
                //Htmlstring = Regex.Replace(Htmlstring, "delete", "", RegexOptions.IgnoreCase);
                // Htmlstring = Regex.Replace(Htmlstring, "drop", "", RegexOptions.IgnoreCase);
                //Htmlstring = Regex.Replace(Htmlstring, "script", "", RegexOptions.IgnoreCase);

                //特殊的字符
                //Htmlstring = Htmlstring.Replace("<", "");
                //Htmlstring = Htmlstring.Replace(">", "");
                Htmlstring = Htmlstring.Replace("*", "");
                // Htmlstring = Htmlstring.Replace("-", "");
                //Htmlstring = Htmlstring.Replace("?", "");
                // Htmlstring = Htmlstring.Replace(",", "");
                //Htmlstring = Htmlstring.Replace("/", "");
                Htmlstring = Htmlstring.Replace(";", "");
                Htmlstring = Htmlstring.Replace("*/", "");
                Htmlstring = Htmlstring.Replace("\r\n", "");
                //Htmlstring = HttpContext.Current.Server.HtmlEncode(Htmlstring).Trim();

                return Htmlstring;
            }

        }
        public static string EncodeBase64(string code_type, string code)
        {
            string encode = "";
            byte[] bytes = Encoding.GetEncoding(code_type).GetBytes(code);
            try
            {
                encode = Convert.ToBase64String(bytes);
            }
            catch
            {
                encode = code;
            }
            return encode;
        }
        public static string DecodeBase64(string code_type, string code)
        {
            string decode = "";
            byte[] bytes = Convert.FromBase64String(code);
            try
            {
                decode = Encoding.GetEncoding(code_type).GetString(bytes);
            }
            catch
            {
                decode = code;
            }
            return decode;
        }
        protected void ClearIp(string ip)
        {
            var model = RedisClienHelper.List_GetList<Cc>("Banned").FindAll(t => t.UserName == ip || t.Ip == ip);
            model.Each(t =>
            {
                if (model != null) RedisClienHelper.List_Remove("Banned", t);
            });
        }
        public class Cc
        {
            public string Ip { get; set; }
            public string UserName { get; set; }

            public string Action { get; set; }
        }


        private static Dictionary<string, short> _IpAdresses = new Dictionary<string, short>();
        //private static Stack<string> _Banned = new Stack<string>();
        //private static List<Cc> _Banned = new List<Cc>();
        private static Timer _Timer = CreateTimer();
        private static Timer _BannedTimer = CreateBanningTimer();

        private int BANNED_REQUESTS = Convert.ToInt32(ConfigurationManager.AppSettings["BANNED_REQUESTS"]); //规定时间内访问的最大次数  
        private const int REDUCTION_INTERVAL = 2000; // 1 秒（检查访问次数的时间段）  
        private const int RELEASE_INTERVAL = 5 * 60 * 1000; // 5 minutes 
        private static List<string> WhiteList = ConfigurationManager.AppSettings["WhiteList"].Split(';').ToList();

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //var action = filterContext.ActionDescriptor.ActionName.ToLower();
            //if (action.Contains("invalid")) return;
            ////访问站点特征码，站点 + IP地址 
            //string code = "[" + action + "]" + "[" + ClientIp + "]" + "[" + HttpContext.Request.Url + "]";
            //if (RedisClienHelper.List_GetList<Cc>("Banned").Any(t => (t.Ip == ClientIp || t.UserName == GetUserNameFromCookie()) && t.Action == action))
            //{
            //    filterContext.Result = new RedirectResult("/Home/Invalid");
            //}
            //CheckIpAddress(code, action);
            base.OnActionExecuting(filterContext);
        }



        /// <summary> 
        /// Checks the requesting IP address in the collection 
        /// and bannes the IP if required. 
        /// </summary> 
        private void CheckIpAddress(string code, string action)
        {
            if (!_IpAdresses.ContainsKey(code))
            {
                _IpAdresses[code] = 1;
            }
            else if (_IpAdresses[code] == BANNED_REQUESTS)
            {

                if (!WhiteList.Contains(ClientIp))
                {
                    RedisClienHelper.List_Add("Banned",
                        new Cc() { Ip = ClientIp, UserName = GetUserNameFromCookie(), Action = action });
                }
                else
                {
                    new Task(() =>
                    {
                        WriteCC(code);//记录CC可疑IP
                    });
                    _IpAdresses.Remove(code);
                }

            }
            else
            {
                _IpAdresses[code]++;
            }
        }
        /// <summary> 
        /// 将可疑CC攻击IP写到文件中 
        /// </summary> 
        /// <param name="ip"></param> 
        private void WriteCC(string code)
        {
            StreamWriter sw = null;
            try
            {
                string dir = Server.MapPath("~/CC/");
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                sw = new StreamWriter(dir + DateTime.Now.ToString("yyyy-MM-dd") + ".txt", true);
                var brower = HttpContext.Request.Browser.Browser;
                var version = HttpContext.Request.Browser.Version;
                sw.Write(GetUserNameFromCookie() + " " + code + " " + brower + " " + version + "  " + "  次数：" + _IpAdresses[code] + " " + DateTime.Now + "\r\n");
                sw.Close();
            }
            catch (Exception ex)
            {
                sw.Dispose();
                sw.Close();
            }

        }



        /// <summary> 
        /// Creates the timer that substract a request 
        /// from the _IpAddress dictionary. 
        /// </summary> 
        private static Timer CreateTimer()
        {

            Timer timer = GetTimer(REDUCTION_INTERVAL);
            timer.Elapsed += new ElapsedEventHandler(TimerElapsed);
            return timer;
        }
        /// <summary> 
        /// Creates the timer that removes 1 banned IP address 
        /// everytime the timer is elapsed. 
        /// </summary> 
        /// <returns></returns> 
        private static Timer CreateBanningTimer()
        {
            Timer timer = GetTimer(RELEASE_INTERVAL);
            timer.Elapsed += delegate
            {
                if (RedisClienHelper.List_Count("Banned") > 0)
                {
                    var model = RedisClienHelper.List_GetList<Cc>("Banned").First();
                    RedisClienHelper.List_Remove("Banned", model);
                }
            };
            return timer;
        }
        /// <summary> 
        /// Creates a simple timer instance and starts it. 
        /// </summary> 
        /// <param name="interval">The interval in milliseconds.</param> 
        private static Timer GetTimer(int interval)
        {
            Timer timer = new Timer();
            timer.Interval = interval;
            timer.Start();
            return timer;
        }
        /// <summary> 
        /// Substracts a request from each IP address in the collection. 
        /// </summary> 
        private static void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                foreach (string key in _IpAdresses.Keys)
                {
                    _IpAdresses[key]--;
                    if (_IpAdresses[key] == 0)
                        _IpAdresses.Remove(key);
                }
            }
            catch { }
        }
    }
}