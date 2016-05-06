using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Aspose.Cells;
using Online.DbHelper.Model.UserCenter;
using Online.Web.Areas.Admin.Models;
using Online.Web.DAL;
using PagedList;

namespace Online.Web.Areas.Admin.Controllers
{
    public class GiftController : BaseController
    {
        // GET: Admin/Gift
        public ActionResult Index()
        {
            ViewBag.Menus = ReadMenu.Instance.Menues;
            var gifts = UserSource.Gifts;
            return View(gifts);
        }

        public ActionResult GiftCreate()
        {
            return View();
        }
        [HttpPost]
        public ActionResult CreateSave(Gift gift)
        {
            gift.GiftName = gift.GiftName == null ? string.Empty : gift.GiftName;
            gift.GiftUnit = gift.GiftUnit == null ? string.Empty : gift.GiftUnit;
            gift.CreateTime = DateTime.Now;
            UserSource.Gifts.Add(gift);
            UserSource.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult GiftEdit(int giftId)
        {
            var gifts = UserSource.Gifts.FirstOrDefault(t => t.GiftId == giftId);
            return View(gifts);
        }

        public ActionResult EditSave(Gift gift)
        {
            var gifts = UserSource.Gifts.FirstOrDefault(t => t.GiftId == gift.GiftId);
            gifts.GiftName = gift.GiftName == null ? string.Empty : gift.GiftName;
            gifts.GiftType = gift.GiftType;
            gifts.GiftUnit = gift.GiftUnit == null ? string.Empty : gift.GiftUnit;
            gifts.GiftCount = gift.GiftCount;
            UserSource.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult GiftDel(int giftId)
        {
            var gift = UserSource.Gifts.FirstOrDefault(t => t.GiftId == giftId);
            UserSource.Gifts.Remove(gift);
            UserSource.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult UserGifts(string SearchString = "", int page = 1)
        {
            IEnumerable<UserGifts> result;
            ViewBag.Menus = ReadMenu.Instance.Menues;
            result = UserSource.UserGiftses;
            ViewBag.CurrentFilter = SearchString;
            if (!string.IsNullOrEmpty(SearchString)) result = result.Where(t => t.User.UserName.Contains(SearchString));

            return View(result.OrderByDescending(t => t.Id).ToPagedList(page, 10));
        }

        public ActionResult UserGiftEdit(int id)
        {
            var usergift = UserSource.UserGiftses.FirstOrDefault(t => t.Id == id);
            return View(usergift);
        }

        public ActionResult UserGiftEditSave(UserGifts model)
        {
            var usergift = UserSource.UserGiftses.FirstOrDefault(t => t.Id == model.Id);
            usergift.GiftNum = model.GiftNum;
            AddUpdateSettingLog("用户礼物更改", string.Format("更改用户{0}的礼物数量为：{1}", usergift.UserId, model.GiftNum), 200);

            UserSource.SaveChanges();


            return RedirectToAction("UserGifts");
        }

        public ActionResult UserGiftAdd(long id)
        {
            var list = UserSource.Gifts.Select(t => new SelectListItem()
            {
                Text = t.GiftName,
                //Value = t.GiftId.ToString()
            });
            ViewBag.userId = id;
            ViewBag.giftList = list;
            return View();
        }

        public ActionResult UserGiftAddSave(UserGifts model)
        {
            var exists = UserSource.UserGiftses.FirstOrDefault(
                              t => t.UserId == model.UserId && t.GiftName == model.GiftName);
            if (exists != null) //如果该用户已存在该礼物，只要对该用户礼物的数量进行增加
            {
                exists.GiftNum += model.GiftNum;
            }
            else
            {
                var usergift = new UserGifts();
                usergift.UserId = model.UserId;
                usergift.GiftName = model.GiftName;
                usergift.GiftId = UserSource.Gifts.FirstOrDefault(t => t.GiftName == model.GiftName).GiftId;
                usergift.GiftNum = model.GiftNum;
                UserSource.UserGiftses.Add(usergift);
            }

            UserSource.SaveChanges();
            AddUpdateSettingLog("用户礼物更改", string.Format("用户{0}的礼物增加数量为：{1}", model.UserId, model.GiftNum), 200);
            return RedirectToAction("UserIndex", "User");
        }

        public ActionResult GiftLogIndex(string searchString, string reservation, string endreservation, int page = 1)
        {
            ViewBag.Menus = ReadMenu.Instance.Menues;
            IEnumerable<GiftLog> result = UserSource.GiftLogs.ToList();
            if (!string.IsNullOrEmpty(searchString))
            {
                result = result.Where(t => t.ToUserName.Contains(searchString));
                ViewBag.CurrentFilter = searchString;
            }

            if (!String.IsNullOrEmpty(reservation) && !String.IsNullOrEmpty(endreservation))
            {
                DateTime begintime = Convert.ToDateTime(reservation);
                DateTime endtime = Convert.ToDateTime(endreservation);
                result = result.Where(x => x.CreateTime >= begintime && x.CreateTime <= endtime);
                ViewBag.reservation = begintime;
                ViewBag.endreservation = endtime;
            }
            return View(result.OrderByDescending(t => t.LogId).ToPagedList(page, 10));
        }
        string PrepareFolder()
        {
            string path = Server.MapPath("~/Image/Excel/");
            string filepath = Server.MapPath("~/Image/Excel/GiftLogExport.xls");
            if (System.IO.File.Exists(filepath)) System.IO.File.Delete(filepath);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return path;
        }

        public ActionResult ReportGiftLog(string searchString, string reservation, string endreservation)
        {
            IEnumerable<GiftLog> result = UserSource.GiftLogs.Where(t => t.ToUserId == 0).ToList();

            if (!string.IsNullOrEmpty(searchString))
            {
                result = result.Where(t => t.ToUserName.Contains(searchString));
                ViewBag.CurrentFilter = searchString;
            }
            if (!String.IsNullOrEmpty(reservation) && !String.IsNullOrEmpty(endreservation))
            {
                DateTime begintime = Convert.ToDateTime(reservation);
                DateTime endtime = Convert.ToDateTime(endreservation);
                result = result.Where(x => x.CreateTime >= begintime && x.CreateTime <= endtime);
            }
            string path = PrepareFolder() + "GiftLogExport.xls";
            Aspose.Cells.Workbook workbook = new Aspose.Cells.Workbook();   //实例化
                                                                            //workbook.Open(path);
            Aspose.Cells.Worksheet sheet = workbook.Worksheets[0];
            Cells cells = sheet.Cells;//单元格

            cells[0, 0].PutValue("ID");
            cells[0, 1].PutValue("发送人");
            cells[0, 2].PutValue("发送人姓名");
            cells[0, 3].PutValue("接收人");
            cells[0, 4].PutValue("接收人姓名");
            cells[0, 5].PutValue("礼物名称");
            cells[0, 6].PutValue("礼物数量");
            cells[0, 7].PutValue("赠送时间");

            int j = 1;
            foreach (var item in result)
            {
                cells[j, 0].PutValue(item.LogId);
                cells[j, 1].PutValue(item.UserId);
                cells[j, 2].PutValue(item.UserName);
                cells[j, 3].PutValue(item.ToUserId);
                cells[j, 4].PutValue(item.ToUserName);
                cells[j, 5].PutValue(item.GiftName);
                cells[j, 6].PutValue(item.GiftNum);
                cells[j, 7].PutValue(item.CreateTime.ToString("yyyy-MM-dd hh:mm:ss"));
                j++;
            }
            workbook.Save(path, SaveFormat.Auto);
            var fileName = Server.MapPath("~/Image/Excel/GiftLogExport.xls");

            return File(fileName, "application/ms-excel", "" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".xls");
        }
    }
}