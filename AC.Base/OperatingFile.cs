using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace AC.Base
{
    public class OperatingFile
    {
        string _path = System.Windows.Forms.Application.StartupPath + "\\Log\\";
        string _filePath;

        public OperatingFile(string _fileName)
        {

            this._filePath = this._path + _fileName + ".txt";
            //判断文件夹是否存在，不存在则创建。
            if(!this.isPathExist())
            {
                this.createPathFile();
            }
            //判断文件是否存在，不存在则创建。
            if (!this.isExist())
            {
                this.createFile();
            }

        }

        /// <summary>
        /// 追加文件内容
        /// </summary>
        public void appendFile(string file)
        {
            StreamWriter sw = File.AppendText(this._filePath);
            sw.WriteLine(file);
            sw.Flush();
            sw.Close();
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        public void deleteFile()
        {
            File.Delete(this._filePath);
        }

        /// <summary>
        /// 判断文件是否存在
        /// </summary>
        /// <returns></returns>
        public bool isExist()
        {
            return File.Exists(this._filePath);
        }

        /// <summary>
        /// 读取文件
        /// </summary>
        /// <returns></returns>
        public string getFile()
        {
            /**/
            ///从指定的目录以打开或者创建的形式读取日志文件
            FileStream fs = new FileStream(this._filePath, FileMode.OpenOrCreate, FileAccess.Read);

            /**/
            ///定义输出字符串
            StringBuilder output = new StringBuilder();

            /**/
            ///初始化该字符串的长度为0
            output.Length = 0;

            /**/
            ///为上面创建的文件流创建读取数据流
            StreamReader read = new StreamReader(fs, Encoding.GetEncoding("UTF-8"));

            /**/
            ///设置当前流的起始位置为文件流的起始点
            read.BaseStream.Seek(0, SeekOrigin.Begin);

            /**/
            ///读取文件
            while (read.Peek() > -1)
            {
                /**/
                ///取文件的一行内容并换行
                output.Append(read.ReadLine() + "<br>");
            }

            /**/
            ///关闭释放读数据流
            read.Close();

            ////返回读到的日志文件内容
            return output.ToString();
            //string file="";
            //FileStream fs = new FileStream(this._filePath, FileMode.Open, FileAccess.Read);
            //StreamReader sr=new StreamReader(fs,Encoding.GetEncoding("GB2312"));
            //sr.BaseStream.Seek(0, SeekOrigin.Begin);
            //string str=sr.ReadLine();
            //while(str!=null)
            //{
            //    file+=str;
            //    str=sr.ReadLine();
            //}
            //sr.Close();
            //fs.Close();
            //return file;
        }


        /// <summary>
        /// 创建文件
        /// </summary>
        public void createFile()
        {
            StreamWriter sw = new StreamWriter(this._filePath, false, Encoding.Default);
            sw.Flush();
            sw.Close();
        }

        /// <summary>
        /// 判断目录是否存在
        /// </summary>
        /// <returns></returns>
        public bool isPathExist()
        {
            bool result = false;
            if (Directory.Exists(this._path))
            {
                result = true;
            }
            return result;
        }

        /// <summary>
        /// 创建目录
        /// </summary>
        public void createPathFile()
        {
            Directory.CreateDirectory(this._path);
        }

        /// <summary>
        /// 获取文件完整路径
        /// </summary>
        /// <returns></returns>
        public string getFileFullPath()
        {
            return this._filePath;
        }
    }
}
