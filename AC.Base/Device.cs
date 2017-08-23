using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AC.Base.DeviceSearchs;
using AC.Base.Drives;
using System.Collections;

namespace AC.Base
{
    /// <summary>
    /// 设备创建后产生的事件所调用的委托。
    /// </summary>
    /// <param name="parentDevice">被创建设备的上级设备，如果创建的是根节点设备则此参数为null。</param>
    /// <param name="createdDevice">被创建的设备。</param>
    public delegate void DeviceCreatedEventHandler(Device parentDevice, Device createdDevice);

    /// <summary>
    /// 设备档案、属性信息更改后产生的事件所调用的委托。
    /// </summary>
    /// <param name="device">产生档案更改事件的设备。</param>
    public delegate void DeviceUpdatedEventHandler(Device device);

    /// <summary>
    /// 设备删除后产生的事件所调用的委托。
    /// </summary>
    /// <param name="device">已被删除的设备。</param>
    public delegate void DeviceDeletedEventHandler(Device device);

    /// <summary>
    /// 表示一个具有通讯、数据运算、数据处理等能力的设备。继承该基类的实体类必须提供一个无参数的构造函数，且必须添加 DeviceTypeAttribute 特性。
    /// </summary>
    public abstract class Device
    {
        #region << 设备基本信息 >>

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
        /// 获取当前设备的对象来源集合。该集合内保存了搜索设备时同一批搜索的设备，保留该集合的引用为后续读取设备数据时能够以高效的批量方式将该集合内的数据一次性读取。
        /// </summary>
        public IDeviceCollection Source { get; internal set; }

        private DeviceType m_DeviceType;
        /// <summary>
        /// 设备类型。
        /// </summary>
        public DeviceType DeviceType
        {
            get
            {
                Type t = this.GetType();
                if (m_DeviceType == null)
                {
                    m_DeviceType = this.Application.DeviceTypeSort.GetDeviceType(this.GetType());
                }
                return m_DeviceType;
            }
            internal set
            {
                this.m_DeviceType = value;
            }
        }

        /// <summary>
        /// 获取设备编号。
        /// </summary>
        public int DeviceId { get; private set; }

        private string m_Name;
        /// <summary>
        /// 获取或设置设备名称。
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
        /// 获取或设置设备快捷码。
        /// </summary>
        public string NameShortcut { get; set; }

        /// <summary>
        /// 设备通讯地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 获取或设置设备标识码、条形码、资产编号等。
        /// </summary>
        public string Identifier { get; set; }

        /// <summary>
        /// 获取或设置设备状态。默认为“运行”。
        /// </summary>
        public DeviceStateOptions State { get; set; }

        /// <summary>
        /// 获取或设置设备状态描述。用于对设备状态做进一步描述。
        /// </summary>
        public string StateDescription { get; set; }

        /// <summary>
        /// 获取或设置设备排序序号。
        /// </summary>
        public int OrdinalNumber { get; set; }

        /// <summary>
        /// 获取或设置设备地理坐标的经度值。
        /// </summary>
        public decimal Longitude { get; set; }

        /// <summary>
        /// 获取或设置设备地理坐标的纬度值。
        /// </summary>
        public decimal Latitude { get; set; }

        /// <summary>
        /// 获取当前设备16*16像素含设备状态及在线情况的实时图标。
        /// </summary>
        /// <returns></returns>
        public System.Drawing.Image GetIcon16()
        {
            return this.DeviceType.GetIcon16(this.State);
        }

        /// <summary>
        /// 获取当前设备32*32像素含设备状态及在线情况的实时图标。
        /// </summary>
        /// <returns></returns>
        public System.Drawing.Image GetIcon32()
        {
            return this.DeviceType.GetIcon32(this.State);
        }

        private static List<string> st_lstIconUrl = new List<string>();

        /// <summary>
        /// 获取当前设备 16*16 像素含设备状态的图标路径。
        /// </summary>
        /// <returns></returns>
        public string GetIcon16Url()
        {
            string strName = this.GetType().Name + (int)this.State + "16.gif";
            if (st_lstIconUrl.Contains(strName) == false)
            {
                System.Drawing.Image img = this.DeviceType.GetIcon16(this.State);
                img.Save(this.Application.TemporaryDirectory + strName);
                st_lstIconUrl.Add(strName);
            }
            return this.Application.TemporaryDirectoryRelativePath + strName;
        }

        /// <summary>
        /// 获取当前设备 32*32 像素含设备状态的图标路径。
        /// </summary>
        /// <returns></returns>
        public string GetIcon32Url()
        {
            string strName = this.GetType().Name + (int)this.State + "32.gif";
            if (st_lstIconUrl.Contains(strName) == false)
            {
                System.Drawing.Image img = this.DeviceType.GetIcon16(this.State);
                img.Save(this.Application.TemporaryDirectory + strName);
                st_lstIconUrl.Add(strName);
            }
            return this.Application.TemporaryDirectoryRelativePath + strName;
        }

        /// <summary>
        /// 从保存此设备数据的 XML 文档节点初始化当前设备。继承的类应注意：该方法在设备对象创建之后可能仍会多次调用，如刷新设备档案时不会新创建该对象，而是调用 Reload 方法直接从数据库取出数据后赋值，所以对于集合一类的属性应先清空集合再进行添加操作。
        /// </summary>
        /// <param name="deviceConfig">该设备对象节点的数据</param>
        protected virtual void SetDeviceConfig(System.Xml.XmlNode deviceConfig)
        {
        }

        /// <summary>
        /// 获取当前设备的配置信息，将序列化的内容填充至 XmlNode 的 InnerText 属性或者 ChildNodes 子节点中。
        /// </summary>
        /// <param name="xmlDoc">创建 XmlNode 时所需使用到的 System.Xml.XmlDocument。</param>
        /// <returns>如无配置内容则返回 null。</returns>
        protected virtual System.Xml.XmlNode GetDeviceConfig(System.Xml.XmlDocument xmlDoc)
        {
            return null;
        }

        #endregion

        #region << 档案存取 >>

        /// <summary>
        /// 重新载入设备档案。
        /// </summary>
        public void Reload()
        {
        }

