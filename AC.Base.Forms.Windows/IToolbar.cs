using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AC.Base.Forms.Windows
{
    /// <summary>
    /// 可在应用程序工具栏容器中呈现的一个工具栏。实现该接口的状态栏项必须继承自System.Windows.Forms.ToolStrip，同时提供无参数的构造函数，并且添加ToolbarTypeAttribute特性。
    /// </summary>
    public interface IToolbar
    {
        /// <summary>
        /// 设置应用程序框架。
        /// </summary>
        /// <param name="application">应用程序框架。</param>
        void SetApplication(WindowsFormApplicationClass application);
    }
}
