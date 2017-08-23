using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AC.Base.DeviceSearchs;

namespace AC.Base.Forms.Windows.Manages
{
    /// <summary>
    /// 分类属性设置控件。
    /// </summary>
    public partial class ClassifySetting : UserControl
    {
        /// <summary>
        /// 分类属性设置控件。
        /// </summary>
        public ClassifySetting()
        {
            InitializeComponent();
        }

        private void chkEnabledDevice_CheckedChanged(object sender, EventArgs e)
        {
            if (this.chkEnabledDevice.Checked)
            {
                this.btnDeviceFilters.Enabled = true;
            }
            else
            {
                this.btnDeviceFilters.Enabled = false;
                this.txtDeviceFilters.Text = "";
                this.txtDeviceFilters.Tag = null;
            }
        }

        private Classify m_Classify;
        /// <summary>
        /// 获取或设置分类。
        /// </summary>
        public Classify Classify
        {
            get
            {
                this.m_Classify.Name = this.txtName.Text;
                this.m_Classify.NameShortcut = this.txtNameShortcut.Text;
                this.m_Classify.Identifier = this.txtIdentifier.Text;
                this.m_Classify.OrdinalNumber = (int)this.numOrdinalNumber.Value;
                this.m_Classify.EnabledDevice = this.chkEnabledDevice.Checked;
                this.m_Classify.DeviceFilters = this.txtDeviceFilters.Tag as DeviceFilterCollection;
                return this.m_Classify;
            }
            set
            {
                this.m_Classify = value;

                this.txtName.Text = this.m_Classify.Name;
                this.txtNameShortcut.Text = this.m_Classify.NameShortcut;
                this.txtIdentifier.Text = this.m_Classify.Identifier;
                this.numOrdinalNumber.Value = this.m_Classify.OrdinalNumber;
                this.chkEnabledDevice.Checked = this.m_Classify.EnabledDevice;
                this.txtDeviceFilters.Tag = this.m_Classify.DeviceFilters;
                if (this.m_Classify.DeviceFilters == null)
                {
                    this.txtDeviceFilters.Text = "";
                }
                else
                {
                    this.txtDeviceFilters.Text = this.m_Classify.DeviceFilters.GetFilterDescription();
                }
            }
        }


        /// <summary>
        /// 获取或设置分类名称。
        /// </summary>
        public string GetClassifyName()
        {
            return this.txtName.Text;
        }

        private void btnDeviceFilters_Click(object sender, EventArgs e)
        {
            DeviceFilterForm _DeviceFilterForm = new DeviceFilterForm();
            _DeviceFilterForm.SetApplication(this.Classify.Application);
            _DeviceFilterForm.Filters = this.txtDeviceFilters.Tag as DeviceFilterCollection;
            if (_DeviceFilterForm.ShowDialog(this) == DialogResult.OK)
            {
                this.txtDeviceFilters.Tag = _DeviceFilterForm.Filters;
                this.txtDeviceFilters.Text = _DeviceFilterForm.Filters.GetFilterDescription();
            }
        }
    }
}
