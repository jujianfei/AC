using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AC.Base.DeviceSearchs;

namespace AC.Base.Forms.Windows
{
    /// <summary>
    /// 基本设备搜索条件界面。
    /// </summary>
    public class DeviceFilterBasic : TableLayoutPanel
    {
        private ApplicationClass m_Application;
        private IAccount m_Account;

        /// <summary>
        /// 基本设备搜索条件界面（仅加载未使用账号的设备筛选器）。
        /// </summary>
        /// <param name="application">应用程序框架。</param>
        /// <param name="enabledRemove">是否在每个筛选器后显示一个移除按钮，以便可以动态更改筛选条件。</param>
        public DeviceFilterBasic(ApplicationClass application, bool enabledRemove)
        {
            this.m_Application = application;
            this.EnabledRemove = enabledRemove;

            this.Init();
        }

        /// <summary>
        /// 基本设备搜索条件界面。
        /// </summary>
        /// <param name="application">应用程序框架。</param>
        /// <param name="account">当前账号，用于需要使用帐号的筛选器控件。</param>
        /// <param name="enabledRemove">是否在每个筛选器后显示一个移除按钮，以便可以动态更改筛选条件。</param>
        public DeviceFilterBasic(ApplicationClass application, IAccount account, bool enabledRemove)
        {
            this.m_Application = application;
            this.m_Account = account;
            this.EnabledRemove = enabledRemove;

            this.Init();
        }

        private void Init()
        {
            this.AutoSize = true;

            if (this.EnabledRemove)
            {
                this.ColumnCount = 2;
                this.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
                this.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            }
            else
            {
                this.ColumnCount = 1;
                this.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            }
        }

        /// <summary>
        /// 是否在每个筛选器后显示一个移除按钮，以便可以动态更改筛选条件。
        /// </summary>
        public bool EnabledRemove { get; private set; }

        /// <summary>
        /// 添加设备筛选条件。
        /// </summary>
        /// <param name="filter"></param>
        public void AddFilter(IDeviceFilter filter)
        {
            this.AddFilter(filter, false);
        }

        private void AddFilter(IDeviceFilter filter, bool isReadonly)
        {
            this.RowCount++;
            this.RowStyles.Add(new RowStyle());

            Type typControl = this.m_Application.GetControlType(filter.GetType(), typeof(System.Windows.Forms.Control));
            if (typControl == null)
            {
                Label lab = new Label();
                lab.Text = "未发现 " + filter.GetType().FullName + " 的筛选器控件";
                lab.AutoEllipsis = true;
                lab.Padding = new System.Windows.Forms.Padding(3);
                lab.Dock = DockStyle.Bottom;
                this.Controls.Add(lab);

                if (this.EnabledRemove)
                {
                    System.Windows.Forms.Button btnFilterRemove = new Button();
                    btnFilterRemove.Image = Properties.Resources.DeleteBlack;
                    btnFilterRemove.Width = 24;
                    btnFilterRemove.FlatStyle = FlatStyle.Flat;
                    btnFilterRemove.FlatAppearance.BorderSize = 0;
                    btnFilterRemove.Dock = DockStyle.Right;
                    btnFilterRemove.Click += new EventHandler(btnFilterRemove_Click);
                    this.Controls.Add(btnFilterRemove);
                }
            }
            else
            {
                System.Reflection.ConstructorInfo ciFilter = typControl.GetConstructor(new System.Type[] { });
                object objFilterInstance = ciFilter.Invoke(new object[] { });

                IDeviceFilterControl _FilterControl = objFilterInstance as IDeviceFilterControl;
                _FilterControl.SetApplication(this.m_Application);

                if (objFilterInstance is IUseAccount)
                {
                    IUseAccount useAccount = objFilterInstance as IUseAccount;
                    useAccount.SetAccount(this.m_Account);
                }

                _FilterControl.SetFilter(filter);

                Control ctl = objFilterInstance as Control;
                ctl.Dock = DockStyle.Bottom;
                if (isReadonly)
                {
                    ctl.Enabled = false;
                }
                this.Controls.Add(ctl);

                if (this.EnabledRemove)
                {
                    if (isReadonly)
                    {
                        Control ctlButton = new Control();
                        this.Controls.Add(ctlButton);
                    }
                    else
                    {
                        System.Windows.Forms.Button btnFilterRemove = new Button();
                        btnFilterRemove.Image = Properties.Resources.DeleteBlack;
                        btnFilterRemove.Width = 24;
                        btnFilterRemove.FlatStyle = FlatStyle.Flat;
                        btnFilterRemove.FlatAppearance.BorderSize = 0;
                        btnFilterRemove.Dock = DockStyle.Right;
                        btnFilterRemove.Click += new EventHandler(btnFilterRemove_Click);
                        this.Controls.Add(btnFilterRemove);
                    }
                }
            }
        }

        private void btnFilterRemove_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Button btn = (System.Windows.Forms.Button)sender;
            if (btn.Parent is System.Windows.Forms.TableLayoutPanel)
            {
                int intRowIndex = this.GetPositionFromControl(btn).Row;

                this.Controls.RemoveAt(intRowIndex * 2);
                this.Controls.RemoveAt(intRowIndex * 2);
            }
        }

        private DeviceFilterCollection m_ReadonlyFilters;
        /// <summary>
        /// 设置设备只读筛选器。只读筛选器通常用于强制性的添加默认搜索条件。
        /// </summary>
        /// <param name="filters"></param>
        public void SetReadonlyFilters(DeviceFilterCollection filters)
        {
            this.m_ReadonlyFilters = filters;

            foreach (IDeviceFilter _Filter in filters)
            {
                this.AddFilter(_Filter, true);
            }
        }

        /// <summary>
        /// 获取或设置设备筛选器。
        /// </summary>
        public DeviceFilterCollection Filters
        {
            get
            {
                int intStartIndex = 0;
                if (this.m_ReadonlyFilters != null && this.m_ReadonlyFilters.Count > 0)
                {
                    if (this.EnabledRemove)
                    {
                        intStartIndex = this.m_ReadonlyFilters.Count * 2;
                    }
                    else
                    {
                        intStartIndex = this.m_ReadonlyFilters.Count;
                    }
                }

                DeviceFilterCollection _Filters = new DeviceFilterCollection();
                _Filters.SetApplication(this.m_Application);

                for (int intIndex = intStartIndex; intIndex < this.Controls.Count; intIndex++)
                {
                    if (this.Controls[intIndex] is IDeviceFilterControl)
                    {
                        IDeviceFilterControl ctlFilter = this.Controls[intIndex] as IDeviceFilterControl;
                        _Filters.Add(ctlFilter.GetFilter());
                    }
                }

                return _Filters;
            }
            set
            {
                int intStartIndex = 0;
                if (this.m_ReadonlyFilters != null)
                {
                    if (this.EnabledRemove)
                    {
                        intStartIndex = this.m_ReadonlyFilters.Count * 2;
                    }
                    else
                    {
                        intStartIndex = this.m_ReadonlyFilters.Count;
                    }
                }

                while (this.Controls.Count > intStartIndex)
                {
                    if (this.Controls[intStartIndex] is IDeviceFilterControl)
                    {
                        this.Controls[intStartIndex].Dispose();
                    }
                    else
                    {
                        this.Controls.RemoveAt(intStartIndex);
                    }
                }

                this.RowCount = (this.m_ReadonlyFilters != null ? this.m_ReadonlyFilters.Count : 0);

                if (value != null)
                {
                    foreach (IDeviceFilter _Filter in value)
                    {
                        this.AddFilter(_Filter);
                    }
                }
            }
        }
    }
}
