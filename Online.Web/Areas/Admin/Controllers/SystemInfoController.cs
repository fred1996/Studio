using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Online.DbHelper.Common;
using Online.DbHelper.Model;
using Online.Web.Areas.Admin.Enum;
using Online.Web.Areas.Admin.Models;
using Online.Web.DAL;
using PagedList;

namespace Online.Web.Areas.Admin.Controllers
{
    [AdminAuthorize]
    public class SystemInfoController : BaseController
    {
        // GET: Admin/SystemInfo
        public ActionResult Index(int page=1)
        {
            var list = DataSource.SystemInfos.OrderByDescending(t => t.SysInfoID).ToPagedList(page, 10);
            ViewBag.Menus = ReadMenu.Instance.Menues;
            return View(list);
        }

        public ActionResult Edit(long id)
        {
            var model = DataSource.SystemInfos.FirstOrDefault(t => t.SysInfoID == id);
            var roomList = DataSource.LiveRoomses.Where(t => !t.IsDeleted).Select(t => new SelectListItem()
            {
                Text = t.RoomName,
                Value = t.RoomID.ToString()
            });
            var column = System.Enum.GetNames(typeof(SysInfoType)).ToList();
            var columns = column.Select(t => new SelectListItem()
            {
                Text = t,
                Value = (column.IndexOf(t) + 1).ToString()
            });
            ViewBag.roomList = roomList;
            ViewBag.columns = columns;
            return View(model);
        }
        [HttpPost]
        public ActionResult EditSave(SystemInfo info, HttpPostedFileBase image)
        {
            var savaurl = SaveFile(image);
            var model = DataSource.SystemInfos.FirstOrDefault(t => t.SysInfoID == info.SysInfoID);
            model.RoomID = info.RoomID;
            model.InfoTitle = info.InfoTitle == null ? "" : info.InfoTitle;
            model.InfoType = info.InfoType;
            model.InfoContent = info.InfoContent == null ? "" : info.InfoContent;
            model.ImgUrl = savaurl;
            model.SourceLink = info.SourceLink == null ? "" : info.SourceLink;
            model.InfoWeight = info.InfoWeight;
            DataSource.SaveChanges();
            AddUpdateSettingLog("房间消息管理", "修改房间消息", 118);
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
            var column = System.Enum.GetNames(typeof(SysInfoType)).ToList();
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
        public ActionResult CreateSave(SystemInfo info, HttpPostedFileBase image)
        {
            var savaurl = SaveFile(image);
            info.CreateUser = CrmUsers == null ? "" : CrmUsers.UserName;
            info.ImgUrl = savaurl;
            info.SendTime=DateTime.Now;
            info.InfoTitle = info.InfoTitle == null ? "" : info.InfoTitle;
            info.InfoContent = info.InfoContent == null ? "" : info.InfoContent;
            info.SourceLink = info.SourceLink == null ? "" : info.SourceLink;
            DataSource.SystemInfos.Add(info);
            DataSource.SaveChanges();
            AddUpdateSettingLog("房间消息管理", "新增房间消息", 118);
            RefreshLiveRoom();
            return RedirectToAction("Index");
        }
        [HttpPost]
        public ActionResult Delete(long id)
        {
            var model = DataSource.SystemInfos.FirstOrDefault(t => t.SysInfoID == id);
            DataSource.SystemInfos.Remove(model);
            DataSource.SaveChanges();
            AddUpdateSettingLog("房间消息管理", "删除房间消息", 118);
            RefreshLiveRoom();
            return Json(true,JsonRequestBehavior.AllowGet);
        }
       

        private string SaveFile(HttpPostedFileBase image)
        {
            var savaurl = string.Empty;
            if (image!=null&&image.ContentLength>0)
            {
                var newName = DateTime.Now.ToString("yyyyMMddHHmmss") + Path.GetExtension(image.FileName);
                if (!Directory.Exists(Server.MapPath("~/Image/uploadfiles/systeminfo/")))
                {
                    Directory.CreateDirectory(Server.MapPath("~/Image/uploadfiles/systeminfo/"));
                }
                savaurl = "../Image/uploadfiles/users/" + newName;
                var filepath = Path.Combine(Server.MapPath("~/Image/uploadfiles/systeminfo/"), newName);
                //保存文件
                image.SaveAs(filepath);
                //try
                //{
                //    var url = GetUrlPath(filepath);
                //}
                //catch (Exception ex)
                //{
                //    LogHelper.Instance.WriteError(ex, GetType(), MethodBase.GetCurrentMethod().Name);
                //}

            }
            return savaurl;
        }


    }
}