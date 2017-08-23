using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AC.Base.ClassifySearchs;

namespace AC.Base
{
    /// <summary>
    /// 设备分类。继承该基类的实体类必须提供一个无参数的构造函数，且必须添加 ClassifyTypeAttribute 特性。
    /// </summary>
    public abstract class Classify
    {
        #region 分类基本信息

        /// <summary>
        /// 应用程序框架。
        /// </summary>
        public ApplicationClass Application { get; private set; }

        /// <summary>
        /// 设置应用程序框架。
        /// </summary>
        /// <param name="application"></param>
        protected internal virtual void SetApplication(ApplicationClass application)
        {
            this.Application = application;
        }

        /// <summary>
        /// 获取当前分类的对象来源集合。该集合内保存了搜索分类时同一批搜索的分类，保留该集合的引用为后续读取分类数据时能够以高效的批量方式将该集合内的数据一次性读取。
        /// </summary>
        public IClassifyCollection Source { get; internal set; }

        private ClassifyType m_ClassifyType;
        /// <summary>
        /// 分类类型。
        /// </summary>
        public ClassifyType ClassifyType
        {
            get
            {
                if (this.m_ClassifyType == null)
                {
                    this.m_ClassifyType = this.Application.ClassifyTypes.GetClassifyType(this.GetType());
                }
                return this.m_ClassifyType;
            }
            internal set
            {
                this.m_ClassifyType = value;
            }
        }

        /// <summary>
        /// 分类编号。
        /// </summary>
        public int ClassifyId { get; internal set; }

        private string m_Name;
        /// <summary>
        /// 获取或设置分类名称。
        /// </summary>
        public string Name
        {
            get
            {
                if (this.m_Name == null)
                {
                    this.m_Name = "";
                }
                return this.m_Name;
            }
            set
            {
                this.m_Name = value;
            }
        }

        /// <summary>
        /// 获取或设置分类快捷码。
        /// </summary>
        public string NameShortcut { get; set; }

        /// <summary>
        /// 获取或设置分类标识码，通常用于与外部关联时的标识。
        /// </summary>
        public string Identifier { get; set; }

        /// <summary>
        /// 获取或设置分类排序序号。
        /// </summary>
        public int OrdinalNumber { get; set; }

        /// <summary>
        /// 获取或设置是否允许将设备设置到该分类下。
        /// </summary>
        public bool EnabledDevice { get; set; }

        /// <summary>
        /// 获取或设置该分类允许设置的设备条件。如果该属性为 null 则表示无限制，任何设备均可设置到该分类下。
        /// </summary>
        public DeviceSearchs.DeviceFilterCollection DeviceFilters { get; set; }

        /// <summary>
        /// 获取当前分类 16*16 像素图标。
        /// </summary>
        /// <returns></returns>
        public virtual System.Drawing.Image GetIcon16()
        {
            return Properties.Resources.Classify16;
        }

        /// <summary>
        /// 获取当前分类 32*32 图标。
        /// </summary>
        /// <returns></returns>
        public virtual System.Drawing.Image GetIcon32()
        {
            return Properties.Resources.Classify32;
        }

        private static bool IconIsSaved = false;
        private void IconSave()
        {
            if (IconIsSaved == false)
            {
                this.GetIcon16().Save(this.Application.TemporaryDirectory + this.GetType().Name + "16.gif");
                this.GetIcon32().Save(this.Application.TemporaryDirectory + this.GetType().Name + "32.gif");
            }
        }

        /// <summary>
        /// 获取当前分类 16*16 像素的图标路径。
        /// </summary>
        /// <returns></returns>
        public virtual string GetIcon16Url()
        {
            this.IconSave();
            return this.Application.TemporaryDirectoryRelativePath + this.GetType().Name + "16.gif";
        }

        /// <summary>
        /// 获取当前分类 32*32像素的图标路径。
        /// </summary>
        /// <returns></returns>
        public virtual string GetIcon32Url()
        {
            this.IconSave();
            return this.Application.TemporaryDirectoryRelativePath + this.GetType().Name + "32.gif";
        }

        /// <summary>
        /// 从保存此分类数据的 XML 文档节点初始化当前分类。
        /// </summary>
        /// <param name="classifyConfig">该分类对象节点的数据</param>
        public virtual void SetClassifyConfig(System.Xml.XmlNode classifyConfig)
        {
        }

