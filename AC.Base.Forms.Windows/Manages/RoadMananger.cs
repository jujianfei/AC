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
    /// 路灯位置管理。
    /// </summary>
    [SystemManage("路段灯杆配置", typeof(RoadPosition))]
    public partial class RoadMananger : UserControl , ISystemManage
    {
        private FormApplicationClass m_Application;

        /// <summary>
        /// 初始化
        /// </summary>
        public RoadMananger()
        {
            InitializeComponent();
        }

        #region << ISystemManage 成员 >>

        /// <summary>
        /// 设置应用程序框架。
        /// </summary>
        /// <param name="application"></param>
        public void SetApplication(FormApplicationClass application)
        {
            this.m_Application = application;

            ClassifySearch _Search = new ClassifySearch(this.m_Application);
            _Search.Filters.Add(new ParentIdFilter(0));
            _Search.Filters.Add(new ClassifyTypeFilter(typeof(RoadPosition)));
            this.Add(_Search.Search(), this.tvClassify.Nodes);

            if (this.tvClassify.Nodes.Count > 0)
            {
                this.tvClassify.ExpandAll();
            }
        }

        #endregion


        private void Add(IClassifyCollection classifys, TreeNodeCollection treeNodes)
        {
            if (classifys.Count == 0)
            {
                this.btnAddroot.Enabled = true;
                this.btnAddRoad.Enabled = false;
                this.btnAddlamp.Enabled = false;
                this.btnUpdate.Enabled = false;
                this.btnDelete.Enabled = false;
            }
            else
            {
                btnAddroot.Enabled = false;
                foreach (RoadPosition classify in classifys)
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
        }

        private void btnAddroot_Click(object sender, EventArgs e)
        {
            RoadPosition _Classify = this.m_Application.ClassifyTypes.GetClassifyType(typeof(RoadPosition)).CreateClassify() as RoadPosition;
            RoadClassifyManageForm frm = new RoadClassifyManageForm(_Classify, false);
            frm.Text = "添加城市名称";
            if (frm.ShowDialog(this) == DialogResult.OK)
            {
                try
                {
                    frm.Classify.Save();
                    btnAddroot.Enabled = false;
                    TreeNode tn = new TreeNode();
                    this.FillClassifyNode(frm.Classify, tn);
                    this.tvClassify.Nodes.Add(tn);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "公共分类", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnAddRoad_Click(object sender, EventArgs e)
        {
            RoadPosition _Parent = this.tvClassify.SelectedNode.Tag as RoadPosition;
            RoadPosition _Classify = this.m_Application.ClassifyTypes.GetClassifyType(typeof(RoadPosition)).CreateClassify(_Parent) as RoadPosition;
            _Classify.OrdinalNumber = _Parent.Children.Count + 1;

            RoadClassifyManageForm frm = new RoadClassifyManageForm(_Classify, false);
            frm.Text = "添加路段名称";
            if (frm.ShowDialog(this) == DialogResult.OK)
            {
                try
                {
                    frm.Classify.Save();
                    TreeNode tn = new TreeNode();
                    this.FillClassifyNode(frm.Classify, tn);
                    this.tvClassify.SelectedNode.Nodes.Add(tn);
                    this.tvClassify.SelectedNode.Expand();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "位置", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnAddlamp_Click(object sender, EventArgs e)
        {
            RoadPosition _Parent = this.tvClassify.SelectedNode.Tag as RoadPosition;
            RoadPosition _Classify = this.m_Application.ClassifyTypes.GetClassifyType(typeof(RoadPosition)).CreateClassify(_Parent) as RoadPosition;
            _Classify.OrdinalNumber = _Parent.Children.Count + 1;

            RoadClassifyManageForm frm = new RoadClassifyManageForm(_Classify, false);
            frm.Text = "添加灯杆名称";
            if (frm.ShowDialog(this) == DialogResult.OK)
            {
                try
                {
                    frm.Classify.Save();
                    TreeNode tn = new TreeNode();
                    this.FillClassifyNode(frm.Classify, tn);
                    this.tvClassify.SelectedNode.Nodes.Add(tn);
                    this.tvClassify.SelectedNode.Expand();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "位置", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            RoadPosition _Classify = this.tvClassify.SelectedNode.Tag as RoadPosition;
            RoadClassifyManageForm frm = new RoadClassifyManageForm(_Classify, true);
            frm.Text = "修改名称";
            if (frm.ShowDialog(this) == DialogResult.OK)
            {
                try
                {
                    frm.Classify.Save();
                    this.FillClassifyNode(frm.Classify, this.tvClassify.SelectedNode);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "位置", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            RoadPosition _Classify = this.tvClassify.SelectedNode.Tag as RoadPosition;
            if (MessageBox.Show("确实要删除“" + _Classify.Name + "”及其下级位置吗？", "位置", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {
                _Classify.Delete();
                this.tvClassify.SelectedNode.Remove();
            }
        }

        private void tvClassify_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (this.tvClassify.SelectedNode == null)
            {
                this.btnAddRoad.Enabled = false;
                this.btnAddlamp.Enabled = false;
                this.btnUpdate.Enabled = false;
                this.btnDelete.Enabled = false;
            }
            else
            {
                if (this.tvClassify.SelectedNode.Parent != null && this.tvClassify.SelectedNode.Parent.Parent != null)
                {
                    this.btnAddRoad.Enabled = false;
                    this.btnAddlamp.Enabled = false;
                    this.btnUpdate.Enabled = true;
                    this.btnDelete.Enabled = true;
                }
                else if (this.tvClassify.SelectedNode.Parent != null)
                {
                    this.btnAddRoad.Enabled = false;
                    this.btnAddlamp.Enabled = true;
                    this.btnUpdate.Enabled = true;
                    this.btnDelete.Enabled = true;
                }
                else
                {
                    this.btnAddRoad.Enabled = true;
                    this.btnAddlamp.Enabled = false;
                    this.btnUpdate.Enabled = true;
                    this.btnDelete.Enabled = false;
                }
            }
        }

        private void FillClassifyNode(RoadPosition classify, TreeNode treeNode)
        {
            treeNode.Text = classify.Name;
            treeNode.Tag = classify;
            treeNode.ImageIndex = 0;
            treeNode.SelectedImageIndex = 0;
        }
    }
}
