using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AC.Base.Forms;
using System.Windows.Forms;
using AC.Base.DeviceSearchs;
using System.Runtime.InteropServices;

namespace AC.Base.Forms.Windows
{
    /// <summary>
    /// 在应用程序中心区域或四周显示的HTML插件、控件插件视图界面。
    /// </summary>
    public abstract class PluginViewBase : ViewBase
    {
        //首次加载：查询参数、翻页参数、界面控件
        //重新打开本页：不初始化参数，重新初始化控件
        //跳转至本页：如果更新参数则“改变查询参数”
        //改变查询参数：控件插件调用参数方法；HTML重新加载
        //翻页：控件插件调用翻页参数方法；HTML重新加载

        private bool m_IsHtmlPlugin;                                    //当前插件是否是HTML插件，如为false则是控件插件
        private List<IParameterControl> m_lsParameterControls = null;   //参数
        private Pagination m_Pagination;                                //分页
        private Control m_PluginControl;                                //实例化的插件控件
        private IPlugin m_Plugin;                                       //实例化的插件。如果当前是HTML插件，则此对象是插件的实例，m_PluginControl是浏览器实例。
        private bool m_IsCreateViewControl;                             //是否已创建控件
        private ParameterValue[] m_ParameterValues;                     //各参数指定值

        /// <summary>
        /// 在应用程序中心区域或四周显示的HTML插件、控件插件视图界面。
        /// </summary>
        /// <param name="application">应用程序框架。</param>
        /// <param name="pluginType">插件类型。</param>
        public PluginViewBase(WindowsFormApplicationClass application, PluginType pluginType) : base(application)
        {
            this.PluginType = pluginType;
           
            if (Function.IsInheritableBaseType(pluginType.Type, typeof(System.Windows.Forms.UserControl)))
            {
                this.m_IsHtmlPlugin = false;
            }

            if (pluginType.Type.GetInterface(typeof(IHtmlPlugin).FullName, true) != null)
            {
                this.m_IsHtmlPlugin = true;
            }
            else if (Function.IsInheritableBaseType(pluginType.Type, typeof(System.Windows.Forms.Control)))
            {
                this.m_IsHtmlPlugin = false;
            }
            else
            {
                throw new Exception(pluginType.Type.FullName + " 不能作为视图插件加载。");
            }
        }

        /// <summary>
        /// 插件类型。
        /// </summary>
        public PluginType PluginType { get; private set; }

        /// <summary>
        /// 设置插件参数。PluginViewBase将插件初始化后调用该方法，由继承的类对插件进行一些设置后再进行后续的加载步骤。
        /// </summary>
        /// <param name="plugin">需要被设置参数的插件。</param>
        protected abstract void SetPlugin(IPlugin plugin);

