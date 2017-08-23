using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Forms
{
    /// <summary>
    /// 全局插件。
    /// </summary>
    public class GlobalPluginType : PluginType
    {
        internal GlobalPluginType(ApplicationClass application, Type type)
            : base(application, type)
        {
        }

        /// <summary>
        /// 获取继承自 PluginTypeAttribute 的特性。
        /// </summary>
        protected override PluginTypeAttribute PluginTypeAttr
        {
            get { return (PluginTypeAttribute)this.Type.GetCustomAttributes(typeof(PluginTypeAttribute), false)[0]; }
        }
    }
}
