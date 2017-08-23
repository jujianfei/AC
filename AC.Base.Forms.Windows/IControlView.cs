using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Forms.Windows
{
    /// <summary>
    /// 添加到 ControlView 控件视图中的控件必须实现的接口，同时实现该接口的类必须提供无参数的构造函数。
    /// 当应用程序关闭时，框架会调用 GetViewConfig() 方法获取该控件视图内控件的配置参数。
    /// 下次启动应用程序，框架将加载上次窗口布局，并初始化各控件视图内的控件，如果实现了 IUseAccount 接口则首先调用 SetAccount 方法将账号传入，
    /// 然后调用 SetViewConfig 方法将配置参数传入，最后调用 SetApplication 方法将应用程序框架传入。
    /// </summary>
    public interface IControlView
    {
        /// <summary>
        /// 设置应用程序框架。
        /// </summary>
        /// <param name="application"></param>
        void SetApplication(WindowsFormApplicationClass application);

        /// <summary>
        /// 设置控件视图的配置参数。
        /// </summary>
        /// <param name="config"></param>
        void SetViewConfig(System.Xml.XmlNode config);

        /// <summary>
        /// 返回当前控件视图的配置参数，以便下次打开该视图时可以通过 SetViewConfig 复原当前视图。
        /// </summary>
        /// <param name="xmlDoc"></param>
        /// <returns>如果当前控件视图无任何配置参数，则可以返回 null。</returns>
        System.Xml.XmlNode GetViewConfig(System.Xml.XmlDocument xmlDoc);
    }
}