        /// <summary>
        /// 获取该视图所呈现的包含参数设置、插件控件、翻页控件的界面。
        /// </summary>
        /// <returns></returns>
        public override Control CreateViewControl()
        {
            Control ctlView = new Control();
            FlowLayoutPanel flpParameters = null;

            //初始化插件
            System.Reflection.ConstructorInfo ci = this.PluginType.Type.GetConstructor(new System.Type[] { });
            object objInstance = ci.Invoke(new object[] { });
            this.m_Plugin = objInstance as IPlugin;

            if (this.m_IsHtmlPlugin)
            {
                this.m_PluginControl = new System.Windows.Forms.WebBrowser();
                ((System.Windows.Forms.WebBrowser)this.m_PluginControl).ObjectForScripting = new ScriptManager(this);
                ((System.Windows.Forms.WebBrowser)this.m_PluginControl).NewWindow += new System.ComponentModel.CancelEventHandler(PluginViewBase_NewWindow);
            }
            else
            {
                this.m_PluginControl = objInstance as Control;
            }

            //设置应用程序框架
            this.m_Plugin.SetApplication(base.Application);

            //设置账号
            if (this.m_Plugin is IUseAccount)
            {
                if (this.Application.CurrentAccount == null)
                {
                    throw new Exception("“" + this.PluginType.Name + "”需要使用操作员账号，但应用程序框架无账号信息。");
                }

                IUseAccount useAccount = this.m_Plugin as IUseAccount;
                useAccount.SetAccount(this.Application.CurrentAccount);
            }

            //由继承的类设置对象
            this.SetPlugin(this.m_Plugin);

            //设置参数
            foreach (Type typParameter in this.PluginType.ParameterTypes)
            {
                if (flpParameters == null)
                {
                    flpParameters = new FlowLayoutPanel();
                }
                if (this.m_lsParameterControls == null)
                {
                    this.m_lsParameterControls = new List<IParameterControl>();
                }

                Type typControlType = this.Application.GetParameterControlType(typParameter);
                if (typControlType != null)
                {
                    System.Reflection.ConstructorInfo ciParameter = typControlType.GetConstructor(new System.Type[] { });
                    object objParameterInstance = ciParameter.Invoke(new object[] { });

                    IParameterControl parameterControl = objParameterInstance as IParameterControl;
                    parameterControl.SetApplication(base.Application);

                    if (objParameterInstance is IUseAccount)
                    {
                        IUseAccount useAccount = objParameterInstance as IUseAccount;
                        useAccount.SetAccount(this.Application.CurrentAccount);
                    }

                    parameterControl.SetOrientation(true);

                    if (this.m_ParameterValues != null)
                    {
                        foreach (ParameterValue _ParameterValue in this.m_ParameterValues)
                        {
                            if (_ParameterValue.Type.Equals(typParameter))
                            {
                                parameterControl.SetParameterValue(_ParameterValue.Value);
                                break;
                            }
                        }
                    }
                    m_lsParameterControls.Add(parameterControl);
                    parameterControl.SetParameterPlugin(this.m_Plugin as IParameter);
                    flpParameters.Controls.Add(objParameterInstance as Control);
                }
                else
                {
                    Label labError = new Label();
                    labError.Text = "未发现“" + typParameter.FullName + "”的查询参数界面控件。";
                    labError.ForeColor = System.Drawing.Color.Red;
                    labError.AutoSize = true;
                    flpParameters.Controls.Add(labError);
                }
            }
            //edit by xch 3.23
            if (Function.IsInheritableBaseType(this.PluginType.Type, typeof(System.Windows.Forms.UserControl)))
                ctlView.Size = new System.Drawing.Size((this.m_PluginControl as UserControl).Width + 10, (this.m_PluginControl as UserControl).Height + 40);
            else
                ctlView.Size = new System.Drawing.Size(800, 600);

            this.m_PluginControl.Dock = DockStyle.Fill;
            ctlView.Controls.Add(this.m_PluginControl);

            if (flpParameters != null)
            {
                Button btnParameter = new Button();
                btnParameter.Text = "确定";
                btnParameter.Click += new EventHandler(btnParameter_Click);
                flpParameters.Controls.Add(btnParameter);

                flpParameters.Dock = DockStyle.Top;
                flpParameters.AutoSize = true;
                ctlView.Controls.Add(flpParameters);
            }

            if (this.PluginType.IsPagination)
            {
                IPagination pluginPagination = this.m_Plugin as IPagination;
                pluginPagination.Pagination += new PaginationEventHandler(plugin_Pagination);
                this.m_Pagination = new Pagination();
                this.m_Pagination.Dock = DockStyle.Bottom;
                this.m_Pagination.Paging += new PagingEventHandler(m_Pagination_Paging);
                ctlView.Controls.Add(this.m_Pagination);
                pluginPagination.SetPageNumber(1);
            }

            if (this.m_IsHtmlPlugin)
                this.WriterHtmlPlugin(this.m_Plugin);

            


            this.m_IsCreateViewControl = true;
            return ctlView;
        }

        private static int HtmlFileNum = 0;

