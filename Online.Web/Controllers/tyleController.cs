using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Online.Web.Controllers
{
    public class tyleController : Controller
    {
        // GET: tyle
        public ActionResult Index()
        {
            return RedirectToAction("Index","Home");
        }
    }
}