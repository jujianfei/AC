using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AC.Base.Forms;

namespace AC.Base.Forms.Windows
{
    /// <summary>
    /// 全局插件视图。
    /// </summary>
    public class GlobalPluginView : PluginViewBase
    {
        string _id;
        /// <summary>
        /// 全局插件视图。
        /// </summary>
        /// <param name="application">应用程序框架。</param>
        /// <param name="pluginType">插件类型。</param>
        public GlobalPluginView(WindowsFormApplicationClass application, PluginType pluginType)
            : base(application, pluginType)
        {
        }

        /// <summary>
        /// 重写带参数的全局视图，主用用来做增 删 改 操作
        /// </summary>
        /// <param name="application">应用程序框架</param>
        /// <param name="pluginType">插件类型</param>
        /// <param name="id">具体操作的ID,这个参数可以变位hashtable 或者list</param>
        public GlobalPluginView(WindowsFormApplicationClass application, PluginType pluginType,string id)
            : base(application, pluginType)
        {
            this._id = id;
        }

        /// <summary>
        /// 设置插件参数。PluginViewBase将插件初始化后调用该方法，由继承的类对插件进行一些设置后再进行后续的加载步骤。
        /// </summary>
        /// <param name="plugin">需要被设置参数的插件。</param>
        protected override void SetPlugin(IPlugin plugin)
        {
            //全局插件不用设置任何参数。
            //edited by xc  如果需要带参数的全局变量。
            if (_id != null)
            {
                IOperationPlugin devicePlugin = plugin as IOperationPlugin;
                devicePlugin.SetParameter(_id);
            }
        }
    }
}
