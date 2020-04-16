using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Printer
{
    public static class Common
    {
        #region SystemConfig

        public static SystemConfig SystemConfig;

        public static void GetDeviceConfigFromFile(string filepath)
        {
            SystemConfig = Deserialize<SystemConfig>(filepath);
        }

        public static void GetSystemConfigFromXmlFile()
        {
            SystemConfig = Deserialize<SystemConfig>(GetConfigFilePath());
        }

        public static void GetSerialNoFromXmlFile()
        {

        }

        public static void SaveConfigToFile(object config, string filepath)
        {
            SerializeToFile(config, filepath, Encoding.UTF8);
        }
        public static void SaveConfigToFile(object config)
        {
            SerializeToFile(config, GetConfigFilePath(), Encoding.UTF8);
        }

        public static T Deserialize<T>(string filePath)
        {
            var type = typeof(T);

            object targetObj;

            var serializer = new XmlSerializer(type);

            using (var fs = new FileStream(filePath, FileMode.Open))
            {
                using (var reader = XmlReader.Create(fs))
                {
                    targetObj =
                        (T)serializer.Deserialize(reader);
                }
            }
            return (T)targetObj;
        }
        private static void SerializeToFile(object o, string path, Encoding encoding)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            using (FileStream file = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                XmlSerializeInternal(file, o, encoding);
            }
        }
        private static void XmlSerializeInternal(Stream stream, object o, Encoding encoding)
        {
            if (o == null)
                throw new ArgumentNullException("o");
            if (encoding == null)
                throw new ArgumentNullException("encoding");

            XmlSerializer serializer = new XmlSerializer(o.GetType());

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.NewLineChars = "\r\n";
            settings.Encoding = encoding;
            settings.IndentChars = "    ";

            using (XmlWriter writer = XmlWriter.Create(stream, settings))
            {
                serializer.Serialize(writer, o);
                writer.Close();
            }
        }

        public static string GetConfigFilePath()
        {
            string path = Directory.GetCurrentDirectory();
            DirectoryInfo info = new DirectoryInfo(path);
            Debug.Assert(info.Parent != null, "info.Parent != null");
            Debug.Assert(info.Parent.Parent != null, "info.Parent.Parent != null");
            return info.Parent.Parent.FullName + @"\SystemConfig.xml";
            //  _filePath = info.Parent.FullName + @"\XML\DeviceConfig.xml";
       
        }

        public static string GetSerialConfigFilePath()
        {
            string path = Directory.GetCurrentDirectory();
            DirectoryInfo info = new DirectoryInfo(path);
            Debug.Assert(info.Parent != null, "info.Parent != null");
            Debug.Assert(info.Parent.Parent != null, "info.Parent.Parent != null");
            return info.Parent.Parent.FullName + @"\serialNo.xml";
            //  _filePath = info.Parent.FullName + @"\XML\DeviceConfig.xml";

        }


        #endregion

        //#region UserControlsDll

        //public static List<string> LstUserControlNames = new List<string>();
        //public static void GetLstUserControls()
        //{
        //    var types = Assembly.LoadFrom("UserControls.dll").GetTypes().ToList().FindAll(t => t.BaseType != null && t.BaseType.Name == "UserControl");

        //    LstUserControlNames = types.Select(t => t.Name).ToList();
        //}


        //#endregion




        public static string GetAppPathFullName()
        {
            var path = Directory.GetCurrentDirectory();
            DirectoryInfo info = new DirectoryInfo(path);
            Debug.Assert(info.Parent != null, "info.Parent != null");
            Debug.Assert(info.Parent.Parent != null, "info.Parent.Parent != null");
            return info.Parent.Parent.FullName;
        }

        public static void BackupFile(string sourcefilepath, string destinationfilepath, bool isrewrite)
        {
            // true=覆盖已存在的同名文件,false则反之
            File.Copy(sourcefilepath, destinationfilepath, isrewrite);
        }

        public static void BackupFile()
        {
            //File.Copy(GetConfigFilePath(), @"D:\BackupFile\"+DateTime.Now.ToString("yyyyMMddHHmmss")+".xml", true);
        }

    

        #region 字符串处理




        /// <summary>
        /// 整理字符串,移除空格、回车换行、换行
        /// </summary>
        /// <param name="str">待整理的字符串</param>
        /// <returns>整理后的字符串,移除空格、回车换行、换行</returns>
        public static string StrTrim(this string str)
        {
            if (str == null)
                return "";
            var strTemp = str.Replace(" ", "");  //取出空格
            strTemp = strTemp.Replace("\r\n", "");//取出回车换行
            strTemp = strTemp.Replace("\n", "");//取出换行

            return strTemp;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetStrFromLeftSingleEqualitySign(this string value)
        {
            const string singleEqualitySign = "=";

            if (!value.Contains(singleEqualitySign))
                return string.Empty;
            else
            {
                var result = value.Substring(0, value.IndexOf(singleEqualitySign, StringComparison.Ordinal));
                return result;
            }
        }

        public static string GetStrFromRightSingleEqualitySign(this string value)
        {
            const string singleEqualitySign = "=";

            if (!value.Contains(singleEqualitySign))
                return string.Empty;
            else
            {
                var result = value.Substring(value.IndexOf(singleEqualitySign, StringComparison.Ordinal) + 1,
                    value.Length - value.IndexOf(singleEqualitySign, StringComparison.Ordinal) - 1);
                return result.Trim(';');
            }
        }

        public static string GetStrFromLeftSign(this string value)
        {
            const string doubleEqualitySign = "==";
            const string unequalSign = "!=";
            const string greatthanOrequaltoSign = ">=";
            const string lessthanOrequaltoSign = "<=";
            const string greatthanSign = ">";
            const string lessthanSign = "<";

            var func = new Func<string, string>(sign =>
            {
                var result = value.Substring(0, value.IndexOf(sign, StringComparison.Ordinal));
                return result;
            });

            if (!value.Contains(doubleEqualitySign) && !value.Contains(unequalSign)
                && !value.Contains(greatthanOrequaltoSign) && !value.Contains(lessthanOrequaltoSign)
                && !value.Contains(greatthanSign) && !value.Contains(lessthanSign))
                return string.Empty;
            else if (value.Contains(doubleEqualitySign))
                return func.Invoke(doubleEqualitySign);
            else if (value.Contains(unequalSign))
                return func.Invoke(unequalSign);
            else if (value.Contains(greatthanOrequaltoSign))
                return func.Invoke(greatthanOrequaltoSign);
            else if (value.Contains(lessthanOrequaltoSign))
                return func.Invoke(lessthanOrequaltoSign);
            else if (value.Contains(greatthanSign))
                return func.Invoke(greatthanSign);
            else if (value.Contains(lessthanSign))
                return func.Invoke(lessthanSign);

            return string.Empty;
        }

        public static string GetStrFromRightSign(this string value)
        {
            const string equalitySign = "==";
            const string unequalSign = "!=";
            const string greatthanOrequaltoSign = ">=";
            const string lessthanOrequaltoSign = "<=";
            const string greatthanSign = ">";
            const string lessthanSign = "<";

            var func = new Func<string, string>(sign =>
            {
                var result = value.Substring(value.IndexOf(sign, StringComparison.Ordinal) + 2,
                     value.Length - value.IndexOf(sign, StringComparison.Ordinal) - 2);
                return result.Trim(';').Trim('|').Trim('&');
            });

            if (!value.Contains(equalitySign) && !value.Contains(unequalSign) && !value.Contains(greatthanOrequaltoSign) &&
                !value.Contains(lessthanOrequaltoSign)
                && !value.Contains(greatthanSign) && !value.Contains(lessthanSign))
                return string.Empty;
            else if (value.Contains(equalitySign))
                return func.Invoke(equalitySign);
            else if (value.Contains(unequalSign))
                return func.Invoke(unequalSign);
            else if (value.Contains(greatthanOrequaltoSign))
                return func.Invoke(greatthanOrequaltoSign);
            else if (value.Contains(lessthanOrequaltoSign))
                return func.Invoke(lessthanOrequaltoSign);
            else if (value.Contains(greatthanSign))
                return func.Invoke(greatthanSign);
            else if (value.Contains(lessthanSign))
                return func.Invoke(lessthanSign);

            return string.Empty;
        }

        public static string[] GetStrsSplitByDot(this string value)
        {
            var splitByDot = value.Split('.');
            return splitByDot.Length != 3 ? null : splitByDot;
        }

        #endregion

    }
}
