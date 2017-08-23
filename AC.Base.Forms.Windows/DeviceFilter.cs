using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AC.Base.DeviceSearchs;

namespace AC.Base.Forms.Windows
{
    /// <summary>
    /// 提供筛选设备的筛选条件设置界面控件，具有基本、高级两种模式。
    /// </summary>
    public class DeviceFilter : Control
    {
        private ApplicationClass m_Application;
        private ToolStrip tsToolbar;
        private ToolStripDropDownButton tsddbAddFilter;
        private ToolStripButton tsbIsAdvanced;
        private Panel panFilterPanel;
        private DeviceFilterBasic filterBasic;
        private DeviceFilterAdvanced filterAdvanced;

        /// <summary>
        /// 提供筛选设备的筛选条件设置界面控件，具有基本、高级两种模式。
        /// </summary>
        public DeviceFilter()
        {
            this.panFilterPanel = new Panel();
            this.panFilterPanel.AutoScroll = true;
            this.panFilterPanel.BorderStyle = BorderStyle.Fixed3D;
            this.panFilterPanel.Dock = DockStyle.Fill;
            this.Controls.Add(this.panFilterPanel);

            tsToolbar = new ToolStrip();
            tsToolbar.Dock = DockStyle.Top;

            tsddbAddFilter = new ToolStripDropDownButton();
            tsddbAddFilter.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
            tsddbAddFilter.Text = "添加条件";
            tsddbAddFilter.Image = Properties.Resources.FilterAdd16;
            tsToolbar.Items.Add(tsddbAddFilter);

            ToolStripButton tsbDescription = new ToolStripButton();
            tsbDescription.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tsbDescription.Text = "显示设定的搜索条件";
            tsbDescription.Image = Properties.Resources.FilterInfo16;
            tsToolbar.Items.Add(tsbDescription);

            ToolStripButton tsbReset = new ToolStripButton();
            tsbReset.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
            tsbReset.Text = "重置";
            tsbReset.ToolTipText = "重置搜索条件";
            tsbReset.Image = Properties.Resources.Reset16;
            tsToolbar.Items.Add(tsbReset);

            tsToolbar.Items.Add(new ToolStripSeparator());

            tsbIsAdvanced = new ToolStripButton();
            tsbIsAdvanced.CheckOnClick = true;
            tsbIsAdvanced.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
            tsbIsAdvanced.Text = "基本";
            tsbIsAdvanced.Image = Properties.Resources.FilterBasic16;
            tsbIsAdvanced.Click += new EventHandler(tsbIsAdvanced_Click);
            tsToolbar.Items.Add(tsbIsAdvanced);

            this.Controls.Add(tsToolbar);
        }

        /// <summary>
        /// 设置应用程序框架。
        /// </summary>
        /// <param name="application">应用程序框架。</param>
        public void SetApplication(ApplicationClass application)
        {
            this.m_Application = application;

            foreach (AC.Base.DeviceSearchs.IDeviceFilter _Filter in this.m_Application.GetDeviceFilters(null))
            {
                string[] strNames = _Filter.FilterNamesAttribute;
                if (strNames != null && strNames.Length > 0)
                {
                    foreach (string strName in strNames)
                    {
                        ToolStripMenuItem tsmiFilter = new ToolStripMenuItem();
                        tsmiFilter.Text = strName;
                        tsmiFilter.Tag = _Filter;
                        tsmiFilter.Click += new EventHandler(tsmiFilter_Click);
                        tsddbAddFilter.DropDownItems.Add(tsmiFilter);
                    }
                }
            }

            BaseAdvancedSwitch();
        }

        /// <summary>
        /// 设置设备只读筛选器。只读筛选器通常用于强制性的添加默认搜索条件。
        /// </summary>
        /// <param name="filters"></param>
        public void SetReadonlyFilters(DeviceFilterCollection filters)
        {
            if (this.IsAdvanced)
            {
                this.IsAdvanced = false;
                this.filterBasic.SetReadonlyFilters(filters);

                this.IsAdvanced = true;
                this.filterAdvanced.SetReadonlyFilters(filters);
            }
            else
            {
                this.IsAdvanced = true;
                this.filterAdvanced.SetReadonlyFilters(filters);

                this.IsAdvanced = false;
                this.filterBasic.SetReadonlyFilters(filters);
            }
        }

        /// <summary>
        /// 获取或设置设备筛选器。
        /// </summary>
        public DeviceFilterCollection Filters
        {
            get
            {
                if (this.IsAdvanced)
                {
                    return this.filterAdvanced.Filters;
                }
                else
                {
                    return this.filterBasic.Filters;
                }
            }
            set
            {
                if (this.IsAdvanced)
                {
                    this.filterAdvanced.Filters = value;
                }
                else
                {
                    this.filterBasic.Filters = value;
                }
            }
        }

        private void tsmiFilter_Click(object sender, EventArgs e)
        {
            AC.Base.DeviceSearchs.IDeviceFilter _Filter = ((ToolStripMenuItem)sender).Tag as AC.Base.DeviceSearchs.IDeviceFilter;
            _Filter = this.m_Application.GetDeviceFilter(_Filter.GetType(), null);
            _Filter.SetFilterName(((ToolStripMenuItem)sender).Text);
            this.filterBasic.AddFilter(_Filter);
        }

        /// <summary>
        /// 获取或设置当前是否使用高级搜索条件设置界面
        /// </summary>
        public bool IsAdvanced
        {
            get
            {
                return this.tsbIsAdvanced.Checked;
            }
            set
            {
                this.tsbIsAdvanced.Checked = value;
                this.BaseAdvancedSwitch();
            }
        }

        private void tsbIsAdvanced_Click(object sender, EventArgs e)
        {
            this.BaseAdvancedSwitch();
        }

        private void BaseAdvancedSwitch()
        {
            if (this.tsbIsAdvanced.Checked)
            {
                if (this.filterBasic != null)
                {
                    this.filterBasic.Visible = false;
                }

                if (this.filterAdvanced == null)
                {
                    this.filterAdvanced = new DeviceFilterAdvanced();
                    this.filterAdvanced.Dock = DockStyle.Top;
                    this.panFilterPanel.Controls.Add(this.filterAdvanced);
                }
                this.filterAdvanced.Visible = true;

                this.tsbIsAdvanced.Text = "高级";
                this.tsbIsAdvanced.ToolTipText = "点击切换至基本设置界面";
                this.tsbIsAdvanced.Image = Properties.Resources.FilterAdvanced16;
            }
            else
            {
                if (this.filterAdvanced != null)
                {
                    this.filterAdvanced.Visible = false;
                }

                if (this.filterBasic == null)
                {
                    this.filterBasic = new DeviceFilterBasic(this.m_Application, true);
                    this.filterBasic.Dock = DockStyle.Top;
                    this.panFilterPanel.Controls.Add(this.filterBasic);
                }
                this.filterBasic.Visible = true;

                this.tsbIsAdvanced.Text = "基本";
                this.tsbIsAdvanced.ToolTipText = "点击切换至高级设置界面";
                this.tsbIsAdvanced.Image = Properties.Resources.FilterBasic16;
            }

            this.tsddbAddFilter.Enabled = !this.tsbIsAdvanced.Checked;
        }
    }
}
