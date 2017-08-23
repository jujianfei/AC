using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Searchs
{
    /// <summary>
    /// 用于搜索对象或数据的搜索筛选器接口。
    /// 筛选器被初始化后会首先调用 SetFilterName 设置当前使用的筛选器(对于筛选器有多个名称时该方法可以区分当前使用哪个名称的筛选器)。
    /// 实现该接口的类可以添加 IUseAccount 接口，表明该筛选器需要使用当前操作员账号对象。
    /// </summary>
    public interface IFilter
    {
        /// <summary>
        /// 筛选器应用场合选项。
        /// </summary>
        FilterAppliesOptions FilterAppliesAttribute { get; }

        /// <summary>
        /// 有关该筛选器的功能说明、使用说明。
        /// </summary>
        string FilterRemarkAttribute { get; }

        /// <summary>
        /// 设置应用程序框架。
        /// </summary>
        /// <param name="application"></param>
        void SetApplication(ApplicationClass application);

        /// <summary>
        /// 筛选器名称。一个筛选器可以有一个或多个名称，通常应至少设置一个名称；如果不希望当前筛选器被使用，则 null。
        /// </summary>
        string[] FilterNamesAttribute { get; }


        /// <summary>
        /// 设置当前使用的筛选器名称。例如用户自定义分类，筛选器只能有一个，而通过 FilterNamesAttribute 返回诸如地区、行业、类型等参数后，可以将此筛选器变为地区筛选器、行业筛选器、类型筛选器，filterName 参数则是指定当前是地区筛选器还是行业筛选器。
        /// </summary>
        /// <param name="filterName">当前使用的筛选器名称。</param>
        void SetFilterName(string filterName);

        /// <summary>
        /// 此筛选器执行的 SQL 语句。
        /// </summary>
        /// <param name="enableTableName">返回的条件语句是否需要附加表名。例如 table.column = 12 或者 column = 12</param>
        /// <returns></returns>
        string GetFilterSql(bool enableTableName);

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
        /// <returns></returns>
        System.Xml.XmlNode GetFilterConfig(System.Xml.XmlDocument xmlDoc);
    }
}
