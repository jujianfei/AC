using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Logs
{
    /// <summary>
    /// 不存在的日志。如果数据库中保存的某一日志类型不存在，则将使用此类型进行替换。
    /// </summary>
    public class NonexistentLog : Log
    {
        private System.Xml.XmlNode m_LogConfig;

        /// <summary>
        /// 不存在的日志。如果数据库中保存的某一日志类型不存在，则将使用此类型进行替换。
        /// </summary>
        /// <param name="application">应用程序框架。</param>
        public NonexistentLog(ApplicationClass application)
            : base(application)
        {
        }

        /// <summary>
        /// 获取当前日志的配置信息，将序列化的内容填充至 XmlNode 的 InnerText 属性或者 ChildNodes 子节点中。
        /// </summary>
        /// <param name="xmlDoc">创建 XmlNode 时所需使用到的 System.Xml.XmlDocument。</param>
        /// <returns>如无配置内容则返回 null。</returns>
        public override System.Xml.XmlNode GetLogConfig(System.Xml.XmlDocument xmlDoc)
        {
            return this.m_LogConfig;
        }

        /// <summary>
        /// 从保存此日志数据的 XML 文档节点初始化当前日志。
        /// </summary>
        /// <param name="logConfig">该日志对象的数据</param>
        public override void SetLogConfig(System.Xml.XmlNode logConfig)
        {
            this.m_LogConfig = logConfig;
        }

        /// <summary>
        /// 获取当前日志简要的描述性文字。
        /// </summary>
        /// <returns></returns>
        public override string GetContent()
        {
            if (this.m_LogConfig != null)
            {
                return "日志类型 " + base.LogType.Code + " 已丢失。" + this.m_LogConfig.OuterXml;
            }
            else
            {
                return "日志类型 " + base.LogType.Code + " 已丢失。";
            }
        }

        /// <summary>
        /// 向页面输出 HTML 格式的日志内容，使用 UTF-8 编码格式。
        /// </summary>
        /// <param name="output">字符输出对象，调用 WriteLine 方法向界面输出 HTML 内容。</param>
        /// <param name="temporaryDirectory">可写入临时文件的临时目录。</param>
        /// <param name="relativePath">当前页面相对于临时目录的相对路径。</param>
        public override void WriterHtml(System.IO.TextWriter output, string temporaryDirectory, string relativePath)
        {
            if (this.m_LogConfig != null)
            {
                output.WriteLine("日志类型 " + base.LogType.Code + " 已丢失。<br>" + Function.OutHtml(this.m_LogConfig.OuterXml));
            }
            else
            {
                output.WriteLine("日志类型 " + base.LogType.Code + " 已丢失。");
            }
        }

        /// <summary>
        /// 从日志附加数值中初始化整型属性。
        /// </summary>
        /// <param name="code">属性类型，通常使用搜索筛选器的类名。</param>
        /// <param name="ordinalNumber">序号，用于存储同一属性的多个值。</param>
        /// <param name="dataValue">数据值。</param>
        public override void SetDataValue(string code, int ordinalNumber, int? dataValue)
        {
        }

        /// <summary>
        /// 从日志附加数值中初始化长整型属性。
        /// </summary>
        /// <param name="code">属性类型，通常使用搜索筛选器的类名。</param>
        /// <param name="ordinalNumber">序号，用于存储同一属性的多个值。</param>
        /// <param name="dataValue">数据值。</param>
        public override void SetDataValue(string code, int ordinalNumber, long? dataValue)
        {
        }

        /// <summary>
        /// 从日志附加数值中初始化带小数的数字属性。
        /// </summary>
        /// <param name="code">属性类型，通常使用搜索筛选器的类名。</param>
        /// <param name="ordinalNumber">序号，用于存储同一属性的多个值。</param>
        /// <param name="dataValue">数据值。</param>
        public override void SetDataValue(string code, int ordinalNumber, decimal? dataValue)
        {
        }

        /// <summary>
        /// 从日志附加数值中初始化字符串属性。
        /// </summary>
        /// <param name="code">属性类型，通常使用搜索筛选器的类名。</param>
        /// <param name="ordinalNumber">序号，用于存储同一属性的多个值。</param>
        /// <param name="dataValue">数据值。</param>
        public override void SetDataValue(string code, int ordinalNumber, string dataValue)
        {
        }
    }
}
