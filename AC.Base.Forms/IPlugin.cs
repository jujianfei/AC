using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Forms
{
    /// <summary>
    /// WEB应用程序和桌面应用程序界面功能插件接口。如果需要账号信息可以添加 IUseAccount 接口。
    /// </summary>
    public interface IPlugin
    {
        /// <summary>
        /// 设置应用程序框架。
        /// </summary>
        /// <param name="application">应用程序框架。</param>
        void SetApplication(FormApplicationClass application);
    }
}
