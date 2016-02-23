using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Online.DbHelper.BLL;
using Online.DbHelper.Model;

namespace Online.DbHelper.Common
{
    public delegate void NotifyErrorMessageHandle(LogMessage message);
    public class LogHelper : IDisposable
    {
        #region Private Variable

        //Log Message queue
        private static Queue<LogMessage> _logMessages;
        //log write file state
        private static bool _state;

        /// <summary>
        /// Wait enqueue wirte log message semaphore will release
        /// </summary>
        private Semaphore _semaphore;

        /// <summary>
        /// Single instance
        /// </summary>
        private static LogHelper _log;

        private object _lockObject;

        private DataContextBll _dataSource;

        #endregion

        #region Log Help


        #endregion

        #region Private Property

        #endregion

        #region Construct Method

        /// <summary>
        /// Create a log instance
        /// </summary>
        private LogHelper()
        {
            Initialize();
        }

        #endregion

        #region Public Property


        /// <summary>
        /// Gets a single instance
        /// </summary>
        public static LogHelper Instance
        {
            get
            {
                if (_log == null) _log = new LogHelper();
                return _log;
            }
        }

        #endregion

        #region Private Method


        /// <summary>
        /// Initialize Log instance
        /// </summary>
        private void Initialize()
        {
            _state = true;

            _dataSource = new DataContextBll();
            _lockObject = new object();
            _semaphore = new Semaphore(0, int.MaxValue);
            _logMessages = new Queue<LogMessage>();
            var thread = new Thread(Work) { IsBackground = true };
            thread.Start();
        }

        /// <summary>
        /// Write Log file  work method
        /// </summary>
        private void Work()
        {
            while (true)
            {
                if (_logMessages.Count > 0)
                {
                    SaveLogMessage();
                }
                else
                    if (WaitLogMessage()) break;
            }
        }

        /// <summary>
        /// Write message to log file
        /// </summary>
        private void SaveLogMessage()
        {
            LogMessage logMessage = null;
            lock (_lockObject)
            {
                if (_logMessages.Count > 0)
                    logMessage = _logMessages.Dequeue();
            }
            if (logMessage != null)
            {
                SaveMessage(logMessage);
            }
        }


        /// <summary>
        /// The thread wait a log message
        /// </summary>
        /// <returns>is close or not</returns>
        private bool WaitLogMessage()
        {
            //determine log life time is true or false
            if (_state)
            {
                WaitHandle.WaitAny(new WaitHandle[] { _semaphore }, -1, false);
                return false;
            }

            return true;
        }


        private void InitDataSource()
        {
            if (_dataSource == null)
                _dataSource = new DataContextBll();
        }

        /// <summary>
        /// Write log file message
        /// </summary>
        /// <param name="msg"></param>
        private void SaveMessage(LogMessage msg)
        {
            try
            {
                InitDataSource();
                msg.Id = Guid.NewGuid();
                _dataSource.LogMessages.AddOrUpdate(msg);
                _dataSource.SaveChanges();
            }
            catch (Exception ex)
            {
                Trace.Write(ex.Message + ex.StackTrace);
                Console.Out.Write(ex);
            }
        }


        public event NotifyErrorMessageHandle NotifyErrorMessage;

        protected virtual void OnNotifyErrorMessage(LogMessage message)
        {
            NotifyErrorMessageHandle handler = NotifyErrorMessage;
            if (handler != null) handler(message);
        }




        #endregion

        #region Public Mehtod

        /// <summary>
        /// Enqueue a new log message and release a semaphore
        /// </summary>
        /// <param name="msg">Log message</param>
        public void Write(LogMessage msg)
        {
            if (msg != null)
            {
                lock (_lockObject)
                {
                    _logMessages.Enqueue(msg);
                    _semaphore.Release();
                }
            }
        }

        /// <summary>
        /// Write message by message content and type
        /// </summary>
        /// <param name="text">log message</param>
        /// <param name="messageType">message type</param>
        /// <param name="classType">class Type </param>
        /// <param name="methodName">method Name </param>
        public void Write(string text, LogMessageType messageType, Type classType, string methodName)
        {
            Write(new LogMessage(text, messageType.ToString(), classType, methodName));
        }

        public void WriteError(string text, Type classType, string methodName)
        {
            Write(new LogMessage(text, LogMessageType.Error.ToString(), classType, methodName));
        }

        /// <summary>
        /// Write message
        /// </summary>
        /// <param name="text">message texte</param>

        public void WriteInformation(string text)
        {
            Write(new LogMessage(text, LogMessageType.Information.ToString(), null, null));
        }


        /// <summary>
        /// Write message ty exception and message type 
        /// </summary>
        /// <param name="e">exception</param>
        /// <param name="messageType">message type</param>
        /// <param name="classType">classType </param>
        /// <param name="methodName"> method Name</param>
        public void Write(Exception e, LogMessageType messageType, Type classType, string methodName)
        {
            Write(new LogMessage(e.Message + e.StackTrace, messageType.ToString(), classType, methodName));
        }

        public void WriteError(Exception e, Type classType, string methodName)
        {
            Write(new LogMessage(e.Message + e.StackTrace, LogMessageType.Error.ToString(), classType, methodName));
        }

        #region IDisposable member


        /// <summary>
        /// Dispose log
        /// </summary>
        public void Dispose()
        {
            _state = false;
        }

        #endregion

        #endregion

        #region Public Event

        #endregion

    }




    /// <summary>
    /// Log Message Type enum
    /// </summary>
    public enum LogMessageType
    {
        /// <summary>
        /// unknown type 
        /// </summary>
        Unknown,

        /// <summary>
        /// information type
        /// </summary>
        Information,

        /// <summary>
        /// User operation type
        /// </summary>
        UserOperation,

        /// <summary>
        /// warning type
        /// </summary>
        Warning,

        /// <summary>
        /// error type
        /// </summary>
        Error,

        /// <summary>
        /// Process runing
        /// </summary>
        Runing,

        /// <summary>
        /// success type
        /// </summary>
        Success,

        /// <summary>
        /// Areas type
        /// </summary>
        Admin
    }
}
