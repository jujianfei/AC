using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.DeviceSearchs
{
    /// <summary>
    /// 按照设备名称搜索设备。
    /// </summary>
    public class NameFilter : IDeviceFilter
    {
        /// <summary>
        /// 按照设备通讯地址搜索设备。
        /// </summary>
        public NameFilter()
        {
        }

        /// <summary>
        /// 按照设备通讯地址搜索设备。
        /// </summary>
        /// <param name="addresss">通讯地址，或者使用逗号分隔的多个通讯地址。</param>
        public NameFilter(string name)
        {
            this.Name = name.Split(new char[] { ',' });
        }

        /// <summary>
        /// 按照设备通讯地址搜索设备。
        /// </summary>
        /// <param name="addresss"></param>
        public NameFilter(string[] name)
        {
            this.Name = name;
        }

        /// <summary>
        /// 按照设备通讯地址搜索设备。
        /// </summary>
        /// <param name="addresss"></param>
        public NameFilter(ICollection<string> name)
        {
            this.Name = name.ToArray();
        }

        /// <summary>
        /// 设备名称。
        /// </summary>
        public string[] Name { get; set; }

        #region << IDeviceFilter 成员 >>

        /// <summary>
        /// 检查传入的设备是否符合该筛选器筛选要求。
        /// </summary>
        /// <param name="device">被检查的设备。</param>
        /// <returns></returns>
        public bool DeviceFilterCheck(Device device)
        {
            if (device.Name != null && this.Name != null && this.Name.Length > 0)
            {
                foreach (string strName in this.Name)
                {
                    if (device.Address.Equals(strName))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        #endregion

        #region << IFilter 成员 >>
        /// <summary>
        /// 筛选器应用场合选项。如果该筛选器不可以应用在任何场合则返回“0”
        /// </summary>
        public Searchs.FilterAppliesOptions FilterAppliesAttribute
        {
            get { return AC.Base.Searchs.FilterAppliesOptions.None; }
        }

        /// <summary>
        /// 有关该筛选器的功能说明、使用说明。
        /// </summary>
        public string FilterRemarkAttribute
        {
            get { return "按设备名称进行筛选"; }
        }

        /// <summary>
        /// 设置应用程序框架。
        /// </summary>
        /// <param name="application"></param>
        public void SetApplication(ApplicationClass application)
        {
        }

        /// <summary>
        /// 筛选器名称。一个筛选器可以有一个或多个名称，通常应至少设置一个名称；如果不希望当前筛选器被使用，则 null。
        /// </summary>
        public string[] FilterNamesAttribute
        {
            get { return new string[] { "设备名称" }; }
        }

        /// <summary>
        /// 设置当前使用的筛选器名称。例如用户自定义分类，筛选器只能有一个，而通过 FilterNamesAttribute 返回诸如地区、行业、类型等参数后，可以将此筛选器变为地区筛选器、行业筛选器、类型筛选器，filterName 参数则是指定当前是地区筛选器还是行业筛选器。
        /// </summary>
        /// <param name="filterName">当前使用的筛选器名称。</param>
        public void SetFilterName(string filterName)
        {
        }

        /// <summary>
        /// 此筛选器执行的 SQL 语句。
        /// </summary>
        /// <param name="enableTableName">返回的条件语句是否需要附加表名。例如 table.column = 12 或者 column = 12</param>
        /// <returns></returns>
        public string GetFilterSql(bool enableTableName)
        {
            if (this.Name != null && this.Name.Length > 0)
            {
                if (this.Name.Length == 1)
                {
                    return (enableTableName ? Tables.Device.TableName + "." : "") + Tables.Device.Name + " like " + Function.SqlStr("%"+this.Name[0]+"%");
                }
                else
                {
                    List<string> lstName = new List<string>();
                    string strSql = (enableTableName ? Tables.Device.TableName + "." : "") + Tables.Device.DeviceId + " IN (";
                    foreach (string strName in this.Name)
                    {
                        if (lstName.Contains(strName) == false)
                        {
                            strSql += Function.SqlStr(strName) + ",";
                            lstName.Add(strName);
                        }
                    }
                    strSql = strSql.Substring(0, strSql.Length - 1) + ")";
                    return strSql;
                }
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// 获取该筛选器所执行的筛选逻辑的文字描述。
        /// </summary>
        /// <returns></returns>
        public string GetFilterDescription()
        {
            if (this.Name != null && this.Name.Length > 0)
            {
                if (this.Name.Length == 1)
                {
                    return "设备名称为" + this.Name[0];
                }
                else
                {
                    string strSql = "";
                    foreach (string strName in this.Name)
                    {
                        strSql += "、" + strName;
                    }
                    strSql = "设备名称是" + strSql.Substring(1) + "的设备";
                    return strSql;
                }
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// 从保存此筛选器数据的 XML 文档节点集合初始化当前筛选器。
        /// </summary>
        /// <param name="xmlNode">该对象节点的数据集合</param>
        public void SetFilterConfig(System.Xml.XmlNode xmlNode)
        {
            this.Name = new string[xmlNode.ChildNodes.Count];
            for (int intIndex = 0; intIndex < xmlNode.ChildNodes.Count; intIndex++)
            {
                this.Name[intIndex] = xmlNode.ChildNodes[intIndex].InnerText;
            }
        }

        /// <summary>
        /// 获取当前筛选器的状态，将序列化的内容填充至 XmlNode 的 InnerText 属性或者 ChildNodes 子节点中。
        /// </summary>
        /// <param name="xmlDoc">创建 XmlNode 时所需使用到的 System.Xml.XmlDocument。</param>
        public System.Xml.XmlNode GetFilterConfig(System.Xml.XmlDocument xmlDoc)
        {
            if (this.Name != null && this.Name.Length > 0)
            {
                System.Xml.XmlNode xnConfig = xmlDoc.CreateElement(this.GetType().Name);

                foreach (string strAddress in this.Name)
                {
                    System.Xml.XmlNode xnId = xmlDoc.CreateElement("Name");
                    xnId.InnerText = strAddress.ToString();
                    xnConfig.AppendChild(xnId);
                }
                return xnConfig;
            }
            else
            {
                return null;
            }
        }

        #endregion
    }
}
