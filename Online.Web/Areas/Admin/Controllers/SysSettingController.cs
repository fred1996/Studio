using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Online.DbHelper.Model;
using Online.Web.Areas.Admin.Models;
using Online.Web.DAL;
using Online.Web.Help;
using PagedList;

namespace Online.Web.Areas.Admin.Controllers
{
    [AdminAuthorize]
    public class SysSettingController : BaseController
    {
        #region 角色模块

        // GET: Admin/SysSetting
        public ActionResult RoleIndex(int page = 1)
        {
            var list = UserSource.Roleses.OrderBy(t => t.PowerId).ToPagedList(page, 10);
            ViewBag.Menus = ReadMenu.Instance.Menues;
            return View(list);
        }

        public ActionResult RoleEdit(long id)
        {
            var model = UserSource.Roleses.FirstOrDefault(t => t.RoleID == id);
            return View(model);
        }

        [HttpPost]
        public ActionResult RoleEditSave(Roles role)
        {
            var model = UserSource.Roleses.FirstOrDefault(t => t.RoleID == role.RoleID);
            model.RoleName = role.RoleName;
            model.PowerId = role.PowerId;
            model.Description = role.Description == null ? "" : role.Description;
            UserSource.SaveChanges();
            AddUpdateSettingLog("角色管理", "修改角色信息", 113);

            return RedirectToAction("RoleIndex");
        }

        public ActionResult RoleCreate()
        {
            return View();
        }

        [HttpPost]
        public ActionResult RoleCreateSave(Roles role)
        {
            role.CreateTime = DateTime.Now;
            role.RoleName = role.RoleName == null ? "" : role.RoleName;
            role.CreateUser = CrmUsers == null ? "" : CrmUsers.UserName;
            role.Description = role.Description == null ? "" : role.Description;
            UserSource.Roleses.Add(role);
            UserSource.SaveChanges();
            AddUpdateSettingLog("角色管理", "新增角色信息", 113);
            return RedirectToAction("RoleIndex");
        }

        public ActionResult Roleable(long id)
        {
            var model = UserSource.Roleses.FirstOrDefault(t => t.RoleID == id);
            model.BizStatus = 1;
            UserSource.SaveChanges();
            AddUpdateSettingLog("角色管理", "启用角色", 113);
            return RedirectToAction("RoleIndex");
        }

        public ActionResult RoleEnable(long id)
        {
            var model = UserSource.Roleses.FirstOrDefault(t => t.RoleID == id);
            model.BizStatus = 0;
            UserSource.SaveChanges();
            AddUpdateSettingLog("角色管理", "禁用角色", 113);
            return RedirectToAction("RoleIndex");
        }