        private void WriterHtmlPlugin(IPlugin plugin)
        {
            IHtmlPlugin htmlPlugin = plugin as IHtmlPlugin;
            HtmlFileNum++;
            string strFileName = this.Application.TemporaryDirectory + HtmlFileNum + ".htm";
            System.IO.StreamWriter sw = new System.IO.StreamWriter(strFileName, false, System.Text.Encoding.UTF8);
            sw.WriteLine("<html>");
            sw.WriteLine("<head>");
            sw.WriteLine("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=UTF-8\" />");
            sw.WriteLine("<style type=\"text/css\">");
            sw.WriteLine("<!--");
            sw.WriteLine("body{font-family:宋体;font-size:9pt}");
            sw.WriteLine("input{font-family:宋体;font-size:9pt}");
            sw.WriteLine("select{font-family:宋体;font-size:9pt}");
            sw.WriteLine("textarea{font-family:宋体;font-size:9pt}");
            sw.WriteLine("td{font-family:宋体;font-size:9pt}");
            sw.WriteLine("A{text-decoration:none;}");
            sw.WriteLine("A:hover{color:#FF0000;text-decoration:underline;}");
            sw.WriteLine(".mytablecss{width: 100%;border:1px solid #efefef;border-collapse:collapse; }");
            sw.WriteLine(".mytablecss td{padding:3;}");
            sw.WriteLine(".mytablecss th{padding:3;font-size:9pt}");
            sw.WriteLine(".mytablecss tr{background-color: expression(this.sectionRowIndex == 0 ? \"#E4E4E4\" : ((this.sectionRowIndex % 2 == 0) ? \"#FFFFFF\" : \"#F6F7F8\"));color: expression(this.sectionRowIndex == 0 ? \"#000000\" : \"\");font-weight: expression(this.sectionRowIndex == 0 ? \"BOLD\" :\"\");tableselect: expression(this.sectionRowIndex == 0 ? \"\" : (onmouseover = function(){this.style.backgroundColor = (this.style.backgroundColor != \"#ffffdd\" ? \"#ffffdd\" : (this.sectionRowIndex == 0 ? \"#FFCCCC\" : (this.sectionRowIndex % 2 == 0 ? \"#FFFFFF\" : \"#f6f7f8\" )))},onmouseout = function(){this.style.backgroundColor = (this.style.backgroundColor != \"#ffffdd\" ? \"#ffffdd\" : (this.sectionRowIndex == 0 ? \"#FFCCCC\" : (this.sectionRowIndex % 2 == 0 ? \"#FFFFFF\" : \"#f6f7f8\")))}))}");
            //sw.WriteLine(".mytablecss td{background-color:expression(this.cellIndex == 0 ? (this.parentElement.sectionRowIndex == 0 ?\"#FFCCCC\" : \"#DDDDDD\") : null)}"); //第一列的颜色不做改变
            sw.WriteLine("");
            sw.WriteLine("-->");

            sw.WriteLine("</style>");
            sw.WriteLine("<title>" + this.GetViewTitle() + "</title>");
            sw.WriteLine("<script language='javascript'>");
            sw.WriteLine("// 转到指定的全局视图。");
            sw.WriteLine("// pluginTypeName   : 实现 IGlobalPlugin 接口的类型声明的全名称。");
            sw.WriteLine("// arguments        : 实现 IParameter 接口的条件参数默认值，该参数为可选参数且必须是“类型声明全名称”与“参数值”成对传入。");
            sw.WriteLine("function LoadGlobalView(pluginTypeName) {");
            sw.WriteLine("  var parameters = \"\";");
            sw.WriteLine("  for (intIndex = 1; intIndex < arguments.length; intIndex++) {");
            sw.WriteLine("      parameters += arguments[intIndex] + \"\\t\";");
            sw.WriteLine("  }");
            sw.WriteLine("  window.external.LoadGlobalView(pluginTypeName, parameters);");
            sw.WriteLine("}");
            sw.WriteLine("");

            sw.WriteLine("// 转到指定的设备视图。");
            sw.WriteLine("// pluginTypeName   : 实现 IDevicePlugin 接口的类型声明的全名称。");
            sw.WriteLine("// deviceIds        : 设备编号，多个设备编号用“,”隔开。");
            sw.WriteLine("// arguments        : 实现 IParameter 接口的条件参数默认值，该参数为可选参数且必须是“类型声明全名称”与“参数值”成对传入。");
            sw.WriteLine("function LoadDeviceView(pluginTypeName, deviceIds) {");
            sw.WriteLine("  var parameters = \"\";");
            sw.WriteLine("  for (intIndex = 2; intIndex < arguments.length; intIndex++) {");
            sw.WriteLine("      parameters += arguments[intIndex] + \"\\t\";");
            sw.WriteLine("  }");
            sw.WriteLine("  window.external.LoadDeviceView(pluginTypeName, deviceIds, parameters);");
            sw.WriteLine("}");
            sw.WriteLine("");
            sw.WriteLine("// 异步获取实现 IHtmlContext 接口的类中 GetContext 方法的内容。");
            sw.WriteLine("// htmlContextTypeName  : 实现 IHtmlContext 接口的类型声明的全名称。");
            sw.WriteLine("// parameterValue       : GetContext 方法中的参数值，该值内容应符合该对象的要求。");
            sw.WriteLine("// callbackName         : 此页面上的回调方法名。例如 function PrintValue(succeed, value) { } ，当异步获取到内容后将调用 PrintValue 函数。succeed == true：value 是正确返回的内容；succeed == false：value 是出错内容。");
            sw.WriteLine("function GetHtmlContext(htmlContextTypeName, parameterValue, callbackName) {");
            sw.WriteLine("  window.external.GetHtmlContext(htmlContextTypeName, parameterValue, callbackName);");
            sw.WriteLine("}");
            sw.WriteLine("</script>");
            sw.WriteLine("</head>");
            sw.WriteLine("<body>");
            sw.WriteLine("");
            sw.WriteLine("<font color='#15428b'>" + this.ToString() + "</font><hr size='1'>");
            sw.WriteLine("");

            htmlPlugin.WriterHtml(sw);

            sw.WriteLine("");
            sw.WriteLine("</body>");
            sw.WriteLine("</html>");

            sw.Flush();
            sw.Close();

            ((WebBrowser)this.m_PluginControl).Navigate(strFileName);
        }

