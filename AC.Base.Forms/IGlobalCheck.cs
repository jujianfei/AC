using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Forms
{
    /// <summary>
    /// 全局插件检查接口。
    /// 实现IGlobalPlugin接口并添加GlobalPluginTypeAttribute特性的插件可以指定实现该接口的ForCheckType属性，当在设备上显示菜单前会初始化ForCheckType指向的类并调用Check方法。
    /// </summary>
    public interface IGlobalCheck
    {
        /// <summary>
        /// 检查全局插件能否使用。
        /// </summary>
        /// <param name="application"></param>
        /// <returns></returns>
        bool Check(FormApplicationClass application);
    }
}
