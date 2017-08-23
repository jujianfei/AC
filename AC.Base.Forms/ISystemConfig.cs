using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Forms
{
    /// <summary>
    /// 提供给我方实施人员使用的系统配置插件接口。
    /// 实现该接口的类必须继承自 System.Windows.Forms.Control ，并且添加 SystemConfigAttribute 特性。
    /// </summary>
    public interface ISystemConfig
    {
        /// <summary>
        /// 设置应用程序框架。
        /// </summary>
        /// <param name="application">应用程序框架。</param>
        void SetApplication(FormApplicationClass application);
    }
}
