using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AC.Base.DeviceSearchs;

namespace AC.Base.Forms.Windows.Plugins
{
    /// <summary>
    /// 显示搜索设备的视图界面。
    /// </summary>
    public class DeviceSearchControlView : UserControl, IControlView, IUseAccount
    {
        private WindowsFormApplicationClass m_Application;
        private IAccount m_Account;
        private AC.Base.DeviceSearchs.DeviceSearchAccountConfig m_SearchConfig;
        private bool m_IsSearching;                                             //搜索是否正在进行中
        private ToolStrip tsToolbar;
        private Panel panFilterPanel;
        private DeviceFilterBasic filterBasic;
        private DeviceFilterAdvanced filterAdvanced;
        private Control ctlList;
        private DeviceList deviceList;
        private Label labListInfo;
        private ToolStripButton tsbSearch;
        private ToolStripButton tsbBack;
        private ToolStripButton tsbPageUp;
        private ToolStripButton tsbPageDown;
        private ToolStripComboBox tscPageGoto;
        private ToolStripButton tsbReset;
        private ToolStripButton tsbIsAdvanced;
        private ToolStripButton tsbSetting;

        /// <summary>
        /// 显示搜索设备的视图界面。
        /// </summary>
        public DeviceSearchControlView()
        {
            base.Load += new EventHandler(DeviceSearchControlView_Load);

            this.panFilterPanel = new Panel();
            this.panFilterPanel.AutoScroll = true;
            this.panFilterPanel.BorderStyle = BorderStyle.None;
            this.panFilterPanel.Dock = DockStyle.Fill;
           
            this.Controls.Add(this.panFilterPanel);

            this.ctlList = new Control();
            this.ctlList.Dock = DockStyle.Fill;
            this.Controls.Add(ctlList);

            this.deviceList = new DeviceList();
            this.deviceList.Dock = DockStyle.Fill;
            this.ctlList.Controls.Add(this.deviceList);

            this.labListInfo = new Label();
            this.labListInfo.AutoEllipsis = true;
            this.labListInfo.AutoSize = false;
            this.labListInfo.Dock = DockStyle.Bottom;
            this.labListInfo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.ctlList.Controls.Add(this.labListInfo);

            this.tsToolbar = new ToolStrip();
            this.Controls.Add(this.tsToolbar);

            this.tsbSearch = new ToolStripButton();
            this.tsbSearch.Text = "开始搜索";
            this.tsbSearch.Image = Properties.Resources.Search16;
            this.tsbSearch.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
            this.tsbSearch.Click += new EventHandler(tsbSearch_Click);
            this.tsToolbar.Items.Add(this.tsbSearch);

            this.tsbBack = new ToolStripButton();
            this.tsbBack.Text = "返回";
            this.tsbBack.Image = Properties.Resources.BrowseBack16;
            this.tsbBack.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
            this.tsbBack.Click += new EventHandler(tsbBack_Click);
            this.tsToolbar.Items.Add(this.tsbBack);

            this.tsToolbar.Items.Add(new ToolStripSeparator());

            this.tsbPageUp = new ToolStripButton();
            this.tsbPageUp.Text = "上页";
            this.tsbPageUp.Image = Properties.Resources.PageUp16;
            this.tsbPageUp.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
            this.tsbPageUp.Click += new EventHandler(tsbPageUp_Click);
            this.tsToolbar.Items.Add(this.tsbPageUp);

            this.tsbPageDown = new ToolStripButton();
            this.tsbPageDown.Text = "下页";
            this.tsbPageDown.Image = Properties.Resources.PageDown16;
            this.tsbPageDown.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
            this.tsbPageDown.Click += new EventHandler(tsbPageDown_Click);
            this.tsToolbar.Items.Add(this.tsbPageDown);

            this.tscPageGoto = new ToolStripComboBox();
            this.tscPageGoto.Size = new System.Drawing.Size(75, 25);
            this.tscPageGoto.ToolTipText = "输入页码后请按回车键";
            this.tscPageGoto.KeyPress += new KeyPressEventHandler(tscPageGoto_KeyPress);
            this.tscPageGoto.SelectedIndexChanged += new EventHandler(tscPageGoto_SelectedIndexChanged);
            this.tsToolbar.Items.Add(this.tscPageGoto);

            this.tsbReset = new ToolStripButton();
            this.tsbReset.Text = "重置搜索条件";
            this.tsbReset.Image = Properties.Resources.Reset16;
            this.tsbReset.DisplayStyle = ToolStripItemDisplayStyle.Image;
            this.tsbReset.Click += new EventHandler(tsbReset_Click);
            this.tsToolbar.Items.Add(this.tsbReset);

            this.tsbIsAdvanced = new ToolStripButton();
            this.tsbIsAdvanced.Enabled = false;
            this.tsbIsAdvanced.Text = "基本";
            this.tsbIsAdvanced.ToolTipText = "点击切换至高级设置界面";
            this.tsbIsAdvanced.CheckOnClick = true;
            this.tsbIsAdvanced.Image = Properties.Resources.FilterBasic16;
            this.tsbIsAdvanced.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
            this.tsbIsAdvanced.Click += new EventHandler(tsbIsAdvanced_Click);
            this.tsToolbar.Items.Add(this.tsbIsAdvanced);

            this.tsToolbar.Items.Add(new ToolStripSeparator());

            this.tsbSetting = new ToolStripButton();
            this.tsbSetting.Text = "设置";
            this.tsbSetting.Image = Properties.Resources.Setting16;
            this.tsbSetting.DisplayStyle = ToolStripItemDisplayStyle.Image;
            this.tsbSetting.Click += new EventHandler(tsbSetting_Click);
            //this.tsbSetting.Enabled = false;
            this.tsToolbar.Items.Add(this.tsbSetting);
        }