        [HttpPost]
        public ActionResult RoleDelete(long id)
        {
            var model = UserSource.Roleses.FirstOrDefault(t => t.RoleID == id);
            UserSource.Roleses.Remove(model);
            UserSource.SaveChanges();
            AddUpdateSettingLog("角色管理", "删除角色信息", 113);
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RoleUsers(long roleid = 0, int page = 1)
        {
            roleid = roleid == 0 ? long.Parse(Session["roleid"].ToString()) : roleid;
            Session["roleid"] = roleid;
            var model = UserSource.Roleses.FirstOrDefault(t => t.RoleID == roleid);
            var list = model.UserRoleses.Select(t => t.UserId).OrderByDescending(r => r);
            var userList =
                UserSource.Userses.Where(t => list.Contains(t.UserID))
                    .OrderByDescending(r => r.UserID)
                    .ToPagedList(page, 10);
            ViewBag.Menus = ReadMenu.Instance.Menues;
            return View(userList);
        }

        public ActionResult RoleUserEdit(long userid)
        {
            var roleid = long.Parse(Session["roleid"].ToString());
            var user = UserSource.UserRoleses.FirstOrDefault(t => t.UserId == userid && t.RoleId == roleid);
            var list = UserSource.Roleses.OrderBy(r => r.PowerId).Select(t => new SelectListItem()
            {
                Text = t.RoleName,
                Value = t.RoleID.ToString()
            });
            ViewBag.roleList = list;
            return View(user);
        }

        public ActionResult RoleUserEditSave(UserRoles userRole, string Password)
        {
            var model = UserSource.UserRoleses.FirstOrDefault(t => t.AutoId == userRole.AutoId);
            model.RoleId = userRole.RoleId;
            if (!string.IsNullOrEmpty(Password))
            {
                var newpwd = UntilHelper.GetMd5HashCode(Password);
                var user = UserSource.Userses.FirstOrDefault(t => t.UserID == userRole.UserId);
                user.Password = newpwd;

            }

            AddUpdateSettingLog("角色管理", "修改用户角色", 113);
            UserSource.SaveChanges();
            return RedirectToAction("RoleUsers");
        }

        #endregion

        #region 权限模块

        public ActionResult PermissionIndex(int page = 1)
        {
            var list = UserSource.Permissions.OrderByDescending(t => t.PermissionID).ToPagedList(page, 10);
            ViewBag.Menus = ReadMenu.Instance.Menues;
            return View(list);
        }

        public ActionResult PermissionEdit(long id)
        {
            var model = UserSource.Permissions.FirstOrDefault(t => t.PermissionID == id);
            return View(model);
        }

        public ActionResult PermissionEditSave(Permissions permission)
        {
            var model = UserSource.Permissions.FirstOrDefault(t => t.PermissionID == permission.PermissionID);
            model.PName = permission.PName;
            model.Description = permission.Description == null ? "" : permission.Description;
            UserSource.SaveChanges();
            AddUpdateSettingLog("权限管理", "修改权限信息", 114);
            return RedirectToAction("PermissionIndex");
        }

        public ActionResult PermissionCreate()
        {
            return View();
        }

        public ActionResult PermissionCreateSave(Permissions permissions)
        {
            permissions.Description = permissions.Description == null ? "" : permissions.Description;
            permissions.PName = permissions.PName == null ? "" : permissions.PName;
            UserSource.Permissions.Add(permissions);
            UserSource.SaveChanges();
            AddUpdateSettingLog("权限管理", "新增权限信息", 114);
            return RedirectToAction("PermissionIndex");
        }

        public ActionResult PermissionDelete(long id)
        {
            var model = UserSource.Permissions.FirstOrDefault(t => t.PermissionID == id);
            UserSource.Permissions.Remove(model);
            UserSource.SaveChanges();
            AddUpdateSettingLog("权限管理", "删除权限信息", 113);
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region 角色权限

        public ActionResult RolePermissionIndex(int page = 1)
        {
            var RPList = UserSource.Role_Permissions.Select(t => new
            {
                Role_PermissionID = t.Role_PermissionID,
                RoleID = t.RoleID,
                RoleName = t.Roles.RoleName,
                PermissionID = t.PermissionID,
                PName = t.Permissions.PName
            });
            var rolelist = UserSource.Role_Permissions.Select(t => new
            {
                RoleID = t.RoleID,
                RoleName = t.Roles.RoleName
            }).Distinct();
            var grouplist =
                rolelist.GroupJoin(RPList, a => a.RoleID, b => b.RoleID, (a, b) => new RolePermissionViewModel
                {
                    RoleName = a.RoleName,
                    RoleID = a.RoleID,
                    Permissions = b.Select(x => x.PName).ToList()
                }).OrderByDescending(t => t.RoleID).ToPagedList(page, 10);
            ViewBag.Menus = ReadMenu.Instance.Menues;
            return View(grouplist);
        }

        public ActionResult RolePermissionEdit(long roleid)
        {
            var allPermissions = UserSource.Permissions;
            var allRole = UserSource.Roleses.Select(t => new SelectListItem()
            {
                Text = t.RoleName,
                Value = t.RoleID.ToString()
            });
            var rolepermisssion =
                UserSource.Role_Permissions.Where(t => t.RoleID == roleid).Select(t => t.PermissionID).ToList();
            var model = new RolePermissionEditViewModel()
            {
                RoleID = roleid,
                Permissions = rolepermisssion
            };
            ViewBag.allPermissions = allPermissions;
            ViewBag.allRole = allRole;
            return View(model);
        }

        public ActionResult RolePermissionEditSave(int? roleId, int?[] permissions)
        {
            var old = UserSource.Role_Permissions.Where(t => t.RoleID == roleId);
            foreach (var item in old)
            {
                UserSource.Role_Permissions.Remove(item);
            }
            UserSource.SaveChanges();

            foreach (var permission in permissions)
            {
                var model = new Role_Permissions()
                {
                    RoleID = roleId,
                    PermissionID = permission,
                };
                UserSource.Role_Permissions.Add(model);
            }
            AddUpdateSettingLog("角色权限管理", "修改角色权限", 115);
            UserSource.SaveChanges();
            return RedirectToAction("RolePermissionIndex");
        }

        public ActionResult RolePermissionCreate()
        {
            var allPermissions = UserSource.Permissions;
            var allRole = UserSource.Roleses.OrderBy(k => k.PowerId).Select(t => new SelectListItem()
            {
                Text = t.RoleName,
                Value = t.RoleID.ToString()
            });
            ViewBag.allPermissions = allPermissions;
            ViewBag.allRole = allRole;
            return View();
        }

        [HttpPost]
        public ActionResult ExistsRole(int roleId)
        {
            var result = UserSource.Role_Permissions.Any(t => t.RoleID == roleId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RolePermissionRemove(int? roleId)
        {
            var old = UserSource.Role_Permissions.Where(t => t.RoleID == roleId);
            foreach (var item in old)
            {
                UserSource.Role_Permissions.Remove(item);
            }
            UserSource.SaveChanges();
            AddUpdateSettingLog("角色权限管理", "删除角色权限", 115);
            return RedirectToAction("RolePermissionIndex");
        }

        #endregion

        #region 系统字典

        public ActionResult SysDictionarieIndex(int page = 1)
        {
            var list = DataSource.SysDictionarieses.OrderByDescending(t => t.SysDictID).ToPagedList(page, 10);
            ViewBag.Menus = ReadMenu.Instance.Menues;
            return View(list);
        }

        public ActionResult SysDictionarieCreate()
        {
            return View();
        }

        public ActionResult SysDictionarieCreateSave(SysDictionaries model)
        {
            model.OperatorName = CrmUsers == null ? "" : CrmUsers.UserName;
            model.CreateTime = DateTime.Now;
            model.UpdateTime = DateTime.Now;
            model.Description = model.Description == null ? "" : model.Description;
            DataSource.SysDictionarieses.Add(model);
            DataSource.SaveChanges();
            AddUpdateSettingLog("系统字典", "新增字典", 116);
            return RedirectToAction("SysDictionarieIndex");
        }

        public ActionResult SysDictionarieEdit(int id)
        {
            var model = DataSource.SysDictionarieses.FirstOrDefault(t => t.SysDictID == id);
            return View(model);
        }

        public ActionResult SysDictionarieEditSave(SysDictionaries model)
        {
            var dic = DataSource.SysDictionarieses.FirstOrDefault(t => t.SysDictID == model.SysDictID);
            dic.FiledName = model.FiledName;
            dic.FiledValue = model.FiledValue;
            dic.Description = model.Description;
            DataSource.SaveChanges();
            AddUpdateSettingLog("系统字典", "修改字典", 116);
            return RedirectToAction("SysDictionarieIndex");
        }

        public ActionResult SysDictionarieRemove(int id)
        {
            var model = DataSource.SysDictionarieses.FirstOrDefault(t => t.SysDictID == id);
            DataSource.SysDictionarieses.Remove(model);
            DataSource.SaveChanges();
            AddUpdateSettingLog("系统字典", "删除字典", 116);
            return RedirectToAction("SysDictionarieIndex");
        }

        #endregion

        #region 课程表

        public ActionResult TVClassScheduleIndex()
        {
            var entityList = DataSource.TvClassSchedules.ToList();
            ViewBag.Menus = ReadMenu.Instance.Menues;
            return View(entityList);
        }

        public ActionResult TVClassScheduleEdit(int id)
        {
            var entity = DataSource.TvClassSchedules.FirstOrDefault(t => t.SCId == id);
            ViewBag.LiveRoomId = new SelectList(DataSource.LiveRoomses.Where(t => !t.IsDeleted), "RoomID", "RoomName");
            return View(entity);
        }

        [HttpPost]
        public ActionResult TVClassScheduleEditSave(TVClassSchedule entity)
        {
            DataSource.Entry(entity).State = EntityState.Modified;
            DataSource.SaveChanges();
            AddUpdateSettingLog("课程表安排", "修改课程表", 117);
            return RedirectToAction("TVClassScheduleIndex");
        }

        public ActionResult TvClassScheduleCreate()
        {
            ViewBag.LiveRoomId = new SelectList(DataSource.LiveRoomses.Where(t => !t.IsDeleted), "RoomID", "RoomName");
            var entity = new TVClassSchedule();
            entity.EffectiveStartTime = DateTime.Now;
            entity.EffectiveEndTime = DateTime.Now.AddDays(7);
            return View(entity);
        }

        [HttpPost]
        public ActionResult TvClassScheduleCreateSave(TVClassSchedule entity)
        {
            DataSource.TvClassSchedules.Add(entity);
            DataSource.SaveChanges();
            AddUpdateSettingLog("课程表安排", "新增课程表", 117);
            return RedirectToAction("TVClassScheduleIndex");
        }

        public ActionResult TvClassScheduleRemove(int id)
        {
            var entity = DataSource.TvClassSchedules.FirstOrDefault(t => t.SCId == id);
            if (entity != null)
            {
                DataSource.TvClassSchedules.Remove(entity);
                DataSource.SaveChanges();
                AddUpdateSettingLog("课程表安排", "删除课程表", 117);
            }

            return RedirectToAction("TVClassScheduleIndex");
        }
        #endregion
    }

}