using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Online.DbHelper.Model;
using Online.Web.DAL;
using Online.DbHelper.Common;
using System.Reflection;
using Online.Web.Areas.Admin.Models;

namespace Online.Web.Areas.Admin.Controllers
{
    [AdminAuthorize]
    public class LiveTvController : BaseController
    {
        // GET: Admin/LiveTv
        public ActionResult Index()
        {
            ViewBag.Menus = ReadMenu.Instance.Menues;
            var roomList = DataSource.LiveRoomses.Where(t => !t.IsDeleted);
            return View(roomList);
        }

        public ActionResult Edit(long id)
        {
            var model = DataSource.LiveRoomses.FirstOrDefault(t => t.RoomID == id);
            return View(model);
        }

        public ActionResult EditSave(LiveRooms room)
        {
            var model = DataSource.LiveRoomses.FirstOrDefault(t => t.RoomID == room.RoomID);
            model.RoomName = room.RoomName;
            model.BizStatus = room.BizStatus;
            DataSource.SaveChanges();
            AddUpdateSettingLog("房间管理", "房间修改操作", 109);
            RefreshLiveRoom();
            return RedirectToAction("Index");
        }

        public ActionResult Delete(long id)
        {
            var model = DataSource.LiveRoomses.FirstOrDefault(t => t.RoomID == id);
            model.IsDeleted = true;
            DataSource.SaveChanges();
            AddUpdateSettingLog("房间管理", "房间删除操作", 110);
            RefreshLiveRoom();
            return Json(true,JsonRequestBehavior.AllowGet);
        }

        public ActionResult Create()
        {
            return View();
        }

        public ActionResult CreateSave(LiveRooms room)
        {
            Save(room);
            RefreshLiveRoom();
            return RedirectToAction("Index");
        }

        private void Save(LiveRooms room)
        {
            room.CreateUser = CrmUsers == null ? "" : CrmUsers.UserName;
            room.CreateTime = DateTime.Now;
            room.RType = 0;
            room.AdminMsgBackgroundStyle = string.Empty;
            room.AnchorName = string.Empty;
            room.giftNum = 0;
            room.LastChangeTime = DateTime.Now;
            room.LimitUserNum = 2000;
            room.LiveTVID = 0;
            room.LogoTitle = string.Empty;
            room.RoomTags = string.Empty;
            room.TransitionYYRoomID = string.Empty;
            room.LiveStartTime = DateTime.Now;
            room.LiveEndTime = DateTime.Now;
            room.WelcomeUrl = string.Empty;
            room.LogoUrl = string.Empty;
            room.WelcomeUrl = string.Empty;
            room.WelcomeText = string.Empty;
            room.UpdateUser = CrmUsers==null?"":CrmUsers.UserName;
            AddUpdateSettingLog("房间管理", "房间保存操作", 121);
            DataSource.LiveRoomses.Add(room);
            DataSource.SaveChanges();
        }
    }
}