using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Forms
{
    /// <summary>
    /// 可呈现界面或某一执行逻辑的全局插件。
    /// 实现该接口的插件将显示在桌面应用程序或WEB应用程序的主菜单中，如果希望将插件添加到桌面应用程序中，则必须从 System.Windows.Forms.Control 或 System.Windows.Forms.ToolStripItem 继承，如果希望将插件添加到WEB应用中，则必须从 System.Web.UI.Control 继承。
    /// 实现该接口的插件必须添加 GlobalPluginTypeAttribute 特性，如果需要账号信息可以添加 IUseAccount 接口。
    /// 注：如果实现 IGlobalHtmlPlugin 接口，可以以 HTML 代码方式编写桌面应用及 WEB 应用均适用的插件。
    /// </summary>
    public interface IGlobalPlugin : IPlugin
    {
    }
}
