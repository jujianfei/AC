using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AC.Base.Forms.Windows;
using AC.Base.Database;
using AC.Base;
using System.Net;

namespace MainForm
{
    internal class MainApplicationClass : WindowsFormApplicationClass
    {
        internal frmMain m_MainForm;
        internal protected MainApplicationClass()
        {
            base.SetDbConnection(DbConnection.LoadDbConnectionConfig(DbConnection.DbConnectionConfigFileName));
            base.ViewRequestFocus += new ViewRequestFocusEventHandler(MainApplicationClass_ViewRequestFocus);
            base.ViewRequestClose += new ViewRequestCloseEventHandler(MainApplicationClass_ViewRequestClose);
        }

        public new void AddType(Type type)
        {
            base.AddType(type);
        }

        public IAccount m_CurrentAccount;
        /// <summary>
        /// 成功登录的账号。
        /// </summary>
        public override IAccount CurrentAccount
        {
            get
            {
                return this.m_CurrentAccount;
            }
        }

        private ViewCollection m_Views;
        public override IViewCollection Views
        {
            get
            {
                if (this.m_Views == null)
                {
                    this.m_Views = new ViewCollection(this.m_MainForm);
                }
                return this.m_Views;
            }
        }

        internal new void OnViewLoaded(ViewBase view, ViewDockOptions viewDock)
        {
            base.OnViewLoaded(view, viewDock);
        }

        /// <summary>
        /// 引发视图可见状态发生改变后的事件。
        /// </summary>
        /// <param name="view">可见状态发生改变的视图。</param>
        /// <param name="visible">是否可见。</param>
        internal new void OnViewVisibleChanged(ViewBase view, bool visible)
        {
            base.OnViewVisibleChanged(view, visible);
        }

        internal new void OnViewClosed(ViewBase view)
        {
            base.OnViewClosed(view);
        }

        private void MainApplicationClass_ViewRequestFocus(ViewBase view)
        {
            //this.m_MainForm.ViewRequestFocus(view);
        }

        private void MainApplicationClass_ViewRequestClose(ViewBase view)
        {
            //this.m_MainForm.ViewRequestClose(view);
        }
    }
}
