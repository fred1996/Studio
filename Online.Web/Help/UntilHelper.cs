using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Online.DbHelper.Common;

namespace Online.Web.Help
{
    public static  class UntilHelper
    {
        public static string GetMd5HashCode(string input)
        {
            string result = string.Empty;
            MD5 md5 = MD5.Create();//实例化一个md5对像
            // 加密后是一个字节类型的数组，这里要注意编码UTF8/Unicode等的选择　
            byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
            // 通过使用循环，将字节类型的数组转换为字符串，此字符串是常规字符格式化所得
            for (int i = 0; i < s.Length; i++)
            {
                // 将得到的字符串使用十六进制类型格式。格式后的字符是小写的字母，如果使用大写（X）则格式后的字符是大写字符 
                result = result + s[i].ToString("x2");
            }
            return result;
        }

        public static string CreateRandomCode(int length)
        {
      
            char code;
            string randomcode = String.Empty;
            char[] lettersAndNumbersChar =  {
            '2','3','4','5','6','7','8','9'};
            //生成一定长度的验证码
            Random random = new Random();
            for (int i = 0; i < length; i++)
            {
                code = lettersAndNumbersChar[random.Next(lettersAndNumbersChar.Length)];
                randomcode += code.ToString();
            }
            return randomcode;
        }

        public static  byte[] CreateImage(string randomcode)
        {
            Random rand = new Random();
            int randAngle = rand.Next(15, 45);
            int mapwidth = (int)(randomcode.Length * 22);
            using (Bitmap map = new Bitmap(mapwidth, 35))//创建图片背景
            {
                using (Graphics graph = Graphics.FromImage(map))
                {
                    graph.Clear(Color.AliceBlue);//清除画面，填充背景

                    graph.DrawRectangle(new Pen(Color.Black, 0), 0, 0, map.Width - 1, map.Height - 1);//画一个边框
                                                                                                      //验证码旋转，防止机器识别
                    char[] chars = randomcode.ToCharArray();//拆散字符串成单字符数组

                    //文字距中
                    StringFormat format = new StringFormat(StringFormatFlags.NoWrap);
                    format.Alignment = StringAlignment.Center;
                    format.LineAlignment = StringAlignment.Center;
                    //定义颜色
                    Color[] c = { Color.Black, Color.Red, Color.DarkBlue, Color.Green, Color.Orange, Color.Brown, Color.DarkCyan, Color.Purple };
                    //噪点线
                    for (int i = 0; i < 4; i++)
                    {
                        int x1 = rand.Next(map.Width);
                        int x2 = rand.Next(map.Width);
                        int y1 = rand.Next(map.Height);
                        int y2 = rand.Next(map.Height);
                        float dd2 = Convert.ToSingle(0.1);
                        float dd = Convert.ToSingle(dd2 * Convert.ToSingle(rand.Next(10, 17)));
                        graph.DrawLine(new Pen(c[rand.Next(7)], dd), x1, y1, x2, y2);//根据坐标画线        
                    }
                    //定义字体
                    string[] font = { "Verdana", "Microsoft Sans Serif", "Comic Sans MS", "Arial", "宋体", "隶书" };
                    for (int i = 0; i < chars.Length; i++)
                    {
                        int cindex = rand.Next(7);
                        int findex = rand.Next(5);
                        Font f = new Font(font[findex], 17, FontStyle.Bold);//字体样式(参数2为字体大小)
                        Brush b = new SolidBrush(c[cindex]);
                        Point dot = new Point(16, 16);
                        if (i == 0)
                        {
                            dot = new Point(rand.Next(10, 16), 16);
                        }
                        float angle = rand.Next(-randAngle, randAngle);//转动的度数
                        graph.TranslateTransform(dot.X, dot.Y);//移动光标到指定位置
                        graph.RotateTransform(angle);
                        graph.DrawString(chars[i].ToString(), f, b, 1, 1, format);
                        graph.RotateTransform(-angle);//转回去
                        graph.TranslateTransform(rand.Next(1, 9), -dot.Y + rand.Next(-3, 3));//移动光标到指定位置
                    }
                    //生成图片
                    MemoryStream ms = new MemoryStream();
                    map.Save(ms, ImageFormat.Gif);
                    return ms.ToArray();
                }
            }
        }


        public static int RoomId= Convert.ToInt32(ConfigurationManager.AppSettings["RoomId"]);


        public static int ToInt(this long value)
        {
            return Convert.ToInt32(value);
        }

        public static bool SendSms(string phone, string message)
        {
            try
            {
                if (string.IsNullOrEmpty(phone) || string.IsNullOrEmpty(message)) return false;
                var postUrl = ConfigurationManager.AppSettings["PostUrl"];
                string account = ConfigurationManager.AppSettings["SMSsend-Account"];
                string password = ConfigurationManager.AppSettings["SMSsend-Password"];
                string smsContent = "account=" + account + "&password=" + password + "&mobile=" + phone + "&content=" + message;
                if (!string.IsNullOrEmpty(phone))
                {
                    UTF8Encoding encoding = new UTF8Encoding();
                    byte[] postData = encoding.GetBytes(smsContent);
                    HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(postUrl);
                    myRequest.Method = "POST";
                    myRequest.ContentType = "application/x-www-form-urlencoded;charset=UTF-8";
                    myRequest.ContentLength = postData.Length;
                    Stream newStream = myRequest.GetRequestStream();
                    newStream.Write(postData, 0, postData.Length);
                    newStream.Flush();
                    newStream.Close();
                    HttpWebResponse myResponse = (HttpWebResponse)myRequest.GetResponse();
                    if (myResponse.StatusCode == HttpStatusCode.OK)
                    {
                        StreamReader reader = new StreamReader(myResponse.GetResponseStream(), Encoding.UTF8);
                        var result = reader.ReadToEnd();
                        if (result == "100")
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteError(ex, typeof(UntilHelper), MethodBase.GetCurrentMethod().Name);
                throw;
            }
        }
        private static string PICTURE_FILE = "[.gif.png.jpeg.jpg.bmp]";
        public static bool IsUploadImage(string fileExtension)
        {
            bool isImage = PICTURE_FILE.IndexOf(fileExtension, StringComparison.OrdinalIgnoreCase) > 0;
            return isImage;
        }
        public static string Token = Guid.NewGuid().ToString().Replace("-", "");
        /// <summary>
        /// 判断文件是否为图片
        /// </summary>
        /// <param name="path">文件的完整路径</param>
        /// <returns>返回结果</returns>
        public static bool IsImage(HttpPostedFileBase file)
        {
            try
            {
                Image img = Image.FromStream(file.InputStream);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}