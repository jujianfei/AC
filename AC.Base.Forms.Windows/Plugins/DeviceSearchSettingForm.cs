using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AC.Base.DeviceSearchs;

namespace AC.Base.Forms.Windows.Plugins
{
    /// <summary>
    /// 用于设置某一账号默认搜索条件、搜索列表及搜索排序的设置窗体。
    /// </summary>
    public partial class DeviceSearchSettingForm : Form
    {
        private FormApplicationClass m_Application;
        private IAccount m_Account;
        private bool m_IsLoad = false;                      //是否载入完毕
        private bool m_ListIsLoad = false;                  //列表项是否已载入

        /// <summary>
        /// 用于设置某一账号默认搜索条件、搜索列表及搜索排序的设置窗体。
        /// </summary>
        /// <param name="application">应用程序框架。</param>
        /// <param name="account">操作员账号。</param>
        public DeviceSearchSettingForm(FormApplicationClass application, IAccount account)
        {
            this.m_Application = application;
            this.m_Account = account;
            this.DeviceSearchConfig = this.m_Application.GetAccountConfig(this.m_Account, typeof(DeviceSearchAccountConfig)) as DeviceSearchAccountConfig;

            InitializeComponent();
        }

        /// <summary>
        /// 设备搜索列表中账户默认的搜索配置。
        /// </summary>
        public DeviceSearchAccountConfig DeviceSearchConfig { get; private set; }

        private void DeviceSearchSettingForm_Load(object sender, EventArgs e)
        {
            //筛选器
            IList<IDeviceFilter> lstFilters = this.m_Application.GetDeviceFilters(this.m_Account);

            foreach (string strTypeNames in this.DeviceSearchConfig.SearchFilterNames)
            {
                string[] strTypeName = strTypeNames.Split(new char[] { '@' });
                if (strTypeName.Length == 2)
                {
                    foreach (IDeviceFilter _Filter in lstFilters)
                    {
                        if (_Filter.GetType().FullName.Equals(strTypeName[1]))
                        {
                            ListViewItem lvi = new ListViewItem();
                            lvi.Text = strTypeName[0];
                            lvi.SubItems.Add(_Filter.GetFilterDescription());
                            lvi.Tag = strTypeNames;
                            lvi.Checked = true;
                            lvi.ToolTipText = _Filter.GetType().FullName;
                            this.lvFilter.Items.Add(lvi);
                            break;
                        }
                    }
                }
            }

            foreach (AC.Base.DeviceSearchs.IDeviceFilter _Filter in lstFilters)
            {
                string[] strNames = _Filter.FilterNamesAttribute;
                if (strNames != null && strNames.Length > 0)
                {
                    foreach (string strName in strNames)
                    {
                        string strTypeName = strName + "@" + _Filter.GetType().FullName;
                        if (this.DeviceSearchConfig.SearchFilterNames.Contains(strTypeName) == false)
                        {
                            ListViewItem lvi = new ListViewItem();
                            lvi.Text = strName;
                            lvi.SubItems.Add(_Filter.GetFilterDescription());
                            lvi.Tag = strTypeName;
                            lvi.ToolTipText = _Filter.GetType().FullName;
                            this.lvFilter.Items.Add(lvi);
                        }
                    }
                }
            }

            //搜索列表项
            IList<IDeviceListItem> lstListItems = this.m_Application.GetDeviceListItems(this.m_Account);

            foreach (string strTypeNames in this.DeviceSearchConfig.SearchListItemNames)
            {
                string[] strTypeName = strTypeNames.Split(new char[] { '@' });
                if (strTypeName.Length == 2)
                {
                    foreach (IDeviceListItem _ListItem in lstListItems)
                    {
                        if (_ListItem.GetType().FullName.Equals(strTypeName[1]))
                        {
                            ListViewItem lvi = new ListViewItem();
                            lvi.Text = strTypeName[0];
                            lvi.Tag = strTypeNames;
                            lvi.Checked = true;
                            lvi.ToolTipText = _ListItem.GetType().FullName;
                            this.lvList.Items.Add(lvi);
                            break;
                        }
                    }
                }
            }

            foreach (AC.Base.DeviceSearchs.IDeviceListItem _ListItem in lstListItems)
            {
                string[] strNames = _ListItem.GetListItemNames();
                if (strNames != null && strNames.Length > 0)
                {
                    foreach (string strName in strNames)
                    {
                        string strTypeName = strName + "@" + _ListItem.GetType().FullName;
                        if (this.DeviceSearchConfig.SearchListItemNames.Contains(strTypeName) == false)
                        {
                            ListViewItem lvi = new ListViewItem();
                            lvi.Text = strName;
                            lvi.Tag = strTypeName;
                            lvi.ToolTipText = _ListItem.GetType().FullName;
                            this.lvList.Items.Add(lvi);
                        }
                    }
                }
            }

            this.numPageSize.Value = this.DeviceSearchConfig.PageSize;

            this.chkCanChildren.Checked = this.DeviceSearchConfig.CanChildren;

            this.chkUseDefaultConfig.Checked = this.DeviceSearchConfig.UseDefaultConfig;
            m_IsLoad = true;
        }

