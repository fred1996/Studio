using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using ServiceStack;
using ServiceStack.Redis;

namespace Online.Web.Help
{
    public class RedisClienHelper
    {
       #region .ctor
        private RedisClienHelper() { }
        #endregion

       #region private Property
        private static PooledRedisClientManager _redisClientsMan = null;
        private static readonly object padlock = new object();
        #endregion

       #region 客户端连接池
        private static void CreateManager(string[] readWriteHosts, Int32 maxWritePoolSize, string[] readOnlyHosts, Int32 maxReadPoolSize)
        {
            lock (padlock)
            {
                // 支持读写分离，均衡负载 
                if (_redisClientsMan == null)
                {
                    _redisClientsMan = new PooledRedisClientManager(readWriteHosts, readOnlyHosts, new RedisClientManagerConfig
                    {
                        MaxWritePoolSize = maxWritePoolSize, // “写”链接池链接数 
                        MaxReadPoolSize = maxReadPoolSize, // “读”链接池链接数 
                        AutoStart = true,
                    });
                }
            }
        }

        /// <summary>
        /// 初始化，构造客户端链接池
        /// </summary>
        /// <param name="readWriteHosts">读写服务地址，eg:127.0.0.1:6379，127.0.0.1:6378</param>
        /// <param name="readOnlyHosts">只读读服务地址，eg:127.0.0.1:6379，127.0.0.1:6378</param>
        public static void Init(string[] readWriteHosts, string[] readOnlyHosts)
        {
            if (_redisClientsMan == null)
            {
                CreateManager(readWriteHosts, 5, readOnlyHosts, 5);
            }

        }

        /// <summary>
        /// 初始化，构造客户端链接池
        /// </summary>
        /// <param name="readWriteHosts">读写服务地址，eg:127.0.0.1:6379，127.0.0.1:6378</param>
        /// <param name="maxWritePoolSize">最大写链接池大小,默认5</param>
        /// <param name="readOnlyHosts">只读读服务地址，eg:127.0.0.1:6379，127.0.0.1:6378</param>
        /// <param name="maxReadPoolSize">最大读链接池大小,默认5</param>
        public static void Init(string[] readWriteHosts, Int32 maxWritePoolSize, string[] readOnlyHosts, Int32 maxReadPoolSize)
        {
            if (_redisClientsMan == null)
            {
                CreateManager(readWriteHosts, maxWritePoolSize, readOnlyHosts, maxReadPoolSize);
            }

        }
        #endregion

       #region Public Methods
        #region Item
        /// <summary>
        /// 设置单体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="t"></param>
        /// <param name="timeSpan"></param>
        /// <returns></returns>
        public static bool Item_Set<T>(string key, T t)
        {
            try
            {
                using (IRedisClient redis = _redisClientsMan.GetClient())
                {
                    return redis.Set<T>(key, t);
                }
            }
            catch (Exception ex)
            {
                // LogInfo
            }
            return false;
        }

        public static bool Item_Set<T>(string key, T t, TimeSpan expire)
        {
            try
            {
                using (IRedisClient redis = _redisClientsMan.GetClient())
                {
                    return redis.Set<T>(key, t, expire);
                }
            }
            catch (Exception ex)
            {
                // LogInfo
            }
            return false;
        }

        /// <summary>
        /// 获取单体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T Item_Get<T>(string key) where T : class
        {
            using (IRedisClient redis = _redisClientsMan.GetClient())
            {
                return redis.Get<T>(key);
            }
        }

        /// <summary>
        /// 移除单体
        /// </summary>
        /// <param name="key"></param>
        public static bool Item_Remove(string key)
        {
            using (IRedisClient redis = _redisClientsMan.GetClient())
            {
                return redis.Remove(key);
            }
        }

        #endregion

        #region List
        public static void List_Add<T>(string key, T t)
        {
            using (IRedisClient redis = _redisClientsMan.GetClient())
            {
                var redisTypedClient = redis.As<T>();
                redisTypedClient.AddItemToList(redisTypedClient.Lists[key], t);
            }
        }

        public static bool List_Remove<T>(string key, T t)
        {
            using (IRedisClient redis = _redisClientsMan.GetClient())
            {
                var redisTypedClient = redis.As<T>();
                return redisTypedClient.RemoveItemFromList(redisTypedClient.Lists[key], t) > 0;
            }
        }
        public static void List_RemoveAll<T>(string key)
        {
            using (IRedisClient redis = _redisClientsMan.GetClient())
            {
                var redisTypedClient = redis.As<T>();
                redisTypedClient.Lists[key].RemoveAll();
            }
        }

