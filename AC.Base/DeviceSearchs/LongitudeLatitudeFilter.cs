using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.DeviceSearchs
{
    /// <summary>
    /// 按设备经纬度搜索设备。
    /// </summary>
    public class LongitudeLatitudeFilter : IDeviceFilter
    {
        /// <summary>
        /// 按设备经纬度搜索设备。
        /// </summary>
        public LongitudeLatitudeFilter()
        {
        }

        /// <summary>
        /// 指定经纬度范围搜索设备。
        /// </summary>
        /// <param name="longitude1">经度1</param>
        /// <param name="longitude2">经度2</param>
        /// <param name="latitude1">纬度1</param>
        /// <param name="latitude2">纬度2</param>
        public LongitudeLatitudeFilter(decimal longitude1, decimal longitude2, decimal latitude1, decimal latitude2)
        {
            this.Longitude1 = longitude1;
            this.Longitude2 = longitude2;
            this.Latitude1 = latitude1;
            this.Latitude2 = latitude2;
        }

        /// <summary>
        /// 经度1
        /// </summary>
        public decimal Longitude1 { get; set; }

        /// <summary>
        /// 经度2
        /// </summary>
        public decimal Longitude2 { get; set; }

        /// <summary>
        /// 纬度1
        /// </summary>
        public decimal Latitude1 { get; set; }

        /// <summary>
        /// 纬度2
        /// </summary>
        public decimal Latitude2 { get; set; }

        #region IDeviceFilter 成员

        /// <summary>
        /// 检查传入的设备是否符合该筛选器筛选要求。
        /// </summary>
        /// <param name="device">被检查的设备。</param>
        /// <returns></returns>
        public bool DeviceFilterCheck(Device device)
        {
            if (this.Longitude1 <= this.Longitude2)
            {
                if (device.Longitude >= this.Longitude1 && device.Longitude <= this.Longitude2)
                {
                }
                else
                {
                    return false;
                }
            }
            else
            {
                if (device.Longitude >= this.Longitude2 && device.Longitude <= this.Longitude1)
                {
                }
                else
                {
                    return false;
                }
            }

            if (this.Latitude1 <= this.Latitude2)
            {
                if (device.Latitude >= this.Latitude1 && device.Latitude <= this.Latitude2)
                {
                }
                else
                {
                    return false;
                }
            }
            else
            {
                if (device.Latitude >= this.Latitude1 && device.Latitude <= this.Latitude2)
                {
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        #endregion

        #region IFilter 成员

        /// <summary>
        /// 筛选器应用场合选项。如果该筛选器不可以应用在任何场合则返回“0”
        /// </summary>
        public Searchs.FilterAppliesOptions FilterAppliesAttribute
        {
            get { return Searchs.FilterAppliesOptions.All; }
        }

        /// <summary>
        /// 有关该筛选器的功能说明、使用说明。
        /// </summary>
        public string FilterRemarkAttribute
        {
            get { return "搜索指定经纬度范围内的设备"; }
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
            string strTableName = (enableTableName ? Tables.Device.TableName + "." : "");
            string strSql = "";
            if (this.Longitude1 <= this.Longitude2)
            {
                strSql += strTableName + Tables.Device.Longitude + ">=" + this.Longitude1 + " AND " + strTableName + Tables.Device.Longitude + "<=" + this.Longitude2;
            }
            else
            {
                strSql += strTableName + Tables.Device.Longitude + ">=" + this.Longitude2 + " AND " + strTableName + Tables.Device.Longitude + "<=" + this.Longitude1;
            }

            strSql += " AND ";

            if (this.Latitude1 <= this.Latitude2)
            {
                strSql += strTableName + Tables.Device.Latitude + ">=" + this.Latitude1 + " AND " + strTableName + Tables.Device.Latitude + "<=" + this.Latitude2;
            }
            else
            {
                strSql += strTableName + Tables.Device.Latitude + ">=" + this.Latitude2 + " AND " + strTableName + Tables.Device.Latitude + "<=" + this.Latitude1;
            }

            return strSql;
        }

        /// <summary>
        /// 获取该筛选器所执行的筛选逻辑的文字描述。
        /// </summary>
        /// <returns></returns>
        public string GetFilterDescription()
        {
            string strDescription = "";
            if (this.Longitude1 == this.Longitude2)
            {
                strDescription += "经度位于" + this.Longitude1;
            }
            else
            {
                strDescription += "经度位于" + this.Longitude1 + "和" + this.Longitude2 + "之间";
            }

            strDescription += " ";

            if (this.Latitude1 == this.Latitude2)
            {
                strDescription += "纬度位于" + this.Latitude1;
            }
            else
            {
                strDescription += "纬度位于" + this.Latitude1 + "和" + this.Latitude2 + "之间";
            }

            return strDescription;
        }

        /// <summary>
        /// 从保存此筛选器数据的 XML 文档节点集合初始化当前筛选器。
        /// </summary>
        /// <param name="xmlNode">该对象节点的数据集合</param>
        public void SetFilterConfig(System.Xml.XmlNode xmlNode)
        {
            for (int intIndex = 0; intIndex < xmlNode.ChildNodes.Count; intIndex++)
            {
                if (xmlNode.ChildNodes[intIndex].Name.Equals("Longitude1"))
                {
                    this.Longitude1 = Function.ToDecimal(xmlNode.ChildNodes[intIndex].InnerText);
                }
                else if (xmlNode.ChildNodes[intIndex].Name.Equals("Longitude2"))
                {
                    this.Longitude2 = Function.ToDecimal(xmlNode.ChildNodes[intIndex].InnerText);
                }
                else if (xmlNode.ChildNodes[intIndex].Name.Equals("Latitude1"))
                {
                    this.Latitude1 = Function.ToDecimal(xmlNode.ChildNodes[intIndex].InnerText);
                }
                else if (xmlNode.ChildNodes[intIndex].Name.Equals("Latitude2"))
                {
                    this.Latitude2 = Function.ToDecimal(xmlNode.ChildNodes[intIndex].InnerText);
                }
            }
        }

        /// <summary>
        /// 获取当前筛选器的状态，将序列化的内容填充至 XmlNode 的 InnerText 属性或者 ChildNodes 子节点中。
        /// </summary>
        /// <param name="xmlDoc">创建 XmlNode 时所需使用到的 System.Xml.XmlDocument。</param>
        public System.Xml.XmlNode GetFilterConfig(System.Xml.XmlDocument xmlDoc)
        {
            System.Xml.XmlNode xnConfig = xmlDoc.CreateElement(this.GetType().Name);

            System.Xml.XmlNode xnLongitude1 = xmlDoc.CreateElement("Longitude1");
            xnLongitude1.InnerText = this.Longitude1.ToString();
            xnConfig.AppendChild(xnLongitude1);

            System.Xml.XmlNode xnLongitude2 = xmlDoc.CreateElement("Longitude2");
            xnLongitude2.InnerText = this.Longitude2.ToString();
            xnConfig.AppendChild(xnLongitude2);

            System.Xml.XmlNode xnLatitude1 = xmlDoc.CreateElement("Latitude1");
            xnLatitude1.InnerText = this.Latitude1.ToString();
            xnConfig.AppendChild(xnLatitude1);

            System.Xml.XmlNode xnLatitude2 = xmlDoc.CreateElement("Latitude2");
            xnLatitude2.InnerText = this.Latitude2.ToString();
            xnConfig.AppendChild(xnLatitude2);

            return xnConfig;
        }

        #endregion
    }
}
