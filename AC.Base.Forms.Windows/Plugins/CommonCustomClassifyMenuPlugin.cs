using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AC.Base.ClassifySearchs;
using AC.Base.Forms.Plugins;

namespace AC.Base.Forms.Windows.Plugins
{
    /// <summary>
    /// 公共菜单
    /// </summary>
    [GlobalPluginType("设备分类", "", typeof(ViewGlobalSortPlugin), 10, null, typeof(CommonCustomClassifyMenuPluginCheck))]
    public class CommonCustomClassifyMenuPlugin : System.Windows.Forms.ToolStripMenuItem, Forms.IGlobalPlugin, IUseAccount
    {
        private WindowsFormApplicationClass m_Application;
        private IAccount m_Account;

        #region << IUseAccount 成员 >>

        /// <summary>
        /// 设置当前操作员。
        /// </summary>
        /// <param name="account"></param>
        public void SetAccount(IAccount account)
        {
            this.m_Account = account;
        }

        #endregion

        /// <summary>
        /// 设置应用程序框架。
        /// </summary>
        /// <param name="application">应用程序框架。</param>
        public void SetApplication(Forms.FormApplicationClass application)
        {
            this.m_Application = (WindowsFormApplicationClass)application;
            ClassifySearch _Search = new ClassifySearch(this.m_Application);
            _Search.Filters.Add(new ParentIdFilter(0));
            _Search.Filters.Add(new ClassifyTypeFilter(typeof(CommonCustomClassify)));
            ClassifyCollection _Classifys = _Search.Search();

            if (_Classifys.Count > 0)
            {
                if (_Classifys.Count == 1)
                {
                    CommonCustomClassify _Classify = _Classifys[0] as CommonCustomClassify;
                    this.Text = _Classify.Name;
                    this.Tag = _Classify;
                    this.Image = _Classify.GetIcon16();
                    this.Click += new EventHandler(CommonCustomClassifyMenuPlugin_Click);
                }
                else
                {
                    if (base.OwnerItem is ToolStripMenuItem)
                    {
                        ToolStripMenuItem owner = base.OwnerItem as ToolStripMenuItem;

                        for (int intIndex = 0; intIndex < _Classifys.Count; intIndex++)
                        {
                            CommonCustomClassify _Classify = _Classifys[intIndex] as CommonCustomClassify;

                            if (intIndex == 0)
                            {
                                this.Text = _Classify.Name;
                                this.Tag = _Classify;
                                this.Image = _Classify.GetIcon16();
                                this.Click += new EventHandler(CommonCustomClassifyMenuPlugin_Click);
                            }
                            else
                            {
                                ToolStripMenuItem tsmi = new ToolStripMenuItem();
                                tsmi.Text = _Classify.Name;
                                tsmi.Tag = _Classify;
                                tsmi.Image = _Classify.GetIcon16();
                                tsmi.Click += new EventHandler(CommonCustomClassifyMenuPlugin_Click);
                                owner.DropDownItems.Add(tsmi);
                            }
                        }
                    }
                    else
                    {
                        for (int intIndex = 0; intIndex < _Classifys.Count; intIndex++)
                        {
                            CommonCustomClassify _Classify = _Classifys[intIndex] as CommonCustomClassify;
                            ToolStripMenuItem tsmi = new ToolStripMenuItem();
                            tsmi.Text = _Classify.Name;
                            tsmi.Tag = _Classify;
                            tsmi.Image = _Classify.GetIcon16();
                            tsmi.Click += new EventHandler(CommonCustomClassifyMenuPlugin_Click);
                            this.DropDownItems.Add(tsmi);
                        }
                    }
                }
            }
        }

        private void CommonCustomClassifyMenuPlugin_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem tsmi = sender as ToolStripMenuItem;
            if (tsmi != null)
            {
                CommonCustomClassify _Classify = tsmi.Tag as CommonCustomClassify;
                foreach (ViewBase _View in this.m_Application.Views)
                {
                    if (_View is ControlView)
                    {
                        ControlView _ControlView = _View as ControlView;
                        if (_ControlView.GetControl() is CommonCustomClassifyListControlView)
                        {
                            CommonCustomClassifyListControlView _CommonCustomClassifyListControlView = _ControlView.GetControl() as CommonCustomClassifyListControlView;
                            if (_CommonCustomClassifyListControlView.Classify != null && _CommonCustomClassifyListControlView.Classify.Equals(_Classify))
                            {
                                _ControlView.Focus();
                                return;
                            }
                        }
                    }
                }

                CommonCustomClassifyListControlView ctlControlView = new CommonCustomClassifyListControlView();
                ctlControlView.SetApplication(this.m_Application);
                ctlControlView.Classify = _Classify;

                ControlView view = new ControlView(this.m_Application, ctlControlView, _Classify.Name, _Classify.GetIcon16());
                this.m_Application.Views.Load(view, ViewDockOptions.Left);
            }
        }
    }
}
