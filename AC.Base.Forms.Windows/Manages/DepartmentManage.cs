using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AC.Base.Forms;
using AC.Base.ClassifySearchs;
using AC.Base;

namespace AC.Base.Forms.Windows.Manages
{
    /// <summary>
    /// 路灯部门管理。
    /// </summary>
    public class DepartmentManage : UserControl , ISystemManage
    {
        private FormApplicationClass m_Application;
        private TreeView tvClassify;
        private ToolStripButton tsbAddChildren;
        private ToolStripButton tsbUpdate;
        private ToolStripButton tsbDelete;

        /// <summary>
        /// 路灯部门管理
        /// </summary>
        public DepartmentManage()
        {
            tvClassify = new TreeView();
            tvClassify.HideSelection = false;
            tvClassify.Dock = System.Windows.Forms.DockStyle.Fill;
            tvClassify.AfterSelect += new TreeViewEventHandler(tvClassify_AfterSelect);
            base.Controls.Add(tvClassify);

            ToolStrip tsClassify = new ToolStrip();
            base.Controls.Add(tsClassify);

            tsbAddChildren = new ToolStripButton();
            tsbAddChildren.Text = "添加下级";
            tsbAddChildren.Enabled = false;
            tsbAddChildren.Click += new EventHandler(tsbAddChildren_Click);
            tsClassify.Items.Add(tsbAddChildren);

            tsbUpdate = new ToolStripButton();
            tsbUpdate.Text = "修改";
            tsbUpdate.Enabled = false;
            tsbUpdate.Click += new EventHandler(tsbUpdate_Click);
            tsClassify.Items.Add(tsbUpdate);

            tsbDelete = new ToolStripButton();
            tsbDelete.Text = "删除";
            tsbDelete.Enabled = false;
            tsbDelete.Click += new EventHandler(tsbDelete_Click);
            tsClassify.Items.Add(tsbDelete);

            ImageList img = new ImageList();
            img.Images.Add(Properties.Resources.Organization16);
            this.tvClassify.ImageList = img;
        }

        #region ISystemManage 成员

        /// <summary>
        /// 设置应用程序框架。
        /// </summary>
        /// <param name="application"></param>
        public void SetApplication(FormApplicationClass application)
        {
            this.m_Application = application;

            ClassifySearch _Search = new ClassifySearch(this.m_Application);
            _Search.Filters.Add(new ParentIdFilter(0));
            _Search.Filters.Add(new ClassifyTypeFilter(typeof(Department)));
            this.Add(_Search.Search(), this.tvClassify.Nodes);

            if (this.tvClassify.Nodes.Count > 0)
            {
                this.tvClassify.ExpandAll();
            }
        }

        #endregion

        private void Add(IClassifyCollection classifys, TreeNodeCollection treeNodes)
        {
            foreach (Department classify in classifys)
            {
                TreeNode tn = new TreeNode();
                this.FillClassifyNode(classify, tn);
                treeNodes.Add(tn);
                if (classify.Children.Count > 0)
                {
                    this.Add(classify.Children, tn.Nodes);
                }
            }
        }

        private void tsbAddChildren_Click(object sender, EventArgs e)
        {
            Department _Parent = this.tvClassify.SelectedNode.Tag as Department;
            Department _Classify = this.m_Application.ClassifyTypes.GetClassifyType(typeof(Department)).CreateClassify(_Parent) as Department;
            _Classify.OrdinalNumber = _Parent.Children.Count + 1;

            DepartmentManageForm _DepartmentManageForm = new DepartmentManageForm(_Classify, false);
            if (_DepartmentManageForm.ShowDialog(this) == DialogResult.OK)
            {
                try
                {
                    _DepartmentManageForm.Classify.Save();

                    TreeNode tn = new TreeNode();
                    this.FillClassifyNode(_DepartmentManageForm.Classify, tn);
                    this.tvClassify.SelectedNode.Nodes.Add(tn);
                    this.tvClassify.SelectedNode.Expand();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "部门", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void tsbUpdate_Click(object sender, EventArgs e)
        {
            Department _Classify = this.tvClassify.SelectedNode.Tag as Department;

            DepartmentManageForm _DepartmentManageForm = new DepartmentManageForm(_Classify, true);
            if (_DepartmentManageForm.ShowDialog(this) == DialogResult.OK)
            {
                try
                {
                    _DepartmentManageForm.Classify.Save();

                    this.FillClassifyNode(_DepartmentManageForm.Classify, this.tvClassify.SelectedNode);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "部门", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void tsbDelete_Click(object sender, EventArgs e)
        {
            Department _Classify = this.tvClassify.SelectedNode.Tag as Department;

            if (MessageBox.Show("确实要删除“" + _Classify.Name + "”及其子部门吗？", "部门", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {
                _Classify.Delete();
                this.tvClassify.SelectedNode.Remove();
            }
        }

        private void tvClassify_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (this.tvClassify.SelectedNode == null)
            {
                this.tsbAddChildren.Enabled = false;
                this.tsbUpdate.Enabled = false;
                this.tsbDelete.Enabled = false;
            }
            else
            {
                this.tsbAddChildren.Enabled = true;
                this.tsbUpdate.Enabled = true;
                this.tsbDelete.Enabled = true;
            }
        }

        private void FillClassifyNode(Department classify, TreeNode treeNode)
        {
            treeNode.Text = classify.Name;
            treeNode.Tag = classify;
            treeNode.ImageIndex = 0;
            treeNode.SelectedImageIndex = 0;
        }
    }
}
