using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AC.Base.ClassifySearchs;

namespace AC.Base.Forms.Windows.Plugins
{
    /// <summary>
    /// 公共分类列表菜单所使用的控件视图中的控件。
    /// </summary>
    public class CommonCustomClassifyListControlView : System.Windows.Forms.Control, IControlView
    {
        private TreeView treeView;
        private ImageList img;
        private WindowsFormApplicationClass m_Application;

        #region IControlView 成员

        private Classify m_Classify;
        /// <summary>
        /// 该分类列表所显示的分类。
        /// </summary>
        public Classify Classify
        {
            get
            {
                return this.m_Classify;
            }
            set
            {
                this.m_Classify = value;

                if (this.Classify != null)
                {
                    this.AddClassifys(this.Classify.Children, this.treeView.Nodes);
                }
            }
        }


        /// <summary>
        /// 设置应用程序框架。
        /// </summary>
        /// <param name="application"></param>
        public void SetApplication(WindowsFormApplicationClass application)
        {
            this.m_Application = application;
            this.img = new ImageList();
            this.img.Images.Add(Properties.Resources.CommonClassifyRoot16);
            this.img.Images.Add(Properties.Resources.CommonClassify16);
            this.img.Images.Add(Properties.Resources.CommonClassifyItem16);
            this.img.Images.Add(Properties.Resources.Transparent);

            this.treeView = new TreeView();
            this.treeView.Dock = DockStyle.Fill;
            this.treeView.BorderStyle = BorderStyle.None;
            this.treeView.ImageList = this.img;
            this.treeView.ItemHeight = 18;
            this.treeView.AfterSelect += new TreeViewEventHandler(treeView_AfterSelect);
            this.treeView.AfterExpand += new TreeViewEventHandler(treeView_AfterExpand);
            this.treeView.MouseClick += new MouseEventHandler(treeView_MouseClick);
            this.Controls.Add(this.treeView);
        }

        /// <summary>
        /// 设置控件视图的配置参数。
        /// </summary>
        /// <param name="config"></param>
        public void SetViewConfig(System.Xml.XmlNode config)
        {
            ClassifySearch _Search = new ClassifySearch(this.m_Application);
            _Search.Filters.Add(new IdFilter(Function.ToInt(config.InnerText)));
            _Search.Filters.Add(new ClassifyTypeFilter(typeof(CommonCustomClassify)));
            foreach (CommonCustomClassify _Classify in _Search.Search())
            {
                this.Classify = _Classify;
                break;
            }
        }

        /// <summary>
        /// 返回当前控件视图的配置参数，以便下次打开该视图时可以通过 SetViewConfig 复原当前视图。
        /// </summary>
        /// <param name="xmlDoc"></param>
        /// <returns>如果当前控件视图无任何配置参数，则可以返回 null。</returns>
        public System.Xml.XmlNode GetViewConfig(System.Xml.XmlDocument xmlDoc)
        {
            System.Xml.XmlNode xnConfig = xmlDoc.CreateElement(this.GetType().Name);
            xnConfig.InnerText = this.Classify.ClassifyId.ToString();
            return xnConfig;
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
                if (e.Node.Tag is CommonCustomClassify)
                {
                    CommonCustomClassify _Classify = e.Node.Tag as CommonCustomClassify;
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
            foreach (CommonCustomClassify _Classify in classifys)
            {
                TreeNode tn = new TreeNode();
                tn.Text = _Classify.Name;
                tn.Tag = _Classify;
                if (_Classify.GetLevel() == 0)
                {
                    tn.ImageIndex = 0;
                    tn.SelectedImageIndex = 0;
                }
                else if (_Classify.GetLevel() == 1)
                {
                    tn.ImageIndex = 1;
                    tn.SelectedImageIndex = 1;
                }
                else
                {
                    tn.ImageIndex = 2;
                    tn.SelectedImageIndex = 2;
                }
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

                string strDeviceIconKey = device.DeviceType.Code + ((int)device.State).ToString();
                if (this.img.Images.ContainsKey(strDeviceIconKey) == false)
                {
                    this.img.Images.Add(strDeviceIconKey, device.GetIcon16());
                }
                tn.ImageKey = strDeviceIconKey;
                tn.SelectedImageKey = strDeviceIconKey;

                if (device.Children.Count > 0)
                {
                    tn.Nodes.Add("", "", 3, 3);
                }

                treeNodes.Add(tn);
            }
        }

        #endregion
    }
}