        private void chkUseDefaultConfig_CheckedChanged(object sender, EventArgs e)
        {
            this.tacSetting.Enabled = !this.chkUseDefaultConfig.Checked;
        }

        private void btnAccept_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("确认使用该搜索配置吗？", this.Text, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.OK)
            {
                this.DeviceSearchConfig.UseDefaultConfig = this.chkUseDefaultConfig.Checked;

                this.DeviceSearchConfig.SearchFilterNames.Clear();
                foreach (ListViewItem lvi in this.lvFilter.CheckedItems)
                {
                    this.DeviceSearchConfig.SearchFilterNames.Add(lvi.Tag.ToString());
                }

                this.DeviceSearchConfig.SearchListItemNames.Clear();
                foreach (ListViewItem lvi in this.lvList.CheckedItems)
                {
                    this.DeviceSearchConfig.SearchListItemNames.Add(lvi.Tag.ToString());
                }

                this.DeviceSearchConfig.PageSize = (int)this.numPageSize.Value;

                this.DeviceSearchConfig.CanChildren = this.chkCanChildren.Checked;

                this.m_Application.SetAccountConfig(this.m_Account, this.DeviceSearchConfig);
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
            }
        }

        #region 筛选器

        private bool m_FilterIsChange;
        /// <summary>
        /// 筛选器是否已改变
        /// </summary>
        public bool FilterIsChange
        {
            get { return m_FilterIsChange; }
        }

        private void tsbFilterUp_Click(object sender, EventArgs e)
        {
            if (lvFilter.SelectedItems.Count > 0 && lvFilter.SelectedItems[0].Index > 0)
            {
                for (int intIndex = 0; intIndex < lvFilter.SelectedItems.Count; intIndex++)
                {
                    ListViewItem lvi = lvFilter.SelectedItems[intIndex];
                    int intItemIndex = lvFilter.SelectedItems[intIndex].Index;
                    lvFilter.Items.Remove(lvi);
                    lvFilter.Items.Insert(intItemIndex - 1, lvi);
                }

                lvFilter.SelectedItems[0].EnsureVisible();
                m_FilterIsChange = true;
                FilterSelectedIndexChanged();
            }
        }

        private void tsbFilterDown_Click(object sender, EventArgs e)
        {
            if (lvFilter.SelectedItems.Count > 0 && lvFilter.SelectedItems[lvFilter.SelectedItems.Count - 1].Index < lvFilter.Items.Count - 1)
            {
                for (int intIndex = lvFilter.SelectedItems.Count - 1; intIndex >= 0; intIndex--)
                {
                    ListViewItem lvi = lvFilter.SelectedItems[intIndex];
                    int intItemIndex = lvFilter.SelectedItems[intIndex].Index;
                    lvFilter.Items.Remove(lvi);
                    lvFilter.Items.Insert(intItemIndex + 1, lvi);
                }

                lvFilter.SelectedItems[lvFilter.SelectedItems.Count - 1].EnsureVisible();
                m_FilterIsChange = true;
                FilterSelectedIndexChanged();
            }
        }

