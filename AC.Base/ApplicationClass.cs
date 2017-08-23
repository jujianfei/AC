using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using AC.Base.Tasks;
using AC.Base.Logs;
using AC.Base.ChannelServices;
using AC.Base.Database;

namespace AC.Base
{
    /// <summary>
    /// 接收到系统消息后产生的事件所调用的委托。
    /// </summary>
    /// <param name="ip">发送系统消息的网络地址。</param>
    /// <param name="receiveData">接收到的消息。</param>
    public delegate void ReceivedSystemMessageEventHandler(IPEndPoint ip, byte[] receiveData);

    /// <summary>
    /// 应用程序框架。
    /// </summary>
    public abstract class ApplicationClass
    {
        /// <summary>
        /// 实现 IControl 接口的控件类型集合。其中键存放 ControlAttribute 特性中的 ForType 对象，值存放实现 IControl 接口的类型集合。
        /// </summary>
        protected System.Collections.Generic.Dictionary<Type, List<Type>> ControlTypes = new Dictionary<Type, List<Type>>();

        /// <summary>
        /// 初始化应用程序框架。
        /// </summary>
        protected ApplicationClass()
        {
        }

        #region << 系统 >>

        private IDb m_Db = null;                                        //数据库连接对象。

        /// <summary>
        /// 获取一个可用的已打开的数据库连接。
        /// </summary>
        /// <returns>调用该方法的程序应注意，如果应用程序宿主未配置数据库连接则此处返回 null，表明正在运行的宿主程序不希望使用数据库。</returns>
        public virtual DbConnection GetDbConnection()
        {
            if (this.m_Db != null)
            {
                //*** 从数据库空闲连接池中提取一个可用连接，如无可用空闲连接则创建新连接。

                DbConnection conn = new DbConnection(this.m_Db);

                return conn;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 根据指定的数据库连接接口获取数据库连接。
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        protected DbConnection GetDbConnection(IDb db)
        {
            return new DbConnection(db);
        }

        /// <summary>
        /// 设置数据库连接。数据库连接不是必须的，如果不设置数据库连接，则系统内所有对象关系由应用程序自己建立。
        /// </summary>
        /// <param name="db"></param>
        protected virtual void SetDbConnection(IDb db)
        {
            this.m_Db = db;
        }

        private DbTypeCollection m_DbTypes;
        /// <summary>
        /// 获取系统支持的数据库类型。
        /// </summary>
        public DbTypeCollection DbTypes
        {
            get
            {
                if (this.m_DbTypes == null)
                {
                    this.m_DbTypes = new DbTypeCollection();
                }
                return this.m_DbTypes;
            }
        }

        private List<string> m_lsLoadFileName = new List<string>(); //已载入的文件名，防止重复引用的程序集出现在多个目录中被重复加载。

        /// <summary>
        /// 从当前宿主程序目录搜索 dll 文件加载程序集。
        /// </summary>
        public void SearchAssembly()
        {
            this.SearchAssembly(this.MainDirectory, new string[] {"ComponentFactory.Krypton.Design.dll","ComponentFactory.Krypton.Ribbon.dll", "ComponentFactory.Krypton.Docking.dll", "ComponentFactory.Krypton.Navigator.dll", "ComponentFactory.Krypton.Toolkit.dll", "ComponentFactory.Krypton.Workspace.dll", "Interop.DAO.dll", "System.Runtime.InteropServices.APIs.dll", "System.Windows.Forms.TreeListView.dll" });
        }

        /// <summary>
        /// 加载指定目录及其子目录内所有后缀名为 dll 的程序集。
        /// </summary>
        /// <param name="directory">目录路径。</param>
        /// <param name="excludeFileNames">需要排除加载的程序集文件名。（不区分大小写）</param>
        public void SearchAssembly(string directory, string[] excludeFileNames)
        {
            if (System.IO.Directory.Exists(directory))
            {
                System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(directory);
                this.SearchAssembly(di, excludeFileNames);
            }
        }

        private void SearchAssembly(System.IO.DirectoryInfo directory, string[] excludeFileNames)
        {
            System.IO.FileInfo[] fis = directory.GetFiles("*.dll");
            foreach (System.IO.FileInfo fi in fis)
            {
                bool bolIsExclude = false;
                if (excludeFileNames != null)
                {
                    for (int intIndex = 0; intIndex < excludeFileNames.Length; intIndex++)
                    {
                        if (excludeFileNames[intIndex].ToLower().Equals(fi.Name.ToLower()))
                        {
                            bolIsExclude = true;
                            break;
                        }
                    }
                }
                if (bolIsExclude == false && this.m_lsLoadFileName.Contains(fi.Name.ToLower()) == false)
                {
                    try
                    {
                        this.SearchAssembly(fi.FullName);
                        this.m_lsLoadFileName.Add(fi.Name.ToLower());
                    }
                    catch (Exception e)
                    {
                       // Console.WriteLine(e);
                    }
                }
            }

            foreach (System.IO.DirectoryInfo di in directory.GetDirectories())
            {
                this.SearchAssembly(di, excludeFileNames);
            }
        }

        /// <summary>
        /// 加载指定程序集文件。
        /// </summary>
        /// <param name="loadFileName">程序集文件，通常为 dll 文件。</param>
        public void SearchAssembly(string loadFileName)
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.LoadFrom(loadFileName);
            Type[] types = assembly.GetTypes();

            foreach (Type type in types)
            {
                //Console.WriteLine(type.FullName);
                this.AddType(type);
            }
        }

        /// <summary>
        /// 向应用程序框架内添加对象类型。
        /// </summary>
        /// <param name="type"></param>
        public virtual void AddType(Type type)
        {
            if (Function.IsInheritableBaseType(type, typeof(Device)) && type.IsAbstract == false)
            {
                this.AddDeviceType(type);
            }
            else if (Function.IsInheritableBaseType(type, typeof(Log)) && type.IsAbstract == false)
            {
                this.AddLogType(type);
            }
            else if (type.GetInterface(typeof(IControl).FullName) != null && type.IsAbstract == false)
            {
                object[] objAttrs = type.GetCustomAttributes(typeof(ControlAttribute), false);
                if (objAttrs.Length > 0)
                {
                    ControlAttribute attr = objAttrs[0] as ControlAttribute;
                    if (this.ControlTypes.ContainsKey(attr.ForType))
                    {
                        this.ControlTypes[attr.ForType].Add(type);
                    }
                    else
                    {
                        List<Type> lstType = new List<Type>();
                        lstType.Add(type);
                        this.ControlTypes.Add(attr.ForType, lstType);
                    }
                }
            }
            else if (type.GetInterface(typeof(AC.Base.DeviceSearchs.IDeviceFilter).FullName) != null && type.IsAbstract == false)
            {
                this.m_DeviceFilterTypes.Add(type);
            }
            else if (type.GetInterface(typeof(AC.Base.DeviceSearchs.IDeviceListItem).FullName) != null && type.IsAbstract == false)
            {
                this.m_DeviceListItemTypes.Add(type);
            }
            else if (type.GetInterface(typeof(AC.Base.DeviceSearchs.IDeviceOrder).FullName) != null && type.IsAbstract == false)
            {
                this.m_DeviceOrderTypes.Add(type);
            }
            else if (Function.IsInheritableBaseType(type, typeof(Task)) && type.IsAbstract == false)
            {
                this.TaskTypes.Add(new TaskType(this, type));
            }
            else if (Function.IsInheritableBaseType(type, typeof(TaskPeriod)) && type.IsAbstract == false)
            {
                this.TaskPeriodTypes.Add(new TaskPeriodType(this, type));
            }
            else if (type.GetInterface(typeof(IChannelService).FullName) != null && type.IsAbstract == false)
            {
                //通道服务
                object[] objAttrs = type.GetCustomAttributes(typeof(ChannelServiceTypeAttribute), false);
                if (objAttrs.Length > 0)
                {
                    this.ChannelServiceTypes.Add(new ChannelServiceType(type));
                }
            }
            else if (Function.IsInheritableBaseType(type, typeof(Classify)) && type.IsAbstract == false)
            {
                //分类
                object[] objAttrs = type.GetCustomAttributes(typeof(ClassifyTypeAttribute), false);
                if (objAttrs.Length > 0)
                {
                    this.ClassifyTypes.Add(new ClassifyType(this, type));
                }
            }
            else if (type.GetInterface(typeof(IDb).FullName) != null && type.IsAbstract == false)
            {
                //数据库连接
                object[] objAttrs = type.GetCustomAttributes(typeof(DbTypeAttribute), false);
                if (objAttrs.Length > 0)
                {
                    this.DbTypes.Add(new DbType(this, type));
                }
            }
        }

        private string m_MainDirectory;
        /// <summary>
        /// 当前宿主程序所在目录
        /// </summary>
        public string MainDirectory
        {
            get
            {
                if (m_MainDirectory == null)
                {
                    this.m_MainDirectory = this.GetType().Assembly.CodeBase.Substring(8);
                    this.m_MainDirectory = this.m_MainDirectory.Substring(0, this.m_MainDirectory.LastIndexOf("/") + 1);
                }
                return m_MainDirectory;
            }
        }

        /// <summary>
        /// 获取可写入临时文件的临时目录，该属性返回类似“C:\Users\User\AppData\Local\Temp\AMS\”或“C:\amsmainweb\Temp”的路径。
        /// </summary>
        public abstract string TemporaryDirectory { get; }

        /// <summary>
        /// 获取可访问临时目录的相对路径，该属性返回类似于“../Temp/”形式或长度为 0 的相对路径字符串。。
        /// </summary>
        public abstract string TemporaryDirectoryRelativePath { get; }

        /// <summary>
        /// 获取所服务业务对象的控件类型。
        /// </summary>
        /// <param name="forType">所服务的业务对象类型。该类型为 ControlAttribute.ForType 中指定的类型。</param>
        /// <returns>返回所有该类型的控件类型。</returns>
        public List<Type> GetControlTypes(Type forType)
        {
            if (this.ControlTypes.ContainsKey(forType))
            {
                return this.ControlTypes[forType];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 获取所服务业务对象的控件类型。
        /// </summary>
        /// <param name="forType">所服务的业务对象类型。该类型为 ControlAttribute.ForType 中指定的类型。</param>
        /// <param name="baseType">指定控件必须继承的基类。</param>
        /// <returns>控件类型，如无对应的控件类型则返回 null。</returns>
        public Type GetControlType(Type forType, Type baseType)
        {
            if (this.ControlTypes.ContainsKey(forType))
            {
                foreach (Type typ in this.ControlTypes[forType])
                {
                    if (Function.IsInheritableBaseType(typ, baseType))
                    {
                        return typ;
                    }
                }
            }
            return null;
        }

        private Dictionary<Type, ISystemConfig> m_SystemConfig = new Dictionary<Type, ISystemConfig>();
        /// <summary>
        /// 获取指定类型的系统配置参数。
        /// </summary>
        /// <param name="systemConfigType">实现 ISystemConfig 接口的系统配置参数类型。</param>
        /// <returns></returns>
        public ISystemConfig GetSystemConfig(Type systemConfigType)
        {
            if (this.m_SystemConfig.ContainsKey(systemConfigType))
            {
                return this.m_SystemConfig[systemConfigType];
            }
            else
            {
                System.Reflection.ConstructorInfo ci = systemConfigType.GetConstructor(new System.Type[] { });
                object objInstance = ci.Invoke(new object[] { });

                ISystemConfig config = objInstance as ISystemConfig;

                DbConnection dbConn = this.GetDbConnection();
                if (dbConn != null)
                {
                    try
                    {
                        string strSql = "SELECT " + Tables.SystemConfig.XMLConfig + " FROM " + Tables.SystemConfig.TableName + " WHERE " + Tables.SystemConfig.ConfigType + "=" + Function.SqlStr(systemConfigType.FullName, 250);
                        System.Data.IDataReader dr = dbConn.ExecuteReader(strSql);
                        if (dr.Read())
                        {
                            if (dr[Tables.SystemConfig.XMLConfig] != null && !(dr[Tables.SystemConfig.XMLConfig] is System.DBNull))
                            {
                                System.Xml.XmlDocument xmlDoc = new System.Xml.XmlDocument();
                                try
                                {
                                    xmlDoc.LoadXml(dr[Tables.SystemConfig.XMLConfig].ToString());

                                    if (xmlDoc.ChildNodes.Count > 0)
                                    {
                                        config.SetConfig(xmlDoc.ChildNodes[0]);
                                    }
                                }
                                catch (Exception e)
                                {
                                    throw e;
                                }
                            }
                        }
                        dr.Close();
                    }
                    finally
                    {
                        dbConn.Close();
                    }
                }

                this.m_SystemConfig.Add(systemConfigType, config);
                return config;
            }
        }

        /// <summary>
        /// 保存系统配置参数。
        /// </summary>
        /// <param name="systemConfig">实现 ISystemConfig 接口的系统配置参数。</param>
        public void SetSystemConfig(ISystemConfig systemConfig)
        {
            DbConnection dbConn = this.GetDbConnection();
            if (dbConn != null)
            {
                try
                {
                    string strConfig = null;
                    System.Xml.XmlDocument xmlDoc = new System.Xml.XmlDocument();
                    System.Xml.XmlNode xnConfig = systemConfig.GetConfig(xmlDoc);
                    if (xnConfig != null)
                    {
                        xmlDoc.AppendChild(xnConfig);
                        strConfig = xmlDoc.OuterXml;
                    }

                    bool bolIsInsert = true;

                    string strSql = "SELECT " + Tables.SystemConfig.ConfigType + " FROM " + Tables.SystemConfig.TableName + " WHERE " + Tables.SystemConfig.ConfigType + "=" + Function.SqlStr(systemConfig.GetType().FullName, 250);
                    System.Data.IDataReader dr = dbConn.ExecuteReader(strSql);
                    if (dr.Read())
                    {
                        bolIsInsert = false;
                    }
                    dr.Close();

                    if (bolIsInsert)
                    {
                        strSql = Function.SqlStr(systemConfig.GetType().FullName, 250) + "," + Function.SqlStr(strConfig);
                        strSql = "INSERT INTO " + Tables.SystemConfig.TableName + " (" + Tables.SystemConfig.ConfigType + "," + Tables.SystemConfig.XMLConfig + ") VALUES (" + strSql + ")";
                        dbConn.ExecuteNonQuery(strSql);
                    }
                    else
                    {
                        strSql = "UPDATE " + Tables.SystemConfig.TableName + " Set " + Tables.SystemConfig.XMLConfig + "=" + Function.SqlStr(strConfig) + " Where " + Tables.SystemConfig.ConfigType + "=" + Function.SqlStr(systemConfig.GetType().FullName, 250);
                        dbConn.ExecuteNonQuery(strSql);
                    }

                    if (this.m_SystemConfig.ContainsKey(systemConfig.GetType()))
                    {
                        this.m_SystemConfig[systemConfig.GetType()] = systemConfig;
                    }
                    else
                    {
                        this.m_SystemConfig.Add(systemConfig.GetType(), systemConfig);
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

        private bool m_SystemInitializeDateIsRead;
        private DateTime m_SystemInitializeDate;
        /// <summary>
        /// 系统初始日期，表明系统建立时间及系统内最早的数据产生时间。
        /// </summary>
        public DateTime SystemInitializeDate
        {
            get
            {
                //*** 以后会从配置文件中取
                if (this.m_SystemInitializeDateIsRead == false)
                {
                    this.m_SystemInitializeDate = new DateTime(DateTime.Today.Year - 5, 1, 1);
                    this.m_SystemInitializeDateIsRead = true;
                }
                return this.m_SystemInitializeDate;
            }
        }

        /// <summary>
        /// 刷新缓存的设备档案、分类档案、已配置的自动任务数据
        /// </summary>
        public virtual void RefreshData()
        {
            this.m_TaskGroups = null;
            this.m_slClassifys.Clear();
        }

        #endregion

        #region << 安全 >>

        private ISecuritySolution m_SecuritySolution;
        /// <summary>
        /// 账号登陆及密码验证安全方案。
        /// </summary>
        public ISecuritySolution SecuritySolution
        {
            get
            {
                if (this.m_SecuritySolution == null)
                {
                    this.m_SecuritySolution = new SecuritySolution();
                    this.m_SecuritySolution.SetApplication(this);

                }
                return this.m_SecuritySolution;
            }
        }

        /// <summary>
        /// 获取指定类型指定账号的配置参数。
        /// </summary>
        /// <param name="account">账号。</param>
        /// <param name="accountConfigType">实现 IAccountConfig 接口的账号配置参数类型。</param>
        /// <returns></returns>
        public IAccountConfig GetAccountConfig(IAccount account, Type accountConfigType)
        {
            System.Reflection.ConstructorInfo ci = accountConfigType.GetConstructor(new System.Type[] { });
            object objInstance = ci.Invoke(new object[] { });

            IAccountConfig config = objInstance as IAccountConfig;
            config.SetApplication(this);
            config.SetAccount(account);

            DbConnection dbConn = this.GetDbConnection();
            if (dbConn != null)
            {
                try
                {
                    string strSql = "SELECT " + Tables.AccountConfig.XMLConfig + " FROM " + Tables.AccountConfig.TableName + " WHERE " + Tables.AccountConfig.AccountCode + "=" + Function.SqlStr(account.AccountCode, 250) + " AND " + Tables.AccountConfig.ConfigType + "=" + Function.SqlStr(accountConfigType.FullName, 250);
                    System.Data.IDataReader dr = dbConn.ExecuteReader(strSql);
                    if (dr.Read())
                    {
                        if (dr[Tables.AccountConfig.XMLConfig] != null && !(dr[Tables.AccountConfig.XMLConfig] is System.DBNull))
                        {
                            System.Xml.XmlDocument xmlDoc = new System.Xml.XmlDocument();
                            try
                            {
                                xmlDoc.LoadXml(dr[Tables.AccountConfig.XMLConfig].ToString());

                                if (xmlDoc.ChildNodes.Count > 0)
                                {
                                    config.SetConfig(xmlDoc.ChildNodes[0]);
                                }
                            }
                            catch (Exception e)
                            {
                                throw e;
                            }
                        }
                    }
                    dr.Close();
                }
                finally
                {
                    dbConn.Close();
                }
            }

            return config;
        }

        /// <summary>
        /// 保存系统配置参数。
        /// </summary>
        /// <param name="account">账号。</param>
        /// <param name="accountConfig">实现 IAccountConfig 接口的系统配置参数。</param>
        public void SetAccountConfig(IAccount account, IAccountConfig accountConfig)
        {
            DbConnection dbConn = this.GetDbConnection();
            if (dbConn != null)
            {
                try
                {
                    string strConfig = null;
                    System.Xml.XmlDocument xmlDoc = new System.Xml.XmlDocument();
                    System.Xml.XmlNode xnConfig = accountConfig.GetConfig(xmlDoc);
                    if (xnConfig != null)
                    {
                        xmlDoc.AppendChild(xnConfig);
                        strConfig = xmlDoc.OuterXml;
                    }

                    bool bolIsInsert = true;

                    string strSql = "SELECT " + Tables.AccountConfig.ConfigType + " FROM " + Tables.AccountConfig.TableName + " WHERE " + Tables.AccountConfig.AccountCode + "=" + Function.SqlStr(account.AccountCode, 250) + " AND " + Tables.AccountConfig.ConfigType + "=" + Function.SqlStr(accountConfig.GetType().FullName, 250);
                    System.Data.IDataReader dr = dbConn.ExecuteReader(strSql);
                    if (dr.Read())
                    {
                        bolIsInsert = false;
                    }
                    dr.Close();

                    if (bolIsInsert)
                    {
                        strSql = Function.SqlStr(account.AccountCode, 250) + "," + Function.SqlStr(accountConfig.GetType().FullName, 250) + "," + Function.SqlStr(strConfig);
                        strSql = "INSERT INTO " + Tables.AccountConfig.TableName + " (" + Tables.AccountConfig.AccountCode + "," + Tables.AccountConfig.ConfigType + "," + Tables.AccountConfig.XMLConfig + ") VALUES (" + strSql + ")";
                        dbConn.ExecuteNonQuery(strSql);
                    }
                    else
                    {
                        strSql = "UPDATE " + Tables.AccountConfig.TableName + " Set " + Tables.AccountConfig.XMLConfig + "=" + Function.SqlStr(strConfig) + " Where " + Tables.AccountConfig.AccountCode + "=" + Function.SqlStr(account.AccountCode, 250) + " AND " + Tables.AccountConfig.ConfigType + "=" + Function.SqlStr(accountConfig.GetType().FullName, 250);
                        dbConn.ExecuteNonQuery(strSql);
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

        #endregion

        #region << 设备 >>

        //设备的排序缓存集合
        protected System.Collections.Generic.SortedList<int, Device> m_slDevices = new SortedList<int, Device>();

        /// <summary>
        /// 系统内设备缓存
        /// </summary>
        public System.Collections.Generic.SortedList<int, Device> slDevices
        {
            get
            {
                if (m_slDevices.Count > 0)
                    return m_slDevices;
                else
                    return null;
            }
        }

        //设备筛选器类型集合。
        private ICollection<Type> m_DeviceFilterTypes = new List<Type>();

        /// <summary>
        /// 获取指定的设备筛选器类型声明。
        /// </summary>
        /// <param name="deviceFilterType"></param>
        /// <returns></returns>
        public Type GetDeviceFilterType(string deviceFilterType)
        {
            foreach (Type typ in this.m_DeviceFilterTypes)
            {
                if (typ.FullName.Equals(deviceFilterType))
                {
                    return typ;
                }
            }
            return null;
        }

        /// <summary>
        /// 获取指定类型的设备筛选器实例。
        /// </summary>
        /// <param name="deviceFilterType">设备筛选器类型声明。</param>
        /// <param name="account">操作员账号。</param>
        /// <returns></returns>
        public AC.Base.DeviceSearchs.IDeviceFilter GetDeviceFilter(Type deviceFilterType, IAccount account)
        {
            System.Reflection.ConstructorInfo ciObject = deviceFilterType.GetConstructor(new System.Type[] { });
            object obj = ciObject.Invoke(new object[] { });
            AC.Base.DeviceSearchs.IDeviceFilter _DeviceFilter = obj as AC.Base.DeviceSearchs.IDeviceFilter;
            _DeviceFilter.SetApplication(this);
            if (_DeviceFilter is IUseAccount)
            {
                IUseAccount _UseAccount = _DeviceFilter as IUseAccount;
                _UseAccount.SetAccount(account);
            }
            return _DeviceFilter;
        }

        /// <summary>
        /// 获取系统内所有的设备筛选器。
        /// </summary>
        /// <param name="account">操作员账号。如果该参数为 null 则表示不需要与账号相关(实现 IUseAccount 接口)的设备筛选器。</param>
        /// <returns></returns>
        public IList<AC.Base.DeviceSearchs.IDeviceFilter> GetDeviceFilters(IAccount account)
        {
            List<AC.Base.DeviceSearchs.IDeviceFilter> lst = new List<DeviceSearchs.IDeviceFilter>();
            foreach (Type typ in this.m_DeviceFilterTypes)
            {
                if (account == null && typ.GetInterface(typeof(IUseAccount).FullName) != null)
                {
                    continue;
                }
                lst.Add(this.GetDeviceFilter(typ, account));
            }
            return lst;
        }

        //设备列表项类型集合。
        private ICollection<Type> m_DeviceListItemTypes = new List<Type>();

        /// <summary>
        /// 获取指定类型的设备列表项实例。
        /// </summary>
        /// <param name="deviceListItemType">设备列表项类型声明。</param>
        /// <param name="account">操作员账号。</param>
        /// <returns></returns>
        public AC.Base.DeviceSearchs.IDeviceListItem GetDeviceListItem(Type deviceListItemType, IAccount account)
        {
            System.Reflection.ConstructorInfo ciObject = deviceListItemType.GetConstructor(new System.Type[] { });
            object obj = ciObject.Invoke(new object[] { });
            AC.Base.DeviceSearchs.IDeviceListItem _DeviceListItem = obj as AC.Base.DeviceSearchs.IDeviceListItem;
            _DeviceListItem.SetApplication(this);
            if (_DeviceListItem is IUseAccount)
            {
                IUseAccount _UseAccount = _DeviceListItem as IUseAccount;
                _UseAccount.SetAccount(account);
            }
            return _DeviceListItem;
        }

        /// <summary>
        /// 获取系统内所有的设备列表项。
        /// </summary>
        /// <param name="account">操作员账号。如果该参数为 null 则表示不需要与账号相关(实现 IUseAccount 接口)的设备筛选器。</param>
        /// <returns></returns>
        public IList<AC.Base.DeviceSearchs.IDeviceListItem> GetDeviceListItems(IAccount account)
        {
            List<AC.Base.DeviceSearchs.IDeviceListItem> lst = new List<DeviceSearchs.IDeviceListItem>();
            foreach (Type typ in this.m_DeviceListItemTypes)
            {
                if (account == null && typ.GetInterface(typeof(IUseAccount).FullName) != null)
                {
                    continue;
                }
                lst.Add(this.GetDeviceListItem(typ, account));
            }
            return lst;
        }

        //设备排序器类型集合。
        private ICollection<Type> m_DeviceOrderTypes = new List<Type>();

        /// <summary>
        /// 获取指定类型的设备排序器实例。
        /// </summary>
        /// <param name="deviceOrderType">设备排序器类型声明。</param>
        /// <returns></returns>
        public AC.Base.DeviceSearchs.IDeviceOrder GetDeviceOrder(Type deviceOrderType)
        {
            System.Reflection.ConstructorInfo ciObject = deviceOrderType.GetConstructor(new System.Type[] { });
            object obj = ciObject.Invoke(new object[] { });
            AC.Base.DeviceSearchs.IDeviceOrder _DeviceOrder = obj as AC.Base.DeviceSearchs.IDeviceOrder;
            return _DeviceOrder;
        }

        /// <summary>
        /// 获取系统内所有设备排序器。
        /// </summary>
        /// <returns></returns>
        public IList<AC.Base.DeviceSearchs.IDeviceOrder> GetDeviceOrders()
        {
            List<AC.Base.DeviceSearchs.IDeviceOrder> lst = new List<DeviceSearchs.IDeviceOrder>();
            foreach (Type typ in this.m_DeviceOrderTypes)
            {
                lst.Add(this.GetDeviceOrder(typ));
            }
            return lst;
        }

        /// <summary>
        /// 获取设备在框架内缓存的实例。该方法默认返回 null。通常桌面应用程序在窗体各界面上显示同一设备时应保持这些设备引用的是同一个对象，以便在其中一个界面上对设备进行操作产生事件后可以在其它界面上即时反映出来，桌面应用程序框架应该重写该方法实现对象缓存策略。
        /// </summary>
        /// <param name="deviceId">设备编号</param>
        /// <returns>如果该编号设备存在于缓存内则返回设备实例，否则返回 null。</returns>
        protected internal virtual Device GetDeviceInstance(int deviceId)
        {
            return null;
        }

        /// <summary>
        /// 设置设备实例在框架内缓存。该方法默认不执行任何操作，如果当前应用需要对设备对象进行缓存则应重写该方法实现对象缓存策略。
        /// </summary>
        /// <param name="device"></param>
        protected internal virtual void SetDeviceInstance(Device device)
        {
        }

        private DeviceTypeSort m_DeviceTypeSort;
        /// <summary>
        /// 获取系统内所支持的所有设备类的分类集合。
        /// </summary>
        public DeviceTypeSort DeviceTypeSort
        {
            get
            {
                if (this.m_DeviceTypeSort == null)
                {
                    this.m_DeviceTypeSort = new DeviceTypeSort(this, "", null);
                }
                return this.m_DeviceTypeSort;
            }
        }

        System.Collections.Generic.List<Type> lsDeviceType = new List<Type>();
        /// <summary>
        /// 将继承自 Device 的设备类型添加至 DeviceTypes 属性中。对于未继承 Device、未提供无参数的构造函数、未添加 DeviceTypeAttribute 属性，或者 deviceType 是抽象类的类型将不予添加且不抛出异常。
        /// </summary>
        /// <param name="deviceType">继承自 Device 的设备</param>
        /// <returns>是否成功将指定的类型加入设备类型中。</returns>
        protected bool AddDeviceType(Type deviceType)
        {
            if (Function.IsInheritableBaseType(deviceType, typeof(Device)) && deviceType.IsAbstract == false)
            {
                System.Reflection.ConstructorInfo ci = deviceType.GetConstructor(new System.Type[] { });
                if (ci != null)
                {
                    object[] objAttr = deviceType.GetCustomAttributes(typeof(DeviceTypeAttribute), false);
                    if (objAttr != null && objAttr.Length > 0)
                    {
                        DeviceTypeAttribute attr = objAttr[0] as DeviceTypeAttribute;
                        string strSort = (attr.Sort != null ? attr.Sort.Replace('\\', '/').Trim() : "");

                        DeviceTypeSort sort = this.DeviceTypeSort.FindSort(strSort);
                        DeviceType type = new DeviceType(this, deviceType);
                        type.Sort = sort;
                        sort.DeviceTypes.Add(type);

                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// 当新建根节点或二级节点的设备后产生的事件。
        /// </summary>
        public event DeviceCreatedEventHandler DeviceCreated;

        internal void OnDeviceCreated(Device parentDevice, Device createdDevice)
        {
            if (this.DeviceCreated != null)
            {
                this.DeviceCreated(parentDevice, createdDevice);
            }
        }

        #endregion

        #region << 系统消息 >>

        private byte[] m_SystemTempId;
        private byte[] SystemTempId
        {
            get
            {
                if (this.m_SystemTempId == null)
                {
                    this.m_SystemTempId = new byte[4];

                    Random ran = new Random();
                    ran.NextBytes(this.m_SystemTempId);
                }
                return this.m_SystemTempId;
            }
        }

        private int m_SystemMessagePort = 36020;
        /// <summary>
        /// 获取或设置系统消息收发端口，默认为 36020。
        /// </summary>
        public int SystemMessagePort
        {
            get
            {
                return this.m_SystemMessagePort;
            }
            set
            {
                this.m_SystemMessagePort = value;
            }
        }

        private int m_SystemMessageBufferSize = 1024;
        /// <summary>
        /// 获取或设置系统消息数据缓冲区大小。
        /// </summary>
        public int SystemMessageBufferSize
        {
            get
            {
                return this.m_SystemMessageBufferSize;
            }
            set
            {
                this.m_SystemMessageBufferSize = value;
            }
        }

        private Socket m_SystemMessageSocket;
        private byte[] m_SystemMessageReceivedBuffer;                                   //接收数据缓冲
        private SocketAsyncEventArgs m_SystemMessageReceivedSocketAsync;                //接收数据使用的异步套接字
        private Stack<SocketAsyncEventArgs> m_SystemMessageSendSocketAsyncStack;        //发送数据使用的异步套接字栈
        private List<IPEndPoint> m_SystemMessageBroadcastAddress;                       //发送数据时需要发送的地址集合

        /// <summary>
        /// 开始接收/发送系统广播消息。
        /// </summary>
        protected void SystemMessageStart()
        {
            this.m_SystemMessageBroadcastAddress = new List<IPEndPoint>();

            System.Management.ManagementClass mc = new System.Management.ManagementClass("Win32_NetworkAdapterConfiguration");
            System.Management.ManagementObjectCollection nics = mc.GetInstances();
            foreach (System.Management.ManagementObject nic in nics)
            {
                if (Convert.ToBoolean(nic["ipEnabled"]) == true)
                {
                    try
                    {
                        String[] ipAddresss = nic["IPAddress"] as String[];
                        String[] ipSubnet = nic["IPSubnet"] as String[];
                        for (int intIndex = 0; intIndex < ipAddresss.Length; intIndex++)
                        {
                            string[] strIpAddress = ipAddresss[intIndex].Split(new char[] { '.' });
                            string[] strIpSubnet = ipSubnet[intIndex].Split(new char[] { '.' });
                            if (strIpAddress.Length == 4 && strIpSubnet.Length == 4)
                            {
                               // Console.WriteLine(ipAddresss[intIndex] + " " + ipSubnet[intIndex]);

                                byte[] bytIpAddress = new byte[4];
                                byte[] bytIpSubnet = new byte[4];
                                for (int intIpIndex = 0; intIpIndex < 4; intIpIndex++)
                                {
                                    bytIpAddress[intIpIndex] = Byte.Parse(strIpAddress[intIpIndex]);
                                    bytIpSubnet[intIpIndex] = Byte.Parse(strIpSubnet[intIpIndex]);
                                }

                                IPEndPoint ip = new IPEndPoint(new IPAddress(this.GetBroadcastAddress(bytIpAddress, bytIpSubnet)), this.SystemMessagePort);
                                this.m_SystemMessageBroadcastAddress.Add(ip);
                            }
                        }
                    }
                    catch
                    {
                    }
                }
            }

            IPEndPoint ipep = new IPEndPoint(System.Net.IPAddress.Any, this.SystemMessagePort);
            this.m_SystemMessageSocket = new Socket(ipep.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
            this.m_SystemMessageSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, true);

            try
            {
                this.m_SystemMessageSocket.Bind(ipep);
            }
            catch (System.Net.Sockets.SocketException ex)
            {
                if (ex.SocketErrorCode == SocketError.AddressAlreadyInUse)
                {
                    ipep = new IPEndPoint(System.Net.IPAddress.Any, 0);
                    this.m_SystemMessageSocket.Bind(ipep);

                    //向默认端口的服务发送消息，通知当前服务所监听的端口，请求其转发数据。
                }
                else
                {
                    throw ex;
                }
            }

            this.m_SystemMessageReceivedBuffer = new byte[this.SystemMessageBufferSize];
            this.m_SystemMessageReceivedSocketAsync = new SocketAsyncEventArgs();
            this.m_SystemMessageReceivedSocketAsync.RemoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
            this.m_SystemMessageReceivedSocketAsync.Completed += new EventHandler<SocketAsyncEventArgs>(SystemMessageReceived_Completed);
            this.m_SystemMessageReceivedSocketAsync.SetBuffer(this.m_SystemMessageReceivedBuffer, 0, this.m_SystemMessageReceivedBuffer.Length);

            this.m_SystemMessageSendSocketAsyncStack = new Stack<SocketAsyncEventArgs>();

            this.SystemMessageReceiveFrom();
        }

        private byte[] GetBroadcastAddress(byte[] ipAddress, byte[] ipSubnet)
        {
            ulong lngNetAddress = 0;
            ulong lngIpSubnet = 0;

            for (int intIndex = 0; intIndex < 4; intIndex++)
            {
                lngNetAddress <<= 8;
                lngNetAddress += (byte)(ipAddress[intIndex] & ipSubnet[intIndex]);

                lngIpSubnet <<= 8;
                lngIpSubnet += ipSubnet[intIndex];
            }

            lngIpSubnet = ~lngIpSubnet;

            ulong lngBroadcast = lngIpSubnet | lngNetAddress;

            byte[] bytBroadcast = new byte[4];
            for (int intIndex = 3; intIndex >= 0; intIndex--)
            {
                bytBroadcast[intIndex] = (byte)(lngBroadcast & 255);
                lngBroadcast >>= 8;
            }

            return bytBroadcast;
        }

        /// <summary>
        /// 停止发送/接收系统广播消息。
        /// </summary>
        protected void SystemMessageStop()
        {
            this.m_SystemMessageSocket.Shutdown(SocketShutdown.Both);
            this.m_SystemMessageSocket.Close();
            this.m_SystemMessageSocket = null;
        }

        /// <summary>
        /// 接收到系统消息后产生的事件。
        /// </summary>
        public event ReceivedSystemMessageEventHandler ReceivedSystemMessage;

        private void SystemMessageReceiveFrom()
        {
            if (this.m_SystemMessageSocket.ReceiveFromAsync(m_SystemMessageReceivedSocketAsync) == false)
            {
                throw new Exception("开始接收系统消息时，I/O 操作同步完成。");
            }
        }

        private void SystemMessageReceived_Completed(object sender, SocketAsyncEventArgs args)
        {
            if (args.SocketError == SocketError.Success && args.BytesTransferred > 0)
            {
                if (args.BytesTransferred >= 10)
                {
                    byte[] data = new byte[args.BytesTransferred];
                    Array.Copy(args.Buffer, args.Offset, data, 0, data.Length);

                    if (data[0] == 0xAA && data[data.Length - 1] == 0x55)
                    {
                        bool bolIsSelf = true;
                        for (int intIndex = 0; intIndex < 4; intIndex++)
                        {
                            if (data[intIndex + 1] != this.SystemTempId[intIndex])
                            {
                                bolIsSelf = false;
                                break;
                            }
                        }

                        if (bolIsSelf == false)
                        {
                            int intMessageType = (data[5] << 8) + data[6];
                            SystemMessageTypeOptions messageType = (SystemMessageTypeOptions)intMessageType;

                            switch (messageType)
                            {
                                case SystemMessageTypeOptions.DeviceCreated:
                                case SystemMessageTypeOptions.DeviceUpdated:
                                case SystemMessageTypeOptions.DeviceDeleted:
                                    int intDeviceId = 0;
                                    intDeviceId += data[9] << 24;
                                    intDeviceId += data[10] << 16;
                                    intDeviceId += data[11] << 8;
                                    intDeviceId += data[12];

                                    if (messageType == SystemMessageTypeOptions.DeviceCreated)
                                    {
                                        if (this.DeviceCreated != null)
                                        {
                                        }
                                    }
                                    else if (messageType == SystemMessageTypeOptions.DeviceUpdated)
                                    {
                                        Device device = this.GetDeviceInstance(intDeviceId);
                                        if (device != null)
                                        {
                                            device.OnUpdated(false);
                                        }
                                    }
                                    else if (messageType == SystemMessageTypeOptions.DeviceDeleted)
                                    {
                                        Device device = this.GetDeviceInstance(intDeviceId);
                                        if (device != null)
                                        {
                                            device.OnDeleted(false);
                                        }
                                    }
                                    break;

                                case SystemMessageTypeOptions.CustomData:
                                    if (this.ReceivedSystemMessage != null)
                                    {
                                        int intLength = 0;
                                        intLength += data[7] << 8;
                                        intLength += data[8];

                                        byte[] bytReceiveData = new byte[intLength];
                                        for (int intIndex = 0; intIndex < intLength; intIndex++)
                                        {
                                            bytReceiveData[intIndex] = data[intIndex + 9];
                                        }
                                        this.ReceivedSystemMessage(args.RemoteEndPoint as IPEndPoint, bytReceiveData);
                                    }
                                    break;

                                default:
                                    throw new Exception("未支持的系统消息类型：" + messageType.ToString());
                            }

                           // Console.WriteLine(args.RemoteEndPoint + " : " + BitConverter.ToString(data).Replace("-", " "));
                        }
                    }
                }
            }
            else
            {
                throw new Exception("收到的系统消息数据异步套接字结果是 " + args.SocketError.ToString() + "，数据长度为 " + args.BytesTransferred.ToString() + " 字节。");
            }

            this.SystemMessageReceiveFrom();
        }

        /// <summary>
        /// 以网络广播形式发送系统消息。如果计算机上有多块网卡或多个网络地址，则依次发送。
        /// </summary>
        /// <param name="sendData">欲发送的消息数据。</param>
        protected void SendSystemMessage(byte[] sendData)
        {
            this.SendSystemMessage(SystemMessageTypeOptions.CustomData, sendData);
        }

        internal void SendSystemMessage(SystemMessageTypeOptions messageType, byte[] sendData)
        {
            if (this.m_SystemMessageSocket != null)
            {
                int intIndex = 0;
                byte[] bytData = new byte[sendData.Length + 10];
                bytData[intIndex++] = 0xAA;

                bytData[intIndex++] = this.SystemTempId[0];
                bytData[intIndex++] = this.SystemTempId[1];
                bytData[intIndex++] = this.SystemTempId[2];
                bytData[intIndex++] = this.SystemTempId[3];

                int intMessageType = (int)messageType;
                bytData[intIndex++] = (byte)((intMessageType >> 8) % 0x100);
                bytData[intIndex++] = (byte)(intMessageType % 0x100);

                bytData[intIndex++] = (byte)((sendData.Length >> 8) % 0x100);
                bytData[intIndex++] = (byte)(sendData.Length % 0x100);

                foreach (byte bytSendData in sendData)
                {
                    bytData[intIndex++] = bytSendData;
                }
                bytData[intIndex++] = 0x55;

                foreach (IPEndPoint ip in this.m_SystemMessageBroadcastAddress)
                {
                    SocketAsyncEventArgs args = null;
                    if (this.m_SystemMessageSendSocketAsyncStack.Count > 0)
                    {
                        args = this.m_SystemMessageSendSocketAsyncStack.Pop();
                    }
                    else
                    {
                        args = new SocketAsyncEventArgs();
                        args.Completed += new EventHandler<SocketAsyncEventArgs>(SystemMessageSend_Completed);
                    }
                    args.RemoteEndPoint = ip;
                    args.SetBuffer(bytData, 0, bytData.Length);

                    if (this.m_SystemMessageSocket.SendToAsync(args) == false)
                    {
                        //throw new Exception("开始发送系统消息时，I/O 操作同步完成。");
                    }
                }
            }
        }

        private void SystemMessageSend_Completed(object sender, SocketAsyncEventArgs args)
        {
            this.m_SystemMessageSendSocketAsyncStack.Push(args);
        }

        #endregion

        #region << 日志 >>

        private LogTypeSort m_LogTypeSort;
        /// <summary>
        /// 获取系统内所支持的所有日志类的分类集合。
        /// </summary>
        public LogTypeSort LogTypeSort
        {
            get
            {
                if (this.m_LogTypeSort == null)
                {
                    this.m_LogTypeSort = new LogTypeSort(this, "", null);
                }
                return this.m_LogTypeSort;
            }
        }

        System.Collections.Generic.List<Type> lsLogType = new List<Type>();
        /// <summary>
        /// 将继承自 Log 的日志类型添加至 LogTypes 属性中。对于未继承 Log、未提供无参数的构造函数、未添加 LogTypeAttribute 属性，或者 logType 是抽象类的类型将不予添加且不抛出异常。
        /// </summary>
        /// <param name="logType">继承自 Log 的日志</param>
        /// <returns>是否成功将指定的类型加入日志类型中。</returns>
        protected bool AddLogType(Type logType)
        {
            if (Function.IsInheritableBaseType(logType, typeof(Log)) && logType.IsAbstract == false)
            {
                System.Reflection.ConstructorInfo ci = logType.GetConstructor(new System.Type[] { typeof(ApplicationClass) });
                if (ci != null)
                {
                    object[] objAttr = logType.GetCustomAttributes(typeof(LogTypeAttribute), false);
                    if (objAttr != null && objAttr.Length > 0)
                    {
                        LogTypeAttribute attr = objAttr[0] as LogTypeAttribute;
                        string strSort = (attr.Sort != null ? attr.Sort.Replace('\\', '/').Trim() : "");

                        LogTypeSort sort = this.LogTypeSort.FindSort(strSort);
                        LogType type = new LogType(this, logType);
                        type.Sort = sort;
                        sort.LogTypes.Add(type);

                        return true;
                    }
                }
            }

            return false;
        }

        #endregion

        #region << 通道 >>

        private ChannelServiceTypeCollection m_ChannelServiceTypes;
        /// <summary>
        /// 获取系统内可用的通道服务类型。
        /// </summary>
        public ChannelServiceTypeCollection ChannelServiceTypes
        {
            get
            {
                if (this.m_ChannelServiceTypes == null)
                {
                    this.m_ChannelServiceTypes = new ChannelServiceTypeCollection();
                }
                return this.m_ChannelServiceTypes;
            }
        }

        #endregion

        #region << 自动任务 >>

        private TaskTypeCollection m_TaskTypes;
        /// <summary>
        /// 任务类型。
        /// </summary>
        public TaskTypeCollection TaskTypes
        {
            get
            {
                if (this.m_TaskTypes == null)
                {
                    this.m_TaskTypes = new TaskTypeCollection();
                }
                return this.m_TaskTypes;
            }
        }

        private TaskPeriodTypeCollection m_TaskPeriodTypes;
        /// <summary>
        /// 任务周期类型。
        /// </summary>
        public TaskPeriodTypeCollection TaskPeriodTypes
        {
            get
            {
                if (this.m_TaskPeriodTypes == null)
                {
                    this.m_TaskPeriodTypes = new TaskPeriodTypeCollection();
                }
                return this.m_TaskPeriodTypes;
            }
        }

        private TaskGroupCollection m_TaskGroups;
        /// <summary>
        /// 获取系统内已配置的任务组。
        /// </summary>
        public TaskGroupCollection TaskGroups
        {
            get
            {
                if (this.m_TaskGroups == null)
                {
                    this.m_TaskGroups = new TaskGroupCollection(this);
                }
                return this.m_TaskGroups;
            }
        }


        #endregion

        #region << 分类 >>

        private ClassifyTypeCollection m_ClassifyTypes;
        /// <summary>
        /// 可用的分类类型集合。
        /// </summary>
        public ClassifyTypeCollection ClassifyTypes
        {
            get
            {
                if (this.m_ClassifyTypes == null)
                {
                    this.m_ClassifyTypes = new ClassifyTypeCollection();
                }
                return this.m_ClassifyTypes;
            }
        }

        private System.Collections.Generic.SortedList<int, Classify> m_slClassifys = new SortedList<int, Classify>();

        /// <summary>
        /// 获取分类在框架内缓存的实例。该方法默认返回 null。
        /// </summary>
        /// <param name="classifyId">分类编号</param>
        /// <returns>如果该编号分类存在于缓存内则返回分类实例，否则返回 null。</returns>
        internal Classify GetClassifyInstance(int classifyId)
        {
            if (this.m_slClassifys.ContainsKey(classifyId))
            {
                return this.m_slClassifys[classifyId];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 设置分类实例在框架内缓存。
        /// </summary>
        /// <param name="classify"></param>
        internal void SetClassifyInstance(Classify classify)
        {
            if (classify.ClassifyId > 0)
            {
                if (this.m_slClassifys.ContainsKey(classify.ClassifyId) == false)
                {
                    this.m_slClassifys.Add(classify.ClassifyId, classify);
                }
            }
        }

        #endregion
    }
}