        private void DeviceSearchControlView_Load(object sender, EventArgs e)
        {
            this.m_SearchConfig = this.m_Application.GetAccountConfig(this.m_Account, typeof(AC.Base.DeviceSearchs.DeviceSearchAccountConfig)) as AC.Base.DeviceSearchs.DeviceSearchAccountConfig;

            this.IsFilterView = true;
            this.IsAdvanced = false;

            this.DeviceListCanChildrenChange();
            this.DeviceListItemInfoChange();
        }

        private bool m_IsFilterView;
        /// <summary>
        /// 获取或设置当前是否是条件设置界面。true：搜索条件设置界面；false：设备列表界面。
        /// </summary>
        public bool IsFilterView
        {
            get
            {
                return this.m_IsFilterView;
            }
            set
            {
                if (this.m_IsFilterView != value)
                {
                    if (value)
                    {
                        this.ctlList.Visible = false;
                        this.panFilterPanel.Visible = true;

                        this.tsbSearch.Visible = true;
                        this.tsbBack.Visible = false;
                        this.tsbPageUp.Visible = false;
                        this.tsbPageDown.Visible = false;
                        this.tscPageGoto.Visible = false;
                        this.tsbReset.Visible = true;
                        this.tsbIsAdvanced.Visible = true;
                    }
                    else
                    {
                        this.panFilterPanel.Visible = false;
                        this.ctlList.Visible = true;

                        this.tsbSearch.Visible = false;
                        this.tsbBack.Visible = true;
                        this.tsbPageUp.Visible = true;
                        this.tsbPageDown.Visible = true;
                        this.tscPageGoto.Visible = true;
                        this.tsbReset.Visible = false;
                        this.tsbIsAdvanced.Visible = false;
                    }
                    this.m_IsFilterView = value;
                }
            }
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
                    this.filterBasic = new DeviceFilterBasic(this.m_Application, this.m_Account, false);
                    this.filterBasic.Dock = DockStyle.Top;
                    this.panFilterPanel.Controls.Add(this.filterBasic);
                    this.FilterBasicInit();
                }
                this.filterBasic.Visible = true;

