using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AC.Base.Forms;

namespace AC.Base.Forms.Plugins
{
    /// <summary>
    /// 全局“管理”菜单
    /// </summary>
    [GlobalPluginType("管理", "", null, 1200, null, null)]
    public class AdminGolbalSortplugin : IGlobalPlugin
    {
        #region IPlugin 成员
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
