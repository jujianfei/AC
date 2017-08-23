using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AC.Base.DeviceSearchs;

namespace AC.Base.Forms.Windows.DeviceSearchs
{
    /// <summary>
    /// 设备类型筛选器。
    /// </summary>
    [Control(typeof(AC.Base.DeviceSearchs.DeviceTypeFilter))]
    public class DeviceTypeFilter : Control, IDeviceFilterControl
    {
        private AC.Base.DeviceSearchs.DeviceTypeFilter m_Filter;
        private ComboBox m_Value;

        /// <summary>
        /// 设备类型筛选器。
        /// </summary>
        public DeviceTypeFilter()
        {
            this.Size = new System.Drawing.Size(0, 23);

            this.m_Value = new ComboBox();
            this.m_Value.Dock = DockStyle.Fill;
            this.m_Value.DropDownStyle = ComboBoxStyle.DropDownList;
            this.Controls.Add(this.m_Value);

            Label lab = new Label();
            lab.Text = "设备类型:";
            lab.AutoSize = false;
            lab.Dock = DockStyle.Left;
            lab.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            lab.Size = new System.Drawing.Size(80, 0);
            this.Controls.Add(lab);
        }

        #region IFilterControl<IDeviceFilter> 成员

        /// <summary>
        /// 设置应用程序框架。
        /// </summary>
        /// <param name="application">应用程序框架。</param>
        public void SetApplication(ApplicationClass application)
        {
            this.m_Value.Items.Clear();
            this.m_Value.Items.Add("");
            this.FillDeviceType(application.DeviceTypeSort);
        }

        private void FillDeviceType(DeviceTypeSort deviceTypeSort)
        {
            foreach (DeviceType deviceType in deviceTypeSort.DeviceTypes)
            {
                this.m_Value.Items.Add(deviceType);
            }

            foreach (DeviceTypeSort children in deviceTypeSort.Children)
            {
                this.FillDeviceType(children);
            }
        }

        /// <summary>
        /// 设置搜索筛选器。
        /// </summary>
        /// <param name="filter">传入需要设置的搜索筛选器对象</param>
        public void SetFilter(IDeviceFilter filter)
        {
            this.m_Filter = filter as AC.Base.DeviceSearchs.DeviceTypeFilter;

            if (this.m_Filter.DeviceType == null)
            {
                this.m_Value.SelectedIndex = 0;
            }
            else
            {
                for (int intIndex = 1; intIndex < this.m_Value.Items.Count; intIndex++)
                {
                    if (this.m_Value.Items[intIndex] is DeviceType)
                    {
                        DeviceType _DeviceType = this.m_Value.Items[intIndex] as DeviceType;
                        if (_DeviceType.Type.Equals(this.m_Filter.DeviceType))
                        {
                            this.m_Value.SelectedIndex = intIndex;
                            return;
                        }
                    }
                }

                this.m_Value.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// 获取搜索筛选器。操作员在界面上设置搜索条件后，由框架调用该方法获取设置后的搜索筛选器对象。
        /// </summary>
        /// <returns></returns>
        public IDeviceFilter GetFilter()
        {
            if (this.m_Value.SelectedItem is DeviceType)
            {
                this.m_Filter.DeviceType = ((DeviceType)this.m_Value.SelectedItem).Type;
            }
            else
            {
                this.m_Filter.DeviceType = null;
            }
            return this.m_Filter;
        }

        /// <summary>
        /// 重置筛选器。清空设置值，使筛选器界面恢复默认状态。
        /// </summary>
        public void ResetFilter()
        {
            this.m_Value.SelectedIndex = 0;
        }

        #endregion
    }
}