        private void PluginViewBase_NewWindow(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
        }

        internal void SetHtmlContext(string callback, bool succeed, string context)
        {
            ((WebBrowser)this.m_PluginControl).Document.InvokeScript(callback, new object[] { succeed, context });
        }


        internal void LoadGlobalView(string pluginTypeName, string parameters)
        {
            MessageBox.Show("12");
            Type typPluginType = Function.GetType(pluginTypeName);
            GlobalPluginType _PluginType = base.Application.GetGlobalPluginType(typPluginType);

            if (parameters != null && parameters.Length > 1)
            {
                List<ParameterValue> lstParameterValues = new List<ParameterValue>();
                string[] strParameters = parameters.Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);

                for (int intIndex = 0; intIndex < strParameters.Length; intIndex += 2)
                {
                    if (strParameters.Length >= intIndex + 2)
                    {
                        ParameterValue _ParameterValue = new ParameterValue();
                        _ParameterValue.Type = Function.GetType(strParameters[intIndex]);
                        if (_ParameterValue.Type != null)
                        {
                            _ParameterValue.Value = strParameters[intIndex + 1];
                            lstParameterValues.Add(_ParameterValue);
                        }
                    }
                }
                this.Application.LoadView(_PluginType, lstParameterValues.ToArray());
            }
            else
            {
                this.Application.LoadView(_PluginType);
            }
        }

