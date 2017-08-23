using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.DeviceSearchs
{
    /// <summary>
    /// 按照设备所属公共分类搜索设备。
    /// </summary>
    public class CommonCustomClassifyFilter : IDeviceFilter
    {
        private ApplicationClass m_Application;

        /// <summary>
        /// 按照设备所属分类搜索设备。
        /// </summary>
        public CommonCustomClassifyFilter()
        {
        }

        /// <summary>
        /// 按照设备所属分类搜索设备。
        /// </summary>
        /// <param name="classify">设备所属的分类。</param>
        /// <param name="containsChildren">是否包含所有下级分类中的设备。</param>
        public CommonCustomClassifyFilter(CommonCustomClassify classify, bool containsChildren)
        {
            this.Classify = classify;
            this.ContainsChildren = containsChildren;
        }

        /// <summary>
        /// 获取筛选器名称。
        /// </summary>
        public string FilterName { get; private set; }

        /// <summary>
        /// 设备所属的分类。
        /// </summary>
        public CommonCustomClassify Classify { get; set; }

        /// <summary>
        /// 是否包含所有下级分类中的设备。
        /// </summary>
        public bool ContainsChildren { get; set; }

        #region IDeviceFilter 成员

        /// <summary>
        /// 检查传入的设备是否符合该筛选器筛选要求。
        /// </summary>
        /// <param name="device">被检查的设备。</param>
        /// <returns></returns>
        public bool DeviceFilterCheck(Device device)
        {
            if (this.Classify != null)
            {
                if (device.Classifys.Contains(this.Classify))
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
            get { return "搜索属于指定分类的设备"; }
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
            get
            {
                List<string> lstName = new List<string>();

                AC.Base.ClassifySearchs.ClassifySearch _Search = new AC.Base.ClassifySearchs.ClassifySearch(this.m_Application);
                _Search.Filters.Add(new AC.Base.ClassifySearchs.ParentIdFilter(0));
                _Search.Filters.Add(new AC.Base.ClassifySearchs.ClassifyTypeFilter(typeof(CommonCustomClassify)));
                foreach (CommonCustomClassify _Classify in _Search.Search())
                {
                    lstName.Add(_Classify.Name);
                }

                return lstName.ToArray();
            }
        }

        /// <summary>
        /// 设置当前使用的筛选器名称。例如用户自定义分类，筛选器只能有一个，而通过 FilterNamesAttribute 返回诸如地区、行业、类型等参数后，可以将此筛选器变为地区筛选器、行业筛选器、类型筛选器，filterName 参数则是指定当前是地区筛选器还是行业筛选器。
        /// </summary>
        /// <param name="filterName">当前使用的筛选器名称。</param>
        public void SetFilterName(string filterName)
        {
            this.FilterName = filterName;
        }

        /// <summary>
        /// 此筛选器执行的 SQL 语句。
        /// </summary>
        /// <param name="enableTableName">返回的条件语句是否需要附加表名。例如 table.column = 12 或者 column = 12</param>
        /// <returns></returns>
        public string GetFilterSql(bool enableTableName)
        {
            if (this.Classify != null)
            {
                if (this.ContainsChildren)
                {
                    return (enableTableName ? Tables.Device.TableName + "." : "") + Tables.Device.DeviceId + " IN (SELECT " + Tables.ClassifyDeviceAll.DeviceId + " FROM " + Tables.ClassifyDeviceAll.TableName + " WHERE " + Tables.ClassifyDeviceAll.ClassifyId + "=" + this.Classify.ClassifyId + ")";
                }
                else
                {
                    return (enableTableName ? Tables.Device.TableName + "." : "") + Tables.Device.DeviceId + " IN (SELECT " + Tables.ClassifyDevice.DeviceId + " FROM " + Tables.ClassifyDevice.TableName + " WHERE " + Tables.ClassifyDevice.ClassifyId + "=" + this.Classify.ClassifyId + ")";
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
            if (this.Classify != null)
            {
                if (this.ContainsChildren)
                {
                    return this.FilterName + "属于" + this.Classify.Name + "及其所有下级的设备";
                }
                else
                {
                    return this.FilterName + "属于" + this.Classify.Name + "的设备";
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
            int intClassifyId = Function.ToInt(xmlNode.InnerText);
            AC.Base.ClassifySearchs.ClassifySearch _Search = new AC.Base.ClassifySearchs.ClassifySearch(this.m_Application);
            _Search.Filters.Add(new AC.Base.ClassifySearchs.IdFilter(intClassifyId));
            foreach (Classify _Classify in _Search.Search())
            {
                if (_Classify is CommonCustomClassify)
                {
                    this.Classify = _Classify as CommonCustomClassify;

                    AC.Base.Classify parent = this.Classify;
                    while (parent != null)
                    {
                        this.FilterName = parent.Name;
                        parent = parent.Parent;
                    }
                }
            }
        }

        /// <summary>
        /// 获取当前筛选器的状态，将序列化的内容填充至 XmlNode 的 InnerText 属性或者 ChildNodes 子节点中。
        /// </summary>
        /// <param name="xmlDoc">创建 XmlNode 时所需使用到的 System.Xml.XmlDocument。</param>
        /// <returns></returns>
        public System.Xml.XmlNode GetFilterConfig(System.Xml.XmlDocument xmlDoc)
        {
            if (this.Classify != null)
            {
                System.Xml.XmlNode xnConfig = xmlDoc.CreateElement(this.GetType().Name);
                xnConfig.InnerText = this.Classify.ClassifyId.ToString();
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
