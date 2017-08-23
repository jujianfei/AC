using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Forms.Plugins
{
    /// <summary>
    /// 全局文件菜单
    /// </summary>
    [GlobalPluginType("文件", "", null, 200, null,null)]
    public class FileGlobalSortPlugin : IGlobalPlugin
    {
        #region << IPlugin 成员 >>

        /// <summary>
        /// 设置应用程序框架。
        /// </summary>
        /// <param name="application">应用程序框架。</param>
        public void SetApplication(FormApplicationClass application)
        {
        }

        #endregion
    }
}
