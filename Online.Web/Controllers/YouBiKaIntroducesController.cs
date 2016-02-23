using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Online.Web.Controllers
{
    public class YouBiKaIntroducesController : Controller
    {

        public ActionResult YouBiKa()
        {
            return View();
        }

        public ActionResult Coin(string id)
        {
            ViewBag.id = id;
            return View();
        }

        public ActionResult Stamp(string id)
        {
            ViewBag.id = id;
            return View();
        }
        public ActionResult Currency(string id)
        {
            ViewBag.id = id;
            return View();
        }

        public ActionResult GoldCoin(string id)
        {
            ViewBag.id = id;
            return View();
        }
        public ActionResult PostageStamp(string id)
        {
            ViewBag.id = id;
            return View();
        }
        
    }
}