        public static Int64 List_Count(string listId)
        {
            using (IRedisClient redis = _redisClientsMan.GetClient())
            {
                return redis.GetListCount(listId);
            }
        }

        public static List<T> List_GetRange<T>(string key, int start, int count)
        {
            using (IRedisClient redis = _redisClientsMan.GetClient())
            {
                var c = redis.As<T>();
                return c.Lists[key].GetRange(start, start + count - 1);
            }
        }

        public static List<T> List_GetList<T>(string key)
        {
            using (IRedisClient redis = _redisClientsMan.GetClient())
            {
                var c = redis.As<T>();
                return c.Lists[key].GetRange(0, c.Lists[key].Count);
            }
        }

        public static List<T> List_GetList<T>(string key, int pageIndex, int pageSize)
        {
            int start = pageSize * (pageIndex - 1);
            return List_GetRange<T>(key, start, pageSize);
        }

        public static List<T> GetListTopByKey<T>(string key, int take)
        {
            using (var client = _redisClientsMan.GetClient())
            {
                var redis = client.As<T>().Lists[key];
                return redis.GetRange(redis.Count - take, redis.Count);
            }
        }

        public static List<T> GetListTopByConditionKey<T>(string key, Func<T, bool> fun, Func<T, DateTime> orderBy, int take)
        {
            using (var client = _redisClientsMan.GetClient())
            {
                return client.As<T>().Lists[key].Where(fun).OrderByDescending(orderBy).Take(take).ToList();

            }
        }

        public static List<T> GetListTopByKey<T>(string key)
        {
            using (var client = _redisClientsMan.GetClient())
            {
                return client.As<T>().Lists[key].GetAll();
            }
        }

        /// <summary>
        /// 设置缓存过期
        /// </summary>
        /// <param name="key"></param>
        /// <param name="datetime"></param>
        public static void List_SetExpire(string key, DateTime datetime)
        {
            using (IRedisClient redis = _redisClientsMan.GetClient())
            {
                redis.ExpireEntryAt(key, datetime);
            }
        }
        #endregion

        #region Set
        public static void Set_Add<T>(string key, T t)
        {
            using (IRedisClient redis = _redisClientsMan.GetClient())
            {
                var redisTypedClient = redis.As<T>();
                redisTypedClient.Sets[key].Add(t);
            }
        }
        public static bool Set_Contains<T>(string key, T t)
        {
            using (IRedisClient redis = _redisClientsMan.GetClient())
            {
                var redisTypedClient = redis.As<T>();
                return redisTypedClient.Sets[key].Contains(t);
            }
        }
        public static bool Set_Remove<T>(string key, T t)
        {
            using (IRedisClient redis = _redisClientsMan.GetClient())
            {
                var redisTypedClient = redis.As<T>();
                return redisTypedClient.Sets[key].Remove(t);
            }
        }

        #endregion

        #region SortedSet
        /// <summary>
        ///  添加数据到 SortedSet
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="t"></param>
        /// <param name="score"></param>
        public static bool SortedSet_Add<T>(string key, T t, double score)
        {
            using (IRedisClient redis = _redisClientsMan.GetClient())
            {
                string value = ServiceStack.Text.JsonSerializer.SerializeToString<T>(t);
                return redis.AddItemToSortedSet(key, value, score);
            }
        }

        /// <summary>
        /// 移除数据从SortedSet
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static bool SortedSet_Remove<T>(string key, T t)
        {
            using (IRedisClient redis = _redisClientsMan.GetClient())
            {
                string value = ServiceStack.Text.JsonSerializer.SerializeToString<T>(t);
                return redis.RemoveItemFromSortedSet(key, value);
            }
        }

        /// <summary>
        /// 修剪SortedSet
        /// </summary>
        /// <param name="key"></param>
        /// <param name="size">保留的条数</param>
        /// <returns></returns>
        public static Int64 SortedSet_Trim(string key, int size)
        {
            using (IRedisClient redis = _redisClientsMan.GetClient())
            {
                return redis.RemoveRangeFromSortedSet(key, size, 9999999);
            }
        }

        /// <summary>
        /// 获取SortedSet的长度
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static Int64 SortedSet_Count(string key)
        {
            using (IRedisClient redis = _redisClientsMan.GetClient())
            {
                return redis.GetSortedSetCount(key);
            }
        }

