using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AC.Base.Forms.Windows.Manages
{
    class CommonCustomClassifyManageTabPage : TabPage
    {
        private TreeView tvClassify;
        private ToolStripButton tsbAddChildren;
        private ToolStripButton tsbUpdate;
        private ToolStripButton tsbDelete;

        public CommonCustomClassifyManageTabPage(CommonCustomClassify classify)
        {
            this.Classify = classify;

            base.Text = this.Classify.Name;

            tvClassify = new TreeView();
            tvClassify.HideSelection = false;
            tvClassify.Dock = System.Windows.Forms.DockStyle.Fill;
            tvClassify.AfterSelect += new TreeViewEventHandler(tvClassify_AfterSelect);
            base.Controls.Add(tvClassify);

            ToolStrip tsClassify = new ToolStrip();
            base.Controls.Add(tsClassify);

            ToolStripButton tsbAdd = new ToolStripButton();
            tsbAdd.Text = "添加";
            tsbAdd.Click += new EventHandler(tsbAdd_Click);
            tsClassify.Items.Add(tsbAdd);

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
            img.Images.Add(Properties.Resources.CommonClassifyRoot16);
            img.Images.Add(Properties.Resources.CommonClassify16);
            img.Images.Add(Properties.Resources.CommonClassifyItem16);
            this.tvClassify.ImageList = img;

            this.Add(this.Classify.Children, this.tvClassify.Nodes);
        }

        private void Add(IClassifyCollection classifys, TreeNodeCollection treeNodes)
        {
            foreach (CommonCustomClassify classify in classifys)
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

        private void tsbAdd_Click(object sender, EventArgs e)
        {
            CommonCustomClassify _Classify = this.Classify.Application.ClassifyTypes.GetClassifyType(typeof(CommonCustomClassify)).CreateClassify(this.Classify) as CommonCustomClassify;

            CommonCustomClassifyManageForm _CommonCustomClassifyManageForm = new CommonCustomClassifyManageForm(_Classify, false);
            if (_CommonCustomClassifyManageForm.ShowDialog(this) == DialogResult.OK)
            {
                try
                {
                    _CommonCustomClassifyManageForm.Classify.Save();

                    TreeNode tn = new TreeNode();
                    this.FillClassifyNode(_CommonCustomClassifyManageForm.Classify, tn);
                    this.tvClassify.Nodes.Add(tn);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "公共分类", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void tsbAddChildren_Click(object sender, EventArgs e)
        {
            CommonCustomClassify _Parent = this.tvClassify.SelectedNode.Tag as CommonCustomClassify;
            CommonCustomClassify _Classify = this.Classify.Application.ClassifyTypes.GetClassifyType(typeof(CommonCustomClassify)).CreateClassify(_Parent) as CommonCustomClassify;

            CommonCustomClassifyManageForm _CommonCustomClassifyManageForm = new CommonCustomClassifyManageForm(_Classify, false);
            if (_CommonCustomClassifyManageForm.ShowDialog(this) == DialogResult.OK)
            {
                try
                {
                    _CommonCustomClassifyManageForm.Classify.Save();

                    TreeNode tn = new TreeNode();
                    this.FillClassifyNode(_CommonCustomClassifyManageForm.Classify, tn);
                    this.tvClassify.SelectedNode.Nodes.Add(tn);
                    this.tvClassify.SelectedNode.Expand();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "公共分类", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void tsbUpdate_Click(object sender, EventArgs e)
        {
            CommonCustomClassify _Classify = this.tvClassify.SelectedNode.Tag as CommonCustomClassify;

            CommonCustomClassifyManageForm _CommonCustomClassifyManageForm = new CommonCustomClassifyManageForm(_Classify, true);
            if (_CommonCustomClassifyManageForm.ShowDialog(this) == DialogResult.OK)
            {
                try
                {
                    _CommonCustomClassifyManageForm.Classify.Save();

                    this.FillClassifyNode(_CommonCustomClassifyManageForm.Classify, this.tvClassify.SelectedNode);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "公共分类", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void tsbDelete_Click(object sender, EventArgs e)
        {
            CommonCustomClassify _Classify = this.tvClassify.SelectedNode.Tag as CommonCustomClassify;

            if (MessageBox.Show("确实要删除“" + _Classify.Name + "”及其子分类吗？", "公共分类", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
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

        public CommonCustomClassify Classify { get; private set; }

        private void FillClassifyNode(CommonCustomClassify classify, TreeNode treeNode)
        {
            treeNode.Text = classify.Name;
            treeNode.Tag = classify;

            if (classify.Parent == null)
            {
                treeNode.ImageIndex = 1;
                treeNode.SelectedImageIndex = 1;
            }
            else
            {
                treeNode.ImageIndex = 2;
                treeNode.SelectedImageIndex = 2;
            }
        }
    }
}
