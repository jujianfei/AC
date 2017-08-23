using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Forms.Windows
{
    /// <summary>
    /// 可在应用程序状态栏中呈现的一个状态栏项。实现该接口的状态栏项必须继承自System.Windows.Forms.ToolStripItem，同时提供无参数的构造函数，并且添加StatusBarItemTypeAttribute特性。
    /// 可以继承已有的状态栏项重写该项的功能。
    /// </summary>
    public interface IStatusBarItem
    {
        /// <summary>
        /// 设置应用程序框架。
        /// </summary>
        /// <param name="application">应用程序框架。</param>
        void SetApplication(WindowsFormApplicationClass application);
    }
}