        /// <summary>
        /// 获取SortedSet的分页数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static List<T> SortedSet_GetList<T>(string key, int pageIndex, int pageSize)
        {
            using (IRedisClient redis = _redisClientsMan.GetClient())
            {
                var list = redis.GetRangeFromSortedSet(key, (pageIndex - 1) * pageSize, pageIndex * pageSize - 1);
                if (list != null && list.Count > 0)
                {
                    List<T> result = new List<T>();
                    foreach (var item in list)
                    {
                        var data = ServiceStack.Text.JsonSerializer.DeserializeFromString<T>(item);
                        result.Add(data);
                    }
                    return result;
                }
            }
            return null;
        }

        /// <summary>
        /// 获取SortedSet的全部数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static List<T> SortedSet_GetListALL<T>(string key)
        {
            using (IRedisClient redis = _redisClientsMan.GetClient())
            {
                var list = redis.GetRangeFromSortedSet(key, 0, 9999999);
                if (list != null && list.Count > 0)
                {
                    List<T> result = new List<T>();
                    foreach (var item in list)
                    {
                        var data = ServiceStack.Text.JsonSerializer.DeserializeFromString<T>(item);
                        result.Add(data);
                    }
                    return result;
                }
            }
            return null;
        }

        /// <summary>
        /// 设置缓存过期
        /// </summary>
        /// <param name="key"></param>
        /// <param name="datetime"></param>
        public static void SortedSet_SetExpire(string key, DateTime datetime)
        {
            using (IRedisClient redis = _redisClientsMan.GetClient())
            {
                redis.ExpireEntryAt(key, datetime);
            }
        }

        public static double SortedSet_GetItemScore<T>(string key, T t)
        {
            using (IRedisClient redis = _redisClientsMan.GetClient())
            {
                var data = ServiceStack.Text.JsonSerializer.SerializeToString<T>(t);
                return redis.GetItemScoreInSortedSet(key, data);
            }

            return 0;
        }

        #endregion

        #region Hash
        /// <summary>
        /// 判断某个数据是否已经被缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        public static bool Hash_Exist<T>(string key, string dataKey)
        {
            using (IRedisClient redis = _redisClientsMan.GetClient())
            {
                return redis.HashContainsEntry(key, dataKey);
            }
        }

        /// <summary>
        /// 存储数据到hash表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        public static bool Hash_Set<T>(string key, string dataKey, T t)
        {
            using (IRedisClient redis = _redisClientsMan.GetClient())
            {
                string value = ServiceStack.Text.JsonSerializer.SerializeToString<T>(t);
                return redis.SetEntryInHash(key, dataKey, value);
            }
        }

        /// <summary>
        /// 移除hash中的某值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        public static bool Hash_Remove<T>(string key, string dataKey)
        {
            using (IRedisClient redis = _redisClientsMan.GetClient())
            {
                return redis.As<T>().GetHash<string>(key).Remove(dataKey);

            }
        }

        /// <summary>
        /// 移除整个hash
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        public static bool Hash_Remove(string key)
        {
            using (IRedisClient redis = _redisClientsMan.GetClient())
            {
                return redis.Remove(key);
            }
        }

        /// <summary>
        /// 从hash表获取数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        public static T Hash_Get<T>(string key, string dataKey)
        {
            using (IRedisClient redis = _redisClientsMan.GetClient())
            {
                string value = redis.GetValueFromHash(key, dataKey);
                return ServiceStack.Text.JsonSerializer.DeserializeFromString<T>(value);
            }
        }

        /// <summary>
        /// 获取整个hash的数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public static List<T> Hash_GetAll<T>(string key)
        {
            using (IRedisClient redis = _redisClientsMan.GetClient())
            {
                var list = redis.GetHashValues(key);
                if (list != null && list.Count > 0)
                {
                    List<T> result = new List<T>();
                    foreach (var item in list)
                    {
                        var value = ServiceStack.Text.JsonSerializer.DeserializeFromString<T>(item);
                        result.Add(value);
                    }
                    return result;
                }
                return null;
            }
        }

        /// <summary> 
        /// 获取Hash集合数量 
        /// </summary> 
        /// <param name="key">Hashid</param> 
        public static Int64 Hash_GetCount(string key)
        {
            using (IRedisClient redis = _redisClientsMan.GetReadOnlyClient())
            {
                return redis.GetHashCount(key);
            }
        }

        /// <summary>
        /// 设置缓存过期
        /// </summary>
        /// <param name="key"></param>
        /// <param name="datetime"></param>
        public static void Hash_SetExpire(string key, DateTime datetime)
        {
            using (IRedisClient redis = _redisClientsMan.GetClient())
            {
                redis.ExpireEntryAt(key, datetime);
            }
        }

       
        #endregion
        #endregion

    }
}