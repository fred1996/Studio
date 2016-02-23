using System.Configuration;
using System.Net.Mail;
using System.Text;
using ServiceStack.Logging;


namespace Online.Web.Help
{
    /// <summary>
    /// 发送邮件类
    /// </summary>
    public static class MailHelp
    {
        static ILog log = LogManager.GetLogger(typeof(MailHelp));

        /// <summary>
        /// 找回密码发送邮件
        /// </summary>
        /// <param name="mailTo"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static bool FindPassword(string mailTo, string password)
        {
            string content = mailTo + ", 您好！<br>" +
                            "您的登录密码已重置为：<br>" +
                            "<p style='font-size:30px;color:red'>" + password + " </p>";
            bool flag = SendMail(mailTo, "融藏邮币卡重置登录密码", content, true);
            return flag;
        }

        public static bool SendEmailCode(string mailTo, string code)
        {
            string content = mailTo + ", 您好！<br>" +
                           "您的验证码为：<br>" +
                           "<p style='font-size:30px;color:red'>" + code + " </p>";
            bool flag = SendMail(mailTo, "验证码", content, true);
            return flag;
        }

        public static bool SendMail(string mailTo, string mailSubject, string mailBody, bool isHtml)
        {
            string mailFrom = ConfigurationManager.AppSettings["Email-Address"];
            string mailFromName = ConfigurationManager.AppSettings["Email-NickName"];
            string mailPwd = ConfigurationManager.AppSettings["Email-Password"];
            string mailSmtpHost = ConfigurationManager.AppSettings["Email-SmtpHost"];
            int mailSmtpPort = int.Parse(ConfigurationManager.AppSettings["Email-SmtpPort"]);

            SmtpClient _smtpClient = new SmtpClient();
            _smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;//指定电子邮件发送方式
            _smtpClient.Host = mailSmtpHost; ;//指定SMTP服务器
            _smtpClient.Credentials = new System.Net.NetworkCredential(mailFrom, mailPwd);//用户名和密码

            MailMessage _mailMessage = new MailMessage(mailFrom, mailTo);
            _mailMessage.Subject = mailSubject;//主题
            _mailMessage.Body = mailBody;//内容
            _mailMessage.BodyEncoding = Encoding.UTF8;//正文编码
            _mailMessage.IsBodyHtml = true;//设置为HTML格式
            _mailMessage.Priority = MailPriority.High;//优先级

            try
            {
                _smtpClient.Send(_mailMessage);
                return true;
            }
            catch
            {
                return false;
            }
            return true;
        }

        static bool sendMail(string to, string title, string content)
        {
            string mailFrom = ConfigurationManager.AppSettings["Email-Address"];
            string mailFromName = ConfigurationManager.AppSettings["Email-NickName"];
            string mailPwd = ConfigurationManager.AppSettings["Email-Password"];
            string mailSmtpHost = ConfigurationManager.AppSettings["Email-SmtpHost"];
            int mailSmtpPort = int.Parse(ConfigurationManager.AppSettings["Email-SmtpPort"]);

            SmtpClient _smtpClient = new SmtpClient();
            _smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;//指定电子邮件发送方式
            _smtpClient.Host = mailSmtpHost; ;//指定SMTP服务器
            _smtpClient.Credentials = new System.Net.NetworkCredential(mailFromName, mailPwd);//用户名和密码

            MailMessage _mailMessage = new MailMessage(mailFrom, to);
            _mailMessage.Subject = title;//主题
            _mailMessage.Body = content;//内容
            _mailMessage.BodyEncoding = System.Text.Encoding.UTF8;//正文编码
            _mailMessage.IsBodyHtml = true;//设置为HTML格式
            _mailMessage.Priority = MailPriority.High;//优先级

            try
            {
                _smtpClient.Send(_mailMessage);
                return true;
            }
            catch
            {
                return false;
            }
        }



    }
}