                this.tsbIsAdvanced.Text = "基本";
                this.tsbIsAdvanced.ToolTipText = "点击切换至高级设置界面";
                this.tsbIsAdvanced.Image = Properties.Resources.FilterBasic16;
            }
        }

        //初始化基本设备搜索条件界面。
        private void FilterBasicInit()
        {
            if (this.filterBasic != null)
            {
                DeviceFilterCollection _Filters = new DeviceFilterCollection();
                if (m_SearchConfig.UseDefaultConfig)
                {
                    _Filters.Add(new AC.Base.DeviceSearchs.AddressFilter());
                    _Filters.Add(new AC.Base.DeviceSearchs.NameFilter());
                    _Filters.Add(new AC.Base.DeviceSearchs.DeviceTypeFilter());
                }
                else
                {
                    foreach (string strSearchFilterName in m_SearchConfig.SearchFilterNames)
                    {
                        string[] strFilterName = strSearchFilterName.Split(new char[] { '@' });
                        if (strFilterName.Length == 2)
                        {
                            Type typ = this.m_Application.GetDeviceFilterType(strFilterName[1]);
                            if (typ != null)
                            {
                                IDeviceFilter _Filter = this.m_Application.GetDeviceFilter(typ, this.m_Account);
                                _Filter.SetFilterName(strFilterName[0]);
                                _Filters.Add(_Filter);
                            }
                        }
                    }
                }

                this.filterBasic.Filters = _Filters;
            }
        }

        private void tsbSearch_Click(object sender, EventArgs e)
        {
            DeviceFilterCollection _Filters = null;
            if (IsAdvanced)
            {
            }
            else if (this.filterBasic != null)
            {
                _Filters = this.filterBasic.Filters;
            }

            this.IsFilterView = false;

            AC.Base.DeviceSearchs.DeviceSearch search = new AC.Base.DeviceSearchs.DeviceSearch(this.m_Application);
            search.Filters.Add(_Filters);
            search.PageSize = this.GetPageSize();

            deviceList.SetDeviceSearch(search);
            this.PageGoto(this.deviceList.PageNum + 1);
        }

        private int GetPageSize()
        {
            if (this.m_SearchConfig.UseDefaultConfig)
            {
                return 50;
            }
            else
            {
                return this.m_SearchConfig.PageSize;
            }
        }

        private void tsbBack_Click(object sender, EventArgs e)
        {
            this.IsFilterView = true;
        }

        //转到指定页面
        private void PageGoto(int pageNum)
        {
            if (this.m_IsSearching == false)
            {
                this.m_IsSearching = true;
                this.tsbPageUp.Enabled = false;
                this.tsbPageDown.Enabled = false;
                this.tscPageGoto.Enabled = false;

                this.deviceList.DeviceSearch(pageNum);
                this.PageChanged();

                this.m_IsSearching = false;
            }
        }

        private void PageChanged()
        {
            this.labListInfo.Text = "第" + this.deviceList.PageNum + "页 共" + this.deviceList.PageCount + "页 计" + this.deviceList.RecordsetCount + "条 第" + (this.deviceList.RecordsetStartIndex + 1) + "-" + (this.deviceList.RecordsetEndIndex + 1) + "条 ";

            if (this.deviceList.PageCount == 0 || this.deviceList.PageNum <= 1)
            {
                this.tsbPageUp.Enabled = false;
                this.tsbPageUp.ToolTipText = "";
            }
            else
            {
                this.tsbPageUp.Enabled = true;
                this.tsbPageUp.ToolTipText = "当前第 " + this.deviceList.PageNum + "页,翻到第 " + (this.deviceList.PageNum - 1) + " 页";
            }

            if (this.deviceList.PageCount == 0 || this.deviceList.PageNum >= this.deviceList.PageCount)
            {
                this.tsbPageDown.Enabled = false;
                this.tsbPageDown.ToolTipText = "";
            }
            else
            {
                this.tsbPageDown.Enabled = true;
                this.tsbPageDown.ToolTipText = "当前第 " + this.deviceList.PageNum + "页,翻到第 " + (this.deviceList.PageNum + 1) + " 页";
            }

            if (this.deviceList.PageCount == 0)
            {
                this.tscPageGoto.Enabled = false;
            }
            else
            {
                this.tscPageGoto.Enabled = true;

                if (this.tscPageGoto.Items.Count != this.deviceList.PageCount)
                {
                    //该控件的下拉菜单内容肯定是从1开始递增的
                    //移除多余的选项
                    int intPageGotoControlItemsCount = this.tscPageGoto.Items.Count;
                    for (int intIndex = this.deviceList.PageCount; intIndex < intPageGotoControlItemsCount; intIndex++)
                    {
                        this.tscPageGoto.Items.RemoveAt(this.tscPageGoto.Items.Count - 1);
                    }

                    //增补不够的选项
                    for (int intIndex = this.tscPageGoto.Items.Count + 1; intIndex <= this.deviceList.PageCount; intIndex++)
                    {
                        this.tscPageGoto.Items.Add(intIndex.ToString());
                    }
                }

                if (this.tscPageGoto.SelectedIndex != this.deviceList.PageNum - 1)
                {
                    this.tscPageGoto.SelectedIndex = this.deviceList.PageNum - 1;
                }
            }
        }

        private void tsbPageUp_Click(object sender, EventArgs e)
        {
            this.PageGoto(this.deviceList.PageNum - 1);
        }

        private void tsbPageDown_Click(object sender, EventArgs e)
        {
            this.PageGoto(this.deviceList.PageNum + 1);
        }

        private void tscPageGoto_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                this.PageGoto(Function.ToInt(this.tscPageGoto.Text));
            }
        }

        private void tscPageGoto_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.PageGoto(Function.ToInt(this.tscPageGoto.Text));
        }

        private void DeviceListItemInfoChange()
        {
            if (this.m_SearchConfig.UseDefaultConfig || this.m_SearchConfig.SearchListItemNames == null || this.m_SearchConfig.SearchListItemNames.Count == 0)
            {
                this.deviceList.ListItemInfos = null;
            }
            else
            {
                DeviceListItemInfoCollection _ListItemInfos = new DeviceListItemInfoCollection();
                IList<IDeviceListItem> lstListItems = this.m_Application.GetDeviceListItems(this.m_Account);

                foreach (string strTypeNames in this.m_SearchConfig.SearchListItemNames)
                {
                    string[] strTypeName = strTypeNames.Split(new char[] { '@' });
                    if (strTypeName.Length == 2)
                    {
                        foreach (IDeviceListItem _ListItem in lstListItems)
                        {
                            if (_ListItem.GetType().FullName.Equals(strTypeName[1]))
                            {
                                _ListItemInfos.Add(_ListItem, strTypeName[0]);
                                break;
                            }
                        }
                    }
                }

                this.deviceList.ListItemInfos = _ListItemInfos;
            }
        }

        private void DeviceListCanChildrenChange()
        {
            if (this.m_SearchConfig.UseDefaultConfig)
            {
                this.deviceList.CanChildren = false;
            }
            else
            {
                this.deviceList.CanChildren = this.m_SearchConfig.CanChildren;
            }
        }

        private void PageSizeChange()
        {
        }

        private void tsbReset_Click(object sender, EventArgs e)
        {
        }

        private void tsbIsAdvanced_Click(object sender, EventArgs e)
        {
            this.BaseAdvancedSwitch();
        }

        private void tsbSetting_Click(object sender, EventArgs e)
        {
            DeviceSearchSettingForm _DeviceSearchSettingForm = new DeviceSearchSettingForm(this.m_Application, this.m_Account);
            _DeviceSearchSettingForm.StartPosition = FormStartPosition.CenterParent;
            if (_DeviceSearchSettingForm.ShowDialog(this) == DialogResult.OK)
            {
                bool bolDefaultChange = this.m_SearchConfig.UseDefaultConfig;

                this.m_SearchConfig = this.m_Application.GetAccountConfig(this.m_Account, typeof(AC.Base.DeviceSearchs.DeviceSearchAccountConfig)) as AC.Base.DeviceSearchs.DeviceSearchAccountConfig;
                if (this.m_SearchConfig.UseDefaultConfig == bolDefaultChange)
                {
                    bolDefaultChange = false;
                }
                else
                {
                    bolDefaultChange = true;
                }

                if (this.filterBasic != null && (_DeviceSearchSettingForm.FilterIsChange || bolDefaultChange))
                {
                    this.FilterBasicInit();
                }

                this.DeviceListCanChildrenChange();

                if (bolDefaultChange || _DeviceSearchSettingForm.ListIsChange || _DeviceSearchSettingForm.PageSizeIsChange)
                {
                    if (this.IsFilterView == false && (bolDefaultChange || _DeviceSearchSettingForm.PageSizeIsChange))
                    {
                        this.deviceList.PageSize = this.GetPageSize();

                        if (_DeviceSearchSettingForm.ListIsChange == false)
                        {
                            PageGoto(this.deviceList.PageNum);
                        }
                    }

                    if (_DeviceSearchSettingForm.ListIsChange)
                    {
                        this.DeviceListItemInfoChange();

                        if (bolDefaultChange || _DeviceSearchSettingForm.PageSizeIsChange)
                        {
                            this.PageChanged();
                        }
                    }
                }

            }
        }

        private void BackFilter()
        {
        }

        private void Search()
        {
        }

        #region IUseAccount 成员

        /// <summary>
        /// 设置当前操作员账号。
        /// </summary>
        /// <param name="account"></param>
        public void SetAccount(IAccount account)
        {
            this.m_Account = account;
        }

        #endregion

        #region IControlView 成员

        /// <summary>
        /// 返回当前控件视图的配置参数，以便下次打开该视图时可以通过 SetViewConfig 复原当前视图。
        /// </summary>
        /// <param name="xmlDoc"></param>
        /// <returns>如果当前控件视图无任何配置参数，则可以返回 null。</returns>
        public System.Xml.XmlNode GetViewConfig(System.Xml.XmlDocument xmlDoc)
        {
            return null;
        }

        /// <summary>
        /// 设置控件视图的配置参数。
        /// </summary>
        /// <param name="config"></param>
        public void SetViewConfig(System.Xml.XmlNode config)
        {
        }

        /// <summary>
        /// 设置应用程序框架。
        /// </summary>
        /// <param name="application"></param>
        public void SetApplication(WindowsFormApplicationClass application)
        {
            this.m_Application = application;
            this.deviceList.SetApplication(this.m_Application);
        }

        #endregion

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // DeviceSearchControlView
            // 
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Name = "DeviceSearchControlView";
            this.ResumeLayout(false);

        }
    }
}
