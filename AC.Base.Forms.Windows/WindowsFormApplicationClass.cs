using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AC.Base.Forms;
using AC.Base.Database;
using System.Windows.Forms;

namespace AC.Base.Forms.Windows
{
    /// <summary>
    /// 当视图被加载到程序主窗体并显示后产生的事件所调用的委托。
    /// </summary>
    /// <param name="view">发生加载事件的视图。</param>
    /// <param name="viewDock">视图停靠位置。</param>
    public delegate void ViewLoadedEventHandler(ViewBase view, ViewDockOptions viewDock);

    /// <summary>
    /// 当视图的可见状态发生改变后产生的事件所调用的委托。
    /// </summary>
    /// <param name="view">发生可见状态改变的视图。</param>
    public delegate void ViewVisibleChangedEventHandler(ViewBase view);

    /// <summary>
    /// 当视图被关闭后产生的事件所调用的委托。
    /// </summary>
    /// <param name="view">被关闭的视图。</param>
    public delegate void ViewClosedEventHandler(ViewBase view);

    /// <summary>
    /// Windows 桌面应用程序框架。
    /// </summary>
    public abstract class WindowsFormApplicationClass : FormApplicationClass
    {
        #region  << 初始化 >>

        /// <summary>
        /// Windows 桌面应用程序框架。
        /// </summary>
        protected WindowsFormApplicationClass()
        {
        }

        /// <summary>
        /// 当前操作员账号。
        /// </summary>
        public abstract IAccount CurrentAccount { get; }

        private string m_TemporaryDirectory;
        /// <summary>
        /// 获取可写入临时文件的临时目录。
        /// </summary>
        public override string TemporaryDirectory
        {
            get
            {
                if (this.m_TemporaryDirectory == null)
                {
                    this.m_TemporaryDirectory = System.IO.Path.GetTempPath() + "AMS\\";
                    if (System.IO.Directory.Exists(this.m_TemporaryDirectory))
                    {
                        System.IO.Directory.Delete(this.m_TemporaryDirectory, true);
                    }

                    System.Threading.Thread.Sleep(100);
                    System.IO.Directory.CreateDirectory(this.m_TemporaryDirectory);
                }
                return this.m_TemporaryDirectory;
            }
        }

        /// <summary>
        /// 获取可访问临时目录的相对路径，该属性返回类似于“../Temp/”形式或长度为 0 的相对路径字符串。。
        /// </summary>
        public override string TemporaryDirectoryRelativePath
        {
            get { return this.TemporaryDirectory; }
        }

        /// <summary>
        /// 向应用程序框架内添加对象类型。
        /// </summary>
        /// <param name="type">欲添加的类型的声明。</param>
        public override void AddType(Type type)
        {
            if (type.GetInterface(typeof(IToolbar).FullName) != null)                                                               //实现了 IToolbar 工具栏插件接口
            {
                if (type.IsAbstract == false && Function.IsInheritableBaseType(type, typeof(System.Windows.Forms.ToolStrip)))       //不是抽象类，并且继承自System.Windows.Forms.ToolStrip
                {
                    System.Reflection.ConstructorInfo ci = type.GetConstructor(new System.Type[] { });
                    if (ci != null)                                                                                                 //提供无参数的构造函数
                    {
                        object[] objAttr = type.GetCustomAttributes(typeof(ToolbarTypeAttribute), false);
                        if (objAttr != null && objAttr.Length > 0)                                                                  //添加了 ToolbarTypeAttribute 特性。
                        {
                            bool bolIsAdd = true;

                            for (int intIndex = 0; intIndex < this.ToolbarTypes.Count; intIndex++)
                            {
                                ToolbarType itemType = this.ToolbarTypes[intIndex];
                                if (Function.IsInheritableBaseType(type, itemType.Type))
                                {
                                    this.ToolbarTypes[intIndex] = null;
                                    this.ToolbarTypes[intIndex] = new ToolbarType(this, type);
                                    bolIsAdd = false;
                                    break;
                                }
                                else if (Function.IsInheritableBaseType(itemType.Type, type))
                                {
                                    bolIsAdd = false;
                                    break;
                                }
                            }

                            if (bolIsAdd)
                            {
                                this.ToolbarTypes.Add(new ToolbarType(this, type));
                            }
                        }
                    }
                }
            }
            else if (type.GetInterface(typeof(IStatusBarItem).FullName) != null)                                                    //实现了 IStatusBarItem 状态栏项插件接口
            {
                if (type.IsAbstract == false && Function.IsInheritableBaseType(type, typeof(System.Windows.Forms.ToolStripItem)))   //不是抽象类，并且继承自System.Windows.Forms.ToolStripItem
                {
                    System.Reflection.ConstructorInfo ci = type.GetConstructor(new System.Type[] { });
                    if (ci != null)                                                                                                 //提供无参数的构造函数
                    {
                        object[] objAttr = type.GetCustomAttributes(typeof(StatusBarItemTypeAttribute), false);
                        if (objAttr != null && objAttr.Length > 0)                                                                  //添加了 StatusBarItemTypeAttribute 特性。
                        {
                            bool bolIsAdd = true;

                            for (int intIndex = 0; intIndex < this.StatusBarItemTypes.Count; intIndex++)
                            {
                                StatusBarItemType itemType = this.StatusBarItemTypes[intIndex];
                                if (Function.IsInheritableBaseType(type, itemType.Type))
                                {
                                    this.StatusBarItemTypes[intIndex] = null;
                                    this.StatusBarItemTypes[intIndex] = new StatusBarItemType(this, type);
                                    bolIsAdd = false;
                                    break;
                                }
                                else if (Function.IsInheritableBaseType(itemType.Type, type))
                                {
                                    bolIsAdd = false;
                                    break;
                                }
                            }

                            if (bolIsAdd)
                            {
                                this.StatusBarItemTypes.Add(new StatusBarItemType(this, type));
                            }
                        }
                    }
                }
            }
            else
            {
                base.AddType(type);
            }
        }

