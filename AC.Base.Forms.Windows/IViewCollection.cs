using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Forms.Windows
{
    /// <summary>
    /// 视图集合。
    /// </summary>
    public interface IViewCollection : IEnumerable<ViewBase>
    {
        /// <summary>
        /// 将指定的视图加载到程序主窗体并显示。
        /// </summary>
        /// <param name="view">被加载的视图。</param>
        /// <param name="viewDock">该视图的停靠位置。</param>
        void Load(ViewBase view, ViewDockOptions viewDock);

        /// <summary>
        /// 获取指定的插件视图。
        /// </summary>
        /// <param name="plugin">已被加载至界面的插件。</param>
        /// <returns></returns>
        PluginViewBase GetView(Forms.IPlugin plugin);

        /// <summary>
        /// 获取指定的控件视图。
        /// </summary>
        /// <param name="control">已被加载至界面的控件。</param>
        /// <returns></returns>
        ControlView GetView(System.Windows.Forms.Control control);
    }
}
