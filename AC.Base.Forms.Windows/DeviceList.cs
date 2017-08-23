using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AC.Base.DeviceSearchs;
using AC.Base.Forms;

namespace AC.Base.Forms.Windows
{
    /// <summary>
    /// 设备列表界面控件(必须在用户操作前调用 SetApplication 方法设置应用程序框架)。提供显示设备的列表界面，可以定义需要显示的列，同时以树形列表的形式显示下级子设备。
    /// </summary>
    public class DeviceList : Control
    {
        private WindowsFormApplicationClass m_Application;
        private AC.Base.DeviceSearchs.DeviceSearch m_DeviceSearch;

        private bool m_IsSearching;                                             //搜索是否正在进行中
        private System.Windows.Forms.TreeListView m_tlvList;        //设备列表
        private ImageList m_imgDevice;                              //设备图标
        private List<string> m_lstImageKey;                         //根据设备代码获取设备图标索引

        /// <summary>
        /// 设备列表界面控件。
        /// </summary>
        public DeviceList()
        {
            this.m_imgDevice = new ImageList();
            this.m_imgDevice.ColorDepth = ColorDepth.Depth32Bit;
            this.m_imgDevice.ImageSize = new System.Drawing.Size(16, 16);
            this.m_imgDevice.TransparentColor = System.Drawing.Color.Transparent;
            this.m_lstImageKey = new List<string>();
            this.m_tlvList = new TreeListView();
            this.m_tlvList.BorderStyle = BorderStyle.None;
            this.m_tlvList.Dock = DockStyle.Fill;
            this.m_tlvList.FullRowSelect = true;
            this.m_tlvList.HideSelection = false;
            this.m_tlvList.Sorting = SortOrder.Ascending;
            this.m_tlvList.ShowItemToolTips = true;
            this.m_tlvList.SmallImageList = this.m_imgDevice;
            this.m_tlvList.UseXPHighlightStyle = false;
            this.m_tlvList.ColumnWidthChanged += new ColumnWidthChangedEventHandler(m_tlvList_ColumnWidthChanged);
            this.m_tlvList.SizeChanged += new EventHandler(m_tlvList_SizeChanged);
            this.m_tlvList.AfterExpand += new TreeListViewEventHandler(m_tlvList_AfterExpand);
            this.m_tlvList.SelectedIndexChanged += new EventHandler(m_tlvList_SelectedIndexChanged);
            this.m_tlvList.DoubleClick += new EventHandler(m_tlvList_DoubleClick);
            this.Controls.Add(this.m_tlvList);
  
            this.m_tlvList.Columns.Add("设备",260);
            this.m_tlvList.Columns.Add("状态", 60);
        }

        /// <summary>
        /// 设置应用程序框架。
        /// </summary>
        /// <param name="application">应用程序框架。</param>
        public void SetApplication(WindowsFormApplicationClass application)
        {
            this.m_Application = application;
        }

        private void m_tlvList_ColumnWidthChanged(object sender, ColumnWidthChangedEventArgs e)
        {
        }

        private void m_tlvList_SizeChanged(object sender, EventArgs e)
        {
            if (this.m_tlvList.HeaderStyle == ColumnHeaderStyle.None && this.m_tlvList.Columns.Count == 1)
            {
                this.m_tlvList.Columns[0].Width = this.m_tlvList.Width - 20;
            }
        }

        void m_tlvList_AfterExpand(object sender, TreeListViewEventArgs e)
        {
            if (e.Item.Tag is Device)
            {
                Device device = e.Item.Tag as Device;
                if (e.Item.Items.Count == 1 && e.Item.Items[0].Tag == null && device.Children.Count > 0)
                {
                    e.Item.Items.Clear();

                    foreach (Device deviceChildren in device.Children)
                    {
                        TreeListViewItem tlvi = new TreeListViewItem();
                        FillDeviceListItem(tlvi, deviceChildren);
                        e.Item.Items.Add(tlvi);
                    }
                }
            }
        }

        private void m_tlvList_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.DoMenu();
        }

        private void m_tlvList_DoubleClick(object sender, EventArgs e)
        {

        }

        private void DoMenu()
        {
            if (this.m_tlvList.SelectedItems.Count > 0)
            {
                int intDeviceNum = 0;
                foreach (TreeListViewItem tlvi in this.m_tlvList.SelectedItems)
                {
                    if (tlvi.Tag != null && tlvi.Tag is Device)
                        intDeviceNum++;
                }

                if (intDeviceNum > 0)
                {
                    Device[] devices = new Device[intDeviceNum];
                    intDeviceNum = 0;
                    foreach (TreeListViewItem tlvi in this.m_tlvList.SelectedItems)
                    {
                        if (tlvi.Tag != null && tlvi.Tag is Device)
                        {
                            devices[intDeviceNum] = tlvi.Tag as Device;
                            intDeviceNum++;
                        }
                    }

                    this.m_tlvList.ContextMenuStrip = this.m_Application.GetDeviceMenu(devices);
                }
                else
                    this.m_tlvList.ContextMenuStrip = null;
            }
            else
                this.m_tlvList.ContextMenuStrip = null;
        }

