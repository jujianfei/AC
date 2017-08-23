using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AC.Base.Forms.Windows.Plugins
{
    /// <summary>
    /// 新建设备时选择设备类型的窗体（无设备类型分类列表）。
    /// </summary>
    public partial class SelectDeviceTypeNonSortForm : Form
    {
        private WindowsFormApplicationClass m_Application;
        private bool m_DisplayChannel;
        private Device m_ParentDevice;
        private ImageList m_imgDeviceTypeSmall;
        private ImageList m_imgDeviceTypeLarge;
        private bool m_bolEnabelDeviceTypeName = true; //点击设备类型后，是否允许将设备类型名字填入设备名称文本框中

        /// <summary>
        /// 新建根节点通道或选择根节点通道新建二级设备
        /// </summary>
        /// <param name="application"></param>
        /// <param name="displayChannel">true:选择根节点通道新建二级设备；false:新建根节点通道</param>
        public SelectDeviceTypeNonSortForm(WindowsFormApplicationClass application, bool displayChannel)
        {
            InitializeComponent();

            this.m_Application = application;
            this.m_DisplayChannel = displayChannel;

            this.Init();
        }

        /// <summary>
        /// 新建指定设备的下级设备
        /// </summary>
        /// <param name="application"></param>
        /// <param name="parentDevice">指定的上级设备</param>
        public SelectDeviceTypeNonSortForm(WindowsFormApplicationClass application, Device parentDevice)
        {
            InitializeComponent();

            this.m_Application = application;
            this.m_ParentDevice = parentDevice;

            this.Init();
        }
        
        private void Init()
        {
            if (this.m_DisplayChannel)
            {
                this.panChannel.Visible = true;

                AC.Base.DeviceSearchs.DeviceSearch search = new AC.Base.DeviceSearchs.DeviceSearch(this.m_Application);
                search.Filters.Add(new AC.Base.DeviceSearchs.ParentIdFilter(0));
                foreach (Device device in search.Search())
                    this.cmbChannel.Items.Add(new DeviceInfo(device));

                if (this.cmbChannel.Items.Count > 0)
                    this.cmbChannel.SelectedIndex = 0;

                this.Text = "新建下级设备";
            }
            else
            {
                this.panChannel.Visible = false;
                this.Text = "新建通道";
            }

            if (this.m_ParentDevice != null)
            {
                this.panParentDevice.Visible = true;
                this.txtParentDevice.Text = this.m_ParentDevice.Name + " (" + this.m_ParentDevice.DeviceType.Name + ")";
            }
            else
            {
                this.panParentDevice.Visible = false;
            }

            this.m_imgDeviceTypeLarge = new ImageList();
            this.m_imgDeviceTypeSmall = new ImageList();
            this.LoadDeviceTypeIcon(this.m_Application.DeviceTypeSort);
            this.m_imgDeviceTypeLarge.ImageSize = new Size(32, 32);
            this.m_imgDeviceTypeLarge.ColorDepth = ColorDepth.Depth32Bit;
            this.lvSelectDeviceType.LargeImageList = this.m_imgDeviceTypeLarge;
            this.m_imgDeviceTypeSmall.ImageSize = new Size(16, 16);
            this.m_imgDeviceTypeSmall.ColorDepth = ColorDepth.Depth32Bit;
            this.lvSelectDeviceType.SmallImageList = this.m_imgDeviceTypeSmall;
            this.SelectedChanged();
        }

        class DeviceInfo
        {
            public DeviceInfo(Device device)
            {
                this.Device = device;
            }

            public Device Device { get; private set; }

            public override string ToString()
            {
                return this.Device.Name;
            }
        }

        //将设备图标装入ImageList
        private void LoadDeviceTypeIcon(DeviceTypeSort sort)
        {
            foreach (DeviceType deviceType in sort.DeviceTypes)
            {
                this.m_imgDeviceTypeLarge.Images.Add(deviceType.Code, deviceType.Icon32);
                this.m_imgDeviceTypeSmall.Images.Add(deviceType.Code, deviceType.Icon16);
            }

            foreach (DeviceTypeSort childrenSort in sort.Children)
            {
                this.LoadDeviceTypeIcon(childrenSort);
            }
        }

        private void cmbChannel_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.SelectedChanged();
        }

        private void SelectedChanged()
        {
            this.lvSelectDeviceType.Items.Clear();
            this.DeviceTypeInfoClear();

            DeviceTypeSort sort = this.m_Application.DeviceTypeSort;
            this.LoadDeviceType(sort);
        }

        //显示某一分类下的设备类型列表
        private void LoadDeviceType(DeviceTypeSort sort)
        {
            foreach (DeviceType deviceType in sort.DeviceTypes)
            {
                bool bolIsAdd = true;

                if (this.m_ParentDevice != null)
                {
                    //新建下级设备
                    bolIsAdd = deviceType.CanChildren(this.m_ParentDevice.DeviceType);
                }
                else
                {
                    if (this.m_DisplayChannel)
                    {
                        //新建二级设备
                        if (this.cmbChannel.SelectedItem != null && this.cmbChannel.SelectedItem is DeviceInfo)
                        {
                            bolIsAdd = deviceType.CanChildren(((DeviceInfo)this.cmbChannel.SelectedItem).Device.DeviceType);
                        }
                        else
                        {
                            bolIsAdd = false;
                        }
                    }
                    else
                    {
                        //新建通道
                        bolIsAdd = deviceType.CanRoot();
                    }
                }

                if (bolIsAdd)
                {
                    ListViewItem lvi = new ListViewItem();
                    lvi.Text = deviceType.Name;
                    lvi.Tag = deviceType;
                    lvi.ImageKey = deviceType.Code;
                    lvi.ToolTipText = deviceType.Type.FullName + "\r\n" + deviceType.Type.Assembly.Location;
                    this.lvSelectDeviceType.Items.Add(lvi);
                }
            }

            foreach (DeviceTypeSort childrenSort in sort.Children)
            {
                this.LoadDeviceType(childrenSort);
            }
        }

        private void lvSelectDeviceType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.lvSelectDeviceType.SelectedItems.Count > 0)
            {
                DeviceType deviceType = this.lvSelectDeviceType.SelectedItems[0].Tag as DeviceType;
                this.DeviceTypeInfo(deviceType);
            }
            else
            {
                this.DeviceTypeInfoClear();
            }
        }

        //显示详细的设备类型信息。
        private void DeviceTypeInfo(DeviceType deviceType)
        {
            this.picDevicePhoto.Image = deviceType.Photo;
            if (this.picDevicePhoto.Image == null)
            {
                scDeviceDescription.SplitterDistance = 0;
            }
            else
            {
                scDeviceDescription.SplitterDistance = scDeviceDescription.Height / 2;
            }
            this.labDeviceDescription.Text = deviceType.Description;
            this.btnAccept.Enabled = true;

            if (this.m_bolEnabelDeviceTypeName)
            {
                this.txtDeviceName.Text = deviceType.Name;
            }
        }

        //清除显示的设备类型信息
        private void DeviceTypeInfoClear()
        {
            this.picDevicePhoto.Image = null;
            this.labDeviceDescription.Text = "";
            this.btnAccept.Enabled = false;
        }

        private void labDeviceDescription_DoubleClick(object sender, EventArgs e)
        {
            if (this.lvSelectDeviceType.SelectedItems.Count > 0)
            {
                DeviceType deviceType = this.lvSelectDeviceType.SelectedItems[0].Tag as DeviceType;
                MessageBox.Show("设备编程代码：\r\n" + deviceType.Code + "\r\n\r\n设备编程文件：\r\n" + deviceType.Type.Assembly.Location, "设备编程信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnAccept_Click(object sender, EventArgs e)
        {
            if (this.lvSelectDeviceType.SelectedItems.Count > 0)
            {
                DeviceType deviceType = this.lvSelectDeviceType.SelectedItems[0].Tag as DeviceType;
                string strName = Function.ToTrim(txtDeviceName.Text);

                if (strName.Length > 0)
                {
                    if (MessageBox.Show("确定新建类型为“" + deviceType.Name + "”的“" + strName + "”设备吗？", "新建设备", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.OK)
                    {
                        this.DeviceType = deviceType;
                        this.DialogResult = System.Windows.Forms.DialogResult.OK;
                    }
                }
                else
                {
                    MessageBox.Show("设备名称必须输入。", "新建设备", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.txtDeviceName.Focus();
                }
            }
            else
            {
                MessageBox.Show("请选择设备类型。", "新建设备", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.lvSelectDeviceType.Focus();
            }
        }

        /// <summary>
        /// 获取选择的根节点设备(通道)
        /// </summary>
        /// <returns></returns>
        public Device GetRootDevice()
        {
            if (this.cmbChannel.SelectedItem != null)
            {
                return ((DeviceInfo)this.cmbChannel.SelectedItem).Device;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 选择的设备类型。
        /// </summary>
        public DeviceType DeviceType { get; private set; }

        /// <summary>
        /// 设备名称。
        /// </summary>
        public string DeviceName
        {
            get
            {
                return this.txtDeviceName.Text;
            }
        }

        private void txtDeviceName_KeyUp(object sender, KeyEventArgs e)
        {
            this.m_bolEnabelDeviceTypeName = this.txtDeviceName.Text.Length == 0;
        }
    }
}