        private void lvFilter_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            if (m_IsLoad)
            {
                m_FilterIsChange = true;
            }
        }

        private void lvFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterSelectedIndexChanged();
        }

        private void FilterSelectedIndexChanged()
        {
            if (lvFilter.SelectedItems.Count > 0)
            {
                if (lvFilter.SelectedItems[0].Index > 0)
                {
                    this.tsbFilterUp.Enabled = true;
                }
                else
                {
                    this.tsbFilterUp.Enabled = false;
                }

                if (lvFilter.SelectedItems[lvFilter.SelectedItems.Count - 1].Index < lvFilter.Items.Count - 1)
                {
                    this.tsbFilterDown.Enabled = true;
                }
                else
                {
                    this.tsbFilterDown.Enabled = false;
                }
            }
            else
            {
                this.tsbFilterUp.Enabled = false;
                this.tsbFilterDown.Enabled = false;
            }
        }

        private void tsbFilterSelectAll_Click(object sender, EventArgs e)
        {
            foreach (System.Windows.Forms.ListViewItem lvi in this.lvFilter.Items)
            {
                lvi.Checked = true;
            }
        }

        private void tsbFilterSelectClear_Click(object sender, EventArgs e)
        {
            foreach (System.Windows.Forms.ListViewItem lvi in this.lvFilter.Items)
            {
                lvi.Checked = false;
            }
        }

        private void tsbFilterSelectReverse_Click(object sender, EventArgs e)
        {
            foreach (System.Windows.Forms.ListViewItem lvi in this.lvFilter.Items)
            {
                lvi.Checked = !lvi.Checked;
            }
        }

        #endregion

        #region 列表项

        private bool m_ListIsChange;
        /// <summary>
        /// 列表项是否已改变
        /// </summary>
        public bool ListIsChange
        {
            get { return m_ListIsChange; }
        }

        private void tsbListUp_Click(object sender, EventArgs e)
        {
            if (lvList.SelectedItems.Count > 0 && lvList.SelectedItems[0].Index > 0)
            {
                for (int intIndex = 0; intIndex < lvList.SelectedItems.Count; intIndex++)
                {
                    ListViewItem lvi = lvList.SelectedItems[intIndex];
                    int intItemIndex = lvList.SelectedItems[intIndex].Index;
                    lvList.Items.Remove(lvi);
                    lvList.Items.Insert(intItemIndex - 1, lvi);
                }

                lvList.SelectedItems[0].EnsureVisible();
                m_ListIsChange = true;
                ListSelectedIndexChanged();
            }
        }

        private void tsbListDown_Click(object sender, EventArgs e)
        {
            if (lvList.SelectedItems.Count > 0 && lvList.SelectedItems[lvList.SelectedItems.Count - 1].Index < lvList.Items.Count - 1)
            {
                for (int intIndex = lvList.SelectedItems.Count - 1; intIndex >= 0; intIndex--)
                {
                    ListViewItem lvi = lvList.SelectedItems[intIndex];
                    int intItemIndex = lvList.SelectedItems[intIndex].Index;
                    lvList.Items.Remove(lvi);
                    lvList.Items.Insert(intItemIndex + 1, lvi);
                }

                lvList.SelectedItems[lvList.SelectedItems.Count - 1].EnsureVisible();
                m_ListIsChange = true;
                ListSelectedIndexChanged();
            }
        }

        private void lvList_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            if (m_ListIsLoad)
            {
                m_ListIsChange = true;
            }
        }

        private void lvList_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListSelectedIndexChanged();
        }

        private void ListSelectedIndexChanged()
        {
            if (lvList.SelectedItems.Count > 0)
            {
                if (lvList.SelectedItems[0].Index > 0)
                {
                    this.tsbListUp.Enabled = true;
                }
                else
                {
                    this.tsbListUp.Enabled = false;
                }

                if (lvList.SelectedItems[lvList.SelectedItems.Count - 1].Index < lvList.Items.Count - 1)
                {
                    this.tsbListDown.Enabled = true;
                }
                else
                {
                    this.tsbListDown.Enabled = false;
                }
            }
            else
            {
                this.tsbListUp.Enabled = false;
                this.tsbListDown.Enabled = false;
            }
        }

        private void tsbListSelectAll_Click(object sender, EventArgs e)
        {
            foreach (System.Windows.Forms.ListViewItem lvi in this.lvList.Items)
            {
                lvi.Checked = true;
            }
        }

        private void tsbListSelectClear_Click(object sender, EventArgs e)
        {
            foreach (System.Windows.Forms.ListViewItem lvi in this.lvList.Items)
            {
                lvi.Checked = false;
            }
        }

        private void tsbListSelectReverse_Click(object sender, EventArgs e)
        {
            foreach (System.Windows.Forms.ListViewItem lvi in this.lvList.Items)
            {
                lvi.Checked = !lvi.Checked;
            }
        }

        #endregion

        private void tacSetting_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.tacSetting.SelectedIndex == 1)
            {
                m_ListIsLoad = true;
            }
        }

        private bool m_PageSizeIsChange = false;
        /// <summary>
        /// 每页显示的记录数是否已改变。
        /// </summary>
        public bool PageSizeIsChange
        {
            get
            {
                return this.m_PageSizeIsChange;
            }
        }

        //页大小改变。
        private void numPageSize_ValueChanged(object sender, EventArgs e)
        {
            if (this.m_IsLoad)
            {
                this.m_PageSizeIsChange = true;
            }
        }

        private void chkCanChildren_CheckedChanged(object sender, EventArgs e)
        {
            this.chkCanChildrenConfirm.Enabled = this.chkCanChildren.Checked;
        }
    }
}