        private DeviceListItemInfoCollection m_ListItemInfos;
        /// <summary>
        /// 搜索结果列表中显示信息的集合。
        /// </summary>
        public DeviceListItemInfoCollection ListItemInfos
        {
            get { return this.m_ListItemInfos; }
            set
            {
                this.m_ListItemInfos = value;

                if (this.m_ListItemInfos == null)
                {
                    this.m_tlvList.HeaderStyle = ColumnHeaderStyle.None;

                    if (this.m_tlvList.Columns.Count == 0)
                    {
                        ColumnHeader clh = new ColumnHeader();
                        clh.Width = 160;
                        this.m_tlvList.Columns.Add(clh);
                    }
                    else if (this.m_tlvList.Columns.Count > 1)
                    {
                        while (this.m_tlvList.Columns.Count == 1)
                        {
                            this.m_tlvList.Columns.RemoveAt(1);
                        }
                    }

                    this.m_tlvList_SizeChanged(null, null);
                }
                else
                {
                    this.m_tlvList.HeaderStyle = ColumnHeaderStyle.Clickable;
                    this.m_tlvList.Columns.Clear();

                    foreach (Searchs.ListItemInfo<AC.Base.DeviceSearchs.IDeviceListItem> listItemInfo in this.m_ListItemInfos)
                    {
                        ColumnHeader clh = new ColumnHeader();
                        clh.Text = listItemInfo.Name;
                        this.m_tlvList.Columns.Add(clh);
                    }
                }

                if (this.m_DeviceSearch != null)
                {
                    this.DeviceSearch(this.m_DeviceSearch.PageNum);
                }
            }
        }

        /// <summary>
        /// 设置设备搜索器。设置后将显示第1页的设备。
        /// </summary>
        /// <param name="deviceSearch"></param>
        public void SetDeviceSearch(AC.Base.DeviceSearchs.DeviceSearch deviceSearch)
        {
            this.m_DeviceSearch = deviceSearch;
        }

        /// <summary>
        /// 显示指定页数的设备列表。
        /// </summary>
        /// <param name="pageNum">页数</param>
        public void DeviceSearch(int pageNum)
        {
            if (this.m_IsSearching == false && this.m_DeviceSearch != null)
            {
                this.m_IsSearching = true;

                this.m_tlvList.Items.Clear();

                DeviceCollection devices = this.m_DeviceSearch.Search(pageNum);
                foreach (Device device in devices)
                {
                    TreeListViewItem tlvi = new TreeListViewItem();
                    FillDeviceListItem(tlvi, device);
                    this.m_tlvList.Items.Add(tlvi);
                }
                devices.KeepSource = false;

                this.m_IsSearching = false;
            }
        }

        private void FillDeviceListItem(TreeListViewItem tlvi, Device device)
        {
            tlvi.ImageIndex = this.GetDeviceImageIndex(device);
            tlvi.Tag = device;
            if (this.ListItemInfos == null || this.ListItemInfos.Count == 0)
            {
                tlvi.Text = device.Name;
                tlvi.SubItems.Add(device.State.GetDescription());
            }
            else
            {
                for (int intIndex = 0; intIndex < this.ListItemInfos.Count; intIndex++)
                {
                    if (intIndex == 0)
                    {
                        tlvi.Text = this.ListItemInfos[intIndex].GetListItemValue(device);
                    }
                    else
                    {
                        tlvi.SubItems.Add(this.ListItemInfos[intIndex].GetListItemValue(device));
                    }
                }
            }

            if (this.m_tlvList.ShowPlusMinus)
            {
                if (device.Children.Count > 0)
                {
                    tlvi.Items.Add("");
                }
            }
        }

        /// <summary>
        /// 获取 TreeListViewItem 所使用图标的索引。
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        private int GetDeviceImageIndex(Device device)
        {
            string strDeviceIconKey = device.DeviceType.Code + ((int)device.State).ToString();
            if (this.m_lstImageKey.Contains(strDeviceIconKey) == false)
            {
                this.m_imgDevice.Images.Add(device.GetIcon16());
                this.m_lstImageKey.Add(strDeviceIconKey);
            }

            return this.m_lstImageKey.IndexOf(strDeviceIconKey);
        }

        /// <summary>
        /// 是否显示子节点。
        /// </summary>
        public bool CanChildren
        {
            get
            {
                return this.m_tlvList.ShowPlusMinus;
            }
            set
            {
                if (this.m_tlvList.ShowPlusMinus != value)
                {
                    this.m_tlvList.ShowPlusMinus = value;
                }
            }
        }

        /// <summary>
        /// 获取或设置每页显示的数据数量。当此属性设置为“0”时，则表示不进行分页，将所有符合条件的数据全部读取出。
        /// </summary>
        public int PageSize
        {
            get
            {
                return this.m_DeviceSearch.PageSize;
            }
            set
            {
                this.m_DeviceSearch.PageSize = value;
            }
        }

        /// <summary>
        /// 获取当前是第几页的数据。
        /// </summary>
        public int PageNum
        {
            get
            {
                return this.m_DeviceSearch.PageNum;
            }
        }

        /// <summary>
        /// 获取此搜索结果数据的总页数。
        /// </summary>
        public int PageCount
        {
            get
            {
                return this.m_DeviceSearch.PageCount;
            }
        }

        /// <summary>
        /// 获取此搜索结果数据的总数据量。
        /// </summary>
        public int RecordsetCount
        {
            get
            {
                return this.m_DeviceSearch.RecordsetCount;
            }
        }

        /// <summary>
        /// 当前页记录集数据在整个搜索结果中的开始索引，该值从0开始。
        /// </summary>
        public int RecordsetStartIndex
        {
            get
            {
                return this.m_DeviceSearch.RecordsetStartIndex;
            }
        }

        /// <summary>
        /// 当前页记录集数据在整个搜索结果中的结束索引，该值从0开始。
        /// </summary>
        public int RecordsetEndIndex
        {
            get
            {
                return this.m_DeviceSearch.RecordsetEndIndex;
            }
        }
    }
}
