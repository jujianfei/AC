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
    /// 机构配置。
    /// </summary>
    [SystemConfig("机构配置", typeof(OrganizationConfigIcon))]
    public partial class OrganizationConfig : UserControl, ISystemConfig
    {
        private FormApplicationClass m_Application;

        /// <summary>
        /// 机构配置。
        /// </summary>
        public OrganizationConfig()
        {
            InitializeComponent();
        }

        #region ISystemConfig 成员

        /// <summary>
        /// 设置应用程序框架。
        /// </summary>
        /// <param name="application">应用程序框架。</param>
        public void SetApplication(FormApplicationClass application)
        {
            this.m_Application = application;
        }

        #endregion

        private Organization m_Organization;
        Form f;
        private void OrganizationConfig_Load(object sender, EventArgs e)
        {
            f = this.FindForm();

            ClassifySearch _Search = new ClassifySearch(this.m_Application);
            _Search.Filters.Add(new ParentIdFilter(0));
            _Search.Filters.Add(new ClassifyTypeFilter(typeof(Organization)));
            foreach (Organization _Organization in _Search.Search())
            {
                this.m_Organization = _Organization;
                this.txtName.Text = this.m_Organization.Name;
                break;
            }


            foreach (ClassifyType _ClassifyType in this.m_Application.ClassifyTypes)
            {
                if (Function.IsInheritableBaseType(_ClassifyType.Type, typeof(AC.Base.Organization)))
                {
                  
                    this.cmbClassifyType.Items.Add(_ClassifyType);

                    if (this.m_Organization != null && this.m_Organization.ClassifyType.Equals(_ClassifyType))
                    {
                        this.cmbClassifyType.SelectedItem = _ClassifyType;
                    }
                }
            }

            if (this.cmbClassifyType.SelectedIndex == -1 && this.cmbClassifyType.Items.Count > 0)
            {
                this.cmbClassifyType.SelectedIndex = 0;
            }
        }

        private void btnAccept_Click(object sender, EventArgs e)
        {
            if (this.cmbClassifyType.SelectedItem == null || (this.cmbClassifyType.SelectedItem is ClassifyType) == false)
            {
                MessageBox.Show("必须选择机构类型。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (this.txtName.Text.Length == 0)
            {
                MessageBox.Show("请输入根节点名称。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (this.m_Organization == null)
            {
                if (MessageBox.Show("确定创建名为“" + this.txtName.Text + "”类型是“" + this.cmbClassifyType.SelectedItem + "”的根节点机构吗？", this.Text, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.OK)
                {
                    ClassifyType _ClassifyType = this.cmbClassifyType.SelectedItem as ClassifyType;
                    Organization _Organization = _ClassifyType.CreateClassify() as Organization;
                    _Organization.Name = this.txtName.Text;
                    _Organization.Save();
                    this.m_Organization = _Organization;

                    MessageBox.Show("根节点机构“" + _Organization.Name + "”已创建。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                if (this.m_Organization.ClassifyType.Equals(this.cmbClassifyType.SelectedItem))
                {
                    if (this.m_Organization.Name.Equals(this.txtName.Text) == false)
                    {
                        if (MessageBox.Show("确定要将“" + this.m_Organization.Name + "”修改为“" + this.txtName.Text + "”吗？", this.Text, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.OK)
                        {
                            this.m_Organization.Name = this.txtName.Text;
                            this.m_Organization.Save();
                            MessageBox.Show("根节点机构“" + this.m_Organization.Name + "”已改名。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            f.Close();
                        }
                    }
                }
                else
                {
                    if (MessageBox.Show("确定要将类型是“" + this.m_Organization.ClassifyType.Name + "”的根节点机构更改为“" + this.cmbClassifyType.SelectedItem + "”类型吗？", this.Text, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.OK)
                    {
                        if (MessageBox.Show("变更根节点机构的类型将删除现有机构数据及机构与设备的对应关系，是否继续？", this.Text, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.OK)
                        {
                            this.m_Organization.Delete();
                            this.m_Organization = null;

                            ClassifyType _ClassifyType = this.cmbClassifyType.SelectedItem as ClassifyType;
                            Organization _Organization = _ClassifyType.CreateClassify() as Organization;
                            _Organization.Name = this.txtName.Text;
                            _Organization.Save();
                            this.m_Organization = _Organization;
                            MessageBox.Show("根节点机构“" + _Organization.Name + "”已重新创建。", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                           
                            f.Close();
                        }
                    }
                }
            }
        }
    }
}
