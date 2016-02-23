using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Online.DbHelper.Model;
using Online.Web.Areas.Admin.Enum;
using Online.Web.Areas.Admin.Models;
using Online.Web.DAL;
using PagedList;
using Action = Antlr.Runtime.Misc.Action;

namespace Online.Web.Areas.Admin.Controllers
{
    [AdminAuthorize]
    public partial class SysTvColumnController : BaseController
    {
        // GET: Admin/SysTVColumn
        public ActionResult Index(int page = 1)
        {
            var list = DataSource.SysTvColumnses.OrderByDescending(t=>t.SysTVColumnID).ToPagedList(page,10);
            ViewBag.Menus = ReadMenu.Instance.Menues;
            return View(list);
        }

        public ActionResult Edit(long id)
        {
            var model = DataSource.SysTvColumnses.FirstOrDefault(t => t.SysTVColumnID==id);
            var roomList = DataSource.LiveRoomses.Where(t => !t.IsDeleted).Select(t=>new SelectListItem()
            {
                Text = t.RoomName,
                Value = t.RoomID.ToString()
            });
            var column = System.Enum.GetNames(typeof (SysTvColumn)).ToList();
            var columns = column.Select(t => new SelectListItem()
            {
                Text = t,
                Value =(column.IndexOf(t)+1).ToString()
            });
            ViewBag.roomList = roomList;
            ViewBag.columns = columns;
            return View(model);
        }

        [HttpPost]
        public ActionResult EditSave(SysTVColumns column)
        {
            var model = DataSource.SysTvColumnses.FirstOrDefault(t => t.SysTVColumnID == column.SysTVColumnID);
            model.RoomID = column.RoomID;
            model.ItemTitle = column.ItemTitle ?? "";
            model.ItemName = column.ItemName == null ? "" : column.ItemName;
            model.ItemType = column.ItemType;
            model.ItemImgUrl= column.ItemImgUrl == null ? "" : column.ItemImgUrl;
            model.ItemLink = column.ItemLink == null ? "" : column.ItemLink;
            model.ISummary=column.ISummary == null ? "" : column.ISummary;
            DataSource.SaveChanges();
            AddUpdateSettingLog("栏目管理", "修改房间栏目", 119);
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
            var column = System.Enum.GetNames(typeof(SysTvColumn)).ToList();
            var columns = column.Select(t => new SelectListItem()
            {
                Text = t,
                Value = (column.IndexOf(t) + 1).ToString()
            });
            ViewBag.roomList = roomList;
            ViewBag.columns = columns;
            return View();
        }

        [HttpPost]
        public ActionResult CreateSave(SysTVColumns column)
        {
            column.CreateUser = CrmUsers == null ? "" : CrmUsers.UserName;
            column.CreateTime=DateTime.Now;
            DataSource.SysTvColumnses.Add(column);
            DataSource.SaveChanges();
            AddUpdateSettingLog("栏目管理", "新增房间栏目", 119);
            RefreshLiveRoom();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Delete(long id)
        {
            var model = DataSource.SysTvColumnses.FirstOrDefault(t => t.SysTVColumnID == id);
            DataSource.SysTvColumnses.Remove(model);
            DataSource.SaveChanges();
            AddUpdateSettingLog("栏目管理", "删除房间栏目", 119);
            RefreshLiveRoom();
            return Json(true,JsonRequestBehavior.AllowGet);
        }
    }
}