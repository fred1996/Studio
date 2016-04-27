using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using Online.DbHelper.BLL;
using Online.DbHelper.Common;
using Online.DbHelper.Model;
using Online.DbHelper.Model.UserCenter;
using ServiceStack;

namespace Online.Web.DAL
{
    public class GiftHandler
    {
        private static GiftHandler _init=new GiftHandler();

        private GiftHandler()
        {
        }

        public static GiftHandler Instance
        {
            get {
                if (_init != null)
                {
                    _init = new GiftHandler();
                }
                return _init;
            }
        }
       /// <summary>
       /// 根据用户id 和礼物id获取对应用户礼物
       /// </summary>
       /// <param name="userId">用户id</param>
       /// <param name="giftId">礼物id</param>
       /// <returns></returns>
        public UserGifts GetUserGifts(long userId,int giftId)
        {
            using (var db=new UserContextBll())
            {
                return db.UserGiftses.FirstOrDefault(t=>t.UserId== userId&&t.GiftId== giftId);
            }
        }
       /// <summary>
       /// 赠送礼物
       /// </summary>
       /// <param name="model">用户礼物实体</param>
       /// <param name="user">当前用户</param>
       /// <param name="giveTeacher">是否是送给老师</param>
       /// <returns></returns>
        public bool AddUserGift(GiftLog model,bool giveTeacher=false)
        {
            using (var db = new UserContextBll())
            {
                try
                {
                    if (!giveTeacher)
                    {
                        var exists =
                            db.UserGiftses.FirstOrDefault(
                                t => t.UserId == model.ToUserId && t.GiftName == model.GiftName);
                        if (exists != null) //如果该用户已存在该礼物，只要对该用户礼物的数量进行增加
                        {
                            exists.GiftNum += model.GiftNum;
                        }
                        else
                        {
                            var usergift = new UserGifts();
                            usergift.UserId = model.ToUserId;
                            usergift.GiftName = model.GiftName;
                            usergift.GiftId = db.Gifts.FirstOrDefault(t => t.GiftName == model.GiftName).GiftId;
                            usergift.GiftNum = model.GiftNum;
                            db.UserGiftses.Add(usergift);
                        }
                    }
                    else
                    {
                       var usergift= db.UserGiftses.FirstOrDefault(t => t.UserId == model.UserId&&t.GiftName == model.GiftName);
                        usergift.GiftNum -= model.GiftNum;

                    }
                    //记录到礼物日志表中
                    db.GiftLogs.Add(model);
                    var result= db.SaveChanges();
                    return result > 0;
                }
                catch (Exception ex)
                {
                    LogHelper.Instance.WriteError(ex, GetType(), MethodBase.GetCurrentMethod().Name);
                    return false;
                }
            }
        }

        /// <summary>
        /// 注册赠送100钻石
        /// </summary>
        /// <param name="user"></param>
        public void RegistAddGift(Users user)
        {
            using (var db = new UserContextBll())
            {
                try
                {
                    var gift=db.Gifts.FirstOrDefault(t => t.GiftType == 2);
                    var GiftLog = new GiftLog();
                    GiftLog.GiftName = gift.GiftName;
                    GiftLog.GiftNum = 100;
                    GiftLog.ToUserId = user.UserID;
                    GiftLog.ToUserName = user.UserName;
                    GiftLog.UserId = 0;
                    GiftLog.UserName ="注册赠送";
                    GiftLog.CreateTime = DateTime.Now;

                    AddUserGift(GiftLog);
                }
                catch (Exception ex)
                {
                    LogHelper.Instance.WriteError(ex, GetType(), MethodBase.GetCurrentMethod().Name);
                }
            }
          
        }
    }
}