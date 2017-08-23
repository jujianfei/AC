using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AC.Base.Forms.Windows
{
    /// <summary>
    /// 在应用程序中心区域或四周显示指定控件的视图界面。
    /// </summary>
    public class ControlView : ViewBase
    {
        private System.Windows.Forms.Control m_Control;
        private string m_Title;
        private System.Drawing.Image m_Icon16;

        /// <summary>
        /// 在应用程序中心区域或四周显示指定控件的视图界面。
        /// </summary>
        /// <param name="application">应用程序框架。</param>
        /// <param name="control">需要显示的控件，该控件必需实现 IControlView 接口并提供无参数的构造函数，另外可根据需要添加 IUseAccount 接口。</param>
        /// <param name="title">显示在视图标签页上的文字</param>
        /// <param name="icon16">显示在视图标签页上的图标。如不需显示图标则传入null。</param>
        public ControlView(WindowsFormApplicationClass application, System.Windows.Forms.Control control, string title, System.Drawing.Image icon16)
            : base(application)
        {
            if ((control is IControlView) == false)
            {
                throw new Exception(control.GetType().FullName + " 必须实现 IControlView 接口方可作为控件视图呈现在界面上。");
            }

            System.Reflection.ConstructorInfo ci = control.GetType().GetConstructor(new System.Type[] { });
            if (ci == null)
            {
                throw new Exception(control.GetType().FullName + " 必须无参数的构造函数方可作为控件视图呈现在界面上。");
            }

            this.m_Control = control;
            this.m_Title = title;
            this.m_Icon16 = icon16;
        }

        /// <summary>
        /// 获取该视图所呈现的包含参数设置、插件控件、翻页控件的界面。
        /// </summary>
        /// <returns></returns>
        public override System.Windows.Forms.Control CreateViewControl()
        {
            return this.m_Control;
        }

        /// <summary>
        /// 获取显示在视图标签页上的文字。
        /// </summary>
        /// <returns></returns>
        public override string GetViewTitle()
        {
            return this.m_Title;
        }

        /// <summary>
        /// 获取当前视图内被加载的控件。
        /// </summary>
        /// <returns></returns>
        public override Control GetControl()
        {
            return this.m_Control;
        }

        /// <summary>
        /// 获取当前视图16*16像素的图标。
        /// </summary>
        /// <returns></returns>
        public override System.Drawing.Image GetIcon16()
        {
            return this.m_Icon16;
        }

        /// <summary>
        /// 释放该视图使用的所有资源。
        /// </summary>
        public override void Dispose()
        {
            this.m_Control.Dispose();
        }

        /// <summary>
        /// 确定指定的 System.Object 是否等于当前的 System.Object。
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is Control)
            {
                Control ctl = obj as Control;
                return ctl.Equals(this.m_Control);
            }
            else
            {
                return base.Equals(obj);
            }
        }

        /// <summary>
        /// 用作特定类型的哈希函数。
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
