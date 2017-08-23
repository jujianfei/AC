using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.DeviceSearchs
{
    /// <summary>
    /// 按照指定的上级设备编号搜索隶属于该设备的所有下级设备。
    /// </summary>
    public class StatusFilter : IDeviceFilter
    {
        /// <summary>
        /// 按照指定的设备状态搜索设备。
        /// </summary>
        public StatusFilter()
        {
        }

        /// <summary>
        /// 按照指定的设备状态搜索设备。
        /// </summary>
        /// <param name="id"></param>
        public StatusFilter(int id)
        {
            this.SIds = new int[] { id };
        }

        /// <summary>
        /// 按照指定的上级设备编号搜索隶属于该设备的所有下级设备。
        /// </summary>
        /// <param name="ids"></param>
        public StatusFilter(int[] ids)
        {
            this.SIds = ids;
        }

        /// <summary>
        /// 按照指定的上级设备编号搜索隶属于该设备的所有下级设备。
        /// </summary>
        /// <param name="ids"></param>
        public StatusFilter(ICollection<int> ids)
        {
            this.SIds = new int[ids.Count];
            ids.CopyTo(this.SIds, 0);
        }

        /// <summary>
        /// 上级设备编号。
        /// </summary>
        public int[] SIds { get; set; }

        #region IDeviceFilter 成员
        /// <summary>
        /// 检查传入的设备是否符合该筛选器筛选要求。
        /// </summary>
        /// <param name="device">被检查的设备。</param>
        /// <returns></returns>
        public bool DeviceFilterCheck(Device device)
        {
            if (device.Parent != null && device.State > 0 && this.SIds != null && this.SIds.Length > 0)
            {
                foreach (int intId in this.SIds)
                {
                    if ((int)device.State == intId)
                        return true;
                }
            }
            return false;
        }
        #endregion

        #region IFilter 成员

        /// <summary>
        /// 筛选器应用场合选项。如果该筛选器不可以应用在任何场合则返回“0”
        /// </summary>
        public AC.Base.Searchs.FilterAppliesOptions FilterAppliesAttribute
        {
            get { return AC.Base.Searchs.FilterAppliesOptions.None; }
        }

        /// <summary>
        /// 有关该筛选器的功能说明、使用说明。
        /// </summary>
        public string FilterRemarkAttribute
        {
            get { return "按状态搜索设备"; }
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
            if (this.SIds != null && this.SIds.Length > 0)
            {
                if (this.SIds.Length == 1)
                {
                    return (enableTableName ? Tables.Device.TableName + "." : "") + Tables.Device.StateOption + "=" + this.SIds[0];
                }
                else
                {
                    string strSql = (enableTableName ? Tables.Device.TableName + "." : "") + Tables.Device.StateOption + " IN (";
                    foreach (int intId in this.SIds)
                    {
                        strSql += intId + ",";
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
            if (this.SIds != null && this.SIds.Length > 0)
            {
                if (this.SIds.Length == 1)
                {
                    return string.Format("设备状态为 {0}", DeviceStateExtensions.GetDescription((DeviceStateOptions)this.SIds[0]));
                }
                else
                {
                    string strSql = "";
                    foreach (int intId in this.SIds)
                    {
                        strSql += "、" + DeviceStateExtensions.GetDescription((DeviceStateOptions)intId);
                    }
                    strSql = "设备状态为" + strSql.Substring(1) + "的设备";
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
            this.SIds = new int[xmlNode.ChildNodes.Count];
            for (int intIndex = 0; intIndex < xmlNode.ChildNodes.Count; intIndex++)
            {
                this.SIds[intIndex] = Function.ToInt(xmlNode.ChildNodes[intIndex].InnerText);
            }
        }

        /// <summary>
        /// 获取当前筛选器的状态，将序列化的内容填充至 XmlNode 的 InnerText 属性或者 ChildNodes 子节点中。
        /// </summary>
        /// <param name="xmlDoc">创建 XmlNode 时所需使用到的 System.Xml.XmlDocument。</param>
        public System.Xml.XmlNode GetFilterConfig(System.Xml.XmlDocument xmlDoc)
        {
            if (this.SIds != null && this.SIds.Length > 0)
            {
                System.Xml.XmlNode xnConfig = xmlDoc.CreateElement(this.GetType().Name);

                foreach (int intId in this.SIds)
                {
                    System.Xml.XmlNode xnId = xmlDoc.CreateElement("StateOption");
                    xnId.InnerText = intId.ToString();
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