        /// <summary>
        /// 获取指定查询参数类型的实现 IParameterControl 接口的控件类类型。
        /// </summary>
        /// <param name="parameterType"></param>
        /// <returns></returns>
        internal Type GetParameterControlType(Type parameterType)
        {
            if (base.ControlTypes.ContainsKey(parameterType))
            {
                foreach (Type typControlType in base.ControlTypes[parameterType])
                {
                    if (typControlType.GetInterface(typeof(IParameterControl).FullName) != null && Function.IsInheritableBaseType(typControlType, typeof(System.Windows.Forms.Control)))
                    {
                        return typControlType;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// 刷新缓存的设备档案、分类档案、已配置的自动任务数据
        /// </summary>
        public override void RefreshData()
        {
            base.RefreshData();

            m_slDevices.Clear();
        }

        #endregion

        //全局菜单
        //全局工具栏
        //设备菜单
        //设备属性
        //设备分类
        //设备高级属性
        //设备高级分类
        //添加一个全局HTML功能项，并加到全局菜单或全局工具栏中
        //自定义全局菜单项、全局工具栏项
        //菜单
        //工具栏
        //视图窗口
        //状态栏

        #region << 视图 >>

        /// <summary>
        /// 加载指定插件类型的全局视图，如果视图已加载则将该视图置于可见状态。
        /// </summary>
        /// <param name="globalPluginType">实现 IGlobalPlugin 接口的插件类。</param>
        /// <param name="parameterValues">需要为该界面插件设置的参数默认值，如无参数可不设置。</param>
        /// <returns></returns>
        public GlobalPluginView LoadView(Type globalPluginType, params ParameterValue[] parameterValues)
        {
            GlobalPluginType pluginType = base.GetGlobalPluginType(this.CurrentAccount, globalPluginType, new Type[] { typeof(System.Windows.Forms.Control), typeof(System.Windows.Forms.ToolStripItem) });
            if (pluginType != null)
            {
                return this.LoadView(pluginType, parameterValues);
            }
            else
            {
                throw new Exception("未发现可用的全局插件“" + globalPluginType + "”");
            }
        }

        /// <summary>
        /// 加载指定插件类型的全局视图，如果视图已加载则将该视图置于可见状态。
        /// </summary>
        /// <param name="pluginType">全局插件类型。</param>
        /// <param name="parameterValues">需要为该界面插件设置的参数默认值，如无参数可不设置。</param>
        /// <returns>成功加载至界面中的全局视图。</returns>
        public GlobalPluginView LoadView(GlobalPluginType pluginType, params ParameterValue[] parameterValues)
        {
            GlobalPluginView view = null;

            foreach (ViewBase v in this.Views)
            {
                if (v is GlobalPluginView)
                {
                    GlobalPluginView dv = v as GlobalPluginView;
                    if (dv.PluginType.Equals(pluginType))
                    {
                        view = v as GlobalPluginView;
                        break;
                    }
                }
            }

            if (view == null)
            {
                view = new GlobalPluginView(this, pluginType);
                if (parameterValues != null && parameterValues.Length > 0)
                {
                    view.SetParameterValue(parameterValues);
                }
                this.Views.Load(view, ViewDockOptions.Center);
            }
            else
            {
                if (parameterValues != null && parameterValues.Length > 0)
                {
                    view.SetParameterValue(parameterValues);
                }
                view.Focus();
            }

            return view;
        }

        /// <summary>
        /// 跳转至指定分类的插件。
        /// </summary>
        /// <param name="classifyPluginType">分类插件类型声明。</param>
        /// <param name="classifys">分类集合。</param>
        /// <param name="parameterValues">需要为该界面插件设置的参数默认值，如无参数可不设置。</param>
        /// <returns></returns>
        public ClassifyPluginView LoadView(Type classifyPluginType, Classify[] classifys, params ParameterValue[] parameterValues)
        {
            ClassifyPluginType pluginType = base.GetClassifyPluginType(this.CurrentAccount, classifyPluginType, new Type[] { typeof(System.Windows.Forms.Control), typeof(System.Windows.Forms.ToolStripItem) }, classifys);
            if (pluginType != null)
            {
                return this.LoadView(pluginType, classifys, parameterValues);
            }
            else
            {
                throw new Exception("未发现可用的分类插件“" + classifyPluginType + "”");
            }
        }

        /// <summary>
        /// 跳转至指定分类的插件。
        /// </summary>
        /// <param name="pluginType">分类插件。</param>
        /// <param name="classifys">分类集合。</param>
        /// <param name="parameterValues">需要为该界面插件设置的参数默认值，如无参数可不设置。</param>
        /// <returns></returns>
        public ClassifyPluginView LoadView(ClassifyPluginType pluginType, Classify[] classifys, params ParameterValue[] parameterValues)
        {
            ClassifyPluginView view = null;

            foreach (ViewBase v in this.Views)
            {
                if (v is ClassifyPluginView)
                {
                    ClassifyPluginView dv = v as ClassifyPluginView;
                    if (dv.PluginType.Equals(pluginType))
                    {
                        if (dv.Classifys.Length == classifys.Length)
                        {
                            bool bolIsEqually = true;
                            for (int intIndex = 0; intIndex < classifys.Length; intIndex++)
                            {
                                if (dv.Classifys[intIndex].Equals(classifys[intIndex]) == false)
                                {
                                    bolIsEqually = false;
                                    break;
                                }
                            }

                            if (bolIsEqually)
                            {
                                view = v as ClassifyPluginView;
                                break;
                            }
                        }
                    }
                }
            }

            if (view == null)
            {
                view = new ClassifyPluginView(this, pluginType, classifys);
                if (parameterValues != null && parameterValues.Length > 0)
                {
                    view.SetParameterValue(parameterValues);
                }
                this.Views.Load(view, ViewDockOptions.Center);
            }
            else
            {
                if (parameterValues != null && parameterValues.Length > 0)
                {
                    view.SetParameterValue(parameterValues);
                }
                view.Focus();
            }

            return view;
        }

        /// <summary>
        /// 跳转至指定设备的插件。
        /// </summary>
        /// <param name="devicePluginType">设备插件类型声明。</param>
        /// <param name="devices">设备集合。</param>
        /// <param name="parameterValues">需要为该界面插件设置的参数默认值，如无参数可不设置。</param>
        /// <returns></returns>
        public DevicePluginView LoadView(Type devicePluginType, Device[] devices, params ParameterValue[] parameterValues)
        {
            DevicePluginType pluginType = base.GetDevicePluginType(this.CurrentAccount, devicePluginType, new Type[] { typeof(System.Windows.Forms.Control), typeof(System.Windows.Forms.ToolStripItem) }, devices);
            if (pluginType != null)
            {
                return this.LoadView(pluginType, devices, parameterValues);
            }
            else
            {
                throw new Exception("未发现可用的设备插件“" + devicePluginType + "”");
            }
        }

        /// <summary>
        /// 跳转至指定设备的插件。
        /// </summary>
        /// <param name="pluginType">设备插件。</param>
        /// <param name="devices">设备集合。</param>
        /// <param name="parameterValues">需要为该界面插件设置的参数默认值，如无参数可不设置。</param>
        /// <returns></returns>
        public DevicePluginView LoadView(DevicePluginType pluginType, Device[] devices, params ParameterValue[] parameterValues)
        {
            DevicePluginView view = null;

            foreach (ViewBase v in this.Views)
            {
                if (v is DevicePluginView)
                {
                    DevicePluginView dv = v as DevicePluginView;
                    if (dv.PluginType.Equals(pluginType))
                    {
                        if (dv.Devices.Length == devices.Length)
                        {
                            bool bolIsEqually = true;
                            for (int intIndex = 0; intIndex < devices.Length; intIndex++)
                            {
                                if (dv.Devices[intIndex].Equals(devices[intIndex]) == false)
                                {
                                    bolIsEqually = false;
                                    break;
                                }
                            }

                            if (bolIsEqually)
                            {
                                view = v as DevicePluginView;
                                break;
                            }
                        }
                    }
                }
            }

            if (view == null)
            {
                view = new DevicePluginView(this, pluginType, devices);
                if (parameterValues != null && parameterValues.Length > 0)
                {
                    view.SetParameterValue(parameterValues);
                }
                this.Views.Load(view, ViewDockOptions.Center);
            }
            else
            {
                if (parameterValues != null && parameterValues.Length > 0)
                {
                    view.SetParameterValue(parameterValues);
                }
                view.Focus();
            }

            return view;
        }

        /// <summary>
        /// 视图请求可视并获得焦点时产生的事件所调用的委托。
        /// </summary>
        /// <param name="view">请求可视并获得焦点的视图。</param>
        protected delegate void ViewRequestFocusEventHandler(ViewBase view);

        /// <summary>
        /// 视图请求关闭时产生的事件所调用的委托。
        /// </summary>
        /// <param name="view">请求关闭的视图。</param>
        protected delegate void ViewRequestCloseEventHandler(ViewBase view);

        /// <summary>
        /// 应用程序中已打开的视图集合。
        /// </summary>
        public abstract IViewCollection Views { get; }

        /// <summary>
        /// 当视图被加载到程序主窗体后产生的事件。
        /// </summary>
        public event ViewLoadedEventHandler ViewLoaded;

        /// <summary>
        /// 引发视图被加载到程序主窗体后的事件。
        /// </summary>
        /// <param name="view">发生加载事件的视图。</param>
        /// <param name="viewDock">视图停靠位置。</param>
        protected virtual void OnViewLoaded(ViewBase view, ViewDockOptions viewDock)
        {
            if (this.ViewLoaded != null)
            {
                this.ViewLoaded(view, viewDock);
            }
        }

        /// <summary>
        /// 当视图的可见状态发生改变后产生的事件。
        /// </summary>
        public event ViewVisibleChangedEventHandler ViewVisibleChanged;

        /// <summary>
        /// 引发视图可见状态发生改变后的事件。
        /// </summary>
        /// <param name="view">可见状态发生改变的视图。</param>
        /// <param name="visible">是否可见。</param>
        protected virtual void OnViewVisibleChanged(ViewBase view, bool visible)
        {
            view.OnViewVisibleChanged(visible);

            if (this.ViewVisibleChanged != null)
            {
                this.ViewVisibleChanged(view);
            }
        }

        /// <summary>
        /// 当视图被关闭后产生的事件。
        /// </summary>
        public event ViewClosedEventHandler ViewClosed;

        /// <summary>
        /// 引发视图被关闭后的事件。
        /// </summary>
        /// <param name="view"></param>
        protected virtual void OnViewClosed(ViewBase view)
        {
            view.OnViewClosed();

            if (ViewClosed != null)
            {
                this.ViewClosed(view);
            }
        }

        /// <summary>
        /// 视图请求可视并获得焦点时产生的事件。
        /// </summary>
        protected event ViewRequestFocusEventHandler ViewRequestFocus;

        internal void OnViewRequestFocus(ViewBase view)
        {
            if (this.ViewRequestFocus != null)
            {
                this.ViewRequestFocus(view);
            }
        }

        /// <summary>
        /// 视图请求关闭时产生的事件。
        /// </summary>
        protected event ViewRequestCloseEventHandler ViewRequestClose;

        internal void OnViewRequestClose(ViewBase view)
        {
            if (this.ViewRequestClose != null)
            {
                this.ViewRequestClose(view);
            }
        }

        #endregion

        #region << 全局 >>

        /// <summary>
        /// 获取指定操作员可用的所有全局插件类型。
        /// </summary>
        /// <returns></returns>
        public PluginTypeCollection GetGlobalPluginTypes()
        {
            return base.GetGlobalPluginTypes(this.CurrentAccount, new Type[] { typeof(System.Windows.Forms.Control), typeof(System.Windows.Forms.ToolStripItem) });
        }

        /// <summary>
        /// 获取指定的全局插件。
        /// </summary>
        /// <param name="globalPluginType"></param>
        /// <returns></returns>
        public GlobalPluginType GetGlobalPluginType(Type globalPluginType)
        {
            return base.GetGlobalPluginType(this.CurrentAccount, globalPluginType, new Type[] { typeof(System.Windows.Forms.Control), typeof(System.Windows.Forms.ToolStripItem) });
        }

        #endregion

        #region << 工具栏 >>

        private ToolbarType.ToolbarTypeCollection m_ToolbarTypes;
        /// <summary>
        /// 状态栏插件类型描述集合。
        /// </summary>
        public ToolbarType.ToolbarTypeCollection ToolbarTypes
        {
            get
            {
                if (this.m_ToolbarTypes == null)
                {
                    this.m_ToolbarTypes = new ToolbarType.ToolbarTypeCollection();
                }
                return this.m_ToolbarTypes;
            }
        }

        #endregion

        #region << 状态栏 >>

        private StatusBarItemType.StatusBarItemTypeCollection m_StatusBarItemTypes;
        /// <summary>
        /// 状态栏插件类型描述集合。
        /// </summary>
        public StatusBarItemType.StatusBarItemTypeCollection StatusBarItemTypes
        {
            get
            {
                if (this.m_StatusBarItemTypes == null)
                {
                    this.m_StatusBarItemTypes = new StatusBarItemType.StatusBarItemTypeCollection();
                }
                return this.m_StatusBarItemTypes;
            }
        }


        #endregion

        #region << 分类 >>

        /// <summary>
        /// 获取当前操作员可用的适合指定分类的所有插件类型。
        /// </summary>
        /// <param name="classifys">插件所必须适用的分类。</param>
        /// <returns></returns>
        public PluginTypeCollection GetClassifyPluginTypes(Classify[] classifys)
        {
            return base.GetClassifyPluginTypes(this.CurrentAccount, new Type[] { typeof(System.Windows.Forms.Control), typeof(System.Windows.Forms.ToolStripItem) }, classifys);
        }

        /// <summary>
        /// 获取指定的分类插件类型。
        /// </summary>
        /// <param name="classifyPluginType">实现 IClassifyPlugin 接口的分类插件类型声明，也可以是实现 IClassifyPlugin 接口及其它接口的接口类型声明(主要用于WEB和应用程序实现同一接口，功能跳转时指明该接口即可)。</param>
        /// <param name="classifys"></param>
        /// <returns></returns>
        public ClassifyPluginType GetClassifyPluginType(Type classifyPluginType, Classify[] classifys)
        {
            return base.GetClassifyPluginType(this.CurrentAccount, classifyPluginType, new Type[] { typeof(System.Windows.Forms.Control), typeof(System.Windows.Forms.ToolStripItem) }, classifys);
        }

        /// <summary>
        /// 获取指定分类的菜单
        /// </summary>
        /// <param name="classifys">分类。</param>
        /// <returns></returns>
        public System.Windows.Forms.ContextMenuStrip GetClassifyMenu(Classify[] classifys)
        {
            System.Windows.Forms.ContextMenuStrip mnuClassify = new System.Windows.Forms.ContextMenuStrip();
            mnuClassify.Tag = classifys;
            this.GetClassifyMenu(this.GetClassifyPluginTypes(classifys), mnuClassify.Items);
            return mnuClassify;
        }

        private bool GetClassifyMenu(PluginTypeCollection pluginTypes, ToolStripItemCollection menuItems)
        {
            bool bolIsOutMenu = false;

            for (int intIndex = 0; intIndex < pluginTypes.Count; intIndex++)
            {
                ClassifyPluginType pluginType = pluginTypes[intIndex] as ClassifyPluginType;

                if ((pluginType.Type.GetInterface(typeof(IClassifyHtmlPlugin).FullName, true) != null) || (Function.IsInheritableBaseType(pluginType.Type, typeof(System.Windows.Forms.Control))))
                {
                    //HTML、控件
                    ToolStripMenuItem mnuClassifyPlugin = new ToolStripMenuItem();
                    mnuClassifyPlugin.Text = pluginType.Name;
                    mnuClassifyPlugin.Image = pluginType.Icon16;
                    mnuClassifyPlugin.Tag = pluginType;
                    if (pluginType.Description != null && pluginType.Description.Length > 0)
                    {
                        mnuClassifyPlugin.ToolTipText = pluginType.Description;
                    }
                    if (pluginType.Icon16 != null)
                    {
                        mnuClassifyPlugin.Image = pluginType.Icon16;
                    }
                    mnuClassifyPlugin.Click += new EventHandler(mnuClassifyPlugin_Click);

                    if (pluginType.Children.Count > 0)
                    {
                        this.GetClassifyMenu(pluginType.Children, mnuClassifyPlugin.DropDownItems);
                    }
                    //mnuClassifyPlugin.MouseEnter += new EventHandler(mnuClassifyPlugin_MouseEnter);
                    menuItems.Add(mnuClassifyPlugin);
                    bolIsOutMenu = true;
                }
                else if (Function.IsInheritableBaseType(pluginType.Type, typeof(System.Windows.Forms.ToolStripItem)))
                {
                    //自定义的菜单项
                    System.Reflection.ConstructorInfo ci = pluginType.Type.GetConstructor(new System.Type[] { });
                    object objInstance = ci.Invoke(new object[] { });

                    IClassifyPlugin plugin = objInstance as IClassifyPlugin;
                    ToolStripItem mnuClassifyPlugin = plugin as ToolStripItem;
                    if (pluginType.Icon16 != null)
                    {
                        mnuClassifyPlugin.Image = pluginType.Icon16;
                    }
                    menuItems.Add(mnuClassifyPlugin);

                    plugin.SetClassifys((Classify[])mnuClassifyPlugin.Owner.Tag);
                    if (plugin is IUseAccount)
                    {
                        IUseAccount useAccount = plugin as IUseAccount;
                        useAccount.SetAccount(this.CurrentAccount);
                    }
                    plugin.SetApplication(this);

                    if (mnuClassifyPlugin.Text.Length == 0)
                    {
                        mnuClassifyPlugin.Text = pluginType.Name;
                    }
                    if (pluginType.Description != null && pluginType.Description.Length > 0 && (mnuClassifyPlugin.ToolTipText == null || mnuClassifyPlugin.ToolTipText.Length == 0))
                    {
                        mnuClassifyPlugin.ToolTipText = pluginType.Description;
                    }
                    //tsi.MouseEnter += new EventHandler(mnuClassifyPlugin_MouseEnter);
                    bolIsOutMenu = true;
                }
                else if (pluginType.Name == null)
                {
                    //分隔线
                    if (bolIsOutMenu && pluginType.Children.Count > 0)
                    {
                        menuItems.Add(new ToolStripSeparator());
                    }

                    //分隔线的子插件
                    if (this.GetClassifyMenu(pluginType.Children, menuItems))
                    {
                        bolIsOutMenu = true;
                    }
                }
                else
                {
                    //作为分类处理
                    if (pluginType.Children.Count > 0)
                    {
                        ToolStripMenuItem mnuClassifyPlugin = new ToolStripMenuItem();
                        mnuClassifyPlugin.Text = pluginType.Name;
                        if (pluginType.Description != null && pluginType.Description.Length > 0)
                        {
                            mnuClassifyPlugin.ToolTipText = pluginType.Description;
                        }
                        if (pluginType.Icon16 != null)
                        {
                            mnuClassifyPlugin.Image = pluginType.Icon16;
                        }
                        //mnuClassifyPlugin.MouseEnter += new EventHandler(mnuClassifyPlugin_MouseEnter);

                        this.GetClassifyMenu(pluginType.Children, mnuClassifyPlugin.DropDownItems);
                        menuItems.Add(mnuClassifyPlugin);
                        bolIsOutMenu = true;
                    }
                }
            }
            return bolIsOutMenu;
        }

        private void mnuClassifyPlugin_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem tsmi = sender as ToolStripMenuItem;
            Classify[] classifys = tsmi.Owner.Tag as Classify[];

            this.LoadView(tsmi.Tag as ClassifyPluginType, classifys);
        }

        #endregion

        #region << 设备 >>

        /// <summary>
        /// 获取设备在框架内缓存的实例。该方法默认返回 null。通常桌面应用程序在窗体各界面上显示同一设备时应保持这些设备引用的是同一个对象，以便在其中一个界面上对设备进行操作产生事件后可以在其它界面上即时反映出来，桌面应用程序框架应该重写该方法实现对象缓存策略。
        /// </summary>
        /// <param name="deviceId">设备编号</param>
        /// <returns>如果该编号设备存在于缓存内则返回设备实例，否则返回 null。</returns>
        protected override Device GetDeviceInstance(int deviceId)
        {
            if (base.m_slDevices.ContainsKey(deviceId))
            {
                return base.m_slDevices[deviceId];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 设置设备实例在框架内缓存。该方法默认不执行任何操作，如果当前应用需要对设备对象进行缓存则应重写该方法实现对象缓存策略。
        /// </summary>
        /// <param name="device"></param>
        protected override void SetDeviceInstance(Device device)
        {
            if (device.DeviceId > 0)
            {
                if (base.m_slDevices.ContainsKey(device.DeviceId) == false)
                {
                    base.m_slDevices.Add(device.DeviceId, device);
                }
            }
        }

        /// <summary>
        /// 获取当前操作员可用的适合指定设备的所有插件类型。
        /// </summary>
        /// <param name="devices">插件所必须适用的设备。</param>
        /// <returns></returns>
        public PluginTypeCollection GetDevicePluginTypes(Device[] devices)
        {
            return base.GetDevicePluginTypes(this.CurrentAccount, new Type[] { typeof(System.Windows.Forms.Control), typeof(System.Windows.Forms.ToolStripItem) }, devices);
        }

        /// <summary>
        /// 获取指定的设备插件类型。
        /// </summary>
        /// <param name="devicePluginType">实现 IDevicePlugin 接口的设备插件类型声明，也可以是实现 IDevicePlugin 接口及其它接口的接口类型声明(主要用于WEB和应用程序实现同一接口，功能跳转时指明该接口即可)。</param>
        /// <param name="devices"></param>
        /// <returns></returns>
        public DevicePluginType GetDevicePluginType(Type devicePluginType, Device[] devices)
        {
            return base.GetDevicePluginType(this.CurrentAccount, devicePluginType, new Type[] { typeof(System.Windows.Forms.Control), typeof(System.Windows.Forms.ToolStripItem) }, devices);
        }


        /// <summary>
        /// 获取指定设备的菜单
        /// </summary>
        /// <param name="devices">设备。</param>
        /// <returns></returns>
        public System.Windows.Forms.ContextMenuStrip GetDeviceMenu(Device[] devices)
        {
            System.Windows.Forms.ContextMenuStrip mnuDevice = new System.Windows.Forms.ContextMenuStrip();
            mnuDevice.Tag = devices;
            this.GetDeviceMenu(this.GetDevicePluginTypes(devices), mnuDevice.Items);
            return mnuDevice;
        }

        //如果返回 true 则表示该次调用有菜单项被输出
        private bool GetDeviceMenu(PluginTypeCollection pluginTypes, ToolStripItemCollection menuItems)
        {
            bool bolIsOutput = false;

            for (int intIndex = 0; intIndex < pluginTypes.Count; intIndex++)
            {
                DevicePluginType _PluginType = pluginTypes[intIndex] as DevicePluginType;

                if ((_PluginType.Type.GetInterface(typeof(IDeviceHtmlPlugin).FullName, true) != null) || (Function.IsInheritableBaseType(_PluginType.Type, typeof(System.Windows.Forms.Control))))
                {
                    //HTML、控件
                    ToolStripMenuItem mnuDevicePlugin = new ToolStripMenuItem();
                    mnuDevicePlugin.Text = _PluginType.Name;
                    mnuDevicePlugin.Image = _PluginType.Icon16;
                    mnuDevicePlugin.Tag = _PluginType;
                    if (_PluginType.Description != null && _PluginType.Description.Length > 0)
                    {
                        mnuDevicePlugin.ToolTipText = _PluginType.Description;
                    }
                    if (_PluginType.Icon16 != null)
                    {
                        mnuDevicePlugin.Image = _PluginType.Icon16;
                    }
                    mnuDevicePlugin.Click += new EventHandler(mnuDevicePlugin_Click);

                    if (_PluginType.Children.Count > 0)
                    {
                        this.GetDeviceMenu(_PluginType.Children, mnuDevicePlugin.DropDownItems);
                    }
                    //mnuDevicePlugin.MouseEnter += new EventHandler(mnuDevicePlugin_MouseEnter);
                    menuItems.Add(mnuDevicePlugin);
                    bolIsOutput = true;
                }
                else if (Function.IsInheritableBaseType(_PluginType.Type, typeof(System.Windows.Forms.ToolStripItem)))
                {
                    //自定义的菜单项
                    System.Reflection.ConstructorInfo ci = _PluginType.Type.GetConstructor(new System.Type[] { });
                    object objInstance = ci.Invoke(new object[] { });

                    IDevicePlugin plugin = objInstance as IDevicePlugin;
                    ToolStripItem mnuDevicePlugin = plugin as ToolStripItem;
                    if (_PluginType.Icon16 != null)
                    {
                        mnuDevicePlugin.Image = _PluginType.Icon16;
                    }
                    menuItems.Add(mnuDevicePlugin);

                    plugin.SetDevices((Device[])mnuDevicePlugin.Owner.Tag);
                    if (plugin is IUseAccount)
                    {
                        IUseAccount useAccount = plugin as IUseAccount;
                        useAccount.SetAccount(this.CurrentAccount);
                    }
                    plugin.SetApplication(this);

                    if (mnuDevicePlugin.Text.Length == 0)
                    {
                        mnuDevicePlugin.Text = _PluginType.Name;
                    }
                    if (_PluginType.Description != null && _PluginType.Description.Length > 0 && (mnuDevicePlugin.ToolTipText == null || mnuDevicePlugin.ToolTipText.Length == 0))
                    {
                        mnuDevicePlugin.ToolTipText = _PluginType.Description;
                    }
                    //tsi.MouseEnter += new EventHandler(mnuDevicePlugin_MouseEnter);
                    bolIsOutput = true;
                }
                else if (_PluginType.Name == null)
                {
                    //分隔线
                    if (bolIsOutput && _PluginType.Children.Count > 0)
                    {
                        menuItems.Add(new ToolStripSeparator());
                    }

                    //分隔线的子插件
                    if (this.GetDeviceMenu(_PluginType.Children, menuItems))
                    {
                        bolIsOutput = true;
                    }
                }
                else
                {
                    //作为分类处理
                    if (_PluginType.Children.Count > 0)
                    {
                        ToolStripMenuItem mnuDevicePlugin = new ToolStripMenuItem();
                        mnuDevicePlugin.Text = _PluginType.Name;
                        if (_PluginType.Description != null && _PluginType.Description.Length > 0)
                        {
                            mnuDevicePlugin.ToolTipText = _PluginType.Description;
                        }
                        if (_PluginType.Icon16 != null)
                        {
                            mnuDevicePlugin.Image = _PluginType.Icon16;
                        }
                        //mnuDevicePlugin.MouseEnter += new EventHandler(mnuDevicePlugin_MouseEnter);

                        this.GetDeviceMenu(_PluginType.Children, mnuDevicePlugin.DropDownItems);
                        menuItems.Add(mnuDevicePlugin);
                        bolIsOutput = true;
                    }
                }
            }
            return bolIsOutput;
        }

        private void mnuDevicePlugin_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem tsmi = sender as ToolStripMenuItem;
            Device[] devices = tsmi.Owner.Tag as Device[];

            this.LoadView(tsmi.Tag as DevicePluginType, devices);
        }

        /// <summary>
        /// 获取指定设备可用的档案更新项。
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        public DeviceArchiveItemTypeCollection GetDeviceArchiveUpdateItemTypes(Device device)
        {
            return base.GetDeviceArchiveUpdateItemTypes(typeof(System.Windows.Forms.Control), device);
        }

        /// <summary>
        /// 获取指定设备可用的档案删除项。
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        public DeviceArchiveItemTypeCollection GetDeviceArchiveDeleteItemTypes(Device device)
        {
            return base.GetDeviceArchiveDeleteItemTypes(typeof(System.Windows.Forms.Control), device);
        }

        #endregion
    }
}
