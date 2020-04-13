using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Printer
{
    public class TXTLogHelper
    {
        /// <summary>
        /// 对某些操作进行TXT日志记录
        /// </summary>
        public static void LogBackup(string LogString)
        {
            //处理logstring,添加日期和换行
            //LogString = LogString;
            LogString += "\r\n";

            string logFolder = GetOrCreateLogFilePath();
            string logFile = GetBackupLogFileName();
            
            FileInfo file = new FileInfo(logFile);
            //FileStream fs = file.Open(FileMode.Append, FileAccess.Write);
            //byte[] bytes = Encoding.UTF8.GetBytes(LogString);
            //fs.Write(bytes, 0, bytes.Length);
            //fs.Flush();
            //fs.Close();
            //fs.Dispose();
            using (FileStream fs = file.Open(FileMode.Append, FileAccess.Write))
            {
                byte[] bytes = Encoding.UTF8.GetBytes(LogString);
                fs.Write(bytes, 0, bytes.Length);
                fs.Flush();
                fs.Close();
                fs.Dispose();
            }
        }

        //获取目录路径，如果不存在则创建
        private static string GetOrCreateLogFilePath()
        {
            string backupFolder = System.Environment.CurrentDirectory + "\\log";
            if (!Directory.Exists(backupFolder))
                Directory.CreateDirectory(backupFolder);
            return backupFolder;
        }

        private static string GetBackupLogFileName()
        {
            //为了防止数据量过大，按照日期每天生成一个日志文件
            string logFileId = DateTime.Now.ToString("yyyy-MM-dd");
            return GetOrCreateLogFilePath() + "\\" + logFileId + ".txt";
        }
    }
}
