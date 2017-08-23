using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AC.Base.ClassifySearchs;

namespace AC.Base.Forms.Windows.Manages
{
    /// <summary>
    /// 管理公共分类。
    /// </summary>
    [SystemManage("公共分类", null)]
    public partial class CommonCustomClassifyManage : UserControl, ISystemManage
    {
        private FormApplicationClass m_Application;

        /// <summary>
        /// 管理公共分类。
        /// </summary>
        public CommonCustomClassifyManage()
        {
            InitializeComponent();
        }

        #region ISystemManage 成员

        /// <summary>
        /// 设置应用程序框架。
        /// </summary>
        /// <param name="application">应用程序框架。</param>
        public void SetApplication(FormApplicationClass application)
        {
            this.m_Application = application;

            ClassifySearch _Search = new ClassifySearch(this.m_Application);
            _Search.Filters.Add(new ParentIdFilter(0));
            _Search.Filters.Add(new ClassifyTypeFilter(typeof(CommonCustomClassify)));
            foreach (CommonCustomClassify _Classify in _Search.Search())
            {
                this.tabClassify.TabPages.Add(new CommonCustomClassifyManageTabPage(_Classify));
            }

            this.tabClassify_SelectedIndexChanged(null, null);
        }

        #endregion

        private void tabClassify_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.btnUpdateRoot.Enabled = (this.tabClassify.TabCount > 0);
            this.btnDeleteRoot.Enabled = (this.tabClassify.TabCount > 0);
        }

        private void btnAddRoot_Click(object sender, EventArgs e)
        {
            CommonCustomClassify _Classify = this.m_Application.ClassifyTypes.GetClassifyType(typeof(CommonCustomClassify)).CreateClassify() as CommonCustomClassify;

            CommonCustomClassifyManageForm _CommonCustomClassifyManageForm = new CommonCustomClassifyManageForm(_Classify, false);
            if (_CommonCustomClassifyManageForm.ShowDialog(this) == DialogResult.OK)
            {
                try
                {
                    _CommonCustomClassifyManageForm.Classify.Save();
                    this.tabClassify.TabPages.Add(new CommonCustomClassifyManageTabPage(_CommonCustomClassifyManageForm.Classify));
                    this.tabClassify.SelectedIndex = this.tabClassify.TabPages.Count - 1;

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "公共分类", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnUpdateRoot_Click(object sender, EventArgs e)
        {
            CommonCustomClassifyManageTabPage _ClassifyTabPage = this.tabClassify.SelectedTab as CommonCustomClassifyManageTabPage;
            CommonCustomClassify _Classify = _ClassifyTabPage.Classify;

            CommonCustomClassifyManageForm _CommonCustomClassifyManageForm = new CommonCustomClassifyManageForm(_Classify, true);
            if (_CommonCustomClassifyManageForm.ShowDialog(this) == DialogResult.OK)
            {
                try
                {
                    _CommonCustomClassifyManageForm.Classify.Save();
                    _ClassifyTabPage.Text = _CommonCustomClassifyManageForm.Classify.Name;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "公共分类", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnDeleteRoot_Click(object sender, EventArgs e)
        {
            try
            {
                CommonCustomClassifyManageTabPage _ClassifyTabPage = this.tabClassify.SelectedTab as CommonCustomClassifyManageTabPage;
                CommonCustomClassify _Classify = _ClassifyTabPage.Classify;

                if (MessageBox.Show("确实要删除“" + _Classify.Name + "”分类及其所有的子分类吗？", "公共分类", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                {
                    _Classify.Delete();
                    this.tabClassify.TabPages.Remove(_ClassifyTabPage);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
