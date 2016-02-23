using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Online.Web.Areas.Admin.Models;
using Online.Web.DAL;
using PagedList;

namespace Online.Web.Areas.Admin.Controllers
{
    [AdminAuthorize]
    public class SysChatMsgController : BaseController
    {
        // GET: Admin/SysChatMsg
        public ActionResult Index(int page=1)
        {
            var list = DataSource.SysChatMsgses.OrderByDescending(t => t.ChatID).ToPagedList(page, 10);
            ViewBag.Menus = ReadMenu.Instance.Menues;
            return View(list);
        }

        public ActionResult Delete(long id)
        {
            var model = DataSource.SysChatMsgses.FirstOrDefault(t => t.ChatID == id);
            DataSource.SysChatMsgses.Remove(model);
            DataSource.SaveChanges();
            AddUpdateSettingLog("聊天信息", "删除聊天信息", 111);
            return RedirectToAction("Index");
        }
    }
}