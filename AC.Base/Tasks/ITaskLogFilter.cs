using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Tasks
{
    /// <summary>
    /// 任务运行日志搜索筛选器，作为搜索任务运行日志数据时的一个条件。
    /// 实现该接口的类可以添加 IUseAccount 接口，表明当前筛选器需要使用到当前操作员账号对象。
    /// </summary>
    public interface ITaskLogFilter
    {
        /// <summary>
        /// 设置应用程序框架。
        /// </summary>
        /// <param name="application"></param>
        void SetApplication(ApplicationClass application);

        /// <summary>
        /// 此筛选器执行的 SQL 语句。
        /// </summary>
        /// <param name="enableTableName">返回的条件语句是否需要附加表名。例如 table.column = 12 或者 column = 12</param>
        /// <param name="month">所需查询数据的月份，该参数中年、月属性有效。</param>
        /// <returns></returns>
        string GetFilterSql(bool enableTableName, DateTime month);

        /// <summary>
        /// 获取该筛选器所执行的筛选逻辑的文字描述。
        /// </summary>
        /// <returns></returns>
        string GetFilterDescription();

        /// <summary>
        /// 从保存此筛选器数据的 XML 文档节点集合初始化当前筛选器。
        /// </summary>
        /// <param name="xmlNode">该对象节点的数据集合</param>
        void SetFilterConfig(System.Xml.XmlNode xmlNode);

        /// <summary>
        /// 获取当前筛选器的状态，将序列化的内容填充至 XmlNode 的 InnerText 属性或者 ChildNodes 子节点中。
        /// </summary>
        /// <param name="xmlDoc">创建 XmlNode 时所需使用到的 System.Xml.XmlDocument。</param>
        System.Xml.XmlNode GetFilterConfig(System.Xml.XmlDocument xmlDoc);
    }
}
