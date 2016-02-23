using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Online.DbHelper.Model
{
   public class LogMessage
    {

        /// <summary>
        /// Create Log message instance
        /// </summary>
        public LogMessage()
            : this("", "Unknown", null, null)
        {
        }

        public LogMessage(string text, string messageType, Type classType, string methodName)
        {
            CreateDateTime = DateTime.Now;
            MessageType = messageType;
            Text = text;
            ClassType = classType == null ? "" : classType.FullName;
            MethodName = methodName;
        }

        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets method name
        /// </summary>
        public string MethodName { get; set; }

        /// <summary>
        /// Gets or sets datetime
        /// </summary>
        public DateTime? CreateDateTime { get; set; }

        /// <summary>
        /// Gets or sets message content
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets message type
        /// </summary>
        public string MessageType { get; set; }


        public string ClassType { get; set; }
    }
}
