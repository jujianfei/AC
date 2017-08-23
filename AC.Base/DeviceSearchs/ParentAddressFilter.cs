using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.DeviceSearchs
{
    /// <summary>
    /// 按照指定的上级设备通讯地址搜索隶属于该设备的所有下级设备。
    /// </summary>
    public class ParentAddressFilter : IDeviceFilter
    {
        /// <summary>
        /// 按照指定的上级设备通讯地址搜索隶属于该设备的所有下级设备。
        /// </summary>
        public ParentAddressFilter()
        {
        }

        /// <summary>
        /// 按照指定的上级设备通讯地址搜索隶属于该设备的所有下级设备。
        /// </summary>
        /// <param name="addresss">通讯地址，或者使用逗号分隔的多个通讯地址。</param>
        public ParentAddressFilter(string addresss)
        {
            this.Addresss = addresss.Split(new char[] { ',' });
        }

        /// <summary>
        /// 按照指定的上级设备通讯地址搜索隶属于该设备的所有下级设备。
        /// </summary>
        /// <param name="addresss"></param>
        public ParentAddressFilter(string[] addresss)
        {
            this.Addresss = addresss;
        }

        /// <summary>
        /// 按照指定的上级设备通讯地址搜索隶属于该设备的所有下级设备。
        /// </summary>
        /// <param name="addresss"></param>
        public ParentAddressFilter(ICollection<string> addresss)
        {
            this.Addresss = addresss.ToArray();
        }

        /// <summary>
        /// 上级设备通讯地址。
        /// </summary>
        public string[] Addresss { get; set; }

        #region IDeviceFilter 成员

        /// <summary>
        /// 检查传入的设备是否符合该筛选器筛选要求。
        /// </summary>
        /// <param name="device">被检查的设备。</param>
        /// <returns></returns>
        public bool DeviceFilterCheck(Device device)
        {
            if (device.Parent != null && device.Parent.DeviceId > 0 && this.Addresss != null && this.Addresss.Length > 0)
            {
                foreach (string strAddress in this.Addresss)
                {
                    if (device.Parent.Address.Equals(strAddress))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        #endregion

        #region IFilter 成员

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
            get { return "按上级设备通讯地址进行筛选"; }
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
            get { return null; }
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
            if (this.Addresss != null && this.Addresss.Length > 0)
            {
                if (this.Addresss.Length == 1)
                {
                    return (enableTableName ? Tables.Device.TableName + "." : "") + Tables.Device.ParentId + " IN (SELECT " + Tables.Device.DeviceId + " FROM " + Tables.Device.TableName + " WHERE " + Tables.Device.DeviceAddress + "=" + Function.SqlStr(this.Addresss[0]) + ")";
                }
                else
                {
                    string strSql = (enableTableName ? Tables.Device.TableName + "." : "") + Tables.Device.ParentId + " IN (SELECT " + Tables.Device.DeviceId + " FROM " + Tables.Device.TableName + " WHERE " + Tables.Device.DeviceAddress + " IN (";
                    foreach (string strAddress in this.Addresss)
                    {
                        strSql += "," + Function.SqlStr(strAddress);
                    }
                    strSql = strSql.Substring(1) + "))";
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
            if (this.Addresss != null && this.Addresss.Length > 0)
            {
                if (this.Addresss.Length == 1)
                {
                    return "上级设备通讯地址为" + this.Addresss[0];
                }
                else
                {
                    string strSql = "";
                    foreach (string strAddress in this.Addresss)
                    {
                        strSql += "、" + strAddress;
                    }
                    strSql = "上级设备通讯地址是" + strSql.Substring(1) + "的设备";
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
            this.Addresss = new string[xmlNode.ChildNodes.Count];
            for (int intIndex = 0; intIndex < xmlNode.ChildNodes.Count; intIndex++)
            {
                this.Addresss[intIndex] = xmlNode.ChildNodes[intIndex].InnerText;
            }
        }

        /// <summary>
        /// 获取当前筛选器的状态，将序列化的内容填充至 XmlNode 的 InnerText 属性或者 ChildNodes 子节点中。
        /// </summary>
        /// <param name="xmlDoc">创建 XmlNode 时所需使用到的 System.Xml.XmlDocument。</param>
        public System.Xml.XmlNode GetFilterConfig(System.Xml.XmlDocument xmlDoc)
        {
            if (this.Addresss != null && this.Addresss.Length > 0)
            {
                System.Xml.XmlNode xnConfig = xmlDoc.CreateElement(this.GetType().Name);

                foreach (string strAddress in this.Addresss)
                {
                    System.Xml.XmlNode xnId = xmlDoc.CreateElement("Address");
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
