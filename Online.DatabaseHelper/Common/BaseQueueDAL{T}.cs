using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Online.DbHelper.Common
{
    public abstract class BaseQueueDAL<T> : IDisposable where T : new()
    {
        /// <summary>
        /// Wait enqueue write log message semaphore will release
        /// </summary>
        private readonly Semaphore _semaphore;

        public void Dispose()
        {
            IsDispose = true;
        }

        public BaseQueueDAL()
        {
            _messageList = new ConcurrentQueue<T>();
            _semaphore = new Semaphore(0, int.MaxValue);
            var thread = new Thread(Work)
            {
                IsBackground = true,
                Priority = ThreadPriority.Highest
            };
            thread.Start();
        }

        private bool IsDispose { get; set; }

        private void Work(object obj)
        {
            while (true)
            {
                if (IsDispose) break;
                try
                {
                    var entity = Get();
                    if (entity == null)
                        WaitHandle.WaitAny(new WaitHandle[]
                        {
                            _semaphore
                        }, 10000, false);
                    else
                    {
                        OnNotify(entity);
                    }
                    entity = default(T);
                }
                catch (Exception ex)
                {
                    LogHelper.Instance.WriteError(ex, GetType(), MethodBase.GetCurrentMethod().Name);
                }
            }
        }

        private readonly ConcurrentQueue<T> _messageList;

        private readonly object _syncMessage = new object();

        public void Add(T entity)
        {
            if (entity == null)
                return;
            try
            {
                lock (_syncMessage)
                {
                    _messageList.Enqueue(entity);
                    _semaphore.Release();
                }
            }
            catch (Exception ex)
            {

            }
        }

        //public void Add(List<T> entityList)
        //{
        //    entityList.ForEach(Add);
        //}

        public abstract void OnNotify(T entity);

        public T Get()
        {
            T entity;
            _messageList.TryDequeue(out entity);
            return entity;
        }
    }
}
