using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.DeviceSearchs
{
    /// <summary>
    /// 按照设备所属分类搜索设备。
    /// </summary>
    public class ClassifyFilter : IDeviceFilter
    {
        /// <summary>
        /// 按照设备所属分类搜索设备。
        /// </summary>
        public ClassifyFilter()
        {
        }

        /// <summary>
        /// 按照设备所属分类搜索设备。
        /// </summary>
        /// <param name="classify">设备所属的分类。</param>
        /// <param name="containsChildren">是否包含所有下级分类中的设备。true:返回当前分类及所有下级分类中关联的设备；false:仅当前分类直接隶属的设备。</param>
        public ClassifyFilter(Classify classify, bool containsChildren)
        {
            this.Classifys = new Classify[] { classify };
            this.ContainsChildren = containsChildren;
        }

        /// <summary>
        /// 按照设备所属分类搜索设备。
        /// </summary>
        /// <param name="classifys">设备所属的分类。</param>
        /// <param name="containsChildren">是否包含所有下级分类中的设备。true:返回当前分类及所有下级分类中关联的设备；false:仅当前分类直接隶属的设备。</param>
        public ClassifyFilter(Classify[] classifys, bool containsChildren)
        {
            this.Classifys = classifys;
            this.ContainsChildren = containsChildren;
        }

        /// <summary>
        /// 按照设备所属分类搜索设备。
        /// </summary>
        /// <param name="classifys">设备所属的分类。</param>
        /// <param name="containsChildren">是否包含所有下级分类中的设备。true:返回当前分类及所有下级分类中关联的设备；false:仅当前分类直接隶属的设备。</param>
        public ClassifyFilter(ICollection<Classify> classifys, bool containsChildren)
        {
            this.Classifys = classifys.ToArray();
            this.ContainsChildren = containsChildren;
        }

        /// <summary>
        /// 设备所属的分类。
        /// </summary>
        public Classify[] Classifys { get; set; }

        /// <summary>
        /// 是否包含所有下级分类中的设备。true:返回当前分类及所有下级分类中关联的设备；false:仅当前分类直接隶属的设备。
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
            foreach (Classify _Classify in this.Classifys)
            {
                if (device.Classifys.Contains(_Classify))
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
            if (this.Classifys != null && this.Classifys.Length > 0)
            {
                List<int> lstId = new List<int>();
                string strId = "";
                foreach (Classify _Classify in this.Classifys)
                {
                    if (lstId.Contains(_Classify.ClassifyId) == false)
                    {
                        strId += "," + _Classify.ClassifyId;
                        lstId.Add(_Classify.ClassifyId);
                    }
                }

                if (this.ContainsChildren)
                {
                    return (enableTableName ? Tables.Device.TableName + "." : "") + Tables.Device.DeviceId + " IN (SELECT " + Tables.ClassifyDeviceAll.DeviceId + " FROM " + Tables.ClassifyDeviceAll.TableName + " WHERE " + Tables.ClassifyDeviceAll.ClassifyId + " IN (" + strId.Substring(1) + "))";
                }
                else
                {
                    return (enableTableName ? Tables.Device.TableName + "." : "") + Tables.Device.DeviceId + " IN (SELECT " + Tables.ClassifyDevice.DeviceId + " FROM " + Tables.ClassifyDevice.TableName + " WHERE " + Tables.ClassifyDevice.ClassifyId + " IN (" + strId.Substring(1) + "))";
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
            if (this.Classifys != null && this.Classifys.Length > 0)
            {
                List<int> lstId = new List<int>();
                string strName = "";
                foreach (Classify _Classify in this.Classifys)
                {
                    if (lstId.Contains(_Classify.ClassifyId) == false)
                    {
                        strName += "、" + _Classify.Name;
                        lstId.Add(_Classify.ClassifyId);
                    }
                }

                if (this.ContainsChildren)
                {
                    return "属于" + strName + "及其所有下级的设备";
                }
                else
                {
                    return "属于" + strName + "的设备";
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
