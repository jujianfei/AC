using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AC.Base.Drives;

namespace AC.Base.DeviceSearchs
{
    /// <summary>
    /// 搜索指定设备类型或指定驱动类型的设备。
    /// </summary>
    public class DeviceTypeFilter : IDeviceFilter
    {
        private AC.Base.ApplicationClass m_Application;

        /// <summary>
        /// 搜索指定设备类型或指定驱动类型的设备。
        /// </summary>
        public DeviceTypeFilter()
        {
        }

        /// <summary>
        /// 搜索指定设备类型或指定驱动类型的设备。
        /// </summary>
        /// <param name="deviceType">设备类型或驱动类型。</param>
        public DeviceTypeFilter(Type deviceType)
        {
            this.DeviceType = deviceType;
        }

        private Type m_DeviceType;
        /// <summary>
        /// 获取或设置设备类型、设备功能实现接口、驱动类型、驱动功能实现接口。
        /// </summary>
        public Type DeviceType
        {
            get
            {
                return this.m_DeviceType;
            }
            set
            {
                this.m_DeviceType = value;
            }
        }


        #region IDeviceFilter 成员

        /// <summary>
        /// 检查传入的设备是否符合该筛选器筛选要求。
        /// </summary>
        /// <param name="device">被检查的设备。</param>
        /// <returns></returns>
        public bool DeviceFilterCheck(Device device)
        {
            if (this.DeviceType != null)
            {
                if (this.DeviceType.IsClass)
                {
                    if (Function.IsInheritableBaseType(device.DeviceType.Type, this.DeviceType))
                    {
                        return true;
                    }
                    else if (device.DeviceType.DriveType != null && Function.IsInheritableBaseType(device.DeviceType.DriveType, this.DeviceType))
                    {
                        return true;
                    }
                }
                else if (this.DeviceType.IsInterface)
                {
                    if (device.DeviceType.Type.GetInterface(this.DeviceType.FullName) != null)
                    {
                        return true;
                    }
                    else if (device.DeviceType.DriveType != null && device.DeviceType.DriveType.GetInterface(this.DeviceType.FullName) != null)
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
        /// 筛选器应用场合选项。
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
            get { return "搜索指定类型的设备"; }
        }

        /// <summary>
        /// 设置应用程序框架。
        /// </summary>
        /// <param name="application"></param>
        public void SetApplication(ApplicationClass application)
        {
            this.m_Application = application;
        }

        /// <summary>
        /// 筛选器名称。一个筛选器可以有一个或多个名称，通常应至少设置一个名称；如果不希望当前筛选器被使用，则 null。
        /// </summary>
        public string[] FilterNamesAttribute
        {
            get { return new string[] { "设备类型" }; }
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
            string strSql = "";

            if (this.DeviceType != null)
            {
                strSql = this.GetDeviceType(this.m_Application.DeviceTypeSort);

                if (strSql.Length > 0)
                {
                    strSql = (enableTableName ? Tables.Device.TableName + "." : "") + Tables.Device.DeviceType + " IN (" + strSql.Substring(1) + ")";
                }
            }

            return strSql;
        }

        private string GetDeviceType(DeviceTypeSort deviceTypeSort)
        {
            string strType = "";

            foreach (DeviceType deviceType in deviceTypeSort.DeviceTypes)
            {
                if (this.DeviceType.IsClass)
                {
                    if (Function.IsInheritableBaseType(deviceType.Type, this.DeviceType))
                    {
                        strType += ",'" + deviceType.Type.FullName + "'";
                    }
                    else if (deviceType.DriveType != null && Function.IsInheritableBaseType(deviceType.DriveType, this.DeviceType))
                    {
                        strType += ",'" + deviceType.Type.FullName + "'";
                    }
                }
                else if (this.DeviceType.IsInterface)
                {
                    if (deviceType.Type.GetInterface(this.DeviceType.FullName) != null)
                    {
                        strType += ",'" + deviceType.Type.FullName + "'";
                    }
                    else if (deviceType.DriveType != null && deviceType.DriveType.GetInterface(this.DeviceType.FullName) != null)
                    {
                        strType += ",'" + deviceType.Type.FullName + "'";
                    }
                }
            }

            foreach (DeviceTypeSort children in deviceTypeSort.Children)
            {
                strType += this.GetDeviceType(children);
            }

            return strType;
        }

        /// <summary>
        /// 获取该筛选器所执行的筛选逻辑的文字描述。
        /// </summary>
        /// <returns></returns>
        public string GetFilterDescription()
        {
            string strDescription = "";

            if (this.DeviceType != null)
            {
                strDescription = this.GetDeviceDescription(this.m_Application.DeviceTypeSort);

                if (strDescription.Length > 0)
                {
                    strDescription = "设备类型是" + strDescription.Substring(1) + "的设备";
                }
            }

            return strDescription;
        }

        private string GetDeviceDescription(DeviceTypeSort deviceTypeSort)
        {
            string strType = "";

            foreach (DeviceType deviceType in deviceTypeSort.DeviceTypes)
            {
                if (this.DeviceType.IsClass)
                {
                    if (Function.IsInheritableBaseType(deviceType.Type, this.DeviceType))
                    {
                        strType += "、" + deviceType.Type.FullName;
                    }
                    else if (deviceType.DriveType != null && Function.IsInheritableBaseType(deviceType.DriveType, this.DeviceType))
                    {
                        strType += "、" + deviceType.Type.FullName;
                    }
                }
                else if (this.DeviceType.IsInterface)
                {
                    if (deviceType.Type.GetInterface(this.DeviceType.FullName) != null)
                    {
                        strType += "、" + deviceType.Type.FullName;
                    }
                    else if (deviceType.DriveType != null && deviceType.DriveType.GetInterface(this.DeviceType.FullName) != null)
                    {
                        strType += "、" + deviceType.Type.FullName;
                    }
                }
            }

            foreach (DeviceTypeSort children in deviceTypeSort.Children)
            {
                strType += this.GetDeviceType(children);
            }

            return strType;
        }

        /// <summary>
        /// 从保存此筛选器数据的 XML 文档节点集合初始化当前筛选器。
        /// </summary>
        /// <param name="xmlNode">该对象节点的数据集合</param>
        public void SetFilterConfig(System.Xml.XmlNode xmlNode)
        {
            this.DeviceType = Function.GetType(xmlNode.InnerText);
        }

        /// <summary>
        /// 获取当前筛选器的状态，将序列化的内容填充至 XmlNode 的 InnerText 属性或者 ChildNodes 子节点中。
        /// </summary>
        /// <param name="xmlDoc">创建 XmlNode 时所需使用到的 System.Xml.XmlDocument。</param>
        public System.Xml.XmlNode GetFilterConfig(System.Xml.XmlDocument xmlDoc)
        {
            if (this.DeviceType != null)
            {
                System.Xml.XmlNode xnConfig = xmlDoc.CreateElement(this.GetType().Name);
                xnConfig.InnerText = this.DeviceType.FullName;
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