        /// <summary>
        /// 设置设备状态
        /// </summary>
        /// <param name="dso"></param>
        public void SetState(DeviceStateOptions dso)
        {
            if (this.State != dso)
            {
                this.State = dso;
                this.OnUpdated(false);
            }
        }
        /// <summary>
        /// 设置设备数据集。
        /// </summary>
        /// <param name="dr"></param>
        protected internal virtual void SetDataReader(System.Data.IDataReader dr)
        {
            this.DeviceId = Function.ToInt(dr[Tables.Device.DeviceId]);
            this.m_ParentId = Function.ToInt(dr[Tables.Device.ParentId]);
            this.Name = Function.ToString(dr[Tables.Device.Name]);
            this.NameShortcut = Function.ToString(dr[Tables.Device.NameShortcut]);
            this.Address = Function.ToString(dr[Tables.Device.DeviceAddress]);
            this.Identifier = Function.ToString(dr[Tables.Device.Identifier]);
            this.State = (DeviceStateOptions)Function.ToInt(dr[Tables.Device.StateOption]);
            this.StateDescription = Function.ToString(dr[Tables.Device.StateDescription]);
            this.OrdinalNumber = Function.ToInt(dr[Tables.Device.OrdinalNumber]);
            this.Longitude = Function.ToDecimal(dr[Tables.Device.Longitude]);
            this.Latitude = Function.ToDecimal(dr[Tables.Device.Latitude]);

            object objConfig = dr[Tables.Device.XMLConfig];
            if (objConfig != null && !(objConfig is System.DBNull))
            {
                System.Xml.XmlDocument xmlDoc = new System.Xml.XmlDocument();
                try
                {
                    xmlDoc.LoadXml(objConfig.ToString());

                    if (xmlDoc.ChildNodes.Count > 0)
                    {
                        foreach (System.Xml.XmlNode xnConfig in xmlDoc.ChildNodes[0].ChildNodes)
                        {
                            if (xnConfig.Name.Equals("Device"))
                            {
                                if (xnConfig.ChildNodes.Count > 0)
                                {
                                    this.SetDeviceConfig(xnConfig.ChildNodes[0]);
                                }
                            }
                            else if (xnConfig.Name.Equals("Drive") && this.Drive != null)
                            {
                                if (xnConfig.ChildNodes.Count > 0)
                                {
                                    this.m_DriveConfig = xnConfig.ChildNodes[0];
                                    this.Drive.SetDriveConfig(xnConfig.ChildNodes[0]);
                                }
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
        /// 保存设备档案。
        /// </summary>
        public virtual void Save()
        {
           
            if (this.Name == null || this.Name.Trim().Length == 0)
            {
                throw new Exception("未设置设备名称。");
            }
            else
                this.Save(0);
        }

        private void Save(int tryCount)
        {
            if (tryCount >= 3)
            {
                throw new Exception("保存设备档案时发生错误，已重试3次。");
            }

            //配置信息
            string strConfig = null;
            System.Xml.XmlDocument xmlDoc = new System.Xml.XmlDocument();
            System.Xml.XmlNode xnConfig = xmlDoc.CreateElement(this.GetType().Name);
            xmlDoc.AppendChild(xnConfig);

            System.Xml.XmlNode xnDeviceConfig = this.GetDeviceConfig(xmlDoc);
            if (xnDeviceConfig != null)
            {
                System.Xml.XmlNode xn = xmlDoc.CreateElement("Device");
                xn.AppendChild(xnDeviceConfig);
                xnConfig.AppendChild(xn);
            }

            if (this.Drive != null)
            {
                System.Xml.XmlNode xnDriveConfig = this.Drive.GetDriveConfig(xmlDoc);
                if (xnDriveConfig != null)
                {
                    System.Xml.XmlNode xn = xmlDoc.CreateElement("Drive");
                    xn.AppendChild(xnDriveConfig);
                    xnConfig.AppendChild(xn);
                }
            }

            if (xnConfig.ChildNodes.Count > 0)
            {
                strConfig = xmlDoc.OuterXml;
            }

            //插入或更新
            string strSql;
            if (this.DeviceId == 0)
            {
                AC.Base.Database.DbConnection dbConn = this.Application.GetDbConnection();
                if (dbConn != null)
                {
                    strSql = "SELECT MAX(" + Tables.Device.DeviceId + ") FROM " + Tables.Device.TableName;
                    this.DeviceId = Function.ToInt(dbConn.ExecuteScalar(strSql)) + 1;

                    try
                    {
                        if (this.NameShortcut == null)
                        {
                            this.NameShortcut = Function.GetChineseSpell(this.Name, false, false);
                        }

                        int intParentId = 0;
                        if (this.Parent != null)
                        {
                            if (this.Parent.DeviceId == 0)
                            {
                                throw new Exception("保存当前设备前必须首先保存上级设备。");
                            }
                            intParentId = this.Parent.DeviceId;
                        }
                        else if (this.NameShortcut == "#chl")
                        {
                            intParentId = -1;
                        }


                        if (this.OrdinalNumber == 0)
                        {
                            this.OrdinalNumber = this.DeviceId;
                        }

                        strSql = this.DeviceId + "," + Function.SqlStr(this.DeviceType.Code, 250) + "," + intParentId + "," + Function.SqlStr(this.Name, 250) + "," + Function.SqlStr(this.NameShortcut, 250) + "," + Function.SqlStr(this.Address, 250) + "," + Function.SqlStr(this.Identifier, 250) + "," + (int)this.State + "," + Function.SqlStr(this.StateDescription, 250) + "," + this.OrdinalNumber + "," + Function.SqlDecimal(this.Longitude, 8, 6) + "," + Function.SqlDecimal(this.Latitude, 8, 6) + "," + Function.SqlStr(strConfig);
                        strSql = "INSERT INTO " + Tables.Device.TableName + " (" + Tables.Device.DeviceId + "," + Tables.Device.DeviceType + "," + Tables.Device.ParentId + "," + Tables.Device.Name + "," + Tables.Device.NameShortcut + "," + Tables.Device.DeviceAddress + "," + Tables.Device.Identifier + "," + Tables.Device.StateOption + "," + Tables.Device.StateDescription + "," + Tables.Device.OrdinalNumber + "," + Tables.Device.Longitude + "," + Tables.Device.Latitude + "," + Tables.Device.XMLConfig + ") VALUES (" + strSql + ")";
                        dbConn.ExecuteNonQuery(strSql);

                        if (this.Parent != null)
                        {
                            this.Parent.Children.Add(this);
                            this.Parent.OnCreatedChildren(true, this);
                        }
                    }
                    catch
                    {
                        this.Save(tryCount + 1);
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
                        string strUpdate = "";
                        strSql = "SELECT * FROM " + Tables.Device.TableName + " WHERE " + Tables.Device.DeviceId + "=" + this.DeviceId;
                        System.Data.IDataReader dr = dbConn.ExecuteReader(strSql);
                        while (dr.Read())
                        {
                            if (this.Name != null && this.Name.Equals(Function.ToString(dr[Tables.Device.Name])) == false)
                            {
                                strUpdate += "," + Tables.Device.Name + "=" + Function.SqlStr(this.Name, 250);
                            }
                            if (this.NameShortcut != null && this.NameShortcut.Equals(Function.ToString(dr[Tables.Device.NameShortcut])) == false)
                            {
                                strUpdate += "," + Tables.Device.NameShortcut + "=" + Function.SqlStr(this.NameShortcut, 250);
                            }
                            if (this.Address != null && this.Address.Equals(Function.ToString(dr[Tables.Device.DeviceAddress])) == false)
                            {
                                strUpdate += "," + Tables.Device.DeviceAddress + "=" + Function.SqlStr(this.Address, 250);
                            }
                            if (this.Identifier != null && this.Identifier.Equals(Function.ToString(dr[Tables.Device.Identifier])) == false)
                            {
                                strUpdate += "," + Tables.Device.Identifier + "=" + Function.SqlStr(this.Identifier, 250);
                            }
                            if (this.State != (DeviceStateOptions)Function.ToInt(dr[Tables.Device.StateOption]))
                            {
                                strUpdate += "," + Tables.Device.StateOption + "=" + (int)this.State;
                            }
                            if (this.StateDescription != null && this.StateDescription.Equals(Function.ToString(dr[Tables.Device.StateDescription])) == false)
                            {
                                strUpdate += "," + Tables.Device.StateDescription + "=" + Function.SqlStr(this.StateDescription, 250);
                            }
                            if (this.OrdinalNumber != Function.ToInt(dr[Tables.Device.OrdinalNumber]))
                            {
                                strUpdate += "," + Tables.Device.OrdinalNumber + "=" + this.OrdinalNumber;
                            }
                            if (this.Longitude != Function.ToDecimal(dr[Tables.Device.Longitude]))
                            {
                                strUpdate += "," + Tables.Device.Longitude + "=" + Function.SqlDecimal(this.Longitude, 6);
                            }
                            if (this.Latitude != Function.ToDecimal(dr[Tables.Device.Latitude]))
                            {
                                strUpdate += "," + Tables.Device.Latitude + "=" + Function.SqlDecimal(this.Latitude, 6);
                            }

                            object objConfig = dr[Tables.Device.XMLConfig];
                            if (objConfig != null && !(objConfig is System.DBNull))
                            {
                                if (strConfig == null)
                                {
                                    strUpdate += "," + Tables.Device.XMLConfig + "=Null";
                                }
                                else
                                {
                                    if (strConfig.Equals(objConfig.ToString()) == false)
                                    {
                                        strUpdate += "," + Tables.Device.XMLConfig + "=" + Function.SqlStr(strConfig);
                                    }
                                }
                            }
                            else
                            {
                                if (strConfig != null)
                                {
                                    strUpdate += "," + Tables.Device.XMLConfig + "=" + Function.SqlStr(strConfig);
                                }
                            }
                        }
                        dr.Close();

                        if (strUpdate.Length > 0)
                        {
                            strSql = "UPDATE " + Tables.Device.TableName + " SET " + strUpdate.Substring(1) + " WHERE " + Tables.Device.DeviceId + "=" + this.DeviceId;
                            dbConn.ExecuteNonQuery(strSql);

                            this.OnUpdated(true);
                        }
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

            if (this.m_Propertys != null)
            {
                DevicePropertyCollection old = new DevicePropertyCollection(this, true, true);
                if (this.m_Propertys.Equals(old) == false)
                {
                    foreach (DeviceProperty _DeviceProperty in old)
                    {
                        DeviceProperty p = this.m_Propertys.Get(_DeviceProperty);
                        if (p == null)
                        {
                            //删除属性
                            _DeviceProperty.Delete();
                            this.m_Propertys.Remove(_DeviceProperty);
                        }
                        else if (p.Equals(_DeviceProperty) == false)
                        {
                            //修改属性
                            _DeviceProperty.Delete();
                            p.SaveSQL = new List<string>();
                            p.SaveValue();
                            p.SaveSqlToDb();
                            this.m_Propertys.SetProperty(p);
                        }
                    }

                    int intCount = this.m_Propertys.Count;
                    for (int intIndex = 0; intIndex < intCount; intIndex++)
                    {
                        DeviceProperty _DeviceProperty = this.m_Propertys[intIndex];
                        DeviceProperty p = old.Get(_DeviceProperty);
                        if (p == null)
                        {
                            //新增属性
                            _DeviceProperty.SaveSQL = new List<string>();
                            _DeviceProperty.SaveValue();
                            _DeviceProperty.SaveSqlToDb();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 删除当前设备。
        /// </summary>
        public virtual void Delete()
        {
            foreach (Device children in this.Children)
            {
                children.Delete();
            }

            string strSql;

            for (int intIndex = 0; intIndex < this.Classifys.Count; intIndex++)
            {
                this.Classifys.RemoveAt(0);
                intIndex--;
            }

            AC.Base.Database.DbConnection dbConn = this.Application.GetDbConnection();
            if (dbConn != null)
            {
                try
                {
                    strSql = "DELETE FROM " + Tables.DevicePropertyInt.TableName + " WHERE " + Tables.DevicePropertyInt.DeviceId + "=" + this.DeviceId;
                    dbConn.ExecuteNonQuery(strSql);
                }
                catch { }

                try
                {
                    strSql = "DELETE FROM " + Tables.DevicePropertyLong.TableName + " WHERE " + Tables.DevicePropertyLong.DeviceId + "=" + this.DeviceId;
                    dbConn.ExecuteNonQuery(strSql);
                }
                catch { }

                try
                {
                    strSql = "DELETE FROM " + Tables.DevicePropertyDecimal.TableName + " WHERE " + Tables.DevicePropertyDecimal.DeviceId + "=" + this.DeviceId;
                    dbConn.ExecuteNonQuery(strSql);
                }
                catch { }

                try
                {
                    strSql = "DELETE FROM " + Tables.DevicePropertyString.TableName + " WHERE " + Tables.DevicePropertyString.DeviceId + "=" + this.DeviceId;
                    dbConn.ExecuteNonQuery(strSql);
                }
                catch { }

                try
                {
                    strSql = "DELETE FROM " + Tables.DevicePropertyText.TableName + " WHERE " + Tables.DevicePropertyText.DeviceId + "=" + this.DeviceId;
                    dbConn.ExecuteNonQuery(strSql);
                }
                catch { }

                try
                {
                    strSql = "DELETE FROM " + Tables.DevicePropertyBytes.TableName + " WHERE " + Tables.DevicePropertyBytes.DeviceId + "=" + this.DeviceId;
                    dbConn.ExecuteNonQuery(strSql);
                }
                catch { }

                try
                {
                    strSql = "DELETE FROM " + Tables.Device.TableName + " WHERE " + Tables.Device.DeviceId + "=" + this.DeviceId;
                    dbConn.ExecuteNonQuery(strSql);

                    this.OnDeleted(true);
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
        /// 当新建下级设备后产生的事件。
        /// </summary>
        public event DeviceCreatedEventHandler CreatedChildren;

        /// <summary>
        /// 引发 CreatedChildren 创建子设备事件，并通过系统消息交换在网络上发布该事件。
        /// </summary>
        /// <param name="isSendSystemMessage">是否发送系统消息。</param>
        /// <param name="createdDevice">被创建的子设备。</param>
        internal void OnCreatedChildren(bool isSendSystemMessage, Device createdDevice)
        {
            if (this.CreatedChildren != null)
            {
                this.CreatedChildren(this, createdDevice);
            }

            this.Application.OnDeviceCreated(this, createdDevice);

            //if (isSendSystemMessage)
            //{
            //    byte[] bytId = new byte[4];
            //    bytId[0] = (byte)((createdDevice.DeviceId >> 24) % 0x100);
            //    bytId[1] = (byte)((createdDevice.DeviceId >> 16) % 0x100);
            //    bytId[2] = (byte)((createdDevice.DeviceId >> 8) % 0x100);
            //    bytId[3] = (byte)(createdDevice.DeviceId % 0x100);

            //    this.Application.SendSystemMessage(SystemMessageTypeOptions.DeviceCreated, bytId);
            //}
        }

        /// <summary>
        /// 当前设备的档案、属性信息更改后产生的事件。
        /// </summary>
        public event DeviceUpdatedEventHandler Updated;

        /// <summary>
        /// 引发 Updated 档案修改事件，并通过系统消息交换在网络上发布该事件。
        /// </summary>
        /// <param name="isSendSystemMessage">是否发送系统消息。</param>
        protected internal void OnUpdated(bool isSendSystemMessage)
        {
            if (this.Updated != null)
            {
                this.Updated(this);
            }
        }

        /// <summary>
        /// 当前设备被删除后产生的事件。
        /// </summary>
        public event DeviceDeletedEventHandler Deleted;

        /// <summary>
        /// 引发 Deleted 设备删除事件，并通过系统消息交换在网络上发布该事件。
        /// </summary>
        /// <param name="isSendSystemMessage">是否发送系统消息。</param>
        internal void OnDeleted(bool isSendSystemMessage)
        {
            if (this.Deleted != null)
            {
                this.Deleted(this);
            }

            //if (isSendSystemMessage)
            //{
            //    byte[] bytId = new byte[4];
            //    bytId[0] = (byte)((this.DeviceId >> 24) % 0x100);
            //    bytId[1] = (byte)((this.DeviceId >> 16) % 0x100);
            //    bytId[2] = (byte)((this.DeviceId >> 8) % 0x100);
            //    bytId[3] = (byte)(this.DeviceId % 0x100);

            //    this.Application.SendSystemMessage(SystemMessageTypeOptions.DeviceDeleted, bytId);
            //}
        }

        /// <summary>
        /// 确定指定的对象是否等于当前的对象。
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is Device && this.DeviceId > 0)
            {
                Device device = obj as Device;
                if (device.DeviceId > 0 && device.DeviceId == this.DeviceId)
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
            if (this.DeviceId > 0)
            {
                return "[" + Function.StringInterceptFill(this.DeviceId.ToString(), 6, "0", false) + "]" + Function.OutString(this.Name, "", " ", "");
            }
            else
            {
                return "[新设备]" + (this.Name.Length > 0 ? " " + this.Name : "");
            }
        }

        #endregion

        #region << 级联及子设备集合 >>
        private int m_ParentId = 0;
        private Device m_Parent;
        /// <summary>
        /// 获取上级设备，如果返回 null 则表示当前设备是根节点设备(根节点设备通常为通道)。
        /// </summary>
        public Device Parent
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
                        //读取上级设备
                        if (this.Source != null && this.Source.Count > 1)
                        {
                            DeviceSearch ds = new DeviceSearch(this.Application);
                            List<int> lstParentId = new List<int>();
                            foreach (Device device in this.Source)
                            {
                                if (device.m_ParentId > 0 && lstParentId.Contains(device.m_ParentId) == false)
                                {
                                    lstParentId.Add(device.m_ParentId);
                                }
                            }
                            ds.Filters.Add(new IdFilter(lstParentId));

                            foreach (Device parentDevice in ds.Search(1, false, true))
                            {
                                foreach (Device device in this.Source)
                                {
                                    if (device.m_ParentId == parentDevice.DeviceId)
                                    {
                                        device.SetParent(parentDevice);
                                    }
                                }
                            }
                        }
                        else
                        {
                            DeviceSearch ds = new DeviceSearch(this.Application);
                            ds.Filters.Add(new IdFilter(this.m_ParentId));

                            foreach (Device parentDevice in ds.Search(1, false, false))
                            {
                                this.SetParent(parentDevice);
                            }
                        }
                    }
                    return this.m_Parent;
                }
            }
        }

        internal void SetParent(Device parent)
        {
            if (parent != null)
            {
                this.m_ParentId = parent.DeviceId;
            }
            else
            {
                this.m_ParentId = 0;
            }
            this.m_Parent = parent;
        }

        private DeviceChildrenCollection m_Children;
        /// <summary>
        /// 获取下级子设备集合。
        /// </summary>
        public DeviceChildrenCollection Children
        {
            get
            {
                if (this.m_Children == null)
                {
                    this.m_Children = new DeviceChildrenCollection(this);
                }
                return this.m_Children;
            }
        }

        /// <summary>
        /// 某一设备的下级子设备集合。
        /// </summary>
        public class DeviceChildrenCollection : IDeviceCollection
        {
            private Device m_Parent;
            private DeviceCollection m_Children;
            private int m_Count = -1;               //下级设备的总数量

            internal DeviceChildrenCollection(Device parent)
            {
                this.m_Parent = parent;
            }

            internal void Add(Device item)
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

            internal void Remove(Device item)
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

            #region IList<Device> 成员

            /// <summary>
            /// 搜索指定的设备，并返回第一个匹配项的从零开始的索引。
            /// </summary>
            /// <param name="item">要在集合中查找的设备。</param>
            /// <returns>如果在整个子设备集合中找到 item 的第一个匹配项，则为该项的从零开始的索引；否则为 -1。</returns>
            public int IndexOf(Device item)
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
            /// 获取指定索引处的设备。
            /// </summary>
            /// <param name="index"></param>
            /// <returns></returns>
            public Device this[int index]
            {
                get
                {
                    if (index < 0)
                    {
                        throw new Exception("子设备索引不得小于0。");
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
                    foreach (Device deviceParent in this.m_Parent.Source)
                    {
                        deviceParent.Children.m_Children = new DeviceCollection(true);
                    }

                    //批量读取子设备
                    DeviceSearch ds = new DeviceSearch(this.m_Parent.Application);
                    ds.Filters.Add(new ParentIdFilter(this.m_Parent.Source.GetIdForArray()));
                    //ds.Filters.Add(new StatusFilter(1));//测试
                    ds.OrderInfos.Add(false, new OrdinalNumberOrder());
                    ds.OrderInfos.Add(false, new IdOrder());

                    foreach (Device deviceChildren in ds.Search(1, false, false))
                    {
                        foreach (Device deviceParent in this.m_Parent.Source)
                        {
                            if (deviceChildren.m_ParentId == deviceParent.DeviceId)
                            {
                                deviceParent.m_Children.Add(deviceChildren);
                                break;
                            }
                        }
                    }

                    foreach (Device deviceParent in this.m_Parent.Source)
                    {
                        deviceParent.Children.m_Count = deviceParent.Children.m_Children.Count;
                    }
                }
                else
                {
                    DeviceSearch ds = new DeviceSearch(this.m_Parent.Application);
                    ds.Filters.Add(new ParentIdFilter(this.m_Parent.DeviceId));

                    ds.OrderInfos = new Searchs.SearchOrderInfoCollection<IDeviceOrder>();
                    ds.OrderInfos.Add(false, new OrdinalNumberOrder());
                    ds.OrderInfos.Add(false, new IdOrder());

                    this.m_Children = ds.Search();
                    this.m_Count = ds.RecordsetCount;

                    foreach (Device device in this.m_Children)
                    {
                        device.SetParent(this.m_Parent);
                    }
                }
            }

            #endregion

            #region ICollection<Device> 成员

            /// <summary>
            /// 确定指定的设备是否在该子集合中。
            /// </summary>
            /// <param name="item"></param>
            /// <returns></returns>
            public bool Contains(Device item)
            {
                foreach (Device device in this)
                {
                    if (device.Equals(item))
                    {
                        return true;
                    }
                }
                return false;
            }

            /// <summary>
            /// 将整个子集合的设备复制到兼容的一维数组中，从目标数组的指定索引位置开始放置。
            /// </summary>
            /// <param name="array"></param>
            /// <param name="arrayIndex"></param>
            public void CopyTo(Device[] array, int arrayIndex)
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
            /// 获取当前集合子设备的总数。
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
                                string strSql = "SELECT " + Tables.Device.ParentId + ",COUNT(*) AS COUNT_NUM FROM " + Tables.Device.TableName + " WHERE " + Tables.Device.ParentId + " IN (" + strIds + ") GROUP BY " + Tables.Device.ParentId;
                                System.Data.IDataReader dr = dbConn.ExecuteReader(strSql);
                                while (dr.Read())
                                {
                                    int intDeviceId = Convert.ToInt32(dr[Tables.Device.ParentId]);

                                    foreach (Device device in this.m_Parent.Source)
                                    {
                                        if (device.DeviceId == intDeviceId)
                                        {
                                            device.Children.m_Count = Convert.ToInt32(dr["COUNT_NUM"]);
                                            break;
                                        }
                                    }
                                }
                                dr.Close();

                                foreach (Device device in this.m_Parent.Source)
                                {
                                    if (device.Children.m_Count == -1)
                                    {
                                        device.Children.m_Count = 0;            //将无下级设备的 Count 设为 0。
                                    }
                                }
                            }
                            else
                            {
                                string strSql = "SELECT COUNT(*) FROM " + Tables.Device.TableName + " WHERE " + Tables.Device.ParentId + "=" + this.m_Parent.DeviceId;
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

            #region IEnumerable<Device> 成员

            /// <summary>
            /// 返回循环访问设备集合的枚举器。
            /// </summary>
            /// <returns></returns>
            public IEnumerator<Device> GetEnumerator()
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
            /// 返回循环访问设备集合的枚举器。
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
                return "共 " + this.Count + " 个子设备";
            }

            #region IDeviceCollection 成员

            /// <summary>
            /// 获取该集合中指定 ID 的设备。
            /// </summary>
            /// <param name="deviceId">设备 ID。</param>
            /// <returns>返回集合中指定 ID 的设备，如果未查找到该设备则返回 null。</returns>
            public Device GetDevice(int deviceId)
            {
                foreach (Device device in this)
                {
                    if (device.DeviceId == deviceId)
                    {
                        return device;
                    }
                }
                return null;
            }

            /// <summary>
            /// 获取该集合中符合筛选条件的设备集合。
            /// </summary>
            /// <param name="filters">设备筛选条件。</param>
            /// <returns></returns>
            public DeviceCollection GetDevices(DeviceFilterCollection filters)
            {
                DeviceCollection devices = new DeviceCollection(false);

                foreach (Device device in this)
                {
                    if (filters.DeviceFilterCheck(device))
                    {
                        devices.Add(device);
                    }
                }

                return devices;
            }

            /// <summary>
            /// 获取当前集合内所有的设备编号。
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
                    intIds[intIndex] = this[intIndex].DeviceId;
                }

                return intIds;
            }

            /// <summary>
            /// 获取当前集合内所有的设备编号。
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
                    lstIds[intIndex] = this[intIndex].DeviceId;
                }

                return lstIds;
            }

            /// <summary>
            /// 获取当前集合内所有的设备编号。
            /// </summary>
            /// <returns>以逗号分隔的字符串形式的设备编号。</returns>
            public string GetIdForString()
            {
                return this.GetIdForString(",");
            }

            /// <summary>
            /// 获取当前集合内所有的设备编号。
            /// </summary>
            /// <param name="separator">分隔各设备编号的字符。</param>
            /// <returns>以指定字符分隔的字符串形式的设备编号。</returns>
            public string GetIdForString(string separator)
            {
                string strIds = "";

                foreach (Device device in this)
                {
                    strIds += separator + device.DeviceId.ToString();
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

        #region << 设备驱动 >>

        private System.Xml.XmlNode m_DriveConfig;
        private IDrive m_Drive;
        /// <summary>
        /// 获取当前设备驱动对象。如果当前设备无驱动则返回 null。
        /// </summary>
        public IDrive Drive
        {
            get
            {
                if (this.m_Drive == null && this.DeviceType.DriveType != null)
                {
                    this.m_Drive = this.DeviceType.CreateDrive();
                    this.m_Drive.GettingParent += new GettingParentDriveEventHandler(Drive_GetParent);
                    this.m_Drive.GettingChildren += new GettingChildrenDriveEventHandler(Drive_GetChildren);
                    if (this.m_DriveConfig != null)
                    {
                        this.m_Drive.SetDriveConfig(this.m_DriveConfig);
                    }
                    this.DriveInitialize();
                }
                return this.m_Drive;
            }
        }

        /// <summary>
        /// 继承的类可重写该方法，在设备驱动被实例化后对驱动进行初始设置。
        /// </summary>（
        protected virtual void DriveInitialize()
        {
            
        }

        private IDrive Drive_GetParent()
        {
            if (this.Parent != null)
            {
                return this.Parent.Drive;
            }
            else
            {
                return null;
            }
        }

        private DriveChildrenCollection m_DriveChildren;

        private IDriveCollection Drive_GetChildren()
        {
            if (this.m_DriveChildren == null)
            {
                this.m_DriveChildren = new DriveChildrenCollection(this);
            }
            return this.m_DriveChildren;
        }

        /// <summary>
        /// 驱动对象集合。
        /// </summary>
        public class DriveChildrenCollection : IDriveCollection
        {
            private Device m_Device;
            internal DriveChildrenCollection(Device device)
            {
                this.m_Device = device;
            }

            #region IDeviceCollection 成员

            /// <summary>
            /// 搜索指定的驱动，并返回整个集合中第一个匹配项的从零开始的索引。
            /// </summary>
            /// <param name="item"></param>
            /// <returns></returns>
            public int IndexOf(IDrive item)
            {
                for (int intIndex = 0; intIndex < this.m_Device.Children.Count; intIndex++)
                {
                    if (this.m_Device.Children[intIndex].Drive.Equals(item))
                    {
                        return intIndex;
                    }
                }

                return -1;
            }

            /// <summary>
            /// 获取指定索引处的驱动。
            /// </summary>
            /// <param name="index"></param>
            /// <returns></returns>
            public IDrive this[int index]
            {
                get { return this.m_Device.Children[index].Drive; }
            }

            /// <summary>
            /// 确定某驱动是否在集合中。
            /// </summary>
            /// <param name="item"></param>
            /// <returns></returns>
            public bool Contains(IDrive item)
            {
                foreach (Device device in this.m_Device.Children)
                {
                    if (device.Drive.Equals(item))
                    {
                        return true;
                    }
                }
                return false;
            }

            /// <summary>
            /// 将整个驱动集合复制到兼容的一维数组中，从目标数组的指定索引位置开始放置。
            /// </summary>
            /// <param name="array"></param>
            /// <param name="arrayIndex"></param>
            public void CopyTo(IDrive[] array, int arrayIndex)
            {
                for (int intIndex = 0; intIndex < this.m_Device.Children.Count; intIndex++)
                {
                    array[arrayIndex + intIndex] = this.m_Device.Children[intIndex].Drive;
                }
            }

            /// <summary>
            /// 获取集合中实际包含的驱动数。
            /// </summary>
            public int Count
            {
                get { return this.m_Device.Children.Count; }
            }

            #endregion

            #region IEnumerable<IDevice> 成员

            /// <summary>
            /// 返回循环访问驱动集合的枚举数。
            /// </summary>
            /// <returns></returns>
            public IEnumerator<IDrive> GetEnumerator()
            {
                return new DriveEnumerator(this.m_Device);
            }

            #endregion

            #region IEnumerable 成员

            /// <summary>
            /// 返回一个循环访问驱动集合的枚举数。
            /// </summary>
            /// <returns></returns>
            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return new DriveEnumerator(this.m_Device);
            }

            #endregion

            /// <summary>
            /// 获取当前对象的字符串表示形式。
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                return "共 " + this.Count + " 个子驱动";
            }
        }

        /// <summary>
        /// 从设备信息枚举设备时用到的集合迭代。
        /// </summary>
        public class DriveEnumerator : IEnumerator<IDrive>
        {
            private Device m_Device;
            private int m_Index = -1;

            internal DriveEnumerator(Device device)
            {
                this.m_Device = device;
            }

            #region IEnumerator<IDevice> 成员

            /// <summary>
            /// 获取集合中位于枚举数当前位置的元素。
            /// </summary>
            public IDrive Current
            {
                get { return this.m_Device.Children[this.m_Index].Drive; }
            }

            #endregion

            #region IDisposable 成员

            /// <summary>
            /// 执行与释放或重置非托管资源相关的应用程序定义的任务。
            /// </summary>
            public void Dispose()
            {
                this.m_Device = null;
            }

            #endregion

            #region IEnumerator 成员

            /// <summary>
            /// 获取集合中位于枚举数当前位置的元素。
            /// </summary>
            object System.Collections.IEnumerator.Current
            {
                get { return this.m_Device.Children[this.m_Index].Drive; }
            }

            /// <summary>
            /// 将枚举数推进到集合的下一个元素。
            /// </summary>
            /// <returns></returns>
            public bool MoveNext()
            {
                this.m_Index++;
                return this.m_Index < this.m_Device.Children.Count;
            }

            /// <summary>
            /// 将枚举数设置为其初始位置，该位置位于集合中第一个元素之前。
            /// </summary>
            public void Reset()
            {
                this.m_Index = -1;
            }

            #endregion
        }

        #endregion

        #region << 数据日期范围 >>

        private DateRange m_DateRange;
        /// <summary>
        /// 获取查询设备数据时所使用的日期范围。
        /// </summary>
        public DateRange DateRange
        {
            get
            {
                return this.m_DateRange;
            }
            internal set
            {
                this.m_DateRange = value;
            }
        }

        /// <summary>
        /// 为后续的数据查询操作预先设置数据日期范围。
        /// </summary>
        /// <param name="dateRange"></param>
        public void SetDateRange(DateRange dateRange)
        {
            this.DateRange = dateRange;
        }

        /// <summary>
        /// 为后续的数据查询操作预先设置数据日期。
        /// </summary>
        /// <param name="date"></param>
        public void SetDateRange(DateTime date)
        {
            this.DateRange = new DateRange(date);
        }

        /// <summary>
        /// 为后续的数据查询操作预先设置数据日期范围。
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        public void SetDateRange(DateTime startDate, DateTime endDate)
        {
            this.DateRange = new DateRange(startDate, endDate);
        }

        #endregion

        #region << 设备分类 >>

        private DeviceClassifyCollection m_Classifys;
        /// <summary>
        /// 获取设备所属的分类。
        /// </summary>
        public DeviceClassifyCollection Classifys
        {
            get
            {
                if (this.m_Classifys == null)
                    this.m_Classifys = new DeviceClassifyCollection(this, true);

                return this.m_Classifys;
            }
        }

        /// <summary>
        /// 设备所属的分类集合。
        /// </summary>
        public class DeviceClassifyCollection : System.Collections.Generic.IList<Classify>, IClassifyCollection
        {
            private System.Collections.Generic.List<Classify> Items = new List<Classify>();

            internal DeviceClassifyCollection(Device device, bool isLoad)
            {
                this.Device = device;

                if (isLoad)
                {
                    if (this.Device.Source == null)
                    {
                        ClassifySearchs.ClassifySearch _Search = new ClassifySearchs.ClassifySearch(this.Device.Application);
                        _Search.Filters.Add(new ClassifySearchs.DeviceFilter(this.Device, false));
                        foreach (Classify _Classify in _Search.Search())
                        {
                            this.Items.Add(_Classify);
                        }
                    }
                    else
                    {
                        foreach (AC.Base.Device _Device in this.Device.Source)
                        {
                            if (_Device.DeviceId != device.DeviceId)
                                _Device.m_Classifys = new DeviceClassifyCollection(_Device, false);
                        }

                        ClassifySearchs.ClassifySearch _Search = new ClassifySearchs.ClassifySearch(this.Device.Application);
                        _Search.Filters.Add(new ClassifySearchs.DeviceFilter(this.Device.Source.ToArray(), false));
                        ClassifyCollection _Classifys = _Search.Search();

                        AC.Base.Database.DbConnection dbConn = this.Device.Application.GetDbConnection();
                        if (dbConn != null)
                        {
                            try
                            {
                                string strSql = "SELECT * FROM " + Tables.ClassifyDevice.TableName + " WHERE " + Tables.ClassifyDevice.DeviceId + " IN (" + this.Device.Source.GetIdForString() + ")";
                                System.Data.IDataReader dr = dbConn.ExecuteReader(strSql);
                                while (dr.Read())
                                {
                                    int intClassifyId = Convert.ToInt32(dr[Tables.ClassifyDevice.ClassifyId]);
                                    int intDeviceId = Convert.ToInt32(dr[Tables.ClassifyDevice.DeviceId]);

                                    AC.Base.Device _Device = this.Device.Source.GetDevice(intDeviceId);
                                    Classify _Classify = _Classifys.GetClassify(intClassifyId);
                                    if (_Device != null && _Classify != null)
                                    {
                                        if (_Device.DeviceId != device.DeviceId)
                                            _Device.m_Classifys.Items.Add(_Classify);
                                        else
                                            this.Items.Add(_Classify);
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
                }
            }

            /// <summary>
            /// 该集合内的分类所关联的设备。
            /// </summary>
            public Device Device { get; private set; }

            //新增设备与分类的对应关系。
            private void Insert(Classify classify, int index)
            {
                AC.Base.Database.DbConnection dbConn = this.Device.Application.GetDbConnection();
                if (dbConn != null)
                {
                    Classify _Classify = classify;
                    try
                    {
                        string strSql = _Classify.ClassifyId + "," + this.Device.DeviceId + "," + index;
                        strSql = "INSERT INTO " + Tables.ClassifyDevice.TableName + " (" + Tables.ClassifyDevice.ClassifyId + "," + Tables.ClassifyDevice.DeviceId + "," + Tables.ClassifyDevice.OrdinalNumber + ") VALUES (" + strSql + ")";
                        dbConn.ExecuteNonQuery(strSql);

                        if (_Classify.m_DeviceCount != -1)
                        {
                            _Classify.m_DeviceCount++;
                        }
                    }
                    catch { }

                    while (_Classify != null)
                    {
                        try
                        {
                            string strSql = _Classify.ClassifyId + "," + this.Device.DeviceId + "," + classify.ClassifyId;
                            strSql = "INSERT INTO " + Tables.ClassifyDeviceAll.TableName + " (" + Tables.ClassifyDeviceAll.ClassifyId + "," + Tables.ClassifyDeviceAll.DeviceId + "," + Tables.ClassifyDeviceAll.OwnerClassifyId + ") VALUES (" + strSql + ")";
                            dbConn.ExecuteNonQuery(strSql);
                        }
                        catch { }

                        _Classify = _Classify.Parent;
                    }
                    dbConn.Close();
                }
            }

            //删除设备与分类的对应关系。
            private void Delete(Classify classify)
            {
                AC.Base.Database.DbConnection dbConn = this.Device.Application.GetDbConnection();
                if (dbConn != null)
                {
                    Classify _Classify = classify;
                    try
                    {
                        string strSql = "DELETE FROM " + Tables.ClassifyDevice.TableName + " WHERE " + Tables.ClassifyDevice.ClassifyId + "=" + _Classify.ClassifyId + " AND " + Tables.ClassifyDevice.DeviceId + "=" + this.Device.DeviceId;
                        dbConn.ExecuteNonQuery(strSql);

                        if (_Classify.m_DeviceCount != -1)
                        {
                            _Classify.m_DeviceCount--;
                        }
                    }
                    catch { }

                    while (_Classify != null)
                    {
                        try
                        {
                            string strSql = "DELETE FROM " + Tables.ClassifyDeviceAll.TableName + " WHERE " + Tables.ClassifyDeviceAll.ClassifyId + "=" + _Classify.ClassifyId + " AND " + Tables.ClassifyDeviceAll.DeviceId + "=" + this.Device.DeviceId;
                            dbConn.ExecuteNonQuery(strSql);
                        }
                        catch { }

                        _Classify = _Classify.Parent;
                    }
                    dbConn.Close();
                }
            }

            /// <summary>
            /// 获取当前对象的字符串表示形式。
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                return "共 " + this.Count + " 个分类";
            }

            #region IList<Classify> 成员

            /// <summary>
            /// 搜索指定的分类，并返回整个集合中第一个匹配项的从零开始的索引。
            /// </summary>
            /// <param name="item"></param>
            /// <returns></returns>
            public int IndexOf(Classify item)
            {
                for (int intIndex = 0; intIndex < this.Items.Count; intIndex++)
                {
                    if (this.Items[intIndex].Equals(item))
                    {
                        return intIndex;
                    }
                }
                return -1;
            }

            /// <summary>
            /// 将分类插入集合的指定索引处。
            /// </summary>
            /// <param name="index"></param>
            /// <param name="item"></param>
            public void Insert(int index, Classify item)
            {
                this.Insert(item, index);

                this.Items.Insert(index, item);
            }

            /// <summary>
            /// 移除集合的指定索引处的分类。
            /// </summary>
            /// <param name="index"></param>
            public void RemoveAt(int index)
            {
                this.Delete(this.Items[index]);

                this.Items.RemoveAt(index);
            }

            /// <summary>
            /// 获取或设置指定索引处的分类。
            /// </summary>
            /// <param name="index"></param>
            /// <returns></returns>
            public Classify this[int index]
            {
                get
                {
                    return this.Items[index];
                }
                set
                {
                    this.Delete(this.Items[index]);

                    this.Items[index] = value;

                    this.Insert(this.Items[index], index);
                }
            }

            #endregion

            #region ICollection<Classify> 成员

            /// <summary>
            /// 将分类添加到集合的结尾处。
            /// </summary>
            /// <param name="item"></param>
            public void Add(Classify item)
            {
                this.Insert(item, this.Count);

                this.Items.Add(item);
            }

            /// <summary>
            /// 从集合中移除所有分类。
            /// </summary>
            public void Clear()
            {
                foreach (Classify c in this.Items)
                {
                    this.Delete(c);
                }

                this.Items.Clear();
            }

            /// <summary>
            /// 确定某分类是否在集合中。
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
            /// 将整个分类集合复制到兼容的一维数组中，从目标数组的指定索引位置开始放置。
            /// </summary>
            /// <param name="array"></param>
            /// <param name="arrayIndex"></param>
            public void CopyTo(Classify[] array, int arrayIndex)
            {
                this.Items.CopyTo(array, arrayIndex);
            }

            /// <summary>
            /// 获取集合中实际包含的分类数。
            /// </summary>
            public int Count
            {
                get { return this.Items.Count; }
            }

            /// <summary>
            /// 获取一个值，该值指示分类集合是否为只读。
            /// </summary>
            public bool IsReadOnly
            {
                get { return false; }
            }

            /// <summary>
            /// 从集合中移除特定分类的第一个匹配项。
            /// </summary>
            /// <param name="item"></param>
            /// <returns></returns>
            public bool Remove(Classify item)
            {
                this.Delete(item);

                return this.Items.Remove(item);
            }

            #endregion

            #region IEnumerable<Classify> 成员

            /// <summary>
            /// 返回循环访问分类集合的枚举数。
            /// </summary>
            /// <returns></returns>
            public IEnumerator<Classify> GetEnumerator()
            {
                return this.Items.GetEnumerator();
            }

            #endregion

            #region IEnumerable 成员

            /// <summary>
            /// 返回一个循环访问分类集合的枚举数。
            /// </summary>
            /// <returns></returns>
            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return this.Items.GetEnumerator();
            }

            #endregion

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
            public ClassifyCollection GetClassifys(AC.Base.ClassifySearchs.ClassifyFilterCollection filters)
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
                int[] ids = new int[this.Count];

                for (int intIndex = 0; intIndex < this.Count; intIndex++)
                {
                    ids[intIndex] = this[intIndex].ClassifyId;
                }

                return ids;
            }

            /// <summary>
            /// 获取当前集合内所有的分类编号。
            /// </summary>
            /// <returns></returns>
            public ICollection<int> GetIdForCollection()
            {
                List<int> lsId = new List<int>(this.Count);

                foreach (Classify classify in this)
                {
                    lsId.Add(classify.ClassifyId);
                }

                return lsId;
            }

            /// <summary>
            /// 获取当前分类集合中以逗号分隔的分类编号字符串。
            /// </summary>
            /// <returns></returns>
            public string GetIdForString()
            {
                return this.GetIdForString(",");
            }

            /// <summary>
            /// 获取当前分类集合中以指定字符分隔的分类编号字符串。
            /// </summary>
            /// <param name="separator">用于分隔分类编号的字符。</param>
            /// <returns></returns>
            public string GetIdForString(string separator)
            {
                string strIds = "";

                foreach (Classify classify in this)
                {
                    strIds += separator + classify.ClassifyId;
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

        #region << 设备属性 >>

        private DevicePropertyCollection m_Propertys;
        /// <summary>
        /// 获取设备所有的属性。
        /// </summary>
        public DevicePropertyCollection Propertys
        {
            get
            {
                if (this.m_Propertys == null)
                {
                    this.m_Propertys = new DevicePropertyCollection(this, true, false);
                }
                return this.m_Propertys;
            }
        }

        /// <summary>
        /// 设备所属的属性集合。
        /// </summary>
        public class DevicePropertyCollection : IEnumerable<DeviceProperty>, IEnumerable
        {
            private List<DeviceProperty> m_Items = new List<DeviceProperty>();
            private Device m_Device;

            internal DevicePropertyCollection(Device device, bool isLoad, bool onlySelf)
            {
                this.m_Device = device;

                if (isLoad)
                {
                    if (this.m_Device.Source == null || this.m_Device.Source.Count == 1 || onlySelf)
                    {
                        AC.Base.Database.DbConnection dbConn = this.m_Device.Application.GetDbConnection();
                        if (dbConn != null)
                        {
                            try
                            {
                                string strSql = "SELECT * FROM " + Tables.DevicePropertyInt.TableName + " WHERE " + Tables.DevicePropertyInt.DeviceId + "=" + this.m_Device.DeviceId;
                                System.Data.IDataReader dr = dbConn.ExecuteReader(strSql);
                                while (dr.Read())
                                {
                                    DeviceProperty _DeviceProperty = this.GetOrAdd(Function.ToString(dr[Tables.DevicePropertyInt.PropertyType]));
                                    if (_DeviceProperty != null)
                                    {
                                        _DeviceProperty.SetIntValue(Function.ToInt(dr[Tables.DevicePropertyInt.SerialNumber]), Function.ToInt(dr[Tables.DevicePropertyInt.DataValue]));
                                    }
                                }
                                dr.Close();
                            }
                            catch { }

                            try
                            {
                                string strSql = "SELECT * FROM " + Tables.DevicePropertyLong.TableName + " WHERE " + Tables.DevicePropertyLong.DeviceId + "=" + this.m_Device.DeviceId;
                                System.Data.IDataReader dr = dbConn.ExecuteReader(strSql);
                                while (dr.Read())
                                {
                                    DeviceProperty _DeviceProperty = this.GetOrAdd(Function.ToString(dr[Tables.DevicePropertyLong.PropertyType]));
                                    if (_DeviceProperty != null)
                                    {
                                        _DeviceProperty.SetLongValue(Function.ToInt(dr[Tables.DevicePropertyLong.SerialNumber]), Function.ToLong(dr[Tables.DevicePropertyLong.DataValue]));
                                    }
                                }
                                dr.Close();
                            }
                            catch { }

                            try
                            {
                                string strSql = "SELECT * FROM " + Tables.DevicePropertyDecimal.TableName + " WHERE " + Tables.DevicePropertyDecimal.DeviceId + "=" + this.m_Device.DeviceId;
                                System.Data.IDataReader dr = dbConn.ExecuteReader(strSql);
                                while (dr.Read())
                                {
                                    DeviceProperty _DeviceProperty = this.GetOrAdd(Function.ToString(dr[Tables.DevicePropertyDecimal.PropertyType]));
                                    if (_DeviceProperty != null)
                                    {
                                        _DeviceProperty.SetDecimalValue(Function.ToInt(dr[Tables.DevicePropertyDecimal.SerialNumber]), Function.ToDecimal(dr[Tables.DevicePropertyDecimal.DataValue]));
                                    }
                                }
                                dr.Close();
                            }
                            catch { }

                            try
                            {
                                string strSql = "SELECT * FROM " + Tables.DevicePropertyString.TableName + " WHERE " + Tables.DevicePropertyString.DeviceId + "=" + this.m_Device.DeviceId;
                                System.Data.IDataReader dr = dbConn.ExecuteReader(strSql);
                                while (dr.Read())
                                {
                                    DeviceProperty _DeviceProperty = this.GetOrAdd(Function.ToString(dr[Tables.DevicePropertyString.PropertyType]));
                                    if (_DeviceProperty != null)
                                    {
                                        _DeviceProperty.SetStringValue(Function.ToInt(dr[Tables.DevicePropertyString.SerialNumber]), Function.ToString(dr[Tables.DevicePropertyString.DataValue]));
                                    }
                                }
                                dr.Close();
                            }
                            catch { }

                            try
                            {
                                string strSql = "SELECT * FROM " + Tables.DevicePropertyText.TableName + " WHERE " + Tables.DevicePropertyText.DeviceId + "=" + this.m_Device.DeviceId;
                                System.Data.IDataReader dr = dbConn.ExecuteReader(strSql);
                                while (dr.Read())
                                {
                                    DeviceProperty _DeviceProperty = this.GetOrAdd(Function.ToString(dr[Tables.DevicePropertyText.PropertyType]));
                                    if (_DeviceProperty != null)
                                    {
                                        _DeviceProperty.SetTextValue(Function.ToInt(dr[Tables.DevicePropertyText.SerialNumber]), Function.ToString(dr[Tables.DevicePropertyText.DataValue]));
                                    }
                                }
                                dr.Close();
                            }
                            catch { }

                            try
                            {
                                string strSql = "SELECT * FROM " + Tables.DevicePropertyBytes.TableName + " WHERE " + Tables.DevicePropertyBytes.DeviceId + "=" + this.m_Device.DeviceId;
                                System.Data.IDataReader dr = dbConn.ExecuteReader(strSql);
                                while (dr.Read())
                                {
                                    DeviceProperty _DeviceProperty = this.GetOrAdd(Function.ToString(dr[Tables.DevicePropertyBytes.PropertyType]));
                                    if (_DeviceProperty != null)
                                    {
                                        if (dr[Tables.DevicePropertyBytes.DataValue] != null)
                                        {
                                            _DeviceProperty.SetBytesValue(Function.ToInt(dr[Tables.DevicePropertyBytes.SerialNumber]), (byte[])dr[Tables.DevicePropertyBytes.DataValue]);
                                        }
                                    }
                                }
                                dr.Close();
                            }
                            catch { }

                            dbConn.Close();
                        }
                    }
                    else
                    {
                        foreach (AC.Base.Device _Device in this.m_Device.Source)
                        {
                            if (_Device.DeviceId != this.m_Device.DeviceId)
                            {
                                _Device.m_Propertys = new DevicePropertyCollection(_Device, false, false);
                            }
                        }

                        AC.Base.Database.DbConnection dbConn = this.m_Device.Application.GetDbConnection();
                        if (dbConn != null)
                        {
                            try
                            {
                                string strSql = "SELECT * FROM " + Tables.DevicePropertyInt.TableName + " WHERE " + Tables.DevicePropertyInt.DeviceId + " IN (" + this.m_Device.Source.GetIdForString() + ")";
                                System.Data.IDataReader dr = dbConn.ExecuteReader(strSql);
                                while (dr.Read())
                                {
                                    DeviceProperty _DeviceProperty = this.Get(Function.ToInt(dr[Tables.DevicePropertyInt.DeviceId]), Function.ToString(dr[Tables.DevicePropertyInt.PropertyType]));
                                    if (_DeviceProperty != null)
                                    {
                                        _DeviceProperty.SetIntValue(Function.ToInt(dr[Tables.DevicePropertyInt.SerialNumber]), Function.ToInt(dr[Tables.DevicePropertyInt.DataValue]));
                                    }
                                }
                                dr.Close();
                            }
                            catch { }

                            try
                            {
                                string strSql = "SELECT * FROM " + Tables.DevicePropertyLong.TableName + " WHERE " + Tables.DevicePropertyLong.DeviceId + " IN (" + this.m_Device.Source.GetIdForString() + ")";
                                System.Data.IDataReader dr = dbConn.ExecuteReader(strSql);
                                while (dr.Read())
                                {
                                    DeviceProperty _DeviceProperty = this.Get(Function.ToInt(dr[Tables.DevicePropertyInt.DeviceId]), Function.ToString(dr[Tables.DevicePropertyInt.PropertyType]));
                                    if (_DeviceProperty != null)
                                    {
                                        _DeviceProperty.SetLongValue(Function.ToInt(dr[Tables.DevicePropertyLong.SerialNumber]), Function.ToLong(dr[Tables.DevicePropertyLong.DataValue]));
                                    }
                                }
                                dr.Close();
                            }
                            catch { }

                            try
                            {
                                string strSql = "SELECT * FROM " + Tables.DevicePropertyDecimal.TableName + " WHERE " + Tables.DevicePropertyDecimal.DeviceId + " IN (" + this.m_Device.Source.GetIdForString() + ")";
                                System.Data.IDataReader dr = dbConn.ExecuteReader(strSql);
                                while (dr.Read())
                                {
                                    DeviceProperty _DeviceProperty = this.Get(Function.ToInt(dr[Tables.DevicePropertyInt.DeviceId]), Function.ToString(dr[Tables.DevicePropertyInt.PropertyType]));
                                    if (_DeviceProperty != null)
                                    {
                                        _DeviceProperty.SetDecimalValue(Function.ToInt(dr[Tables.DevicePropertyDecimal.SerialNumber]), Function.ToDecimal(dr[Tables.DevicePropertyDecimal.DataValue]));
                                    }
                                }
                                dr.Close();
                            }
                            catch { }

                            try
                            {
                                string strSql = "SELECT * FROM " + Tables.DevicePropertyString.TableName + " WHERE " + Tables.DevicePropertyString.DeviceId + " IN (" + this.m_Device.Source.GetIdForString() + ")";
                                System.Data.IDataReader dr = dbConn.ExecuteReader(strSql);
                                while (dr.Read())
                                {
                                    DeviceProperty _DeviceProperty = this.Get(Function.ToInt(dr[Tables.DevicePropertyInt.DeviceId]), Function.ToString(dr[Tables.DevicePropertyInt.PropertyType]));
                                    if (_DeviceProperty != null)
                                    {
                                        _DeviceProperty.SetStringValue(Function.ToInt(dr[Tables.DevicePropertyString.SerialNumber]), Function.ToString(dr[Tables.DevicePropertyString.DataValue]));
                                    }
                                }
                                dr.Close();
                            }
                            catch { }

                            try
                            {
                                string strSql = "SELECT * FROM " + Tables.DevicePropertyText.TableName + " WHERE " + Tables.DevicePropertyText.DeviceId + " IN (" + this.m_Device.Source.GetIdForString() + ")";
                                System.Data.IDataReader dr = dbConn.ExecuteReader(strSql);
                                while (dr.Read())
                                {
                                    DeviceProperty _DeviceProperty = this.Get(Function.ToInt(dr[Tables.DevicePropertyInt.DeviceId]), Function.ToString(dr[Tables.DevicePropertyInt.PropertyType]));
                                    if (_DeviceProperty != null)
                                    {
                                        _DeviceProperty.SetTextValue(Function.ToInt(dr[Tables.DevicePropertyText.SerialNumber]), Function.ToString(dr[Tables.DevicePropertyText.DataValue]));
                                    }
                                }
                                dr.Close();
                            }
                            catch { }

                            try
                            {
                                string strSql = "SELECT * FROM " + Tables.DevicePropertyBytes.TableName + " WHERE " + Tables.DevicePropertyBytes.DeviceId + " IN (" + this.m_Device.Source.GetIdForString() + ")";
                                System.Data.IDataReader dr = dbConn.ExecuteReader(strSql);
                                while (dr.Read())
                                {
                                    DeviceProperty _DeviceProperty = this.Get(Function.ToInt(dr[Tables.DevicePropertyInt.DeviceId]), Function.ToString(dr[Tables.DevicePropertyInt.PropertyType]));
                                    if (_DeviceProperty != null)
                                    {
                                        if (dr[Tables.DevicePropertyBytes.DataValue] != null)
                                        {
                                            _DeviceProperty.SetBytesValue(Function.ToInt(dr[Tables.DevicePropertyBytes.SerialNumber]), (byte[])dr[Tables.DevicePropertyBytes.DataValue]);
                                        }
                                    }
                                }
                                dr.Close();
                            }
                            catch { }

                            dbConn.Close();
                        }
                    }
                }
            }

            private DeviceProperty Get(int deviceId, string propertyType)
            {
                if (deviceId == this.m_Device.DeviceId)
                {
                    return this.GetOrAdd(propertyType);
                }
                else
                {
                    AC.Base.Device _Device = this.m_Device.Source.GetDevice(deviceId);
                    if (_Device != null)
                    {
                        return _Device.m_Propertys.GetOrAdd(propertyType);
                    }
                    else
                    {
                        return null;
                    }
                }
            }

            private DeviceProperty GetOrAdd(string propertyType)
            {
                foreach (DeviceProperty p in this.m_Items)
                {
                    if (p.GetType().FullName.Equals(propertyType))
                    {
                        return p;
                    }
                }

                Type typ = Function.GetType(propertyType);
                System.Reflection.ConstructorInfo ciObject = typ.GetConstructor(new System.Type[] { });
                object obj = ciObject.Invoke(new object[] { });

                DeviceProperty _DeviceProperty = obj as DeviceProperty;
                _DeviceProperty.SetDevice(this.m_Device);
                this.m_Items.Add(_DeviceProperty);
                return _DeviceProperty;
            }

            internal DeviceProperty Get(DeviceProperty property)
            {
                foreach (DeviceProperty p in this.m_Items)
                {
                    if (p.GetType().Equals(property.GetType()))
                    {
                        return p;
                    }
                }
                return null;
            }

            internal void Add(DeviceProperty property)
            {
                this.m_Items.Add(property);
            }

            internal void Remove(DeviceProperty property)
            {
                this.m_Items.Remove(property);
            }

            /// <summary>
            /// 获取或设置指定索引处的属性。
            /// </summary>
            /// <param name="index"></param>
            /// <returns></returns>
            public DeviceProperty this[int index]
            {
                get
                {
                    return m_Items[index];
                }
            }

            /// <summary>
            /// 获取集合内指定类型的属性，如无该类型的属性则创建并返回该属性的新实例。
            /// </summary>
            /// <param name="propertyType"></param>
            /// <returns></returns>
            public DeviceProperty GetProperty(Type propertyType)
            {
                foreach (DeviceProperty p in this.m_Items)
                {
                    if (p.GetType().Equals(propertyType))
                    {
                        return p;
                    }
                }

                System.Reflection.ConstructorInfo ciObject = propertyType.GetConstructor(new System.Type[] { });
                object obj = ciObject.Invoke(new object[] { });

                DeviceProperty _DeviceProperty = obj as DeviceProperty;
                _DeviceProperty.SetDevice(this.m_Device);
                return _DeviceProperty;
            }

            /// <summary>
            /// 设置设备属性，如果属性不存在则添加到集合中，如果属性已存在则替换现有属性。
            /// </summary>
            /// <param name="item"></param>
            public void SetProperty(DeviceProperty item)
            {
                for (int intIndex = 0; intIndex < this.m_Items.Count; intIndex++)
                {
                    if (this.m_Items[intIndex].GetType().Equals(item.GetType()))
                    {
                        this.m_Items[intIndex] = item;
                        return;
                    }
                }
                this.m_Items.Add(item);
            }

            /// <summary>
            /// 将集合内的设备属性复制到一个数组中。
            /// </summary>
            /// <param name="array"></param>
            /// <param name="arrayIndex"></param>
            public void CopyTo(DeviceProperty[] array, int arrayIndex)
            {
                this.m_Items.CopyTo(array, arrayIndex);
            }

            /// <summary>
            /// 获取集合中实际包含的设备属性数量。
            /// </summary>
            public int Count
            {
                get { return this.m_Items.Count; }
            }

            /// <summary>
            /// 返回循环访问设备属性集合的枚举数。
            /// </summary>
            /// <returns></returns>
            public IEnumerator<DeviceProperty> GetEnumerator()
            {
                return this.m_Items.GetEnumerator();
            }


            /// <summary>
            /// 返回循环访问设备属性集合的枚举数。
            /// </summary>
            /// <returns></returns>
            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.m_Items.GetEnumerator();
            }

            /// <summary>
            /// 比较两个设备属性集合是否完全相同。
            /// </summary>
            /// <param name="propertys">设备属性集合。</param>
            /// <returns></returns>
            public bool Equals(IEnumerable<DeviceProperty> propertys)
            {
                int intIndex = 0;
                foreach (DeviceProperty source in propertys)
                {
                    bool bolIsFind = false;
                    foreach (DeviceProperty currently in this.m_Items)
                    {
                        if (source.Equals(currently))
                        {
                            bolIsFind = true;
                            break;
                        }
                    }

                    if (bolIsFind == false)
                    {
                        return false;
                    }
                    intIndex++;
                }
                return intIndex == this.m_Items.Count;
            }

            /// <summary>
            /// 获取当前对象的字符串表示形式。
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                return "共 " + this.Count + " 个属性";
            }
        }

        #endregion
    }
}
