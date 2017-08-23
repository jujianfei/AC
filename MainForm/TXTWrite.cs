using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MainForm
{
    public class TXTWrite
    {
        public enum TXTErrType
        {

        }
        private static string BasePath = System.Windows.Forms.Application.StartupPath;
        public static void WriteERRTxt(string cls, string fun, string info, string data, string errstr)
        {
            DateTime timenow = DateTime.Now;
            string filePath = BasePath + "\\ERR\\";

            #region 检查文件夹           
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }
            #endregion

            #region 保存
            string filePathPP = filePath + string.Format("{0}.txt", timenow.ToString("yyyy-MM-dd"));
            //int beginlen = 0;
            //FileStream fs = new FileStream(filePathPP, FileMode.OpenOrCreate, FileAccess.Write);
            //BinaryWriter bw = new BinaryWriter(fs);
            //bw.BaseStream.Seek(0, SeekOrigin.End); // 数据流指针移到最后

            //bw.Write(string.Format("[{0}]：[Class：{1}],[Function：{2}],[Info：{3}],[Data：{4}],[Err：{5}]", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"), cls, fun, info, data, errstr));
            //bw.Write("\r\n");
            //if (fs.Length == beginlen)
            //{
            //    bw.Close();  // 关闭文件流                 
            //    fs.Close();
            //    File.Delete(filePathPP);
            //}
            //else
            //{
            //    bw.Close();  // 关闭文件流                 
            //    fs.Close();
            //}
            StreamWriter sw = File.AppendText(filePathPP);
            sw.WriteLine(string.Format("[{0}]：[Class：{1}],[Function：{2}],[Info：{3}],[Data：{4}],[Err：{5}]", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), cls, fun, info, data, errstr));
            sw.Flush();
            sw.Close();
            #endregion
        }
        public static void WriteInfo(string info)
        {
            DateTime timenow = DateTime.Now;
            string filePath = BasePath + "\\LOG\\";
            
            #region 检查文件夹
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }
            #endregion
            
            #region 保存
            string filePathPP = filePath + string.Format("{0}.txt", timenow.ToString("yyyy-MM-dd"));
            //int beginlen = 0;
            //FileStream fs = new FileStream(filePathPP, FileMode.OpenOrCreate, FileAccess.Write);
            //BinaryWriter bw = new BinaryWriter(fs);
            //bw.BaseStream.Seek(0, SeekOrigin.End); // 数据流指针移到最后

            //bw.Write(string.Format("[{0}]：[Info：{1}]", timenow.ToString("yyyy-MM-dd hh:mm:ss"), info));
            //bw.Write("\r\n");
            //if (fs.Length == beginlen)
            //{
            //    bw.Close();  // 关闭文件流                 
            //    fs.Close();
            //    File.Delete(filePathPP);
            //}
            //else
            //{
            //    bw.Close();  // 关闭文件流                 
            //    fs.Close();
            //}

            StreamWriter sw = File.AppendText(filePathPP);
            sw.WriteLine(string.Format("[{0}]：[Info：{1}]", timenow.ToString("yyyy-MM-dd HH:mm:ss"), info));
            sw.Flush();
            sw.Close();

            #endregion
        }
        public static void WriteCBInfo(string info, int bwindex)
        {
            DateTime timenow = DateTime.Now;
            string filePath = BasePath + "\\CB\\";

            #region 检查文件夹
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }
            #endregion

            #region 保存
            string filePathPP = filePath + string.Format("[表位{1}]{0}.txt", bwindex, timenow.ToString("yyyy-MM-dd"));
            StreamWriter sw = File.AppendText(filePathPP);
            sw.WriteLine(string.Format("[{0}]：[Info：{1}]", timenow.ToString("yyyy-MM-dd HH:mm:ss"), info));
            sw.Flush();
            sw.Close();

            #endregion
        }
    }
}
