using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.ClassifySearchs
{
    /// <summary>
    /// 按照设备搜索关联的分类。
    /// </summary>
    public class DeviceFilter : IClassifyFilter
    {
        /// <summary>
        /// 按照设备搜索关联的分类。
        /// </summary>
        public DeviceFilter()
        {
        }

        /// <summary>
        /// 按照设备搜索关联的分类。
        /// </summary>
        /// <param name="device">分类所属的设备。</param>
        /// <param name="containsParent">是否包含与该设备关联的所有上级分类。</param>
        public DeviceFilter(Device device, bool containsParent)
        {
            this.Devices = new Device[] { device };
            this.ContainsParent = containsParent;
        }

        /// <summary>
        /// 按照设备搜索关联的分类。
        /// </summary>
        /// <param name="devices">分类所属的设备。</param>
        /// <param name="containsParent">是否包含与该设备关联的所有上级分类。</param>
        public DeviceFilter(Device[] devices, bool containsParent)
        {
            this.Devices = devices;
            this.ContainsParent = containsParent;
        }

        /// <summary>
        /// 按照设备搜索关联的分类。
        /// </summary>
        /// <param name="devices">分类所属的设备。</param>
        /// <param name="containsParent">是否包含与该设备关联的所有上级分类。</param>
        public DeviceFilter(ICollection<Device> devices, bool containsParent)
        {
            this.Devices = devices.ToArray();
            this.ContainsParent = containsParent;
        }

        /// <summary>
        /// 设备。
        /// </summary>
        public Device[] Devices { get; set; }

        /// <summary>
        /// 是否包含与该设备关联的所有上级分类。
        /// </summary>
        public bool ContainsParent { get; set; }

        #region IClassifyFilter 成员

        /// <summary>
        /// 检查传入的分类是否符合该筛选器筛选要求。
        /// </summary>
        /// <param name="classify">被检查的分类。</param>
        /// <returns></returns>
        public bool ClassifyFilterCheck(Classify classify)
        {
            foreach (Device _Device in this.Devices)
            {
                if (_Device.Classifys.Contains(classify))
                {
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
        public Searchs.FilterAppliesOptions FilterAppliesAttribute
        {
            get { return Searchs.FilterAppliesOptions.All; }
        }

        /// <summary>
        /// 有关该筛选器的功能说明、使用说明。
        /// </summary>
        public string FilterRemarkAttribute
        {
            get { return "搜索关联指定设备的分类"; }
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
            get { return new string[] { "设备" }; }
        }

        /// <summary>
        /// 设置当前使用的筛选器名称。例如用户自定义设备，筛选器只能有一个，而通过 FilterNamesAttribute 返回诸如地区、行业、类型等参数后，可以将此筛选器变为地区筛选器、行业筛选器、类型筛选器，filterName 参数则是指定当前是地区筛选器还是行业筛选器。
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
            if (this.Devices != null && this.Devices.Length > 0)
            {
                List<int> lstId = new List<int>();
                string strId = "";
                foreach (Device _Device in this.Devices)
                {
                    if (lstId.Contains(_Device.DeviceId) == false)
                    {
                        strId += "," + _Device.DeviceId;
                        lstId.Add(_Device.DeviceId);
                    }
                }

                if (this.ContainsParent)
                {
                    return (enableTableName ? Tables.Classify.TableName + "." : "") + Tables.Classify.ClassifyId + " IN (SELECT " + Tables.ClassifyDeviceAll.ClassifyId + " FROM " + Tables.ClassifyDeviceAll.TableName + " WHERE " + Tables.ClassifyDeviceAll.DeviceId + " IN (" + strId.Substring(1) + "))";
                }
                else
                {
                    return (enableTableName ? Tables.Classify.TableName + "." : "") + Tables.Classify.ClassifyId + " IN (SELECT " + Tables.ClassifyDevice.ClassifyId + " FROM " + Tables.ClassifyDevice.TableName + " WHERE " + Tables.ClassifyDevice.DeviceId + " IN (" + strId.Substring(1) + "))";
                }
            }
            return "";
        }

        /// <summary>
        /// 获取该筛选器所执行的筛选逻辑的文字描述。
        /// </summary>
        /// <returns></returns>
        public string GetFilterDescription()
        {
            if (this.Devices != null && this.Devices.Length > 0)
            {
                List<int> lstId = new List<int>();
                string strName = "";
                foreach (Device _Device in this.Devices)
                {
                    if (lstId.Contains(_Device.DeviceId) == false)
                    {
                        strName += "、" + _Device.Name;
                        lstId.Add(_Device.DeviceId);
                    }
                }

                if (this.ContainsParent)
                {
                    return "与" + strName + "关联的及其所有上级的分类";
                }
                else
                {
                    return "与" + strName + "关联的分类";
                }
            }
            return "";
        }

        /// <summary>
        /// 从保存此筛选器数据的 XML 文档节点集合初始化当前筛选器。
        /// </summary>
        /// <param name="xmlNode">该对象节点的数据集合</param>
        public void SetFilterConfig(System.Xml.XmlNode xmlNode)
        {
        }

        /// <summary>
        /// 获取当前筛选器的状态，将序列化的内容填充至 XmlNode 的 InnerText 属性或者 ChildNodes 子节点中。
        /// </summary>
        /// <param name="xmlDoc">创建 XmlNode 时所需使用到的 System.Xml.XmlDocument。</param>
        public System.Xml.XmlNode GetFilterConfig(System.Xml.XmlDocument xmlDoc)
        {
            return null;
        }

        #endregion
    }
}
