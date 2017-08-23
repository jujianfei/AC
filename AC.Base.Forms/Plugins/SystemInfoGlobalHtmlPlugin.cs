using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AC.Base.Forms;

namespace AC.Base.Forms.Plugins
{
    /// <summary>
    /// 系统信息HTML全局插件
    /// </summary>
    [GlobalPluginType("系统信息", "", typeof(AdminGolbalSortplugin), 99, null, null)]
    public class SystemInfoGlobalHtmlPlugin : IGlobalHtmlPlugin
    {
        #region IPlugin 成员

        /// <summary>
        /// 设置应用程序框架。
        /// </summary>
        /// <param name="application"></param>
        public void SetApplication(FormApplicationClass application)
        {
        }

        #endregion

        #region IHtmlPlugin 成员

        /// <summary>
        /// 向页面输出HTML。
        /// </summary>
        /// <param name="output">字符输出对象，调用 WriteLine 方法向界面输出 HTML 内容。</param>
        public void WriterHtml(System.IO.TextWriter output)
        {
            output.WriteLine("系统信息<hr>");
            output.WriteLine(DateTime.Now.ToString());
        }

        #endregion
    }
}
