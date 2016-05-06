using Online.DbHelper.Common;
using Online.DbHelper.Model;
using Online.Web.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace Online.Web.Controllers
{
    public class WechatController : BaseController
    {
        // GET: Tactics
        public ActionResult Index()
        {
            try
            {
                List<SysTVColumns> q= DataSource.SysTvColumnses.Where(t => t.RoomID == RoomId && t.ItemType == 14).OrderByDescending(x => x.CreateTime).Take(20).ToList();
                return View(q);
            }
            catch (Exception ex)  
            { 
                LogHelper.Instance.WriteError(ex, GetType(), MethodBase.GetCurrentMethod().Name);
                throw ex;
            }
        }
        public ActionResult Detail(int id = 0)
        {
            try
            {
                SysTVColumns q = DataSource.SysTvColumnses.FirstOrDefault(t => t.SysTVColumnID==id);
                return View(q);
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteError(ex, GetType(), MethodBase.GetCurrentMethod().Name);
                throw ex;
            }
        }
    }
}