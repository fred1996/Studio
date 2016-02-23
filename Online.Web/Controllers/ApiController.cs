using System;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using Online.DbHelper.Common;
using Online.Web.DAL;

namespace Online.Web.Controllers
{
    public class ApiController : BaseController
    {
        // GET: Api
        public ActionResult UpdateSysConfig()
        {
            try
            {
                UpdateLiveRoom();
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteError(ex,GetType(),MethodBase.GetCurrentMethod().Name);
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult UpdateWordFilters()
        {
            try
            {
                WordFilters = ContextFactory.DataSource.SysDictionarieses.Where(t=>t.FiledName== "WordFilterKey").Select(t=>t.FiledValue).ToList();
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteError(ex, GetType(), MethodBase.GetCurrentMethod().Name);
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdateSystemInfos()
        {
            try
            {
                UpdateLiveRoom();
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteError(ex, GetType(), MethodBase.GetCurrentMethod().Name);
                return Json(false, JsonRequestBehavior.AllowGet);
            }
             
        }

        private void UpdateLiveRoom()
        {
            LiveRooms = DataSource.LiveRoomses.FirstOrDefault(t => t.RoomID == RoomId);
        }
    }
}