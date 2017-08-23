using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AC.Base.Forms;

namespace AC.Base.Forms.Windows
{
    /// <summary>
    /// 主要用于HTML插件增删改类型的操作接口
    /// </summary>
    public class OperationPluginView : PluginViewBase
    {
       /// <summary>
        /// 基础操作插件视图
       /// </summary>
       /// <param name="application">应用程序框架</param>
       /// <param name="pluginType">插件类型</param>
       /// <param name="ID">操作key</param>
        public OperationPluginView(WindowsFormApplicationClass application, PluginType pluginType, int ID)
            : base(application, pluginType)
        {
            this.ID = ID;
        }

        /// <summary>
        /// 传递的keyID信息。
        /// </summary>
        public Int32 ID { get; private set; }


        /// <summary>
        /// 获取显示在视图标签页上的文字。
        /// </summary>
        /// <returns></returns>
        public override string GetViewTitle()
        {
            return this.PluginType.Name;
        }

        /// <summary>
        /// 获取当前对象的字符串表示形式。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.PluginType.Name;
        }


        /// <summary>
        /// 设置插件参数。PluginViewBase将插件初始化后调用该方法，由继承的类对插件进行一些设置后再进行后续的加载步骤。
        /// </summary>
        /// <param name="plugin">需要被设置参数的插件。</param>
        protected override void SetPlugin(IPlugin plugin)
        {
            //全局插件不用设置任何参数。
        }
    }
}
