using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace YTTimeRentWeb
{
    public class logHelper
    {
        static string logname = "YTimeRentLogSSSSS";
        static string logaddress = "D:\\";
        static bool isLock = false;
        // string rootPath = System.Web.HttpContext.Current.Server.MapPath("~\\");
        string logFile = "";
       

        /// 日志
        /// <param name="logname">日志名称</param>
        /// <param name="logaddress">日志保存地址</param>
        /// <param name="message">日志内容</param>
        public static void WriteLog(string message)
        {
            try
            {

                if (isLock) { return; }
                isLock = true;
                //string path = AppDomain.CurrentDomain.BaseDirectory + @"System\Log\";
                string path = logaddress;
                string fileFullPath = path + logname + ".txt";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                if (!File.Exists(fileFullPath))
                {
                    File.Create(fileFullPath);
                }
                listen(fileFullPath);
                StringBuilder str = new StringBuilder();
                str.Append("Time：" + DateTime.Now.ToString() + "\r\n");
                str.Append("Msg ：" + message + "\r\n");
                str.Append("----------------------------------------------------------");
                StreamWriter sw;
                if (!File.Exists(fileFullPath))
                {
                    sw = File.AppendText(fileFullPath);
                }
                else
                {
                    sw = File.AppendText(fileFullPath);
                }
                sw.WriteLine(str.ToString());
                sw.Close();
                isLock = false;
            }
            catch (System.Exception ex)
            {
                return;
            }
        }

        public static void listen(string path)
        {
            FileInfo f = new FileInfo(path);
            int startindex = path.LastIndexOf("\\") + 2;
            int endindex = path.LastIndexOf(".");
            int len = endindex - startindex;
            string filename = path.Substring(startindex, len);
            if (f.Length >= 102400)
            {
                //超出了100KB大小，则自动备份并清除当前文本
                string time = DateTime.Now.ToString("yyyyMMddHHssmm");
                time = time.Replace(" ", "");
                string srcfile = path;
                string desfile = "D:\\Log\\TimeRent\\" + filename + "_" + time + ".txt";
                CreateDirector("D:\\Log");
                CreateDirector("D:\\Log\\TimeRent");
                System.IO.File.Copy(srcfile, desfile, true);
                StreamWriter sw = new StreamWriter(srcfile, false);
                sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:ss:mm") + "备份并清除内容");
                sw.Close();
                sw.Dispose();
            }
        }
        public static void CreateDirector(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

        }
    }
}