        internal void LoadDeviceView(string pluginTypeName, string deviceIds, string parameters)
        {
            Type typPluginType = Function.GetType(pluginTypeName);

            if (deviceIds != null && deviceIds.Length > 0)
            {
                DeviceSearch _Search = new DeviceSearch(base.Application);
                _Search.Filters.Add(new AC.Base.DeviceSearchs.IdFilter(deviceIds));
                Device[] devices = _Search.Search(0).ToArray();

                DevicePluginType _PluginType = base.Application.GetDevicePluginType(typPluginType, devices);

                if (parameters != null && parameters.Length > 1)
                {
                    List<ParameterValue> lstParameterValues = new List<ParameterValue>();
                    string[] strParameters = parameters.Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);

                    for (int intIndex = 0; intIndex < strParameters.Length; intIndex += 2)
                    {
                        if (strParameters.Length >= intIndex + 2)
                        {
                            ParameterValue _ParameterValue = new ParameterValue();
                            _ParameterValue.Type = Function.GetType(strParameters[intIndex]);
                            if (_ParameterValue.Type != null)
                            {
                                _ParameterValue.Value = strParameters[intIndex + 1];
                                lstParameterValues.Add(_ParameterValue);
                            }
                        }
                    }
                    this.Application.LoadView(_PluginType, devices, lstParameterValues.ToArray());
                }
                else
                {
                    this.Application.LoadView(_PluginType, devices);
                }
            }
        }

        /// <summary>
        /// 更改当前插件视图中各参数的默认值。
        /// </summary>
        /// <param name="parameterValues"></param>
        public void SetParameterValue(ParameterValue[] parameterValues)
        {
            if (parameterValues != null && parameterValues.Length > 0)
            {
                if (m_IsCreateViewControl)
                {
                    foreach (ParameterValue _ParameterValue in parameterValues)
                    {
                        foreach (IParameterControl parameterControl in this.m_lsParameterControls)
                        {
                            ControlAttribute attrControl = parameterControl.GetType().GetCustomAttributes(typeof(ControlAttribute), false)[0] as ControlAttribute;
                            if (attrControl.ForType.Equals(_ParameterValue.Type))
                            {
                                parameterControl.SetParameterValue(_ParameterValue.Value);
                                parameterControl.SetParameterPlugin((IParameter)this.m_Plugin);
                                break;
                            }
                        }
                    }

                    this.m_Plugin.SetApplication(base.Application);

                    if (this.m_IsHtmlPlugin)
                    {
                        this.WriterHtmlPlugin(this.m_Plugin);
                    }
                }
                else
                {
                    this.m_ParameterValues = parameterValues;
                }
            }
        }

        //更改参数
        private void btnParameter_Click(object sender, EventArgs e)
        {
            foreach (IParameterControl parameterControl in this.m_lsParameterControls)
            {
                parameterControl.SetParameterPlugin((IParameter)this.m_Plugin);
            }

            this.m_Plugin.SetApplication(base.Application);

            if (this.m_IsHtmlPlugin)
            {
                this.WriterHtmlPlugin(this.m_Plugin);
            }
        }

        //翻页
        private void m_Pagination_Paging(int pageNumber)
        {
            IPagination pluginPagination = this.m_Plugin as IPagination;
            pluginPagination.SetPageNumber(pageNumber);

            this.m_Plugin.SetApplication(base.Application);

            if (this.m_IsHtmlPlugin)
            {
                this.WriterHtmlPlugin(this.m_Plugin);
            }
        }

        //分页数据
        private void plugin_Pagination(int recordsetCount, int pageSize)
        {
            this.m_Pagination.SetPagination(recordsetCount, pageSize);
        }

        /// <summary>
        /// 获取显示在视图标签页上的文字。
        /// </summary>
        /// <returns></returns>
        public override string GetViewTitle()
        {
            return this.PluginType.Name;
        }

        /// <summary>
        /// 获取当前视图内的控件。如果插件继承 System.Windows.Forms.Control 则返回该插件的实例，如果是 HTML 插件则返回 WebBrowser。
        /// </summary>
        /// <returns></returns>
        public override Control GetControl()
        {
            return this.m_PluginControl;
        }

        /// <summary>
        /// 获取当前视图16*16像素的图标。
        /// </summary>
        /// <returns></returns>
        public override System.Drawing.Image GetIcon16()
        {
            return this.PluginType.Icon16;
        }

        /// <summary>
        /// 确定指定的对象是否等于当前的对象。
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is PluginType)
            {
                PluginType pluginType = obj as PluginType;
                if (pluginType.Equals(this.PluginType))
                {
                    return true;
                }
                return false;
            }
            else if (obj is IPlugin)
            {
                return this.m_Plugin.Equals(obj);
            }
            else
            {
                return base.Equals(obj);
            }
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
        /// 释放该视图使用的所有资源。
        /// </summary>
        public override void Dispose()
        {
            this.m_PluginControl.Dispose();
        }

        /// <summary>
        /// HTML 视图中通过 JavaScript 调用 .Net 里的方法。
        /// </summary>
        [ComVisible(true)]
        public class ScriptManager
        {
            private PluginViewBase PluginView;

            internal ScriptManager(PluginViewBase pluginView)
            {
                this.PluginView = pluginView;
            }

            /// <summary>
            /// 载入全局视图。
            /// </summary>
            /// <param name="pluginTypeName"></param>
            /// <param name="parameters"></param>
            public void LoadGlobalView(string pluginTypeName, string parameters)
            {
                this.PluginView.LoadGlobalView(pluginTypeName, parameters);
            }

            /// <summary>
            /// 载入设备视图。
            /// </summary>
            /// <param name="pluginTypeName"></param>
            /// <param name="deviceIds"></param>
            /// <param name="parameters"></param>
            public void LoadDeviceView(string pluginTypeName, string deviceIds, string parameters)
            {
                this.PluginView.LoadDeviceView(pluginTypeName, deviceIds, parameters);
            }

            /// <summary>
            /// 获取实现 IHtmlContext 接口的 GetContext 方法的内容。 
            /// </summary>
            /// <param name="typeName">类型名称</param>
            /// <param name="parameter">参数</param>
            /// <param name="callback">HTML 页面中回调方法的名称</param>
            public void GetHtmlContext(string typeName, string parameter, string callback)
            {
                Type typ = Function.GetType(typeName);
                if (typ != null)
                {
                    try
                    {
                        System.Reflection.ConstructorInfo ciObject = typ.GetConstructor(new System.Type[] { });
                        object obj = ciObject.Invoke(new object[] { });

                        IHtmlContext _IHtmlContext = obj as IHtmlContext;
                        if (_IHtmlContext is IUseAccount)
                        {
                            IUseAccount _IUseAccount = _IHtmlContext as IUseAccount;
                            _IUseAccount.SetAccount(this.PluginView.Application.CurrentAccount);
                        }
                        this.PluginView.SetHtmlContext(callback, true, _IHtmlContext.GetContext(this.PluginView.Application, parameter));
                    }
                    catch (Exception ex)
                    {
                        this.PluginView.SetHtmlContext(callback, false, ex.Message);
                    }
                }
                else
                {
                    MessageBox.Show("无法加载名称为“" + typeName + "”的类型", "获取内容", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
