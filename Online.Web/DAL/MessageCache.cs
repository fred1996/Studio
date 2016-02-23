using System;
using System.Collections.Generic;
using System.Linq;
using Online.DbHelper.BLL;
using Online.DbHelper.Model;
using Online.Web.Help;
using Online.Web.Models;

namespace Online.Web.DAL
{
    public class MessageCache
    {
        private MessageCache()
        {
            if (!MessageList.Any())
            {
                InitMessages();
            }
        }

        private void InitMessages()
        {
            try
            {
                using (var db = new DataContextBll())
                {
                    var sysChatMsgses = db.SysChatMsgses.Where(t => t.RoomID == UntilHelper.RoomId).OrderByDescending(t => t.SendTime).Take(300).ToList();
                    ConvertMessage(sysChatMsgses);
                }
            }
            catch (Exception ex)
            {
            }
        }

        private void ConvertMessage(List<SysChatMsgs> sysChatMsgses)
        {
            foreach (var entity in sysChatMsgses)
            {
                var messageInfo = new MessageInfo();
                messageInfo.ChatID = entity.ChatID;
                messageInfo.uid = Convert.ToInt32(entity.FromUserID);
                messageInfo.from = entity.FromUserName;
                messageInfo.touid = Convert.ToInt32(entity.ToUserID);
                messageInfo.to = entity.ToUserName;
                messageInfo.roomid = Convert.ToInt32(entity.RoomID);
                messageInfo.msg = entity.MsgContent;
                messageInfo.msgtype = entity.MsgType;
                messageInfo.ischeck = entity.IsCheck ? 1 : 0;
                messageInfo.postfile = entity.FilePath;
                messageInfo.createTime = entity.SendTime;
                messageInfo.sendtime = entity.SendTime.ToString("HH:mm");
                using (var db = new UserContextBll())
                {
                    long userId = Convert.ToInt64(entity.FromUserID);
                    var userRoles = db.UserRoleses.Where(t => t.UserId == userId);
                    if (userRoles.Any())
                    {
                        var role = userRoles.FirstOrDefault(t => t.RoleId == userRoles.Where(c => c.Roles.PowerId < 1000).Max(c => c.RoleId));
                        if (role != null)
                        {
                            messageInfo.roleid = Convert.ToInt32(role.Roles.PowerId);
                            messageInfo.rolename = role.Roles.RoleName;
                        }
                        else
                        {
                            messageInfo.roleid = 0;
                            messageInfo.rolename = "游客";
                        }
                    }
                }
                MessageList.Add(messageInfo);
            }
        }

        private static MessageCache _instance;

        private static object SyncObj = new object();

        public static MessageCache Instance
        {
            get
            {
                lock (SyncObj)
                {
                    if (_instance == null)
                    {
                        _instance = new MessageCache();
                    }
                }
                return _instance;
            }
        }

        public List<MessageInfo> MessageList = new List<MessageInfo>();

        public void AddMessage(MessageInfo entity)
        {
            entity.createTime=DateTime.Now;
            MessageList.Add(entity);
            if (MessageList.Count >= 300)
                MessageList.Remove(MessageList.First(t => t.createTime == MessageList.Min(c => c.createTime)));
        }

        public IEnumerable<MessageInfo> GetTop(int count)
        {
            return MessageList.OrderByDescending(t => t.createTime).Take(count);
        }

        public MessageInfo FirstOrDefault(string msg, DateTime createTime)
        {
            return MessageList.FirstOrDefault(t => t.createTime == createTime && t.msg == msg);
        }
        public MessageInfo FirstOrDefault(long chatId)
        {
            return MessageList.FirstOrDefault(t => t.ChatID == chatId);
        }

        public bool Update(MessageInfo entity)
        {
            var first = MessageList.FirstOrDefault(t => t.ChatID == entity.ChatID);
            if (first != null)
            {
                MessageList.Remove(first);
                MessageList.Add(entity);
                return true;
            }
            return false;
        }

        public bool SetChecked(long chatId)
        {
            var first = MessageList.FirstOrDefault(t => t.ChatID == chatId);
            if (first != null)
            {
                first.ischeck = 1;
                return true;
            }
            return false;
        }

        public IEnumerable<MessageInfo> GetCheckedTop(int count)
        {
            return MessageList.Where(t => t.ischeck == 1).OrderByDescending(t => t.createTime).Take(count);
        }

        public bool RemoveMessage(long chatId)
        {
            var entity = MessageList.FirstOrDefault(t => t.ChatID == chatId);
            if (entity != null)
                MessageList.Remove(entity);
            return true;
        }
    }
}