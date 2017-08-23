using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AC.Base.Forms.Windows
{
    /// <summary>
    /// 分类树控件。使用该控件必须首先调用 SetApplication 方法，然后设置 Classifys 属性。
    /// </summary>
    public class ClassifyTree : TreeView
    {
        private WindowsFormApplicationClass m_Application;
        private ImageList img;

        /// <summary>
        /// 设置应用程序框架。
        /// </summary>
        /// <param name="application"></param>
        public void SetApplication(WindowsFormApplicationClass application)
        {
            this.m_Application = application;
            this.img = new ImageList();

            this.ImageList = this.img;
            this.ItemHeight = 18;
            this.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.AfterSelect += new TreeViewEventHandler(treeView_AfterSelect);
            this.AfterExpand += new TreeViewEventHandler(treeView_AfterExpand);
            this.MouseClick += new MouseEventHandler(treeView_MouseClick);
        }

        private IClassifyCollection m_Classifys;
        /// <summary>
        /// 该分类列表所显示的分类集合。
        /// </summary>
        public IClassifyCollection Classifys
        {
            get
            {
                return this.m_Classifys;
            }
            set
            {
                this.m_Classifys = value;

                this.Nodes.Clear();
                this.img.Images.Clear();

                if (this.Classifys != null)
                {
                    this.AddClassifys(this.Classifys, this.Nodes);
                }
            }
        }

        private void treeView_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                System.Windows.Forms.TreeView tv = (System.Windows.Forms.TreeView)sender;
                tv.SelectedNode = tv.GetNodeAt(e.Location);
            }
        }

        private void treeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.ContextMenuStrip == null && e.Node.Tag != null)
            {
                if (e.Node.Tag is Classify)
                {
                    Classify _Classify = e.Node.Tag as Classify;
                    e.Node.ContextMenuStrip = this.m_Application.GetClassifyMenu(new Classify[] { _Classify });
                }
                else if (e.Node.Tag is Device)
                {
                    Device _Device = e.Node.Tag as Device;
                    e.Node.ContextMenuStrip = this.m_Application.GetDeviceMenu(new Device[] { _Device });
                }
            }
        }

        private void treeView_AfterExpand(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Tag != null && e.Node.Nodes.Count == 1 && e.Node.Nodes[0].Tag == null)
            {
                e.Node.Nodes.Clear();

                if (e.Node.Tag is Classify)
                {
                    AC.Base.Classify _Classify = e.Node.Tag as Classify;
                    this.AddClassifys(_Classify.Children, e.Node.Nodes);
                    this.AddDevices(_Classify.GetDevices(), e.Node.Nodes);
                }
                else if (e.Node.Tag is Device)
                {
                    Device _Device = e.Node.Tag as Device;
                    this.AddDevices(_Device.Children, e.Node.Nodes);
                }
            }
        }

        private void AddClassifys(IClassifyCollection classifys, TreeNodeCollection treeNodes)
        {
            foreach (Classify _Classify in classifys)
            {
                string strImageKey = "C" + _Classify.ClassifyId;

                if (this.img.Images.ContainsKey(strImageKey) == false)
                {
                    this.img.Images.Add(strImageKey, _Classify.GetIcon16());
                }

                TreeNode tn = new TreeNode();
                tn.Text = _Classify.Name;
                tn.Tag = _Classify;
                tn.ImageKey = strImageKey;
                tn.SelectedImageKey = strImageKey;
                treeNodes.Add(tn);

                if (_Classify.Children.Count > 0 || _Classify.DeviceCount > 0)
                {
                    tn.Nodes.Add("", "", 3, 3);
                }
            }
        }

        private void AddDevices(IDeviceCollection devices, TreeNodeCollection treeNodes)
        {
            foreach (Device device in devices)
            {
                TreeNode tn = new TreeNode();
                tn.Text = device.Name;
                tn.Tag = device;

                string strImageKey = device.DeviceType.Code + ((int)device.State).ToString();
                if (this.img.Images.ContainsKey(strImageKey) == false)
                {
                    this.img.Images.Add(strImageKey, device.GetIcon16());
                }
                tn.ImageKey = strImageKey;
                tn.SelectedImageKey = strImageKey;

                if (device.Children.Count > 0)
                {
                    tn.Nodes.Add("");
                }

                treeNodes.Add(tn);
            }
        }

    }
}
