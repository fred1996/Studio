using Online.Web.DAL;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Reflection;
using PagedList.Mvc;
using Online.Web.Areas.Admin.Models;

namespace Online.Web.Areas.Admin.Controllers
{
    [AdminAuthorize]
    public class LogerController : BaseController
    {

        public ActionResult UserLogerIndex(int page = 1)
        {
            ViewBag.Menus = ReadMenu.Instance.Menues;
            var messages = UserSource.UserActionLogs.Where(x => x.Type < 100);
            return View(messages.OrderByDescending(x => x.UserActionLogId).ToPagedList(page, 10));
        }
        public ActionResult AreasLogerIndex(string SearchString,int page = 1)
        {
            ViewBag.Menus = ReadMenu.Instance.Menues;
            var messages = UserSource.UserActionLogs.Where(x => x.Type >= 100);
            if (!string.IsNullOrEmpty(SearchString))
            {
                messages= messages.Where(t => t.Title.Contains(SearchString) || t.Description.Contains(SearchString));
                ViewBag.CurrentFilter = SearchString;
            }
            return View(messages.OrderByDescending(x => x.UserActionLogId).ToPagedList(page, 10));
        }
    }
}