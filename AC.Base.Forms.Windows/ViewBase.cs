using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AC.Base.Forms;

namespace AC.Base.Forms.Windows
{
    /// <summary>
    /// 在应用程序中心区域或四周显示的视图界面。
    /// </summary>
    public abstract class ViewBase
    {
        /// <summary>
        /// 在应用程序中心区域或四周显示的视图界面。
        /// </summary>
        /// <param name="application">应用程序框架。</param>
        public ViewBase(WindowsFormApplicationClass application)
        {
            this.Application = application;
        }

        /// <summary>
        /// 应用程序框架。
        /// </summary>
        public WindowsFormApplicationClass Application { get; private set; }

        /// <summary>
        /// 创建该视图所呈现的包含参数设置、插件控件、翻页控件的界面。
        /// </summary>
        /// <returns></returns>
        public abstract Control CreateViewControl();

        /// <summary>
        /// 获取显示在视图标签页上的文字。
        /// </summary>
        /// <returns></returns>
        public abstract string GetViewTitle();

        /// <summary>
        /// 获取当前视图内的控件。如果插件继承 System.Windows.Forms.Control 则返回该插件的实例，如果是 HTML 插件则返回 WebBrowser，如果是 ControlView 则返回当前被加载的控件。
        /// </summary>
        /// <returns></returns>
        public abstract Control GetControl();

        /// <summary>
        /// 获取当前视图16*16像素的图标。
        /// </summary>
        /// <returns></returns>
        public abstract System.Drawing.Image GetIcon16();

        /// <summary>
        /// 释放该视图使用的所有资源。
        /// </summary>
        public abstract void Dispose();

        /// <summary>
        /// 获取当前视图的可见状态。如果该视图在主程序中可见，则返回true；如果被至于其它Tab页之后或该视图被隐藏则返回false。
        /// </summary>
        public bool Visible { get; private set; }

        /// <summary>
        /// 当视图的可见状态发生改变后产生的事件。
        /// </summary>
        public event ViewVisibleChangedEventHandler ViewVisibleChanged;

        //引发视图可见状态发生改变后的事件，该方法由WindowsFormApplicationClass调用。
        internal virtual void OnViewVisibleChanged(bool visible)
        {
            this.Visible = visible;

            if (this.ViewVisibleChanged != null)
            {
                this.ViewVisibleChanged(this);
            }
        }

        /// <summary>
        /// 当前视图被关闭后产生的事件。
        /// </summary>
        public event ViewClosedEventHandler ViewClosed;

        //引发视图被关闭后的事件，该方法由WindowsFormApplicationClass调用。
        internal virtual void OnViewClosed()
        {
            if (ViewClosed != null)
            {
                this.ViewClosed(this);
            }
        }

        /// <summary>
        /// 将当前视图呈现在其它视图前端，并获取焦点。
        /// </summary>
        public void Focus()
        {
            this.Application.OnViewRequestFocus(this);
        }

        /// <summary>
        /// 关闭当前视图，并释放所有资源。
        /// </summary>
        public void Close()
        {
            this.Application.OnViewRequestClose(this);
        }

        /// <summary>
        /// 获取当前对象的字符串表示形式。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.GetViewTitle();
        }
    }
}
