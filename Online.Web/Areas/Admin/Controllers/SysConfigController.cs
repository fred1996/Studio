using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Online.DbHelper.Model;
using Online.Web.Areas.Admin.Models;
using Online.Web.DAL;

namespace Online.Web.Areas.Admin.Controllers
{
    [AdminAuthorize]
    public class SysConfigController : BaseController
    {
        // GET: Admin/SysConfig
        public ActionResult Index()
        {
            var list = DataSource.SysConfigses;
            ViewBag.Menus = ReadMenu.Instance.Menues;
            return View(list);
        }

        public ActionResult Edit(long id)
        {
            var model = DataSource.SysConfigses.FirstOrDefault(t => t.SysConfigID == id);
            var roomList = DataSource.LiveRoomses.Where(t => !t.IsDeleted).Select(t => new SelectListItem()
            {
                Text = t.RoomName,
                Value = t.RoomID.ToString()
            });
            ViewBag.roomList = roomList;
            return View(model);
        }

        public ActionResult EditSave(SysConfigs config)
        {
            var model = DataSource.SysConfigses.FirstOrDefault(t => t.SysConfigID == config.SysConfigID);
            model.RoomId = config.RoomId;
            model.IsAllowPost = config.IsAllowPost;
            model.IsFilterMsg = config.IsFilterMsg;
            model.IsCheckMsg = config.IsCheckMsg;
            model.IsUploadFile = config.IsUploadFile;
            model.IsAllowTouristPost = config.IsAllowTouristPost;
            model.ServiceQQs = config.ServiceQQs == null ? "" : config.ServiceQQs;
            DataSource.SaveChanges();
            AddUpdateSettingLog("房间配置", "修改房间配置", 112);
            RefreshLiveRoom();
            return RedirectToAction("Index");
        }

        public ActionResult Create()
        {
            var roomList = DataSource.LiveRoomses.Where(t => !t.IsDeleted).Select(t => new SelectListItem()
            {
                Text = t.RoomName,
                Value = t.RoomID.ToString()
            });
            ViewBag.roomList = roomList;
            return View();
        }

        public ActionResult CreateSave(SysConfigs config)
        {
            config.ServiceQQs = config.ServiceQQs == null ? "" : config.ServiceQQs;
            config.LoginPwd=String.Empty;

            DataSource.SysConfigses.Add(config);
            DataSource.SaveChanges();
            AddUpdateSettingLog("房间配置", "新增房间配置", 112);
            RefreshLiveRoom();
            return RedirectToAction("Index");
        }

        public ActionResult Delete(long id)
        {
            var model = DataSource.SysConfigses.FirstOrDefault(t => t.SysConfigID == id);
            DataSource.SysConfigses.Remove(model);
            DataSource.SaveChanges();
            AddUpdateSettingLog("房间配置", "删除房间配置", 112);
            RefreshLiveRoom();
            return Json(true,JsonRequestBehavior.AllowGet);
        }
    }
}