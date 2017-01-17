using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Text;

    /// <summary>
    /// Log 的摘要说明
    /// </summary>
    public class Log
    {
        public Log()
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
        }

        /// <summary>
        /// 日志
        /// </summary>
        /// <param name="logname">日志名称</param>
        /// <param name="logaddress">日志保存地址</param>
        /// <param name="message">日志内容</param>
        public static void WriteLog(string logname, string logaddress, string message)
        {
            //string path = AppDomain.CurrentDomain.BaseDirectory + @"System\Log\";
            string path = logaddress;
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            //string fileFullPath = path + time.ToString("yyyy-MM-dd") + ".System.txt";
            string fileFullPath = path + logname + ".txt";
            StringBuilder str = new StringBuilder();
            str.Append("Time: " + DateTime.Now.ToString());
            str.Append("      Message: " + message);
            //str.Append("Time:    " + time.ToString() + "\r\n");
            //str.Append("Action:  " + code + "\r\n");
            //str.Append("Message: " + message + "\r\n");
            //str.Append("-----------------------------------------------------------\r\n\r\n");
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
        }

    }