        /// <summary>
        /// 获取当前分类的配置信息，将序列化的内容填充至 XmlNode 的 InnerText 属性或者 ChildNodes 子节点中。
        /// </summary>
        /// <param name="xmlDoc">创建 XmlNode 时所需使用到的 System.Xml.XmlDocument。</param>
        /// <returns>如无配置内容则返回 null。</returns>
        public virtual System.Xml.XmlNode GetClassifyConfig(System.Xml.XmlDocument xmlDoc)
        {
            return null;
        }

        #endregion

        #region 数据读写

        /// <summary>
        /// 设置分类数据集。
        /// </summary>
        /// <param name="dr"></param>
        protected internal virtual void SetDataReader(System.Data.IDataReader dr)
        {
            this.ClassifyId = Function.ToInt(dr[Tables.Classify.ClassifyId]);
            this.m_ParentId = Function.ToInt(dr[Tables.Classify.ParentId]);
            this.Name = Function.ToString(dr[Tables.Classify.Name]);
            this.NameShortcut = Function.ToString(dr[Tables.Classify.NameShortcut]);
            this.Identifier = Function.ToString(dr[Tables.Classify.Identifier]);
            this.OrdinalNumber = Function.ToInt(dr[Tables.Classify.OrdinalNumber]);

            object objConfig = dr[Tables.Classify.XMLConfig];
            if (objConfig != null && !(objConfig is System.DBNull))
            {
                System.Xml.XmlDocument xmlDoc = new System.Xml.XmlDocument();
                try
                {
                    xmlDoc.LoadXml(objConfig.ToString());

                    if (xmlDoc.ChildNodes.Count > 0)
                    {
                        foreach (System.Xml.XmlNode xnConfigItem in xmlDoc.ChildNodes[0].ChildNodes)
                        {
                            switch (xnConfigItem.Name)
                            {
                                case "EnabledDevice":
                                    this.EnabledDevice = Function.ToBool(xnConfigItem.InnerText);
                                    break;

                                case "DeviceFilters":
                                    if (xnConfigItem.ChildNodes.Count > 0)
                                    {
                                        this.DeviceFilters = new DeviceSearchs.DeviceFilterCollection();
                                        this.DeviceFilters.SetApplication(this.Application);
                                        this.DeviceFilters.SetFilterConfig(xnConfigItem.ChildNodes[0]);
                                    }
                                    break;

                                default:
                                    this.SetClassifyConfig(xnConfigItem);
                                    break;
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }

        /// <summary>
        /// 保存分类。
        /// </summary>
        public virtual void Save()
        {
            if (this.Name == null || this.Name.Trim().Length == 0)
            {
                throw new Exception("未设置分类名称。");
            }
            else
            {
                //配置信息
                System.Xml.XmlDocument xmlDoc = new System.Xml.XmlDocument();
                System.Xml.XmlNode xnConfig = xmlDoc.CreateElement(this.GetType().Name);
                xmlDoc.AppendChild(xnConfig);

                System.Xml.XmlNode xnEnabledDevice = xmlDoc.CreateElement("EnabledDevice");
                xnEnabledDevice.InnerText = Function.BoolToByte(this.EnabledDevice).ToString();
                xnConfig.AppendChild(xnEnabledDevice);

                if (this.DeviceFilters != null && this.DeviceFilters.Count > 0)
                {
                    System.Xml.XmlNode xnDeviceFilters = xmlDoc.CreateElement("DeviceFilters");
                    xnDeviceFilters.AppendChild(this.DeviceFilters.GetFilterConfig(xmlDoc));
                    xnConfig.AppendChild(xnDeviceFilters);
                }

                System.Xml.XmlNode xnClassifyConfig = this.GetClassifyConfig(xmlDoc);
                if (xnClassifyConfig != null)
                {
                    xnConfig.AppendChild(xnClassifyConfig);
                }

                string strConfig = xmlDoc.OuterXml;

                //插入或更新
                string strSql;
                if (this.ClassifyId == 0)
                {
                    AC.Base.Database.DbConnection dbConn = this.Application.GetDbConnection();
                    if (dbConn != null)
                    {
                        strSql = "SELECT MAX(" + Tables.Classify.ClassifyId + ") FROM " + Tables.Classify.TableName;
                        this.ClassifyId = Function.ToInt(dbConn.ExecuteScalar(strSql)) + 1;

                        try
                        {
                            if (this.OrdinalNumber == 0)
                            {
                                this.OrdinalNumber = this.ClassifyId;
                            }

                            strSql = this.ClassifyId + "," + Function.SqlStr(this.ClassifyType.Code, 250) + "," + (this.Parent == null ? 0 : this.Parent.ClassifyId) + "," + Function.SqlStr(this.Name, 250) + "," + Function.SqlStr(this.NameShortcut, 250) + "," + Function.SqlStr(this.Identifier, 250) + "," + this.OrdinalNumber + "," + Function.SqlStr(strConfig);
                            strSql = "INSERT INTO " + Tables.Classify.TableName + " (" + Tables.Classify.ClassifyId + "," + Tables.Classify.ClassifyType + "," + Tables.Classify.ParentId + "," + Tables.Classify.Name + "," + Tables.Classify.NameShortcut + "," + Tables.Classify.Identifier + "," + Tables.Classify.OrdinalNumber + "," + Tables.Classify.XMLConfig + ") VALUES (" + strSql + ")";
                            dbConn.ExecuteNonQuery(strSql);

                            if (this.Parent != null)
                            {
                                this.Parent.Children.Add(this);
                            }
                        }
                        finally
                        {
                            dbConn.Close();
                        }
                    }
                }
                else
                {
                    AC.Base.Database.DbConnection dbConn = this.Application.GetDbConnection();
                    if (dbConn != null)
                    {
                        try
                        {
                            strSql = "UPDATE " + Tables.Classify.TableName + " Set " + Tables.Classify.Name + "=" + Function.SqlStr(this.Name, 250) + "," + Tables.Device.NameShortcut + "=" + Function.SqlStr(this.NameShortcut, 250) + "," + Tables.Classify.Identifier + "=" + Function.SqlStr(this.Identifier, 250) + "," + Tables.Classify.OrdinalNumber + "=" + this.OrdinalNumber + "," + Tables.Classify.XMLConfig + "=" + Function.SqlStr(strConfig) + " Where " + Tables.Classify.ClassifyId + "=" + this.ClassifyId;
                            dbConn.ExecuteNonQuery(strSql);
                        }
                        finally
                        {
                            dbConn.Close();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 删除分类。
        /// </summary>
        public virtual void Delete()
        {
            foreach (Classify children in this.Children)
            {
                children.Delete();
            }

            AC.Base.Database.DbConnection dbConn = this.Application.GetDbConnection();
            if (dbConn != null)
            {
                try
                {
                    string strSql;

                    strSql = "Delete From " + Tables.ClassifyDeviceAll.TableName + " Where " + Tables.ClassifyDeviceAll.ClassifyId + "=" + this.ClassifyId;
                    dbConn.ExecuteNonQuery(strSql);

                    strSql = "Delete From " + Tables.ClassifyDevice.TableName + " Where " + Tables.ClassifyDevice.ClassifyId + "=" + this.ClassifyId;
                    dbConn.ExecuteNonQuery(strSql);

                    strSql = "Delete From " + Tables.Classify.TableName + " Where " + Tables.Classify.ClassifyId + "=" + this.ClassifyId;
                    dbConn.ExecuteNonQuery(strSql);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    dbConn.Close();
                }
            }
        }

        /// <summary>
        /// 确定指定的对象是否等于当前的对象。
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is Classify && this.ClassifyId > 0)
            {
                Classify classify = obj as Classify;
                if (classify.ClassifyId > 0 && classify.ClassifyId == this.ClassifyId)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 用作特定类型的哈希函数。
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// 获取当前对象的字符串表示形式。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (this.ClassifyId > 0)
            {
                return "[" + Function.StringInterceptFill(this.ClassifyId.ToString(), 6, "0", false) + "]" + Function.OutString(this.Name, "", " ", "");
            }
            else
            {
                return "[新分类]" + (this.Name.Length > 0 ? " " + this.Name : "");
            }
        }

        #endregion

        #region 分类级联关系

        /// <summary>
        /// 获取当前分类树节点的层数（从零开始）。
        /// </summary>
        /// <returns></returns>
        public int GetLevel()
        {
            if (this.Parent == null)
            {
                return 0;
            }
            else
            {
                return this.Parent.GetLevel() + 1;
            }
        }

        private int m_ParentId = 0;
        private Classify m_Parent;
        /// <summary>
        /// 获取上级分类，如果返回 null 则表示当前分类是根节点分类。
        /// </summary>
        public Classify Parent
        {
            get
            {
                if (this.m_ParentId == 0)
                {
                    return null;
                }
                else
                {
                    if (this.m_Parent == null)
                    {
                        //读取上级分类
                        if (this.Source != null && this.Source.Count > 1)
                        {
                            ClassifySearch ds = new ClassifySearch(this.Application);
                            List<int> lstParentId = new List<int>();
                            foreach (Classify classify in this.Source)
                            {
                                if (classify.m_ParentId > 0 && lstParentId.Contains(classify.m_ParentId) == false)
                                {
                                    lstParentId.Add(classify.m_ParentId);
                                }
                            }
                            ds.Filters.Add(new IdFilter(lstParentId));

                            foreach (Classify parentClassify in ds.Search(1, false, true, false))
                            {
                                foreach (Classify classify in this.Source)
                                {
                                    if (classify.m_ParentId == parentClassify.ClassifyId)
                                    {
                                        classify.SetParent(parentClassify);
                                    }
                                }
                            }
                        }
                        else
                        {
                            ClassifySearch ds = new ClassifySearch(this.Application);
                            ds.Filters.Add(new IdFilter(this.m_ParentId));

                            foreach (Classify parentClassify in ds.Search(1, false, false, false))
                            {
                                this.SetParent(parentClassify);
                            }
                        }
                    }
                    return this.m_Parent;
                }
            }
        }

        internal void SetParent(Classify parent)
        {
            if (parent != null)
            {
                this.m_ParentId = parent.ClassifyId;
            }
            else
            {
                this.m_ParentId = 0;
            }
            this.m_Parent = parent;
        }

        private ClassifyChildrenCollection m_Children;
        /// <summary>
        /// 获取下级子分类集合。
        /// </summary>
        public ClassifyChildrenCollection Children
        {
            get
            {
                if (this.m_Children == null)
                {
                    this.m_Children = new ClassifyChildrenCollection(this);
                }
                return this.m_Children;
            }
        }

        #endregion

        #region 集合对象

        /// <summary>
        /// 某一分类的下级子分类集合。
        /// </summary>
        public class ClassifyChildrenCollection : IClassifyCollection
        {
            private Classify m_Parent;
            private ClassifyCollection m_Children;
            private int m_Count = -1;               //下级分类的总数量

            internal ClassifyChildrenCollection(Classify parent)
            {
                this.m_Parent = parent;
            }

            internal void Add(Classify item)
            {
                if (this.m_Children != null)
                {
                    item.SetParent(this.m_Parent);
                    this.m_Children.Add(item);
                }

                if (this.m_Count > -1)
                {
                    this.m_Count++;
                }
            }

            internal void Remove(Classify item)
            {
                if (this.m_Children != null)
                {
                    for (int intIndex = 0; intIndex < this.m_Children.Count; intIndex++)
                    {
                        if (this.m_Children[intIndex].Equals(item))
                        {
                            this.m_Children[intIndex].SetParent(null);
                            this.m_Children.RemoveAt(intIndex);
                            break;
                        }
                    }
                }

                if (this.m_Count > -1)
                {
                    this.m_Count--;
                }
            }

            #region IList<Classify> 成员

            /// <summary>
            /// 搜索指定的分类，并返回第一个匹配项的从零开始的索引。
            /// </summary>
            /// <param name="item">要在集合中查找的分类。</param>
            /// <returns>如果在整个子分类集合中找到 item 的第一个匹配项，则为该项的从零开始的索引；否则为 -1。</returns>
            public int IndexOf(Classify item)
            {
                if (this.m_Children == null)
                {
                    this.LoadChildren();
                }

                for (int intIndex = 0; intIndex < this.Count; intIndex++)
                {
                    if (this[intIndex].Equals(item))
                    {
                        return intIndex;
                    }
                }
                return -1;
            }

            /// <summary>
            /// 获取指定索引处的分类。
            /// </summary>
            /// <param name="index"></param>
            /// <returns></returns>
            public Classify this[int index]
            {
                get
                {
                    if (index < 0)
                    {
                        throw new Exception("子分类索引不得小于0。");
                    }

                    if (this.m_Children == null)
                    {
                        this.LoadChildren();
                    }
                    return this.m_Children[index];
                }
            }

            private void LoadChildren()
            {
                if (this.m_Parent.Source != null && this.m_Parent.Source.Count > 1)
                {
                    foreach (Classify classifyParent in this.m_Parent.Source)
                    {
                        classifyParent.Children.m_Children = new ClassifyCollection(true);
                    }

                    //批量读取子分类
                    ClassifySearch ds = new ClassifySearch(this.m_Parent.Application);
                    ds.Filters.Add(new ParentIdFilter(this.m_Parent.Source.GetIdForArray()));

                    ds.OrderInfos.Add(false, new OrdinalNumberOrder());
                    ds.OrderInfos.Add(false, new IdOrder());

                    foreach (Classify classifyChildren in ds.Search(1, false, false, false))
                    {
                        foreach (Classify classifyParent in this.m_Parent.Source)
                        {
                            if (classifyChildren.m_ParentId == classifyParent.ClassifyId)
                            {
                                classifyParent.m_Children.Add(classifyChildren);
                                break;
                            }
                        }
                    }

                    foreach (Classify classifyParent in this.m_Parent.Source)
                    {
                        classifyParent.Children.m_Count = classifyParent.Children.m_Children.Count;
                    }
                }
                else
                {
                    ClassifySearch ds = new ClassifySearch(this.m_Parent.Application);
                    ds.Filters.Add(new ParentIdFilter(this.m_Parent.ClassifyId));

                    ds.OrderInfos = new Searchs.SearchOrderInfoCollection<IClassifyOrder>();
                    ds.OrderInfos.Add(false, new OrdinalNumberOrder());
                    ds.OrderInfos.Add(false, new IdOrder());

                    this.m_Children = ds.Search();
                    this.m_Count = ds.RecordsetCount;

                    foreach (Classify classify in this.m_Children)
                    {
                        classify.SetParent(this.m_Parent);
                    }
                }
            }

            #endregion

            #region ICollection<Classify> 成员

            /// <summary>
            /// 确定指定的分类是否在该子集合中。
            /// </summary>
            /// <param name="item"></param>
            /// <returns></returns>
            public bool Contains(Classify item)
            {
                foreach (Classify _Classify in this)
                {
                    if (_Classify.Equals(item))
                    {
                        return true;
                    }
                }
                return false;
            }

            /// <summary>
            /// 将整个子集合的分类复制到兼容的一维数组中，从目标数组的指定索引位置开始放置。
            /// </summary>
            /// <param name="array"></param>
            /// <param name="arrayIndex"></param>
            public void CopyTo(Classify[] array, int arrayIndex)
            {
                if (this.m_Children == null)
                {
                    this.LoadChildren();
                }

                for (int intIndex = 0; intIndex < this.Count; intIndex++)
                {
                    array[intIndex + arrayIndex] = this[intIndex];
                }
            }

            /// <summary>
            /// 获取当前集合子分类的总数。
            /// </summary>
            public int Count
            {
                get
                {
                    if (this.m_Count == -1)
                    {
                        AC.Base.Database.DbConnection dbConn = this.m_Parent.Application.GetDbConnection();
                        if (dbConn != null)
                        {
                            if (this.m_Parent.Source != null && this.m_Parent.Source.Count > 1)
                            {
                                string strIds = this.m_Parent.Source.GetIdForString();
                                string strSql = "SELECT " + Tables.Classify.ParentId + ",COUNT(*) AS COUNT_NUM FROM " + Tables.Classify.TableName + " WHERE " + Tables.Classify.ParentId + " IN (" + strIds + ") GROUP BY " + Tables.Classify.ParentId;
                                System.Data.IDataReader dr = dbConn.ExecuteReader(strSql);
                                while (dr.Read())
                                {
                                    int intClassifyId = Convert.ToInt32(dr[Tables.Classify.ParentId]);

                                    foreach (Classify classify in this.m_Parent.Source)
                                    {
                                        if (classify.ClassifyId == intClassifyId)
                                        {
                                            classify.Children.m_Count = Convert.ToInt32(dr["COUNT_NUM"]);
                                            break;
                                        }
                                    }
                                }
                                dr.Close();

                                foreach (Classify classify in this.m_Parent.Source)
                                {
                                    if (classify.Children.m_Count == -1)
                                    {
                                        classify.Children.m_Count = 0;            //将无下级分类的 Count 设为 0。
                                    }
                                }
                            }
                            else
                            {
                                string strSql = "SELECT COUNT(*) FROM " + Tables.Classify.TableName + " WHERE " + Tables.Classify.ParentId + "=" + this.m_Parent.ClassifyId;
                                this.m_Count = Function.ToInt(dbConn.ExecuteScalar(strSql));
                            }

                            dbConn.Close();
                        }
                        else
                        {
                            this.m_Count = 0;
                        }
                    }
                    return this.m_Count;
                }
            }

            /// <summary>
            /// 该集合是否为只读集合。
            /// </summary>
            public bool IsReadOnly
            {
                get { return true; }
            }

            #endregion

            #region IEnumerable<Classify> 成员

            /// <summary>
            /// 返回循环访问分类集合的枚举器。
            /// </summary>
            /// <returns></returns>
            public IEnumerator<Classify> GetEnumerator()
            {
                if (this.m_Children == null)
                {
                    this.LoadChildren();
                }

                return this.m_Children.GetEnumerator();
            }

            #endregion

            #region IEnumerable 成员

            /// <summary>
            /// 返回循环访问分类集合的枚举器。
            /// </summary>
            /// <returns></returns>
            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                if (this.m_Children == null)
                {
                    this.LoadChildren();
                }

                return this.m_Children.GetEnumerator();
            }

            #endregion

            /// <summary>
            /// 获取当前对象的字符串表示形式。
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                return "共 " + this.Count + " 个子分类";
            }

            #region IClassifyCollection 成员

            /// <summary>
            /// 获取该集合中指定 ID 的分类。
            /// </summary>
            /// <param name="classifyId">分类 ID。</param>
            /// <returns>返回集合中指定 ID 的分类，如果未查找到该分类则返回 null。</returns>
            public Classify GetClassify(int classifyId)
            {
                foreach (Classify classify in this)
                {
                    if (classify.ClassifyId == classifyId)
                    {
                        return classify;
                    }
                }
                return null;
            }

            /// <summary>
            /// 获取该集合中符合筛选条件的分类集合。
            /// </summary>
            /// <param name="filters">分类筛选条件。</param>
            /// <returns></returns>
            public ClassifyCollection GetClassifys(ClassifyFilterCollection filters)
            {
                ClassifyCollection classifys = new ClassifyCollection(false);

                foreach (Classify classify in this)
                {
                    if (filters.ClassifyFilterCheck(classify))
                    {
                        classifys.Add(classify);
                    }
                }

                return classifys;
            }

            /// <summary>
            /// 获取当前集合内所有的分类编号。
            /// </summary>
            /// <returns></returns>
            public int[] GetIdForArray()
            {
                if (this.m_Children == null)
                {
                    this.LoadChildren();
                }

                int[] intIds = new int[this.Count];

                for (int intIndex = 0; intIndex < this.Count; intIndex++)
                {
                    intIds[intIndex] = this[intIndex].ClassifyId;
                }

                return intIds;
            }

            /// <summary>
            /// 获取当前集合内所有的分类编号。
            /// </summary>
            /// <returns></returns>
            public ICollection<int> GetIdForCollection()
            {
                if (this.m_Children == null)
                {
                    this.LoadChildren();
                }

                List<int> lstIds = new List<int>();

                for (int intIndex = 0; intIndex < this.Count; intIndex++)
                {
                    lstIds[intIndex] = this[intIndex].ClassifyId;
                }

                return lstIds;
            }

            /// <summary>
            /// 获取当前集合内所有的分类编号。
            /// </summary>
            /// <returns>以逗号分隔的字符串形式的分类编号。</returns>
            public string GetIdForString()
            {
                return this.GetIdForString(",");
            }

            /// <summary>
            /// 获取当前集合内所有的分类编号。
            /// </summary>
            /// <param name="separator">分隔各分类编号的字符。</param>
            /// <returns>以指定字符分隔的字符串形式的分类编号。</returns>
            public string GetIdForString(string separator)
            {
                string strIds = "";

                foreach (Classify classify in this)
                {
                    strIds += separator + classify.ClassifyId.ToString();
                }

                if (strIds.Length > 0)
                {
                    strIds = strIds.Substring(separator.Length);
                }

                return strIds;
            }

            #endregion
        }

        #endregion

        #region 分类与设备关系

        internal int m_DeviceCount = -1;
        /// <summary>
        /// 获取直接属于该分类的设备数量。
        /// </summary>
        public int DeviceCount
        {
            get
            {
                if (this.m_DeviceCount == -1)
                {
                    if (this.Source != null && this.Source.Count > 1)
                    {
                        foreach (Classify classify in this.Source)
                        {
                            classify.m_DeviceCount = 0;
                        }

                        AC.Base.Database.DbConnection dbConn = this.Application.GetDbConnection();
                        if (dbConn != null)
                        {
                            try
                            {
                                string strSql = "SELECT " + Tables.ClassifyDevice.TableName + "." + Tables.ClassifyDevice.ClassifyId + ",COUNT(*) AS COUNT_NUM FROM " + Tables.Device.TableName + "," + Tables.ClassifyDevice.TableName + " WHERE " + Tables.Device.TableName + "." + Tables.Device.DeviceId + "=" + Tables.ClassifyDevice.TableName + "." + Tables.ClassifyDevice.DeviceId + " AND " + Tables.ClassifyDevice.TableName + "." + Tables.ClassifyDevice.ClassifyId + " IN (" + this.Source.GetIdForString() + ") GROUP BY " + Tables.ClassifyDevice.TableName + "." + Tables.ClassifyDevice.ClassifyId;
                                System.Data.IDataReader dr = dbConn.ExecuteReader(strSql);
                                while (dr.Read())
                                {
                                    int intClassifyId = Convert.ToInt32(dr[Tables.ClassifyDevice.ClassifyId]);
                                    foreach (Classify classify in this.Source)
                                    {
                                        if (classify.ClassifyId == intClassifyId)
                                        {
                                            classify.m_DeviceCount = Function.ToInt(dr["COUNT_NUM"]);
                                        }
                                    }
                                }
                                dr.Close();
                            }
                            catch (Exception ex)
                            {
                                throw ex;
                            }
                            finally
                            {
                                dbConn.Close();
                            }
                        }
                    }
                    else
                    {
                        this.m_DeviceCount = 0;

                        AC.Base.Database.DbConnection dbConn = this.Application.GetDbConnection();
                        if (dbConn != null)
                        {
                            try
                            {
                                string strSql = "SELECT COUNT(*) AS COUNT_NUM FROM " + Tables.Device.TableName + "," + Tables.ClassifyDevice.TableName + " WHERE " + Tables.Device.TableName + "." + Tables.Device.DeviceId + "=" + Tables.ClassifyDevice.TableName + "." + Tables.ClassifyDevice.DeviceId + " AND " + Tables.ClassifyDevice.TableName + "." + Tables.ClassifyDevice.ClassifyId + "=" + this.ClassifyId;
                                System.Data.IDataReader dr = dbConn.ExecuteReader(strSql);
                                if (dr.Read())
                                {
                                    this.m_DeviceCount = Function.ToInt(dr["COUNT_NUM"]);
                                }
                                dr.Close();
                            }
                            catch (Exception ex)
                            {
                                throw ex;
                            }
                            finally
                            {
                                dbConn.Close();
                            }
                        }
                    }
                }
                return this.m_DeviceCount;
            }
        }

        /// <summary>
        /// 获取属于该分类的设备。
        /// </summary>
        /// <returns></returns>
        public DeviceCollection GetDevices()
        {
            DeviceSearchs.DeviceSearch _Search = new DeviceSearchs.DeviceSearch(this.Application);
            _Search.Filters.Add(new DeviceSearchs.ClassifyFilter(this, false));
            return _Search.Search();
        }

        /// <summary>
        /// 根据附加的筛选条件搜索该分类下的设备。
        /// </summary>
        /// <param name="filters">筛选条件。</param>
        /// <param name="containsChildren">是否包含所有下级分类中的设备。true:返回当前分类及所有下级分类中关联的设备；false:仅当前分类直接隶属的设备。</param>
        /// <returns></returns>
        public DeviceCollection GetDevices(AC.Base.DeviceSearchs.DeviceFilterCollection filters, bool containsChildren)
        {
            DeviceSearchs.DeviceSearch _Search = new DeviceSearchs.DeviceSearch(this.Application);
            _Search.Filters.Add(new DeviceSearchs.ClassifyFilter(this, containsChildren));
            if (filters != null && filters.Count > 0)
            {
                _Search.Filters.Add(filters);
            }
            return _Search.Search();
        }

        #endregion
    }
}
