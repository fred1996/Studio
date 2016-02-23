using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Online.DbHelper.BLL;
using Online.DbHelper.Common;
using Online.DbHelper.Model;

namespace Online.Web.DAL
{
    public class DataQueueDal : BaseQueueDAL<dynamic>
    {
        private static object synObj = new object();

        private static DataQueueDal _instance;

        public static DataQueueDal Instance
        {
            get
            {
                lock (synObj)
                {
                    if (_instance == null)
                    {
                        _instance = new DataQueueDal();
                    }
                }
                return _instance;
            }
        }

        private DataQueueDal()
        {

        }

        public override void OnNotify(dynamic entity)
        {
            if (entity is UserActionLog)
                AddUserActionLog(entity);
        }

        private void AddUserActionLog(UserActionLog entity)
        {
            using (var db = new UserContextBll())
            {
                db.UserActionLogs.Add(entity);
                db.SaveChanges();
            }
        }
    }
}